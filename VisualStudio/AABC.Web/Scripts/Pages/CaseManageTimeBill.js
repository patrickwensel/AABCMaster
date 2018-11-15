function initComponent(caseID) {
    TimeBill.Initialize(caseID);
}



var caseIDProvider = null;

function registerCaseIDProvider(func) {
    caseIDProvider = func;
}




var DefinalizeProvider = {

    ShowPopup: function () {

        App.DevEx.GetControl("dcpPopup").Show();

    },

    Definalize: function () {

        var providerID = App.DevEx.GetControl("dcpProvider").GetValue();
        var period = App.DevEx.GetControl("dcpSelectedDate").GetValue();
        var caseID = $("#CaseManageTimeBillCaseID").text();

        $.ajax({
            type: 'POST',
            url: '/Case/TimeBillDefinalizeProvider',
            data: {
                caseID: caseID,
                providerID: providerID,
                firstDayOfPeriod: period.toISOString()
            },
            success: function () {
                App.Dialogs.MessageBox("Finalization removed", App.Dialogs.Icons.Ok);
                App.DevEx.GetControl("dcpPopup").Hide();
                App.DevEx.GetControl(TimeBill.Scrub.Grid.ObjectName).Refresh();
            },
            error: function () {
                App.Dialogs.Error();
            }

        });

    }
};





