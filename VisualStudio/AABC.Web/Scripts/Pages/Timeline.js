var TimelineVM = function (caseId, opt) {
    var options = ko.utils.extend({
        fillMissingMonths: true
    }, opt || {});
    var self = {},
        toDate = function (i) {
            return i.format("YYYYMMDD");
        };
    self.showNotes = ko.observable(false);
    self.items = ko.observableArray([]);
    self.years = ko.pureComputed(function () {
        return _.chain(self.items()).keys().sortBy(function (y) { return -y; }).value();
    });

    self.showNotes.subscribe(function () {
        self.load();
    });

    self.toggleNotes = function () {
        var value = !self.showNotes();
        self.showNotes(value);
    };

    self.getDates = function (year) {
        var items = _.chain(self.items()[year])
            .map(function (i) {
                return i.date;
            })
            .uniq(false, toDate)
            .groupBy(function (i) {
                return i.format("YYYYMM");
            })
            .map(function (dates, key) {
                return { month: moment(key + "01"), dates: dates };
            })
            .sortBy(function (i) {
                return i.month;
            })
            .reverse()
            .value();
        if (options.fillMissingMonths) {
            for (var i = 1; i < items.length;) {
                var prev = items[i - 1].month;
                var current = items[i].month;
                var diff = prev.diff(current, 'months') - 1;
                var newItems = _.times(diff, function (j) {
                    return { month: moment(prev).subtract(j + 1, 'month'), dates: [] };
                });
                Array.prototype.splice.apply(items, [i, 0].concat(newItems));
                i += diff + 1;
            }
        }
        return items;
    };

    self.getEntries = function (date) {
        var d = toDate(date);
        return _.chain(self.items()[date.year()])
            .filter(function (i) {
                return toDate(i.date) === d;
            })
            .sortBy(function (i) {
                return i.date;
            })
            .reverse()
            .value();
    };

    self.load = function () {
        return $.getJSON("/Staffing/GetTimelineEntries", { caseId: caseId, includeNotes: self.showNotes() }).done(function (items) {
            if (items) {
                var g = _.chain(items)
                    .map(function (i) {
                        i.date = moment(i.date);
                        return i;
                    })
                    .groupBy(function (i) {
                        return i.date.year();
                    })
                    .value();
                self.items(g);
            }
        });
    };

    return self;
};