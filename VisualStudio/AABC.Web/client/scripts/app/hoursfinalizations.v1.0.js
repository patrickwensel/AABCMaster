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
	        }
	    }
	}


	var hours = {

		initialize: function () {
            
		},	// end hours.initialize

        
		grid: {

            object: null,

		    selfInit: function (grid) {
                
                grid.object = grid;

		        App.Content.GridViews.InitializeForStretchHeight(grid);

		        grid.BeginCallback.AddHandler(function (s, e) {
		            var period = App.DevEx.GetControl("SelectedDate").GetValue().toISOString();
		            e.customArgs["period"] = period;
		        });

		        grid.EndCallback.AddHandler(function (s, e) {

		            App.Content.GridViews.InitializeForStretchHeight(s);
		            App.DevEx.GetControl("SelectedDate").ValueChanged.AddHandler(function (s, e) {
		                grid.PerformCallback();
		            });

		        });

		        App.DevEx.GetControl("SelectedDate").ValueChanged.AddHandler(function (s, e) {
		            grid.PerformCallback();
		        });
                
		    }
            

		},  // end hours.grid
		

	}	// end hours
	
	window.HoursFinalizations = Interface;

})();