using AtomEngine.Components;
using System;

namespace AtomEngine
{
    public static class AtomObjectExtensions
    {
        public static T AddComponent<T>(this AtomObject atomObject) where T : AtomEngineComponent, new()
        {
            T component = new T();
            atomObject.AddComponent<T>(component);
            return component;
        }

        public static T AddComponent<T>(this AtomObject atomObject, params object[] parameters) where T : AtomEngineComponent
        {
            T component = (T)Activator.CreateInstance(typeof(T), parameters);
            atomObject.AddComponent<T>(component);
            return component;
        }

    }
}