var TimeBill = {

    CaseID: null,

    Initialize: function (caseID) {

        TimeBill.CaseID = $("#CaseManageTimeBillCaseID").text();

        TimeBill.Scrub.Grid.ObjectName = "gvCaseTimeBillGrid";
        TimeBill.Scrub.TargetDate.ObjectName = "SelectedDate";

        TimeBill.Scrub.Grid.Object = App.DevEx.GetControl(TimeBill.Scrub.Grid.ObjectName);
        TimeBill.Scrub.TargetDate.Object = App.DevEx.GetControl(TimeBill.Scrub.TargetDate.ObjectName);

        initHoursEntry(caseID);

        function initHoursEntry(caseID) {

            var aabc = window.aabc || {};
            window.aabc = aabc;

            window.aabc.deleteHoursEntry = function (hoursID) {

                $.ajax({
                    url: '/HoursEntry/GetDeleteConfirmSummary',
                    type: 'GET',
                    data: { hoursID: hoursID },
                    success: function (data) {
                        if (confirm("Are you sure you want to delete this entry? \n\n" + data)) {
                            $.ajax({
                                url: '/HoursEntry/Delete',
                                type: 'POST',
                                data: { hoursID: hoursID },
                                success: function (response) {
                                    TimeBill.Scrub.Grid.Object.Refresh();
                                },
                                error: function () {
                                    alert("We're sorry, we ran into an issue with that.  Please contact your administrator if the problem continues.");
                                }
                            });
                        }
                    }
                });

            };

            window.aabc.showHoursEntry = function (hoursID) {

                //hoursID = 118433;     // bcba test
                //hoursID = 118432;     // aide test

                var cancelCallback = function () {
                    App.DevEx.GetControl("pc-hours-entry").Hide();
                };

                var successCallback = function () {
                    App.DevEx.GetControl("pc-hours-entry").Hide();
                    TimeBill.Scrub.Grid.Object.Refresh();
                };

                var deleteCallback = function () {
                    if (confirm("Are you sure you want to delete this entry?")) {
                        $.ajax({
                            url: '/HoursEntry/Delete',
                            type: 'POST',
                            data: { hoursID: hoursID }
                        }).done(function () {
                            App.DevEx.GetControl("pc-hours-entry").Hide();
                            TimeBill.Scrub.Grid.Object.Refresh();
                        });
                    }
                };

                if (hoursID) {
                    $.ajax({
                        url: '/HoursEntry/GetEntryVM',
                        data: { hoursID: hoursID }
                    }).done(function (data) {
                        var vm = new window.aabc.hoursEntry.HoursEntryVM(data, cancelCallback, successCallback, deleteCallback);
                        var root = document.getElementById('hours-entry-container-pp');
                        ko.cleanNode(root);
                        ko.applyBindings(vm, root);
                        App.DevEx.GetControl("pc-hours-entry").Show();
                    });
                } else {

                    registerCaseIDProvider(function () { return caseID; });

                    $.ajax({
                        url: '/HoursEntry/ProviderSelect',
                        type: 'GET',
                        data: {
                            caseID: caseID
                        },
                        success: function (data) {
                            var popup = App.DevEx.GetControl("pc-hours-entry-provider-select");
                            popup.Show();
                        },
                        error: function () {
                            alert("We're sorry, we ran into an issue with that.  Please contact your administrator if the problem persists.");
                        }
                    });
                }
            };

            var modalId = "auth-resolution-details-modal";
            var vm = new AuthResolutionDetailsVM();
            var root = document.getElementById(modalId);
            ko.cleanNode(root);
            ko.applyBindings(vm, root);

            function AuthResolutionDetailsVM(data) {
                var self = {};
                self.items = ko.observableArray([]);
                self.update = function (data) {
                    self.items(data);
                };
                if (data) {
                    self.update(data);
                }
                return self;
            }

            window.aabc.showAuthResolutionDetails = function (hoursId) {
                $.get("/Case/GetAuthResolutionDetails", { hoursId: hoursId }).done(function (data) {
                    if (data) {
                        vm.update(data);
                        App.DevEx.GetControl(modalId).Show();
                    }
                });
            };

        }


    },  // end TimeBill.Initialize

    Scrub: {


        DownloadHoursReports: function () {
            App.DevEx.GetControl("DownloadHoursReportsPopup").Show();
        },

        DownloadPatientHoursReport: function () {
            var caseID = $("#CaseManageTimeBillCaseID").text();
            var period = TimeBill.Scrub.TargetDate.GetValue();
            window.open("/Case/PatientHoursReport?caseID=" + caseID + "&period=" + period.toISOString(), "_blank");
        },

        DownloadParentHoursReport: function () {
            var caseID = $("#CaseManageTimeBillCaseID").text();
            var period = TimeBill.Scrub.TargetDate.GetValue();
            window.open("/Case/ParentHoursReport?caseID=" + caseID + "&period=" + period.toISOString(), "_blank");
        },

        DownloadPatientHoursWithSignLine: function () {
            var caseID = $("#CaseManageTimeBillCaseID").text();
            var period = TimeBill.Scrub.TargetDate.GetValue();
            window.open("/Case/PatientHoursSignLineReport?caseID=" + caseID + "&period=" + period.toISOString(), "_blank");
        },

        DownloadSupervisingBCBAReport: function () {
            var caseID = $("#CaseManageTimeBillCaseID").text();
            var period = TimeBill.Scrub.TargetDate.GetValue();
            window.open("/Case/PatientHoursSupervisingBCBAReport?caseID=" + caseID + "&period=" + period.toISOString(), "_blank");
        },

        AddNew: function () {

            var caseID = $("#CaseManageTimeBillCaseID").text();

            App.Popup.Show({

                url: "/Case/AddHoursPopup",
                data: {
                    caseID: caseID
                },
                options: {
                    width: 400,
                    height: 200,
                    title: 'Add Hours'
                },
                finished: function (response) {
                    TimeBill.Scrub.Grid.Object.Refresh();
                },
                error: function (response) {
                    App.Dialogs.Error(response);
                }


            });

        },

        SubmitNew: function () {

            var caseID = $("#CaseManageTimeBillCaseID").text();
            var providerID = App.DevEx.GetControl("cahpProvider").GetValue();
            var date = App.DevEx.GetControl("cahpDate").GetValue();
            var timeIn = App.DevEx.GetControl("cahpTimeIn").GetValue();
            var timeOut = App.DevEx.GetControl("cahpTimeOut").GetValue();
            var serviceID = App.DevEx.GetControl("cahpService").GetValue();
            var notes = App.DevEx.GetControl("cahpNotes").GetValue();
            var isAdjustment = App.DevEx.GetControl("cahpIsPayOrBillAdjustment").GetValue();

            if (isAdjustment === null) {
                isAdjustment = false;
            }

            $.ajax({
                type: 'POST',
                url: '/Case/AddHoursSubmit',
                data: {
                    caseID: caseID,
                    providerID: providerID,
                    date: date.toISOString(),
                    timeIn: timeIn.toISOString(),
                    timeOut: timeOut.toISOString(),
                    serviceID: serviceID,
                    notes: notes,
                    isAdjustment: isAdjustment
                },
                success: function (res) {
                    App.Popup.Hide('ok');
                },
                error: function (res) {
                    App.Dialogs.Error();
                }

            });



        },

        FinalizeAll: function () {
            if (confirm('Are you sure you want to finalize all hours shown?  You won\'t be able to edit them once saved...')) {

                // update any pending edits
                TimeBill.Scrub.Grid.Object.UpdateEdit();

                window.setTimeout(function () {

                    $.ajax({
                        url: '/Case/FinalizePeriodHours',
                        type: 'POST',
                        data: {
                            caseID: TimeBill.CaseID,
                            periodStartDate: TimeBill.Scrub.TargetDate.GetValue().toISOString()
                        },
                        success: function (res) {
                            TimeBill.Scrub.Grid.Object.Refresh();
                        },
                        error: function () {
                            App.Dialogs.Error();
                        }
                    });

                }, 250);

            }
        },   // end Scrub.FinalizeAll


        Grid: {

            // Fields
            Object: null,
            ObjectName: null,

            // Functions
            BeginCallback: function (s, e) {
                e.customArgs["caseID"] = TimeBill.CaseID;
                e.customArgs["selectedPeriod"] = TimeBill.Scrub.TargetDate.GetValue().toISOString();
            },

            EndCallback: function (s, e) {
                TimeBill.Scrub.TargetDate.Object = App.DevEx.GetControl(TimeBill.Scrub.TargetDate.ObjectName);
            },

            SaveBatch: function (s, e) {

                var grid = TimeBill.Scrub.Grid.Object;

                if (grid.batchEditApi.ValidateRows()) {
                    if (confirm('Are you sure you want to save these hours?  You won\'t be able to edit them once saved...')) {
                        grid.UpdateEdit();
                    }
                } else {
                    App.Dialogs.MessageBox(
                        "Please make sure all rows are valid before saving.",
                        App.Dialogs.Icons.Information);
                }

            },   // end TimeBill.Scrub.Grid.SaveBatch

            CancelEdits: function () {

                var grid = TimeBill.Scrub.Grid.Object;
                grid.CancelEdit();
                grid.Refresh();

            }   // end TimeBill.Scrub.Grid.CancelEdits




        },  // end Scrub.Grid

        TargetDate: {

            Object: null,
            ObjectName: null,

            GetValue: function () {
                return TimeBill.Scrub.TargetDate.Object.GetValue();
            },

            AfterUpdate: function () {
                window.setTimeout(function () {
                    TimeBill.Scrub.Grid.Object.PerformCallback();
                }, 250);
            }

        }   // end TimeBill.Scrub.TargetDate


    }   // end TimeBill.Scrub




};   // end TimeBill

