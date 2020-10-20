namespace UnityEngine.Query
{
    using System;

    public class QueryTypeMismatchException : QueryException
    {
        public QueryTypeMismatchException(Type enter, Type target, string path) 
            : base($"Type mismath. [{enter.Name} != {target.Name}]", path) { }
    }
}