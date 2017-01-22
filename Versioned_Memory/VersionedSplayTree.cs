﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
    class VersionedSplayTree<T> : Versioned<SplayTree<T>>
    {
        SplayTree<T> myTree;

        Revision mainRev;

        public VersionedSplayTree()
        {
            myTree = new SplayTree<T>();

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, SplayTree<T>>();

            Set(myTree);
        }

        public VersionedSplayTree(IComparer<T> defaultComparer)
        {
            myTree = new SplayTree<T>(defaultComparer);

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, SplayTree<T>>();

            Set(myTree);
        }

        public VersionedSplayTree(IEnumerable<T> collection, IComparer<T> defaultComparer)
        {
            myTree = new SplayTree<T>(collection, defaultComparer);

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, SplayTree<T>>();

            Set(myTree);
        }

        public VersionedSplayTree(IEnumerable<T> collection)
        {
            myTree = new SplayTree<T>(collection);

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, SplayTree<T>>();

            Set(myTree);
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
                Revision newRev = mainRev.Fork(delegate() { v = myTree.Min(); });
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
                Revision newRev = mainRev.Fork(delegate() { v = myTree.Max(); });
                mainRev.Join(newRev);
                value = v;
                return true;
            }
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
            Revision newRev = mainRev.Fork(delegate() { rval = myTree.Contains(item); });
            mainRev.Join(newRev);
            return rval;
        }

        internal void Merge(Revision main, Revision joinRef, Segment join)
        {
            SplayTree<T> A, B, C;
            A = new SplayTree<T>(GetSeg(joinRef.root));
            B = new SplayTree<T>(GetSeg(joinRef.current));
            C = new SplayTree<T>(GetSeg(main.current));

            T[] a = default(T[]), b = default(T[]), c = default(T[]);

            A.CopyTo(a, 0);
            B.CopyTo(b, 0);
            C.CopyTo(c, 0);

            var z = new T[c.Length + b.Length];
            C.CopyTo(z, 0);
            B.CopyTo(z, c.Length);

            for (int i = 0; i < a.Length; i++)
            {
                int index = Array.IndexOf(z, a[i]);
                if (index > -1)
                {
                    z = z.Where((val, idx) => idx != index).ToArray();
                }
            }

            SplayTree<T> BTree = new SplayTree<T>();

            foreach (var el in z)
            {
                BTree.Add(el);
            }

            Set(main, BTree);
        }

        protected SplayTree<T> GetSeg(Segment cur)
        {
            Segment s = cur;
            SplayTree<T> value;
            while (versions.TryGetValue(s.version, out value) == false)
            {
                s = s.parent;
            }
            return value;
        }

    }
}
