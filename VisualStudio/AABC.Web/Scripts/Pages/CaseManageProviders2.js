var caseId, providerId;

function OnBeginCallback(s, e) {
    e.customArgs["caseID"] = $("#ID").val();
    e.customArgs["providerID"] = providerId;
}

function gvProvidersFocusedRowChanged(s, e) {
    s.GetRowValues(s.GetFocusedRowIndex(), "ProviderID", function (value) {
        if (providerId !== value) {
            providerId = value;
            CaseProviderScheduler.Refresh();
        }
    });

}

function openPopUpForNonRecurringAppointmentCreation(s, e) {
    $("input[name='AppointmentType']").val(1);
    $("#appointmentTypeSelector").hide();
    $("#appointmentEditor").show();
}

function openPopUpForRecurringAppointmentCreation(s, e) {
    $("input[name='AppointmentType']").val(0);
    $("#appointmentTypeSelector").hide();
    $("#appointmentEditor").show();
    $(".dxpc-headerText").text("New Recurring Appointment");
}

function openCreatePopUp(date) {
    // make sure we're allowed to edit
    App.LoadingPanel.AutoPanel.SupressSingle();
    App.Popup.Show({
        url: "/Scheduler/CreateAppointment",
        data: {
            caseId: $("#ID").val(),
            providerId: providerId,
            date: date.toISOString()
        },
        options: {
            width: 400,
            height: 300,
            title: "New Appointment",
            draggable: true
        },
        finished: function (res) {
            // popup initialize is handled in razor view for this popup
        },
        error: function () {
        }
    });
}

function CreateAppointment(s, e) {
    var data = {
        caseId: $("#ID").val(),
        providerId: providerId,
        type: $("input[name='AppointmentType']").val(),
        date: date.GetValue(),
        startTime: startTime.GetValue(),
        endTime: endTime.GetValue(),
    };
    if (!isValidForCreate(data.fate, data.startTime, data.endTime)) {
        return;
    }
    $.ajax({
        url: "/Scheduler/CreateAppointment",
        type: "POST",
        data: {
            CaseId: data.caseId,
            ProviderId: data.providerId,
            Type: data.type,
            Date: data.date.toISOString(),
            StartTime: data.startTime.toISOString(),
            EndTime: data.endTime.toISOString(),
        }
    }).done(function () {
        CaseProviderScheduler.Refresh();
        window.setTimeout(function () {
            App.Popup.Hide('ok');
        }, 500);
    }).fail(function () {
        App.Dialogs.Error();
    });
}

function clientSchedulerOnDoubleClick(s, e) {
    var hitResult = s.CalcHitTest(e);
    if (ASPxClientUtils.IsExists(hitResult.appointmentDiv)) {
        var appointmentId = hitResult.appointmentDiv.appointmentId;
        var clickedDate = hitResult.appointmentDiv.appointmentViewInfo.appointmentInterval.start;
        openEditPopUp(appointmentId, clickedDate);
    } else if (ASPxClientUtils.IsExists(hitResult.cell)) {
        var clickedDate = hitResult.cell.interval.start;
        openCreatePopUp(clickedDate);
    }
}

function openEditPopUp(appointmentId, date) {
    // make sure we're allowed to edit
    App.LoadingPanel.AutoPanel.SupressSingle();
    App.Popup.Show({
        url: "/Scheduler/EditAppointment",
        data: {
            appointmentID: appointmentId,
            occurrenceDate: date.toISOString()
        },
        options: {
            width: 400,
            height: 300,
            title: "Edit Appointment",
            draggable: true
        },
        finished: function (res) {
            // popup initialize is handled in razor view for this popup
        },
        error: function () {
        }
    });
}

function initPopUp() {
    var isRecurring = $("input[name='IsRecurring']").val();
    if (isRecurring == "True") {
        $(".recurring").show();
        $(".non-recurring").hide();
        $("#appointmentTypeSelector").show();
        $("#appointmentEditor").hide();
    } else {
        $(".recurring").hide();
        $(".non-recurring").show();
        $("#appointmentTypeSelector").hide();
        $("#appointmentEditor").show();
    }
}
function changeModeToNonRecurringAppointment() {
    var mode = 1;
    $("input[name='Mode']").val(mode);
    $(".recurring").hide();
    $(".non-recurring").show();
    $("#appointmentTypeSelector").hide();
    $("#appointmentEditor").show();
}

function changeModeToRecurringAppointment() {
    var mode = 0;
    $("input[name='Mode']").val(mode);
    $(".recurring").show();
    $(".non-recurring").hide();
    $("#appointmentTypeSelector").hide();
    $("#appointmentEditor").show();
}



