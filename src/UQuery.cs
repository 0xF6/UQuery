namespace UnityEngine.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class UQuery
    {
        private static GameObject[] GetRoot() 
            => SceneManager.GetActiveScene().GetRootGameObjects();

        public static T SelectByPath<T>(string path) where T : class
        {
            if (string.IsNullOrEmpty(path))
                throw new IncorrectPathException("<null or empty>");

            var (root, keys, @class) = DeconstructPath(path);


            if (@class is null && typeof(T) != typeof(GameObject))
                throw new QueryTypeMismatchException( typeof(T), typeof(GameObject), path);

            var target = Find(root, path);

            while (true)
            {
                if(!keys.Any())
                    break;
                target = target.Child(keys.Pop());
            }

            if (@class is null)
                return target as T;
            return target.GetComponent<T>();
        }
        private static GameObject Find(string name, string path)
        {
            var result = GetRoot()
                .FirstOrDefault(x => x.name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (result is null) result = GameObject.Find(name);
            if (result is null) throw new GameObjectNotFoundByPath(name, path);
            return result;
        }

        private static (string root, Stack<string> all, string targetClass) DeconstructPath(string path)
        {
            var keys = path.Split('>');
            var first = keys.Take(1).Single();
            var @class = DeconstructToken(keys.Last(), ref keys);
            keys = keys.Skip(1).ToArray();
            return (first, new Stack<string>(keys.Reverse()), @class);
        }
        private static string DeconstructToken(string fragment, ref string[] paths)
        {
            if (fragment.Contains('[', ']'))
            {
                paths = paths.LastEdit(x => x.TakeUntil('['.IsEq()).SkipLast(1).Join()).ToArray();
                return fragment.SkipUntil('['.IsEq()).TakeUntil(']'.IsEq()).SkipLast(1).Join();
            }
            return null;
        }
    }
}