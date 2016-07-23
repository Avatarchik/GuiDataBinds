using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Assets.Polarhigh.GuiDataBindings.BindableGuiElements;
using Assets.Polarhigh.GuiDataBindings.TypeExtensions;
using UnityEngine;

namespace Assets.Polarhigh.GuiDataBindings
{
    // TODO кэширование отражений
    /// <summary>
    /// Компонент управляет дочерними компонентами IGuiBindable.
    /// Через этот класс осуществляется привязка источника данных к элементам графического интерфейса.
    /// 
    /// Каждому полю из источника данных может соответствовать несколько IGuiBindable компонентов.
    /// </summary>
    public class GuiComponent : MonoBehaviour
    {
        // Список компонентов графического интерфейса поддерживающих привязку, которыми владеет данный компонент.
        private List<IGuiBindable> _bindableGuiComponents;

        // Карта соответствий IGuiBindable.GetBindName() => свойство в источнике данных.
        private Dictionary<string, PropertyInfo> _bindMap;

        private INotifyPropertyChanged _dataSource;
        public INotifyPropertyChanged DataSource
        {
            get { return _dataSource; }
        }

        private readonly List<GuiEventData> _registeredEvents = new List<GuiEventData>();
        public List<GuiEventData> RegisteredEvents
        {
            get { return _registeredEvents; }
        }

        /// <summary>
        /// Связывает источник данных с элементами графического интерфейса, которыми владеет данный компонент.
        /// </summary>
        public void BindDataSource(INotifyPropertyChanged dataSource)
        {
            BindDataSource(dataSource, dataSource.GetType().GetProperties().ToDictionary(info => info.Name));
        }

        /// <summary>
        /// Связывает источник данных с элементами графического интерфейса, которыми владеет данный компонент.
        /// </summary>
        /// <param name="dataSource">Источник данных.</param>
        /// <param name="bindMap">Карта привязки (IGuiBindable.GetBindName() => свойство)</param>
        public void BindDataSource(INotifyPropertyChanged dataSource, Dictionary<string, PropertyInfo> bindMap)
        {
            UnbindCurrentDataSource();

            _dataSource = dataSource;
            _bindMap = bindMap;

            dataSource.PropertyChanged += BindedData_PropertyChanged;

            foreach (var bindItem in bindMap)
                BindedData_PropertyChanged(dataSource, new PropertyChangedEventArgs(bindItem.Key));

            // создание колбэков, которые будут обновлять источник
            foreach (var guiBindable in _bindableGuiComponents)
            {
                var data = guiBindable;
                guiBindable.SetDataUpdateAction(o => bindMap[data.GetBindName()].SetValue(dataSource, o, null)); // TODO возможна утечка ?
            }
        }

        public void UnbindCurrentDataSource()
        {
            if (_dataSource != null)
                _dataSource.PropertyChanged -= BindedData_PropertyChanged;
        }



        /// <summary>
        /// Подписывает элемент графического интерфейса на получение событий, которые для него доступны.
        /// 
        /// Например, у стандартного грфического элемента Toggle есть событие onValueChanged, которое принимает делегат UnityAction&lt;bool&gt;
        /// Пусть у источника данных есть свойство MyToggle, которое связано с соответствующим GuiToggleBind. Тогда подписка на событие может выглядеть так:
        /// AddEventListener("MyToggle", "onValueChanged", (proxydata, args) => Debug.Log("MyToggle changed: " + (bool)args[0]));
        /// </summary>
        public GuiEventData AddEventListener(string bindName, string eventName, UniversalGuiEventDelegate callback)
        {
            GuiEventData guiEventData = null;

            IGuiBindable bindableData = _bindableGuiComponents.SingleOrDefault(data => data.GetBindName() == bindName);
            if (bindableData != null)
            {
                object unityEvent =
                    bindableData.GetGuiComponent()
                        .GetType()
                        .GetProperyOrFieldValue(bindableData.GetGuiComponent(), eventName);

                if (unityEvent != null)
                {
                    Type unityEventType = unityEvent.GetType();

                    UnityActionProxy proxyHandler = UnityActionProxy.Create(unityEventType, callback);
                    guiEventData = new GuiEventData(bindName, eventName, bindableData, proxyHandler);
                    RegisteredEvents.Add(guiEventData);


                    unityEventType.GetMethod("AddListener")
                        .Invoke(unityEvent, new object[] { proxyHandler.UnityActionDelegate });
                }
            }

            return guiEventData;
        }

        public void RemoveEventListener(string bindName, string eventName, UniversalGuiEventDelegate callback, bool throwIfEventNotRegistered = false)
        {
            var ev = _registeredEvents.SingleOrDefault(
                data =>
                    data.BindName == bindName &&
                    data.EventName == eventName &&
                    data.UnityActionProxy.UniversalGuiEventHandler == callback);

            if (ev != null)
            {
                CallRemoveListenerOnUnityEvent(ev);
                _registeredEvents.Remove(ev);
            }
            else if (throwIfEventNotRegistered)
                throw new Exception("Event " + eventName + " not registered"); // TODO custom type exception
        }

        public void RemoveAllEventsListeners()
        {
            foreach (var guiEventData in _registeredEvents)
                CallRemoveListenerOnUnityEvent(guiEventData);

            _registeredEvents.Clear();
        }

        /// <summary>
        /// Отписывает от всех событий, снимает биндинг данных и уничтожает RootObject, после вызова функции объект не должен использоваться
        /// </summary>
        public void SaftyDestroy()
        {
            UnbindCurrentDataSource();
            RemoveAllEventsListeners();
            Destroy(gameObject);
        }

        ///// <summary>
        ///// Если графическое компоненты были добавлены во время исполнения, необходимо вручную просканировать
        ///// </summary>
        //public void RescanElements()
        //{
        //    throw new NotImplementedException(); // TODO
        //}

        private void Awake()
        {
            _bindableGuiComponents = GetComponentsInChildren<IGuiBindable>(true).ToList();
        }

        private void CallRemoveListenerOnUnityEvent(GuiEventData guiEventData)
        {
            object unityEvent =
                guiEventData.GuiBindableData.GetGuiComponent()
                    .GetType()
                    .GetProperyOrFieldValue(guiEventData.GuiBindableData.GetGuiComponent(), guiEventData.EventName);

            if (unityEvent != null)
                unityEvent.GetType().GetMethod("RemoveListener").Invoke(unityEvent, new object[] { guiEventData.UnityActionProxy.UnityActionDelegate });
            // TODO else?
        }

        private void BindedData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var components =
                from comp in _bindableGuiComponents
                where comp.GetBindName() == e.PropertyName
                select comp;

            foreach (var item in components)
                item.DataChanged(_bindMap[e.PropertyName].GetValue(sender, null));
        }

        public class GuiEventData
        {
            public string BindName { get; private set; }
            public string EventName { get; private set; }
            public IGuiBindable GuiBindableData { get; private set; }
            public UnityActionProxy UnityActionProxy { get; private set; }

            public GuiEventData(string bindName, string eventName, IGuiBindable guiBindableData, UnityActionProxy unityActionProxy)
            {
                BindName = bindName;
                EventName = eventName;
                GuiBindableData = guiBindableData;
                UnityActionProxy = unityActionProxy;
            }
        }
    }
}