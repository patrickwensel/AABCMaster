using System;
using System.Collections.Generic;

namespace Dymeng.Framework.Validation
{
    public static class ValidationIssues
    {

        // someday we should test and refactor this into a single logic unit...

        public static string GetDisplayMessages(List<ValidationWarning> warnings, bool reportUnknown = true) {

            string msg = "";

            foreach (var warning in warnings) {

                if (string.IsNullOrEmpty(warning.Message)) {

                    if (warning.Exception != null) {

                        // report exception
                        msg = addNewMessageLine(msg);
                        msg += warning.Exception.ToString();

                    } else {

                        // report unknown if applicable
                        if (reportUnknown) {
                            msg = addNewMessageLine(msg);
                            msg += "Unknown validation error";
                            if (warning.ID.HasValue) {
                                msg = addNewMessageLine(msg);
                                msg += " (Code " + warning.ID.Value.ToString() + ")";
                            }
                        }

                    }

                } else {

                    msg = addNewMessageLine(msg);
                    msg += warning.Message;

                }

            }

            return msg;
        }


        public static string GetDisplayMessages(List<ValidationError> errors, bool reportUnknown = true) {

            string msg = "";

            foreach (var err in errors) {

                if (string.IsNullOrEmpty(err.Message)) {

                    if (err.Exception != null) {

                        // report exception
                        msg = addNewMessageLine(msg);
                        msg += err.Exception.ToString();

                    } else {

                        // report unknown if applicable
                        if (reportUnknown) {
                            msg = addNewMessageLine(msg);
                            msg += "Unknown validation error";
                            if (err.ID.HasValue) {
                                msg = addNewMessageLine(msg);
                                msg += " (Code " + err.ID.Value.ToString() + ")";
                            }
                        }

                    }

                } else {

                    msg = addNewMessageLine(msg);
                    msg += err.Message;

                }

            }

            return msg;
        }

        public static string GetDisplayMessages(List<ValidationIssue> issues, bool reportUnknown = true) {

            string msg = "";

            foreach (var issue in issues) {

                if (string.IsNullOrEmpty(issue.Message)) {

                    if (issue.Exception != null) {

                        // report exception
                        msg = addNewMessageLine(msg);
                        msg += issue.Exception.ToString();

                    } else {

                        // report unknown if applicable
                        if (reportUnknown) {
                            msg = addNewMessageLine(msg);
                            msg += "Unknown validation error";
                            if (issue.ID.HasValue) {
                                msg = addNewMessageLine(msg);
                                msg += " (Code " + issue.ID.Value.ToString() + ")";
                            }
                        }

                    }

                } else {

                    msg = addNewMessageLine(msg);
                    msg += issue.Message;

                }

            }

            return msg;

        }


        static string addNewMessageLine(string msg) {
            
            // don't add newlines if it's a blank message
            if (string.IsNullOrEmpty(msg)) {
                return msg;
            }

            return msg += Environment.NewLine + Environment.NewLine;
        }

    }
}
