namespace UnityEngine.Query
{
    using System;

    public class QueryException : Exception
    {
        private readonly string _path;
        private readonly string _message;

        public QueryException(string path, string message) : base()
        {
            _path = path;
            _message = message;
        }

        public override string Message => $"{_message} Path: [{_path}]";
    }
}