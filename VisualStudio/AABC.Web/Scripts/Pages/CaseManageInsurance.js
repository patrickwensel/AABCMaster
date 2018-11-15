(function () {
	
    var taskCount = 1;

    var insurance = {
        initialize: function (config) {            
            insurance.config = $.extend(true, new insurance.Config(), config);
            insurance.tasks.grid.initialize();            
        },
        config: null,
        Config: function() {
            return {
                caseID: null,
                insuranceID: null,
                caseInsuranceId: null
            }
        },

        tasks: {

            grid: {

                name: 'CaseInsuranceEditGrid',

                initialize: function () {

                    var grid = App.DevEx.GetControl(insurance.tasks.grid.name);
                    var events = insurance.tasks.grid.events;

                    grid.BeginCallback.AddHandler(function (s, e) { events.beginCallback(s, e); });
                    grid.EndCallback.AddHandler(function (s, e) { events.endCallback(s, e); });

                    
                    $("[data-dym-cellClick='CaseInsuranceEditGrid']").dblclick(function () {
                        var cell = $(this);
                        var index = cell.attr("data-dym-visibleIndex");
                        var field = cell.attr("data-dym-fieldName");
                        var id = grid.GetRowKey(index);
                        insurance.get(insurance.config.caseID, id);
                        insurance.Config.caseInsuranceId = id;
                    });

                },  // end insuranceEdit.tasks.grid.initialize

                events: {

                    beginCallback: function (s, e) {
                        e.customArgs["caseID"] = insurance.config.caseID;
                        e.customArgs["insuranceID"] = insurance.config.insuranceID;
                    },
                    endCallback: function (s, e) {
                        insurance.tasks.initializeButtons();
                    }

                }   // end insurance.tasks.grid.events

            },  // end insurance.tasks.grid

            currentId: function() {
                var grid = App.DevEx.GetControl(insurance.tasks.grid.name);
                return App.DevEx.GridView.GetFocusedRowKey(grid);
            },

            add: function () {
                var grid = App.DevEx.GetControl(insurance.tasks.grid.name);
                grid.AddNewRow();

            },  // end insurance.tasks.add

            edit: function (id) {

                alert('editing');

            },  // end insurance.tasks.edit

            remove: function (id) {

                alert('removing');

            },   // end insurance.tasks.remove

            submit: function (model) {

            }   // end insurance.tasks.submit

        },   // end insurance.tasks
        getGrid: function () {
            $.ajax({
                type: "GET",
                url: "/Case/InsuranceGrid?CaseId=" + insurance.config.caseID,
                success: function (data) {
                    $("#InsuranceGridContainer").html(data);
                    insurance.tasks.grid.initialize();
                }
            })
        },
        get: function (CaseId, InsuranceId) {
            $.ajax({
                type: "GET",
                url: "/Case/InsuranceForm?CaseId=" + CaseId + "&CaseInsuranceId=" + InsuranceId,
                success: function (data) {
                    $("#InsuranceFormContainer").html(data);
                    console.log('insurance form loaded');

                    App.DevEx.GetControl("InsuranceId").SelectedIndexChanged.AddHandler(function (s, e) {
                        console.log('selected index changed');
                        App.DevEx.GetControl("InsuranceCarrierID").PerformCallback();
                    });

                    App.DevEx.GetControl("InsuranceCarrierID").BeginCallback.AddHandler(function (s, e) {
                        console.log('beginning callback');
                        e.customArgs["InsuranceID"] = App.DevEx.GetControl("InsuranceId").GetValue();
                    });

                    App.DevEx.GetControl("InsuranceCarrierID").PerformCallback();

                    insurance.MaxOutOfPocket.initialize();
                }
            })
        },
        //save: function () {

        //    $.ajax({
        //        type: "POST",
        //        url: "/Case/SaveInsurance",
        //        data: $("#InsuranceForm").serialize(),
        //        success: function (data) {
        //            $("#InsuranceFormContainer").html(data);
        //            insurance.getGrid();
        //        },
        //        error: function () {
        //            alert('Please make sure the insurance dates do not overlap an existing insurance.  Note if no date is specified, it will be considered infinite.');
        //            return;
        //        }
        //    })
        //},




        MaxOutOfPocket: {
            initialize: function(){
                var grid = App.DevEx.GetControl("CaseInsuranceMaxOutOfPocketEditGrid");
                

                $("[data-dym-cellClick='CaseInsuranceMaxOutOfPocketEditGrid']").dblclick(function () {
                    var cell = $(this);
                    var index = cell.attr("data-dym-visibleIndex");
                    var field = cell.attr("data-dym-fieldName");
                    var id = grid.GetRowKey(index);
                    insurance.MaxOutOfPocket.get(id);
                });
            },
            get: function (id) {
                App.Popup.Show({
                    url: '/Case/CaseInsuranceMaxOutOfPocketForm?CaseInsuranceId=' + insurance.Config.caseInsuranceId + '&CaseInsuranceMaxOutOfPocketId=' + id,
                    options: {
                        width: 200,
                        height: 150,
                        title: "Max Out Of Pocket"
                    },
                    finished: function (response) {

                        insurance.MaxOutOfPocket.refresh();
                    },
                    error: function (response) {
                        App.Dialogs.Error();
                    }
                });
            },
            save: function () {
                var m = {
                    Id: $("#CaseInsuranceMaxOutOfPocketForm input[name='Id']").val(),
                    CaseInsuranceId: $("#CaseInsuranceMaxOutOfPocketForm input[name='CaseInsuranceId']").val(),
                    MaxOutOfPocket: $("#CaseInsuranceMaxOutOfPocketForm input[name='CaseInsuranceMaxOutOfPocket']").val(),
                    EffectiveDate: $("#CaseInsuranceMaxOutOfPocketForm input[name='CaseInsuranceEffectiveDate']").val(),
                };
                $.ajax({
                    type: "POST",
                    url: "/Case/SaveCaseInsuranceMaxOutOfPocket",
                    dataType: "json",
                    data: m,
                    success: function (data) {
                        insurance.MaxOutOfPocket.refresh();
                        
                    }
                })
            },
            refresh: function () {
                $.ajax({
                    type: "GET",
                    url: "/Case/CaseInsuranceMaxOutOfPocketGrid?CaseInsuranceId=" + insurance.Config.caseInsuranceId,
                    success: function (data) {
                        $('#CaseInsuranceMaxOutOfPocketContainer').html(data);
                        insurance.MaxOutOfPocket.initialize();
                    }
                })
            },
            delete: function (id) {
                if (confirm("Are you sure you want to delete this record?")) {
                    $.ajax({
                        type: "DELETE",
                        url: "/Case/CaseInsuranceMaxOutOfPocket/" + id,
                        success: function (data) {
                            $('#CaseInsuranceMaxOutOfPocketContainer').html(data);
                            insurance.MaxOutOfPocket.initialize();
                        }
                    })

                }
                
            }
        }
    }   // end insurance

    if (window.Case == null) {
        window.Case = { Insurance: insurance };
    } else {
        window.Case.Insurance = insurance;
    }
	

})();