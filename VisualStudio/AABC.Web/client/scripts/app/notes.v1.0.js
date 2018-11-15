// hoursedit.js
(function () {
    var taskCount = 1;
    var Interface = {
        NotesEdit: {
            Initialize: function (containerElementId) { notesEdit.initialize(containerElementId); },
            EditSubmit: function (s) { notesEdit.EditSubmit(s); },
            AddTask: function (s) { notesEdit.AddTask(s); }
        },
        NotesList: {
            Initialize: function (containerElementId) { notesList.initialize(containerElementId); },
            Config: function () { return notesList.Config(); },
            FollowupComplete: function (id) { return notesList.FollowupComplete(id); },
            FollowupCompleteClose: function () { return notesList.FollowupCompleteClose(); },
            ToggleTaskCheck: function (s) {
                var $element = $(s.mainElement);
                var taskId = $element.attr("taskId");
                var sourceType = $element.attr("sourceType");
                return notesList.ToggleTaskCheck(taskId, sourceType);
            },
            ToggleTaskCheck2: function (taskId, sourceType) {
                return notesList.ToggleTaskCheck(taskId, sourceType);
            }
        }
    };
    var notesEdit = {
        initialize: function (config) {
            notesEdit.config = $.extend(true, new notesEdit.Config(), config);
            //notesEdit.tasks.initializeButtons();
            //notesEdit.tasks.grid.initialize();
        },
        config: null,
        Config: function () {
            return {
                parentID: null,
                notesID: null
            };
        },
        AddTask: function (s) {
            var $element = $(s.mainElement);
            var sourceType = $element.attr("sourceType") || "";
            $.ajax({
                type: "GET",
                url: "/" + sourceType + "Notes/TaskEditRow?taskId=0",
                success: function (data) {
                    $("#task-list-table").append(data);
                }
            });
            taskCount++;
        },
        EditSubmit: function (s) {
            var $element = $(s.mainElement);
            var isModal = $element.attr("isModal") === "true" || false;
            var sourceType = $element.attr("sourceType");
            // for MVC binding to work, all these need to have the same name
            // but for DevEx controls to work, they need different names,
            // so we change them all at the last minute.
            $(".TaskRow").each(function (i, r) {
                $(r).find(".TaskID").attr("name", "Tasks[" + i + "].ID");
                $(r).find(".TaskDescription input").attr("name", "Tasks[" + i + "].Description");
                $(r).find(".TaskDueDate input").attr("name", "Tasks[" + i + "].DueDate");
                $(r).find(".TaskAction").attr("name", "Tasks[" + i + "].Action");
                $(r).find("[name$=_VI]").attr("name", "Tasks[" + i + "].AssignedTo");
            });
            $.ajax({
                type: "POST",
                url: "/" + sourceType + "Notes/Save",
                data: $("#NoteEditForm").serialize(),
                success: function (data) {
                    $("#noteEditContainer").html(data);
                    notesEdit.EditSuccess(sourceType);
                    if (isModal) {
                        App.Popup.Hide();
                    }
                }
            });
        },
        EditSuccess: function (sourceType) {
            var tmpParentID = $("#ParentID").val();
            $.ajax({
                type: "GET",
                url: "/" + sourceType + "Notes/GetNewNote",
                data: {
                    parentID: tmpParentID
                },
                success: function (res) {
                    $("#notes-list-container").prepend(res);
                    $("#notes-list-container div:first").hide();
                    $("#notes-list-container div:first").slideDown("slow");
                }
            });
        }
    };   // end notesEdit
    var notesList = {
        config: null,
        initialize: function (config) {
            notesList.config = $.extend(true, new notesList.Config(), config);
            // if we have a scroll container, stretch it's height
            if (config.stretch.enabled) {
                notesList.layout.stretch();
                window.addEventListener("resize", function () { notesList.layout.stretch(); });
            }
        },  // end notesList.initialize
        ToggleTaskCheck: function (taskId, sourceType) {
            App.Popup.Show({
                url: "/" + sourceType + "Notes/TaskCompleteForm?TaskId=" + taskId,
                options: {
                    width: 500,
                    height: 400,
                    title: "Task Complete"
                },
                finished: function () {
                    //refresh grid here.
                    $.ajax({
                        type: "GET",
                        url: "/" + sourceType + "Notes/NotesSummaryList/?parentID=" + notesList.config.parentID,
                        data: $("#CaseNoteTaskCompleteForm").serialize(),
                        success: function (data) {
                            $("#case-manage-notes-list-container").html(data);
                        }
                    });
                    App.Popup.Hide();
                },
                error: function () {
                    App.Dialogs.Error();
                }
            });
        },
        ToggleTaskSave: function () { },
        layout: {
            stretch: function () {
                if (notesList.config.stretch.devExClientAreaResolution) {
                    var clientHeight = App.Content.GetContentClientHeight();
                    var offset = notesList.config.stretch.topOffset;
                    $("#notes-list-container").height(clientHeight - offset);
                } else {
                    console.log("non-devex resolution not supported");
                }
            }
        },  // end notesList.layout
        Config: function () {
            return {
                anchorElementId: null,
                stretch: {
                    enabled: false,
                    devExClientAreaResolution: true,
                    topOffset: 0
                }
            };
        },   // end notesList.Config
        FollowupComplete: function (id, sourceType) {
            App.Popup.Show({
                url: "/" + sourceType + "Notes/FollowupCompletePopup",
                data: {
                    noteID: id
                },
                options: {
                    width: 400,
                    height: 200,
                    title: "Followup Complete Note"
                },
                finished: function () {
                    TimeBill.Scrub.Grid.Object.Refresh();
                },
                error: function (response) {
                    App.Dialogs.Error(response);
                }
            });
        },
        FollowupCompleteClose: function () {
            App.Popup.Hide("ok");
        }
    };   // end notesList
    window.Notes = Interface;
})();