
var gFormID = "form-default";
var ctlFirstName = null;
var ctlLastName = null;
var ctlEmail = null;

initLinks();

function initLinks() {
    $('#lnkRegenOptions').on('click', function () {
        regenOptions();
        return false;
    });

    $('#lnkRegenPermissions').on('click', function () {
        regenPermissions();
        return false;
    });
}


function regenPermissions() {
    var userID = getSelectedUserID();
    $.ajax({
        type: 'POST',
        url: '/Admin/RegenUserPermissions',
        data: { userID: userID },
        success: function () {
            userList.Refresh();
        }
    });
}

function regenOptions() {
    var userID = getSelectedUserID();
    $.ajax({
        type: 'POST',
        url: '/Admin/RegenUserOptions',
        data: { userID: userID },
        success: function () {
            userList.Refresh();
        }
    });
}


function initComponent() {
	ActivePage.Initialize();
}

// ActivePage
var ActivePage = {

	Form: {

		Validate: function () {

			ret = true;

			return ret;

		},

		SubmitHandler: function (event) {

			event = event || window.event || event.srcElement;

			if (ActivePage.Form.Validate()) {
				return true;
			} else {
				event.preventDefault();
				App.Dialogs.ValidationFailure("Unable to save.  Please validate all fields.");
				return false;
			}
		}
	},

	ClientState: {

		Initialize: function () {

			ActivePage.Form.Validate();

		}
	}, 

	Initialize() {

		$("#" + gFormID).on('submit', function (event) {
			return ActivePage.Form.SubmitHandler(event);
		});

		App.DataPage.InitSectionExpanders();
		ActivePage.Controls.Initialize();
		ActivePage.ClientState.Initialize();

	}, 

	Controls: {

		Initialize: function () {

		},

		Email: {

			AfterUpdate: function () {
				ActivePage.Controls.Email.Validate();
			},

			Validate: function () {

				var ret = true;
				
				return ret;
			}
		},

		FirstName: {

			AfterUpdate: function () {
				ActivePage.Controls.FirstName.Validate();
			},

			Validate: function () {

				var ret = true;

				return ret;

			}
		},

		LastName: {

			AfterUpdate: function () {
				ActivePage.Controls.LastName.Validate();
			},

			Validate: function () {

				var ret = true;
				
				return ret;

			}
		}
	},  

	zzDummy: null

}  


function bnSaveUser(s, e) {

	$.ajax ({
		type: 'POST',
		url: '/Admin/UserFormSave',
		data: $("#form-default").serialize(),
		success: function (response) {
		    $("#UserFormContainer").html(response);
		    refreshUserGrid();
		},
		error: function(r) {
			App.Dialogs.Error(r);
		}
	});
}
function bnNewUser(s, e) {

    $.ajax({
        type: 'GET',
        url: '/Admin/UserForm',
        data: { Id: 0 },
        success: function (response) {
            $("#UserFormContainer").html(response);
        },
        error: function (r) {
            App.Dialogs.Error(r);
        }
    });
}
function gvUsersFocusedRowChanged(s, e) {
    App.LoadingPanel.AutoPanel.SupressSingle();
	s.GetRowValues(s.GetFocusedRowIndex(), "ID", OnGetRowValues);
}


function OnGetRowValues(values) {

    App.LoadingPanel.AutoPanel.SupressSingle();
	$.ajax ({
		type: 'GET',
		url: '/Admin/UserPermissionGridCallback',
		data: { userId: values },
		success: function (response) {
			if (response.substr(0, 4) == "ERR:") {
				App.Dialogs.Error(response.substr(4));
			} else {
				$("#permissions-grid-container").empty().append(response);
			}
		},
		error: function(r) {
			App.Dialogs.Error(r);
		}
	});

	App.LoadingPanel.AutoPanel.SupressSingle();
	$.ajax ({
		type: 'GET',
		url: '/Admin/UserOptionGridCallback',
		data: { userId: values },
		success: function (response) {
			if (response.substr(0, 4) == "ERR:") {
				App.Dialogs.Error(response.substr(4));
			} else {
				$("#options-grid-container").empty().append(response);
			}
		},
		error: function(r) {
			App.Dialogs.Error(r);
		}
	});
	App.LoadingPanel.AutoPanel.SupressSingle();
	$.ajax({
	    type: 'POST',
	    url: '/Admin/UserForm',
	    data: { Id: values },
	    success: function (response) {
	        $("#UserFormContainer").html(response);
	        //users.getGrid();
	    },
	    error: function (r) {
	        App.Dialogs.Error(r);
	    }
	});
}

