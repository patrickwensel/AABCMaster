// hoursvalidate.js

(function () {
	
	var Interface = {
		Initialize: function () { hours.initialize(); }
	}


	var hours = {

		initialize: function () {

			hours.initializeGrid();

		},	// end hours.initialize

		runValidation: function () {

			var period = App.DevEx.GetControl("SelectedDate").GetValue().toISOString();

			$.ajax({

				type: 'get',
				url: '/Hours/RunValidation',
				data: {
					period: period
				},
				success: function (res) {
					$("#HoursValidationResultsGridContainer").empty().append(res);
					hours.initializeGrid();
				},
				error: function () {
					App.Dialogs.Error();
				}
				
			});

		},	// end hours.runValidation

		initializeGrid: function () {
			
			window.setTimeout(function () {

				var grid = App.DevEx.GetControl("gvHoursValidationResults");

				App.Content.GridViews.InitializeForStretchHeight(grid, -8);

				grid.BeginCallback.AddHandler(function (s, e) {
					var period = App.DevEx.GetControl("SelectedDate").GetValue().toISOString();
					e.customArgs["period"] = period;
				});
				
				grid.EndCallback.AddHandler(function (s, e) {

				    App.Content.GridViews.InitializeForStretchHeight(s, -8);

				    App.DevEx.GetControl("SelectedDate").ValueChanged.AddHandler(function (s, e) {
				        hours.runValidation();
				    });

				    App.DevEx.GetControl("btnProcessValidation").Click.AddHandler(function (s, e) {
				        hours.runValidation();
				    })
				});

				App.DevEx.GetControl("SelectedDate").ValueChanged.AddHandler(function (s, e) {
					hours.runValidation();
				});

				App.DevEx.GetControl("btnProcessValidation").Click.AddHandler(function (s, e) {
					hours.runValidation();
				})


			}, 250);

		}	// end hours.reinitGrid





	}	// end hours
	
	window.HoursValidate = Interface;

})();