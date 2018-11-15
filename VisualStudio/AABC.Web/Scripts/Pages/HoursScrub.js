// Scripts/Pages/HoursScrub.js

var HoursScrub = {

    Initialize: function () {
        HoursScrub.ScrubGrid.Initialize("gvCaseHoursScrubOverviewGrid");
    },  // end HoursScrub.Initialize

    UnfinalizedProviders: {

        Download: function () {

            var d = SelectedDate.GetValue();
            var win = window.open("/Hours/DownloadUnfinalizedProviders/?period=" + d.toISOString(), "_blank");
            win.focus();

        }   // end HoursScrub.UnfinalizedProviders.Download

    },  // end HoursScrub.UnfinalizedProviders

    
    ScrubGrid: {

        Object: null,
        ObjectName: null,

        Initialize: function(gridName) {
            HoursScrub.ScrubGrid.Object = App.DevEx.GetControl(gridName);
            HoursScrub.ScrubGrid.ObjectName = gridName;
            HoursScrub.ScrubGrid.TargetDate.Object = App.DevEx.GetControl("SelectedDate");
            HoursScrub.ScrubGrid.InitRowHover();
        },

        TargetDate: {
            GetValue: function() {
                return SelectedDate.GetValue();
            },
            AfterUpdate: function () {
                window.setTimeout(function () {
                    gvCaseHoursScrubOverviewGrid.PerformCallback();
                }, 250);
            },
            Object: null
        },  // end HoursScrub.ScrubGrid.TargetDate

        BeginCallback: function (s, e) {
            e.customArgs["targetDate"] = SelectedDate.GetValue().toISOString();
        },

        EndCallback: function (s, e) {
            //App.Content.GridViews.StretchHeight(s)
            HoursScrub.ScrubGrid.Initialize("gvCaseHoursScrubOverviewGrid");
        },

        PeriodExport: {

            Preview: function () {
                var d = HoursScrub.ScrubGrid.TargetDate.GetValue();

                var win = window.open("/Hours/MBHBillingPeriodPreview/?period=" + d.toISOString(), "_blank");
                win.focus();
            },

            CommitAndExport: function() {
                var d = HoursScrub.ScrubGrid.TargetDate.GetValue();
                var win = window.open("/Hours/MBHBillingPeriodCommitAndExport/?period=" + d.toISOString(), "_blank");
                win.focus();
            }

        },

        InitRowHover: function () {

            var rowElements = $("#" + HoursScrub.ScrubGrid.ObjectName + " [class*=dxgvDataRow_]");
            
            //rowElements.mouseover(function(event) {
            //    var index = getIndex(this);
            //    App.DevEx.GetControl("hoursScrubSummaryPopup").ShowAtPos(event.clientX, event.clientY);
            //});

            //rowElements.mouseleave(function (event) {
            //App.DevEx.GetControl("hoursScrubSummaryPopup").Hide();
            //});

            rowElements.on("contextmenu", function (e) {

                var index = getIndex(this);
                var caseID = App.DevEx.GetControl(HoursScrub.ScrubGrid.ObjectName).GetRowKey(index);

                showSummary(e, caseID);
                
                return false;
            });

            
            function getIndex(element) {
                var id = $(element).attr('id');
                return id.replace("gvCaseHoursScrubOverviewGrid_DXDataRow", "");
            }

            function showSummary(event, caseID) {

                $.ajax({
                    type: 'GET',
                    url: '/Hours/GetScrubListItemSummary',
                    data: {
                        caseID: caseID,
                        selectedPeriod: HoursScrub.ScrubGrid.TargetDate.GetValue().toISOString()
                    },
                    success: function(res) {
                        $("#hoursScrubSummaryPopupContent").empty().append(res);
                        App.DevEx.GetControl("hoursScrubSummaryPopup").ShowAtPos(event.clientX, event.clientY);
                    }
                });

            }

        }

    }   // end HoursScrub.ScrubGrid


}   // end HoursScrub



