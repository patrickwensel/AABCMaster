
using CommandLine;

namespace AABC.Services
{

    /// <summary>
    /// Contains standard options present for all commands.  
    /// All handler options classes should inherit from this
    /// T is the final options class that the handler will accept
    /// </summary>
    public abstract class VerbOptionsBase
    {
        [Option('w', "wait", Required = false, DefaultValue = false)]
        public bool WaitOnExit { get; set; } = false;

        public abstract bool ValidateInput();

        public abstract object Transform();
    }
}
