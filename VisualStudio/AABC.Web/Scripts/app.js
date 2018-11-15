/******************************


====
!D = deprecated, see intellisense for new usage
!R = subject to refactor


==== MISC LOOSE ENDS TO BE REFACTORED ====

!R autoLoadingPanelIsSupressed;
!R autoLoadingPanelIsSingleSupression;

!R initView(url, displayName, state, navGroup, navGroupItem);


==== COMMON/CORE STUFF ====

Common.Dates.Convert(a);
Common.Dates.Compare(a, b);
Common.Dates.IsInRange(a, start, end);

Common.Email.IsValid(email);

Common.Objects.MergeBasic(o1, o2);


App.Navigate(url);

!R App.Navigation.NavBaseUrl;
!R App.Navigation.NavBackUrl;
!R App.Navigation.PushState(url, displayName, state);
!D App.Navigation.NavToUrl(url);
!D App.Navigation.NavTo(action, controller);

App.Hotkeys.Init();
App.Hotkeys.Search();
App.Hotkeys.Help();

App.Search.Universal();


App.Popup.Hide(response);
App.Popup.Show(options);

App.Sleep(ms);

App.Internal.Selectors.SelectedDate;
App.Internal.Selectors.SelectedDateCancelled;
App.Internal.DialogResults.ConfirmResult;
App.Internal.PopupResults.Result;

App.Selectors.GetDate(existingDate, fn);

App.LoadingPanel.Show();
App.LoadingPanel.Hide();
App.LoadingPanel.AutoPanel.SupressSingle();
App.LoadingPanel.AutoPanel.SupressAll();
App.LoadingPanel.AutoPanel.RevokeSupress();

App.Themes.Colors.TextColorDisabled;
App.Themes.Colors.TextColorBadStatus;

App.Dialogs.Icons.{enum};
App.Dialogs.MessageBox(msg, ico, title, showLoadingPanel);
App.Dialogs.Error(msg);
App.Dialogs.ConfirmDelete(options);
App.Dialogs.Warning(msg);
App.Dialogs.ValidationFailure(msg);

App.DevEx.GetControl(ctlName);
App.DevEx.GridView.GetFocusedRowKey(gvObj);
App.DevEx.DisabledControls.GetCheckboxValue(cbName);

!R App.Init(contentElementID, navBaseUrl, navBackUrl);

!R App.Content.ContentElementID;
App.Content.GridViews.InitializeForStretchHeight(gvObj);
App.Content.GridViews.StretchHeight(gvObj);

App.DataPage.InitSectionExpanders();


App.NavBar.Object;
App.NavBar.Init(objNavBar);
App.NavBar.SetSelectedItem(navGroupNane, navGroupItemName);


App.Settings.Shell.MainContentPane;
App.Settings.DataPage.ValidationMessageTransitionTime;
App.Settings.DataPage.SectionExpanderHeaderAreaTime;
App.Settings.DataPage.GridStretchLowerBuffer;
    

*******************/



// used by App.LoadingPanel.AutoPanel.Supress/Revoke methods
// see _rootlayout for ajax event binding 
var autoLoadingPanelIsSupressed = false;
var autoLoadingPanelIsSingleSupression = false;

