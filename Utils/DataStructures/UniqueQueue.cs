namespace Utils.DataStructures
{
    public class UniqueQueue<T>
    {
        private LinkedList<T>                    m_queue      = new LinkedList<T>();
        private Dictionary<T, LinkedListNode<T>> m_dictionary = new Dictionary<T, LinkedListNode<T>>();
        private object                           m_lock       = new object();

        public bool Any()
        {
            lock (m_lock)
            {
                return m_queue.Any();
            }
        }


        public bool TryPeek(out T item)
        {
            lock (m_lock)
            {
                if (Any())
                {
                    item = m_queue.First();
                    return true;
                }

                item = default;
                return false;
            }
        }

        public bool TryEnqueue(T item)
        {
            lock (m_lock)
            {
                if (m_dictionary.ContainsKey(item))
                {
                    return false;
                }

                LinkedListNode<T> node = m_queue.AddLast(item);
                m_dictionary.Add(item, node);
                return true;
            }
        }

        public bool TryDequeue(out T item)
        {
            lock (m_lock)
            {
                if (Any())
                {
                    item = m_queue.First();
                    m_queue.RemoveFirst();
                    m_dictionary.Remove(item);
                    return true;
                }

                item = default;
                return false;
            }
        }

        public bool TryRemove(T item)
        {
            lock (m_lock)
            {
                if (m_dictionary.TryGetValue(item, out LinkedListNode<T>? node))
                {
                    m_queue.Remove(node);
                    m_dictionary.Remove(item);
                    return true;
                }

                return false;
            }
        }

    }
}
