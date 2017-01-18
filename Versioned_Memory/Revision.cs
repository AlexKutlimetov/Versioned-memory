using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Versioned_Memory
{
    class Revision
    {
        internal Segment root;
        internal Segment current;
        Task task;
        //ThreadLocal<Revision> currentRevision;
        [ThreadStatic] internal static Revision currentRevision;

        Revision(Segment root, Segment current)
        {
            this.root = root;
            this.current = current;
        }

        public Revision Fork(Action action)
        {
            Revision r;
            r = new Revision(current, new Segment(current));
            current.Release();
            current = new Segment(current);
            task = Task.Factory.StartNew(delegate()
            {
                Revision previous = currentRevision;
                currentRevision = r;
                try { action(); }
                finally { currentRevision = previous; }

            }
            );

            return r;

        }

        public void Join(Revision join)
        {
            try
            {
                join.task.Wait();
                Segment s = join.current;
                while (s != join.root)
                {
                    foreach (Versioned v in s.written)
                    {
                        v.Merge(this, join, s);
                    }
                    s = s.parent;
                }
            }
            finally
            {
                join.current.Release();
                this.current.Collapse(this);
            }
        }

    }
}
