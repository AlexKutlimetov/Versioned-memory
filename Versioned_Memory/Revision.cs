﻿using System;
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
        internal static ThreadLocal<Revision> currentRevision;

        internal Revision(Segment root, Segment current)
        {
            this.root = root;
            this.current = current;
            currentRevision = new ThreadLocal<Revision>(() => { return this; });
            currentRevision.Value = this;
        }

        public Revision Fork(Action action)
        {
            Revision r;
            r = new Revision(current, new Segment(current));
            current.Release();
            current = new Segment(current);
            task = Task.Run( () =>
            {
                Revision previous = currentRevision.Value;
                currentRevision.Value = r;
                try { action(); }
                finally { currentRevision.Value = previous; }

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
