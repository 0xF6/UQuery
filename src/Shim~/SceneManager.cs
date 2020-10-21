namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace SceneManagement
    {
        public class SceneManager
        {
            private static Scene _scene = new Scene();
            public static Scene GetActiveScene() => _scene;
        }
    }
    


    public static class __GO__
    {
        public static IDictionary<string, GameObject> goStorage = new Dictionary<string, GameObject>();
        public static void Add(GameObject go) => goStorage.Add(go.name, go);
    }

    public class Scene
    {
        public GameObject[] GetRootGameObjects() => __GO__.goStorage.Values.ToArray();
    }

    public class Object
    {
        public string name;
    }

    public class Transform : Object
    {
        public List<GameObject> Childs = new List<GameObject>();

        public Transform(GameObject go) => this.gameObject = go;

        public GameObject gameObject;

        public Transform Find(string name) => Childs.FirstOrDefault(x => x.name == name)?.transform;
    }

    public class Component : Object
    {
    }
    public class GameObject : Object
    {
        public Transform transform;

        public List<object> components = new List<object>();

        public GameObject(string Name)
        {
            this.name = Name;
            this.transform = new Transform(this);
        }

        public T GetComponent<T>() where T : class 
            => components.FirstOrDefault(x => x.GetType() == typeof(T)) as T;

        public static GameObject Find(string name)
        {
            if (__GO__.goStorage.ContainsKey(name))
                return __GO__.goStorage[name];
            return null;
        }
    }
}