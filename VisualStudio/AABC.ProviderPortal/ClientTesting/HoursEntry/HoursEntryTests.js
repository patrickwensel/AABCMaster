(() => {

    var q = QUnit;
    var t = q.test;

    q.module("HoursEntry.ViewModel");
    


    function BlankEntryBCBA() {
        return {
            EntryID: null,
            Status: 0,
            IsAdminMode: false,
            HasData: false,
            CatalystPreLoadID: null,
            ActivePatients: [],
            Date: '2018-03-01T00:00:00',
            InsuranceID: null,
            InsuranceName: 'Select a date to determine the active insurance',
            IsEditable: true,
            IsNonParentSSGEntry: false,
            IsTrainingEntry: false,
            NonEditableReason: null,
            Note: null,
            NoteGroups: [],
            PatientID: 123,
            PatientName: 'Mary Smith',
            ProviderID: 456,
            ProviderTypeCode: 'BCBA',
            ProviderTypeID: 15,
            SSGCaseIDs: null,
            ServiceID: null,
            ServiceLocationID: 1,
            ServiceLocations: [
                { ID: 1, Name: 'Home' },
                { ID: 4, Name: 'Other' }
            ],
            Services: [
                { ID: 9, Code: 'DR', Description: 'Direct Care', TypeID: 2, TypeName: 'Care' },
                { ID: 14, Code: 'SSG', Description: 'Social Skills Group', TypeID: 3, TypeName: 'Social' },
                { ID: 15, Code: 'SDR', Description: 'Supervision Received', TypeID: 4, TypeName: 'Supervision' }
            ],
            TimeIn: null,
            TimeOut: null
        }
    }

    function BlankEntryAide() {
        return {
            EntryID: null,
            Status: 0,
            IsAdminMode: false,
            HasData: false,
            CatalystPreLoadID: null,
            ActivePatients: [],
            Date: '2018-03-01T00:00:00',
            InsuranceID: null,
            InsuranceName: 'Select a date to determine the active insurance',
            IsEditable: true,
            IsNonParentSSGEntry: false,
            IsTrainingEntry: false,
            NonEditableReason: null,
            Note: null,
            NoteGroups: [],
            PatientID: 123,
            PatientName: 'Mary Smith',
            ProviderID: 456,
            ProviderTypeCode: 'AIDE',
            ProviderTypeID: 17,
            SSGCaseIDs: null,
            ServiceID: null,
            ServiceLocationID: 1,
            ServiceLocations: [
                { ID: 1, Name: 'Home' },
                { ID: 4, Name: 'Other' }
            ],
            Services: [
                { ID: 9, Code: 'DR', Description: 'Direct Care', TypeID: 2, TypeName: 'Care' },
                { ID: 14, Code: 'SSG', Description: 'Social Skills Group', TypeID: 3, TypeName: 'Social' },
                { ID: 15, Code: 'SDR', Description: 'Supervision Received', TypeID: 4, TypeName: 'Supervision' }
            ],
            TimeIn: null,
            TimeOut: null
        }
    }

    function getVM(model) {
        return new window.aabc.hoursEntry.HoursEntryVM(model, null, null, null, "test");
    }


    t("SelectedServiceDefaultsToServiceIDLink", (assert) => {
        var model = new BlankEntryAide();
        model.ServiceID = 15;
        var vm = getVM(model);
        assert.ok(vm.selectedService().id() === model.ServiceID);
    });

    t("IsSSGTrueIfSocialService", (assert) => {
        var model = new BlankEntryAide();
        model.ServiceID = 14;
        var vm = getVM(model);
        assert.ok(vm.isSSG() === true);
    });

    t("IsSSGFalseIfNotSocialService", (assert) => {
        var model = new BlankEntryAide();
        model.ServiceID = 0;
        var vm = getVM(model);
        assert.ok(vm.isSSG() === false);
    });

    t("HasDateTrueIfDate", (assert) => {
        var model = new BlankEntryAide();
        model.Date = '2018-01-01';
        var vm = getVM(model);
        assert.ok(vm.hasDate() === true);
    });

    t("HasDateFalseIfNoDate", (assert) => {
        var model = new BlankEntryAide();
        model.Date = null;
        var vm = getVM(model);
        assert.ok(vm.hasDate() === false);
    });

    t("BuildsBlankAide", (assert) => {
        var model = new BlankEntryAide();
        var vm = getVM(model);
        assert.ok(true, "Blank Aide model builds");
    });

    t("BuildsBlankBCBA", (assert) => {
        var model = new BlankEntryBCBA();
        var vm = getVM(model);
        assert.ok(true, "Blank BCBA model builds");
    });

    t("ShowNotesContainerTrueIfBCBA", (assert) => {
        var model = new BlankEntryBCBA();
        var vm = getVM(model);
        assert.ok(vm.showNotesContainer() === true, "Notes container showing for BCBA");
    });

    t("ShowNoteContainerFalseIfBCBA", (assert) => {
        var model = new BlankEntryBCBA();
        var vm = getVM(model);
        assert.ok(vm.showNoteContainer() === false, "Note container not showing for BCBA");
    });

    t("ShowNotesContainerFalseIfAide", (assert) => {
        var model = new BlankEntryAide();
        var vm = getVM(model);
        assert.ok(vm.showNotesContainer() === false, "Notes container not showing for Aide");
    });

    t("ShowNoteContainerFalseIfAide", (assert) => {
        var model = new BlankEntryAide();
        var vm = getVM(model);
        assert.ok(vm.showNoteContainer() === true, "Note container showing for Aide");
    });


})();
