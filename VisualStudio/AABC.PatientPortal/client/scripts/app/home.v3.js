


(function() {

    var api = {
        HomeViewModel: function(model) { return HomeViewModel(model); }
    }

    function HomeViewModel(model) {

        var self = this;

        // MODEL BINDING
        self.model = model;
        self.hasSignature = ko.observable(model.HasSignature);
        self.activeChildID = ko.observable(model.ActiveChild);
        self.monthlyGroups = ko.observableArray(getMonthlyGroups(model.MonthlyGroups));
        self.patients = ko.observableArray(getPatients(model.Children));
        
        // COMPUTED
        self.hasMonthlyGroups = ko.pureComputed(function () {
            if (self.monthlyGroups().length > 0) {
                return true;
            } else {
                return false;
            }
        });

        self.activeChildName = ko.pureComputed(function() {
            var patients = self.patients();
            for (var i = 0; i < patients.length; i++) {
                if (patients[i].id == self.activeChildID()) {
                    return patients[i].name;
                }
            }
        });

        self.showChildName = ko.pureComputed(function () {
            if (self.patients().length > 1) {
                return true;
            } else {
                return false;
            }
        });

        self.changeChild = function () {
            
            $('#child-selector-modal').modal();

        }

        self.selectChild = function (id) {
            
            if (id == self.activeChildID()) {
                return;
            }

            window.location = "/Home/?currentPatientID=" + id;
        }

        // COMPLETE

        ko.applyBindings();

        return self;
        
        function getMonthlyGroups(groups) {
            var items = [];
            for (var i = 0; i < groups.length; i++) {
                items.push(new MonthlyGroupViewModel(groups[i], self));
            }
            return items;
        }

        function getPatients(patients) {
            var items = [];
            for (var i = 0; i < patients.length; i++) {
                items.push(new PatientViewModel(patients[i], self));
            }
            return items;
        }

    }

    function MonthlyGroupViewModel(model, parent) {

        var self = this;

        self.parent = parent;
        self.groupID = ko.observable(model.GroupID);
        self.MonthDate = ko.observable(model.MonthDate);
        self.hours = ko.observableArray(getHours(model.Hours));

        self.groupMonthDisplay = ko.pureComputed(function () {
            return moment(self.MonthDate()).format("MMMM 'YY");
        });

        self.isFullyApproved = ko.pureComputed(function () {
            var hours = self.hours();
            for (var i = 0; i < hours.length; i++) {
                if (hours[i].isApproved() == false) {
                    return false;
                }
            }
            return true;
        });

        self.canApprove = ko.pureComputed(function () {

            // ensure we're not already fully approved
            if (self.isFullyApproved()) {
                return false;
            }

            // ensure no pending issues reported
            if (self.hoursWithIssues().length > 0) {
                return false;
            }

            // ensure we have pending approvals
            if (self.hoursNotApproved().length == 0) {
                return false;
            }

            if (!self.parent.hasSignature()) {
                return false;
            }

            return true;

        });

        self.approveStatusMessage = ko.pureComputed(function () {

            if (self.isFullyApproved()) {
                return "";
            }

            if (!self.parent.hasSignature()) {
                return "Create a signature in Settings to approve hours"
            }

            if (self.hoursWithIssues().length > 0) {
                return "Cannot approve hours with open issues";
            }

            // default if not approvable
            if (self.canApprove() == false) {
                return "Hours must be approved between the 5th and 10th of the month";
            }

            // default no message
            return "";

        });

        self.hoursNotApproved = ko.pureComputed(function () {
            var hours = self.hours();
            var targets = [];
            for (var i = 0; i < hours.length; i++) {
                if (hours[i].isApproved() == false) {
                    targets.push(hours[i]);
                }
            }
            return targets;
        });

        self.hoursWithIssues = ko.pureComputed(function() {
            var hours = self.hours();
            var issues = [];
            for (var i = 0; i < hours.length; i++) {
                if (hours[i].isReported()) {
                    issues.push(hours[i]);
                }
            }
            return issues;
        });
        
        self.approveHours = function () {
            
            $.ajax({
                type: 'POST',
                url: '/Home/ApproveHours',
                data: {
                    patientID: self.parent.activeChildID(),
                    groupID: self.groupID()
                },
                success: function (res) {
                    var hours = self.hours();
                    for (var i = 0; i < hours.length; i++) {
                        hours[i].isApproved(true);
                    }
                }
            });

        }
        
        return self;
        
        function getHours(hours) {

            var items = [];

            for (var i = 0; i < hours.length; i++) {
                items.push(new HoursEntryViewModel(hours[i]));
            }

            return items;
        }

    }
    
    function HoursEntryViewModel(model) {

        var self = this;

        self.id = model.ID;
        self.date = model.Date;
        self.timeIn = model.TimeIn;
        self.timeOut = model.TimeOut;
        self.providerFirstName = model.ProviderFirstName;
        self.providerLastName = model.ProviderLastName;
        self.providerID = model.ProviderID;
        self.serviceID = model.ServiceID;
        self.service = model.Service;
        self.isApproved = ko.observable(model.IsApproved);
        //self.isApproved = ko.observable(false);
        self.isReported = ko.observable(model.IsReported);
        //self.isReported = ko.observable(false);

        self.timestampDisplay = ko.pureComputed(function () {
            var d = moment(self.date).format('dddd, MMM Do');
            var s = moment(self.timeIn).format('h:mm A');
            var e = moment(self.timeOut).format('h:mm A');
            return d + ' - ' + s + ' to ' + e;
        });

        self.detailDisplay = ko.pureComputed(function () {
            return self.providerLastName + ', ' + self.providerFirstName + ' - ' + self.service
        });

        self.isReportable = ko.pureComputed(function () {
            if (self.isApproved() == true) {
                return false;
            }
            if (self.isReported() == true) {
                return false;
            }
            return true;
        });
        
        self.reportHours = function () {

            $('#reportHoursTitle').text(self.timestampDisplay());
            $('#reportHoursDetail').text(self.detailDisplay());
            $('#reportHoursID').val(self.id);
            $('#reportHoursComment').val("");
            $('#reportHoursModal').modal('show');

            $('#reportHoursSubmitButton').click(function (e) {
                e.preventDefault();
                
                var submitButton = $("#reportHoursSubmitButton").ladda();
                submitButton.ladda("start");

                $.ajax({
                    type: 'post',
                    url: '/Home/ReportHours',
                    data: {
                        hoursID: self.id,
                        comment: $('#reportHoursComment').val()
                    },
                    success: function (res) {
                        submitButton.ladda("stop");
                        self.isReported(true);
                        $('#reportHoursModal').modal('hide');
                    },
                    error: function (res) {
                        alert("An error occurred.");
                        submitButton.ladda("stop");
                    }
                });

                return false;
            });

        }

        return self;

    }

    function PatientViewModel(model, parent) {

        var self = this;

        self.parent = parent;

        self.id = model.ID;
        self.name = model.Name;

        self.active = ko.pureComputed(function () {
            return self.id == parent.activeChildID();
        });

        return self;

    }



    window.aabc = window.aabc || {}
    window.aabc.home = api;

})();