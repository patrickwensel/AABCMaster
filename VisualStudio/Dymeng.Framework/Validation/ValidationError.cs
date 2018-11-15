using System;
using System.Collections.Generic;

namespace Dymeng.Framework.Validation
{
    public class ValidationError : ValidationIssue
    {    
        public static void AddError(List<ValidationError> errors, int? id, string message, Exception exception) {
            var err = new ValidationError();
            err.ID = id;
            err.Message = message;
            err.Exception = exception;
            errors.Add(err);
        }
    }
}
