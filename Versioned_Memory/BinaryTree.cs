using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class BinaryTree<T> : ICollection<T>
    {
        protected class Node<TValue>
        {
            public TValue Value
            {
                get;
                set;
            }
            public Node<TValue> Left
            {
                get;
                set;
            }
            public Node<TValue> Right
            {
                get;
                set;
            }

            public Node(TValue value)
            {
                Value = value;
            }
        }

        protected Node<T> root;
        protected IComparer<T> comparer;

        internal BinaryTree()
            : this(Comparer<T>.Default)
        {
        }
        internal BinaryTree(IComparer<T> defaultComparer)
        {
            if (defaultComparer == null)
                throw new ArgumentNullException("Default comparer is null");
            comparer = defaultComparer;
        }
        internal BinaryTree(IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {

        }
        internal BinaryTree(IEnumerable<T> collection, IComparer<T> defaultComparer)
            : this(defaultComparer)
        {
            AddRange(collection);
        }

        internal T minValue()
        {
            var current = root;
            while (current.Left != null)
                current = current.Left;
            return current.Value;
        }

        internal T maxValue()
        {
            var current = root;
            while (current.Right != null)
                current = current.Right;
            return current.Value;
        }

        internal void AddRange(IEnumerable<T> collection)
        {
            foreach (var value in collection)
                Add(value);
        }

        /*
          Вертикальный обратный обход:
          из текущего узла «спускаемся» до самого нижнего левого узла, 
          добавляя в стек все посещенные узлы. Обрабатываем верхний узел из стека. 
          Если в текущем узле имеется правое поддерево, начинаем следующую итерацию с правого узла. 
          Если правого узла нет, пропускаем шаг со спуском и переходим к обработке следующего узла из стека.
          */
        internal IEnumerable<T> Inorder()
        {
            if (root == null)
                yield break;

            var stack = new Stack<Node<T>>();
            var node = root;

            while (stack.Count > 0 || node != null)
            {
                if (node == null)
                {
                    node = stack.Pop();
                    yield return node.Value;
                    node = node.Right;
                }
                else
                {
                    stack.Push(node);
                    node = node.Left;
                }
            }
        }

         /*
         Вертикальный прямой обход:
         обрабатываем текущий узел, при наличии правого поддерева добавляем его в стек для последующей обработки. 
         Переходим к узлу левого поддерева. Если левого узла нет, переходим к верхнему узлу из стека.
         */
        internal IEnumerable<T> Preorder()
        {
            if (root == null)
                yield break;

            var stack = new Stack<Node<T>>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                var node = stack.Pop();
                yield return node.Value;
                if (node.Right != null)
                    stack.Push(node.Right);
                if (node.Left != null)
                    stack.Push(node.Left);
            }
        }

        /*
        Вертикальный концевой обход:
        Здесь ситуация усложняется – в отличие от обратного обхода, помимо порядка спуска 
        нужно знать обработано ли уже правое поддерево. Одним из вариантов решения является 
        внесение в каждый экземпляр узла флага, который бы хранил соответствующую информацию (не рассматривается). 
        Другим подходом является «кодирование» непосредственно в очередности стека — при спуске, 
        если у очередного узла позже нужно будет обработать еще правое поддерево, в стек вносится 
        последовательность «родитель, правый узел, родитель». Таким образом, при обработке узлов 
        из стека мы сможем определить, нужно ли нам обрабатывать правое поддерево.
        */
        internal IEnumerable<T> Postorder()
        {
            if (root == null)
                yield break;

            var stack = new Stack<Node<T>>();
            var node = root;

            while (stack.Count > 0 || node != null)
            {
                if (node == null)
                {
                    node = stack.Pop();
                    if (stack.Count > 0 && node.Right == stack.Peek())
                    {
                        stack.Pop();
                        stack.Push(node);
                        node = node.Right;
                    }
                    else
                    {
                        yield return node.Value;
                        node = null;
                    }
                }
                else
                {
                    if (node.Right != null)
                        stack.Push(node.Right);
                    stack.Push(node);
                    node = node.Left;
                }
            }
        }
        /*
        Горизонтальный обход:
        обрабатываем первый в очереди узел, при наличии дочерних узлов 
        заносим их в конец очереди. Переходим к следующей итерации.
        */
        internal IEnumerable<T> Levelorder()
        {
            if (root == null)
                yield break;

            var queue = new Queue<Node<T>>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                yield return node.Value;
                if (node.Left != null)
                    queue.Enqueue(node.Left);
                if (node.Right != null)
                    queue.Enqueue(node.Right);
            }
        }

        #region ICollection<T> Members
        public int Count
        {
            get;
            protected set;
        }

        public virtual void Add(T item)
        {
            var node = new Node<T>(item);

            if (root == null)
                root = node;
            else
            {
                Node<T> current = root, parent = null;

                while (current != null)
                {
                    parent = current;
                    if (comparer.Compare(item, current.Value) < 0)
                        current = current.Left;
                    else
                        current = current.Right;
                }

                if (comparer.Compare(item, parent.Value) < 0)
                    parent.Left = node;
                else
                    parent.Right = node;
            }
            ++Count;
        }

        public virtual bool Remove(T item)
        {
            if (root == null)
                return false;

            Node<T> current = root, parent = null;

            int result;
            do
            {
                result = comparer.Compare(item, current.Value);
                if (result < 0)
                {
                    parent = current;
                    current = current.Left;
                }
                else if (result > 0)
                {
                    parent = current;
                    current = current.Right;
                }
                if (current == null)
                    return false;
            }
            while (result != 0);

            if (current.Right == null)
            {
                if (current == root)
                    root = current.Left;
                else
                {
                    result = comparer.Compare(current.Value, parent.Value);
                    if (result < 0)
                        parent.Left = current.Left;
                    else
                        parent.Right = current.Left;
                }
            }
            else if (current.Right.Left == null)
            {
                current.Right.Left = current.Left;
                if (current == root)
                    root = current.Right;
                else
                {
                    result = comparer.Compare(current.Value, parent.Value);
                    if (result < 0)
                        parent.Left = current.Right;
                    else
                        parent.Right = current.Right;
                }
            }
            else
            {
                Node<T> min = current.Right.Left, prev = current.Right;
                while (min.Left != null)
                {
                    prev = min;
                    min = min.Left;
                }
                prev.Left = min.Right;
                min.Left = current.Left;
                min.Right = current.Right;

                if (current == root)
                    root = min;
                else
                {
                    result = comparer.Compare(current.Value, parent.Value);
                    if (result < 0)
                        parent.Left = min;
                    else
                        parent.Right = min;
                }
            }
            --Count;
            return true;
        }

        public void Clear()
        {
            root = null;
            Count = 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var value in this)
                array[arrayIndex++] = value;
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Contains(T item)
        {
            var current = root;
            while (current != null)
            {
                var result = comparer.Compare(item, current.Value);
                if (result == 0)
                    return true;
                if (result < 0)
                    current = current.Left;
                else
                    current = current.Right;
            }
            return false;
        }
        #endregion

        #region IEnumerable<T> Members
        public IEnumerator<T> GetEnumerator()
        {
            return Inorder().GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
