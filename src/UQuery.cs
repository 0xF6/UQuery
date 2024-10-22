﻿namespace UnityEngine.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine.SceneManagement;

    public static class UQuery
    {
        private static GameObject[] GetRoot() 
            => SceneManager.GetActiveScene().GetRootGameObjects();
        /// <summary>
        /// Select <see cref="GameObject"/> by path.
        /// </summary>
        /// <param name="path">
        /// Query path
        /// </param>
        /// <returns>
        /// <see cref="GameObject"/>
        /// </returns>
        /// <exception cref="QueryTypeMismatchException">
        /// Generic type is not GameObject when class it not specified in query path.
        /// </exception>
        /// <exception cref="GameObjectNotFoundByPath">
        /// GameObject by name not found in scene.
        /// </exception>
        /// <exception cref="IncorrectPathException">
        /// Path is not correct. (Null or Empty)
        /// </exception>
        public static GameObject SelectByPath(string path) 
            => SelectByPath<GameObject>(path);
        /// <summary>
        /// Select <see cref="GameObject"/> or <see cref="T"/> by path.
        /// </summary>
        /// <typeparam name="T">
        /// <see cref="GameObject"/> or Unity Component
        /// </typeparam>
        /// <param name="path">
        /// Query path
        /// </param>
        /// <returns>
        /// <see cref="GameObject"/> or Unity Component.
        /// </returns>
        /// <exception cref="QueryTypeMismatchException">
        /// Generic type is not GameObject when class it not specified in query path.
        /// </exception>
        /// <exception cref="GameObjectNotFoundByPath">
        /// GameObject by name not found in scene.
        /// </exception>
        /// <exception cref="IncorrectPathException">
        /// Path is not correct. (Null or Empty)
        /// </exception>
        public static T SelectByPath<T>(string path) where T : class
        {
            if (string.IsNullOrEmpty(path))
                throw new IncorrectPathException("<null or empty>");

            var (root, keys, @class) = DeconstructPath(path);

            if (@class is null && typeof(T) != typeof(GameObject))
                throw new QueryTypeMismatchException(typeof(T), typeof(GameObject), path);

            var target = Find(root, path);

            while (true)
            {
                if(!keys.Any())
                    break;
                var key = keys.Pop();
                var indexer = DeconstructIndexer(key);

                if (indexer != default)
                {
                    var childres = target
                        .Children()
                        .ToArray(x => x.name == indexer.key);

                    if (childres.Length == 0)
                        throw new GameObjectNotFoundByPath(key, path);

                    if (childres.Length <= indexer.index)
                        throw new QueryIndexerException(key, indexer.key, indexer.index ?? 0, childres.Length, path);
                    target = childres[indexer.index ?? 0];
                }
                else
                {
                    target = target.Child(key);
                    if (target is null)
                        throw new GameObjectNotFoundByPath(key, path);
                }
            }

            if (@class is null)
                return target as T;
            return target?.GetComponent<T>();
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

        internal static (int? index, string key) DeconstructIndexer(string key)
        {
            (int? index, string key) defaultValue = (null, null);
            if (string.IsNullOrEmpty(key))
                return defaultValue;
            if (!key.Contains(':', '(', ')'))
                return defaultValue;

            Func<char, char, bool> q = (cur, last) => cur == '(' && last == ':';

            var indexer = key.SkipUntilWithMemory(q).Join();
            var rawKey = key.TakeUntilWithMemory(q).Join();
            if (indexer.Last() != ')')
                return defaultValue;
            if (!rawKey.EndsWith(":("))
                return defaultValue;

            if (int.TryParse(indexer.SkipLast(1).Join(), out var index))
                return (index, rawKey.SkipLast(2).Join());
            return defaultValue;

        }
    }

    // TODO
    /*public struct UnityPath
    {
        public string ClassName { get; set; }
        public string[] Keys { get; set; }
        public string RootKey { get; set; }
        public string LastKey { get; set; }
    }*/
}