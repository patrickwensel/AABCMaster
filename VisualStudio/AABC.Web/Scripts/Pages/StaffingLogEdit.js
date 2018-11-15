function initComponent() {
    $("#btnAddContactWithAide").on("click", function () {
        StaffingLogDetails.ProviderContactLog.Add(StaffingLogDetails.ProviderContactLog.providerTypes.AIDE);
    });
    $("#btnAddContactWithBCBA").on("click", function () {
        StaffingLogDetails.ProviderContactLog.Add(StaffingLogDetails.ProviderContactLog.providerTypes.BCBA);
    });
    $("#btnAddContactWithParent").on("click", function () {
        StaffingLogDetails.ParentContactLog.Add();
    });
    $("#btnAddNote").on("click", function () {
        var parentID = $(this).attr("parentID");
        App.Popup.Show({
            url: "/CaseNotes/NoteEdit?parentID=" + parentID + "&id=0&isModal=true",
            options: {
                width: 500,
                height: 400,
                title: "Note Edit"
            },
            finished: function () {
                App.Popup.Hide();
            },
            error: function () {
                App.Dialogs.Error();
            }
        });
    });
}