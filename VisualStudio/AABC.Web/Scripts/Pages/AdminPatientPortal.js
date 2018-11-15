
(function () {

    var api = {
        existing: {
            popupMenuClick: function (s, e, m) { code.existing.popupMenuClick(s, e, m); },
            addRemovePatients: {
                initializePopup: function (loginID) {
                    code.existing.addRemovePatients.initialize(loginID);
                }
            },
            addLogin: {
                initializePopup: function () {
                    code.existing.addLogin.initializePopup();
                }
            }
        },
        initialize : function () { code.initialize();}
    }

    var code = {

        initialize: function () {
            
            $('#expander-existing-logins-header').click(function () {
                code.existing.toggle();
            });
            
            $('#expander-patient-list-header').click(function () {
                code.patients.toggle();
            });

            code.existing.toggle();


        },

        existing: {

            visible: false,

            toggle: function() {
                if (code.existing.visible == false) {
                    code.existing.load(function () {
                        $('#existing-logins-content').show(250);
                        code.existing.visible = true;
                    });
                    
                } else {
                    $('#existing-logins-content').hide(250);
                    code.existing.visible = false;
                }
            },  // end code.patients.toggle

            popupMenuClick: function(s, e, m) {

                var id = s.name.replace('pmActions', '');
                var action = e.item.name;
                console.log(m);
                switch (action) {

                    case 'ToggleActivation':
                        code.existing.toggleActivation(id);
                        break;
                    case 'ResetPassword':
                        code.existing.resetPassword(id);
                        break;
                    case 'AddRemovePatients':
                        code.existing.addRemovePatients.open(id);
                        break;
                    case 'Edit':
                        code.existing.addLogin.show(m);
                        break;


                }                

            },  // end existing.popupMenuClick
            
            addRemovePatients: {
                
                removePatient: function (patientID) {

                    App.LoadingPanel.AutoPanel.SupressSingle();

                    $.ajax({
                        type: 'POST',
                        url: '/PatientPortal/Login/Patients/Remove',
                        data: {
                            loginID: code.existing.addRemovePatients.currentID,
                            patientID: patientID
                        },
                        success: function (res) {
                            if (res == "ok") {
                                code.existing.addRemovePatients.refreshCurrentList();
                            }
                            else {
                                App.Dialogs.Error(res);
                            }
                        }
                    });

                },  // end code.existing.addRemovePatients.removePatient

                addPatient: function (patientID) {

                    App.LoadingPanel.AutoPanel.SupressSingle();

                    $.ajax({
                        type: 'POST',
                        url: '/PatientPortal/Login/Patients/Add',
                        data: {
                            loginID: code.existing.addRemovePatients.currentID,
                            patientID: patientID
                        },
                        success: function (res) {
                            code.existing.addRemovePatients.refreshCurrentList();
                        }
                    });

                },  // end code.existing.addRemovePatients.addPatient

                currentList: {
                    addActionClickHandler: function () {

                        $('[data-dym-link="remove-patient"]').click(function (event) {
                            event.preventDefault();
                            var id = $(this).attr('id').replace('remove-patient-', '');
                            code.existing.addRemovePatients.removePatient(id);
                            return false;
                        });
                    },

                    addBeginCallbackHandler: function () {

                        App.DevEx.GetControl("CurrentPatientsGrid").BeginCallback.AddHandler(function (s, e) {
                            e.customArgs['loginID'] = code.existing.addRemovePatients.currentID;
                        });
                    },

                    addEndCallbackHandler: function () {

                        App.DevEx.GetControl("CurrentPatientsGrid").EndCallback.AddHandler(function (s, e) {
                            code.existing.addRemovePatients.currentList.addActionClickHandler();
                        });
                        
                    }


                },  // end code.existing.addRemovePatients.currentList

                patientsList: {

                    addActionClickHandler: function () {

                        $('[data-dym-link="add-patient"]').click(function (event) {
                            event.preventDefault();
                            var id = $(this).attr('id').replace('add-patient-', '');
                            code.existing.addRemovePatients.addPatient(id);
                            return false;
                        });
                    },  // end code.existing.addRemovePatients.addActionClickHandler

                    addEndCallbackHandler: function () {
                        App.DevEx.GetControl("PatientListGrid").EndCallback.AddHandler(function (s, e) {
                            code.existing.addRemovePatients.patientsList.addActionClickHandler();
                        });
                    }   // end code.existing.addRemovePatients.addBeginCallbackHandler

                },  // end code.existing.addRemovePatients.patientsList

                initialize: function(id) {

                    var arp = code.existing.addRemovePatients;

                    arp.currentID = id;

                    arp.currentList.addActionClickHandler();
                    arp.currentList.addBeginCallbackHandler();
                    arp.currentList.addEndCallbackHandler();

                    arp.patientsList.addActionClickHandler();
                    arp.patientsList.addEndCallbackHandler();

                },  // end code.existing.addRemovePatients.initialize

                currentID: null,

                refreshCurrentList: function(id) {
                    
                    App.LoadingPanel.AutoPanel.SupressSingle();
                    App.DevEx.GetControl("CurrentPatientsGrid").Refresh();

                },  // end code.existing.addRemovePatients.refreshCurrentList


                open: function (id) {

                    App.LoadingPanel.AutoPanel.SupressSingle();

                    App.Popup.Show({
                        data: {
                            loginID: id
                        },
                        type: 'GET',
                        url: '/PatientPortal/AddRemovePatientsPopup',
                        options: {
                            width: 800,
                            height: 500,
                            title: 'Add/Remove Patients',
                            allowDrag: true
                        },
                        finished: function (res) {
                            App.DevEx.GetControl("ExistingPortalLoginsGrid").Refresh();
                        },
                        error: function (res) {
                            App.Dialogs.Error(res);
                        }
                    });

                }   // end existing.addRemovePatients.open

            },  // end existing.addRemovePatients

            toggleActivation: function(id) {

                App.LoadingPanel.AutoPanel.SupressSingle();

                $.ajax({
                    type: 'GET',
                    url: '/PatientPortal/Activation/ToggleStatus',
                    data: { id: id },
                    success: function (res) {

                        res = JSON.parse(res);

                        console.log(res);

                        var msg = 'Set ' + res.email + ' to';
                        if (res.active == false) {
                            msg += ' Active?';
                        } else {
                            msg += ' Inactive?';
                        }

                        if (confirm(msg)) {
                            
                            $.ajax({
                                type: 'POST',
                                url: '/PatientPortal/Login/' + id + '/Activation/Toggle',
                                success: function (res) {
                                    App.DevEx.GetControl("ExistingPortalLoginsGrid").Refresh();
                                }
                            });

                        }

                    }
                });

            },  // end existing.toggleActivation

            resetPassword: function(id) {

                $.ajax({
                    type: 'POST',
                    url: '/PatientPortal/ResetPassword',
                    data: {
                        loginID: id
                    },
                    success: function (res) {
                        App.Dialogs.MessageBox("New password: " + res);
                    }
                });

            },  // end existing.resetPassword
            
            initialize: function() {

                code.existing.addLoginClickHandler();
                code.existing.addEndCallbackHandler();
                //code.existing.resize();
                
            },

            addLoginClickHandler: function() {
                App.DevEx.GetControl("addLoginViaExistingHeaderButton").Click.AddHandler(function (s, e) {
                    code.existing.addLogin.show();
                });
            },

            addEndCallbackHandler: function() {
                App.DevEx.GetControl("ExistingPortalLoginsGrid").EndCallback.AddHandler(function (s, e) {
                    code.existing.addLoginClickHandler();
                });
            },

            addLogin: {
                
                save: function() {

                    var valid = true;

                    var email = $("#hdPatientLoginEmail").val();
                    try {
                        email =  App.DevEx.GetControl("AddLoginEmail").GetValue();
                    } catch (ex) {

                    }
                    var firstName = App.DevEx.GetControl("AddLoginFirstName").GetValue();
                    var lastName = App.DevEx.GetControl("AddLoginLastName").GetValue();
                    var active = App.DevEx.GetControl("AddLoginActive").GetValue();

                    if (email == null || email == '') {
                        valid = false;
                    }

                    if (firstName == null || firstName == '') {
                        valid = false;
                    }

                    if (lastName == null || lastName == '') {
                        valid = false;
                    }

                    if (active == null) {
                        active = false;
                    }

                    if (!valid) {
                        App.Dialogs.MessageBox("Please ensure all fields are filled out");
                        return;
                    } else {

                        $.ajax({
                            type: 'POST',
                            url: '/PatientPortal/AddUpdateLoginPopup',
                            data: {
                                email: email,
                                firstName: firstName,
                                lastName: lastName,
                                active: active
                            },
                            success: function (res) {
                                if (res.length > 0) {
                                    if (res == 'err') {
                                        alert("Unable to add login.  Please check the email doesn't already exist");
                                    } else {
                                        App.Dialogs.MessageBox("The temporary password is: " + res);
                                        App.Popup.Hide();
                                    }
                                } else {
                                    App.Popup.Hide();
                                }

                            }
                        });

                    }

                },

                initializePopup: function() {
                    
                    App.DevEx.GetControl("addLoginSaveButton").Click.AddHandler(function (s, e) {
                        code.existing.addLogin.save();
                    });

                },

                show: function (email) {

                    App.LoadingPanel.AutoPanel.SupressSingle();

                    App.Popup.Show({
                        type: 'GET',
                        url: '/PatientPortal/AddUpdateLoginPopup?Email=' + email,
                        options: {
                            width: 350,
                            height: 200,
                            title: 'Add Login',
                            allowDrag: true
                        },
                        finished: function (res) {
                            App.DevEx.GetControl("ExistingPortalLoginsGrid").Refresh();
                        },
                        error: function (res) {
                            App.Dialogs.Error(res);
                        }
                    });
                }
            },

            load: function (callback) {

                App.LoadingPanel.AutoPanel.SupressSingle();

                $.ajax({
                    type: 'GET',
                    url: '/PatientPortal/Existing',
                    success: function (res) {
                        $('#existing-logins-content').empty().append(res);
                        code.existing.initialize();
                        callback();
                    }
                });

            },   // end code.existing.load
            resize: function () {
                App.Content.GridViews.InitializeForStretchHeight(App.DevEx.GetControl("ExistingPortalLoginsGrid"), -120);
            }
        },  //  end code.existing

        patients: {

            /*

                Deprecated, no longer used and pulled from UI

            */

            visible: false,

            toggle: function () {
                if (code.patients.visible == false) {
                    code.patients.load(function () {
                        $('#patient-list-content').show(250);
                        code.patients.visible = true;
                    });
                } else {
                    $('#patient-list-content').hide(250);
                    code.patients.visible = false;
                }
            },  // end code.patients.toggle

            load: function (callback) {

                $.ajax({
                    type: 'GET',
                    url: '/PatientPortal/Patients',
                    success: function (res) {
                        $('#patient-list-content').empty().append(res);
                        callback();
                    }
                });

            }   // end code.patients.load

        }   // end code.patients

    }

    var signIn = {
        initialize: function(){

        },
        refresh: function () {
            $.ajax({
                type: "GET",
                url: "/PatientPortal/SignInSummary?StartDate=" + moment(FromDate.GetDate()).format('YYYY-MM-DD') + "&EndDate=" + moment(ToDate.GetDate()).format('YYYY-MM-DD'),
                success: function (data) {
                    $("#signin-list-content").html(data);
                    signIn.initialize();
                }
            })
        },
    }

    if (window.Admin == null) window.Admin= {};
    if (window.Admin.PatientPortal == null) window.Admin.PatientPortal = {};
    if (window.Admin.PatientPortal.Users == null) window.Admin.PatientPortal.Users = api;
    if (window.Admin.PatientPortal.Signin == null) window.Admin.PatientPortal.Signin = signIn;
    api;

})();