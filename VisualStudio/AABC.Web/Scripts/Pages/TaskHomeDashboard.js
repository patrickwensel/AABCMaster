(function () {
    var api = {
        Todo: {
            Initialize: function () { todo.list.initialize(); }
        }
    };
    var todo = {
        preInit: function () { },
        list: {
            currentID: function () {
                var grid = App.DevEx.GetControl("TaskUserDashboardTodoGrid");
                var index = grid.GetFocusedRowIndex();
                var id = grid.GetRowKey(index);
                return id;
            },
            initialize: function () {
                var grid = App.DevEx.GetControl("TaskUserDashboardTodoGrid");
                todo.list.linkCellDoubleClick();
                grid.EndCallback.AddHandler(function () {
                    todo.list.linkCellDoubleClick();
                });
            },
            linkCellDoubleClick: function () {
                $("[data-dym-cellClick='TaskUserDashboardTodoGrid']").dblclick(function () {
                    var cell = $(this);
                    var index = cell.attr("data-dym-visibleIndex");
                    var field = cell.attr("data-dym-fieldName");
                    var sourceType = cell.attr("data-dym-sourceType");
                    var id = App.DevEx.GetControl("TaskUserDashboardTodoGrid").GetRowKey(index);
                    if (field !== "Completed") {
                        todo.list.editNote(id, sourceType);
                    }
                });
                $("[data-dym-cellClick='TaskUserDashboardTodoGrid']").click(function () {
                    var cell = $(this);
                    var index = cell.attr("data-dym-visibleIndex");
                    var field = cell.attr("data-dym-fieldName");
                    var sourceType = cell.attr("data-dym-sourceType");
                    var id = App.DevEx.GetControl("TaskUserDashboardTodoGrid").GetRowKey(index);
                    if (field === "Completed") {
                        todo.list.complete(id, sourceType);
                    }
                });
            },
            editNote: function (taskId, sourceType) {
                App.Popup.Show({
                    url: "/" + sourceType + "Notes/NoteEditFromTask?taskId=" + taskId + "&isModal=true",
                    options: {
                        width: 500,
                        height: 400,
                        title: "Note Edit"
                    },
                    finished: function () {
                        todo.list.refresh(sourceType);
                        App.Popup.Hide();
                    },
                    error: function () {
                        App.Dialogs.Error();
                    }
                });
            },
            complete: function (taskId, sourceType) {
                App.Popup.Show({
                    url: "/" + sourceType + "Notes/TaskCompleteForm?TaskId=" + taskId,
                    options: {
                        width: 500,
                        height: 400,
                        title: "Task Complete"
                    },
                    finished: function () {
                        //refresh grid here.
                        todo.list.refresh();
                        App.Popup.Hide();
                    },
                    error: function () {
                        App.Dialogs.Error();
                    }
                });
            },
            refresh: function () {
                $.ajax({
                    type: "GET",
                    url: "/Home/TaskUserDashboardTodoGrid",
                    success: function (data) {
                        $("#divHomeTasks").html(data);
                        todo.list.initialize();
                    }
                });
                $.ajax({
                    type: "GET",
                    url: "/Home/TaskUserDashboardCompletedGrid",
                    success: function (data) {
                        $("#divHomeTasksCompleted").html(data);
                    }
                });
            }
        },

        detail: {
            initialize: function (insuranceID) { }
        }
    };
    todo.preInit();
    window.TaskHomeDashboard = api;
})();
