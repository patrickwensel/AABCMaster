
var gFormID = "form-default";

var ctlStatus = null;
var ctlStatusReason = null;

var ctlHasIntake = null;
var ctlHasPrescription = null;
var ctlEndingAuthDate = null;




function initComponent() {
    ActivePage.Initialize();
}


// ActivePage
var ActivePage = {


    Status: {

        ToggleCFD: function() {

            var status = ActivePage.Status.CurrentStatus();
            if (status == -1 || status == "History") {
                // history, do nothing
                return;
            }

            if (status == 2 || status == "ConsiderForDischarge") {
                // currently CFD, toggle to standard
                ActivePage.Status.RevokeCFDState();
            } else {
                ActivePage.Status.SetCFDState();
            }

            ActivePage.ClientState.StatusIndicators.SetIndicators();

        },

        SetCFDState: function () {
            $("#Status").val(2);
            ActivePage.Status.RemoveUIWarnings();
            App.DevEx.GetControl("btnSetStatusCFD").SetText("Unset CFD");
            $("input[name=StatusDisplay]").val("Consider for Discharge");
        },

        RevokeCFDState: function() {
            $("#Status").val(0);
            ActivePage.Status.Calculate();
            App.DevEx.GetControl("btnSetStatusCFD").SetText("Set as CFD");
        },

        CurrentStatus: function() {
            return $("#Status").val();
        },

        CurrentStatusReason: function() {
            return $("#StatusReason").val();
        },

        RemoveUIWarnings: function() {

        },

        SetUIWarnings: function() {

        },

        Calculate: function() {

            var status = ActivePage.Status.CurrentStatus();
            var reason = ActivePage.Status.CurrentStatusReason();

            // History or CFD, return without doing anything
            if (status == -1 || status == 2 || status == "History" || status == "ConsiderForDischarge") {
                ActivePage.ClientState.StatusIndicators.SetIndicators();
                return;
            }

            // check the overall state

            var hasSuper = App.DevEx.DisabledControls.GetCheckboxValue("HasSupervisor");
            var hasIntake = ctlHasIntake.GetValue();
            var hasRx = ctlHasPrescription.GetValue();

            getAuthValid(function(authValid) {

                if (!hasSuper || !hasIntake || !hasRx || !authValid) {

                    // TODO: status not good
                    var statusText = "";
                    reason = 0;

                    if (!hasSuper) {
                        statusText += ",BCBA";
                        reason += 1;
                    }
                    if (!hasIntake) {
                        statusText += ",Int";
                        reason += 4;
                    }
                    if (!hasRx) {
                        statusText += ",Rx";
                        reason += 2;
                    }
                    if (!authValid) {
                        statusText += ",Auth";
                        reason += 8;
                    }

                    if (statusText.length > 0) {
                        statusText = statusText.substring(1);
                        statusText = "NG: " + statusText;
                    } else {
                        statusText = "NG: Unknown";
                    }

                    status = 0; // not good
                    $("input[name=StatusDisplay]").val(statusText);
                    $("#Status").val(status);
                    $("#StatusReason").val(reason);

                } else {

                    status = 1;     // good
                    reason = 0;     // none

                    $("input[name=StatusDisplay]").val("Good");
                    $("#Status").val(status);
                    $("#StatusReason").val(reason);
                }

                ActivePage.ClientState.StatusIndicators.SetIndicators(status);
            });
            

            function getAuthValid(callback) {
                
                $.ajax({
                    url: "/Case/AuthValidCheck",
                    data: { caseID: $("#ID").val() },
                    success: function (response) {

                        if (response == "true") {
                            callback(true);
                        } else {
                            callback(false);
                        }

                    },
                    error: function() {
                        App.Dialogs.Error();
                    }
                });

            }

        }

    },


    Form: {


        Validate: function () {
            return true;
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
            ActivePage.ClientState.StatusIndicators.SetIndicators(status);

        },


        StatusIndicators: {

            SetIndicators: function() {

                var status = ActivePage.Status.CurrentStatus();

                var displayElement = $("input[name=StatusDisplay]");
                console.log("displayElement Value: " + displayElement.val());
                console.log("status: " + status);

                if (status == -1 ||
                    status == 1 ||
                    status == 2 || 
                    status == "History" || 
                    status == "ConsiderForDischarge" ||
                    status == "CFD" ||
                    status == "Good") {

                    clearStatusIndicators(displayElement);

                } else {
                    setNotGoodIndicators(displayElement);
                }
                
                function clearStatusIndicators(el) {
                    //console.log('setting good status');
                    //el.css("color", App.Themes.Colors.TextColorDisabled);
                }

                function setNotGoodIndicators(el) {
                    //console.log('setting bad status');
                    //el.css("color", App.Themes.Colors.TextColorBadStatus);
                    
                }

            }

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

            ctlHasIntake = App.DevEx.GetControl("HasIntake");
            ctlHasPrescription = App.DevEx.GetControl("HasPrescription");
                        
            ctlHasIntake.ValueChanged.AddHandler(function (s, e) { ActivePage.Status.Calculate(); });
            ctlHasPrescription.ValueChanged.AddHandler(function (s, e) { ActivePage.Status.Calculate(); });
            
            try {
                App.DevEx.GetControl("btnSetStatusCFD").Click.AddHandler(function (s, e) { ActivePage.Status.ToggleCFD(); });
            } catch (err) {
                // do nothing
                // not sure why this was erroring when going to a discharged patient, but this works around it
            }
            
            
        }





    },  // END Controls



    zzDummy: null




}   // END ActivePage







