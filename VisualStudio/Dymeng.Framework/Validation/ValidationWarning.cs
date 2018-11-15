using System;
using System.Collections.Generic;

namespace Dymeng.Framework.Validation
{
    public class ValidationWarning : ValidationIssue
    {        
        public static void AddWarning(List<ValidationWarning> warnings, int? id, string message, Exception exception) {
            var warning = new ValidationWarning();
            warning.ID = id;
            warning.Message = message;
            warning.Exception = exception;
            warnings.Add(warning);
        }

    }

}
