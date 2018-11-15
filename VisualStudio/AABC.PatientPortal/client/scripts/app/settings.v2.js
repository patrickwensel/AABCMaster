

(function() {

    var api = {

        DisplayName: function (userID, containerID) { return new users.DisplayName(userID, containerID); },
        ChangePassword: function (userID, containerID) { return new users.ChangePassword(userID, containerID); },
        SignaturePad: function(canvas, dataUrl) { return new users.SignaturePad(canvas, dataUrl); }
    }


    var users = {
        
        SignaturePad: function(canvas, dataUrl) {

            //  window.setTimeout(initSigPad(), 1500);

            var sigData = dataUrl;

            initSigPadTrim();

            initSigPad();

            function initSigPadTrim() {

                SignaturePad.prototype.removeBlanks = function () {
                    var imgWidth = this._ctx.canvas.width;
                    var imgHeight = this._ctx.canvas.height;
                    var imageData = this._ctx.getImageData(0, 0, imgWidth, imgHeight),
                    data = imageData.data,
                    getAlpha = function (x, y) {
                        return data[(imgWidth * y + x) * 4 + 3]
                    },
                    scanY = function (fromTop) {
                        var offset = fromTop ? 1 : -1;

                        // loop through each row
                        for (var y = fromTop ? 0 : imgHeight - 1; fromTop ? (y < imgHeight) : (y > -1) ; y += offset) {

                            // loop through each column
                            for (var x = 0; x < imgWidth; x++) {
                                if (getAlpha(x, y)) {
                                    return y;
                                }
                            }
                        }
                        return null; // all image is white
                    },
                    scanX = function (fromLeft) {
                        var offset = fromLeft ? 1 : -1;

                        // loop through each column
                        for (var x = fromLeft ? 0 : imgWidth - 1; fromLeft ? (x < imgWidth) : (x > -1) ; x += offset) {

                            // loop through each row
                            for (var y = 0; y < imgHeight; y++) {
                                if (getAlpha(x, y)) {
                                    return x;
                                }
                            }
                        }
                        return null; // all image is white
                    };

                    var cropTop = 0,
                    cropBottom = imgHeight,
                    cropLeft = 0,
                    cropRight = scanX(false);

                    var relevantData = this._ctx.getImageData(cropLeft, cropTop, cropRight - cropLeft, cropBottom - cropTop);
                    this._canvas.width = cropRight - cropLeft;
                    this._canvas.height = cropBottom - cropTop;
                    this._ctx.clearRect(0, 0, cropRight - cropLeft, cropBottom - cropTop);
                    this._ctx.putImageData(relevantData, 0, 0);
                };

            }

            function initSigPad() {

                var sigPad = new SignaturePad(canvas);

                sigData = dataUrl;

                $('button[name="saveSignature"]').click(function (event) {
                    event.preventDefault();
                                        
                    var data = null;

                    if (sigPad.isEmpty()) {
                        toastr.error("Please make a signature", "Error");
                        return;
                    }

                    if (!sigPad.isEmpty()) {
                        sigPad.removeBlanks();
                        data = sigPad.toDataURL();
                        sigData = data;
                        resizeCanvas();
                    }

                    $.ajax({
                        type: 'POST',
                        url: '/Settings/Signature',
                        data: { data },
                        success: function (res) {
                            if (res == 'ok') {
                                toastr.success('Signature saved', 'Success');
                                sigData = data;
                            }
                        }
                    });

                    return false;
                });

                $('button[name="clearSignature"]').click(function (event) {
                    event.preventDefault();

                    sigPad.clear();

                    return false;
                });

                window.addEventListener('resize', resizeCanvas);
                resizeCanvas();





                function resizeCanvas() {
                    var ratio = Math.max(window.devicePixelRatio || 1, 1);
                    canvas.width = canvas.offsetWidth * ratio;
                    canvas.height = canvas.offsetHeight * ratio;
                    canvas.getContext("2d").scale(ratio, ratio);
                    sigPad.clear();   // otherwise isEmpty() might return incorrect value
                    if (sigData) {

                        var sigImg = new Image;
                        sigImg.src = sigData;
                        var ctx = canvas.getContext("2d");

                        var drawHeight = sigImg.height;
                        var drawWidth = sigImg.width;

                        if (sigImg.height < canvas.height) {
                            drawHeight = canvas.height;
                            drawWidth = sigImg.width * (drawHeight / sigImg.height);
                        }

                        if (sigImg.height > canvas.height) {
                            drawHeight = canvas.height;
                            drawWidth = sigImg.width * (drawHeight / sigImg.height);
                        }

                        if (drawWidth > canvas.width) {
                            drawWidth = canvas.width;
                            drawHeight = sigImg.height * (drawWidth / sigImg.width);
                        }

                        ctx.drawImage(sigImg, 0, 0, drawWidth / ratio, drawHeight / ratio);
                        
                    }
                }
            }

            

        },


        DisplayName: function (userID, containerID) {

            var container = $('#' + containerID);

            initialize();


            var events = {
                updated: {
                    handlers: [],
                    addHandler: function (handler) {
                        events.updated.handlers.push(handler);
                    },
                    fire: function (firstName, lastName) {
                        for (var i = 0; i < events.updated.handlers.length; i++) {
                            var h = events.updated.handlers[i];
                            h(firstName, lastName);
                        }
                    }
                }
            };

            function getModel() {
                return {
                    UserID: userID,
                    FirstName: container.find('input[name="firstName"]').val(),
                    LastName: container.find('input[name="lastName"]').val()
                }
            }

            function initialize() {

                container.find(':submit').click(function (event) {
                    event.preventDefault();

                    var model = getModel();

                    $.ajax({
                        type: 'POST',
                        url: '/Settings/DisplayName',
                        data: model,
                        success: function (res) {
                            if (res == "ok") {
                                toastr.success('Information Updated', 'Success');
                                events.updated.fire(model.FirstName, model.LastName);
                            } else {
                                toastr.error('We ran into an issue with that... (we notified our team so they can look into it)', 'Sorry!');
                            }
                        },
                        error: function (res) {
                            toastr.error('We ran into an issue with that... (we notified our team so they can look into it)', 'Sorry!');
                        }
                    });

                    return false;
                });
            }   // end initialize
            

            

            return {
                Updated: {
                    AddHandler: events.updated.addHandler
                }
            };


        },

        ChangePassword: function (userID, containerID) {

            var container = $('#' + containerID);

            var pw = {

                initialize: function () {

                    container.find(':submit').click(function (event) {
                        event.preventDefault();
                        pw.submit();
                        return false;
                    });
                },

                submit: function () {

                    var model = pw.model();

                    $.ajax({
                        type: 'POST',
                        url: '/Settings/ChangePassword',
                        data: model,
                        success: function (res) {
                            if (res == 'ok') {
                                pw.clearForm();
                                toastr.success('Password Updated Successfully', 'Success');
                            } else {
                                toastr.error(res, 'Failed!');
                            }
                        },
                        error: function () {
                            toastr.error('We ran into an issue with that... (we notified our team so they can look into it)', 'Sorry!');
                        }
                    })

                },

                clearForm: function() {
                    container.find('input[name="oldPassword"]').val('');
                    container.find('input[name="newPassword"]').val('');
                    container.find('input[name="confirmPassword"]').val('');
                },

                model: function () {
                    return {
                        UserID: userID,
                        OldPassword: container.find('input[name="oldPassword"]').val(),
                        NewPassword: container.find('input[name="newPassword"]').val(),
                        ConfirmPassword: container.find('input[name="confirmPassword"]').val()
                    }
                }

            }

            var api = {

            }

            pw.initialize();

            return api;

        }

    }   // end users

    window.aabc = window.aabc || {}
    window.aabc.settings = api;

})();