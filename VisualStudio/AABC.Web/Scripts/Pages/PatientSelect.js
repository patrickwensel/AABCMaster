// reportSelector

(function () {

    var select = {
        patientId: 0,
        patientName: '',
        initialize : function(){
            var grid = App.DevEx.GetControl("SelectPatientGrid");


            $("[data-dym-cellClick='SelectPatientGrid']").dblclick(function () {

                var cell = $(this);
                var index = cell.attr("data-dym-visibleIndex");
                select.patientId = grid.GetRowKey(index);
                select.patientName = cell.attr("data-dym-patientName");

                App.Popup.Hide();
            });
        }
    }

    if (window.Patient == null) window.Patient = {};
    if (window.Patient.Select == null) window.Patient.Select = select;

})();