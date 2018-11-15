
var gFormID = "form-default";
var ctlFirstName = null;
var ctlLastName = null;
var ctlEmail = null;

function initComponent(push, route, title, navGroup, navItem) {

    ActivePage.Initialize();
}

// ActivePage
var ActivePage = {

    Form: {

        Validate: function () {

            /// <summary>Validates all controls and returns true or false accordingly</summary>

            ret = true;

            return ret;

        },

        SubmitHandler: function (event) {

            /// <summary>Handles for submission, cancels or proceeds depending on validation</summary>

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

            // force validation of all controls for client-side proactive reporting
            ActivePage.Form.Validate();

        }

    },  // END ClientState

    Initialize() {

        $("#" + gFormID).on('submit', function (event) {
            return ActivePage.Form.SubmitHandler(event);
        });

        App.DataPage.InitSectionExpanders();
        ActivePage.Controls.Initialize();
        ActivePage.ClientState.Initialize();

    },  // END Initialize

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
    },  // END Controls

    zzDummy: null

};   // END ActivePage



function lnkToggleAppAccessClick(elem, event) {
    var action = elem.name.replace("hlToggleAppAccess_", "");
    var id = action.substr(action.indexOf("_") + 1);
    action = action.substr(0, action.indexOf("_"));
    event.preventDefault();
    if (action === "Grant") {
        $.ajax({
            type: 'POST',
            url: '/ProviderPortal/GrantAppAccess',
            data: {
                providerID: id
            },
            success: function (response) {
                gridProviderUserList.Refresh();
            },
            error: function () {
                App.Dialogs.Error();
            }
        });
    } else if (action === "Revoke") {
        $.ajax({
            type: 'POST',
            url: '/ProviderPortal/RevokeAppAccess',
            data: {
                providerID: id
            },
            success: function (response) {
                gridProviderUserList.Refresh();
            },
            error: function () {
                App.Dialogs.Error();
            }
        });
    }
}

function lnkRegisterClick(elem, event) {
    // elem.name gives the hyperlink's name, which has the action and id stuffed into it...
    var action = elem.name.replace("hlReg_", "");
    var id = action.substr(action.indexOf("_") + 1);
    event.preventDefault();
    action = action.substr(0, action.indexOf("_"));
    if (action === "Unregister") {
        // ConfirmDelete not working, see here: http://stackoverflow.com/questions/36919471/ajax-success-or-error-not-firing-sometimes
        //App.Dialogs.ConfirmDelete({
        //    message: "Are you sure you want to unregister this Provider?",
        //    confirmed: function () {
        // workaround for ConfirmDelete
        var b = confirm("Are you sure you want to unregister this provider?");
        if (b === true) {
            $.ajax({
                url: '/ProviderPortal/UnregisterProvider',
                data: { providerID: id },
                type: 'POST',
                success: function (r) {
                    App.Dialogs.MessageBox("Successfully Unregistered", App.Dialogs.Icons.Ok);
                    gridProviderUserList.Refresh();
                },
                error: function () {
                    App.Dialogs.Error();
                }
            });
        }
        //    },
        //    cancelled: function () {
        //        App.Dialogs.MessageBox("Cancelled", App.Dialogs.Icons.Information, "Operation Cancelled", false);
        //    }
        //});
    } else {
        // register
        App.Popup.Show({
            url: "/ProviderPortal/RegisterPopup",
            data: {
                providerID: id,
                registerAction: "register"
            },
            options: {
                width: 300,
                height: 200,
                title: "Register Provider Portal User"
            },
            finished: function (r) {
                if (r === "ok") {
                    App.Dialogs.MessageBox("Registration successful", App.Dialogs.Icons.Ok, "Success", false);
                    gridProviderUserList.Refresh();
                } else {
                    if (r === "cancelled") {
                        // do nothing
                    } else {
                        // not cancelled, not ok, must be an error
                        App.Dialogs.Error(r);
                    }
                }
            },
            error: function (r) {
                App.Dialogs.Error();
            }
        });
    }
    //return false;
}

function lnkResetPasswordClick(elem, event) {
    event.preventDefault();
    App.Dialogs.MessageBox("We're sorry, this feature isn't implemented yet.  \n\nPlease Unregister and re-register for the same affect.", App.Dialogs.Icons.Information);
    return false;
}
