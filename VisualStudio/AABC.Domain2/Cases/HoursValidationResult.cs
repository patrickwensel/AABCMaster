using Dymeng.Framework.Validation;
using System.Collections.Generic;

namespace AABC.Domain2.Cases
{
    public class HoursValidationResult
    {

        public bool IsAcceptable { get; set; } = false;
        public IEnumerable<ValidationError> ValidationErrors { get; set; } = new List<ValidationError>();
        
    }
}
