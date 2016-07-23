using System;
using System.Collections.ObjectModel;

namespace Assets.Polarhigh.GuiDataBindings.Collections
{
    [Serializable]
    public class ObservableCollection<T> : Collection<T>, IObservableCollection<T>
    {
        private readonly object _objectLock = new object();
        event NotifyCollectionChangedEventHandler CollectionChangedEvent = delegate { };

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                lock(_objectLock)
                {
                    CollectionChangedEvent += value;
                }
            }
            remove
            {
                lock(_objectLock)
                {
                    CollectionChangedEvent -= value;
                }
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            CollectionChangedEvent(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void SetItem(int index, T item)
        {
            T replaced = this[index];
            base.SetItem(index, item);
            CollectionChangedEvent(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, replaced, index));
        }

        protected override void RemoveItem(int index)
        {
            T removed = this[index];
            base.RemoveItem(index);
            CollectionChangedEvent(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, index));
        }

        protected override void ClearItems()
        {
            base.ClearItems();  
            CollectionChangedEvent(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}