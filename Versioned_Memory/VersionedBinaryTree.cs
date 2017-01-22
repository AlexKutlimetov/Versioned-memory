using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
    class VersionedBinaryTree<T> : Versioned<BinaryTree<T>>
    {
        BinaryTree<T> myTree;

        Revision mainRev;

        public VersionedBinaryTree()
        {
            myTree = new BinaryTree<T>();

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, BinaryTree<T>>();

            Set(myTree);
        }

        public VersionedBinaryTree(IComparer<T> defaultComparer)
        {
            myTree = new BinaryTree<T>(defaultComparer);

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, BinaryTree<T>>();

            Set(myTree);
        }

        public VersionedBinaryTree(IEnumerable<T> collection, IComparer<T> defaultComparer)
        {
            myTree = new BinaryTree<T>(collection, defaultComparer);

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, BinaryTree<T>>();

            Set(myTree);
        }

        public VersionedBinaryTree(IEnumerable<T> collection)
        {
            myTree = new BinaryTree<T>(collection);

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, BinaryTree<T>>();

            Set(myTree);
        }

        public bool GetMinValue(out T value)
        {
            T v = default(T);
            if (myTree.Count == 0)
            {
                value = v;
                return false;
            }
            else
            {
                Revision newRev = mainRev.Fork(delegate() { v = myTree.minValue(); });
                mainRev.Join(newRev);
                value = v;
                return true;
            }
        }

        public bool GetMaxValue(out T value)
        {
            T v = default(T);
            if (myTree.Count == 0)
            {
                value = v;
                return false;
            }
            else
            {
                Revision newRev = mainRev.Fork(delegate() { v = myTree.maxValue(); });
                mainRev.Join(newRev);
                value = v;
                return true;
            }
        }

        public IEnumerable<T> Inorder()
        {
            IEnumerable<T> en = default(IEnumerable<T>);
            Revision newRev = mainRev.Fork(delegate() { en = myTree.Inorder(); });
            mainRev.Join(newRev);
            return en;
        }

        public IEnumerable<T> Preorder()
        {
            IEnumerable<T> en = default(IEnumerable<T>);
            Revision newRev = mainRev.Fork(delegate() { en = myTree.Preorder(); });
            mainRev.Join(newRev);
            return en;
        }

        public IEnumerable<T> Postorder()
        {
            IEnumerable<T> en = default(IEnumerable<T>);
            Revision newRev = mainRev.Fork(delegate() { en = myTree.Postorder(); });
            mainRev.Join(newRev);
            return en;
        }

        public IEnumerable<T> Levelorder()
        {
            IEnumerable<T> en = default(IEnumerable<T>);
            Revision newRev = mainRev.Fork(delegate() { en = myTree.Levelorder(); });
            mainRev.Join(newRev);
            return en;
        }

        public int Count()
        {
            return myTree.Count;
        }

        public void Add(T item)
        {
            Revision newRev = mainRev.Fork(delegate() { myTree.Add(item); Set(myTree); });
            mainRev.Join(newRev);
        }

        public bool Remove(T item)
        {
            bool rval = true;

            Revision newRev = mainRev.Fork(delegate() 
            { 
                rval = myTree.Remove(item);
                if (rval)
                {
                    Set(myTree);
                }
            }
            );

            mainRev.Join(newRev);

            return rval;
        }

        public void Clear()
        {
            Revision newRev = mainRev.Fork(delegate() { myTree.Clear(); Set(myTree); });
            mainRev.Join(newRev);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Revision newRev = mainRev.Fork(delegate() { myTree.CopyTo(array, arrayIndex); });
            mainRev.Join(newRev);
        }

        public bool Contains(T item)
        {
            bool rval = false;
            Revision newRev = mainRev.Fork(delegate() { rval = myTree.Contains(item);});
            mainRev.Join(newRev);
            return rval;
        }

    }
}
