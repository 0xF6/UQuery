namespace UnityEngine.Query
{
    public class GameObjectNotFoundByPath : QueryException
    {
        public GameObjectNotFoundByPath(string name, string path) :
            base($"GameObject '{name}' is not found.", path) { }
    }
}