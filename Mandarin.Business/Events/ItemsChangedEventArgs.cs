using System;
using System.Collections.Generic;

namespace Mandarin.Business.Events
{
    public enum ItemsChangedEventType
    {
        None,
        Added,
        Removed,
        Both,
        Moved
    }

    public class ItemsChangedEventArgs<T> : EventArgs
    {
        public ItemsChangedEventType Type;
        public IEnumerable<T> Added;
        public IEnumerable<T> Removed;
        
        public static ItemsChangedEventArgs<T> BuildAddedEvents(IEnumerable<T> added)
        {
            return new ItemsChangedEventArgs<T>
                {
                    Type = ItemsChangedEventType.Added,
                    Added = added
                };
        }

        public static ItemsChangedEventArgs<T> BuildRemovedEvents(IEnumerable<T> removed)
        {
            return new ItemsChangedEventArgs<T>
            {
                Type = ItemsChangedEventType.Removed,
                Removed = removed
            };
        }
    }
}
