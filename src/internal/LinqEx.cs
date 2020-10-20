namespace UnityEngine.Query
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class LinqEx
    {
        internal static IIterable<T> AsIterable<T>(this IEnumerable<T> collection) 
            => new EnumerableAdapter<T>(collection);

        


        #region MoreLinq
        /* START
         *
         * morelinq/MoreLINQ is licensed under the
         *          Apache License 2.0
         */
        internal static int? TryGetCollectionCount<T>(this IEnumerable<T> source) =>
            source switch
            {
                null => throw new ArgumentNullException(nameof(source)),
                ICollection<T> collection => collection.Count,
                IReadOnlyCollection<T> collection => collection.Count,
                _ => null
            };
        internal interface IListLike<out T>
        {
            int Count { get; }
            T this[int index] { get; }
        }
        internal sealed class List<T> : IListLike<T>
        {
            readonly IList<T> _list;
            public List(IList<T> list) => _list = list ?? throw new ArgumentNullException(nameof(list));
            public int Count => _list.Count;
            public T this[int index] => _list[index];
        }
        internal sealed class ReadOnlyList<T> : IListLike<T>
        {
            readonly IReadOnlyList<T> _list;
            public ReadOnlyList(IReadOnlyList<T> list) => _list = list ?? throw new ArgumentNullException(nameof(list));
            public int Count => _list.Count;
            public T this[int index] => _list[index];
        }
        internal static IListLike<T> TryAsListLike<T>(this IEnumerable<T> source) =>
            source switch
            {
                null => throw new ArgumentNullException(nameof(source)),
                IList<T> list => new List<T>(list),
                IReadOnlyList<T> list => new ReadOnlyList<T>(list),
                _ => null
            };

        internal static IEnumerable<TResult> CountDown<T, TResult>(this IEnumerable<T> source,
            int count, Func<T, int?, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            return source.TryAsListLike() is {} listLike
                ? IterateList(listLike)
                : source.TryGetCollectionCount() is {} collectionCount
                    ? IterateCollection(collectionCount)
                    : IterateSequence();

            IEnumerable<TResult> IterateList(IListLike<T> list)
            {
                var countdown = Math.Min(count, list.Count);

                for (var i = 0; i < list.Count; i++)
                {
                    var cd = list.Count - i <= count
                        ? --countdown
                        : (int?) null;
                    yield return resultSelector(list[i], cd);
                }
            }

            IEnumerable<TResult> IterateCollection(int i)
            {
                foreach (var item in source)
                    yield return resultSelector(item, i-- <= count ? i : (int?) null);
            }

            IEnumerable<TResult> IterateSequence()
            {
                var queue = new Queue<T>(Math.Max(1, count + 1));

                foreach (var item in source)
                {
                    queue.Enqueue(item);
                    if (queue.Count > count)
                        yield return resultSelector(queue.Dequeue(), null);
                }

                while (queue.Count > 0)
                    yield return resultSelector(queue.Dequeue(), queue.Count);
            }
        }
        internal static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (count < 1)
                return source;

            return
                source.TryGetCollectionCount() is {} collectionCount
                    ? source.Take(collectionCount - count)
                    : source.CountDown(count, (e, cd) => (Element: e, Countdown: cd ))
                        .TakeWhile(e => e.Countdown == null)
                        .Select(e => e.Element);
        }
        internal static IEnumerable<TSource> TakeUntil<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return _(); IEnumerable<TSource> _()
            {
                foreach (var item in source)
                {
                    yield return item;
                    if (predicate(item))
                        yield break;
                }
            }
        }
        internal static IEnumerable<TSource> SkipUntil<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return _(); IEnumerable<TSource> _()
            {
                using var enumerator = source.GetEnumerator();

                do
                {
                    if (!enumerator.MoveNext())
                        yield break;
                }
                while (!predicate(enumerator.Current));

                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }
        /* END */
        #endregion
        internal static Func<T, bool> IsEq<T>(this T t) => x => x.Equals(t);
        internal static string Join(this IEnumerable<char> t) => string.Join("", t);

        internal static bool Contains<T>(this IEnumerable<T> t, params T[] target) 
            => t.Contains(target, Comparer.Default);

        internal static bool Contains<T>(this IEnumerable<T> t, T[] target, IComparer comparer)
        {
            var sorder = t.ToArray();
            Array.Sort(sorder, comparer);
            return target.All(x => Array.BinarySearch(sorder, x) > 0);
        }


        internal static IEnumerable<T> LastEdit<T>(this IEnumerable<T> collection, Func<T, T> editor)
        {
            var iterator = collection.AsIterable().Iterator();

            while (iterator.HasNext)
            {
                var next = iterator.Next();
                if (!iterator.HasNext)
                    yield return editor(next);
                else
                    yield return next;
            }
        }
    }
}