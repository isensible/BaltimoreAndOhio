using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a queue of items that are sorted based on individual priorities.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    /// <typeparam name="TPriority">Specifies the type of object representing the priority.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class PriorityQueue<T, TPriority>
    {
        private readonly List<KeyValuePair<T, TPriority>> _heap = new List<KeyValuePair<T, TPriority>>();
        private readonly Dictionary<T, int> _indices = new Dictionary<T, int>();

        private readonly IComparer<TPriority> _comparer;
        private readonly bool _invert;

        public PriorityQueue()
            : this(false)
        {
        }

        public PriorityQueue(bool invert)
            : this(Comparer<TPriority>.Default)
        {
            _invert = invert;
        }

        public PriorityQueue(IComparer<TPriority> comparer)
        {
            _comparer = comparer;
            _heap.Add(default(KeyValuePair<T, TPriority>));
        }

        public void Enqueue(T item, TPriority priority)
        {
            var tail = new KeyValuePair<T, TPriority>(item, priority);
            _heap.Add(tail);

            MoveUp(tail, Count);
        }

        public KeyValuePair<T, TPriority> Dequeue()
        {
            var bound = Count;
            if (bound < 1)
                throw new InvalidOperationException("Queue is empty.");

            var head = _heap[1];
            var tail = _heap[bound];

            _heap.RemoveAt(bound);

            if (bound > 1)
                MoveDown(tail, 1);

            _indices.Remove(head.Key);

            return head;
        }

        public KeyValuePair<T, TPriority> Peek()
        {
            if (Count < 1)
                throw new InvalidOperationException("Queue is empty.");

            return _heap[1];
        }

        public bool TryGetValue(T item, out TPriority priority)
        {
            int index;
            if (_indices.TryGetValue(item, out index))
            {
                priority = _heap[_indices[item]].Value;
                return true;
            }
            priority = default(TPriority);
            return false;
        }

        public TPriority this[T item]
        {
            get
            {
                return _heap[_indices[item]].Value;
            }
            set
            {
                int index;

                if (_indices.TryGetValue(item, out index))
                {
                    int order = _comparer.Compare(value, _heap[index].Value);
                    if (order != 0)
                    {
                        if (_invert)
                            order = ~order;

                        var element = new KeyValuePair<T, TPriority>(item, value);
                        if (order < 0)
                            MoveUp(element, index);
                        else
                            MoveDown(element, index);
                    }
                }
                else
                {
                    var element = new KeyValuePair<T, TPriority>(item, value);
                    _heap.Add(element);

                    MoveUp(element, Count);
                }
            }
        }

        public int Count
        {
            get
            {
                return _heap.Count - 1;
            }
        }

        private void MoveUp(KeyValuePair<T, TPriority> element, int index)
        {
            while (index > 1)
            {
                int parent = index >> 1;

                if (IsPrior(_heap[parent], element))
                    break;

                _heap[index] = _heap[parent];
                _indices[_heap[parent].Key] = index;

                index = parent;
            }

            _heap[index] = element;
            _indices[element.Key] = index;
        }

        private void MoveDown(KeyValuePair<T, TPriority> element, int index)
        {
            int count = _heap.Count;

            while (index << 1 < count)
            {
                int child = index << 1;
                int sibling = child | 1;

                if (sibling < count && IsPrior(_heap[sibling], _heap[child]))
                    child = sibling;

                if (IsPrior(element, _heap[child]))
                    break;

                _heap[index] = _heap[child];
                _indices[_heap[child].Key] = index;

                index = child;
            }

            _heap[index] = element;
            _indices[element.Key] = index;
        }

        private bool IsPrior(KeyValuePair<T, TPriority> element1, KeyValuePair<T, TPriority> element2)
        {
            int order =  _comparer.Compare(element1.Value, element2.Value); 
            if (_invert)
                order = ~order;
            return order < 0;
        }
    }
}
