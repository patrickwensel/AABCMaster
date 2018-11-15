/* 
	Dymeng.js
	jleach@dymeng.com
	v0.1.0 - 2016/05/19
*/

var fw = {

    DateTime: {

        Convert: function (d) {
            /// <signature>
            /// <summary>Converts the date in d to a date object</summary>
            /// <param name="d" type="date">The value to convert</param>
            /// <returns type="date">The value as submitted, unconverted</returns>
            /// </signature>

            /// <signature>
            /// <summary>Converts the date in d to a date object</summary>
            /// <param name="d" type="array">The array to convert.  [year, month, day] (month is 0-based)</param>
            /// <returns type="date">The converted date, or NaN</returns>
            /// </signature>

            /// <signature>
            /// <summary>Converts the date in d to a date object</summary>
            /// <param name="d" type="number">The value to convert, interpreted as timestamp (number of milliseconds since 1970/01/01</param>
            /// <returns type="date">The converted date, or NaN</returns>
            /// </signature>

            /// <signature>
            /// <summary>Converts the date in d to a date object</summary>
            /// <param name="d" type="string">The value to convert.  Any JavaScript valid date formatted string</param>
            /// <returns type="date">The converted date, or NaN</returns>
            /// </signature>

            /// <signature>
            /// <summary>Converts the date in d to a date object</summary>
            /// <param name="d" type="object">The value to convert.  Interpreted as object with year, month and day attributes (month is 0-based)</param>
            /// <returns type="date">The converted date, or NaN</returns>
            /// </signature>
            return (
                d.constructor === Date ? d :
                    d.constructor === Array ? new Date(d[0], d[1], d[2]) :
                        d.constructor === Number ? new Date(d) :
                            d.constructor === String ? new Date(d) :
                                typeof d === "object" ? new Date(d.year, d.month, d.date) : NaN
            );
        },

        Compare: function (a, b) {
            /// <summary>Compares any two dates as supported by Common.Dates.Convert()</summary>
            /// <returns type="number">-1: a < b; 0: a = b; 1: a > b or NaN on illegal dates</returns>
            return (
                isFinite(a = this.convert(a).valueOf()) &&
                    isFinite(b = this.convert(b).valueOf()) ?
                    (a > b) - (a < b) :
                    NaN

            );
        },

        IsInRange: function (d, start, end) {
            /// <summary>Checks if date in d is between dates in start and end.  Returns bool or NaN on invalid</summary>
            return (
                isFinite(d = this.convert(d).valueOf()) &&
                    isFinite(start = this.convert(start).valueOf()) &&
                    isFinite(end = this.convert(end).valueOf()) ?
                    start <= d && d <= end : NaN
            );
        },

        /// <summary>Sets the Date portion of a date/time value to the specified date.  If date is not specified, it uses the current local date</summary>
        NormalizeTime: function (date) {
            throw "not implemented";
        },

        /// <summary>Rounds the specified time to the nearest specified minutes</summary>
        RoundTimeToNearestMinutes: function (timeToRound, minutes) {
            // Source: http://stackoverflow.com/questions/497790
            var coeff = 1000 * 60 * minutes;
            return new Date(Math.round(timeToRound.getTime() / coeff) * coeff);
        }

    },

    DevEx: {

        TimeEdit: {

            /// <summary>Sets the spinner increment minutes for the specified TimeEdit control</summary>
            SetSpinMinutesIncrement: function (ctl, minutes) {

            }

        }


    },

    Email: {
        IsValid: function (email) {
            /// <summary>Returns true if the email address appears to be a valid format, false otherwise</summary>
            var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        }
    },


    Objects: {
        MergeBasic: function (obj1, obj2) {
            /// <summary>Performs a shallow merge of obj1 and obj2 and returns the new merged object</summary>
            var obj3 = {};
            for (var attrname1 in obj1) { obj3[attrname1] = obj1[attrname1]; }
            for (var attrname2 in obj2) { obj3[attrname2] = obj2[attrname2]; }
            return obj3;
        }
    },





    /// <summary>Returns true if the value passed is a number</summary>
    IsNumeric: function (n) {
        return !isNan(parseFloat(n)) && isFinite(n);
    }


};