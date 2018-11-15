// HoursValidateMDH.js

(function () {
	
	var mhd = {

	    screen: {
	        period: moment().format('YYYY-MM-DD')
	    },
		initialize: function () {

		    App.DevEx.GetControl("SelectedDate").ValueChanged.AddHandler(function (s, e) {
		        mhd.refreshList();
		    });

		    mhd.initializeList();


		},	// end hours.initialize


		refreshList: function () {

			mhd.screen.period = App.DevEx.GetControl("SelectedDate").GetValue().toISOString();

			$.ajax({

				type: 'get',
				url: '/Hours/MaxPerDayPerPatientList',
				data: {
				    period: mhd.screen.period
				},
				success: function (res) {
				    $("#MaxPerDayPerPatientListContainer").empty().append(res);
					mhd.initializeList();
				},
				error: function () {
					App.Dialogs.Error();
				}
				
			});

		},	// end hours.runValidation
		loadPatientExcessDays : function(Id){
		    $.ajax({

		        type: 'get',
		        url: '/Hours/MaxPerDayPerPatientDays',
		        data: {
                    patientId: Id,
                    period: mhd.screen.period
		        },
		        success: function (res) {
		            $("#MaxPerDayPerPatientDaysContainer").empty().append(res);

		        },
		        error: function () {
		            App.Dialogs.Error();
		        }

		    });
		},

		initializeList: function () {
		    var grid = App.DevEx.GetControl("MaxPerDayPatientList");

		    $('[data-dym-cellClick="MaxPerDayPatientList"]').dblclick(function () {
		        var visibleIndex = $(this).attr('data-dym-visibleIndex');
		        var id = App.DevEx.GetControl("MaxPerDayPatientList").GetRowKey(visibleIndex);
		        mhd.loadPatientExcessDays(id);
		    });

		},	// end hours.reinitGrid




	}	// end hours
	
	if (window.Hours == null) window.Hours = {};
	if (window.Hours.Validate == null) window.Hours.Validate = {};
	if (window.Hours.Validate.MaxHoursPerDay == null) window.Hours.Validate.MaxHoursPerDay = mhd;
})();