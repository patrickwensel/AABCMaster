// Scripts/Pages/CaseNotes.js

var CaseNotes = {
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
    },
    FollowupComplete: function () {
        App.Popup.Show({
            url: "/Case/AddHoursPopup",
            data: {
                caseID: 1
            },
            options: {
                width: 400,
                height: 200,
                title: 'Add Hours'
            },
            finished: function () {
                TimeBill.Scrub.Grid.Object.Refresh();
            },
            error: function (response) {
                App.Dialogs.Error(response);
            }
        });
    }
};
