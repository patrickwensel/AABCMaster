using System;

namespace Dymeng.Framework.Validation
{

    public enum ValidationIssueType
    {
        Warning,
        Error
    }

    public abstract class ValidationIssue
    {

        public int? ID { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

    }
}
