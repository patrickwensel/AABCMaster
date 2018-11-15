// Scripts/Pages/Insurance.js

(function () {

    var api = {
        List: {
            Initialize: function () { code.list.initialize(); }
        },
        Detail: {
            Go: function (insuranceID) { code.detail.go(insuranceID); },
            Initialize: function (insuranceID) { code.detail.initialize(insuranceID); },
            AddAuthRule: function () { code.detail.rules.editor.set(null); },
            RemoveSelectedAuthRule: function () { code.detail.rules.grid.removeSelected(); },
            AddService: function () { code.detail.services.editor.set(null); },
            RemoveSelectedService: function () { code.detail.services.grid.removeSelected(); },
            AddCarrier: function () { code.detail.carriers.addCarrier(); },
            RemoveCarrier: function () { code.detail.carriers.grid.removeSelected(); }
        }
    }



    var code = {

        preInit: function () {

        },


        list: {

            currentID: function () {
                var grid = App.DevEx.GetControl("InsurancesGrid");
                var index = grid.GetFocusedRowIndex();
                var id = grid.GetRowKey(index);
                return id;
            },

            addNew: function () {

                var name = prompt("Enter name of insurance to add");
                if (name == "" || name == null) {
                    return;
                }

                $.ajax({
                    type: 'POST',
                    url: '/Insurance/Add',
                    data: {
                        name: name
                    },
                    success: function (res) {
                        if (res == 'ok') {
                            App.DevEx.GetControl('InsurancesGrid').Refresh();
                            return;
                        } else {
                            App.Dialogs.Error(res);
                        }
                    }
                });

            },
            copy: function (id) {

                var name = prompt("Enter name of the copy");
                if (name == "" || name == null) {
                    return;
                }


                $.ajax({
                    type: 'POST',
                    url: '/Insurance/Add',
                    data: {
                        name: name,
                        copySourceId: id
                    },
                    success: function (res) {
                        if (res == 'ok') {
                            App.DevEx.GetControl("InsurancesGrid").Refresh();
                            return;
                        } else {
                            App.Dialogs.Error(res);
                        }
                    },
                    error: function () {
                        App.Dialogs.Error();
                    }
                })

            },
            remove: function (id) {
                var b = confirm("Are you sure you want to remove the selected insurance?");
                if (b != true) {
                    return;
                }

                $.ajax({
                    type: 'POST',
                    url: '/Insurance/Delete',
                    data: {
                        id: id
                    },
                    success: function (res) {
                        if (res == 'ok') {
                            App.DevEx.GetControl("InsurancesGrid").Refresh();
                            return;
                        } else {
                            App.Dialogs.Error(res);
                        }
                    },
                    error: function () {
                        App.Dialogs.Error();
                    }
                })

            },

            initialize: function () {

                code.list.linkCellDoubleClick();
                code.list.linkHeaderButtonClicks();

                var grid = App.DevEx.GetControl("InsurancesGrid");

                grid.EndCallback.AddHandler(function (s, e) {
                    code.list.linkCellDoubleClick();
                    code.list.linkHeaderButtonClicks();
                });

            },


            linkHeaderButtonClicks: function () {

                App.DevEx.GetControl("btnRemoveSelectedInsurance").Click.AddHandler(function (s, e) {
                    e.preventDefault;
                    code.list.remove(code.list.currentID());
                    return false;
                });

                App.DevEx.GetControl("btnAddNewInsurance").Click.AddHandler(function (s, e) {
                    e.preventDefault;
                    code.list.addNew();
                    return false;
                });

                App.DevEx.GetControl("btnCopyInsurance").Click.AddHandler(function (s, e) {
                    e.preventDefault;
                    code.list.copy(code.list.currentID());
                    return false;
                });

            },

            linkCellDoubleClick: function () {

                $('[data-dym-cellClick="InsurancesGrid"]').dblclick(function () {
                    var visibleIndex = $(this).attr('data-dym-visibleIndex');
                    var id = App.DevEx.GetControl("InsurancesGrid").GetRowKey(visibleIndex);
                    api.Detail.Go(id);
                });

            }

        },

        detail: {

            go: function (insuranceID) {

                $.ajax({
                    type: 'GET',
                    url: '/Insurance/Edit',
                    data: {
                        id: insuranceID
                    },
                    success: function (res) {
                        $('#insurance-detail-container').empty().append(res);
                        code.detail.carriers.load(insuranceID);
                        code.detail.services.load(insuranceID);
                        code.detail.rules.load(insuranceID);
                    }
                });
            },

            initialize: function (insuranceID) {

                code.detail.form.initialize();

            },

            carriers: {
                
                addCarrier: function () {

                    App.DevEx.GetControl("btnSaveCarrierName").Click.AddHandler(function (s, e) {

                        var val = App.DevEx.GetControl("CarrierEditName").GetValue();
                        if (val !== null) {
                            code.detail.carriers.saveCarrier(null, code.list.currentID(), val);
                        } else {
                            App.Dialogs.MessageBox('Please enter a carrier name');
                            return;
                        }
                        App.DevEx.GetControl("CarrierEditName").SetValue(null);
                        App.DevEx.GetControl("CarrierEditPopup").Hide();
                        App.DevEx.GetControl("btnSaveCarrierName").Click.ClearHandlers();
                    });

                    App.DevEx.GetControl("CarrierEditPopup").Show();
                    
                },

                editCarrier: function (carrierID) {
                    
                    App.DevEx.GetControl("btnSaveCarrierName").Click.AddHandler(function (s, e) {

                        var val = App.DevEx.GetControl("CarrierEditName").GetValue();
                        if (val !== null) {
                            code.detail.carriers.saveCarrier(carrierID, code.list.currentID(), val);
                        } else {
                            App.Dialogs.MessageBox('Please enter a carrier name');
                            return;
                        }

                        App.DevEx.GetControl("CarrierEditName").SetValue(null);
                        App.DevEx.GetControl("CarrierEditPopup").Hide();
                        App.DevEx.GetControl("btnSaveCarrierName").Click.ClearHandlers();
                    });

                    code.detail.carriers.grid.currentCarrierName(function (carrierName) {
                        App.DevEx.GetControl("CarrierEditName").SetValue(carrierName);
                        App.DevEx.GetControl("CarrierEditPopup").Show();
                    });
                    
                },

                saveCarrier: function (id, insuranceID, carrierName) {

                    $.ajax({
                        type: 'POST',
                        url: '/Insurance/SaveCarrier',
                        data: {
                            id: id,
                            insuranceID: insuranceID,
                            carrierName: carrierName
                        },
                        success: function (res) {
                            if (res === "ok") {
                                code.detail.carriers.grid.get().Refresh();
                            } else {
                                App.Dialogs.Error();
                            }
                        }
                    });

                },

                load: function (insuranceID) {
                    $.ajax({
                        type: 'GET',
                        url: '/Insurance/CarrierItems',
                        data: { insuranceID: insuranceID },
                        success: function (res) {
                            $('#carriers-grid-container').empty().append(res);
                            code.detail.carriers.grid.initialize();
                        }
                    });
                },

                grid: {
                    currentID: function () {
                        var grid = code.detail.carriers.grid.get();
                        var vi = grid.GetFocusedRowIndex();
                        var id = grid.GetRowKey(vi);
                        return id;
                    },
                    currentCarrierName: function (callback) {
                        var grid = code.detail.carriers.grid.get();
                        var vi = grid.GetFocusedRowIndex();
                        grid.GetRowValues(vi, 'Name', function (values) {
                            callback(values);
                        });
                    },
                    removeSelected: function () {
                        if (code.detail.carriers.grid.currentID() === null) {
                            App.Dialogs.MessageBox("Please select a carrier to remove");
                            return;
                        } 
                        if (confirm("WARNING! Are you sure you want to remove this carrier?")) {
                            $.ajax({
                                type: 'POST',
                                url: '/Insurance/CarrierDelete',
                                data: {
                                    carrierID: code.detail.carriers.grid.currentID()
                                },
                                success: function (res) {
                                    if (res === "cant delete") {
                                        App.Dialogs.MessageBox("Unable to remove this carrier.  Likely there's existing data attached to it.");
                                    } else {
                                        code.detail.carriers.grid.get().Refresh();
                                    }                                    
                                }
                            });
                        }
                    },
                    get: function () {
                        return App.DevEx.GetControl("CarriersGrid");
                    },
                    initialize: function () {
                        var grid = code.detail.carriers.grid.get();
                        grid.EndCallback.AddHandler(function (s, e) { code.detail.carriers.grid.endCallback(); });
                        grid.BeginCallback.AddHandler(function (s, e) {
                            e.customArgs["insuranceID"] = code.list.currentID();
                        });
                        code.detail.carriers.grid.endCallback();
                    },
                    endCallback: function () {
                        $('[data-dym-cellClick="CarriersGrid"]').dblclick(function () {
                            var vi = $(this).attr('data-dym-visibleIndex');
                            var id = code.detail.carriers.grid.get().GetRowKey(vi);
                            code.detail.carriers.editCarrier(id);
                        });
                    }
                }

            },

            services: {

                load: function (insuranceID) {
                    $.ajax({
                        type: 'GET',
                        url: '/Insurance/ServiceItems',
                        data: { insuranceID: insuranceID },
                        success: function (res) {
                            $('#services-grid-container').empty().append(res);
                            code.detail.services.grid.initialize();
                        }
                    });
                },

                grid: {

                    currentID: function () {
                        var grid = code.detail.services.grid.get();
                        var vi = grid.GetFocusedRowIndex();
                        var id = grid.GetRowKey(vi);
                        return id;
                    },

                    removeSelected: function () {
                        if (code.detail.services.currentID() === null) {
                            App.Dialogs.MessageBox("Please select a service to remove");
                            return;
                        }
                        if (confirm("WARNING! Are you sure you want to remove this?  Hours entered by providers will no longer match to this service.")) {
                            $.ajax({
                                type: 'POST',
                                url: '/Insurance/ServiceDelete',
                                data: {
                                    insuranceServiceID: code.detail.services.grid.currentID()
                                },
                                success: function () {
                                    code.detail.services.grid.get().Refresh();
                                }
                            });
                        }
                    },

                    get: function () {
                        return App.DevEx.GetControl("ServicesGrid");
                    },

                    initialize: function () {
                        console.log('initializing grid');
                        var grid = code.detail.services.grid.get();
                        grid.EndCallback.AddHandler(function (s, e) { code.detail.services.grid.endCallback(); });

                        grid.BeginCallback.AddHandler(function (s, e) {
                            e.customArgs["insuranceID"] = code.list.currentID();
                        });

                        code.detail.services.grid.endCallback();
                    },

                    endCallback: function () {
                        $('[data-dym-cellClick="ServicesGrid"]').dblclick(function () {
                            var vi = $(this).attr('data-dym-visibleIndex');
                            var id = code.detail.services.grid.get().GetRowKey(vi);
                            code.detail.services.editor.set(id);
                        });
                    }
                },

                editor: {

                    currentID: null,

                    set: function (insuranceServiceID) {
                        $.ajax({
                            type: 'GET',
                            url: '/Insurance/ServiceEditItem',
                            data: {
                                insuranceServiceID: insuranceServiceID
                            },
                            success: function (res) {
                                $('#services-detail-container').empty().append(res);
                                code.detail.services.editor.initialize(insuranceServiceID);
                            }
                        });
                    },

                    initialize: function (insuranceServiceID) {
                        code.detail.services.editor.currentID = insuranceServiceID;
                        App.DevEx.GetControl("ServiceEditSaveButton").Click.AddHandler(function (s, e) {
                            code.detail.services.editor.save();
                        });
                    },

                    save: function () {

                        var editor = code.detail.services.editor;

                        var model = editor.model.get();
                        if (!editor.validate(model)) {
                            App.Dialogs.MessageBox("Please be sure all required fields are filled out.");
                            return;
                        }

                        $.ajax({
                            type: 'POST',
                            url: '/Insurance/ServiceEdit',
                            data: model,
                            success: function (res) {
                                code.detail.services.grid.get().Refresh();
                            }
                        });
                    },

                    validate: function (model) {
                        return true;
                        // TODO: add validation
                    },

                    model: {
                        get: function () {
                            var model = {};

                            model.ID = code.detail.services.editor.currentID;
                            model.InsuranceID = code.list.currentID();
                            model.ServiceID = App.DevEx.GetControl("ServiceEditServiceID").GetValue();
                            model.ProviderTypeID = App.DevEx.GetControl("ServiceEditProviderTypeID").GetValue();
                            model.EffectiveDate = App.DevEx.GetControl("ServiceEditEffectiveDate").GetValue();
                            model.DefectiveDate = App.DevEx.GetControl("ServiceEditDefectiveDate").GetValue();

                            if (model.EffectiveDate !== null) {
                                model.EffectiveDate = model.EffectiveDate.toISOString();
                            }
                            if (model.DefectiveDate !== null) {
                                model.DefectiveDate = model.DefectiveDate.toISOString();
                            }

                            return model;
                        }
                    }

                }

            },

            rules: {


                load: function (insuranceID) {

                    $.ajax({
                        type: 'GET',
                        url: '/Insurance/AuthRuleItems',
                        data: {
                            insuranceID: insuranceID
                        },
                        success: function (res) {
                            $('#auth-rules-grid-container').empty().append(res);
                            code.detail.rules.grid.initialize();
                        }
                    });

                },   // end code.detail.rules/load


                editor: {

                    currentID: null,

                    set: function (ruleID) {

                        $.ajax({
                            type: 'GET',
                            url: '/Insurance/AuthRuleEditItem',
                            data: {
                                ruleID: ruleID
                            },
                            success: function (res) {
                                $('#auth-rules-detail-container').empty().append(res);
                                code.detail.rules.editor.initialize(ruleID);
                            }

                        });
                    },  // end code.detail.rules.editor.set

                    initialize: function (ruleID) {
                        code.detail.rules.editor.currentID = ruleID;
                        App.DevEx.GetControl("AuthEditSaveButton").Click.AddHandler(function (s, e) {
                            code.detail.rules.editor.save();
                        });
                    },  // end code.detail.rules.editor.initialize

                    save: function () {

                        var editor = code.detail.rules.editor;

                        var model = editor.model.get();
                        if (!editor.validate(model)) {
                            App.Dialogs.MessageBox("Please be sure the Provider Type and Service are filled out and that only numbers are supplied for the units and minutes.");
                            return;
                        }

                        $.ajax({
                            type: 'POST',
                            url: '/Insurance/AuthRuleEdit',
                            data: model,
                            success: function (res) {
                                code.detail.rules.grid.get().Refresh();
                            }
                        });


                    },  // end code.detail.rules.editor.save

                    validate: function(model) {

                        if (model.ProviderTypeID == null) {
                            return false;
                        }

                        if (model.ServiceID == null) {
                            return false;
                        }

                        if (!(model.InitialMinimumMinutes == null || isInt(model.InitialMinimumMinutes))) {
                            return false;
                        }
                        
                        if (!(model.InitialUnitSize == null || isInt(model.InitialUnitSize))) {
                            return false;
                        }

                        if (!(model.FinalMinimumMinutes == null || isInt(model.FinalMinimumMinutes))) {
                            return false;
                        }

                        if (!(model.FinalUnitSize == null || isInt(model.FinalUnitSize))) {
                            return false;
                        }

                        return true;

                        function isInt(value) {
                            return !isNaN(value) &&
                                   parseInt(Number(value)) == value &&
                                   !isNaN(parseInt(value, 10));
                        }

                    },  // end code.detail.rules.editor.validate

                    model: {
                        get: function () {

                            var model = {};

                            model.ID = code.detail.rules.editor.currentID;
                            model.InsuranceID = code.list.currentID();
                            model.ProviderTypeID = App.DevEx.GetControl("AuthEditProviderType").GetValue();
                            model.ServiceID = App.DevEx.GetControl("AuthEditService").GetValue();
                            model.InitialAuthorizationID = App.DevEx.GetControl("AuthEditInitialAuth").GetValue();
                            model.InitialMinimumMinutes = App.DevEx.GetControl("AuthEditInitialMinimumMinutes").GetValue();
                            model.InitialUnitSize = App.DevEx.GetControl("AuthEditInitialUnitSize").GetValue();
                            model.FinalAuthorizationID = App.DevEx.GetControl("AuthEditFinalAuth").GetValue();
                            model.FinalMinimumMinutes = App.DevEx.GetControl("AuthEditFinalMinimumMinutes").GetValue();
                            model.FinalUnitSize = App.DevEx.GetControl("AuthEditFinalUnitSize").GetValue();
                            model.IsUntimed = App.DevEx.GetControl("AuthEditIsUntimed").GetValue();
                            model.AllowOverlapping = App.DevEx.GetControl("AuthEditAllowOverlapping").GetValue();
                            model.RequiresAuthorizedBCBA = App.DevEx.GetControl("AuthEditRequiresBCBA").GetValue();
                            model.RequiresPreAuthorization = App.DevEx.GetControl("AuthEditRequiresPreAuth").GetValue();

                            return model;
                        }
                    }   // end code.detail.rules.editor.model

                },

                grid: {

                    currentID: function() {
                        var grid = code.detail.rules.grid.get();
                        var vi = grid.GetFocusedRowIndex();
                        var id = grid.GetRowKey(vi);
                        return id;                        
                    },

                    removeSelected: function() {
                        if (code.detail.rules.grid.currentID() === null) {
                            App.Dialogs.MessageBox("Please select a rule to remove");
                            return;
                        }
                        if (confirm("WARNING!  Are you sure you want to remove this?  Service hours entered by providers will no longer match to this authorization.")) {
                            $.ajax({
                                type: 'POST',
                                url: '/Insurance/AuthRuleDelete',
                                data: {
                                    ruleID: code.detail.rules.grid.currentID()
                                },
                                success: function () {
                                    code.detail.rules.grid.get().Refresh();
                                }
                            });
                        }
                    },

                    get: function() {
                        return App.DevEx.GetControl("AuthRulesGrid");
                    },

                    initialize: function () {

                        var grid = code.detail.rules.grid.get();
                        grid.EndCallback.AddHandler(function (s, e) { code.detail.rules.grid.endCallback(); })

                        grid.BeginCallback.AddHandler(function (s, e) {
                            e.customArgs["insuranceID"] = code.list.currentID();
                        });

                        code.detail.rules.grid.endCallback();
                    },

                    endCallback: function () {

                        $('[data-dym-cellClick="AuthRulesGrid"]').dblclick(function () {
                            var vi = $(this).attr('data-dym-visibleIndex');
                            var id = code.detail.rules.grid.get().GetRowKey(vi);
                            code.detail.rules.editor.set(id);
                        });

                    }

                }


            },
            form: {
                initialize: function () {
                    $("#InsuranceFormSaveButton").on('click', function (bn) {
                        code.detail.form.save();
                    })
                },
                save: function () {
                    $.ajax({
                        type: 'POST',
                        url: '/Insurance/InsuranceForm',
                        data: $("#InsuranceForm").serialize(),
                        success: function (res) {
                            $('#insurance-form-container').empty().append(res);
                            code.detail.form.initialize();
                            App.DevEx.GetControl("InsurancesGrid").Refresh();
                        }
                    });
                }
            }


        }

    }


    code.preInit();

    window.Insurance = api;

})();
