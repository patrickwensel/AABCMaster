ko.bindingHandlers.cleave = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var $element = $(element),
            container = valueAccessor(),
            observable = container.value;
        var c = new Cleave(element, container.options || {});
        $element.data("cleave", c);
        if (ko.isObservable(observable)) {
            $element.on('focusout change', function () {
                observable(container.options.getValue(c.getFormattedValue()));
            });
        }
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            // This will be called when the element is removed by Knockout or
            // if some other part of your code calls ko.removeNode(element)
            c.destroy();
        });
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var $element = $(element),
            c = $element.data("cleave"),
            container = valueAccessor(),
            observable = container.value;
        if (ko.isObservable(observable) && c) {
            var valueToWrite = observable();
            if (valueToWrite) {
                c.setRawValue(container.options.getDisplay(valueToWrite));
            } else {
                c.setRawValue(valueToWrite);
            }
        }
    }
};