using System;
using Assets.Polarhigh.GuiDataBindings.Converters;
using UnityEngine;

namespace Assets.Polarhigh.GuiDataBindings.BindableGuiElements
{
    public abstract class GuiBindableBase : MonoBehaviour, IGuiBindable
    {
        [SerializeField]
        protected string BindName;

        [SerializeField]
        protected BindWayType BindWay = BindWayType.OneWay;

        [SerializeField]
        protected ValueConverterBase ParameterConverter;

        private Action<object> _updateSourceAction;

        public string GetBindName()
        {
            return BindName;
        }

        public void SetDataUpdateAction(Action<object> updateAction)
        {
            _updateSourceAction = updateAction;
        }

        public void DataChanged(object data)
        {
            if(BindWay != BindWayType.FromGuiToSource)
                DataUpdated(ParameterConverter != null ? ParameterConverter.ConvertFromSourceToGui(data) : data);
        }

        public virtual void DataUpdated(object data) { }
        public abstract object GetGuiComponent();

        /// <summary>
        /// Метод должен вызываться всякий раз при обновлении данных из гуи
        /// если компонент предполагает обновление источника данных из интерфейса (текстовые поля, выпадающие списки, ...)
        /// </summary>
        protected void UpdateSource(object data)
        {
            if (BindWay != BindWayType.OneWay && _updateSourceAction != null)
                _updateSourceAction(ParameterConverter != null ? ParameterConverter.ConvertFromGuiToSource(data) : data);
        }

        protected enum BindWayType
        {
            OneWay,
            TwoWay,
            FromGuiToSource
        }
    }
}