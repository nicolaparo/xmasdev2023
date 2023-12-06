using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookieFactory.Collector
{
    public static class AsyncEnumerableHelpers
    {
        public static async IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            await foreach (var item in source)
                yield return selector(item);
        }
        public static async IAsyncEnumerable<TResult> SelectAwait<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            foreach (var item in source)
                yield return await selector(item);
        }
        public static async IAsyncEnumerable<TResult> SelectAwait<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            await foreach (var item in source)
                yield return await selector(item);
        }

        public static async IAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            await foreach (var item in source)
                foreach (var result in selector(item))
                    yield return result;
        }
        public static async IAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<IEnumerable<TResult>>> selector)
        {
            foreach (var item in source)
                foreach (var result in await selector(item))
                    yield return result;
        }
        public static async IAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, Task<IEnumerable<TResult>>> selector)
        {
            await foreach (var item in source)
                foreach (var result in await selector(item))
                    yield return result;
        }
        public static async IAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IAsyncEnumerable<TResult>> selector)
        {
            foreach (var item in source)
                await foreach (var result in selector(item))
                    yield return result;
        }
        public static async IAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, IAsyncEnumerable<TResult>> selector)
        {
            await foreach (var item in source)
                await foreach (var result in selector(item))
                    yield return result;
        }


        public static async IAsyncEnumerable<TSource> Where<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            await foreach (var item in source)
                if (predicate(item))
                    yield return item;
        }
        public static async IAsyncEnumerable<TSource> WhereAwait<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> predicate)
        {
            foreach (var item in source)
                if (await predicate(item))
                    yield return item;
        }
        public static async IAsyncEnumerable<TSource> WhereAwait<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, Task<bool>> predicate)
        {
            await foreach (var item in source)
                if (await predicate(item))
                    yield return item;
        }

        public static async Task<int> SumAsync(this IAsyncEnumerable<int> values)
        {
            var result = 0;
            await foreach (var item in values)
                result += item;
            return result;
        }
        public static async Task<int> SumAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return await source.Select(selector).SumAsync();
        }

        public static async Task<List<TValue>> ToListAsync<TValue>(this IAsyncEnumerable<TValue> source)
        {
            var result = new List<TValue>();
            await foreach (var value in source)
                result.Add(value);
            return result;
        }
        public static async Task<TValue[]> ToArrayAsync<TValue>(this IAsyncEnumerable<TValue> source)
        {
            return (await source.ToListAsync()).ToArray();
        }
        public static async Task<Dictionary<TKey, TValue>> ToDictionaryAsync<TSource, TKey, TValue>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey, TValue>();

            await foreach (var entry in source)
                dictionary[keySelector(entry)] = valueSelector(entry);

            return dictionary;
        }
    }
}
