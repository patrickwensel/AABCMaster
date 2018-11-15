(function () {

    function ProviderTypeVM(data) {
        var self = {};
        self.id = ko.observable();
        self.code = ko.observable();
        self.name = ko.observable();
        self.fullName = ko.pureComputed(function () {
            return self.code() + " - " + self.name();
        });
        self.update = function (data) {
            ko.mapping.fromJS(data, {
                key: function (data) {
                    return ko.unwrap(data.id);
                }
            }, self);
        }

        if (data) {
            self.update(data);
        }

        return self;
    }

    function ProviderSubTypeVM(data) {
        var self = {};
        self.id = ko.observable();
        self.providerTypeId = ko.observable();
        self.code = ko.observable();
        self.name = ko.observable();
        self.isBeingUsed = ko.observable();

        self.code.extend({ required: true });
        self.name.extend({ required: true });

        self.fullName = ko.pureComputed(function () {
            return self.code() + " - " + self.name();
        });

        self.update = function (data) {
            ko.mapping.fromJS(data, {
                key: function (data) {
                    return ko.unwrap(data.id);
                }
            }, self);
        }

        self.toJS = function () {
            return JSON.parse(ko.toJSON(self));
        };

        if (data) {
            self.update(data);
        }

        return self;
    }

    function ProviderSubTypesVM() {
        var self = {};
        self.providerTypes = ko.observableArray([]);
        self.providerSubTypes = ko.observableArray([]);
        self.selectedProviderType = ko.observable(null);
        self.selectedProviderSubType = ko.observable(null);

        self.selectedProviderType.subscribe(function (newProviderTypeId) {
            if (newProviderTypeId) {
                $.getJSON("/ProviderTypes/GetProviderSubTypes", { providerTypeId: newProviderTypeId }).done(function (providerSubTypes) {
                    self.providerSubTypes(providerSubTypes.map(function (m) {
                        return new ProviderSubTypeVM(m);
                    }));
                });
            }
            self.selectedProviderSubType(null);
        });

        self.selectSubType = function (providerSubType) {
            self.selectedProviderSubType(providerSubType);
        };

        self.addNew = function () {
            var selectedProviderTypeId = self.selectedProviderType();
            if (selectedProviderTypeId) {
                self.selectedProviderSubType(new ProviderSubTypeVM({ providerTypeId: selectedProviderTypeId }));
            }
        };

        self.save = function () {
            var selectedProviderSubType = self.selectedProviderSubType();
            if (selectedProviderSubType) {
                var errors = ko.validation.group(selectedProviderSubType, { deep: true });
                if (errors().length > 0) {
                    errors.showAllMessages();
                } else {
                    var data = selectedProviderSubType.toJS();
                    var isNew = !data.id;
                    var url = isNew ? "/ProviderTypes/InsertProviderSubType" : "/ProviderTypes/SaveProviderSubType";
                    $.ajax({
                        url: url,
                        type: "POST",
                        data: data
                    }).done(function (response) {
                        if (response.success) {
                            if (isNew) {
                                self.providerSubTypes.push(new ProviderSubTypeVM(response.data));
                            }
                            App.Dialogs.MessageBox("Sub type successfully saved.");
                            self.selectedProviderSubType(null);
                        }
                    }).fail(function () {
                        App.Dialogs.Error();
                    });
                }
            }
        };

        self.delete = function (providerSubType) {
            var message = providerSubType.isBeingUsed() ? "Are you sure you want to delete a sub type that is being referenced by providers?" : "Are you sure you want to delete this sub type?";
            App.Dialogs.ConfirmDelete({
                message: message,
                confirmed: function () {
                    $.ajax({
                        url: "/ProviderTypes/DeleteProviderSubType",
                        type: "POST",
                        data: {
                            providerSubTypeId: ko.unwrap(providerSubType.id)
                        }
                    }).done(function (response) {
                        if (response.success) {
                            ko.utils.arrayRemoveItem(self.providerSubTypes(), providerSubType);
                            App.Dialogs.MessageBox("Sub type successfully deleted.");
                        }
                    }).fail(function () {
                        App.Dialogs.Error();
                    });
                }
            });
        };

        return self;
    }

    window.ProviderSubType = {
        init: function (element) {
            LoadingPanel.Show();
            $.getJSON("/ProviderTypes/GetProviderTypes").done(function (providerTypes) {
                var vm = new ProviderSubTypesVM();
                vm.providerTypes(providerTypes.map(function (m) {
                    return new ProviderTypeVM(m);
                }));
                ko.cleanNode(element);
                ko.applyBindings(vm, element);
            }).fail(function () {
                App.Dialogs.Error();
            }).always(function () {
                LoadingPanel.Hide();
            });
        }
    };
})();