function DialogConfirmClosed() {
	if (DialogConfirmResult == 'button1') {

		$.ajax({
			type: 'POST',
			url: '/Admin/Delete',
			data: { id: activeModelID },
			success: function (response) {
				$("#" + App.Content.ContentElementID).empty().append(response);
				App.Dialogs.MessageBox("User deleted successfully.", App.Dialogs.Icons.Ok, "Success");
			},
			error: function (response) {
				App.Dialogs.Error("We're sorry, we ran into a problem with that request.");
			}
		});
	}
}

function actionMenuItemClick(s, e) {

	var id = s.name.replace('muActions', '');
	var action = e.item.name;

	switch (action) {

		case "PWReset":

			$.ajax({
				type: 'POST',
				url: '/Admin/ResetPassword',
				data: { userID: id },
				success: function (response) {
				    if (response != "err") {
				        App.Dialogs.MessageBox("New password: " + response);
				    } else {
				        App.Dialogs.Error();
				    }				    
				},
				error: function (response) {
					App.Dialogs.Error();
				}
			});

			break;

		case "Delete":

			var msg = "Are you sure you want to delete this user?";

			$.ajax({
				type: 'GET',
				url: '/Dialogs/ConfirmDelete',
				data: { contentElementID: App.Content.ContentElementID, message: msg },
				success: function (r) {
					$("#DialogAnchor").html(r);
					activeModelID = id;
					DialogConfirm.Show();
				},
				error: function() {
					App.Dialogs.Error();
				}
			});

			break;
		default:
			App.Dialogs.Error("Action not recognized. Please contact your administator.");
	}
}

function OnCellClick(visibleIndex, fieldName) {

	switch (fieldName) {
		case "isAllowed":
			toggleIsAllowed(visibleIndex);
			break;
		default:
			return;
	}

	function toggleIsAllowed(visibleIndex, fieldName) {
        
	    var ctl = App.DevEx.GetControl("permissionsGrid");

	    App.LoadingPanel.AutoPanel.SupressSingle();

		ctl.GetRowValues(visibleIndex, "isAllowed", function(value) {
                
			var permissionID = ctl.GetRowKey(visibleIndex);
			var isAllowed = value;

			if (isAllowed == false) {
				isAllowed = true;
			} else {
				isAllowed = false;
			}

			App.LoadingPanel.AutoPanel.SupressSingle();
			$.ajax({
				type: "POST",
				url: "/Admin/UserPermissionIsAllowedUpdate",
				data: {
					permissionId: permissionID,
					isAllowed: isAllowed,
				},
				success: function(r) {
					App.LoadingPanel.AutoPanel.SupressSingle();
					ctl.Refresh();
				},
				error: function() {
					App.Dialogs.Error();
				}
			})
		});   
	}
}

function permissionsGridBeginCallback(s, e) {
    e.customArgs["userId"] = getSelectedUserID();
}

function getSelectedUserID() {
    return userList.GetRowKey(userList.GetFocusedRowIndex());
}

function refreshUserGrid() {

    $.ajax({
        type: 'GET',
        url: '/Admin/UserGrid',
        success: function (response) {
            $("#UserGridContainer").html(response);
            //users.getGrid();
        },
        error: function (r) {
            App.Dialogs.Error(r);
        }
    });

    
}