function isValidForCreate(date, startTime, endTime) {
    var ret = true;
    var t = App.Settings.DataPage.ValidationMessageTransitionTime;
    if (date == null) {
        $("#errHoursEditByDate_DateMustBePast").show(t);
    } else {
        $("#errHoursEditByDate_DateMustBePast").hide(t);
        ret = isValidForEdit(startTime, endTime);
    }
    return ret;
}

function isValidForEdit(startTime, endTime) {
    var ret = true;
    var t = App.Settings.DataPage.ValidationMessageTransitionTime;
    if (startTime == null || endTime == null || startTime == endTime || startTime > endTime) {
        ret = false;
        $("#errHoursEditByDate_TimeGeneral").show(t);
    } else {
        $("#errHoursEditByDate_TimeGeneral").hide(t);
    }
    return ret;
}


function SaveAppointment(s, e) {
    var isRecurring = $("input[name='IsRecurring']").val(),
        mode = $("input[name='Mode']").val()
    if (isRecurring == "True") {
        if (mode == "0") {
            // Recurring, Modify series
            SaveRecurringAppointment(s, e);
        } else {
            // Recurring, create occurrence from specific instance of recurring
            SaveRecurringAppointmentAsOccurrence(s, e);
        }
    } else {
        // Occurrence
        SaveNonRecurringAppointment(s, e);
    }
}


function SaveRecurringAppointment(s, e) {
    var data = {
            appointmentId: $("input[name='AppointmentId']").val(),
            startTime: startTime.GetValue(),
            endTime: endTime.GetValue(),
        },
        url = "/Scheduler/EditAppointment";
    if (!isValidForEdit(data.startTime, data.endTime)) {
        return;
    }
    data.startTime = data.startTime.toISOString();
    data.endTime = data.endTime.toISOString();
    return EditAppointment(url, data);
}

function SaveNonRecurringAppointment(s, e) {
    var data = {
            appointmentId: $("input[name='AppointmentId']").val(),
            date: newDate.GetValue(),
            startTime: startTime.GetValue(),
            endTime: endTime.GetValue(),
        },
        url = "/Scheduler/EditAppointment";
    if (!isValidForCreate(data.date, data.startTime, data.endTime)) {
        return;
    }
    data.date = data.date.toISOString();
    data.startTime = data.startTime.toISOString();
    data.endTime = data.endTime.toISOString();
    return EditAppointment(url, data);
}

function SaveRecurringAppointmentAsOccurrence(s, e) {
    var data = {
            recurringAppointmentId: $("input[name='AppointmentId']").val(),
            occurrenceDate: $("input[name='OccurrenceDate']").val(),
            date: newDate.GetValue(),
            startTime: startTime.GetValue(),
            endTime: endTime.GetValue(),
        },
        url = "/Scheduler/CreateAppointmentFromRecurring";
    if (!isValidForCreate(data.date, data.startTime, data.endTime)) {
        return;
    }
    data.date = data.date.toISOString();
    data.startTime = data.startTime.toISOString();
    data.endTime = data.endTime.toISOString();
    return EditAppointment(url, data);
}

function EditAppointment(url, data) {
    $.ajax({
        url: url,
        type: "POST",
        data: data
    }).done(function () {
        CaseProviderScheduler.Refresh();
        window.setTimeout(function () {
            App.Popup.Hide('ok');
        }, 500);
    }).fail(function () {
        App.Dialogs.Error();
    });
}

function CancelAppointment(s, e) {
    if (confirm("Are you sure you want to cancel this appointment?")) {
        $.ajax({
            url: "/Scheduler/CancelAppointment",
            type: "POST",
            data: {
                appointmentId: $("input[name='AppointmentId']").val(),
                date: newDate.GetValue().toISOString()
            }
        }).done(function () {
            CaseProviderScheduler.Refresh();
            window.setTimeout(function () {
                App.Popup.Hide('ok');
            }, 500);
        }).fail(function () {
            App.Dialogs.Error();
        });
    }
}

function DeleteAppointment(s, e) {
    var appointmentId = $("input[name='AppointmentId']").val();
    if (confirm("Are you sure you want to delete this appointment?")) {
        $.ajax({
            url: "/Scheduler/DeleteAppointment",
            type: "POST",
            data: {
                appointmentId: appointmentId
            }
        }).done(function () {
            CaseProviderScheduler.Refresh();
            window.setTimeout(function () {
                App.Popup.Hide('ok');
            }, 500);
        }).fail(function () {
            App.Dialogs.Error();
        });
    }
}