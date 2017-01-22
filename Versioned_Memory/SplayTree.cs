using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
    class SplayTree<T> : ICollection<T>
    {
        protected class Node<TKey>
        {
            public Node<TKey> Parent;
            public Node<TKey> Left;
            public Node<TKey> Right;
            public TKey Key;
        }

        private Node<T> root;
        protected IComparer<T> comparer;

        internal SplayTree()
            : this(Comparer<T>.Default)
        {
        }
        internal SplayTree(IComparer<T> defaultComparer)
        {
            if (defaultComparer == null)
                throw new ArgumentNullException("Default comparer is null");
            comparer = defaultComparer;
        }
        internal SplayTree(IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {

        }
        internal SplayTree(IEnumerable<T> collection, IComparer<T> defaultComparer)
            : this(defaultComparer)
        {
            AddRange(collection);
        }
        internal void AddRange(IEnumerable<T> collection)
        {
            foreach (var value in collection)
                Add(value);
        }
        
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
                    yield return node.Key;
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
                yield return node.Key;
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
                        yield return node.Key;
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
                yield return node.Key;
                if (node.Left != null)
                    queue.Enqueue(node.Left);
                if (node.Right != null)
                    queue.Enqueue(node.Right);
            }
        }


        private void RightRotate(Node<T> x)
        {
            Node<T> b = x.Right;
            Node<T> p = x.Parent;
            Node<T> g = p.Parent;

            if( g != null && g.Left  == p)
                g.Left = x;

            if (g != null && g.Right == p)
                g.Right = x;

            p.Parent = x;
            p.Left = b;

            if (b != null)
                b.Parent = p;

            x.Parent = g;
            x.Right = p;
        }

        private void LeftRotate(Node<T> x)
        {
            Node<T> b = x.Left;
            Node<T> p = x.Parent;
            Node<T> g = p.Parent;

            if (g != null && g.Left == p)
                g.Left = x;

            if (g != null && g.Right == p)
                g.Right = x;

            p.Parent = x;
            p.Right = b;

            if (b != null)
                b.Parent = p;

            x.Parent = g;
            x.Left = p;
        }

        private void Splay(Node<T> x)
        {
            while( x.Parent != null)
            {
                Node<T> p = x.Parent;
                Node<T> g = p.Parent;

                if (g == null)
                {
                    if (x == p.Left)
                        RightRotate(x);
                    else
                        LeftRotate(x);
                }

                else if (x == p.Left && p == g.Left)
                {
                    RightRotate(p);
                    RightRotate(x);
                }

                else if (x == p.Right && p == g.Right)
                {
                    LeftRotate(p);
                    LeftRotate(x);
                }

                else if (x == p.Left && p == g.Right)
                {
                    RightRotate(x);
                    LeftRotate(x);
                }

                else if(x == p.Right && p == g.Left)
                {
                    LeftRotate(x);
                    RightRotate(x);
                }
            }
        }

        private Node<T> Find(Node<T> x, T key)
        {
            if (x == null)
                return null;

            int cr = comparer.Compare(key, x.Key);

            if (cr == 0)
                return x;
            else if (cr < 0)
                return Find(x.Left, key);
            else
                return Find(x.Right, key);
        }

        private int Size(Node<T> node)
        {
            if (node == null)
                return 0;
            else
                return Size(node.Left) + Size(node.Right) + 1;
        }

        private Node<T> Min(Node<T> x)
        {
            Node<T> r = x.Left;

            if (r == null)
                return x;
            else
                return Min(r);
        }
        
        private Node<T> Max(Node<T> x)
        {
            Node<T> r = x.Right;

            if (r == null)
                return x;
            else
                return Max(r);
        }

        private Node<T> Merge(Node<T> x, Node<T> y)
        {
            if (x == null && y == null)
                return null;

            if( x == null)
            {
                y.Parent = null;
                return y;
            }

            if( y == null)
            {
                x.Parent = null;
                return x;
            }

            Node<T> MaxX = Max(x);

            Splay(MaxX);
            MaxX.Right = y;
            y.Parent = MaxX;

            return MaxX;
        }

        // ICollection<T>
        public int Count
        {
            get
            {
                return Size(root);
            }

            protected set
            { 
            }
        }

        public bool Contains(T item)
        {
            return Find(root, item) != null;
        }

        public virtual void Add(T Item)
        {
            if (Contains(Item))
                return;

            Node<T> x = root;
            Node<T> p = null;

            while (x!= null)
            {
                p = x;

                int cr = comparer.Compare(Item, x.Key);

                if (cr < 0)
                    x = x.Left;
                else
                    x = x.Right;
            }

            x = new Node<T>();
            x.Parent = p;
            x.Key = Item;

            if (p != null)
            {
                int cr = comparer.Compare(Item, p.Key);

                if (cr < 0)
                    p.Left = x;
                else
                    p.Right = x;
            }

            Splay(x);
            root = x;
        }

        public virtual bool Remove(T item)
        {
            Node<T> x = Find(root, item);

            if (x == null)
                return false;

            Splay(x);
            root = Merge(x.Left, x.Right);

            return true;
        }

        public virtual void Clear()
        {
            root = null;
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
