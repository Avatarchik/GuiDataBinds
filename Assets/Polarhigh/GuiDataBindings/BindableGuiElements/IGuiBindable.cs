using System;

namespace Assets.Polarhigh.GuiDataBindings.BindableGuiElements
{
    /// <summary>
    /// Каждый элемент интерфейса к которому может быть создана привязка должен реализовывать этот интерфейс;
    /// </summary>
    public interface IGuiBindable
    {
        /// <summary>
        /// Имя по которому будет осуществляться привязка
        /// </summary>
        string GetBindName();

        /// <summary>
        /// Метод должен вернуть gui компонент unity (Text, InputField, etc)
        /// Через этот объект осуществляется подписка на события (Click, ValueChanged и пр.) 
        /// </summary>
        object GetGuiComponent();

        /// <summary>
        /// Метод вызывается когда данные в коллекции/объекте обновились
        /// </summary>
        void DataChanged(object data);

        /// <summary>
        /// Компонент должен вызывать updateAction, когда данные меняются из gui
        /// </summary>
        void SetDataUpdateAction(Action<object> updateAction);
    }
}
