(() => {

    var q = QUnit;
    var t = q.test;

    q.module("HoursEntry.DateManager");

    t("DateManagerBuilds", (assert) => {
        var dm = new window.aabc.hoursEntry.DateManager();
        assert.ok(true, "No errors thrown");
    });


})();
