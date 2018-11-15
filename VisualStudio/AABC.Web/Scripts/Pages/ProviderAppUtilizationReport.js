(function () {
    var model = function () {
        var self = this;
        self.chart = null;

        self.startDate = ko.observable(moment().add(-3, 'months').format('YYYY-MM-DD'))
            .extend({ required: true, date: true });

        self.endDate = ko.observable(moment().format('YYYY-MM-DD'))
            .extend({ required: true, date: true });

        self.startDate.subscribe(requestData);
        self.endDate.subscribe(requestData);

        function requestData() {
            $.ajax({
                url: '/Reporting/ProviderAppUtilizationReportData',
                data: {
                    startDate: self.startDate(),
                    endDate: self.endDate()
                },
                success: function (data) {
                    var cases = $.map(data.cases, function (item) {
                        return new Array([Date.parse(item.date), item.value]);
                    });

                    var providers = $.map(data.providers, function (item) {
                        return new Array([Date.parse(item.date), item.value]);
                    });

                    self.chart.series[0].update({ data: cases }, true);
                    self.chart.series[1].update({ data: providers }, true);
                },
                cache: false
            });
        }

        self.chart = Highcharts.chart('chart',
            {
                title: { text: 'Provider App Utilization' },
                chart: {
                    width: 800,
                    height: 400,
                    events: { load: requestData }
                },
                credits: { enabled: false },
                legend: { layout: 'vertical' },
                tooltip: { xDateFormat: '%Y-%m-%d' },

                yAxis: { title: { text: 'Percent' } },
                xAxis: {
                    type: 'datetime',
                    tickInterval: 7 * 24 * 3600 * 1000, // one week
                    title: { text: 'Date' }
                },
                series: [
                    {
                        name: 'Percent of hours entered via the Provider app',
                        type: 'spline',
                        data: []
                    },
                    {
                        name: 'Percent of providers enabled for app use',
                        step: 'right',
                        data: []
                    }
                ]
            });

        return self;
    };

    $(function () {
        ko.applyBindings(model(), $("#report-selector-container")[0]);
    });
})();