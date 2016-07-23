using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Polarhigh.GuiDataBindings.Collections
{
    public interface IObservableCollection<T> : ICollection<T>, INotifyCollectionChanged { }

    public interface INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    public delegate void NotifyCollectionChangedEventHandler(Object sender, NotifyCollectionChangedEventArgs e);

    public enum NotifyCollectionChangedAction
    {
        Add,        // Один или несколько элементов были добавлены к коллекции.
        Move,       // Один или несколько элементов были перемещены внутри коллекции.
        Remove,     // Один или несколько элементов были удалены из коллекции.
        Replace,    // Один или несколько элементов были заменены в коллекции.
        Reset       // Содержимое коллекции было существенно изменено.
    }

    public class NotifyCollectionChangedEventArgs
    {
        /// <summary>
        /// Получает действие, вызвавшее событие. 
        /// </summary>
        public NotifyCollectionChangedAction Action { get; private set; }

        /// <summary>
        /// Возвращает список новых элементов, участвующих в изменении.
        /// </summary>
        public IList NewItems { get; private set; }

        /// <summary>
        /// Получает индекс, при котором произошло изменение.
        /// </summary>
        public int NewStartingIndex { get; private set; }

        /// <summary>
        /// Получает список элементов, на которые повлияло действие Replace, Remove или Move.
        /// </summary>
        public IList OldItems { get; private set; }

        /// <summary>
        /// Получает индекс, при котором произошло действие Move, Remove или Replace.
        /// </summary>
        public int OldStartingIndex { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр NotifyCollectionChangedEventArgs, описывающий изменение Reset.
        /// </summary>
        /// <param name="action">Действие, вызвавшее событие. Должно быть установлено значение Reset. </param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            if(action != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("Invalid action for this constructor");

            Action = action;
        }

        /// <summary>
        /// Инициализирует новый экземпляр NotifyCollectionChangedEventArgs, описывающий изменение для одного элемента.
        /// </summary>
        /// <param name="action">Действие, вызвавшее событие. Возможными значениями являются: Reset, Add или Remove. </param>
        /// <param name="changedItem">Элемент, на которое повлияло изменение.</param>
        /// <param name="index">Индекс, указывающий, где произошло изменение.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, Object changedItem, int index)
        {
            if (action == NotifyCollectionChangedAction.Move || action == NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("Invalid action for this constructor");

            Action = action;

            if (action == NotifyCollectionChangedAction.Remove)
            {
                OldItems = new List<Object>(1);
                OldItems.Add(changedItem);

                OldStartingIndex = index;
            }
            else
            {
                NewItems = new List<Object>(1);
                NewItems.Add(changedItem);

                NewStartingIndex = index;
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр NotifyCollectionChangedEventArgs, описывающий изменение Replace для одного элемента.
        /// </summary>
        /// <param name="action">Действие, вызвавшее событие. Возможным значением может быть Replace. </param>
        /// <param name="newItem">Новый элемент, заменяющий исходный элемент.</param>
        /// <param name="oldItem">Исходный элемент, который был заменен.</param>
        /// <param name="index">Индекс заменяемого элемента.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, Object newItem, Object oldItem, int index)
        {
            if(action != NotifyCollectionChangedAction.Replace)
                throw new ArgumentException("Invalid action for this constructor");

            Action = action;
            NewItems = new List<Object>(1);
            NewItems.Add(newItem);
            OldItems = new List<Object>(1);
            OldItems.Add(oldItem);
            OldStartingIndex = index;
        }
    }
}