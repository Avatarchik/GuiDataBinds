using System;

namespace Assets.Polarhigh.GuiDataBindings.Collections
{
    public static class ObservableCollectionExtensions
    {
        public static void RemoveAll<T>(this ObservableCollection<T> collection, Predicate<T> predicate)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
                if (predicate(collection[i]))
                    collection.RemoveAt(i);
        }
    }
}