// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_WINRT
using System.Linq.Expressions;
using System.Reflection;
#endif

namespace PixelCrushers
{

    /// <summary>
    /// Utility functions for creating ScriptableObjects.
    /// </summary>
    public static class ScriptableObjectUtility
    {

        /// <summary>
        /// Create a ScriptableObject of type T, calling the method Initialize() if present.
        /// </summary>
        /// <typeparam name="T">The ScriptableObject type.</typeparam>
        /// <returns>The new ScriptableObject.</returns>
        public static T CreateScriptableObject<T>() where T : ScriptableObject
        {
            var scriptableObject = ScriptableObject.CreateInstance<T>() as T;
            InitializeScriptableObject(scriptableObject);
            return scriptableObject;
        }

        /// <summary>
        /// Create a ScriptableObject of a specified type, calling Initialize() if present.
        /// </summary>
        /// <param name="type">The ScriptableObject type.</param>
        /// <returns>The new ScriptableObject.</returns>
        public static ScriptableObject CreateScriptableObject(Type type)
        {
            var scriptableObject = ScriptableObject.CreateInstance(type);
            InitializeScriptableObject(scriptableObject);
            return scriptableObject;
        }

        /// <summary>
        /// Calls Initialize() on a ScriptableObject if present.
        /// </summary>
        /// <param name="scriptableObject">The ScriptableObject to initialize.</param>
        public static void InitializeScriptableObject(ScriptableObject scriptableObject)
        {
            if (scriptableObject == null) return;
            var methodInfo = scriptableObject.GetType().GetMethod("Initialize");
            if (methodInfo != null) methodInfo.Invoke(scriptableObject, null);
        }

        /// <summary>
        /// Makes a deep copy of a ScriptableObject list by instantiating copies of the 
        /// list elements.
        /// </summary>
        /// <param name="original">List to clone.</param>
        /// <returns>A second list containing new instances of the list elements.</returns>
        public static List<T> CloneList<T>(List<T> original) where T : ScriptableObject
        {
            var copy = new List<T>();
            if (original != null)
            {
                for (int i = 0; i < original.Count; i++)
                {
                    if (original[i] is T)
                    {
                        copy.Add(ScriptableObject.Instantiate(original[i]) as T);
                    }
                    else
                    {
                        if (Debug.isDebugBuild) Debug.LogWarning("CloneList<" + typeof(T).Name + ">: Element " + i + " is null.");
                        copy.Add(null);
                    }
                }
            }
            return copy;
        }

#if UNITY_WINRT
        /// <summary>
        /// Given a lambda expression that calls a method, returns the method info.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        /// <summary>
        /// Given a lambda expression that calls a method, returns the method info.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        /// <summary>
        /// Given a lambda expression that calls a method, returns the method info.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        /// <summary>
        /// Given a lambda expression that calls a method, returns the method info.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(LambdaExpression expression)
        {
            MethodCallExpression outermostExpression = expression.Body as MethodCallExpression;

            if (outermostExpression == null)
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            return outermostExpression.Method;
        }
#endif

    }

}