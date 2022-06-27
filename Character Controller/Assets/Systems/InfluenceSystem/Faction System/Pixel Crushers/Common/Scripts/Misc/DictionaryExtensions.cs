// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelCrushers
{

    /// <summary>
    /// Extension methods for generic dictionaries.
    /// </summary>
    public static class DictionaryExtensions
    {

        /// <summary>
        /// Works like List.RemoveAll.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary to remove entries from</param>
        /// <param name="match">Delegate to match keys</param>
        /// <returns>Number of entries removed</returns>
        public static int RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Predicate<TKey> match)
        {
            if (dictionary == null || match == null) return 0;
            var keysToRemove = dictionary.Keys.Where(k => match(k)).ToList();
            if (keysToRemove.Count > 0)
            {
                foreach (var key in keysToRemove)
                {
                    dictionary.Remove(key);
                }
            }
            return keysToRemove.Count;
        }

    }

}
