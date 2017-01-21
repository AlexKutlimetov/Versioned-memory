using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Versioned_Memory
{
    class VersionedHashSet<T> : Versioned<HashSet<T>>
    {
        HashSet<T> mySet;

        Revision mainRev;

        // internal SortedDictionary<int, HashSet<T>> versions;

        public VersionedHashSet()
        {
            mySet = new HashSet<T>();

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, HashSet<T>>();

            Set(mySet);
        }

        public VersionedHashSet(IEnumerable<T> collection)
        {
            mySet = new HashSet<T>(collection);

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, HashSet<T>>();

            Set(mySet);
        }

        public VersionedHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            mySet = new HashSet<T>(collection, comparer);

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, HashSet<T>>();

            Set(mySet);
        }

        public VersionedHashSet(IEqualityComparer<T> comparer)
        {
            mySet = new HashSet<T>(comparer);

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, HashSet<T>>();

            Set(mySet);
        }

        //internal void Set(HashSet<T> v) { Set(Revision.currentRevision.Value, v); }

        //protected void Set(Revision r, HashSet<T> value)
        //{
        //    HashSet<T> v;
        //    if (versions.TryGetValue(r.current.version, out v) == false)
        //    {
        //        r.current.written.Add(this);
        //    }
        //    versions[r.current.version] = value;
        //}

        //internal HashSet<T> Get() { return Get(Revision.currentRevision.Value); }

        //protected HashSet<T> Get(Revision r)
        //{
        //    Segment s = r.current;
        //    HashSet<T> value;
        //    while (versions.TryGetValue(s.version, out value) == false)
        //    {
        //        s = s.parent;
        //    }
        //    return value;
        //}

        public HashSet<T> Elements()
        {
            return mySet;
        }

        public bool Add(T item)
        {
            bool rval = false;
            Revision newRev = mainRev.Fork(delegate() { rval = mySet.Add(item); Set(mySet); });
            mainRev.Join(newRev);
            return rval;
        }

        public void Clear()
        {
            Revision newRev = mainRev.Fork(delegate() { mySet.Clear(); Set(mySet); });
            newRev.Join(mainRev);
        }

        public bool Contains(T item)
        {
            bool rval = false;
            Revision newRev = mainRev.Fork(delegate() { rval = mySet.Contains(item); });
            newRev.Join(mainRev);
            return rval;
        }

        public void CopyTo(T[] array)
        {
            Revision newRev = mainRev.Fork(delegate() { mySet.CopyTo(array); });
            mainRev.Join(newRev);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Revision newRev = mainRev.Fork(delegate() { mySet.CopyTo(array, arrayIndex); });
            mainRev.Join(newRev);
        }

        public void CopyTo(T[] array, int begin, int end)
        {
            Revision newRev = mainRev.Fork(delegate() { mySet.CopyTo(array, begin, end); });
            mainRev.Join(newRev);
        }

        public bool Equals(Object obj)
        {
            bool rval = false;
            Revision newRev = mainRev.Fork(delegate() { rval = mySet.Equals(obj); });
            mainRev.Join(newRev);
            return rval;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            Revision newRev = mainRev.Fork(delegate() { mySet.ExceptWith(other); Set(mySet); });
            mainRev.Join(newRev);
        }

        public Type GetType()
        {
            return mySet.GetType();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            Revision newRev = mainRev.Fork(delegate() { mySet.IntersectWith(other); Set(mySet); });
            mainRev.Join(newRev);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            bool rval = false;
            Revision newRev = mainRev.Fork(delegate() { rval = mySet.Overlaps(other); });
            mainRev.Join(newRev);
            return rval;
        }

        public bool Remove(T item)
        {
            bool rval = false;
            Revision newRev = mainRev.Fork(delegate() { rval = mySet.Remove(item); Set(mySet); });
            mainRev.Join(newRev);
            return rval;
        }

        public int RemoveWhere(Predicate<T> match)
        {
            int num = 0;
            Revision newRev = mainRev.Fork(delegate() { num = mySet.RemoveWhere(match); Set(mySet); });
            mainRev.Join(newRev);
            return num;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            bool rval = false;
            Revision newRev = mainRev.Fork(delegate() { rval = mySet.SetEquals(other); });
            mainRev.Join(newRev);
            return rval;
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            Revision newRev = mainRev.Fork(delegate() { mySet.SymmetricExceptWith(other); Set(mySet); });
            mainRev.Join(newRev);
        }

        public string ToString()
        {
            return mySet.ToString();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            Revision newRev = mainRev.Fork(delegate() { mySet.UnionWith(other); Set(mySet); });
            mainRev.Join(newRev);
        }

        internal void Merge(Revision main, Revision joinRef, Segment join)
        {
            HashSet<T> A, B, C, D;
            A = new HashSet<T>(GetSeg(joinRef.root));
            B = new HashSet<T>(GetSeg(joinRef.current));
            D = new HashSet<T>(B);
            C = new HashSet<T>(GetSeg(main.current));
            D.ExceptWith(A);
            A.ExceptWith(B);
            C.ExceptWith(A);
            C.UnionWith(D);
            Set(main, C);

        }

        protected HashSet<T> GetSeg(Segment cur)
        {
            Segment s = cur;
            HashSet<T> value;
            while (versions.TryGetValue(s.version, out value) == false)
            {
                s = s.parent;
            }
            return value;
        }
    }
}
