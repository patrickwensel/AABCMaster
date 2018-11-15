

var Catalyst = {


    Timesheet: {

        IsValid: false,
        ErrorText: null,
        CallbackData: null,

        UploadFilesComplete: function (s, e) {
            window.setTimeout(function () {

                if (Catalyst.Timesheet.IsValid) {
                    // not actual delete, just poorly named dialog
                    App.Dialogs.ConfirmDelete({
                        message: "Import this data into the timesheet system?",
                        confirmed: function () {
                            Catalyst.Timesheet.ProcessFile(s, e);
                        },
                        cancelled: function () {
                            //d o nothing
                        }
                    });
                } else {
                    App.Dialogs.Error("We're sorry, but we couldn't verify this file is in the correct format.  Please make sure you have a valid file and try again. (error: " + Catalyst.Import.CallbackData + ")");
                }

            }, 150);
        },

        ProcessFile: function (s, e) {
            $.ajax({
                type: 'POST',
                url: '/ExternalData/CatalystTimesheetProcess',
                data: { fileName: e.callbackData },
                success: function (res) {
                    alert("Timesheet data imported successfully.");                    
                },
                error: function (res) {
                    App.Dialogs.Error("We're sorry, but we couldn't verify this file is in the correct format.  Please make sure you have a valid file and try again. (error: " + Catalyst.Import.CallbackData + ")");
                }
            });
        },

        UploadFileComplete: function (s, e) {
            Catalyst.Timesheet.IsValid = e.isValid;
            Catalyst.Timesheet.ErrorText = e.errorText;
            Catalyst.Timesheet.CallbackData = e.callbackData;
        },

        DropZoneEnter: function () {
            $("#dragZone").css('background-color', '#e5e5e5');
        },

        DropZoneLeave: function () {
            $("#dragZone").css('background-color', '#ffffff');
        }

    },

    // Import handles all the HasData stuff, new entries for timesheets are under the Timesheet area
    Import: {
        
        UploadFilesComplete: function (s, e) {

            // wasn't needed during testing but just to be sure as this and the uploadfilecomplete function
            // are called async
            window.setTimeout(function () {

                if (Catalyst.Import.IsValid) {
                    // (this isn't an actual delete, the dialog is just poorly named
                    App.Dialogs.ConfirmDelete({
                        message: "File appears to be valid.  Import and update hours now?",
                        confirmed: function () {

                            $.ajax({
                                type: 'POST',
                                url: '/ExternalData/CatalystStudentAttendanceProcess',
                                data: { fileName: e.callbackData },
                                success: function (res) {
                                    
                                    App.Popup.Show({
                                        url: "/ExternalData/CatalystStudentAttendanceResults",
                                        data: { results: res },
                                        options: {
                                            width: 700,
                                        },
                                        type: 'POST',
                                        error: function() {
                                            App.Dialogs.Error();
                                        },
                                        finished: function () {
                                            // ?
                                        }
                                    });

                                }
                            });

                        },
                        cancelled: function () {
                            // nothing
                        }
                    });
                } else {
                    App.Dialogs.Error("We're sorry, but we couldn't verify this file is in the correct format.  Please make sure you have a valid file and try again. (error: " + Catalyst.Import.CallbackData + ")");
                }

            }, 150);

        },

        UploadFileComplete: function (s, e) {
            Catalyst.Import.IsValid = e.isValid;
            Catalyst.Import.ErrorText = e.errorText;
            Catalyst.Import.CallbackData = e.callbackData;
        },

        IsValid: false,
        ErrorText: null,
        CallbackData: null,

        DropZoneEnter: function() {
            $("#dragZone").css('background-color', '#e5e5e5');
        },

        DropZoneLeave: function () {
            $("#dragZone").css('background-color', '#ffffff');
        }





    }   // End Catalyst.Import



}   // end Catalyst