namespace UnityEngine.Query
{
    #if !UNITY_LINQ
    /// <summary>
    /// LINQ-to-GameObject-for-Unity
    /// </summary>
    public static class GameObjectEx
    {
        public static GameObject Child(this GameObject origin, string name)
        {
            if ((UnityEngine.Object) origin == (UnityEngine.Object) null)
                return (GameObject) null;
            Transform transform = origin.transform.Find(name);
            return (UnityEngine.Object) transform == (UnityEngine.Object) null ? (GameObject) null : transform.gameObject;
        }
    }
    #endif
}