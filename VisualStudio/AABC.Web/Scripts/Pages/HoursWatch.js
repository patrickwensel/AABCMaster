// Scripts/Pages/HoursScrub.js

var HoursWatch = {

    Initialize: function () {

        //HoursWatch.dateSelector.init();

        HoursWatch.refreshAllGrids();


    },  // end HoursScrub.Initialize


    refreshAllGrids: function () {

        var d = HoursWatch.dateSelector.getValue();

        //$('#hours-watch-grids-loading').show(250);

        HoursWatch.missingBcba.showGrid(d, null);
        HoursWatch.missingAide.showGrid(d, null);
        HoursWatch.noSupervision.showGrid(d, null);
        HoursWatch.noHours.showGrid(d, null);
        HoursWatch.noBilledHours.showGrid(d, null);

        /*HoursWatch.missingBcba.showGrid(d, function () {
            HoursWatch.missingAide.showGrid(d, function () {
                HoursWatch.noSupervision.showGrid(d, function () {
                    HoursWatch.noHours.showGrid(d, function () {
                        $('#hours-watch-grids-loading').hide(250);
                    });
                });
            });
        });*/
        
    },
    
    missingBcba: {

        showGrid: function (period, callback) {

            console.log('prepping bcba');

            $.ajax({
                type: 'GET',
                url: '/Hours/Watch/Grid/MissingBCBA',
                data: {
                    period: period.toISOString()
                },
                success: function (res) {
                    console.log('showing bcba');
                    $("#missing-bcba-grid-container").empty().append(res);
                    if (typeof callback == "function") {
                        callback();
                    }
                }
            });

        }

    },

    missingAide: {

        showGrid: function (period, callback) {

            console.log('prepping aides');

            $.ajax({
                type: 'GET',
                url: '/Hours/Watch/Grid/MissingAide',
                data: {
                    period: period.toISOString()
                },
                success: function (res) {
                    console.log('showing aides');
                    $("#missing-aide-grid-container").empty().append(res);
                    if (typeof callback == "function") {
                        callback();
                    }
                }
            });

        }

    },

    noSupervision: {

        showGrid: function (period, callback) {

            console.log('prepping nosupers');

            $.ajax({
                type: 'GET',
                url: '/Hours/Watch/Grid/NoSupervision',
                data: {
                    period: period.toISOString()
                },
                success: function (res) {
                    console.log('showing nosupers');
                    $("#no-supervision-grid-container").empty().append(res);
                    if (typeof callback == "function") {
                        callback();
                    }
                }
            });

        }

    },

    noHours: {

        showGrid: function (period, callback) {

            console.log('prepping nohours');

            $.ajax({
                type: 'GET',
                url: '/Hours/Watch/Grid/NoHours',
                data: {
                    period: period.toISOString()
                },
                success: function (res) {
                    console.log('showing nohours');
                    $("#no-hours-grid-container").empty().append(res);
                    if (typeof callback == "function") {
                        callback();
                    }
                }
            });

        }

    },

    noBilledHours: {
        showGrid: function (period, callback) {
            $.ajax({
                type: 'GET',
                url: '/Hours/Watch/Grid/NoBilledHours',
                data: {
                    period: period.toISOString()
                },
                success: function (res) {
                    $("#no-billed-hours-grid-container").empty().append(res);
                    if (typeof callback == "function") {
                        callback();
                    }
                }
            });
        }
    },

    dateSelector: {

        getValue: function () {
            var dateSelectorTmp = App.DevEx.GetControl("SelectedDate");
            if (dateSelectorTmp == null) {
                var curDate = new Date();
                if (curDate.getDate() <= 10) {
                    if (curDate.getMonth() == 0) {
                        curDate.setMonth(11);
                        curDate.setFullYear(curDate.getFullYear() - 1);
                    } else {
                        curDate.setMonth(curDate.getMonth() - 1);
                    }
                }

                return new Date(curDate.getFullYear(), curDate.getMonth());
            }

            return App.DevEx.GetControl("SelectedDate").GetValue();
        },

        init: function() {

            App.DevEx.GetControl("SelectedDate").ValueChanged.AddHandler(function (s, e) {
                HoursWatch.refreshAllGrids();
            });

        }

    },

    gridViewDoubleClick: function (s, e) {
        var id = s.keys[e.visibleIndex];
        var caseName = "[Case Name]";

        var selectedDate = App.DevEx.GetControl("SelectedDate").GetValue();

        App.Popup.Show({
            url: "/Hours/WatchDetail",
            data: {
                caseID: id,
                month: selectedDate.getMonth() + 1,
                year: selectedDate.getFullYear()
            },
            options: {
                width: 550,
                height: 370,
                title: "Providers",
                allowDrag: true,
                allowResize: false
            },
            opened: function () {
                // doesn't seem to work, made explicit call from popup instead
            },
            done: function (r) {

            },
            error: function (r) {
                App.Dialogs.Error();
            }
        });

    },

    watchDetailSave: function () {

        if (App.DevEx.GetControl("Ignore").GetValue()) {
            if (!confirm("Are you sure you want to ignore this case? It will be removed from all watch lists for this month.")) {
                return;
            }
        }

        $.ajax({
            type: "POST",
            url: "/Hours/SaveWatchDetail",
            data: $("#WatchDetailForm").serialize(),
            success: function (data) {
                App.Popup.Hide();
                HoursWatch.refreshAllGrids();
            }
        });

    }
    
}   // end HoursScrub





