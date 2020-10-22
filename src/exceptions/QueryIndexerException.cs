namespace UnityEngine.Query
{
    public class QueryIndexerException : QueryException
    {
        public QueryIndexerException(string key, string rawKey, int index, int childLen, string path) : 
            base($"The indexer '{key}' is not valid. Index: '{index}', childrens size (with '{rawKey}' name): '{childLen}'", path)
        { }
    }
}