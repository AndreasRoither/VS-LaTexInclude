using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LatechInclude.HelperClasses
{
    public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
    where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Collection contructor
        /// </summary>
        public TrulyObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        /// <summary>
        /// Collection constructor
        /// </summary>
        /// <param name="pItems">Item to add</param>
        public TrulyObservableCollection(IEnumerable<T> pItems) : this()
        {
            foreach (var item in pItems)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Sets Propertychanged event for every item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        /// <summary>
        /// NotifyCollectionChangedEvent 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            OnCollectionChanged(args);
        }
    }
}
