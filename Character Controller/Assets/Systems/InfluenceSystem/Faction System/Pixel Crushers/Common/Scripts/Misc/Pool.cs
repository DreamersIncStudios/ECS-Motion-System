// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers
{

    /// <summary>
    /// This generic class implements an object pool. It helps prevent garbage collection
    /// stutter by reusing objects without allocating and deallocating memory.
    /// </summary>
    public class Pool<T> where T : new()
    {

        private List<T> m_free = new List<T>();

        private List<T> m_used = new List<T>();

        /// <summary>
        /// Gets an object from the pool. After getting an object, you should 
        /// initialize its fields because it will have the values from its
        /// previous use.
        /// </summary>
        public T Get()
        {
            lock (m_free)
            {
                if (m_free.Count > 0)
                {
                    T item = m_free[0];
                    m_used.Add(item);
                    m_free.RemoveAt(0);
                    return item;
                }
                else
                {
                    T item = new T();
                    m_used.Add(item);
                    return item;
                }
            }
        }

        /// <summary>
        /// Releases an object back to the pool.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Release(T item)
        {
            lock (m_free)
            {
                m_free.Add(item);
                m_used.Remove(item);
            }
        }

        /// <summary>
        /// Preallocates a number of objects into the pool.
        /// </summary>
        /// <param name="initialSize">Initial size.</param>
        public void Allocate(int initialSize)
        {
            while (m_free.Count < initialSize)
            {
                m_free.Add(new T());
            }
        }

        /// <summary>
        /// Trims the pool to a maximum number of objects.
        /// </summary>
        /// <param name="max">Max objects.</param>
        public void Trim(int max)
        {
            m_free.RemoveRange(0, Mathf.Min(m_free.Count, max));
        }

    }

}