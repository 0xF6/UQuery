namespace UnityEngine.Query
{
    using System;
    using System.Collections.Generic;

    internal interface IIterable<T>
    {
        IIterator<T> Iterator();
    }
    internal interface IIterator<T> : IDisposable
    {
        bool HasNext { get; }
        T Next();
        void Remove();
    }
    internal sealed class EnumerableAdapter<T> : IIterable<T>
    {
        private readonly IEnumerable<T> enumerable;
        public EnumerableAdapter(IEnumerable<T> enumerable) => 
            this.enumerable = enumerable;
        public IIterator<T> Iterator() => 
            new EnumeratorAdapter<T>(enumerable.GetEnumerator());
    }
    

    internal sealed class EnumeratorAdapter<T> : IIterator<T>
    {
        private readonly IEnumerator<T> enumerator;

        private bool fetchedNext = false;
        private bool nextAvailable = false;
        private T next;

        public EnumeratorAdapter(IEnumerator<T> enumerator) => 
            this.enumerator = enumerator;

        public bool HasNext
        {
            get
            {
                CheckNext();
                return nextAvailable;
            } 
        }

        public T Next()
        {
            CheckNext();
            if (!nextAvailable)
                throw new InvalidOperationException();
            fetchedNext = false;
            return next;
        }

        void CheckNext()
        {
            if (!fetchedNext)
            {
                nextAvailable = enumerator.MoveNext();
                if (nextAvailable) next = enumerator.Current;
                fetchedNext = true;            
            }
        }

        public void Remove() => throw new NotSupportedException();

        public void Dispose() => enumerator.Dispose();
    }
}