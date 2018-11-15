

(function () {


    var init = {

        toastr: function () {

            console.log('initilaizing toaster options');

            toastr.options = {
                "closeButton": true,
                "debug": true,
                "progressBar": true,
                "preventDuplicates": false,
                "positionClass": "toast-top-right",
                "showDuration": "400",
                "hideDuration": "1000",
                "timeOut": "3000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }
        },  // end init.toastr


        appConfirm: function() {

            $('#app-confirm-ok-button').click(function (e) {
                e.preventDefault();
                $('#app-confirm-result').val('ok');
                $('#app-confirm').modal('hide');
                return false;
            });

            $('#app-confirm-cancel-button').click(function (e) {
                e.preventDefault();
                $('#app-confirm-result').val('cancel');
                $('#app-confirm').modal('hide');
                return false;
            });

        }   // end init.appConfirm


    }


    init.appConfirm();
    init.toastr();
    



    var app = {

        callback: function(cb) {
            if (app.isCallback(cb)) {
                cb();
            }
        },  // end app.callback

        isCallback: function(cbTest) {

            if (typeof (cbTest) == 'function') {
                return true;
            } else {
                return false;
            }

        },  // end app.isCallback

        confirm: function (prompt, cbOk, cbCancel) {

            $('#app-confirm-result').val('cancel');
            $('#app-confirm-prompt').empty().append(prompt);

            $('#app-confirm').on('hidden.bs.modal', function () {
                $('#app-confirm').off('hidden.bs.modal');
                if ($('#app-confirm-result').val() == 'ok') {
                    app.callback(cbOk);
                } else {
                    app.callback(cbCancel);
                }
            });

            $('#app-confirm').modal();

        },   // end app.confirm

        alert: function (prompt, callback) {



        }   // end app.alert

    }

    window.app = app;

    ko.extenders.numeric = function (target, precision) {
        var result = ko.dependentObservable({
            read: function () {
                return numeral(target()).format('0,0.00');
            },
            write: function (n) {
                var w = numeral(n).value();
                target(w);
            }
        });

        result(target());
        return result;
    };

    ko.validation.rules['minFormatted'] = {
        validator: function (val, otherVal) {
            return numeral(val).value() >= otherVal;
        },
        message: 'The field has minimum of {0}'
    };

    ko.validation.rules['cardExpiry'] = {
        validator: function (year, month) {
            return moment({ year: year, month: month }).diff(moment()) > 0;
        },
        message: 'Please select a valid expiry date.'
    };

    ko.validation.registerExtenders();


})();