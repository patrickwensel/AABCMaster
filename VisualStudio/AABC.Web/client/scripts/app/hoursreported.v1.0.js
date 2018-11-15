// hoursedit.js

(function () {

    var Interface = {
        Initialize: function () { hours.initialize(); },
        Grid: {
            SelfInitialize: function (s, e) {
                hours.grid.selfInit(s);
            },

            SelectionChanged: function (s, e) {
                hours.grid.selectionChanged(s, e);
            },

            ScrubSelected: function () {
                hours.grid.scrubSelected();
            }
        },
        Editor: {
            Submit: function() {hours.editor.submit();}
        }
    }


    var hours = {

        initialize: function () {

            console.log('hours edit initializing');

        },	// end hours.initialize



        editor: {

            show: function (id) {

                // id is the hours ID to show

                App.Popup.Show({
                    url: "/Hours/ResolvePopup",
                    data: {
                        id: id
                    },
                    options: {
                        width: 350,
                        height: 200,
                        title: 'Resolve Hours',
                        allowDrag: true,
                        allowResize: false
                    },
                    opened: function () {
                        // doesn't seem to work, made explicit call from popup instead
                    },
                    done: function (r) {

                    },
                    error: function (r) {
                        App.Dialogs.Error();
                    }
                });

            },   // end hours.editor.show

            submit: function () {

                var id = App.DevEx.DisabledControls.GetTextboxValue("ctlID");


                $.ajax({
                    type: 'post',
                    url: '/Hours/ResolveSubmit',
                    data: $("#ResolvePopupForm").serialize(),
                    success: function (res) {
                        App.Popup.Hide('ok');
                    },
                    error: function (res) {
                        App.Dialogs.Error(res);
                    }
                });


            }   // end hours.edit.submit


        },  // end hours.editor

        grid: {

            selfInit: function (grid) {

                console.log('grid says it is initialized');

                App.Content.GridViews.InitializeForStretchHeight(grid);

                grid.EndCallback.AddHandler(function (s, e) {

                    App.Content.GridViews.InitializeForStretchHeight(s);
                    $("[data-dym-cellClick='HoursEditGrid']").dblclick(function () { cellClick($(this)); });

                });

                $("[data-dym-cellClick='HoursEditGrid']").dblclick(function () { cellClick($(this)); });

                function cellClick(cell) {
                    window.setTimeout(function () {
                        var key = App.DevEx.GridView.GetFocusedRowKey(grid)
                        hours.editor.show(key);
                    }, 50);
                }

            },

            selectionChanged: function (s, e) {
                s.GetSelectedFieldValues("ID", this.selectionChangedCallback);
            },

            selectionChangedCallback: function (values) {
                $("#hoursGridSelected").val(values.join());
            },

        },  // end hours.grid


    }	// end hours

    window.HoursReported = Interface;

})();