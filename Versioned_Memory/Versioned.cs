using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
 
    internal class Versioned
    {
        internal void Release(Segment release) { }
        internal void Collapse(Revision main, Segment parent) { }
        internal void Merge(Revision main, Revision joinRev, Segment join) { }
    }



    internal class Versioned<T> : Versioned
    {
        internal Versioned()
        {
            versions = new SortedDictionary<int, T>();
        }

        internal SortedDictionary<int, T> versions;

        internal T Get() { return Get(Revision.currentRevision.Value); }
        internal void Set(T v) { Set(Revision.currentRevision.Value, v); }

        protected T Get(Revision r)
        {
            Segment s = r.current;
            T value;
            while (versions.TryGetValue(s.version, out value) == false)
            {
                s = s.parent;
            }
            return value;
        }

        protected void Set(Revision r, T value)
        {
            T v;
            if (versions.TryGetValue(r.current.version, out v) == false)
            {
                r.current.written.Add(this);
            }
            versions[r.current.version] = value;
        }

        internal void Release(Segment release)
        {
            versions.Remove(release.version);
        }

        internal void Collapse(Revision main, Segment parent)
        {
            T v;
            if (versions.TryGetValue(main.current.version, out v) == false)
            {
                Set(main, versions[parent.version]);
            }
            versions.Remove(parent.version);
        }

        internal void Merge(Revision main, Revision joinRef, Segment join)
        {
            Segment s = joinRef.current;
            T v;
            while (versions.TryGetValue(s.version, out v) == false)
            {
                s = s.parent;
            }
            if (s == join)
            {
                Set(main, versions[join.version]);
            }
        }
    }
}
