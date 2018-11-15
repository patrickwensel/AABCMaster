(function () {

    window.aabc = window.aabc || {};
    window.aabc.home = window.aabc.home || {};
    window.aabc.home.hoursEligibility = window.aabc.home.hoursEligibility || new HoursEligibility();

    function HoursEligibility() {

        var self = this;

        self.initialize = function (model) {
            self.activeInsuranceIssues(model.ActiveInsuranceIssues);
            self.activeAuthorizationIssues(model.ActiveAuthorizationIssues);
            self.activeRuleIssues(model.ActiveRuleIssues);
            console.log(ko.toJS(self));
        }

        self.activeInsuranceIssues = ko.observableArray([]);
        self.activeAuthorizationIssues = ko.observableArray([]);
        self.activeRuleIssues = ko.observableArray([]);


        self.hasActiveInsuranceItems = ko.computed(function () {
            return self.activeInsuranceIssues().length > 0;
        });
        self.hasActiveAuthItems = ko.computed(function () {
            return self.activeAuthorizationIssues().length > 0;
        });
        self.hasAuthsWithoutRules = ko.computed(function () {
            return self.activeRuleIssues().length > 0;
        });



        return self;



    }


})()