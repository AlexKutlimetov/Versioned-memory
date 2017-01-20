using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
    class Segment
    {
        internal Segment parent;
        internal int version;
        int refcount; // кол-во ссылок на меня
        internal List<Versioned> written;
        static int versionCount = 0;

        internal Segment(Segment parent)
        {
            this.parent = parent;
            if (parent != null)
            {
                parent.refcount++;
            }
            written = new List<Versioned>();
            version = versionCount++;
            refcount = 1;
        }

        internal void Release()
        {
            if (--refcount == 0)
            {
                foreach (Versioned v in written)
                {
                    v.Release(this);
                }

                if (parent != null)
                {
                    parent.Release();
                }

            }

        }

        internal void Collapse(Revision main)
        {

            while (parent != main.root && parent.refcount == 1)
            {
                foreach (Versioned v in written)
                {
                    v.Collapse(main, parent);
                }
                parent = parent.parent;
            }
        }
    }
}
