using MvLib.Reactive;
using MvLib.Testing.Inspector;
using System;
using System.Reactive.Linq;
using UnityEngine;

namespace Assets._Project.Test
{
    [ExecuteInEditMode]
    internal class Test : MonoBehaviour
    {
        [SerializeField] private ReactiveProperty<int> reactiveProperty22 = new ReactiveProperty<int>();
        [SerializeField] private ReactiveList<int> reactiveList33 = new ReactiveList<int>();
        [SerializeField] private int value = 0;

        private string huy;
        private string huy22 = "22";
        protected int protInt = 1;

        private IDisposable disposable;
         

        private void OnEnable()
        {
            disposable = reactiveProperty22.Subscribe((e) =>
            {
                Debug.Log(e);
            });
        }

        private void OnDisable()
        {
            disposable.Dispose();
        }
    }


}
