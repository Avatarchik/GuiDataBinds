using System.ComponentModel;
using Assets.Polarhigh.GuiDataBindings.Collections;
using UnityEngine;

namespace Assets.Polarhigh.GuiDataBindings
{
    public class GuiComponentsCollection : MonoBehaviour
    {
        [SerializeField]
        private GameObject _template;

        private BindableGuiElementCollection _bindableGuiElementCollection;

        public BindableGuiElementCollection BindableGuiElementCollection
        {
            get { return _bindableGuiElementCollection; }
        }

        public void BindDataSource<T>(IObservableCollection<T> dataSource) where T : INotifyPropertyChanged
        {
            _bindableGuiElementCollection = new BindableGuiElementCollection(_template, gameObject);
            _bindableGuiElementCollection.BindDataSource(dataSource);
        }

        public void AddEventListener(string bindName, string eventName, UniversalGuiEventDelegate callback)
        {
            _bindableGuiElementCollection.AddEventListener(bindName, eventName, callback);
        }

        public void RemoveEventListener(string bindName, string eventName, UniversalGuiEventDelegate callback, bool throwIfEventNotRegistered = false)
        {
            _bindableGuiElementCollection.RemoveEventListener(bindName, eventName, callback, throwIfEventNotRegistered);
        }   

        public void RemoveAllEventListeners()
        {
            _bindableGuiElementCollection.RemoveAllEventsListeners();
        }
    }
}