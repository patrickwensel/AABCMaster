

var gFormID = "form-default";

var ctlStatus = null;
var ctlFirstName = null;
var ctlLastName = null;
var ctlDismissalReasonType = null;
var ctlFollowup = null;
var ctlFollowupDate = null;
var ctlDateOfBirth = null;
var ctlEmail = null;
var ctlCardholderDOB = null;



function initComponent() {
    ActivePage.Initialize();
}


// ActivePage
var ActivePage = {



    Form: {


        Validate: function () {

            /// <summary>Validates all controls and returns true or false accordingly</summary>
            
            ret = true;

            if (!ActivePage.Controls.DismissalReasonType.Validate()) { ret = false; }
            if (!ActivePage.Controls.FirstName.Validate()) { ret = false; }
            if (!ActivePage.Controls.FollowupDate.Validate()) { ret = false; }
            if (!ActivePage.Controls.LastName.Validate()) { ret = false; }
            if (!ActivePage.Controls.Status.Validate()) { ret = false; }
            if (!ActivePage.Controls.DateOfBirth.Validate()) { ret = false; }

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

            /// <summary>Used to initialize the client state for initial page load. 
            /// Handles things like enabled/disabled controls, viewed/hidden areas 
            /// based on data, etc.</summary>

            if (ctlStatus.GetValue() == 'Dismissed') {
                ActivePage.ClientState.DismissalArea.SetVisibility(true);
            } else {
                ActivePage.ClientState.DismissalArea.SetVisibility(false);
            }
            
            // force validation of all controls for client-side proactive reporting
            ActivePage.Form.Validate();

        },


        DismissalArea: {


            SetVisibility: function (visible, transitionTime) {

                if (transitionTime == undefined) { transitionTime = 200; }

                if (visible) {
                    $("#referral-dismissal-info").show(transitionTime);
                } else {
                    $("#referral-dismissal-info").hide(transitionTime);
                }
                
            },


            Validate: function () {
                ActivePage.Controls.DismissalReasonType.Validate();
                ActivePage.Controls.FollowupDate.Validate();
            },


            ClearValues: function () {
                /// <summary>Sets all dismissal-related values to null (Type, Reason and Notes)</summary>
                App.DevEx.GetControl("Detail.DismissalReasonType.ID").SetValue(null);
                App.DevEx.GetControl("Detail.DismissalReason").SetValue(null);
                App.DevEx.GetControl("Detail.DismissalReasonNotes").SetValue(null);
            }


        }   // END DismissalArea


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

            ctlStatus = App.DevEx.GetControl("Detail.Status");
            ctlFirstName = App.DevEx.GetControl("Detail.FirstName");
            ctlLastName = App.DevEx.GetControl("Detail.LastName");
            ctlDismissalReasonType = App.DevEx.GetControl("Detail.DismissalReasonType.ID");
            ctlFollowup = App.DevEx.GetControl("Detail.Followup");
            ctlFollowupDate = App.DevEx.GetControl("Detail.FollowupDate");
            ctlDateOfBirth = App.DevEx.GetControl("Detail.DateOfBirth");
            ctlEmail = App.DevEx.GetControl("Detail.ContactInfo.Email");
            ctlCardholderDOB = App.DevEx.GetControl("Detail.InsuranceInformation.PrimaryCardholderDateOfBirth");

            ctlFirstName.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.FirstName.AfterUpdate(); });
            ctlLastName.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.LastName.AfterUpdate(); });
            ctlStatus.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.Status.AfterUpdate(); });
            ctlDismissalReasonType.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.DismissalReasonType.AfterUpdate(); });
            ctlFollowup.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.Followup.AfterUpdate(); });
            ctlFollowupDate.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.FollowupDate.AfterUpdate(); });
            ctlDateOfBirth.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.DateOfBirth.AfterUpdate(); });
            ctlEmail.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.Email.AfterUpdate(); });
            ctlCardholderDOB.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.CardholderDOB.AfterUpdate(); });
            
        },




        CardholderDOB: {

            AfterUpdate: function () {
                ActivePage.Controls.CardholderDOB.Validate();
            },
            Validate: function () {

                var ret = true;
                var v = ctlCardholderDOB.GetValue();
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                if (v != null) {
                    if (Common.Dates.Compare(v, new Date()) == 1) {
                        $("#CardholderDOBError").show(t);
                    } else {
                        $("#CardholderDOBError").hide(t);
                    }
                }
                return ret;

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
                    $("#EmailErrorBadFormat").hide(t);
                    ret = true;
                } else {
                    $("#EmailErrorBadFormat").show(t);
                    ret = false;
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
                var v = ctlDateOfBirth.GetValue();
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                if (v != null) {
                    if (Common.Dates.Compare(v, new Date()) == 1) {
                        $("#DateOfBirthError").show(t);
                        ret = false;
                    } else {
                        $("#DateOfBirthError").hide(t);
                    }
                }
                return ret;
            }

        },


        Followup: {


            AfterUpdate: function() {
                
                if (ctlFollowup.GetValue() == false) {
                    ctlFollowupDate.SetValue(null);
                } 

                ActivePage.Controls.FollowupDate.Validate();    
            }


        },


        FollowupDate: {


            AfterUpdate: function () {

                if (ctlFollowupDate.GetValue() != null) {
                    ctlFollowup.SetChecked(true);
                }

                ActivePage.Controls.FollowupDate.Validate();

            },


            Validate: function() {

                var ret = true;

                if (ctlFollowup.GetValue() == true) {

                    var v = ctlFollowupDate.GetValue();
                    var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                    // date required if followup specified
                    if (v == null && ctlFollowup.GetValue() == true) {
                        $("#FollowupDateErrorRequired").show(t);
                        ret = false;
                    } else {
                        $("#FollowupDateErrorRequired").hide(t);
                    }
                    
                } else {
                    $("#FollowupDateErrorPastDate").hide(t);
                    $("#FollowupDateErrorRequired").hide(t);
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
                    $("#FirstNameError").show(t);
                    ret = false;
                } else {
                    $("#FirstNameError").hide(t);
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
                    $("#LastNameError").show(t);
                    ret = false;
                } else {
                    $("#LastNameError").hide(t);
                }

                return ret;

            }


        },


        Status: {


            AfterUpdate: function () {

                if (ctlStatus.GetValue() == 'Dismissed') {
                    ActivePage.ClientState.DismissalArea.SetVisibility(true);
                    ActivePage.ClientState.DismissalArea.Validate();
                } else {
                    ActivePage.ClientState.DismissalArea.SetVisibility(false);
                    ActivePage.ClientState.DismissalArea.ClearValues();
                }
                
            },


            Validate: function () {

                var ret = true;
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                if (ctlStatus.GetValue() == null) {
                    $("#StatusError").show(t);
                    ret = false;
                } else {
                    $("#StatusError").hide(t);
                }

                return ret;

            }


        },



        DismissalReasonType: {


            AfterUpdate: function () {
                ActivePage.Controls.DismissalReasonType.Validate();
            },


            Validate: function () {

                var ret = true;
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                if (ctlStatus.GetValue() == 'Dismissed') {
                    if (ctlDismissalReasonType.GetValue() == null) {
                        $("#DismissalReasonTypeError").show(t);
                        ret = false;
                    } else {
                        $("#DismissalReasonTypeError").hide(t);
                    }
                }
                if (ret) {
                    $("#DismissalReasonTypeError").hide(t);
                }
                return ret;
            }


        }

    }  // END Controls


}   // END ActivePage










