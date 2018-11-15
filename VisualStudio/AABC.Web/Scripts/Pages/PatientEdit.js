
var gFormID = "form-default";

var ctlFirstName = null;
var ctlLastName = null;
var ctlDateOfBirth = null;
var ctlEmail = null;

function initComponent() {
    ActivePage.Initialize();
}

function prescriptionDownload(id){
    var win = window.open("/Patients/GetPrescription?id=" + id, "_blank");
    win.focus();
}

// ActivePage
var ActivePage = {



    Form: {


        Validate: function () {

            /// <summary>Validates all controls and returns true or false accordingly</summary>
            
            ret = true;

            if (!ActivePage.Controls.FirstName.Validate()) { ret = false; }
            if (!ActivePage.Controls.LastName.Validate()) { ret = false; }
            if (!ActivePage.Controls.DateOfBirth.Validate()) { ret = false;}
            if (!ActivePage.Controls.Email.Validate()) { ret = false; }

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


        }//,



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

            ctlFirstName = App.DevEx.GetControl("FirstName");
            ctlLastName = App.DevEx.GetControl("LastName");
            ctlDateOfBirth = App.DevEx.GetControl("DateofBirth");
            ctlEmail = App.DevEx.GetControl("Email");

            ctlFirstName.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.FirstName.AfterUpdate(); });
            ctlLastName.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.LastName.AfterUpdate(); });
            ctlDateOfBirth.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.DateOfBirth.AfterUpdate(); });
            ctlEmail.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.Email.AfterUpdate(); });

            btnCaseManager.Click.AddHandler(function (s, e) { ActivePage.Controls.CaseManager.Click(); });
            
        },


        CaseManager: {
            Click: function () {

                $.ajax({
                    type: 'GET',
                    url: '/Case/' + $("#ID").val() + "/Manage/Summary",
                    success: function (response) {
                        $("#" + App.Content.ContentElementID).empty().append(response);
                    },
                    error: function () {
                        App.Dialogs.Error();
                    }

                });

            }
        },


        Email: {


            AfterUpdate: function() {
                ActivePage.Controls.Email.Validate();
            },

            Validate: function() {

                var ret = true;
                var v = ctlEmail.GetValue();
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;
                
                if (v == null || Common.Email.IsValid(v)) {
                    $("#ErrEmailBadFormat").hide(t);
                    ret = true;
                } else {
                    $("#ErrEmailBadFormat").show(t);
                    ret = false;
                }

                return ret;
            }

        },



        FirstName: {


            AfterUpdate: function () {
                ActivePage.Controls.FirstName.Validate();
            },


            Validate: function () {

                var ret = true;
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                if (ctlFirstName.GetValue() == null) {
                    $("#ErrFirstNameRequired").show(t);
                    ret = false;
                } else {
                    $("#ErrFirstNameRequired").hide(t);
                }
                return ret;

            }


        },


        LastName: {


            AfterUpdate: function () {
                ActivePage.Controls.LastName.Validate();
            },


            Validate: function () {

                var ret = true;
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                if (ctlLastName.GetValue() == null) {
                    $("#ErrLastNameRequired").show(t);
                    ret = false;
                } else {
                    $("#ErrLastNameRequired").hide(t);
                }

                return ret;

            }

        },

        DateOfBirth: {

            AfterUpdate: function () {
                ActivePage.Controls.DateOfBirth.Validate();
            },

            Validate: function () {

                var ret = true;
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;
                if (ctlDateOfBirth.GetValue() == null) {
                    $("#ErrDOBRequired").show(t);
                    ret = false;
                } else {
                    $("#ErrDOBRequired").hide(t);
                }

                return ret;
            }

        }

    },  // END Controls



    zzDummy: null




}   // END ActivePage







