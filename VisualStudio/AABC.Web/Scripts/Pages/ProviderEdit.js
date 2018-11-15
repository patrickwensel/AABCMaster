

var gFormID = "form-default";

var ctlFirstName = null;
var ctlLastName = null;
var ctlEmail = null;

function initComponent() {
    ActivePage.Initialize();
    Insurances.Init();
}

function resumeDownload(id) {
    var win = window.open("/Providers/GetResume?id=" + id, "_blank");
    win.focus();
}


var Zips = {


    Lookup: function () {

        // open the lookup popup

        App.Popup.Show({
            url: '/Selectors/ZipLookup',
            options: {
                width: 500,
                height: 400,
                title: "Zip Lookup"
            },
            finished: function (response) {

                if (response.substring(0, 3) == "ok:") {
                    Zips.AddZips(response.substring(3));
                }

            },
            error: function (response) {
                App.Dialogs.Error();
            }
        });
    },

    GetZipsByCounty: function(s, e) {
        // called from zip lookup tool to retrieve the zip values and return via delimited list
        var state = App.DevEx.GetControl("zluState").GetValue();
        var county = App.DevEx.GetControl("zluCounty").GetValue();

        App.DevEx.GetControl("zluCity").SetValue(null);

        $.ajax({
            type: "GET",
            url: "/Selectors/ZipsByStateAndCounty",
            data: {
                state: state,
                county: county
            },
            success: function (res) {
                App.DevEx.GetControl("zluZips").SetValue(res);
            },
            error: function () {
                App.dialogs.Error();
            }
        });
    },

    GetZipsByCity: function(s, e) {
        // called from zip lookup tool to retrieve the zip values and return via delimited list
        var state = App.DevEx.GetControl("zluState").GetValue();
        var city = App.DevEx.GetControl("zluCity").GetValue();

        App.DevEx.GetControl("zluCounty").SetValue(null);

        $.ajax({
            type: "GET",
            url: "/Selectors/ZipsByStateAndCity",
            data: {
                state: state,
                city: city
            },
            success: function(res) {
                App.DevEx.GetControl("zluZips").SetValue(res);
            },
            error: function () {
                App.dialogs.Error();
            }
        });
    },

    Select: function() {

        var zips = App.DevEx.GetControl("zluZips").GetValue();

        App.Popup.Hide('ok:' + zips);

    },

    AddZips: function (zipsToAdd) {
        
        var zipsControl = App.DevEx.GetControl("ServiceZips");
        
        var currentZips = zipsControl.GetValue();
        
        if (currentZips == null) {

            zipsControl.SetValue(zipsToAdd);

        } else {

            var currentArray = currentZips.split(",");
            var newArray = zipsToAdd.split(",");

            for (i = 0; i < currentArray.length; i++) {
                currentArray[i] = currentArray[i].replace(" ", "");
            }

            for (i = 0; i < newArray.length; i++) {
                newArray[i] = newArray[i].replace(" ", "");
            }

            // concat and remove dupes
            var all = newArray.concat(currentArray);
            var filtered = all.filter(function(item, pos) { return all.indexOf(item) == pos} );

            zipsControl.SetValue(filtered.join(", "));
            
        }

    }



}


var Insurances = {
    current: null,
    Init: function () {

        var grid = App.DevEx.GetControl('ProviderInsuranceCredentialGrid');
        var events = {

            beginCallback: function (s, e) {
                e.customArgs["caseID"] = insurance.config.caseID;
                e.customArgs["insuranceID"] = insurance.config.insuranceID;
            },
            endCallback: function (s, e) {
                insurance.tasks.initializeButtons();
            }
        }

        grid.BeginCallback.AddHandler(function (s, e) { events.beginCallback(s, e); });
        grid.EndCallback.AddHandler(function (s, e) { events.endCallback(s, e); });


        $("[data-dym-cellClick='ProviderInsuranceCredentialGrid']").dblclick(function () {
            var cell = $(this);
            var index = cell.attr("data-dym-visibleIndex");
            var field = cell.attr("data-dym-fieldName");
            var id = grid.GetRowKey(index);
            Insurances.Edit(0, id);
        });

    },  // end insuranceEdit.tasks.grid.initialize
    Edit: function (ProviderId, InsuranceCredentialId) {

        // open the lookup popup

        App.Popup.Show({
            url: '/Providers/InsuranceCredentialForm?ProviderId=' + ProviderId + '&InsuranceCredentialId=' + InsuranceCredentialId,
            options: {
                width: 300,
                height: 50,
                title: "Insurance Credentials"
            },
            finished: function (response) {

                //refresh grid here.

            },
            error: function (response) {
                App.Dialogs.Error();
            }
        });
    },
    Select: function () {

        $.ajax({
            type: "POST",
            url: "/Providers/InsuranceCredentialForm",
            data: $("#ProviderInsuranceCredentialForm").serialize(),
            success: function (data) {

                $("#tdInsuranceGrid").html(data);
                Insurances.Init();
            }
        })

        App.Popup.Hide();

    },
    Remove: function () {
        var grid = App.DevEx.GetControl('ProviderInsuranceCredentialGrid');
        var insuranceCredentialId = App.DevEx.GridView.GetFocusedRowKey(grid);
        $.ajax({
            type: "DELETE",
            url: "/Providers/InsuranceCredentialDelete?ProviderId=" + $("#ID").val() + "&InsuranceCredentialId=" + insuranceCredentialId,
            success: function (data) {
                $("#tdInsuranceGrid").html(data);
                Insurances.Init();
            }
        })

    }

}

