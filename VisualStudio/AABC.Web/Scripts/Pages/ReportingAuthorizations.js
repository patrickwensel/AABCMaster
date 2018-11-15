// reportSelector

(function () {

    var code = {

        auths: {

            container: function () {
                return $("#AuthsReportsContentContainer");
            },
            screen: {
                patientId: 0,
                patientName: ''
            },
            loadBreakdownsByCase: function () {

                var container = code.auths.container();

                container.empty();

                $.ajax({
                    type: 'GET',
                    url: '/Reporting/Authorizations/BreakdownByCase',
                    success: function (res) {
                        container.html(res);
                        window.setTimeout(init(), 0);
                    }
                });

                function init() {

                    App.DevEx.GetControl("AuthBreakdownsByCaseGoButton").Click.AddHandler(function (s, e) {

                        var startDate = App.DevEx.GetControl("AuthBreakdownsByCaseStartDate").GetValue();
                        var endDate = App.DevEx.GetControl("AuthBreakdownsByCaseEndDate").GetValue();
                        if (!validate(startDate, endDate, code.auths.screen.patientId)) {
                            App.Dialogs.MessageBox("Please enter all three values and ensure they're correct");
                            return;
                        } else {

                            var startDate = App.DevEx.GetControl("AuthBreakdownsByCaseStartDate").GetValue();
                            var endDate = App.DevEx.GetControl("AuthBreakdownsByCaseEndDate").GetValue();
                            loadResults(startDate, endDate, code.auths.screen.patientId);
                        }
                    });
                }

                function validate(startDate, endDate, patientId) {

                    if (startDate == null || !isDate(startDate)) {
                        return false;
                    }

                    if (endDate == null || !isDate(endDate)) {
                        return false;
                    }

                    if (patientId == null || !isInt(patientId)) {
                        return false;
                    }

                    if (startDate > endDate) {
                        return false;
                    }

                    return true;

                    function isDate(check) {
                        return Object.prototype.toString.call(check) === '[object Date]';
                    }

                    function isInt(value) {
                        var x;
                        if (isNaN(value)) {
                            return false;
                        }
                        x = parseFloat(value);
                        return (x | 0) === x;
                    }

                }

                function loadResults(startDate, endDate, patientID) {


                    $.ajax({
                        type: 'GET',
                        url: '/Reporting/Authorizations/BreakdownsByCaseGrid',
                        data: {
                            startDate: startDate.toISOString(),
                            endDate: endDate.toISOString(),
                            patientID: patientID
                        },
                        success: function (res) {

                            $('#AuthBreakdownsByCaseGridContainer').empty().html(res);
                            window.setTimeout(initGrid(), 0);

                            function initGrid() {


                            }

                        }
                    });
                }

            },  // end code.auths.loadBreakdownsByCase

            selectPatient: function () {
                App.Popup.Show({
                    url: '/Patients/PatientSelect',
                    options: {
                        width: 500,
                        height: 600,
                        title: "Select Patient"
                    },
                    finished: function (response) {

                        var input = App.DevEx.GetControl("AuthBreakdownsByCasePatientName")
                        input.SetValue(window.Patient.Select.patientName);
                        code.auths.screen.patientId = window.Patient.Select.patientId;
                        code.auths.screen.patientName = window.Patient.Select.patientName;
                        App.Popup.Hide();


                    },
                    error: function (response) {
                        App.Dialogs.Error();
                    }
                });
            },
            loadInvalidCaseAuths: function () {

                var container = code.auths.container();

                container.empty();

                $.ajax({
                    type: 'GET',
                    url: '/Reporting/Authorizations/InvalidCaseAuths',
                    success: function (res) {
                        container.html(res);
                    }
                });

            }   // end code.auths.loadInvalidCaseAuths;

        }   // end code.auths

    }   // end code



    var rs = {  // reportSelector

        initialize: function () {
            console.log('initialing report selector');
            rs.billing.init();
            rs.payroll.init();
            rs.catalyst.init();
            rs.approvals.init();
            rs.authrules.init();
            rs.userstaff.init();
            rs.tabledump.init();
            rs.latestnotesandtasks.init();
            rs.providercaseloads.init();
            rs.authorizationutilization.init();
        },

        openReport: function (url) {

            var win = window.open(url, "_blank");
            win.focus();

        },  // end rs.openReport

        billing: {

            init: function () {
                var r = rs.billing;
                r.exportHours.init();
                //r.providersMissingBilling.init();
                r.unfinalizedProviders.init();
            },

            exportHours: {

                init: function () {
                    App.DevEx.GetControl("DownloadButtonBillingBillingExport").Click.AddHandler(function (s, e) {

                        var period = App.DevEx.GetControl("DateSelectorBillingBillingExport").GetValue();
                        rs.openReport("/Reporting/Billing/BillingExport/?period=" + period.toISOString());

                    });
                }

            },  // end rs.billing.exportHours

            providersMissingBilling: {

                init: function () {
                    App.DevEx.GetControl("DownloadButtonBillingProvidersWithoutBilling").Click.AddHandler(function (s, e) {

                        var period = App.DevEx.GetControl("DateSelectorBillingProvidersWithoutBilling").GetValue();
                        rs.openReport("/Reporting/Billing/ProvidersWithoutHours/?period=" + period.toISOString());

                    });
                }

            },   // end rs.billing.providersMissingBilling

            unfinalizedProviders: {

                init: function () {

                    App.DevEx.GetControl("DownloadButtonBillingUnfinalizedProviders").Click.AddHandler(function (s, e) {

                        var period = App.DevEx.GetControl("DateSelectorBillingUnfinalizedProviders").GetValue();
                        rs.openReport("/Reporting/Billing/UnfinalizedProviders/?period=" + period.toISOString());

                    });
                }

            }   // end rs.billing.unfinalizedProviders

        },  // end rs.billing

        payroll: {

            init: function () {
                var r = rs.payroll;
                r.hoursExport.init();
            },

            hoursExport: {

                init: function () {
                    App.DevEx.GetControl("DownloadButtonPayrollPayrollHours").Click.AddHandler(function (s, e) {

                        var period = App.DevEx.GetControl("DateSelectorPayrollPayrollHours").GetValue();
                        rs.openReport("/Reporting/Payroll/PayrollExport/?period=" + period.toISOString());

                    });
                }

            }   // end rs.payroll.hoursExport

        },  // end rs.payroll

        catalyst: {

            init: function () {
                var r = rs.catalyst;
                r.providersMissingData.init();
            },

            providersMissingData: {

                init: function () {
                    App.DevEx.GetControl("DownloadButtonCatalystProvMissingData").Click.AddHandler(function (s, e) {

                        var period = App.DevEx.GetControl("DateSelectorCatalystProvMissingData").GetValue();
                        rs.openReport("/Reporting/Catalyst/ProvidersWithoutData/?period=" + period.toISOString());

                    });
                }

            }   // end rs.catalyst.providersMissingData

        },   // end rs.catalyst

        approvals: {

            init: function () {
                var r = rs.approvals;
                r.guardianApprovals.init();
            },

            guardianApprovals: {

                init: function () {
                    App.DevEx.GetControl("DownloadButtonGuardianApprovalHours").Click.AddHandler(function (s, e) {

                        var period = App.DevEx.GetControl("DateSelectorGuardianApprovalHours").GetValue();
                        var insurance = App.DevEx.GetControl("InsuranceSelectorGuardianApprovalHours").GetValue();
                        if (!insurance) {
                            App.Dialogs.Error("Please select an insurance provider.");
                            return;
                        }

                        $.ajax({
                            type: 'post',
                            url: '/Reporting/Approvals/GuardianApprovalExists',
                            data: {
                                period: period.toISOString(),
                                insuranceID: insurance
                            },
                            success: function (res) {
                                if (res) {
                                    window.location = "/Reporting/Approvals/GuardianApproval?period=" + period.toISOString() + "&insuranceID=" + insurance;
                                }
                                else {
                                    App.Dialogs.Error("No guardian hours approvals exist for this period and insurance.");
                                }
                            },
                            error: function (res) {
                                App.Dialogs.Error(res);
                            }

                        })

                    });
                }

            }
        },
        authrules: {

            init: function () {
                var r = rs.authrules;
                r.insuranceAuthRules.init();
            },

            insuranceAuthRules: {

                init: function () {
                    App.DevEx.GetControl("DownloadButtonInsuranceAuthRuleReport").Click.AddHandler(function (s, e) {

                        var insurance = App.DevEx.GetControl("InsuranceSelectorAuthRuleReport").GetValue();

                        rs.openReport("/Reporting/Insurance/AuthRuleExport/?insuranceId=" + insurance);

                    });
                }

            }
        },
        userstaff: {

            init: function () {
                var r = rs.userstaff;
                r.userstaffComparison.init();
            },

            userstaffComparison: {

                init: function () {
                    App.DevEx.GetControl("DownloadButtonUserStaffReport").Click.AddHandler(function (s, e) {

                        rs.openReport("/Reporting/Users/StaffComparison");

                    });
                }

            }
        },
        tabledump: {
            init: function () {
                App.DevEx.GetControl("DownloadButtonTableDump").Click.AddHandler(function (s, e) {
                    rs.openReport("/Reporting/Dump");
                });
            }
        },
        latestnotesandtasks: {
            init: function() {
                App.DevEx.GetControl("DownloadLatestNotesAndTasks").Click.AddHandler(function (s, e) {
                    rs.openReport("/Reporting/LatestNotesAndTasks");
                });
            }
        },
        providercaseloads: {
            init: function () {
                App.DevEx.GetControl("DownloadProviderCaseloads").Click.AddHandler(function (s, e) {
                    rs.openReport("/Reporting/ProviderCaseloads");
                });
            }
        },
        authorizationutilization: {
            init: function () {
                App.DevEx.GetControl("DownloadAuthorizationUtilization").Click.AddHandler(function (s, e) {
                    rs.openReport("/Reporting/AuthorizationUtilization");
                });
            }
        }
    }


    if (window.Reporting == null) window.Reporting = {};
    if (window.Reporting.Selector == null) window.Reporting.Selector = rs;
    if (window.Reporting.ReportSelector == null) window.Reporting.ReportSelector = rs;
    if (window.Reporting.Authorization == null) window.Reporting.Authorization = code.auths;

})();