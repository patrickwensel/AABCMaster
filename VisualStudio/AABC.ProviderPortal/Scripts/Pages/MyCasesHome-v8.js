
(function () {

    window.page = {

        initialize: function (isBcba, providerID, signingSuccess) {
            page.provider.isBcba = isBcba;
            page.provider.id = providerID;

            if (signingSuccess === 'True') {
                App.Dialogs.MessageBox("Hours signed successfully", App.Dialogs.Icons.Ok);
            }

            if (signingSuccess === 'False') {
                App.Dialogs.MessageBox("Hours were not signed", App.Dialogs.Icons.Warning);
            }

            $('#fv-close').click(function () {
                App.DevEx.GetControl("fv-popup").Hide();
            });

        },
        provider: {
            isBcba: false,
            useExtendedForBCBA: false,
            id: null
        },
        cases: {
            currentID: function () {
                var visibleIndex = gridCases.GetFocusedRowIndex();
                var id = gridCases.GetRowKey(visibleIndex);
                return id;
            }
        }
    };


    window.GenerateAideList = function () {
        if (!page.provider.isBcba) {
            return;
        }

        try {

            $.ajax({
                type: 'GET',
                url: '/Home/GetAideList',
                data: {
                    caseID: getCurrentCaseID()
                },
                success: function (res) {

                    var t = $('#aide-list-table');
                    t.empty();

                    console.log(res);

                    for (var i = 0; i < res.length; i++) {

                        var aide = res[i];
                        var nameCell = '';
                        var emailCell = '';
                        var phoneCell = '';

                        nameCell = aide.Name;

                        if (aide.Email == null) {
                            emailCell = '[no email on file]';
                        } else {
                            emailCell = '<a href="mailto:' + aide.Email + '">' + aide.Email + '</a>';
                        }

                        if (aide.Phone == null) {
                            phoneCell = '[no phone on file]';
                        } else {
                            phoneCell = aide.Phone;
                        }


                        t.append('<tr><td>' + nameCell + '</td><td>' + emailCell + '</td><td>' + phoneCell + '</td></tr>');
                    }

                    $('#CaseProvidersListContainer').show(600);

                },
                error: function () { }
            });

        } catch (err) {
            console.log(err);
        }
    };


    window.lnkFinalizeClick = function (s, e) {
        e.htmlEvent.preventDefault();
        if (page.provider.isBcba) {
            if (!confirm('Did you run progress report for this child for this month?')) {
                return;
            }
        }
        var link = s.name.replace('hlFinalize_', '');
        var year = link.substring(0, 4);
        var monthName = link.substring(4);
        var firstDayOfMonth = getDate(monthName, year);
        $.ajax({
            type: 'GET',
            url: '/Home/ValidateFinalizeMonth',
            data: {
                caseID: getCurrentCaseID(),
                year: firstDayOfMonth.getFullYear(),
                month: firstDayOfMonth.getMonth() + 1
            },
            success: function (res) {
                if (res == "ok") {

                    $.ajax({
                        type: 'GET',
                        url: '/Finalize/GetDeepValidationFailures',
                        data: {
                            caseID: page.cases.currentID(),
                            providerID: page.provider.id,
                            periodYear: firstDayOfMonth.getFullYear(),
                            periodMonth: firstDayOfMonth.getMonth() + 1
                        },
                        success: function (res) {
                            if (res.length === 0) {
                                getSignatureForValidation();
                            } else {
                                handleDeepValidationFailures(res);
                            }
                        }
                    });
                } else {
                    App.Dialogs.MessageBox(res, App.Dialogs.Icons.Warning, "Unable to Finalize");
                }
            },
            error: function () {
                App.Dialogs.Error();
            }
        });



        function getSignatureForValidation() {
            App.Dialogs.ConfirmDelete({
                message: "Finalize hours for " + getMonthName(firstDayOfMonth) + ", " + year + "?",
                confirmed: function () {
                    App.LoadingPanel.Show();
                    $.ajax({
                        url: "/Home/FinalizeMonthSignature",
                        type: 'POST',
                        data: {
                            caseID: getCurrentCaseID(),
                            year: firstDayOfMonth.getFullYear(),
                            month: firstDayOfMonth.getMonth() + 1
                        },
                        success: function (res) {
                            window.location.href = res;
                        },
                        error: function () {
                            App.Dialogs.Error();
                        }
                    });
                }
            });
        }

        return false;
        function getMonthName(date) {
            var monthNames = ["January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"];
            return monthNames[date.getMonth()];
        }
        function getDate(mon, yr) {
            var d = Date.parse(mon + "1, " + yr);
            return new Date(d);
        }
    };

    window.handleDeepValidationFailures = function (failures) {

        //alert('deep validation failures:' + failures);
        //console.log(failures);

        var html = $('<div>');

        for (var i = 0; i < failures.length; i++) {
            var failure = failures[i];
            switch (failure.Type) {
                case 'CAT':
                    html.append(getCAT(failure));
                    break;
                case 'POS':
                    html.append(getPOS(failure));
                    break;
                case 'SM':
                    html.append(getSM(failure));
                    break;
                case 'CO':
                    html.append(getCO(failure));
                    break;
                case 'NM':
                    html.append(getNM(failure));
                    break;
                default:
                    console.log(failure);
                    html.append('<div>default</div>');
                    break;
            }
        }

        $('.fv-messages').empty().append(html);

        App.DevEx.GetControl("fv-popup").Show();



        function getCAT(data) {

            var element = $('#fv-cat-itemtemplate').clone(false, false);
            element.removeAttr('id');

            element.find('.patient').text(data.SourcePatientName);
            element.find('.datetime').text(getTimeRangeDisplay(data.SourceDate, data.SourceTimeIn, data.SourceTimeOut));

            element.show(0);

            return element;

        }


        function getSM(data) {

            var element = $('#fv-sm-itemtemplate').clone(false, false);
            element.removeAttr('id');

            element.find('.patient').text(data.SourcePatientName);
            element.find('.datetime').text(getTimeRangeDisplay(data.SourceDate, data.SourceTimeIn, data.SourceTimeOut));

            element.show(0);

            return element;

        }

        function getPOS(data) {

            var element = $('#fv-pos-itemtemplate').clone(false, false);
            element.removeAttr('id');

            element.find('.patientA').text(data.SourcePatientName);
            element.find('.datetimeA').text(getTimeRangeDisplay(data.SourceDate, data.SourceTimeIn, data.SourceTimeOut));
            element.find('.serviceA').text(data.SourceServiceCode);

            element.find('.patientB').text(data.PartnerPatientName);
            element.find('.datetimeB').text(getTimeRangeDisplay(data.PartnerDate, data.PartnerTimeIn, data.PartnerTimeOut));
            element.find('.serviceB').text(data.PartnerServiceCode);

            element.show(0);

            return element;

        }

        function getCO(data) {

            var element = $('#fv-co-itemtemplate').clone(false, false);
            element.removeAttr('id');

            element.find('.patientA').text(data.SourcePatientName);
            element.find('.datetimeA').text(getTimeRangeDisplay(data.SourceDate, data.SourceTimeIn, data.SourceTimeOut));
            element.find('.serviceA').text(data.SourceServiceCode);

            element.find('.patientB').text(data.PartnerPatientName);
            element.find('.datetimeB').text(getTimeRangeDisplay(data.PartnerDate, data.PartnerTimeIn, data.PartnerTimeOut));
            element.find('.serviceB').text(data.PartnerServiceCode);

            element.show(0);

            return element;

        }


        function getNM(data) {
            var element = $('#fv-nm-itemtemplate').clone(false, false);
            element.removeAttr('id');
            element.find('.patient').text(data.SourcePatientName);
            element.find('.datetime').text(getTimeRangeDisplay(data.SourceDate, data.SourceTimeIn, data.SourceTimeOut));
            element.show(0);
            return element;
        }


        function getTimeRangeDisplay(d, t1, t2) {

            var datePart = new Date(d).toLocaleDateString(undefined, {
                day: 'numeric',
                month: 'numeric'
            });

            var startPart = new Date(t1).toLocaleTimeString(undefined, {
                hour: '2-digit',
                minute: '2-digit'
            });

            var endPart = new Date(t2).toLocaleTimeString(undefined, {
                hour: '2-digit',
                minute: '2-digit'
            });

            return datePart + ' - ' + startPart + ' to ' + endPart;
        }

    };

    window.lnkFinalizeClickNoSignature = function (s, e) {
        var cautionString = "Please note: You are about to finalize your hours and are now attesting to their accuracy. " +
            "In addition you attest to the accuracy of your notes and or data that you have accurately documented.\n\n" +
            "Please be aware that this finalization is considered a Medical Record.   " +
            "Any person who knowingly submits false or inaccurate information on a medical record is subject to punishment under the U.S False Claims Act.";

        alert(cautionString);

        e.htmlEvent.preventDefault();
        if (page.provider.isBcba) {
            if (!confirm('Did you run progress report for this child for this month?')) {
                return;
            }
        }
        var link = s.name.replace('hlFinalize_', '');
        var year = link.substring(0, 4);
        var monthName = link.substring(4);
        var firstDayOfMonth = getDate(monthName, year);
        $.ajax({
            type: 'GET',
            url: '/Home/ValidateFinalizeMonth',
            data: {
                caseID: getCurrentCaseID(),
                year: firstDayOfMonth.getFullYear(),
                month: firstDayOfMonth.getMonth() + 1
            },
            success: function (res) {
                if (res == "ok") {

                    $.ajax({
                        type: 'GET',
                        url: '/Finalize/GetDeepValidationFailures',
                        data: {
                            caseID: page.cases.currentID(),
                            providerID: page.provider.id,
                            periodYear: firstDayOfMonth.getFullYear(),
                            periodMonth: firstDayOfMonth.getMonth() + 1
                        },
                        success: function (res) {
                            if (res.length === 0) {
                                proceedWithValidated();
                            } else {
                                handleDeepValidationFailures(res);
                            }
                        }
                    });

                } else {
                    App.Dialogs.MessageBox(res, App.Dialogs.Icons.Warning, "Unable to Finalize");
                }
            },
            error: function () {
                App.Dialogs.Error();
            }
        });

        function proceedWithValidated() {
            App.Dialogs.ConfirmDelete({
                message: "Finalize hours for " + getMonthName(firstDayOfMonth) + ", " + year + "?",
                confirmed: function () {
                    $.ajax({
                        url: "/Home/PostFinalization",
                        type: 'POST',
                        data: {
                            caseID: getCurrentCaseID(),
                            year: firstDayOfMonth.getFullYear(),
                            month: firstDayOfMonth.getMonth() + 1
                        },
                        success: function (res) {
                            App.Popup.Hide('ok');
                        },
                        error: function () {
                            App.Dialogs.Error();
                        }
                    });
                }
            });
        }
        return false;
        function getMonthName(date) {
            var monthNames = ["January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"];
            return monthNames[date.getMonth()];
        }
        function getDate(mon, yr) {
            var d = Date.parse(mon + "1, " + yr);
            return new Date(d);
        }
    };


    window.gvCaseFocusedRowChanged = function (s, e) {
        var visibleIndex = gridCases.GetFocusedRowIndex();
        var id = gridCases.GetRowKey(visibleIndex);
        caseSelected(id);
    };

    window.setHoursLogConfig = function () {

        if (page.provider.isBcba) {
            return;
        }

        $('#gridCaseHoursDetail_DXFilterRow').hide(0);


    };

    window.caseSelected = function (caseID) {
        if (caseID == null) {
            $("#CaseGeneralHoursContainer").hide(0);
            $('#CaseProvidersListContainer').hide(0);
            $("#CaseHoursEntryContainer").hide(0);
            $("#CaseHoursLogsContainer").hide(0);
            return;
        } else {
            // transition hide
            $("#CaseHoursLogsContainer").hide(200, function () {
                $("#CaseHoursEntryContainer").hide(200, function () {
                    $("#CaseGeneralHoursContainer").hide(200, function () {
                        $('#CaseProvidersListContainer').hide(200, function () {
                            $.ajax({
                                url: "/Home/CaseGeneralHours",
                                type: 'GET',
                                data: { caseID: caseID },
                                success: function (r) {
                                    $("#CaseGeneralHoursContainer").empty().append(r).show(600, function () {
                                        App.LoadingPanel.AutoPanel.SupressSingle();
                                        $.ajax({
                                            url: "/Home/CaseHoursEntry",
                                            type: 'GET',
                                            data: { caseID: caseID },
                                            success: function (r) {

                                                GenerateAideList();

                                                App.LoadingPanel.AutoPanel.SupressSingle();
                                                $("#CaseHoursEntryContainer").empty().append(r).show(300, function () {
                                                    App.LoadingPanel.AutoPanel.SupressSingle();
                                                    $.ajax({
                                                        url: "/Home/CaseHoursLogs",
                                                        type: 'GET',
                                                        data: { caseID: caseID, showAll: false },
                                                        success: function (r) {
                                                            $("#CaseHoursLogsContainer").empty().append(r).show(600);
                                                            setHoursLogConfig();
                                                        },
                                                        error: function () {
                                                            App.Dialogs.Error();
                                                        }
                                                    });
                                                });
                                            },
                                            error: function () {
                                                App.Dialogs.Error();
                                            }
                                        });
                                    });
                                },
                                error: function () {
                                    App.Dialogs.Error();
                                }
                            });
                        });
                    });
                });
            });
        }
    };

    window.btnFinalizeMonthClick = function (s, e) {
        $.ajax({
            url: "/Home/GetTotalPendingHours",
            type: 'GET',
            data: {
                caseID: getCurrentCaseID()
            },
            success: function (res) {
                if (res != 0) {
                    App.Dialogs.MessageBox("Please commit all pending hours before finalizaing.", App.Dialogs.Icons.Warning);
                } else {
                    App.Popup.Show({
                        url: '/Home/FinalizeMonthPopup',
                        data: {
                            caseID: getCurrentCaseID()
                        },
                        options: {
                            width: 500,
                            height: 300,
                            title: "Finalize Monthly Hours"
                        },
                        finished: function (res) {
                            if (res == 'ok') {
                                window.setTimeout(function () {
                                    ReloadCalendar();
                                    RefreshPendingHoursCount();
                                }, 100);
                            }
                        },
                        error: function (err) {
                            App.Dialogs.Error();
                        }
                    });
                }
            },
            error: function (err) {
                App.Dialogs.Error();
            }
        });
    };

    window.RefreshPendingHoursCount = function () {
        $.ajax({
            url: "/Home/GetTotalPendingHours",
            type: 'GET',
            data: {
                caseID: getCurrentCaseID()
            },
            success: function (res) {
                $("#calendar-hours-count").text(res);
            },
            error: function (err) {
                $("#calendar-hours-count").text("0");
            }
        });
    };

    window.btnSavePendingHoursClick = function () {
        $.ajax({
            url: "/Home/CommitPendingHours",
            type: 'POST',
            data: {
                caseID: getCurrentCaseID()
            },
            success: function (res) {
                if (res == "ok") {
                    ReloadCalendar();
                    RefreshHoursLog();
                    RefreshPendingHoursCount();
                    alert('Saved');
                    //App.Dialogs.MessageBox("Saved", App.Dialogs.Icons.Ok);
                } else {
                    App.Dialogs.Error();
                }
            },
            error: function (err) {
                App.Dialogs.Error();
            }
        });
    };

    window.btnDownloadHoursClick = function () {
        var visibleIndex = gridCases.GetFocusedRowIndex();
        var id = gridCases.GetRowKey(visibleIndex);
        window.location.href = "/Home/DownloadHoursRecord?caseID=" + id;
    };

    window.HoursCreateShowPopup = function (hoursDate) {
        $.ajax({
            type: 'GET',
            url: '/Home/CanCreate',
            data: {
                caseID: getCurrentCaseID(),
                date: hoursDate.toISOString()
            }
        })
            .done(function (res) {
                if (res == "true") {
                    proceedWithValidated(hoursDate.toISOString());
                }
            }).fail(function () {
                App.Dialogs.Error();
            });
        function proceedWithValidated(date) {
            $.ajax({
                url: '/HoursEntry/GetEntryHtml',
                type: 'GET'
            }).done(function (html) {
                $('#hours-entry-container-pp').empty().append(html);
                var caseID = page.cases.currentID();
                var providerID = page.provider.id;
                $.ajax({
                    url: '/HoursEntry/GetNewEntryDataTemplate',
                    type: 'GET',
                    data: {
                        caseID: caseID,
                        providerID: providerID,
                        date: date
                    }
                })
                    .done(function (data) {
                        var cancelCallback = function () {
                            App.DevEx.GetControl("pc-hours-entry").Hide();
                        };
                        var successCallback = function () {
                            App.DevEx.GetControl("pc-hours-entry").Hide();
                            ReloadCalendar();
                            RefreshHoursLog();
                            RefreshPendingHoursCount();
                        };
                        var deleteCallback = function () {
                            // do nothing... this should never be called anyway
                        };
                        var vm = new window.aabc.hoursEntry.HoursEntryVM(data, cancelCallback, successCallback, deleteCallback);
                        var root = document.getElementById('hours-entry-container-pp');
                        ko.cleanNode(root);
                        ko.applyBindings(vm, root);
                        App.DevEx.GetControl("pc-hours-entry").Show();
                    });
            }).fail(function () {
                alert("We're sorry, we ran into an error with that.  If the problem persists please contact your administrator.");
            });
        }
    };

    window.HoursEditShowPopup = function (apptID) {

        var providerID = page.provider.id;

        $.ajax({
            url: '/HoursEntry/GetEntryOrPreEntryVM',
            type: 'GET',
            data: {
                hoursID: apptID,
                providerID: providerID
            },
            success: function (data) {

                console.log(data);

                $.ajax({
                    url: '/HoursEntry/GetEntryHtml',
                    type: 'GET',
                    success: function (html) {
                        $('#hours-entry-container-pp').empty().append(html);
                        var cancelCallback = function () {
                            App.DevEx.GetControl("pc-hours-entry").Hide();
                        };
                        var successCallback = function () {
                            App.DevEx.GetControl("pc-hours-entry").Hide();
                            ReloadCalendar();
                            RefreshHoursLog();
                            RefreshPendingHoursCount();
                        };
                        var deleteCallback = function () {
                            HoursDeleteSubmitPopup(apptID);
                            App.DevEx.GetControl("pc-hours-entry").Hide();
                            ReloadCalendar();
                            RefreshHoursLog();
                            RefreshPendingHoursCount();
                        };
                        var vm = new window.aabc.hoursEntry.HoursEntryVM(data, cancelCallback, successCallback, deleteCallback);
                        var root = document.getElementById('hours-entry-container-pp');
                        ko.cleanNode(root);
                        ko.applyBindings(vm, root);
                        App.DevEx.GetControl("pc-hours-entry").Show();
                    }
                });
            },
            error: function () {
                alert("We're sorry, we ran into an error with that.  If the problem persists please contact your administrator.");
            }
        });
    };

    window.HoursDeleteSubmitPopup = function (hoursID) {
        if (confirm("Are you sure you want to delete this entry?")) {
            $.ajax({
                url: "/Home/CaseHoursDelete",
                type: 'POST',
                data: {
                    hoursID: hoursID
                },
                success: function (res) {
                    if (res == "ok") {
                        ReloadCalendar();
                        RefreshHoursLog();
                        RefreshPendingHoursCount();
                    } else {
                        App.Dialogs.Error();
                    }
                },
                error: function () {
                    App.Dialogs.Error();
                }
            });
        }
    };

    window.toggleInstructions = function () {
        var state = $("#instructionsToggleState");
        if (state.text() == "show") {
            state.text("hide");
            $("#InstructionsContainer").show(200);
        } else {
            state.text("show");
            $("#InstructionsContainer").hide(200);
        }
    };

    window.hoursEntryApptDoubleClick = function (s, e) {
        HoursEditShowPopup(e.appointmentId);
        e.handled = true;
    };

    window.ReloadCalendar = function () {
        CaseHoursEntryScheduler.Refresh();
    };

    window.CaseHoursEntrySchedulerBeginCallback = function (s, e) {
        e.customArgs["caseID"] = getCurrentCaseID();
    };

    window.isDblClickInitialized = false;

    window.initCaseHoursEntryCalendar = function () {
        RefreshPendingHoursCount();

        window.setTimeout(function () {
            if (window.isDblClickInitialized) {
                return;
            }
            window.isDblClickInitialized = true;
            var els = $("[id^=CaseHoursEntryScheduler_containerBlock_DXCnth]");
            $(document).on('dblclick', els, function (e) {
                var hitTest = CaseHoursEntryScheduler.CalcHitTest(e);
                var apptDiv = undefined;
                try {
                    apptDiv = hitTest.appointmentDiv;
                } catch (err) {
                    console.log(err);
                }
                if (apptDiv == undefined) {
                    // this is a date selection
                    var cell = hitTest.cell;
                    if (cell != null) {
                        try {
                            var clickedDate = hitTest.cell.interval.start;
                            if (clickedDate > new Date()) {
                                App.Dialogs.Error("Hours may only be entered for dates in the past.");
                                return;
                            }
                            HoursCreateShowPopup(clickedDate);
                        } catch (err) {
                            console.log(err);
                        }
                    }
                } else {
                    // this is an appointment selection
                    // let the appointment double click event handle this
                }
            });
        }, 500);
    };

    window.CaseHoursLogEndCallback = function (s, e) {
        setHoursLogConfig();
    };

    window.CaseHoursLogBeginCallback = function (s, e) {
        e.customArgs["caseID"] = getCurrentCaseID();
        try {
            e.customArgs["showAll"] = App.DevEx.GetControl("hoursLogShowAllHours").GetValue();
        } catch (err) {
            e.customArgs["showAll"] = false;
        }
    };

    window.RefreshHoursLog = function () {
        var caseID = getCurrentCaseID();
        var showAll = false;
        try {
            showAll = App.DevEx.GetControl("hoursLogShowAllHours").GetValue();
        } catch (err) {
            // nothing
        }
        $("#CaseHoursLogsContainer").hide(100, function () {
            App.LoadingPanel.AutoPanel.SupressSingle();
            $.ajax({
                url: "/Home/CaseHoursLogs",
                type: 'GET',
                data: { caseID: caseID, showAll: showAll },
                success: function (r) {
                    $("#CaseHoursLogsContainer").empty().append(r).show();
                    setHoursLogConfig();
                },
                error: function () {
                    App.Dialogs.Error();
                }
            });
        });
    };

    window.ToggleLogShowAllProviders = function () {
        RefreshHoursLog();
    };

    window.getCurrentCaseID = function () {
        var visibleIndex = gridCases.GetFocusedRowIndex();
        var id = gridCases.GetRowKey(visibleIndex);
        return id;
    };

})();
