
var gFormID = "form-default";

var ctlCode = null;
var ctlName = null;


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

            if (!ActivePage.Controls.Code.Validate()) { ret = false; }
            if (!ActivePage.Controls.Name.Validate()) { ret = false; }

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

            ctlCode = App.DevEx.GetControl("Code");
            ctlName = App.DevEx.GetControl("Name");

            ctlCode.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.Code.AfterUpdate(); });
            ctlName.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.Name.AfterUpdate(); });

        },

        Code: {

            AfterUpdate: function() {
                ActivePage.Controls.Code.Validate();
            },

            Validate: function () {

                var ret = true;
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                if (ctlCode.GetValue() == null) {
                    $("#CodeError").show(t);
                    ret = false;
                } else {
                    $("#CodeError").hide(t);
                }
                return ret;

            }

        },

        Name: {

            AfterUpdate: function() {
                ActivePage.Controls.Name.Validate();
            },

            Validate: function() {
                var ret = true;
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                if (ctlName.GetValue() == null) {
                    $("#NameError").show(t);
                    ret = false;
                } else {
                    $("#NameError").hide(t);
                }
                return ret;
            }

        }






    },  // END Controls



    zzDummy: null




}   // END ActivePage