var Common = {

    Dates: {
        
        Convert: function (d) {
            /// <signature>
            /// <summary>Converts the date in d to a date object</summary>
            /// <param name="d" type="date">The value to convert</param>
            /// <returns type="date">The value as submitted, unconverted</returns>
            /// </signature>
            
            /// <signature>
            /// <summary>Converts the date in d to a date object</summary>
            /// <param name="d" type="array">The array to convert.  [year, month, day] (month is 0-based)</param>
            /// <returns type="date">The converted date, or NaN</returns>
            /// </signature>

            /// <signature>
            /// <summary>Converts the date in d to a date object</summary>
            /// <param name="d" type="number">The value to convert, interpreted as timestamp (number of milliseconds since 1970/01/01</param>
            /// <returns type="date">The converted date, or NaN</returns>
            /// </signature>

            /// <signature>
            /// <summary>Converts the date in d to a date object</summary>
            /// <param name="d" type="string">The value to convert.  Any JavaScript valid date formatted string</param>
            /// <returns type="date">The converted date, or NaN</returns>
            /// </signature>

            /// <signature>
            /// <summary>Converts the date in d to a date object</summary>
            /// <param name="d" type="object">The value to convert.  Interpreted as object with year, month and day attributes (month is 0-based)</param>
            /// <returns type="date">The converted date, or NaN</returns>
            /// </signature>
            return (
                d.constructor === Date ? d :
                d.constructor === Array ? new Date(d[0], d[1], d[2]) :
                d.constructor === Number ? new Date(d) :
                d.constructor === String ? new Date(d) :
                typeof d === "object" ? new Date(d.year, d.month, d.date) : NaN
            );
        },

        Compare: function (a, b) {
            /// <summary>Compares any two dates as supported by Common.Dates.Convert()</summary>
            /// <returns type="number">-1: a < b; 0: a = b; 1: a > b or NaN on illegal dates</returns>
            return (
                isFinite(a = this.Convert(a).valueOf()) &&
                isFinite(b = this.Convert(b).valueOf()) ?
                (a > b) - (a < b) :
                NaN

            );
        },

        IsInRange: function (d, start, end) {
            /// <summary>Checks if date in d is between dates in start and end.  Returns bool or NaN on invalid</summary>
            return (
                 isFinite(d = this.Convert(d).valueOf()) &&
                 isFinite(start = this.Convert(start).valueOf()) &&
                 isFinite(end = this.Convert(end).valueOf()) ?
                 start <= d && d <= end :
                 NaN
             );
        }
    },

    Email: {
        IsValid: function (email) {
            /// <summary>Returns true if the email address appears to be a valid format, false otherwise</summary>
            var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        }
    },

    Objects: {
        MergeBasic: function (obj1, obj2) {
            /// <summary>Performs a shallow merge of obj1 and obj2 and returns the new merged object</summary>
            var obj3 = {};
            for (var attrname in obj1) { obj3[attrname] = obj1[attrname]; }
            for (var attrname in obj2) { obj3[attrname] = obj2[attrname]; }
            return obj3;
        }
    }
}



    
    





