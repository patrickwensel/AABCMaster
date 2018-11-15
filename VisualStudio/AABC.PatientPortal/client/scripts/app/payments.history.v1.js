(function () {

    var api = {
        PaymentChargeListVM: PaymentChargeListVM
    }

    function PaymentChargeVM(data) {
        var self = {};
        self.id = ko.observable();
        self.description = ko.observable();
        self.patientName = ko.observable();
        self.amount = ko.observable();
        self.chargeDate = ko.observable();
        self.isPatientGenerated = ko.observable();
        self.isPatientGeneratedStr = ko.pureComputed(function () {
            var isPatientGenerated = self.isPatientGenerated();
            return isPatientGenerated ? "This Account" : "AABC Office";
        });

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

    function PaymentChargeListVM(model) {
        var self = {};
        self.charges = ko.observableArray([]);
        self.update = function (data) {
            ko.mapping.fromJS(data, {
                key: function (data) {
                    return ko.unwrap(data.id);
                },
                create: function (options) {
                    return new PaymentChargeVM(options.data);
                }
            }, self.charges);
        };

        $.ajax({
            url: "/payments/getcharges",
            type: "GET",
            contentType: "application/json"
        }).done(function (data) {
            self.update(data);
        });

        return self;
    }

    window.aabc = window.aabc || {}
    window.aabc.payment = api;

})();