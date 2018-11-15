// Scripts/Pages/Payroll.js

var Payroll = {

    Initialize: function () {
        console.log('initializing');
        //Payroll.PayrollGrid.Object = App.DevEx.GetControl("gvPayrollOverviewGrid");
        Payroll.PayrollGrid.Object = gvPayrollOverviewGrid;
        Payroll.PayrollGrid.TargetDate.Object = App.DevEx.GetControl("SelectedDate");
    },  // end Payroll.Initialize


    ExportWithoutCommit: function () {
        if (gvPayrollOverviewGrid.GetVisibleRowsOnPage() === 0) {
            //App.Dialogs.MessageBox('No rows to export.', App.Dialogs.Icons.Warning);
            alert('No rows to export');
        } else {
            var win = window.open("/Providers/PayrollExportXlsx/?targetDate=" + SelectedDate.GetValue().toISOString() + "&commit=false&filter=" + SelectedFilter.GetValue(), "_blank");
            win.focus();
        }
        
        return false;
    },

    CommitAndExport: function () {

        if (gvPayrollOverviewGrid.GetVisibleRowsOnPage() === 0) {
            //App.Dialogs.MessageBox('No rows to export.', App.Dialogs.Icons.Warning);
            alert('No rows to export');
        } else {
            App.Dialogs.ConfirmDelete({
                message: "This action will finalize the payable hours and they will not be able to be changed after.  Continue?",
                confirmed: function () {
                    var win = window.open("/Providers/PayrollExportXlsx/?targetDate=" + SelectedDate.GetValue().toISOString() + "&commit=true&filter=" + SelectedFilter.GetValue(), "_blank");
                    win.focus();
                    gvPayrollOverviewGrid.Refresh();
                }
            });
        }

        
        return false;
    },


    PayrollGrid: {

        Object: null,

        TargetDate: {
            GetValue: function() {
                return SelectedDate.GetValue();
            },
            AfterUpdate: function () {
                window.setTimeout(function () {
                    gvPayrollOverviewGrid.PerformCallback();
                }, 500);
            },
            Object: null
        },  // end Payroll.PayrollGrid.TargetDate


        FilterSelection: {
            GetValue: function () {
                return SelectedFilter.GetValue();
            },
            AfterUpdate: function () {
                window.setTimeout(function () {
                    gvPayrollOverviewGrid.PerformCallback();
                }, 500);
            },
            Object: null
        },

        BeginCallback: function (s, e) {
            e.customArgs["targetDate"] = SelectedDate.GetValue().toISOString();
            e.customArgs["selectedFilter"] = SelectedFilter.GetValue();
        },

        EndCallback: function (s, e) {
            //App.Content.GridViews.StretchHeight(s)
            // reset the regenerated header control
            Payroll.PayrollGrid.TargetDate.Object = App.DevEx.GetControl("SelectedDate");
            Payroll.PayrollGrid.FilterSelection.Object = App.DevEx.GetControl("SelectedFilter");
        }

    }   // end Payroll.PayrollGrid


}   // end Payroll