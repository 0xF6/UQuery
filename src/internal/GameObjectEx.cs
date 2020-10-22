namespace UnityEngine.Query
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

#if !UNITY_LINQ
    /// <summary>
    /// LINQ-to-GameObject-for-Unity
    /// </summary>
    // TODO use linq2go from deps
    public static class GameObjectEx
    {
        public static GameObject Child(this GameObject origin, string name)
        {
            if ((UnityEngine.Object) origin == (UnityEngine.Object) null)
                return (GameObject) null;
            Transform transform = origin.transform.Find(name);
            return (UnityEngine.Object) transform == (UnityEngine.Object) null ? (GameObject) null : transform.gameObject;
        }

        public static ChildrenEnumerable Children(this GameObject origin) 
            => new ChildrenEnumerable(origin, false);


        public struct ChildrenEnumerable : IEnumerable<GameObject>, IEnumerable
        {
            private readonly GameObject origin;
            private readonly bool withSelf;

            public ChildrenEnumerable(GameObject origin, bool withSelf)
            {
                this.origin = origin;
                this.withSelf = withSelf;
            }

            private int GetChildrenSize() => this.origin.transform.childCount + (this.withSelf ? 1 : 0);


            public GameObject[] ToArray(Func<GameObject, bool> filter)
            {
                GameObject[] array = new GameObject[this.GetChildrenSize()];
                int arrayNonAlloc = this.ToArrayNonAlloc(filter, ref array);
                if (array.Length != arrayNonAlloc)
                    Array.Resize(ref array, arrayNonAlloc);
                return array;
            }


            public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
            {
                int num = 0;
                var enumerator = this.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (filter(current))
                    {
                        if (array.Length == num)
                        {
                            int newSize = num == 0 ? this.GetChildrenSize() : num * 2;
                            Array.Resize(ref array, newSize);
                        }
                        array[num++] = current;
                    }
                }
                return num;
            }



            public Enumerator GetEnumerator() => (UnityEngine.Object) this.origin == (UnityEngine.Object) null ? 
                new Enumerator((Transform) null, this.withSelf, false) : 
                new Enumerator(this.origin.transform, this.withSelf, true);

            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator() => (IEnumerator<GameObject>) this.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();


            public struct Enumerator : IEnumerator<GameObject>, IEnumerator, IDisposable
            {
                private readonly int childCount;
                private readonly Transform originTransform;
                private readonly bool canRun;
                private bool withSelf;
                private int currentIndex;
                private GameObject current;

                internal Enumerator(Transform originTransform, bool withSelf, bool canRun)
                {
                    this.originTransform = originTransform;
                    this.withSelf = withSelf;
                    this.childCount = canRun ? originTransform.childCount : 0;
                    this.currentIndex = -1;
                    this.canRun = canRun;
                    this.current = (GameObject) null;
                }

                public bool MoveNext()
                {
                    if (!this.canRun)
                        return false;
                    if (this.withSelf)
                    {
                        this.current = this.originTransform.gameObject;
                        this.withSelf = false;
                        return true;
                    }
                    ++this.currentIndex;
                    if (this.currentIndex >= this.childCount)
                        return false;
                    this.current = this.originTransform.GetChild(this.currentIndex).gameObject;
                    return true;
                }

                public GameObject Current => this.current;

                object IEnumerator.Current => (object) this.current;

                public void Dispose()
                {
                }

                public void Reset() => throw new NotSupportedException();
            }
        }
    }

    
    #endif
}