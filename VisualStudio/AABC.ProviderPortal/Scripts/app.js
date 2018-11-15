// used by App.LoadingPanel.AutoPanel.Supress/Revoke methods
// see _rootlayout for ajax event binding 
var autoLoadingPanelIsSupressed = false;
var autoLoadingPanelIsSingleSupression = false;


// refactor this into the App namespace
function initView(url, displayName, state, navGroup, navGroupItem) {

    //console.log("initView: " + url + ', ' + displayName + ", " + state + ", " + navGroup + ", " + navGroupItem);

    if (url !== null) {
        window.history.pushState(state, displayName, url);
    }

    try {
        //debugger;
        var group = LeftNavBar.GetGroup(navGroup);
        var item = group.GetItem(navGroupItem);
        LeftNavBar.SetSelectedItem(item);
        //console.log('set selected item: ' + item.GetText() + ' (' + group.GetText() + ')');

    } catch (err) {
        console.log("ERROR: " + err.message);
    }

}





var Common = {

    Email: {
        IsValid: function (email) {
            var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        }
    },

    Objects: {
        MergeBasic: function (obj1, obj2) {
            // WARNING: performs only shallow copy, and will overwrite obj1 properties if same existings in obj2
            var obj3 = {};
            for (var attrname in obj1) { obj3[attrname] = obj1[attrname]; }
            for (var attrname in obj2) { obj3[attrname] = obj2[attrname]; }
            return obj3;
        }
    },

    IsNumeric: function(n) {
        return !isNaN(parseFloat(n)) && isFinite(n);
    }
}




var App = {


    Navigate: function(url) {

        $.ajax({
            url: url,
            type: 'GET',
            success: function(r) {
                $("#" + App.Content.ContentElementID).empty().append(r);
            },
            error: function () {
                App.Dialogs.Error();
            }
        })

    },


    Popup: {

        Hide: function (response) {
            /// <summary>Hides the active popup and sends the response parameter value to the finished callback, if applicable</summary>
            App.Internal.PopupResults.Result = response;
            PopupGeneral.Hide();
        },

        Show: function (popupOptions) {

            // ref: http://manage.dymeng.com/kb/DevExMVCAJAXPopup

            $.ajax({
                url: popupOptions.url,
                data: popupOptions.data,
                success: function (contentResponse) {
                    // now get the popup wrapper and inject this response into it

                    $.ajax({
                        url: "/Dialogs/PopupGeneral",
                        data: popupOptions.options,
                        success: function (popupResponse) {

                            // place the popup response
                            $("#PopupAnchor").empty().append(popupResponse);
                            // place the content response
                            $("#PopupGeneralContent").empty().append(contentResponse);
                            // link up events
                            PopupGeneral.CloseUp.AddHandler(function (s, e) {
                                popupOptions.finished(App.Internal.PopupResults.Result);
                            });

                            // show the popup
                            PopupGeneral.Show();

                            // run the loaded callback if we have one
                            try {
                                popupOptions.loaded();
                            } catch (err) {
                                // ignore
                            }

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
                            try {
                                options.confirmed();
                            } catch (err) {
                                // nothing
                            }
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



    /***** OBJECTS *****/

    Content: {

        ContentElementID: null,

        GetContentClientHeight: function () {
            var pane = App.Settings.Shell.MainContentPane;
            return MainSplitter.GetPane(pane).GetClientHeight();
        },

        GridViews: {

            InitializeForStretchHeight: function(gridView) {
                App.Content.GridViews.StretchHeight(gridView);
                window.addEventListener("resize", function (event) { App.Content.GridViews.StretchHeight(gridView); });
            },

            StretchHeight: function (gridView) {
                var buffer = App.Settings.DataPage.GridStretchLowerBuffer;
                gridView.SetHeight(App.Content.GetContentClientHeight() - buffer);
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


    Navigation: {

        NavBaseUrl: null,
        NavBackUrl: null,

        PushState: function (url, displayName, state) {
            window.history.pushState(state, displayName, url);
        },
                
        NavToUrl: function(url) {
            // show a page based on a url
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
            // show a page based on controller and action

            $.ajax({
                type: 'GET',
                url: "/" + controller + "/" + action,
                success: function (response) {
                    $("#" + App.Content.ContentElementID).empty().append(response);
                },
                error: function (response) {
                    App.Dialogs.Error("We're sorry, but we seem to have encountered an issue loading this page.");
                }
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

    },
    
    ZZDummy: null
}




// Source: http://stackoverflow.com/questions/497790
var dates = {

    RoundToMinutes: function(d, mins) {

        var coeff = 1000 * 60 * mins;
        return new Date(Math.round(d.getTime() / coeff) * coeff);

    },

    convert: function (d) {
        // Converts the date in d to a date-object. The input can be:
        //   a date object: returned without modification
        //  an array      : Interpreted as [year,month,day]. NOTE: month is 0-11.
        //   a number     : Interpreted as number of milliseconds
        //                  since 1 Jan 1970 (a timestamp) 
        //   a string     : Any format supported by the javascript engine, like
        //                  "YYYY/MM/DD", "MM/DD/YYYY", "Jan 31 2009" etc.
        //  an object     : Interpreted as an object with year, month and date
        //                  attributes.  **NOTE** month is 0-11.
        return (
            d.constructor === Date ? d :
            d.constructor === Array ? new Date(d[0], d[1], d[2]) :
            d.constructor === Number ? new Date(d) :
            d.constructor === String ? new Date(d) :
            typeof d === "object" ? new Date(d.year, d.month, d.date) :
            NaN
        );
    },
    compare: function (a, b) {
        // Compare two dates (could be of any type supported by the convert
        // function above) and returns:
        //  -1 : if a < b
        //   0 : if a = b
        //   1 : if a > b
        // NaN : if a or b is an illegal date
        // NOTE: The code inside isFinite does an assignment (=).
        return (
            isFinite(a = this.convert(a).valueOf()) &&
            isFinite(b = this.convert(b).valueOf()) ?
            (a > b) - (a < b) :
            NaN
        );
    },
    inRange: function (d, start, end) {
        // Checks if date in d is between dates in start and end.
        // Returns a boolean or NaN:
        //    true  : if d is between start and end (inclusive)
        //    false : if d is before start or after end
        //    NaN   : if one or more of the dates is illegal.
        // NOTE: The code inside isFinite does an assignment (=).
        return (
             isFinite(d = this.convert(d).valueOf()) &&
             isFinite(start = this.convert(start).valueOf()) &&
             isFinite(end = this.convert(end).valueOf()) ?
             start <= d && d <= end :
             NaN
         );
    }
}