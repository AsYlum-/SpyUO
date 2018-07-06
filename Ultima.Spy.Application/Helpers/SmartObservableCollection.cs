using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Ultima.Spy.Application
{
    /// <summary>
    /// Describes smart collection.
    /// </summary>
    /// <typeparam name="T">Type of objects in collection.</typeparam>
    public class SmartObservableCollection<T> : ObservableCollection<T>
    {
        #region Methods
        /// <summary>
        /// Add range of items to collection.
        /// </summary>
        /// <param name="items">Items to add.</param>
        public void AddRange(IEnumerable<T> items)
        {
            CheckReentrancy();

            items.ToList().ForEach(x => Items.Add(x));

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        #endregion
    }
}
