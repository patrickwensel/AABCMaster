using System;

namespace Dymeng.Framework.COM
{
    public class Exceptions
    {


        private const int INVALID_DATA_CONTEXT = -1;


        public static Exception COMException(int errorNumber, string message) {
            return new Exception(GetCOMExceptionMessage(errorNumber, message));
        }


        public static Exception InvalidDataContextException() {
            // returns a COMException Exception with default error number and text
            return new Exception(GetCOMExceptionMessage(INVALID_DATA_CONTEXT, "Invalid Data Context"));
        }


        /// <summary>
        /// Returns a string in the format "[ErrorNumber] Optional Message"
        /// </summary>
        /// <param name="errorNumber">int: value of the error to go into the string</param>
        /// <param name="message">optional message to display after the error number</param>
        /// <returns>string: bracketed error number and the optional message</returns>
        public static string GetCOMExceptionMessage(int errorNumber, string message = null) {
            string s = "[" + errorNumber.ToString() + "]";
            if (!string.IsNullOrEmpty(message)) {
                s = s + " " + message;
            }
            return message;
        }

    }
}
