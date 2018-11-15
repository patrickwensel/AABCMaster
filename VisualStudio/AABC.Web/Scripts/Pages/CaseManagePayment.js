(function () {
	
    var taskCount = 1;

    var paymentPlan = {

        initialize: function (config) {
            
            paymentPlan.config = $.extend(true, new paymentPlan.Config(), config);

            //paymentPlan.tasks.initializeButtons();
            paymentPlan.tasks.grid.initialize();
            insurancePayment.initialize();

        },

        config: null,

        Config: function() {
            return {
                caseId: null,
                paymentPlanId: null
            }
        },

        tasks: {

            grid: {

                name: 'CasePaymentPlanEditGrid',

                initialize: function () {

                    var grid = App.DevEx.GetControl(paymentPlan.tasks.grid.name);
                    var events = paymentPlan.tasks.grid.events;

                    grid.BeginCallback.AddHandler(function (s, e) { events.beginCallback(s, e); });
                    grid.EndCallback.AddHandler(function (s, e) { events.endCallback(s, e); });

                    
                    $("[data-dym-cellClick='CasePaymentPlanEditGrid']").dblclick(function () {
                        var cell = $(this);
                        var index = cell.attr("data-dym-visibleIndex");
                        var field = cell.attr("data-dym-fieldName");
                        var id = grid.GetRowKey(index);
                        paymentPlan.get(paymentPlan.config.caseId, id);
                    });

                },  // end paymentPlanEdit.tasks.grid.initialize

                events: {

                    beginCallback: function (s, e) {
                        e.customArgs["caseId"] = paymentPlan.config.caseId;
                        e.customArgs["paymentPlanId"] = paymentPlan.config.paymentPlanId;
                    },
                    endCallback: function (s, e) {
                        paymentPlan.tasks.initializeButtons();
                    }

                }   // end paymentPlan.tasks.grid.events

            },  // end paymentPlan.tasks.grid

            currentId: function() {
                var grid = App.DevEx.GetControl(paymentPlan.tasks.grid.name);
                return App.DevEx.GridView.GetFocusedRowKey(grid);
            },

            add: function () {
                var grid = App.DevEx.GetControl(paymentPlan.tasks.grid.name);
                grid.AddNewRow();

            },  // end paymentPlan.tasks.add

            edit: function (id) {

                alert('editing');

            },  // end paymentPlan.tasks.edit

            remove: function (id) {

                alert('removing');

            },   // end paymentPlan.tasks.remove

            submit: function (model) {

            }   // end paymentPlan.tasks.submit

        },   // end paymentPlan.tasks
        getGrid: function () {
            $.ajax({
                type: "GET",
                url: "/Case/PaymentPlanGrid?CaseId=" + paymentPlan.config.caseId,
                success: function (data) {
                    $("#PaymentPlanGridContainer").html(data);
                    paymentPlan.tasks.grid.initialize();
                }
            })
        },
        get : function(CaseId, PaymentPlanId){
            $.ajax({
                type: "GET",
                url: "/Case/PaymentPlanForm?CaseId=" + CaseId + "&CasePaymentPlanId=" + PaymentPlanId,
                success: function (data) {
                    $("#PaymentPlanFormContainer").html(data);
                }
            })
        },
        save: function () {

            $.ajax({
                type: "POST",
                url: "/Case/SavePaymentPlan",
                data: $("#PaymentPlanForm").serialize(),
                success: function (data) {
                    $("#PaymentPlanFormContainer").html(data);
                    paymentPlan.getGrid();
                }
            })

        },
    }   // end paymentPlan

    var insurancePayment = {
        initialize: function () {

            var grid = App.DevEx.GetControl('CaseInsurancePaymentEditGrid');
            var events = paymentPlan.tasks.grid.events;

            grid.BeginCallback.AddHandler(function (s, e) { events.beginCallback(s, e); });
            grid.EndCallback.AddHandler(function (s, e) { events.endCallback(s, e); });


            $("[data-dym-cellClick='CaseInsurancePaymentEditGrid']").dblclick(function () {
                var cell = $(this);
                var index = cell.attr("data-dym-visibleIndex");
                var field = cell.attr("data-dym-fieldName");
                var id = grid.GetRowKey(index);
                insurancePayment.get(paymentPlan.config.caseId, id);
            });

        },
        get: function (CaseId, insurancePaymentId) {

            App.Popup.Show({
                url: "/Case/InsurancePaymentForm?CaseId=" + CaseId + "&InsurancePaymentId=" + insurancePaymentId,
                options: {
                    width: 200,
                    height: 150,
                    title: "Insurance Payment"
                },
                finished: function (response) {

                    insurancePayment.refresh();
                },
                error: function (response) {
                    App.Dialogs.Error();
                }
            });

        },
        save: function () {

            $.ajax({
                type: "POST",
                url: "/Case/InsurancePaymentSave",
                data: $("#CaseInsurancePaymentForm").serialize(),
                success: function (data) {
                    App.Popup.Hide();
                    insurancePayment.refresh();
                }
            })

        },
        refresh: function () {
            $.ajax({
                type: "GET",
                url: "/Case/InsurancePaymentGrid?CaseId=" + paymentPlan.config.caseId,
                success: function (data) {
                    $("#InsurancePaymentGridContainer").html(data);
                    insurancePayment.initialize();
                }
            })
        },
    }

    var correspondence = {
        initialize: function(){

        },
        getList: function () {
            $.ajax({
                type: "GET",
                url: "/Case/BillingCorrespondenceList?CaseId=" + paymentPlan.config.caseId,
                success: function (data) {
                    $("#BillingCorrespondenceListContainer").html(data);
                    correspondence.initialize();
                }
            })
        },
        get: function (CaseId, BillingCorrespondenceId) {
            $.ajax({
                type: "GET",
                url: "/Case/BillingCorrespondenceForm?CaseId=" + CaseId + "&BillingCorrespondenceId=" + BillingCorrespondenceId,
                success: function (data) {
                    $("#BillingCorrespondenceForm").html(data);
                }
            })
        },
        save: function () {

            $.ajax({
                type: "POST",
                url: "/Case/BillingCorrespondenceSave",
                data: $("#BillingCorrespondenceForm").serialize(),
                success: function (data) {
                    $("#BillingCorrespondenceForm").html(data);
                    correspondence.getList();
                }
            })

        },
    }

    if (window.Case == null) window.Case = {};
    if (window.Case.Payment == null) window.Case.Payment = {};
    if (window.Case.Payment.Plan == null) window.Case.Payment.Plan = paymentPlan;
    if (window.Case.Payment.Insurance == null) window.Case.Payment.Insurance = insurancePayment;
    if (window.Case.Payment.Correspondence == null) window.Case.Payment.Correspondence = correspondence;


})();