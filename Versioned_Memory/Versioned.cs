using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
    class Versioned
    {
        internal void Release(Segment release);
        internal void Collapse(Revision main, Segment parent);
        internal void Merge(Revision main, Revision joinRev, Segment join);
    }

    public class Versioned<T> : Versioned
    {
        SortedDictionary<int, T> versions;

        public T Get() { return Get(Revision.currentRevision); }
        public void Set(T v) { Set(Revision.currentRevision, v);}

        T Get(Revision r)
        {
            Segment s = r.current;
            T value;
            while (versions.TryGetValue(s.version, out value) == false) 
            {
                s = s.parent;
            }
            return value;
        }

        void Set(Revision r, T value)
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
