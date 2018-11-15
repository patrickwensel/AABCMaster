// Scripts/Pages/StaffingLogDetails.js
(function () {
    var api = {
        List: {
            Initialize: function() { code.list.initialize(); }
        },
        Detail: {
            Go: function(staffingLogProviderId) { code.detail.go(staffingLogProviderId); },
            Initialize: function(staffingLogProviderId) { code.detail.initialize(staffingLogProviderId); }
            //Submit: function () {
            //    console.log('submit');
            //    var $form = $("#detailsForm");
            //    $form.submit(function (e) {
            //        e.preventDefault();
            //        $.ajax({
            //            url: $form.attr("action"),
            //            type: $form.attr("method"),
            //            data: $form.serialize()
            //        }).done(function () {
            //            alert("submitted");
            //        });
            //    });
            //}
            //AddAuthRule: function () { code.detail.rules.editor.set(null); },
            //RemoveSelectedAuthRule: function () { code.detail.rules.grid.removeSelected(); }
        },
        ProviderContactLog: {
            list: null,
            bcbaList: null,
            caseId: null,
            providerTypes: {
                AIDE: 17,
                BCBA: 15
            },
            Init: function(listSel, bcbaListSel, caseId) {
                this.list = $(listSel) || this.list;
                this.bcbaList = $(bcbaListSel) || this.list;
                this.caseId = caseId || this.caseId;
                this.Load();
            },
            Load: function () {
                var self = this;
                var loadLog = function (list, type) {
                    $.getJSON("/Staffing/LoadProviderContactLog?caseId=" + self.caseId + "&type=" + type)
                        .done(function (data) {
                            if (!data.length) {
                                list.html('<li>[no records]</li>');
                                return;
                            }

                            var items = [];
                            for (var i = 0; i < data.length; i++) {
                                var item = data[i].ContactDate + ' | ' + data[i].ProviderName + ' (' + data[i].ProviderTypeCode + ') | ' + data[i].Status;
                                if (data[i].FollowUpDate) {
                                    item += ' | Follow up on: ' + data[i].FollowUpDate;
                                }

                                if (data[i].Notes) {
                                    item += '<br />' + data[i].Notes;
                                }

                                items.push('<li>' + item + '</li>');
                            }

                            list.html(items.join(''));
                        })
                        .fail(function () {
                            App.Dialogs.Error();
                        });
                };

                loadLog(self.list, self.providerTypes.AIDE);
                loadLog(self.bcbaList, self.providerTypes.BCBA);
            },
            Add: function(type) {
                App.Popup.Show({
                    url: "/Staffing/AddProviderContactLog?caseId=" + this.caseId + "&type=" + type,
                    options: { width: 300, height: 100, title: "Contact Provider" },
                    error: App.Dialogs.Error
                });
            },
            Submit: function () {
                var self = this,
                    form = $("#AddProviderContactLogForm");
                
                $.ajax({
                    type: "POST",
                    url: form.prop("action"),
                    data: form.serialize(),
                    success: function () {
                        $("ul.validation-general", form).remove();
                        App.Popup.Hide();
                        self.Load();
                    },
                    error: function (response) {
                        var errors = code.getErrorsFromResponse(response);
                        $("ul.validation-general", form).remove();
                        form.prepend("<ul class='validation-general'><li>" + errors.join("</li><li>") + "</li></ul>");
                    }
                });
            }
        },
        ParentContactLog: {
            list: null,
            caseId: null,
            Init: function (listSel, caseId) {
                this.list = $(listSel) || this.list;
                this.caseId = caseId || this.caseId;
                this.Load();
            },
            Load: function () {
                var self = this;
                $.getJSON("/Staffing/LoadParentContactLog?caseId=" + this.caseId)
                    .done(function (data) {
                        if (!data.length) {
                            self.list.html('<li>[no records]</li>');
                            return;
                        }

                        var items = [];
                        for (var i = 0; i < data.length; i++) {
                            var item = data[i].ContactDate + ' | via ' + data[i].ContactMethod + ' | ' + data[i].ContactedPerson;
                            
                            if (data[i].Notes) {
                                item += '<br />' + data[i].Notes;
                            }

                            items.push('<li>' + item + '</li>');
                        }

                        self.list.html(items.join(''));
                    })
                    .fail(function() {
                        App.Dialogs.Error();
                    });
            },
            Add: function () {
                App.Popup.Show({
                    url: "/Staffing/AddParentContactLog?caseId=" + this.caseId,
                    options: { width: 300, height: 100, title: "Contact Parent" },
                    error: App.Dialogs.Error
                });
            },
            ContactMethodChanged: function (source) {
                var methodSelected = source.GetValue();
                App.DevEx.GetControl("ContactMethodValue")
                    .SetCaption(methodSelected == 0 ? "Phone Number" : "Email Address");
            },
            Submit: function () {
                var self = this,
                    form = $("#AddParentContactLogForm");

                $.ajax({
                    type: "POST",
                    url: form.prop("action"),
                    data: form.serialize(),
                    success: function () {
                        $("ul.validation-general", form).remove();
                        App.Popup.Hide();
                        self.Load();
                    },
                    error: function (response) {
                        var errors = code.getErrorsFromResponse(response);
                        $("ul.validation-general", form).remove();
                        form.prepend("<ul class='validation-general'><li>" + errors.join("</li><li>") + "</li></ul>");
                    }
                });
            }
        }
    };
    var code = {
        preInit: function () {
        },
        list: {
            currentID: function () {
                var grid = App.DevEx.GetControl("StaffingLogProviderGrid");
                var index = grid.GetFocusedRowIndex();
                var id = grid.GetRowKey(index);
                return id;
            },
            initialize: function () {
                var grid = App.DevEx.GetControl("StaffingLogProviderGrid");
                code.list.linkCellDoubleClick();
                grid.EndCallback.AddHandler(function (s, e) {
                    code.list.linkCellDoubleClick();
                });
            },
            linkCellDoubleClick: function () {
                $('[data-dym-cellClick="StaffingLogProviderGrid"]').dblclick(function () {
                    var visibleIndex = $(this).attr('data-dym-visibleIndex');
                    var staffingLogProviderId = App.DevEx.GetControl("StaffingLogProviderGrid").GetRowKey(visibleIndex);
                    api.Detail.Go(staffingLogProviderId);
                });
            }
        },
        getErrorsFromResponse: function (response) {
            var title = response.responseText.match(/<title>(.+)<\/title>/g)[0];
            var errorsRegex = /Error:\s*([ a-zA-Z0-9_\.]*)/g;
            var matches = errorsRegex.exec(title),
                errors = [];

            while (matches) {
                errors.push(matches[1]);
                matches = errorsRegex.exec(title);
            }

            return errors;
        }
    };
    code.preInit();
    window.StaffingLogDetails = api;
})();
