using System;
using System.Collections.Generic;

namespace Dymeng.Framework.Validation
{
    public class ValidationException : Exception
    {
        public ValidationException() : base() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string format, params object[] args) : base(string.Format(format, args)) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
        public ValidationException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }

        public string GetDisplayMessage(List<ValidationError> validationErrors) {
            return GetDisplayMessage(validationErrors, "Unable to perform this action due to the following validation errors:");
        }

        public string GetDisplayMessage(List<ValidationError> validationErrors, string messageHeader) {

            string s = messageHeader + Environment.NewLine + Environment.NewLine;
            foreach(var err in validationErrors) {
                s += "\t" + err.Message + Environment.NewLine;
            }

            return s;
        }

    }
}