// ActivePage
var ActivePage = {



    Form: {


        Validate: function () {

            /// <summary>Validates all controls and returns true or false accordingly</summary>
            
            ret = true;

            if (!ActivePage.Controls.FirstName.Validate()) { ret = false; }
            if (!ActivePage.Controls.LastName.Validate()) { ret = false; }
            if (!ActivePage.Controls.Email.Validate()) { ret = false; }

            return ret;

        },


        SubmitHandler: function (event) {

            /// <summary>Handles for submission, cancels or proceeds depending on validation</summary>

            event = event || window.event || event.srcElement;

            if (ActivePage.Form.Validate()) {
                return true;
            } else {
                event.preventDefault();
                App.Dialogs.ValidationFailure("Unable to save.  Please validate all fields.");
                return false;
            }

        }


    },



    ClientState: {


        Initialize: function () {

            ActivePage.Form.Validate();

        }//,

    },  // END ClientState



    Initialize() {


        $("#" + gFormID).on('submit', function (event) {
            return ActivePage.Form.SubmitHandler(event);
        });

        App.DataPage.InitSectionExpanders();
        ActivePage.Controls.Initialize();
        ActivePage.ClientState.Initialize();

        $('#provider-payroll-id-gen').click(function (e) {
            e.preventDefault();
            if (confirm("Generate a new Payroll ID for this provider?")) {
                App.LoadingPanel.AutoPanel.SupressAll();
                $.ajax({
                    type: 'GET',
                    url: '/Providers/NewPayrollID',
                    success: function (newID) {
                        App.DevEx.GetControl("PayrollID").SetText(newID);
                    }
                });
            }
            return false;
        });


    },  // END Initialize



    Controls: {


        Initialize: function () {

            ctlFirstName = App.DevEx.GetControl("FirstName");
            ctlLastName = App.DevEx.GetControl("LastName");
            ctlEmail = App.DevEx.GetControl("Email");

            ctlFirstName.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.FirstName.AfterUpdate(); });
            ctlLastName.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.LastName.AfterUpdate(); });
            ctlEmail.ValueChanged.AddHandler(function (s, e) { ActivePage.Controls.Email.AfterUpdate(); });
                        
        },





        Email: {


            AfterUpdate: function() {
                ActivePage.Controls.Email.Validate();
            },

            Validate: function() {

                var ret = true;
                var v = ctlEmail.GetValue();
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;
                
                if (v == null || Common.Email.IsValid(v)) {
                    $("#ErrEmailBadFormat").hide(t);
                    ret = true;
                } else {
                    $("#ErrEmailBadFormat").show(t);
                    ret = false;
                }

                return ret;
            }

        },



        FirstName: {


            AfterUpdate: function () {
                ActivePage.Controls.FirstName.Validate();
            },


            Validate: function () {

                var ret = true;
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                if (ctlFirstName.GetValue() == null) {
                    $("#ErrFirstNameRequired").show(t);
                    ret = false;
                } else {
                    $("#ErrFirstNameRequired").hide(t);
                }
                return ret;

            }


        },


        LastName: {


            AfterUpdate: function () {
                ActivePage.Controls.LastName.Validate();
            },


            Validate: function () {

                var ret = true;
                var t = App.Settings.DataPage.ValidationMessageTransitionTime;

                if (ctlLastName.GetValue() == null) {
                    $("#ErrLastNameRequired").show(t);
                    ret = false;
                } else {
                    $("#ErrLastNameRequired").hide(t);
                }

                return ret;

            }

        }

    },  // END Controls



    zzDummy: null




}   // END ActivePage




