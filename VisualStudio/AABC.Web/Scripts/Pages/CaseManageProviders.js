var gFormID = "form-default";
var ctlFirstName = null;
var ctlLastName = null;
var ctlEmail = null;

function initComponent() {
    caseProviders.initialize();
}

var caseProviders = {

    initialize: function () {

        console.log('beining initialize caseProviders');

        caseProviders.grid.initialize();

    },  // end caseProviders.initialize

    grid: {

        initialize: function() {

            console.log('beining initialize caseProviders Grid');

            var dxGrid = App.DevEx.GetControl("gvProviders");
            
            caseProviders.grid.initEndCallback(dxGrid);
            caseProviders.grid.initRowDoubleClick(dxGrid);

            App.DevEx.GetControl("cmProvidersGrid").ItemClick.AddHandler(function (s, e) {
                console.log('dx object click handler initiated');
                caseProviders.grid.contextMenu.itemClick(s, e);
            });


        },   // end caseProviders.grid.initialize

        initEndCallback: function(dxGrid) {
            console.log('initilaizing grid callback');
            dxGrid.EndCallback.ClearHandlers();
            dxGrid.EndCallback.AddHandler(function (s, e) {
                console.log('grid callback');
                caseProviders.grid.initRowDoubleClick(s);
            });

        },  // end caseProviders.grid.initEndCallback

        initRowDoubleClick: function(dxGrid) {

            console.log('beining initialize caseProviders grid row double click');

            $("[data-dym-cellClick='gvProviders']").dblclick(function () {
                var cell = $(this);
                var index = cell.attr("data-dym-visibleIndex");
                var field = cell.attr("data-dym-fieldName");
                var id = dxGrid.GetRowKey(index);

                if (field == "Active"
                    || field == "IsSupervisor"
                    || field == "IsAssessor"
                    || field == "IsAuthorizedBCBA") {

                    App.LoadingPanel.AutoPanel.SupressSingle();
                    $.ajax({
                        type: 'POST',
                        url: '/Case/ToggleProviderCheckbox',
                        data: {
                            caseProviderID: id,
                            fieldName: field
                        },
                        success: function (res) {
                            if (res == "ok") {
                                App.LoadingPanel.AutoPanel.SupressSingle();
                                dxGrid.Refresh();
                            } else {
                                App.Dialogs.Error("Unable to make change.  Ensure that the role is applied only to a BCBA, and that there's only one assessor, supervisor or authorized BCBA per active date range.");
                            }                            
                        }
                    });
                }

                if (field == "StartDate") {

                    App.LoadingPanel.AutoPanel.SupressSingle();

                    dxGrid.GetRowValues(index, "ID;StartDate", function (values) {
                        var caseProviderID = values[0];
                        var existingDate = values[1];

                        App.LoadingPanel.AutoPanel.SupressSingle();
                        App.Selectors.GetDate(existingDate, function (selectedDate) {
                            var newDate = null;
                            if (selectedDate != null) {
                                newDate = selectedDate.toISOString();
                            }
                            console.log('provider start date: ' + newDate);
                            App.LoadingPanel.AutoPanel.SupressSingle();
                            $.ajax({
                                type: 'POST',
                                url: '/Case/SetProviderStartDate',
                                data: {
                                    caseProviderID: caseProviderID,
                                    newDate: newDate
                                },
                                success: function () {
                                    App.LoadingPanel.AutoPanel.SupressSingle();
                                    dxGrid.Refresh();
                                }
                            });
                        });
                    });

                }

                if (field == "EndDate") {

                    App.LoadingPanel.AutoPanel.SupressSingle();

                    dxGrid.GetRowValues(index, "ID;EndDate", function (values) {
                        var caseProviderID = values[0];
                        var existingDate = values[1];

                        App.Selectors.GetDate(existingDate, function (selectedDate) {
                            var newDate = null;
                            if (selectedDate != null) {
                                newDate = selectedDate.toISOString();
                            }
                            $.ajax({
                                type: 'POST',
                                url: '/Case/SetProviderEndDate',
                                data: {
                                    caseProviderID: caseProviderID,
                                    newDate: newDate
                                },
                                success: function () {
                                    App.LoadingPanel.AutoPanel.SupressSingle();
                                    dxGrid.Refresh();
                                }
                            });
                        });
                    });
                }
                

            });
            
        },  // end caseProviders.grid.initRowDoubleClick



        contextMenu: {

            itemClick: function (s, e) {

                console.log('clientscript click handler initiated');

                if (e.item.name == "cmProvidersGridItemGotoProvider") {

                    gvProviders.GetRowValues(gvProviders.GetFocusedRowIndex(), "ID", function (value) {
                        App.Navigate("/Providers/Edit/" + value);
                    });
                }

            }

        }   // end caseProviders.grid.contextMenu


    }   // end caseProviders.grid

}   // end caseProviders
