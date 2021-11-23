using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableDictionary() : base() { }

        public ObservableDictionary(IDictionary<TKey, TValue> dict) : base(dict) { }
        
        public ObservableDictionary(int capacity) : base(capacity){ }

        public new TValue this[TKey key]
        {
            get { return base[key]; }
            set
            {
                bool isReplacing = !base.ContainsKey(key);
                
                if (!isReplacing)
                {
                    this.Add(key, value);
                }

                NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Replace;
                KeyValuePair<TKey, TValue> oldPair = new KeyValuePair<TKey, TValue>(key, this[key]);
                base[key] = value;

                KeyValuePair<TKey, TValue> newPair = new KeyValuePair<TKey, TValue>(key, base[key]);

                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action, newPair, oldPair);
                CollectionChanged?.Invoke(this, e);
                return;
            }
        }

        public new void Add(TKey key, TValue value)
        {
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Add;
            base[key] = value;
            KeyValuePair<TKey, TValue> newPair = new KeyValuePair<TKey, TValue>(key, base[key]);
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action, newPair);
            CollectionChanged?.Invoke(this, e);
        }

        public new void Clear()
        {
            base.Clear();
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset;
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action);
            CollectionChanged?.Invoke(this, e);
        }

        public new bool Remove(TKey key)
        {

            bool isKeyExist = base.TryGetValue(key, out TValue value);
            if (isKeyExist)
            {
                base.Remove(key);
                NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Remove;
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action, value);
                CollectionChanged?.Invoke(this, e);
            }

            return isKeyExist;
        }
    }
}
