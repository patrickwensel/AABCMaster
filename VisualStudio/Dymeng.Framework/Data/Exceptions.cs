using System;

namespace Dymeng.Framework.Data
{


    /// <summary>
    /// Thrown when an Execute command causes an error
    /// </summary>
    public class DataExecutionException : DataBaseException
    {
        public DataExecutionException() : base() { }
        public DataExecutionException(string message) : base(message) { }
        public DataExecutionException(string format, params object[] args) : base(string.Format(format, args)) { }
        public DataExecutionException(string message, Exception innerException) : base(message, innerException) { }
        public DataExecutionException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
    }

    /// <summary>
    /// Throw when an update command cannot be executed as expected
    /// </summary>
    public class DataSelectException : DataBaseException
    {
        public DataSelectException() : base() { }
        public DataSelectException(string message) : base(message) { }
        public DataSelectException(string format, params object[] args) : base(string.Format(format, args)) { }
        public DataSelectException(string message, Exception innerException) : base(message, innerException) { }
        public DataSelectException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
    }

    /// <summary>
    /// Throw when an update command cannot be executed as expected
    /// </summary>
    public class DataUpdateException : DataBaseException
    {
        public DataUpdateException() : base() { }
        public DataUpdateException(string message) : base(message) { }
        public DataUpdateException(string format, params object[] args) : base(string.Format(format, args)) { }
        public DataUpdateException(string message, Exception innerException) : base(message, innerException) { }
        public DataUpdateException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
    }

    /// <summary>
    /// Throw when an insertion command cannot be executed as expected
    /// </summary>
    public class DataInsertException : DataBaseException
    {
        public DataInsertException() : base() { }
        public DataInsertException(string message) : base(message) { }
        public DataInsertException(string format, params object[] args) : base(string.Format(format, args)) { }
        public DataInsertException(string message, Exception innerException) : base(message, innerException) { }
        public DataInsertException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
    }

    /// <summary>
    /// Throw when a select statement is expected to return a single row but none are found
    /// </summary>
    public class DataRowNotFoundException : DataBaseException
    {
        public DataRowNotFoundException() : base() { }
        public DataRowNotFoundException(string message) : base(message) { }
        public DataRowNotFoundException(string format, params object[] args) : base(string.Format(format, args)) { }
        public DataRowNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        public DataRowNotFoundException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
    }


    /// <summary>
    /// Throw when a Select statement returns multiple rows but is expected to return zero or one only
    /// </summary>
    public class DataSingleRowExpectedException : DataBaseException
    {
        public DataSingleRowExpectedException() : base() { }
        public DataSingleRowExpectedException(string message) : base(message) { }
        public DataSingleRowExpectedException(string format, params object[] args) : base(string.Format(format, args)) { }
        public DataSingleRowExpectedException(string message, Exception innerException) : base(message, innerException) { }
        public DataSingleRowExpectedException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
    }


    /// <summary>
    /// Throw when a data operation cannot be completed because of a constraint rule elsewhere
    /// </summary>
    public class DataConstraintException : DataBaseException
    {
        public DataConstraintException() : base() { }
        public DataConstraintException(string message) : base(message) { }
        public DataConstraintException(string format, params object[] args) : base(string.Format(format, args)) { }
        public DataConstraintException(string message, Exception innerException) : base(message, innerException) { }
        public DataConstraintException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
    }



    /// <summary>
    /// Base class for custom database exceptions.  Includes ConnectionString parameter
    /// </summary>
    public abstract class DataBaseException : Exception
    {
        public string ConnectionString { get; set; }
        public string CommandText { get; set; }

        public DataBaseException() : base() { }
        public DataBaseException(string message) : base(message) { }
        public DataBaseException(string format, params object[] args) : base(string.Format(format, args)) { }
        public DataBaseException(string message, Exception innerException) : base(message, innerException) { }
        public DataBaseException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
    }

}
