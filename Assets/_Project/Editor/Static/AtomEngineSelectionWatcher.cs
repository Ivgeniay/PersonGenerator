using UnityEngine;
using UnityEditor;
using AtomEngine;
using Inspector;
using System;


[InitializeOnLoad]
public static class SelectionWatcher
{
    public static event Action OnConstructElementSelected;
    public static AtomConstructed atomConstructed = null;

    static SelectionWatcher()
    { 
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void OnSelectionChanged()
    { 
        var selectedObjects = Selection.objects;

        if (selectedObjects.Length == 0)
        {
            Miss();
        }
        else
        {
            foreach (var obj in selectedObjects)
            {
                if (obj is GameObject gameObject)
                {
                    AtomConstructed atomConstructed = gameObject.GetComponent<AtomConstructed>();
                    if (atomConstructed != null)
                    {
                        Hit(atomConstructed);
                        return; 
                    }
                }
            }
            Miss();
        }
    }

    public static void Miss()
    {
        AtomConstructorInspector.Instance.OnInspectorClose();
        atomConstructed = null;
        OnConstructElementSelected?.Invoke();
    }

    public static void Hit(AtomConstructed _atomConstructed)
    {
        atomConstructed = _atomConstructed;
        OnConstructElementSelected?.Invoke();
    }
}
