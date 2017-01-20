using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
    class VersionedQ<T> : Versioned<T>
    {

        private Queue<T> myQ;

        List<Revision> revisions;

        Revision mainRev;

        internal SortedDictionary<int, Queue<T>> versions;

        public VersionedQ() : base()
        {

            myQ = new Queue<T>();

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, Queue<T>>();

            Set(myQ);
        }

        public VersionedQ(IEnumerable<T> collection)
        {

            myQ = new Queue<T>(collection);
            
            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, Queue<T>>();
            Set(myQ);
        }

        public VersionedQ(int capacity)
        {

            myQ = new Queue<T>(capacity);

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, Queue<T>>();
            Set(myQ);
        }

        public void Clear()
        {
            Revision newRev = mainRev.Fork( delegate() { myQ.Clear(); });
            newRev.Join(mainRev);
        }

        internal void Set(Queue<T> v) { Set(Revision.currentRevision.Value, v); }

        protected void Set(Revision r, Queue<T> value)
        {
            Queue<T> v;
            if (versions.TryGetValue(r.current.version, out v) == false)
            {
                r.current.written.Add(this);
            }
            versions[r.current.version] = value;
        }

        internal Queue<T> Get() { return Get(Revision.currentRevision.Value); }

        protected Queue<T> Get(Revision r)
        {
            Segment s = r.current;
            Queue<T> value;
            while (versions.TryGetValue(s.version, out value) == false)
            {
                s = s.parent;
            }
            return value;
        }

        public Queue<T> Elements()
        {
            return myQ;
        }


        public void Enqueue(T item)
        {
            Revision newRev = mainRev.Fork(delegate() { myQ.Enqueue(item); Set(myQ);});
            mainRev.Join(newRev);
        }

        internal void Merge(Revision main, Revision joinRef, Segment join)
        {
            Segment s = joinRef.current;
            Queue<T> v;
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