var App = {

    

    Search: {

        Universal: function () {
            
            App.Popup.Show({
                url: "/UniversalSearch/Search",
                options: {
                    width: 500,
                    height: 500,
                    title: "Search"
                },
                finished: function (r) {
                    
                    if (r == "cancelled") {
                        return;
                    }

                    // goto?

                },
                error: function (r) {
                    App.Dialogs.Error();
                }
            });

        }

    },

    Hotkeys: {
                
        Init: function () {
            /// <summary>Initializes the application hotkeys, call once inside the login area</summary>
            // https://craig.is/killing/mice
            
            Mousetrap.bind('/', function (e) { App.Hotkeys.Search() });
            Mousetrap.bind('?', function (e) { App.Hotkeys.Help() });


        },

        Search: function () {
            App.Search.Universal();
        },

        Help: function () {
            App.Dialogs.MessageBox("Press '/' to search");
        }



    },

    Navigate: function(url) {
        /// <summary>Loads the main content area with the specified URL.  If the navbar element is not found, a full page navigation is performed.</summary>

        var el = App.DevEx.GetControl("LeftNavBar");
        
        console.log(el);

        if (el == null) {
            window.location = url;
        } else {
            $.ajax({
                url: url,
                type: 'GET',
                success: function (r) {
                    $("#" + App.Content.ContentElementID).empty().append(r);
                },
                error: function () {
                    App.Dialogs.Error();
                }
            });
        }

        

    },
        
    Navigation: {
                
        NavBaseUrl: null,
        NavBackUrl: null,

        PushState: function (url, displayName, state) {
            window.history.pushState(state, displayName, url);
            App.Navigation.NB
        },

        NavToUrl: function (url) {
            /// <summary>(!!Deprecated, use App.Navigate()!!) Loads the main content area with the specified URL</summary>
            $.ajax({
                type: 'GET',
                url: url,
                success: function (response) {
                    $("#" + App.Content.ContentElementID).empty().append(response);
                },
                error: function (response) {
                    App.Dialogs.Error("We're sorry, but we seem to have encountered an issue loading this page.");
                }
            });
        },

        NavTo: function (action, controller) {
            /// <summary>(!!Deprecated, use App.Navigate()!!) Loads the main content area with the specified controller and action</summary>
            var url = "/" + controller + "/" + action;
            App.Navigation.NavToUrl(url);
        }
    },


    Popup: {

        Hide: function (response) {
            /// <summary>Hides the active popup and sends the response parameter value to the finished callback, if applicable</summary>
            App.Internal.PopupResults.Result = response;
            PopupGeneral.Hide();
        },

        Show: function (popupOptions) {

            // ref: http://manage.dymeng.com/kb/DevExMVCAJAXPopup
            

            var type = 'GET';
            try {
                type = popupOptions.type;
            } catch (ex) {
                // 
            }

            $.ajax({
                url: popupOptions.url,
                data: popupOptions.data,
                type: type,
                success: function (contentResponse) {
                    // now get the popup wrapper and inject this response into it

                    $.ajax({
                        url: "/Dialogs/PopupGeneral",
                        type: type,
                        data: popupOptions.options,
                        success: function (popupResponse) {

                            // place the popup response
                            $("#PopupAnchor").empty().append(popupResponse);
                            // place the content response
                            $("#PopupGeneralContent").empty().append(contentResponse);
                            // link up events
                            PopupGeneral.CloseUp.AddHandler(function (s, e) {
                                try {
                                    popupOptions.finished(App.Internal.PopupResults.Result);
                                } catch (err) {
                                    
                                }
                                
                            });

                            // show the popup
                            PopupGeneral.Show();

                        },
                        error: function () {
                            popupOptions.error();
                        }
                    });

                },
                error: function () {
                    popupOptions.error();
                }
            });

        }

    },

    Sleep: function(milliseconds) {
        // WARNING!  This is a blocking wait loop
        // use wisely, it'll block the UI/browser while looping
        var date = new Date();
        var curDate = null;
        do { curDate = new Date(); }
        while (curDate - date < milliseconds);
    },

    Internal: {
        Selectors: {
            SelectedDate: null,
            SelectedDateCancelled: false
        },
        DialogResults: {
            ConfirmResult: null
        },
        PopupResults: {
            Result: null
        }
    },


    Selectors: {

        GetDate: function (existingDate, callback) {

            var date = null;
            if (existingDate != null) {
                date = existingDate.toISOString();
            }

            $.ajax({
                type: 'GET',
                url: '/Selectors/GetDate',
                data: { existingDate: date },

                success: function (r) {

                    $("#PopupAnchor").html(r);

                    dxDateSelectorPopup.CloseUp.AddHandler(function (s, e) {

                        switch (e.closeReason) {
                            case "Escape":
                            case "OuterMouseClick":
                                App.Internal.Selectors.SelectedDateCancelled = true;
                                break;
                        }

                        if (!App.Internal.Selectors.SelectedDateCancelled) {
                            var newDate = App.Internal.Selectors.SelectedDate;
                            callback(newDate);
                        }

                    });

                    dxDateSelectorPopup.Show();

                },
                error: function () {
                    App.Dialogs.Error("We're sorry, but we seem to have encountered an error with that request.  Please contact your administrator if the problem continues.");
                }
            });

        }

    },

    LoadingPanel: {
        // LoadingPanel object in these functions is set in 
        // _rootLayout (devex loading panel)
        Show: function() {
            LoadingPanel.Show();
        },
        Hide: function() {
            LoadingPanel.Hide();
        },

        AutoPanel: {

            SupressSingle: function () {
                autoLoadingPanelIsSupressed = true;
                autoLoadingPanelIsSingleSupression = true;
            },
            SupressAll: function () {
                autoLoadingPanelIsSupressed = true;
                autoLoadingPanelIsSingleSupression = false;
            },
            RevokeSupress: function () {
                autoLoadingPanelIsSupressed = false;
                autoLoadingPanelIsSingleSupression = false;
            }
        }

    },


    Themes: {
        Colors: {
            TextColorDisabled: "#b1b1b8",
            TextColorBadStatus: '#ff6666'
        }
    },

    Dialogs: {
        
        Icons: {
            Information: 0,
            Critical: 1,
            Question: 2,
            Ok: 3,
            Warning: 4,
            None: 5,
            Save: 6
        },


        MessageBox: function (message, icon, title, showLoadingPanel) {

            if (!showLoadingPanel) {
                App.LoadingPanel.AutoPanel.SupressSingle();
            }

            $.ajax({
                type: 'GET',
                url: '/Dialogs/MessageBox',
                data: { message: message, icon: icon, title: title },
                success: function (r) {
                    $("#MessageBoxAnchor").html(r);
                    MessageBox.Show();
                },
                error: function () {
                    App.Dialogs.Error("We're sorry, but we seem to have encountered an error with that request.  Please contact your administrator if the problem continues.");
                }
            });
        },

        Error: function(message) {

            if (message == undefined || message == null || message == "") {
                message = "We're sorry, but we seems to have encountered an error with that request.  Please contact your administrator if the problem continues.";
            }

            $.ajax({
                type: 'GET',
                url: '/Dialogs/Error',
                data: { message: message },
                success: function (r) {
                    $("#ErrorAnchor").html(r);
                    ErrorPopup.Show();
                },
                error: function () {
                    alert("We're sorry, but we seem to have encountered an error with that request... please contact your administrator if the problem continues.");
                }

            });
        },

        ConfirmDelete: function (options) {

            $.ajax({

                type: 'GET',
                url: '/Dialogs/ConfirmDeleteV2',
                data: { contentElementID: App.Content.ContentElementID, message: options.message },

                success: function (r) {

                    $("#DialogAnchor").html(r);
                    
                    DialogConfirm.CloseUp.AddHandler(function (s, e) {
                        if (App.Internal.DialogResults.ConfirmResult == 'button1') {
                            options.confirmed();
                        } else {
                            try {
                                options.cancelled();
                            } catch (err) {
                                // nothing
                            }
                            
                        }
                    });

                    DialogConfirm.Show();
                },

                error: function () {
                    App.Dialogs.Error();
                }

            });

        },

        

        Warning: function(message) {
            App.Dialogs.MessageBox(message, App.Dialogs.Icons.Warning, "Warning");
        },

        ValidationFailure: function (message) {
            
            if (message == undefined) {
                message = 'Unable to proceed.  Please ensure all fields are validated.'
            }
            App.Dialogs.MessageBox(message, App.Dialogs.Icons.Warning, "Validation Failure");
            
        }
        
    },


    DevEx: {

        GetControl: function (controlName) {
            return ASPxClientControl.GetControlCollection().GetByName(controlName);
        },

        GridView: {

            GetFocusedRowKey: function(gridView) {
                return gridView.GetRowKey(gridView.GetFocusedRowIndex());
            }

        },

        DisabledControls: {

            GetTextboxValue: function(controlName) {
                var v = $("input[name=" + controlName + "]").val();
                return v;
            },

            GetCheckboxValue: function (checkboxName) {
                var v = $("input[name=" + checkboxName + "]").val();
                if (v == "C") {
                    return true;
                } else {
                    return false;
                }
            }

        }


    },

    
    Init: function (contentElementID, navBaseUrl, navBackUrl) {
        // call this from the ASP.NET Razor code to 
        // provider razor engine variables for use in
        // external js files

        App.Content.ContentElementID = contentElementID;
        App.Navigation.NavBaseUrl = navBaseUrl;
        App.Navigation.NavBackUrl = navBackUrl;
        

        // safari seems to fire this on initial page load, which causes a redirect loop
        // ideally would be a flag setting to note that we're not on page loop
        // (http://stackoverflow.com/questions/10756893/how-to-ignore-popstate-initial-load-working-with-pjax)
        // but for now we'll just set a timeout for the initial load
        setTimeout(function () {
            window.onpopstate = function (event) {

                var placehoder = document.getElementById(contentElementID);
                if (placehoder === null) {
                    // force full refresh
                    window.location.reload();
                } else {

                    // update the main content area
                    $.ajax({
                        type: 'POST',
                        url: navBackUrl,
                        data: { routeUrl: window.location.pathname },
                        success: function (response) {
                            $("#" + contentElementID).html(response);
                        },
                        error: function (response) {
                            App.Dialogs.Error("We're sorry, but we seem to have encountered an error loading this page.");
                        }
                    });
                }
            }
        }, 1000);


    },

    Content: {

        ContentElementID: null,

        GetContentClientHeight: function () {
            var pane = App.Settings.Shell.MainContentPane;
            return MainSplitter.GetPane(pane).GetClientHeight();
        },

        GridViews: {

            InitializeForStretchHeight: function(gridView, negativeBuffer) {
                App.Content.GridViews.StretchHeight(gridView, negativeBuffer);
                window.addEventListener("resize", function (event) { App.Content.GridViews.StretchHeight(gridView, negativeBuffer); });
            },

            StretchHeight: function (gridView, negativeBuffer) {
                var nBuf = negativeBuffer || 0;
                var buffer = App.Settings.DataPage.GridStretchLowerBuffer;
                gridView.SetHeight(App.Content.GetContentClientHeight() - buffer + nBuf);
            }
        }
    },

    DataPage: {

        InitSectionExpanders: function () {

            var sectionExpanderHeader = $(".section-expander-header");

            sectionExpanderHeader.click(function () {

                var content = $(this).next();
                var t = App.Settings.DataPage.SectionExpanderHeaderAreaTime;

                if (content.is(':hidden')) {
                    content.show(t);
                } else {
                    content.hide(t);
                }
            });

            sectionExpanderHeader.hover(function () {
                $(this).css('cursor', 'pointer');
            });

        }
    },


    

    NavBar: {

        Object: null,

        Init: function(navBar) {
            App.NavBar.Object = navBar;
        },

        // not yet used, convert view initialization script to point here
        SetSelectedItem: function (navGroupName, navGroupItemName) {
            var g = App.NavBar.Object.GetGroup(navGroupName);
            var i = App.NavBar.Object.GetItem(navGroupItemName);
            App.NavBar.Object.SetSelectedItem(i);
        },

        InitView: function(state) {
            if (state.PushState && state.PushStateRoute) {
                window.history.pushState(null, state.PushStateTitle, state.PushStateRoute);
            }

            try {
                var navBarItems = state.NavBarRoute.split('/'),
                    group = App.NavBar.Object.GetGroupByName(navBarItems[1]),
                    item = group.GetItemByName(navBarItems[2]);

                group.SetExpanded(true);
                App.NavBar.Object.SetSelectedItem(item);
            } catch (err) {
                console.log("ERROR: " + err.message);
            }
        }
    },

    
    Settings: {

        Shell: {
            MainContentPane: 1
        },

        DataPage: {
            ValidationMessageTransitionTime: 200,
            SectionExpanderHeaderAreaTime: 200,
            GridStretchLowerBuffer: 2
        }

    }

}
