// reportSelector

(function () {

    var Interface = {

        Initialize: function () {
            dash.initialize();
        }

    }


    var dash = {

        initialize: function () {

            dash.costPerIns.initialize();

        },  // end dash.initialize



        costPerIns: {

            // costs by insurance company
            initialize: function () {

                App.DevEx.GetControl("DashboardInsuranceDateSelected").ValueChanged.AddHandler(function (s, e) {
                    dash.costPerIns.grid.refresh();
                });

                //App.DevEx.GetControl("DashboardInsuranceSelector").ValueChanged.AddHandler(function (s, e) {
                //    dash.costPerIns.grid.refresh();
                //});

                App.DevEx.GetControl("DashboardInsurance2Selector").ValueChanged.AddHandler(function (s, e) {
                    dash.costPerIns.grid.refresh();
                });

            },   // end dash.costPerIn./initialize
            

            grid: {

                refresh: function () {
                    
                    var period = App.DevEx.GetControl("DashboardInsuranceDateSelected").GetValue().toISOString();
                    var insurance = null; //App.DevEx.GetControl("DashboardInsuranceSelector").GetValue();
                    var insuranceID = App.DevEx.GetControl("DashboardInsurance2Selector").GetValue();


                    $.ajax({
                        type: 'get',
                        url: '/Home/DashboardYakovGetInsuranceCosts',
                        data: {
                            period: period,
                            insuranceName: insurance,
                            insuranceID: insuranceID
                        },
                        success: function (res) {
                            $("#dash-insurance-grid").empty().append(res);
                            dash.costPerIns.grid.initialize();
                        },
                        error: function () {
                            App.Dialogs.Error();
                        }
                    });


                },

                initialize: function () {

                    App.DevEx.GetControl("DashboardYakovInsCostGrid").BeginCallback.AddHandler(function (s, e) {
                        App.LoadingPanel.AutoPanel.SupressSingle();
                        e.customArgs["period"] = App.DevEx.GetControl("DashboardInsuranceDateSelected").GetValue().toISOString();
                        e.customArgs["insuranceName"] = null; //App.DevEx.GetControl("DashboardInsuranceSelector").GetValue();
                        e.customArgs["insuranceID"] = App.DevEx.GetControl("DashboardInsurance2Selector").GetValue();
                    });

                }
            }




        },   // end dash.costPerIns



    }   // end dash



    window.DBY = Interface;

})();