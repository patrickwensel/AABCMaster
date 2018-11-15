(function () {

    var api = {
        PaymentViewModel: PaymentViewModel
    }

    function PaymentViewModel(model) {
        var self = this;
        self.step = ko.observable(1);
        self.configuration = model.configuration;
        self.paymentTypes = ko.observableArray(model.paymentTypes || []);
        self.months = ko.observableArray(model.months || []);
        self.years = ko.observableArray(model.years || []);
        self.frequencies = ko.observableArray(model.frequencies || []);
        self.patients = ko.observableArray(model.patients || []);
        self.data = {
            paymentType: ko.observable(),
            patientId: ko.observable(),
            amount: ko.observable(),
            oneTimePaymentDate: ko.observable(),
            recurringFrequency: ko.observable(),
            recurringDateStart: ko.observable(),
            recurringDateEnd: ko.observable(),
            cardHolder: ko.observable(),
            cardNumber: ko.observable(),
            cardExpiryMonth: ko.observable(),
            cardExpiryYear: ko.observable(),
            cardSecurityCode: ko.observable(),
        };
        self.data.paymentType.extend({ required: true });
        self.data.patientId.extend({ required: true });
        self.data.amount.extend({ required: true, minFormatted: 1, digit: true });
        self.data.oneTimePaymentDate.extend({
            required:
                {
                    onlyIf: function () { return self.data.paymentType() == 0; }
                },
            date: true
        });
        self.data.recurringFrequency.extend({
            required:
                {
                    onlyIf: function () { return self.data.paymentType() == 1; }
                }
        });
        self.data.recurringDateStart.extend({
            required:
                {
                    onlyIf: function () { return self.data.paymentType() == 1; }
                },
            date: true
        });
        self.data.recurringDateEnd.extend({
            required:
                {
                    onlyIf: function () { return self.data.paymentType() == 1; }
                },
            date: true
        });

        self.data.cardHolder.extend({ required: true });
        self.data.cardNumber.extend({ required: true, digit: true });
        self.data.cardExpiryYear.extend({ cardExpiry: self.data.cardExpiryMonth });
        self.data.cardSecurityCode.extend({ required: true, digit: true });
        ko.mapping.fromJS(model.data, {}, self.data);

        self.shouldOneTimeTransactionTimeWindowWarningBeShown = ko.pureComputed(function () {
            var date = self.data.oneTimePaymentDate();
            return date && moment(date).isAfter(moment().add(self.configuration.oneTimeTransactionTimeWindowWarning, "days"), "day");
        });

        self.paymentTypeStr = ko.pureComputed(function () {
            var d = self.data.paymentType();
            var match = ko.utils.arrayFirst(self.paymentTypes(), function (item) {
                return item.value == d;
            });
            return match ? match.text : "Unknown";
        });

        self.patientName = ko.pureComputed(function () {
            var d = self.data.patientId();
            var match = ko.utils.arrayFirst(self.patients(), function (item) {
                return item.value == d;
            });
            return match ? match.text : "Unknown";
        });

        self.frequencyStr = ko.pureComputed(function () {
            var d = self.data.recurringFrequency();
            var match = ko.utils.arrayFirst(self.frequencies(), function (item) {
                return item.value == d;
            });
            return match ? match.text : "Unknown";
        });

        self.oneTimePaymentDatepickerOptions = {
            format: "yyyy-mm-dd",
            startDate: moment().format('YYYY-MM-DD'),
            endDate: moment().add(self.configuration.oneTimeTransactionTimeWindow, 'days').format('YYYY-MM-DD')
        };
        self.recurringDatesDatepickerOptions = {
            format: "yyyy-mm-dd",
            startDate: moment().format('YYYY-MM-DD'),
            endDate: moment().add(self.configuration.recurringTransactionTimeWindow, 'months').format('YYYY-MM-DD'),
        };
        self.hasErrorsOnStep1 = ko.pureComputed(function () {
            var r = ko.validation.group([
                self.data.paymentType,
                self.data.patientId,
                self.data.amount,
                self.data.oneTimePaymentDate,
                self.data.recurringFrequency,
                self.data.recurringDateStart,
                self.data.recurringDateEnd
            ]);
            return ko.unwrap(r).length > 0;
        });
        self.hasErrorsOnStep2 = ko.pureComputed(function () {
            var r = ko.validation.group([
                self.data.cardHolder,
                self.data.cardNumber,
                self.data.cardExpiryMonth,
                self.data.cardExpiryYear,
                self.data.cardSecurityCode,
            ]);
            return ko.unwrap(r).length > 0;
        });

        self.gotoStepTwo = function () {
            if (!self.hasErrorsOnStep1()) {
                self.step(2);
            }
        };

        self.pay = function () {
            var data = ko.mapping.toJS(self.data);
            $.ajax({
                url: "/Payments/SavePayment",
                type: "POST",
                data: data
            }).done(function (r) {
                if (r.message) {
                    alert(r.message);
                }
                if (r.success) {
                    if (data.paymentType == 0 && r.paymentChargeId) {
                        location.href = "/payments/history";
                    } else {
                        location.href = "/payments/scheduled";
                    }
                }
            }).fail(function (err) {
                alert(err.ErrorMessage);
            });
        };
        return self;
    }

    window.aabc = window.aabc || {}
    window.aabc.payment = api;
})();