// client/app/hoursEntry/hoursEntry.js
(function () {

    function getInfoMessages(errors, warnings) {
        var infoMessages = [];
        if (errors) {
            for (var i = 0; i < errors.length; i++) {
                infoMessages.push(new InfoMessage(errors[i], "error"));
            }
        }
        if (warnings) {
            for (i = 0; i < warnings.length; i++) {
                infoMessages.push(new InfoMessage(warnings[i], "warning"));
            }
        }
        return infoMessages;
    }


    function setStatus(vm, valid, errors, warnings) {
        var infoMessages = getInfoMessages(errors, warnings);
        vm.isValid(valid);
        vm.setErrorMessages(infoMessages);
        vm.setWarningMessages(infoMessages);
    }


    function EntryValidator(vm, isOnAideLegacyMode) {
        var self = this;
        var dm = new window.aabc.hoursEntry.DateManager();

        self.warnings = [];
        self.errors = [];

        self.validateBoth = function () {
            resetMessages();
            var valid = self.validateLocal();
            if (valid) {
                self.validateServer(false);
            }
        };

        function resetMessages() {
            self.warnings = [];
            self.errors = [];
        }

        self.validateLocal = function () {
            var valid = true;
            resetMessages();

            // first do the date, because we'll need that for the insurance link
            var d = dm.getDate(vm.date());

            if (d === null) {
                self.errors.push("Date must be specified");
                valid = false;
            } else {
                var today = new Date();
                if (d > today) {
                    self.errors.push("Date must not be in the future.");
                    valid = false;
                }
            }

            // if we can't do insurance, we can't do anything else...
            // if valid is true, they have a date on file.
            // if they don't have a valid date on file, don't bother showing this
            if (valid) {
                var insuranceID = vm.insuranceID();
                if (!insuranceID || insuranceID === null || insuranceID === "") {
                    self.errors.push("Unable to submit hours unless the patient insurance can be determined.  Please select a different date or contact the Applied ABC office to inquire about patient insurance status at the date you've specified (<a href='" + getInsuranceInquiryEmailLink(d) + "'>click here to email</a>)");
                    valid = false;
                    setStatus(vm, valid, self.errors, self.warnings);
                    return;
                }
            }

            var ti = dm.getDateTime(d, vm.timeIn());
            var to = dm.getDateTime(d, vm.timeOut());
            var service = vm.selectedService();
            var note = vm.note();
            var noteGroups = vm.noteGroups();
            var location = vm.selectedServiceLocation();

            if (ti === null) {
                vm.suppressTimeCheckSingle();
                vm.timeIn(null);
                vm.clearSupressTimeCheckSingle();
                self.errors.push("Time In must be specified.");
                valid = false;
            } else {
                vm.suppressTimeCheckSingle();
                vm.timeIn(dm.get24hTimeFormat(ti));
                vm.clearSupressTimeCheckSingle();
            }

            if (to === null) {
                vm.suppressTimeCheckSingle();
                vm.timeOut(null);
                vm.clearSupressTimeCheckSingle();
                self.errors.push("Time Out must be specified.");
                valid = false;
            } else {
                vm.suppressTimeCheckSingle();
                vm.timeOut(dm.get24hTimeFormat(to));
                vm.clearSupressTimeCheckSingle();
            }

            if (ti && to) {
                if (ti.getTime() === to.getTime()) {
                    self.errors.push("Time In and Out cannot be the same.");
                    valid = false;
                }
                if (ti > to) {
                    self.errors.push("Time Out cannot be before Time In.");
                    valid = false;
                }
            }

            if (!service || service === null) {
                self.errors.push("Service must be specified.");
                valid = false;
            }

            if (!location || location === null) {
                self.errors.push("Location must be specified.");
                valid = false;
            }

            if (vm.entryMode() === vm.entryModes.Aide && isOnAideLegacyMode) {
                var noteMessage = "Note must be at least 3 sentences, with at least 18 characters per sentence";
                if (note === null || note.length < 15) {
                    self.errors.push(noteMessage);
                    valid = false;
                } else {
                    var parts = note.split(".");
                    if (parts.length < 4) {
                        self.errors.push(noteMessage);
                        valid = false;
                    } else {
                        if (note.length < 18 * 3) {
                            self.errors.push(noteMessage);
                            valid = false;
                        }
                    }
                }
            }

            if (vm.entryMode() === vm.entryModes.BCBA) {
                var noteCount = 0;
                for (var i = 0; i < noteGroups.length; i++) {
                    var notes = noteGroups[i].notes();
                    for (var j = 0; j < notes.length; j++) {
                        if (notes[j].answer() !== null && notes[j].answer() !== "") {
                            noteCount++;
                        }
                    }
                }
                if (noteCount < 3) {
                    self.errors.push("Must enter three or more appropriate notes.");
                    valid = false;
                }

            }
            setStatus(vm, valid, self.errors, self.warnings);
            return valid;
        };


        self.validateServer = function (resetMessages) {
            var valid = false;
            if (resetMessages) { resetMessages(); }

            if (!vm.useServerValidation()) {
                setStatus(vm, true, self.errors, self.warnings);
                return;
            }

            var d = dm.getDate(vm.date());
            var ti = dm.getDateTime(d, vm.timeIn());
            var to = dm.getDateTime(d, vm.timeOut());
            var status = vm.selectedStatus().id;
            var service = vm.selectedService().id();
            var serviceLocation = vm.selectedServiceLocation().ID;

            //try {
            //    serviceLocation = vm.selectedServiceLocation().ID;
            //} catch (err) {
            //    // do nothing
            //}
            $.ajax({
                url: "/HoursEntry/Validate?isOnAideLegacyMode=" + isOnAideLegacyMode,
                type: "POST",
                data: {
                    CatalystPreloadID: vm.catalystPreloadID(),
                    HoursID: vm.entryID(),
                    Status: status,
                    ProviderID: vm.providerID(),
                    PatientID: vm.patientID(),
                    Date: d.toISOString(),
                    TimeIn: ti.toISOString(),
                    TimeOut: to.toISOString(),
                    ServiceID: service,
                    ServiceLocationID: serviceLocation,
                    Note: vm.note(),
                    ExtendedNotes: vm.getExtendedNotesForSubmit(vm.noteGroups()),
                    SsgIDs: vm.ssgIDsForSubmit(),
                    IsTrainingEntry: vm.isTrainingEntry()
                }
            })
                .done(function (data) {
                    for (var i = 0; i < data.Messages.length; i++) {
                        var msg = data.Messages[i];
                        if (msg.Severity === 1) { // warning
                            self.warnings.push(msg.Message);
                        } else if (msg.Severity === 2) {    // error
                            self.errors.push(msg.Message);
                        } else if (msg.Severity === 0) { // general
                            // we don't handle this yet, maybe someday add if need be
                        }
                    }
                    valid = !(self.errors.length > 0);
                    setStatus(vm, valid, self.errors, self.warnings);
                    return;
                })
                .fail(function () {
                    self.errors.push("An unknown error occurred while trying to process this.  If the problem continues, please contact your administrator.");
                    valid = false;
                    setStatus(vm, valid, self.errors, self.warnings);
                    return;
                });
        };


        function getInsuranceInquiryEmailLink(date) {
            var link = "mailto:info@appliedabc.com";
            link += "?subject=Hours Entry Patient Insurance Inquiry for " + vm.patientName();
            link += "&body=Attempted to enter hours for Patient " + vm.patientName() + " on date " + GetFormattedDate(date);
            link += " but the active insurance at that date was unable to be determined.";
            return encodeURI(link);

            function GetFormattedDate(date) {
                return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear();
            }
        }
    }

    function FieldsManager() {
        var entities = ["behaviors", "interventions", "reinforcers", "goals", "barriers"];
        var genericAdd = function (entity, reportObs) {
            return function (e) {
                var report = reportObs();
                var section = entity + "Section";
                if (!report) {
                    report = {};
                }
                if (!report[section]) {
                    report[section] = {};
                }
                if (!report[section][entity]) {
                    report[section][entity] = [];
                }
                report[section][entity].push(e);
                reportObs(report);
                return true;
            };
        };
        var genericGetData = function (entity, reportObs) {
            return function (key) {
                var report = reportObs();
                var section = entity + "Section";
                if (report && report[section] && report[section][entity]) {
                    var e = ko.utils.arrayFirst(report[section][entity], function (i) {
                        return i.name === key;
                    });
                    return e;
                }
                return;
            };
        };
        var genericRemove = function (entity, reportObs) {
            return function (e) {
                var report = reportObs();
                var section = entity + "Section";
                if (report && report[section] && report[section][entity]) {
                    ko.utils.arrayRemoveItem(report[section][entity], e);
                    return true;
                }
                return false;
            };
        };

        var configuration = {
            summary: {
                createViewModel: function () {
                    return ko.observable();
                },
                createComponentParams: function (field, reportObs) {
                    return {
                        obs: reportObs()[field.name.toLowerCase()]
                    };
                }
            },
            interventions: {
                createViewModel: function (a) {
                    return {
                        name: a.name,
                        response: ko.observable(a.response || ""),
                        description: ko.observable(a.description || "")
                    };
                },
                createComponentParams: function (field, reportObs) {
                    var entity = field.name.toLowerCase();
                    return {
                        entity: entity,
                        options: field.options,
                        getData: genericGetData(entity, reportObs),
                        createViewModel: this.createViewModel,
                        addCallback: genericAdd(entity, reportObs),
                        removeCallback: genericRemove(entity, reportObs),
                        reset: function (vm) {
                            vm.response("");
                            vm.description("");
                        }
                    };
                }
            },
            goals: {
                createViewModel: function (a) {
                    return {
                        name: ko.observable((a || {}).name || ""),
                        progress: ko.observable((a || {}).progress || "")
                    };
                },
                createComponentParams: function (field, reportObs) {
                    var entity = field.name.toLowerCase();
                    return {
                        entity: entity,
                        getData: function () {
                            var report = reportObs();
                            var section = entity + "Section";
                            if (report && report[section] && report[section][entity]) {
                                return report[section][entity];
                            }
                            return;
                        },
                        createViewModel: this.createViewModel,
                        addCallback: genericAdd(entity, reportObs),
                        removeCallback: genericRemove(entity, reportObs),
                        reset: function (vm) {
                            vm.name("");
                            vm.progress("");
                        }
                    };
                }
            }
        };

        entities.forEach(function (i) {
            var common = {
                createViewModel: function (a) {
                    return {
                        name: a.name,
                        description: ko.observable(a.description || "")
                    };
                },
                createComponentParams: function (field, reportObs) {
                    return {
                        entity: i,
                        options: field.options,
                        getData: genericGetData(i, reportObs),
                        createViewModel: this.createViewModel,
                        addCallback: genericAdd(i, reportObs),
                        removeCallback: genericRemove(i, reportObs),
                        reset: function (vm) {
                            vm.description("");
                        }
                    };
                }
            };
            if (!configuration[i]) {
                configuration[i] = common;
            }
        });

        return {
            mapReport: function (report) {
                var e = {
                    summary: ko.observable(report.summary || "")
                };
                entities.forEach(function (entity) {
                    var sectionName = entity + "Section";
                    var conf = configuration[entity];
                    if (report[sectionName] && report[sectionName][entity] && conf) {
                        if (!e[sectionName]) {
                            e[sectionName] = {};
                        }
                        e[sectionName][entity] = report[sectionName][entity].map(conf.createViewModel);
                    }
                });
                return e;
            },
            createComponentConfig(field, reportObs) {
                var conf = configuration[field.name.toLowerCase()];
                var params = conf.createComponentParams(field, reportObs);
                return {
                    name: field.name,
                    componentConfig: {
                        name: field.controlType,
                        params: params
                    }
                };
            }
        };
    }

    window.aabc = window.aabc || {};

    window.aabc.hoursEntry = window.aabc.hoursEntry || {};

    window.aabc.hoursEntry.DateManager = function () {
        function getNumbers(input) {
            var result = "";
            var test = input.toString();
            for (var i = 0; i < test.length; i++) {
                var s = test.charAt(i);
                if (s === "0") result += s;
                if (s === "1") result += s;
                if (s === "2") result += s;
                if (s === "3") result += s;
                if (s === "4") result += s;
                if (s === "5") result += s;
                if (s === "6") result += s;
                if (s === "7") result += s;
                if (s === "8") result += s;
                if (s === "9") result += s;
            }
            if (result === "") {
                result = "0";
            }
            return parseInt(result);
        }

        function _getDate(date, pos) {
            return date ? date.split("T")[pos] : null;
        }

        return {
            getDate: function (dateString) {
                try {
                    if (!dateString) {
                        return null;
                    }

                    var parts = dateString.split("-");
                    if (parts.length === 0) {
                        parts = dateString.split("/");
                    }
                    if (parts[0].length === 4) {
                        // yyyy-mm-dd
                        return new Date(parts[0], parts[1] - 1, parts[2], 0, 0, 0, 0);
                    }
                    if (parts[0].length === 2) {
                        // mm-dd-yyyy
                        return new Date(parts[2], parts[0], parts[1] - 1, 0, 0, 0, 0);
                    }

                    return null;
                } catch (err) {
                    return null;
                }
            },
            getDateTime: function (dateObject, timeString) {
                if (!timeString) {
                    return null;
                }
                try {
                    //console.log("timestring: " + timeString);
                    var hoursPart = 0;
                    var minutesPart = 0;
                    var s = timeString.toLowerCase();
                    var d = new Date(dateObject.getTime());
                    var amIndex = s.indexOf("am");
                    var pmIndex = s.indexOf("pm");
                    var colIndex = s.indexOf(":");
                    var perIndex = s.indexOf(".");

                    s = s.replace("am", "");
                    s = s.replace("pm", "");
                    s = s.trim();

                    if (amIndex === -1) {
                        amIndex = s.indexOf("a");
                        s = s.replace("a");
                    }
                    if (pmIndex === -1) {
                        pmIndex = s.indexOf("p");
                        s = s.replace("p");
                    }

                    var sep = "";
                    if (perIndex >= 0) sep = ".";
                    if (colIndex >= 0) sep = ":";

                    //console.log("s: " + s);
                    //console.log("d: " + d);
                    //console.log("ami: " + amIndex);
                    //console.log("pmi: " + pmIndex);
                    //console.log("sep: " + sep);

                    if (sep === "") {
                        // no separator, this is hours only
                        hoursPart = getNumbers(s);
                        minutesPart = 0;
                    } else {
                        var parts = s.split(sep);
                        hoursPart = getNumbers(parts[0]);
                        minutesPart = getNumbers(parts[1]);
                    }
                    //console.log("hoursPart: " + hoursPart);
                    //console.log("minutesPart: " + minutesPart);

                    if (pmIndex >= 0) {
                        hoursPart += 12;
                    }
                    d.setHours(hoursPart, minutesPart, 0, 0);
                    //console.log("final calculated time:");
                    //console.log(d);
                    return d;
                } catch (err) {
                    return null;
                }
            },
            getDisplayFormattedTime: function (dateObject) {
                var ampm = "am";
                var hours = dateObject.getHours();

                if (hours > 12) {
                    hours = hours - 12;
                    ampm = "pm";
                } else if (hours < 10) {
                    hours = "0" + hours;
                }

                var minutes = dateObject.getMinutes();
                if (minutes < 10) {
                    minutes = "0" + minutes;
                }

                return hours + ":" + minutes + " " + ampm;
            },
            get24hTimeFormat: function (dateObject) {
                var hours = dateObject.getHours();
                if (hours < 10) {
                    hours = "0" + hours;
                }

                var minutes = dateObject.getMinutes();
                if (minutes < 10) {
                    minutes = "0" + minutes;
                }

                return hours + ":" + minutes;
            },
            getDate2: function (date) {
                return _getDate(date, 0);
            },
            getTime: function (date) {
                return _getDate(date, 1);
            }
        };
    };

    window.aabc.hoursEntry.Repository = function Repository(mode) {
        if (mode === "test") {
            return {
                getServices: function (caseID, providerID, date) {
                    var result = [];
                    result.push({});
                    result.push({});
                    result.push({});
                    result.push({});
                    result.push({});
                    return result;
                },
                activePatientsAtDate: function (providerID, date, callback) {
                    var result = [];
                    result.push({ CaseID: 522, PatientID: 711, Name: "John Doe" });
                    result.push({ CaseID: 422, PatientID: 611, Name: "Maurice Doe" });
                    result.push({ CaseID: 322, PatientID: 511, Name: "Kelly Melly" });
                    result.push({ CaseID: 222, PatientID: 411, Name: "Mary Contrary" });
                    result.push({ CaseID: 122, PatientID: 311, Name: "Joe Schmoe" });
                    callback(result);
                },
                activeInsuranceAtDate: function (patientID, date, callback) {
                    return {
                        insuranceID: 55,
                        insuranceName: "Test Insurance"
                    };
                }
            };
        } else {
            return {
                getServices: function (caseID, providerID, date, callback) {
                    $.ajax({
                        url: "/HoursEntry/GetServices",
                        type: "GET",
                        data: {
                            caseID: caseID,
                            providerID: providerID,
                            date: date.toISOString()
                        }
                    })
                        .done(function (result) {
                            callback(result);
                        });
                },
                activePatientsAtDate: function (providerID, date, callback) {
                    $.ajax({
                        url: "/HoursEntry/GetActivePatients",
                        type: "GET",
                        data: {
                            providerID: providerID,
                            date: date.toISOString()
                        }
                    })
                        .done(function (data) {
                            callback(data.patients);
                            return;
                        });
                },
                activeInsuranceAtDate: function (patientID, date, callback) {
                    $.ajax({
                        url: "/HoursEntry/GetActiveInsurance",
                        type: "GET",
                        data: {
                            patientID: patientID,
                            date: date.toISOString()
                        }
                    })
                        .done(function (data) {
                            callback(data);
                        })
                        .fail(function () {
                            callback(null);
                        });
                }
            };
        }
    };

    window.aabc.hoursEntry.ServiceEnum = {
        DirectCare: 9,
        ParentTraining: 10,
        InitialAssessment: 11,
        TreatmentPlanning: 12,
        DirectSupervision: 13,
        SocialSkillsGroup: 14,
        SupervisionReceived: 15,
        TeamMeeting: 16,
        Assessment: 17,
        FollowupAssessment: 18
    };

    window.aabc.hoursEntry.ServiceTypeEnum = {  // keep this in sync with AABC.Domain2.Services.ServiceTypes
        General: 0,
        Assessment: 1,
        Care: 2,
        Social: 3,
        Supervision: 4,
        Management: 5
    };

    window.aabc.hoursEntry.ProviderTypeEnum = {
        BCBA: 15,
        Aide: 17
    };

    window.aabc.hoursEntry.HoursEntryVM = function (model, cancelCallback, successCallback, deleteCallback, testMode) {
        var self = this;
        var repo = new window.aabc.hoursEntry.Repository(testMode);
        var dm = new window.aabc.hoursEntry.DateManager();
        var fm = new FieldsManager();
        var validator = new EntryValidator(self, model.IsOnAideLegacyMode);
        var validate = function () {
            if (!self.isInitialized) {
                return;
            }
            validator.validateBoth();
        };

        // =======================
        // CONSTANTS
        // =======================        
        self.entryModes = {
            BCBA: 0,
            Aide: 1
        };
        self.statusEnum = [
            { id: -1, description: "PreChecked" },
            { id: 0, description: "Pending" },
            { id: 1, description: "Committed by Provider" },
            { id: 2, description: "Finalized by Provider" },
            { id: 3, description: "Scrubbed/Complete" }
        ];

        // =======================
        // PRE-INITIALIZATION
        // =======================
        self.debugMode = ko.observable(false);
        self.isInitialized = false;
        self.isValid = ko.observable(true); // set by EntryValidator and used while setting statuses
        self.useServerValidation = ko.observable(model.isAdminMode);
        self.suppressTimeCheckSingle = function () { self.isSupressTimeCheckSingle = true; };    // used to bypass time validation due to UI quirk when setting times to null
        self.isSupressTimeCheckSingle = false;
        self.clearSupressTimeCheckSingle = function () { self.isSupressTimeCheckSingle = false; };

        // =======================
        // MODEL SETUP
        // =======================
        // core model fields
        self.caseID = ko.observable(model.CaseID);
        self.catalystPreloadID = ko.observable(model.CatalystPreloadID);
        self.isEditable = ko.observable(model.IsEditable);
        self.nonEditableReason = ko.observable(model.NonEditableReason);
        self.entryID = ko.observable(model.EntryID);
        self.hasData = ko.observable(model.HasData);
        self.providerID = ko.observable(model.ProviderID);
        self.providerTypeID = ko.observable(model.ProviderTypeID);
        self.providerTypeCode = ko.observable(model.ProviderTypeCode);
        self.patientID = ko.observable(model.PatientID);
        self.patientName = ko.observable(model.PatientName);
        self.insuranceID = ko.observable(model.InsuranceID);
        self.isNonParentSSGEntry = ko.observable(model.IsNonParentSSGEntry);
        self.isTrainingEntry = ko.observable(model.IsTrainingEntry);
        self.insuranceName = ko.observable(model.InsuranceName || "Unknown insurance or none on file");
        self.note = ko.observable(model.Note).extend({ rateLimit: { method: "notifyWhenChangesStop", timeout: 500 } });
        self.sessionReport = ko.observable(fm.mapReport(model.SessionReport));
        self.sessionReportConfig = ko.observable();

        // format conversions
        self.date = ko.observable(dm.getDate2(model.Date));
        self.timeIn = ko.observable(dm.getTime(model.TimeIn));
        self.timeOut = ko.observable(dm.getTime(model.TimeOut));

        // expression fields/misc
        self.isOnAideLegacyMode = ko.observable(model.IsOnAideLegacyMode);
        self.addMode = ko.observable(model.EntryID === null);                       // determines if we're adding or updating a record
        self.allowDeletion = ko.observable(!self.addMode() && model.IsEditable);
        self.isAdminMode = ko.observable(model.IsAdminMode);
        self.entryMode = ko.observable(getEntryMode(model.ProviderTypeID));         // determines if BCBA or Aide entry mode        

        // collections
        self.serviceLocations = ko.observableArray(model.ServiceLocations);
        self.noteGroups = ko.observableArray(model.NoteGroups.map(function (i) { return new HoursNoteGroupVM(i, validate); }));
        self.services = ko.observableArray(model.Services.map(function (i) { return new HoursServiceVM(i); }));
        self.activePatients = ko.observableArray(model.ActivePatients.map(function (i) { return new HoursActivePatientVM(i); }));
        self.messages = ko.observableArray([]);

        // collection selections
        self.selectedStatus = ko.observable(getInitialStatus(model.Status));
        self.selectedService = ko.observable(getServiceObjectFromID(model.ServiceID));
        self.selectedServiceLocation = ko.observable(getServiceLocationObjectFromID(model.ServiceLocationID));
        self.selectedSSGPatients = ko.observableArray(getSelectedSSGPatients(model.SSGCaseIDs, self.activePatients()));
        self.selectedServiceInfo = ko.observable("");  // this is bound to the UI, our async method from above will update this

        // computed properties, general
        self.enableDateEdit = ko.pureComputed(function () { return self.addMode(); });
        self.showNoteContainer = ko.pureComputed(function () { return self.entryMode() === self.entryModes.Aide; });
        self.showNotesContainer = ko.pureComputed(function () { return self.entryMode() === self.entryModes.BCBA; });
        self.hasDate = ko.pureComputed(function () {
            var d = self.date();
            try {
                d = Date.parse(d);
                return !isNaN(d);
            } catch (err) {
                return false;
            }
        });

        // computed properties, ssg
        self.isSSG = ko.pureComputed(function () {
            var selectedService = self.selectedService();
            //return self.selectedService().id() === window.aabc.hoursEntry.ServiceEnum.SocialSkillsGroup;
            return selectedService && selectedService.typeID() === window.aabc.hoursEntry.ServiceTypeEnum.Social;
            //try {
            //    //return self.selectedService().id() === window.aabc.hoursEntry.ServiceEnum.SocialSkillsGroup;
            //    return self.selectedService().typeID() === window.aabc.hoursEntry.ServiceTypeEnum.Social;
            //} catch (err) {
            //    return false;
            //}
        });
        self.showSSGInfo = ko.pureComputed(function () {
            var isSSG = self.isSSG();
            var hasDate = self.hasDate();
            return !(!isSSG || hasDate);
        });
        self.ssgInfo = ko.pureComputed(function () {
            return !self.hasDate() ? "Select a date to view your active cases for SSG" : "";
        });
        self.ssgIDsForSubmit = ko.computed(function () {
            var selected = self.selectedSSGPatients();
            var results = [];

            if (selected) {
                for (var i = 0; i < selected.length; i++) {
                    results.push(selected[i].caseID);
                }
            }

            return results;
        });

        // computed properties, messages
        var getMessages = function (type) {
            return self.messages().filter(function (obj) {
                return obj.messageType === type;
            });
        };

        self.infoMessages = ko.pureComputed(function () { return getMessages("info"); });
        self.warningMessages = ko.pureComputed(function () { return getMessages("warning"); });
        self.errorMessages = ko.pureComputed(function () { return getMessages("error"); });
        self.hasWarningMessages = ko.pureComputed(function () { return self.warningMessages().length > 0; });
        self.hasErrorMessages = ko.pureComputed(function () { return self.errorMessages().length > 0; });
        self.hasInfoMessages = ko.pureComputed(function () { return self.infoMessages().length > 0; });

        // =======================
        // METHODS
        // =======================
        self.submit = function () {
            //console.log("submitting viewmodel");
            var serviceLocation = null;
            try {
                serviceLocation = self.selectedServiceLocation().ID;
            } catch (err) {
                // do nothing
            }
            var d = dm.getDate(self.date());
            var dataPost = {
                HoursID: self.entryID(),
                Status: self.selectedStatus().id,
                ProviderID: self.providerID(),
                PatientID: self.patientID(),
                Date: d.toISOString(),
                TimeIn: dm.getDateTime(d, self.timeIn()).toISOString(),
                TimeOut: dm.getDateTime(d, self.timeOut()).toISOString(),
                ServiceID: self.selectedService().id(),
                ServiceLocationID: serviceLocation,
                Note: self.note(),
                ExtendedNotes: self.getExtendedNotesForSubmit(self.noteGroups()),
                SsgIDs: self.ssgIDsForSubmit(),
                IsTrainingEntry: self.isTrainingEntry(),
                CatalystPreloadID: self.catalystPreloadID(),
                SessionReport: ko.toJS(self.sessionReport())
            };
            var isOnAideLegacyMode = self.isOnAideLegacyMode();
            $.ajax({
                url: "/HoursEntry/Submit?isOnAideLegacyMode=" + isOnAideLegacyMode,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(dataPost)
            })
                .done(function (data) {
                    if (data.WasProcessed) {
                        try {
                            successCallback();
                        } catch (err) {
                            // do nothing
                        }
                    } else {
                        var warnings = [];
                        var errors = [];
                        for (var i = 0; i < data.Messages.length; i++) {
                            var msg = data.Messages[i];
                            if (msg.Severity === 1) { // warning
                                warnings.push(msg.Message);
                            } else if (msg.Severity === 2) {    // error
                                errors.push(msg.Message);
                            } else if (msg.Severity === 0) { // general
                                // we don't handle this yet, maybe someday add if need be
                            }
                        }
                        valid = !(errors.length > 0);
                        setStatus(self, valid, errors, warnings);
                        return;
                    }
                })
                .fail(function (jqXHR) {
                    //dataPost.jqXHR: JSON.stringify(jqXHR)
                    $.ajax({
                        type: "POST",
                        url: "/Errors/LogJS",
                        data: {
                            message: JSON.stringify(dataPost),
                            source: "HoursEntry.js:Submit",
                            lineno: null,
                            colno: null,
                            jsonError: null
                        }
                    });
                });
        };

        self.test = function () {
            console.log("TESTING");
            console.log(ko.toJS(self));
            console.log(model);
        };
        self.cancel = cancelCallback;
        self.deleteEntry = deleteCallback;
        self.setWarningMessages = function (allMessages) { setMessages(allMessages, "warning"); };
        self.setErrorMessages = function (allMessages) { setMessages(allMessages, "error"); };
        self.setInfoMessages = function (allMessages) { setMessages(allMessages, "info"); };
        self.getExtendedNotesForSubmit = function (noteGroups) { return getExtendedNotesForSubmit(noteGroups); };
        self.clearSSG = function () {
            if (!self.isSSG()) {
                return;
            }
            self.selectedService(null);
            self.activePatients([]);
        };

        // =======================
        // EVENT SUBSCRIPTIONS
        // =======================
        self.date.subscribe(function () {
            if (!self.isInitialized) return;
            getInsuranceAtDate();
            getActivePatientsAtDate();
            validator.validateBoth();
        });
        self.timeIn.subscribe(function () {
            if (!self.isInitialized) return;
            if (self.isSupressTimeCheckSingle) {
                self.isSupressTimeCheckSingle = false;
                return;
            }
            validator.validateBoth();
        });
        self.timeOut.subscribe(function () {
            if (!self.isInitialized) return;
            if (self.isSupressTimeCheckSingle) {
                self.isSupressTimeCheckSingle = false;
                return;
            }
            validator.validateBoth();
        });
        self.note.subscribe(validate);
        self.noteGroups.subscribe(validate);
        self.selectedService.subscribe(function (value) {
            if (!self.isInitialized) return;
            validator.validateBoth();
            if (value && !self.isOnAideLegacyMode()) {
                $.ajax({
                    url: "/HoursEntry/GetSessionReportConfig",
                    type: "GET",
                    data: {
                        providerTypeID: self.providerTypeID(),
                        serviceID: value.id()
                    }
                }).done(function (data) {
                    var f = {
                        fields: data.fields.map(function (i) {
                            return fm.createComponentConfig(i, self.sessionReport);
                        })
                    };
                    self.sessionReportConfig(f);
                });
            }
        });
        self.insuranceID.subscribe(function () {
            if (!self.isInitialized) return;
            validator.validateBoth();
            getServices(function () {
                validator.validateBoth();
            });
        });
        self.selectedServiceLocation.subscribe(validate);
        self.isTrainingEntry.subscribe(validate);

        // =======================
        // FINALIZE INITIALIZATION
        // =======================
        // if new entry and prefilled date, get insurance, active patients, etc
        if (self.addMode && self.date()) {
            getInsuranceAtDate();
            getActivePatientsAtDate(function () {
                // if we were in SSG mode, our initialized SSG got cleared here, let's reset it
                self.selectedService(getServiceObjectFromID(model.ServiceID));
                self.selectedSSGPatients(getSelectedSSGPatients(model.SSGCaseIDs, self.activePatients()));
            });
        }

        self.isInitialized = true;
        validator.validateBoth();

        return self;

        // =======================
        // INITIALIZATION COMPLETE
        // =======================

        function getExtendedNotesForSubmit(noteGroups) {
            var results = [];
            for (var i = 0; i < noteGroups.length; i++) {
                var notes = noteGroups[i].notes();
                for (var j = 0; j < notes.length; j++) {
                    results.push({
                        ID: notes[j].id(),
                        Answer: notes[j].answer(),
                        TemplateID: notes[j].templateID()
                    });
                }
            }
            return results;
        }


        function getInitialStatus(statusID) {
            return self.statusEnum.filter(function (obj) {
                return obj.id === statusID;
            })[0];
        }


        function getSelectedSSGPatients(ssgCaseIDs, activePatients) {
            var results = [];
            if (!ssgCaseIDs || ssgCaseIDs.length === 0) {
                return results;
            }
            for (var i = 0; i < ssgCaseIDs.length; i++) {
                var patient = activePatients.filter(function (obj) {
                    return obj.caseID() === ssgCaseIDs[i];
                });
                if (patient.length > 0) {
                    results.push(patient[0]);
                }
            }
            return results;
        }


        function setMessages(allMessages, messageType) {
            var currentMessages = self.messages();

            for (var i = currentMessages.length - 1; i >= 0; --i) {
                if (currentMessages[i].messageType === messageType) {
                    currentMessages.splice(i, 1);
                }
            }

            var newMessages = allMessages.filter(function (obj) {
                return obj.messageType === messageType;
            });

            for (i = 0; i < newMessages.length; i++) {
                currentMessages.push(newMessages[i]);
            }

            self.messages(currentMessages);
        }


        function getServices(callback) {
            var currentService = self.selectedService();
            var caseID = self.caseID();
            var providerID = self.providerID();
            var d = dm.getDate(self.date());
            if (d) {
                repo.getServices(caseID, providerID, d, function (data) {
                    self.services(data.map(function (i) { return new HoursServiceVM(i); }));
                    if (serviceIsPresent(currentService)) {
                        self.selectedService(currentService);
                    } else {
                        self.selectedService("");
                    }
                    callback();

                    function serviceIsPresent(service) {
                        if (!service) {
                            return false;
                        }
                        var services = self.services();
                        for (var i = 0; i < services.length; i++) {
                            if (services[i].id() === service.id()) {
                                return true;
                            }
                        }
                        return false;
                    }
                });
            }
        }


        function getActivePatientsAtDate(callback) {
            var d = dm.getDate(self.date());
            if (d) {
                self.clearSSG();
                repo.activePatientsAtDate(self.providerID(), d, function (data) {
                    self.activePatients(data.map(function (i) { return new HoursActivePatientVM(i); }));
                    try {
                        callback();
                    } catch (err) {
                        // do nothing
                    }
                    return;
                });
            }
        }


        function getInsuranceAtDate() {
            var d = dm.getDate(self.date());
            if (d === null) {
                self.insuranceID("");
                self.insuranceName("Select a date to determine the active insurance");
            } else {
                repo.activeInsuranceAtDate(self.patientID(), d, function (data) {
                    //console.log("insurance data");
                    //console.log(data);
                    self.insuranceID(data === null ? "" : data.insuranceID);
                    self.insuranceName(data === null ? "Unable to determine active insurance" : data.insuranceName);
                });
            }
        }


        // Initialization methods
        function getEntryMode(providerTypeID) {
            return providerTypeID === window.aabc.hoursEntry.ProviderTypeEnum.BCBA ? self.entryModes.BCBA : self.entryModes.Aide;
        }


        function getServiceObjectFromID(serviceID) {
            var services = self.services();
            for (var i = 0; i < services.length; i++) {
                if (serviceID === services[i].id()) {
                    return services[i];
                }
            }
        }


        function getServiceLocationObjectFromID(locationID) {
            var locations = self.serviceLocations();
            for (var i = 0; i < locations.length; i++) {
                if (locationID === locations[i].ID) {
                    return locations[i];
                }
            }
        }


    };


    // Nested Viewmodels
    function HoursNoteGroupVM(model, noteUpdatedCallback) {
        var self = this;
        self.id = ko.observable(model.ID);
        self.name = ko.observable(model.Name);
        self.notes = ko.observableArray(model.Notes.map(function (i) {
            return new HoursNoteVM(i, noteUpdatedCallback);
        }));
        return self;
    }


    function HoursNoteVM(model, noteUpdatedCallback) {
        var self = this;
        self.id = ko.observable(model.ID);
        self.question = ko.observable(model.Question);
        self.answer = ko.observable(model.Answer);
        self.templateID = ko.observable(model.TemplateID);
        self.answer.subscribe(noteUpdatedCallback);
        return self;
    }


    function HoursServiceVM(model) {
        var self = this;
        self.id = ko.observable(model.ID);
        self.code = ko.observable(model.Code);
        self.name = ko.observable(model.Name);
        self.description = ko.observable(model.Description);
        self.typeID = ko.observable(model.TypeID);
        self.typeName = ko.observable(model.TypeName);

        self.display = ko.pureComputed(function () {
            return ko.unwrap(self.code) + " - " + ko.unwrap(self.name);
        });

        return self;
    }


    function HoursActivePatientVM(model) {
        var self = this;
        self.caseID = ko.observable(model.CaseID);
        self.patientID = ko.observable(model.PatientID);
        self.name = ko.observable(model.Name);
        return self;
    }


    function InfoMessage(message, messageType) {
        // type: error, warning, info
        var self = this;
        self.messageType = messageType;
        self.message = message;
        return self;
    }


    ko.components.register("text", {
        viewModel: function (params) {
            this.summary = params.obs;
        },
        template: {
            element: "component-text"
        }
    });

    var multiListViewModelFactory = function (callback) {
        return function (params) {
            this.entity = params.entity;
            this.items = params.options.map(function (i) {
                var data = params.getData(i.name);
                var item = {
                    checked: ko.observable(!!data),
                    data: data || params.createViewModel(i)
                };
                if (callback) {
                    callback(item, i);
                }
                item.checked.subscribe(function (value) {
                    if (value) {
                        if (params.addCallback) {
                            params.addCallback(this.data);
                        }
                    } else {
                        if (params.removeCallback) {
                            params.removeCallback(this.data);
                        }
                    }
                    params.reset(this.data);
                }, item);
                return item;
            });
        };
    };

    ko.components.register("multiSelect", {
        viewModel: multiListViewModelFactory(),
        template: {
            element: "component-multiSelect"
        }
    });


    ko.components.register("interventions", {
        viewModel: multiListViewModelFactory(function (item, field) {
            item.responses = field.responses;
        }),
        template: {
            element: "component-interventions"
        }
    });


    ko.components.register("goals", {
        viewModel: function (params) {
            this.goals = ko.observableArray(params.getData() || []);
            this.newGoal = params.createViewModel();
            this.canAddGoal = ko.pureComputed(function () {
                var v = this.newGoal.name() || "";
                return v.length > 0;
            }, this);
            this.addGoal = function () {
                var newGoal = params.createViewModel(ko.toJS(this.newGoal));
                var add = true;
                if (params.addCallback) {
                    add = params.addCallback(newGoal);
                }
                if (add) {
                    this.goals.push(newGoal);
                }
                params.reset(this.newGoal);
            }.bind(this);
            this.removeGoal = function (goal) {
                this.goals.remove(function (i) {
                    return i.name() === goal.name();
                });
                if (params.removeCallback) {
                    params.removeCallback(goal);
                }
            }.bind(this);
        },
        template: {
            element: "component-goals"
        }
    });


})();
