(function () {

    var api = {
        ScheduledPaymentListVM: ScheduledPaymentListVM
    }

    function ScheduledPaymentVM(data) {
        var self = {};
        self.id = ko.observable();
        self.paymentType = ko.observable();
        self.scheduledDate = ko.observable();
        self.amount = ko.observable();
        self.paymentChargeId = ko.observable();

        self.update = function (data) {
            ko.mapping.fromJS(data, {
                key: function (data) {
                    return ko.unwrap(data.id);
                },
            }, self);
        };

        if (data) {
            self.update(data);
        }

        return self;
    }

    function PaymentVM(data) {
        var self = {};
        self.id = ko.observable();
        self.active = ko.observable();
        self.patientName = ko.observable();
        self.recurringFrequency = ko.observable();
        self.schedules = ko.observableArray([]);

        self.update = function (data) {
            ko.mapping.fromJS(data, {
                key: function (data) {
                    return ko.unwrap(data.id);
                },
                schedules: {
                    create: function (options) {
                        return new ScheduledPaymentVM(options.data);
                    }
                }
            }, self);
        };

        if (data) {
            self.update(data);
        }
        return self;
    }

    function ScheduledPaymentListVM() {
        var self = this;
        self.payments = ko.observableArray([]);
        self.update = function (data) {
            ko.mapping.fromJS(data, {
                key: function (data) {
                    return ko.unwrap(data.id);
                },
                create: function (options) {
                    return new PaymentVM(options.data);
                }
            }, self.payments);
        };
        self.stopScheduled = function (payment) {
            app.confirm("Are you sure you want to stop this payment?"
                , function () {
                    $.ajax({
                        url: "/Payments/StopScheduled",
                        type: "POST",
                        data: { id: payment.id() }
                    }).done(function (r) {
                        if (r.message) {
                            alert(r.message);
                        }
                        if (r.success) {
                            getScheduledPayments();
                        }
                    }).fail(function (err) {
                        alert(err.ErrorMessage);
                    });
                });
        }


        function getScheduledPayments() {
            $.ajax({
                url: "/payments/getScheduledPayments",
                type: "GET",
                contentType: "application/json",
                async: false,
                success: function (data) {
                    self.update(data);
                }
            });
        }
        getScheduledPayments();
        return self;
    }

    window.aabc = window.aabc || {}
    window.aabc.payment = api;

})();