using AtomEngine.Components;
using UnityEngine;
using System;

namespace AtomEngine
{
    [Serializable]
    public class Diction
    {
        [field: SerializeField] public string Type { get; set; } = string.Empty;
        [SerializeReference] public AtomEngineComponent Component;
    }
}
