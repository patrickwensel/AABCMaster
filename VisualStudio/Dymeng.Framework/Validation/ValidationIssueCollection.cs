using System;
using System.Collections.Generic;
using System.Linq;

namespace Dymeng.Framework.Validation
{
    public class ValidationIssueCollection
    {


        public List<ValidationIssue> Issues { get; set; } = new List<ValidationIssue>();

        public List<ValidationWarning> Warnings { get { return getWarningsIterator().ToList(); } }
        public List<ValidationError> Errors { get { return getErrorsIterator().ToList(); } }

        public bool HasErrors { get { return getErrorsIterator().ToList().Count == 0 ? false : true; } }
        public bool HasWarnings { get { return getWarningsIterator().ToList().Count == 0 ? false : true; } }


        public string GetFriendlyErrors() {
            return GetFriendlyErrors(Environment.NewLine);
        }

        public string GetFriendlyErrors(string newline) {
            string returnValue = null;

            foreach (var issue in Errors) {
                returnValue += issue.Message + Environment.NewLine;
            }
            return returnValue;
        }

        public string GetFriendlyWarnings() {
            return GetFriendlyWarnings(Environment.NewLine);
        }

        public string GetFriendlyWarnings(string newline) {
            string returnValue = null;

            foreach (var issue in Warnings) {
                returnValue += issue.Message + Environment.NewLine;
            }

            return returnValue;
        }

        public string GetFriendlyMessages() {
            return GetFriendlyMessages(Environment.NewLine);
        }

        public string GetFriendlyMessages(string newline) {

            string returnValue = null;

            foreach (var issue in Issues) {
                returnValue += issue.Message + Environment.NewLine;
            }

            return returnValue;
        }


        public void AddIssue(string message) {
            AddIssue(null, message, null, ValidationIssueType.Error);
        }

        public void AddIssue(string message, ValidationIssueType type) {
            AddIssue(null, message, null, type);
        }

        public void AddIssue(int? id, string message, Exception exception, ValidationIssueType type) {

            if (type == ValidationIssueType.Error) {

                Issues.Add(new ValidationError()
                {
                    ID = id,
                    Message = message,
                    Exception = exception
                });

            } else {

                Issues.Add(new ValidationWarning()
                {
                    ID = id,
                    Message = message,
                    Exception = exception
                });

            }

        }

        private IEnumerable<ValidationError> getErrorsIterator() {
            foreach (var issue in Issues) {
                if (issue is ValidationError) {
                    yield return issue as ValidationError;
                }
            }
        }

        private IEnumerable<ValidationWarning> getWarningsIterator() {
            foreach (var issue in Issues) {
                if (issue is ValidationWarning) {
                    yield return issue as ValidationWarning;
                }
            }
        }
        
    }
}
