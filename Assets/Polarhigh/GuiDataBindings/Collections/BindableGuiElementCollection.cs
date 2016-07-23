using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Assets.Polarhigh.GuiDataBindings.BindableGuiElements;
using UnityEngine;

namespace Assets.Polarhigh.GuiDataBindings.Collections
{

    /// <summary>
    /// Коллекция компонентов GuiComponent.
    /// 
    /// Позволяет регистрировать события для каждого компонента каждого элемента коллекции.
    /// События автоматически регистрируются для новых элементов и отписываются из удаленных элементов.
    /// </summary>
    public class BindableGuiElementCollection : Collection<GuiComponent>
    {
        private GameObject _rootObject;
        private GameObject _elementTemplate;
        private List<EventData> _registeredEvents;
        private Dictionary<string, PropertyInfo> _bindMap;

        public BindableGuiElementCollection(GameObject elementTemplate, GameObject parent)
        {
            Constructor(elementTemplate, parent);
        }

        public BindableGuiElementCollection(GameObject elementTemplate, GameObject parent, IList<GuiComponent> list) : base(list)
        {
            Constructor(elementTemplate, parent);

            foreach (var item in list)
                Add(item);
        }

        public void BindDataSource<T>(IObservableCollection<T> dataSource) where T : INotifyPropertyChanged
        {
            _bindMap = CreatePropertiesMap(_elementTemplate.GetComponentsInChildren<IGuiBindable>(true), typeof(T));

            dataSource.CollectionChanged += BindedCollection_CollectionChanged;

            if (dataSource.Count > 0)
            {
                BindedCollection_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                int i = 0;
                foreach (var item in dataSource)
                {
                    BindedCollection_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, i++));
                }
            }
        }

        public void AddEventListener(string bindName, string eventName, UniversalGuiEventDelegate callback)
        {
            _registeredEvents.Add(new EventData(bindName, eventName, callback));

            int i = 0;
            foreach (var guiComponent in this)
            {
                GuiComponent.GuiEventData guiEventData = guiComponent.AddEventListener(bindName, eventName, callback);
                if (guiEventData == null)
                    throw new Exception("eventName or bindName not found"); // TODO custom type exception

                guiEventData.UnityActionProxy.Data.Index = i++;
            }
        }

        public void RemoveEventListener(string bindName, string eventName, UniversalGuiEventDelegate callback, bool throwIfEventNotRegistered = false)
        {
            EventData ev =
                _registeredEvents.SingleOrDefault(
                    data => data.BindName == bindName &&
                    data.EventName == eventName &&
                    data.Callback == callback);

            if (ev != null)
            {
                foreach (var guiComponent in this)
                    guiComponent.RemoveEventListener(bindName, eventName, callback, throwIfEventNotRegistered);

                _registeredEvents.Remove(ev);
            }
            else if (throwIfEventNotRegistered)
                throw new Exception("Event " + eventName + " isn\'t registered for collection"); // TODO custom type exception
        }

        public void RemoveAllEventsListeners()
        {
            foreach(var guiComponent in this)
                guiComponent.RemoveAllEventsListeners();

            _registeredEvents.Clear();
        }

        protected override void InsertItem(int index, GuiComponent item)
        {
            var rootObject = item.gameObject;
            RectTransform rt = rootObject.transform as RectTransform;
            rt.SetParent(_rootObject.transform);
            rt.localPosition = Vector3.zero;
            rt.SetSiblingIndex(index);

            base.InsertItem(index, item);

            foreach (var ev in _registeredEvents)
                item.AddEventListener(ev.BindName, ev.EventName, ev.Callback);

            UpdateProxyEventsDataInItems();
        }

        protected override void SetItem(int index, GuiComponent item)
        {
            this[index].RemoveAllEventsListeners();

            base.SetItem(index, item);

            foreach (var ev in _registeredEvents)
                item.AddEventListener(ev.BindName, ev.EventName, ev.Callback);

            UpdateProxyEventsDataInItems();
        }

        protected override void RemoveItem(int index)
        {
            this[index].SaftyDestroy();

            base.RemoveItem(index);

            UpdateProxyEventsDataInItems();
        }

        protected override void ClearItems()
        {
            foreach (var item in this)
                item.SaftyDestroy();

            base.ClearItems();
        }

        private void Constructor(GameObject elementTemplate, GameObject parent)
        {
            _elementTemplate = elementTemplate;
            _rootObject = parent;
            _registeredEvents = new List<EventData>();
        }

        private void BindedCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    GuiComponent guiComponent = GameObject.Instantiate(_elementTemplate).GetComponent<GuiComponent>();
                    guiComponent.BindDataSource((INotifyPropertyChanged)e.NewItems[0], _bindMap);

                    Insert(e.NewStartingIndex, guiComponent);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    this[e.NewStartingIndex].BindDataSource((INotifyPropertyChanged)e.NewItems[0], _bindMap);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    break;
            }
        }

        private void UpdateProxyEventsDataInItems()
        {
            int i = 0;
            foreach (var item in this)
            {
                foreach (var evInfo in item.RegisteredEvents)
                    evInfo.UnityActionProxy.Data.Index = i;

                i++;
            }
        }

        /// <summary>
        /// Создает словарь в котором имена возвращаемые IGuiBindable.GetBindName() соответствуют свойствам из типа коллекции
        /// </summary>
        /// <param name="components">Список GUI компонентов, которые есть в каждом элементе GUI (компонты шаблона)</param>
        /// <param name="collectionGenericType">Тип элементов коллекции</param>
        /// <returns></returns>
        private Dictionary<string, PropertyInfo> CreatePropertiesMap(IGuiBindable[] components, Type collectionGenericType)
        {
            return
                components
                    .Join(collectionGenericType.GetProperties(),
                        x => x.GetBindName(),
                        y => y.Name,
                        (x, y) => new KeyValuePair<string, PropertyInfo>(x.GetBindName(), y))
                    .Distinct()
                    .ToDictionary(x => x.Key, x => x.Value);
        }


        protected class EventData
        {
            public string BindName { get; private set; }
            public string EventName { get; private set; }
            public UniversalGuiEventDelegate Callback { get; private set; }

            public EventData(string bindName, string eventName, UniversalGuiEventDelegate callback)
            {
                BindName = bindName;
                EventName = eventName;
                Callback = callback;
            }
        }
    }
}
