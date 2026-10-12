namespace LibraryManager.Exceptions
{
    /// <summary>
    /// Custom exception thrown when book details break a validation rule.
    /// Inherits from Exception (the recommended base class for custom exceptions).
    /// </summary>
    public class BookException : Exception
    {
        public string FieldName { get; }

        public BookException(string fieldName, string message)
            : base(message)
        {
            FieldName = fieldName;
        }
    }
}
