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
			},

			ScrubSelected: function(){
				hours.grid.scrubSelected();
			}
		},
		Editor: {
			InitializePopup: function () { hours.editor.init(); }
		}
	};


	var hours = {

		initialize: function () {

			console.log('hours edit initializing');

		},	// end hours.initialize



		editor: {

			show: function (id) {

				// id is the hours ID to show
				console.log('showing hours id ' + id);
				
				var cancelCallback = function () { App.DevEx.GetControl("pc-hours-entry").Hide(); };
				var successCallback = function () {
					App.DevEx.GetControl("pc-hours-entry").Hide();
					hours.grid.object.Refresh();
					//window.location.reload(true);
				};
				var deleteCallback = function () {
					if (confirm("Are you sure you want to delete this entry?")) {
						$.ajax({
							url: '/HoursEntry/Delete',
							type: 'POST',
							data: { hoursID: hoursID },
							success: function () {
								hours.grid.object.Refresh();
								//window.location.reload();
							}
						});
					}
				};

				$.ajax({
					url: '/HoursEntry/GetEntryVM',
					data: { hoursID: id }
				}).done(function (data) {
						var vm = new window.aabc.hoursEntry.HoursEntryVM(data, cancelCallback, successCallback, deleteCallback);
						var root = document.getElementById('hours-entry-container-pp');
						ko.cleanNode(root);
						ko.applyBindings(vm, root);
						App.DevEx.GetControl("pc-hours-entry").Show();
					});		            
				
			},   // end hours.editor.show

			init: function () {
				
				console.log('editor popup initializing');


			},   // end hours.edit.init


			submit: function () {


			}   // end hours.edit.submit


		},  // end hours.editor

		grid: {

			object: null,

			selfInit: function (grid) {

				console.log('grid says it is initialized');

				grid.object = grid;

				App.Content.GridViews.InitializeForStretchHeight(grid);

				grid.BeginCallback.AddHandler(function (s, e) {
					var period = App.DevEx.GetControl("SelectedDate").GetValue().toISOString();
					var viewMode = App.DevEx.GetControl("HoursViewMode").GetValue();
					e.customArgs["period"] = period;
					e.customArgs["viewMode"] = viewMode;
				});

				grid.EndCallback.AddHandler(function (s, e) {

					App.Content.GridViews.InitializeForStretchHeight(s);
					App.DevEx.GetControl("SelectedDate").ValueChanged.AddHandler(function (s, e) {
						grid.PerformCallback();
					});
					App.DevEx.GetControl("HoursViewMode").ValueChanged.AddHandler(function (s, e) {
						grid.PerformCallback();
					});
					$("[data-dym-cellClick='HoursEditGrid']").dblclick(function () { cellClick($(this)); });

				});

				App.DevEx.GetControl("SelectedDate").ValueChanged.AddHandler(function (s, e) {
					grid.PerformCallback();
				});

				App.DevEx.GetControl("HoursViewMode").ValueChanged.AddHandler(function (s, e) {
					grid.PerformCallback();
				});

				$("[data-dym-cellClick='HoursEditGrid']").dblclick(function() { cellClick($(this)); });
				
				function cellClick(cell) {
					window.setTimeout(function () {
						var key = App.DevEx.GridView.GetFocusedRowKey(grid);
						hours.editor.show(key);
					}, 50);
				}

			},

			selectionChanged: function (s, e) {
				s.GetSelectedFieldValues("ID", this.selectionChangedCallback);
			},

			selectionChangedCallback: function (values) {
				$("#hoursGridSelected").val(values.join());
			},

			scrubSelected: function () {

				var selectedList = $("#hoursGridSelected").val().split(',');

				if (confirm("Scrub the " + selectedList.length + " selected hours?")) {

					$.ajax({
						type: 'post',
						url: '/Hours/ScrubSelected',
						data: {
							selectedHours: selectedList
						},
						success: function (res) {
							HoursEditGrid.Refresh();
						},
						error: function (res) {
							App.Dialogs.Error(res);
						}
					});

				}
			}

		}  // end hours.grid
		

	};	// end hours
	
	window.HoursEdit = Interface;

})();