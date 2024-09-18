using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace AtomEngine
{
    [Serializable]
    public abstract class AtomObject 
    {
        [SerializeField] private List<Diction> componentsStorage = new List<Diction>();
        protected System.Random random = new System.Random();

        public T AddComponent<T>(T component) where T : AtomEngineComponent
        {
            string typeStr = component.GetType().FullName;
            componentsStorage.Add(new Diction { Type = typeStr, Component = component });
            return component;
        }

        public T GetComponent<T>() where T : AtomEngineComponent
        {
            string typeStr = typeof(T).FullName;
            return (T)componentsStorage.FirstOrDefault(e => e.Type == typeStr).Component;
        }

        public void RemovedComponent<T>() where T : AtomEngineComponent
        {
            string typeStr = typeof(T).FullName;
            componentsStorage.RemoveAll(x => x.Type == typeStr);
        }
    }

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

    [Serializable]
    public class Diction
    {
        [field: SerializeField] public string Type { get; set; } = string.Empty;
        [SerializeReference] public AtomEngineComponent Component;
    }
}
