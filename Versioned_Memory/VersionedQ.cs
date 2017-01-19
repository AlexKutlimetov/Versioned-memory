using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
    class VersionedQ<T> : Queue<T>
    {
        Versioned<Queue<T>> versQ;

        List<Revision> revisions;

        Revision mainRev;

        public VersionedQ() : base()
        {
            versQ = new Versioned<Queue<T>>();

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            Queue<T> test_Q = new Queue<T>();
            versQ.Set(test_Q);
        }

        public VersionedQ(IEnumerable<T> collection) : base(collection)
        {
            versQ = new Versioned<Queue<T>>();
            
            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            Queue<T> test_Q = new Queue<T>(collection);
            versQ.Set(test_Q);
        }

        public VersionedQ(int capacity) : base(capacity)
        {
            versQ = new Versioned<Queue<T>>();

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));
            

            Queue<T> test_Q = new Queue<T>(capacity);
            versQ.Set(test_Q);
        }

        public void Clear()
        {
            Revision newRev = mainRev.Fork( delegate() { base.Clear(); });
            mainRev.Join(newRev);
        }

        //public T Dequeue()
        //{

        //    T value = new ;

        //    return value;
        //}

        //public void Enqueue(T item)
        //{

        //}

    }
}
