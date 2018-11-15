
var gFormID = "form-default";

var ctlFirstName = null;
var ctlLastName = null;
var ctlEmail = null;



function initComponent(push, route, title, navGroup, navItem) {

    if (push == 'True') {
        initView(route, title, null, navGroup, navItem);
    } else {
        initView(null, null, null, navGroup, navItem);
    }

    ActivePage.Initialize();

}


// ActivePage
var ActivePage = {



    Form: {


        Validate: function () {

            /// <summary>Validates all controls and returns true or false accordingly</summary>

            ret = true;

            //if (!ActivePage.Controls.FirstName.Validate()) { ret = false; }
            //if (!ActivePage.Controls.LastName.Validate()) { ret = false; }
            //if (!ActivePage.Controls.Email.Validate()) { ret = false; }

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

            //if (ctlStatus.GetValue() == 'Dismissed') {
            //    ActivePage.ClientState.DismissalArea.SetVisibility(true);
            //} else {
            //    ActivePage.ClientState.DismissalArea.SetVisibility(false);
            //}

            // force validation of all controls for client-side proactive reporting
            ActivePage.Form.Validate();

        }//,


        //DismissalArea: {


        //    SetVisibility: function (visible, transitionTime) {

        //        if (transitionTime == undefined) { transitionTime = 200; }

        //        if (visible) {
        //            $("#referral-dismissal-info").show(transitionTime);
        //        } else {
        //            $("#referral-dismissal-info").hide(transitionTime);
        //        }

        //    },


        //    Validate: function () {
        //        ActivePage.Controls.DismissalReasonType.Validate();
        //        ActivePage.Controls.FollowupDate.Validate();
        //    },


        //    ClearValues: function () {
        //        /// <summary>Sets all dismissal-related values to null (Type, Reason and Notes)</summary>
        //        App.DevEx.GetControl("Detail.DismissalReasonType.ID").SetValue(null);
        //        App.DevEx.GetControl("Detail.DismissalReason").SetValue(null);
        //        App.DevEx.GetControl("Detail.DismissalReasonNotes").SetValue(null);
        //    }


        //}   // END DismissalArea


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

            //ctlFirstName = App.DevEx.GetControl("FirstName");
            //ctlLastName = App.DevEx.GetControl("LastName");
            //ctlEmail = App.DevEx.GetControl("Email");

            //ctlFirstName.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.FirstName.AfterUpdate(); });
            //ctlLastName.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.LastName.AfterUpdate(); });
            //ctlEmail.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.Email.AfterUpdate(); });

        },





        Email: {


            AfterUpdate: function () {
                ActivePage.Controls.Email.Validate();
            },

            Validate: function () {

                var ret = true;
                //var v = ctlEmail.GetValue();
                //var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                //if (v == null || Common.Email.IsValid(v)) {
                //    $("#ErrEmailBadFormat").hide(t);
                //    ret = true;
                //} else {
                //    $("#ErrEmailBadFormat").show(t);
                //    ret = false;
                //}

                return ret;
            }

        },



        FirstName: {


            AfterUpdate: function () {
                ActivePage.Controls.FirstName.Validate();
            },


            Validate: function () {

                var ret = true;
                //var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                //if (ctlFirstName.GetValue() == null) {
                //    $("#ErrFirstNameRequired").show(t);
                //    ret = false;
                //} else {
                //    $("#ErrFirstNameRequired").hide(t);
                //}
                return ret;

            }


        },


        LastName: {


            AfterUpdate: function () {
                ActivePage.Controls.LastName.Validate();
            },


            Validate: function () {

                var ret = true;
                //var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                //if (ctlLastName.GetValue() == null) {
                //    $("#ErrLastNameRequired").show(t);
                //    ret = false;
                //} else {
                //    $("#ErrLastNameRequired").hide(t);
                //}

                return ret;

            }

        }

    },  // END Controls



    zzDummy: null




}   // END ActivePage







