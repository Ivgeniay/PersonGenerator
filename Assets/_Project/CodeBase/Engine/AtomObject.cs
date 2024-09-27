using System.Collections.Generic;
using AtomEngine.Components; 
using UnityEngine;
using System.Linq;
using System;

namespace AtomEngine
{
    [Serializable]
    public abstract class AtomObject 
    {
        [SerializeField] private List<Diction> componentsStorage = new List<Diction>();
        [SerializeReference] protected AtomEngineTransform transform;

        public readonly string Name;

        protected System.Random random = new System.Random();
        public AtomEngineTransform Transform => transform;

        public AtomObject(string name)
        {
            this.Name = name;
            random = new System.Random();
        }

        public T AddComponent<T>(T component) where T : AtomEngineComponent
        {
            string typeStr = component.GetType().FullName;
            componentsStorage.Add(new Diction { Type = typeStr, Component = component });
            return component;
        }

        public T GetAssignableComponent<T>() where T : AtomEngineComponent
        {
            var result = componentsStorage
                .FirstOrDefault(e => typeof(T).IsAssignableFrom(e.Component.GetType()));
            return result == null ? null : (T)result.Component; 
        }

        public T GetComponent<T>() where T : AtomEngineComponent
        {
            string typeStr = typeof(T).FullName;
            var result = componentsStorage
                .FirstOrDefault(e => e.Type == typeStr);
            return result == null ? null : (T)result.Component;
        }

        public List<T> GetComponents<T>() where T : AtomEngineComponent
        {
            string typeStr = typeof(T).FullName;
            return componentsStorage
                    .Where(e => e.Component is T)
                    .Select(e => (T)e.Component)
                    .ToList();
        }

        public void RemovedComponents<T>() where T : AtomEngineComponent
        {
            string typeStr = typeof(T).FullName;
            componentsStorage.RemoveAll(x => x.Type == typeStr);
        }
    }
}
