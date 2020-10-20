namespace UnityEngine.Query
{
    public class IncorrectPathException : QueryException
    {
        public IncorrectPathException(string path) : 
            base($"Path is not correct format.", path) { }
    }
}