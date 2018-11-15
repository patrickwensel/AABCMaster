ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var $element = $(element);
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().datepickerOptions || {};
        if (!$element.data("datepicker")) {
            $element.datepicker(options);
        }

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "changeDate", function (event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                value(event.format());
            }
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            // This will be called when the element is removed by Knockout or
            // if some other part of your code calls ko.removeNode(element)
            $element.datepicker("destroy");
        });
    },
    update: function (element, valueAccessor) {
        var widget = $(element).data("datepicker");
        var date = ko.utils.unwrapObservable(valueAccessor());
        //when the view model is updated, update the widget
        if (widget) {
            widget.element.datepicker('update', date);
        }
    }
};

ko.bindingHandlers.datepickerRange = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var $element = $(element);
        var inputs = $element.find("input");
        var options = ko.utils.extend(allBindingsAccessor().datepickerOptions || {}, { inputs: inputs});
        $element.datepicker(options);
    }
};