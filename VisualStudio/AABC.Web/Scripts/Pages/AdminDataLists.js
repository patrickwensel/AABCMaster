


function gvDataListsAuthsFocusedRowChanged(s, e) {
    var id = App.DevEx.GridView.GetFocusedRowKey(gvDataListsAuths);
    loadAuthsDetail(id);
}

function getCurrentAuthSelection() {
    return App.DevEx.GridView.GetFocusedRowKey(gvDataListsAuths);
}

function loadAuthsGrid() {
    $.ajax({
        url: '/DataLists/AuthsGrid',
        type: 'GET',
        success: function (r) {
            $("#DataListsAuthGridContainer").empty().append(r);
        },
        error: function (r) {
            App.Dialogs.Error('Sorry, we ecountered an error loading the Authorizations information.  Please contact your administrator if this issue persists.');
        }
    });
}

function loadAuthsDetail(authID) {

    var url = '/DataLists/AuthDetail';
    //if (authID != null) {
    //    url = url + '/' + authID;
    //}

    $.ajax({
        url: url,
        type: 'GET',
        data: { authID: authID },
        success: function(r) {
            $("#DataListsAuthDetailContainer").empty().append(r);
        },
        error: function () {
            App.Dialogs.Error();
        }
    });

}

function authDetailEditSubmit(s, e) {

    var id = $('input[name="AuthDetailEditID"]').val();
    var code = AuthDetailEditCode.GetValue();
    var desc = AuthDetailEditDescription.GetValue();

    if (id == null || code == null || desc == null) {
        App.Dialogs.Error("Code and Description are both required.");
        return;
    }
    
    $.ajax({
        url: "/DataLists/AuthDetailEdit",
        type: 'POST',
        data: {
            authID: id,
            authCode: code,
            authDescription: desc
        },
        success: function (r) {
            loadAuthsGrid();
        },
        error: function () {
            App.Dialogs.Error();
        }

    });

}

function authDetailCreateSubmit(s, e) {

    var code = AuthDetailCreateCode.GetValue();
    var desc = AuthDetailCreateDescription.GetValue();

    if (code == null || desc == null) {
        App.Dialogs.Error("Code and Description are both required.");
        return;
    }

    $.ajax({
        url: "/DataLists/AuthDetailCreate",
        type: 'POST',
        data: {
            authCode: code,
            authDescription: desc
        },
        success: function (r) {
            loadAuthsGrid();
        },
        error: function () {
            App.Dialogs.Error();
        }
    });

}

function authDetailDeleteSubmit(s, e) {

    var id = getCurrentAuthSelection();
    if (id != null) {

        if (confirm("Are you sure you want to delete this Authorization?\r\nIf this authorization is associated with any cases the deletion will fail.")) {
            $.ajax({
                url: "/DataLists/AuthDetailDelete",
                type: 'POST',
                data: {
                    id: id
                },
                success: function (r) {
                    if (r == "dependency") {
                        App.Dialogs.MessageBox(
                            "This authorization could not be deleted due to dependencies.  Please remove all applied cases and insurances and try again.",
                            App.Dialogs.Icons.Warning,
                            "Unable to Delete");
                    } else {
                        loadAuthsGrid();
                    }
                },
                error: function () {
                    App.Dialogs.Error();
                }
            });
        }

        //App.Dialogs.ConfirmDelete({
        //    message: "Are you sure you want to delete this Authorization?\r\n\r\nIf this authorization is associated with any cases the deletion will fail.",
        //    confirmed: function () {

        //        $.ajax({
        //            url: "/DataLists/AuthDetailDelete",
        //            type: 'POST',
        //            data: {
        //                id: id
        //            },
        //            success: function (r) {
        //                if (r == "dependency") {
        //                    App.Dialogs.MessageBox(
        //                        "This authorization could not be deleted due to dependencies.  Please remove all applied cases and insurances and try again.",
        //                        App.Dialogs.Icons.Warning,
        //                        "Unable to Delete");
        //                } else {
        //                    loadAuthsGrid();
        //                }
        //            },
        //            error: function () {
        //                App.Dialogs.Error();
        //            }
        //        });
        //    }
        //});


    }

}







function initDataListsScripts() {
    loadAuthsGrid();
    loadAuthsDetail(null);
}


