

(function () {

    var api = {

        DataTable: function (elementSelector, config) { return new dym.DataTable(elementSelector, config); }   // end api.DataTable

    }   // end api

    var dym = {

        DataTable: function (elementSelector, cfg) {


            var table = {

                getRowIndexByIDField: function(id) {

                    var el = $(elementSelector + ' td[data-dym-id-cell="' + id + '"]');

                    if (el != null) {
                        var idx = el.closest('tr').attr('data-dym-row-index');
                        return idx;
                    } else {
                        return null;
                    }

                },

                getColumnIndexByName: function(colName) {
                    var el = $(elementSelector + ' th[data-dym-cell-name="' + colName + '"]');
                    if (el != null) {
                        return el.attr('data-dym-cell-index');
                    } else {
                        return null;
                    }
                },

                updateCell: function(dataID, rowIndex, colName, colIndex, value) {

                    if (dataID == null && rowIndex == null) {
                        return;
                    }
                    if (colIndex == null && colName == null) {
                        return;
                    }

                    if (rowIndex == null) {
                        rowIndex = table.getRowIndexByIDField(dataID);
                    }

                    if (colIndex == null) {
                        colIndex = table.getColumnIndexByName(colName);
                    }
                    
                    var tbl = $(elementSelector).dataTable();
                    var cell = tbl.api().cell(rowIndex, colIndex);
                    cell.data(value).draw();

                },  // end table.updateCell


                

                initialize: function () {
                    
                    var el = $(elementSelector);

                    var defaultConfig = new table.DefaultConfig();
                    var config = {};
                    $.extend(true, config, defaultConfig, cfg);

                    var t = el.DataTable(config);

                    t.on('dblclick', 'tr', function () {
                        var data = t.row(this).data();
                        table.events.rowDoubleClick.fire(data);
                    });

                    //t.on('select', function (e, dt, type, idxs) {
                    //    if (type === 'row') {
                    //        var data = t.row(idxs[0]).data()[0];
                    //        console.log('selected: ' + data);
                    //    }
                    //});
                    
                },   // end table.initialize


                events: {

                    rowDoubleClick: {
                        handlers: [],
                        addHandler: function (handler) {
                            table.events.rowDoubleClick.handlers.push(handler);
                        },
                        fire: function (data) {
                            var handlers = table.events.rowDoubleClick.handlers;
                            for (var i = 0; i < handlers.length; i++) {
                                var h = handlers[i];
                                h(data);
                            }
                        }
                    }

                },  // end table.events


                DefaultConfig: function () {
                    return {
                        pageLength: 25,
                        scrollCollapse: true,
                        scrollY: '300px',
                        select: {
                            style: 'single'
                        }
                    }
                }   // end table.DefaultConfig

            }   // end table


            table.initialize();

            return {
                DefaultConfig: table.DefaultConfig,
                rowDoubleClick: {
                    addHandler: function (handler) { table.events.rowDoubleClick.addHandler(handler); }
                },
                updateCell: function (dataID, rowIndex, columnName, columnIndex, value) {
                    // identify row by either dataID (assumes column named "ID") or rowIndex (rowIndex takes precedence)
                    // identify column by either columnName or columnIndex (columnIndex takes precedence)
                    table.updateCell(dataID, rowIndex, columnName, columnIndex, value);
                }
            }


        }   // end dym.DataTable

    }   // end dym

    window.dymeng = api;

})();