using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine.Events;

namespace Assets.Polarhigh.GuiDataBindings
{
    public delegate void UniversalGuiEventDelegate(UnityActionProxy.ProxyData proxyData, object[] args);
    
    /// <summary>
    /// Класс предназначен для создания прокси делегатов UnityEvent.
    /// </summary>
    public class UnityActionProxy
    {
        public class ProxyData
        {
            /// <summary>
            /// Индекс элемента массива в коллекции источника, для которого данные событие было вызвано.
            /// </summary>
            public int Index;
        }

        /// <summary>
        /// Специфичные данные прокси.
        /// </summary>
        public readonly ProxyData Data = new ProxyData();

        /// <summary>
        /// Прокси делегат, который должен быть передан методу AddListener типа UnityEvent.
        /// Тип UnityEvent должен соответствовать типу указанному при создании класса UnityActionProxy.
        /// </summary>
        public Delegate UnityActionDelegate { get; private set; }

        /// <summary>
        /// Универсальный делегат, который будет вызван при срабатывании события UnityEvent.
        /// </summary>
        public UniversalGuiEventDelegate UniversalGuiEventHandler { get; private set; }

        private UnityActionProxy()
        { }

        
        /// <param name="unityEventType">Любой тип UnityEvent или производный от UnityEvent</param>
        /// <param name="universalGuiEventHandler">Универсальный делегат для событий</param>
        public static UnityActionProxy Create(Type unityEventType, UniversalGuiEventDelegate universalGuiEventHandler)
        {
            // список параметров делегата, который принимает AddListener
            ParameterInfo[] callbackParameters =
                unityEventType.GetMethod("AddListener").GetParameters()[0]
                    .ParameterType.GetMethod("Invoke").GetParameters();

            // список типов параметров делегата
            Type[] parametersTypes = (from arg in callbackParameters
                                      select arg.ParameterType).ToArray();

            UnityActionProxy proxyHandler = new UnityActionProxy { UniversalGuiEventHandler = universalGuiEventHandler };

            // Далее необходимо собрать делегат для конвертирования параметров
            ParameterExpression[] args = new ParameterExpression[parametersTypes.Length]; // аргументы, соответствующие аргументам UnityAction<>
            Expression[] convertedArgs = new Expression[parametersTypes.Length]; // конвертированные аргументы в object
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = Expression.Parameter(parametersTypes[i], "arg" + i);
                convertedArgs[i] = Expression.Convert(args[i], typeof(object));
            }

            Expression[] handlerEventArgs = new Expression[2]; // список параметров для передачи в универсальный делегат
            handlerEventArgs[0] = Expression.Constant(proxyHandler.Data);
            handlerEventArgs[1] = Expression.NewArrayInit(typeof(object), convertedArgs);

            Expression callHandlerExpression;
            if (universalGuiEventHandler.Target != null)
                callHandlerExpression = Expression.Call(Expression.Constant(universalGuiEventHandler.Target), universalGuiEventHandler.Method, handlerEventArgs);
            else
                callHandlerExpression = Expression.Call(universalGuiEventHandler.Method, handlerEventArgs);

            // делегат, который принимает параметры нужных типов, конвертирует их в object и вызывает универсальный делегат
            Delegate typesConverter = Expression.Lambda(callHandlerExpression, args).Compile();


            // конструирование делегата UnityAction<> с телом typesConverter
            Type unityEventDelegateType = parametersTypes.Length > 0
                ? typeof(UnityAction<>).MakeGenericType(parametersTypes)
                : typeof(UnityAction);

            proxyHandler.UnityActionDelegate = Delegate.CreateDelegate(unityEventDelegateType, typesConverter.Target, typesConverter.Method);

            return proxyHandler;
        }
    }
}