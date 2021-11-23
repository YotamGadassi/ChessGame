using System;
using System.Collections.Generic;

namespace Utils
{
    public class BiDirectionalDictionary<TValue1, TValue2> 
    {
        private TValue1 defaultKey;
        private TValue2 defaultValue;

        private Dictionary<TValue1, TValue2> originalDictionary;
        private Dictionary<TValue2, TValue1> reversedDictionary;

        public BiDirectionalDictionary(TValue1 DefaultKey, TValue2 DefaultValue)
        {
            originalDictionary = new Dictionary<TValue1, TValue2>();
            reversedDictionary = new Dictionary<TValue2, TValue1>();
            
            defaultKey = DefaultKey;
            defaultValue = DefaultValue;
        }

        public void Add(TValue1 value1, TValue2 value2)
        {
            CheckIfValuesExistsInDictionaries(value1, value2);

            originalDictionary[value1] = value2;
            reversedDictionary[value2] = value1;
        }

        private void CheckIfValuesExistsInDictionaries(TValue1 value1, TValue2 value2)
        {
            bool isKeyExistsInOriginalDict = originalDictionary.ContainsKey(value1);
            bool isKeyExistsInReversedDict = reversedDictionary.ContainsKey(value2);

            if (isKeyExistsInOriginalDict || isKeyExistsInReversedDict)
            {
                string err;
                if (isKeyExistsInOriginalDict)
                {
                    err = string.Format("{0} exists in dictionary", value1);
                }
                else
                {
                    err = string.Format("{0} exists in dictionary", value2);
                }

                throw new ArgumentException(err);
            }
        }

        public void Clear()
        {
            originalDictionary.Clear();
            reversedDictionary.Clear();
        }

        public int Count => originalDictionary.Count;

        public bool Contains(TValue1 value)
        {
            return originalDictionary.ContainsKey(value);
        }

        public bool Contains(TValue2 value)
        {
            return reversedDictionary.ContainsKey(value);
        }

        public TValue2 GetValue(TValue1 value)
        {
            return originalDictionary[value];
        }

        public TValue1 GetValue(TValue2 value)
        {
            return reversedDictionary[value];
        }

        public void Remove(TValue1 value)
        {
            originalDictionary.Remove(value);
            RemovePairByValue<TValue2, TValue1>(value, reversedDictionary);
        }

        public void Remove(TValue2 value)
        {
            reversedDictionary.Remove(value);
            RemovePairByValue<TValue1, TValue2>(value, originalDictionary);
        }

        public bool TryGetValue(TValue1 key, out TValue2 value)
        {
            if (originalDictionary.ContainsKey(key))
            {
                value = originalDictionary[key];
                return true;
            }

            value = defaultValue;
            return false;
        }
        public bool TryGetValue(TValue2 key, out TValue1 value)
        {
            if (reversedDictionary.ContainsKey(key))
            {
                value = reversedDictionary[key];
                return true;
            }

            value = defaultKey;
            return false;
        }

        private void RemovePairByValue<TKey, TValue>(TValue value, Dictionary<TKey, TValue> dictionary)
        {
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                if (pair.Value.Equals(value))
                    dictionary.Remove(pair.Key);
            }
        }
    }
}
