﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
    class VersionedQ<T> : Versioned<Queue<T>>
    {
        private Queue<T> myQ;

        Revision mainRev;

        public VersionedQ()
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
            Revision newRev = mainRev.Fork(delegate() { myQ.Clear(); Set(myQ); });
            newRev.Join(mainRev);
        }

        public Queue<T> Elements() //возвращает всю очередь
        {
            return myQ;
        }

        public void Enqueue(T item) //добавляет элемент в конец
        {
            Revision newRev = mainRev.Fork(delegate() { myQ.Enqueue(item); Set(myQ);});
            mainRev.Join(newRev);
        }

        public bool Dequeue(out T value) //берет первый элемент очереди и удаляет
        {
            T v = default(T);
            bool rval = true;
            Revision newRev = mainRev.Fork( delegate()
            {
                rval = tryGet(out v);
                if (rval)
                {
                    Set(myQ);
                }
            });

            mainRev.Join(newRev);

            value = v;
            return rval;
        }

        private bool tryGet(out T value) //пытаемся взять элемент
        {
            if (myQ.Count == 0)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = myQ.Dequeue();
                return true;
            }
        }

        public bool Peek(out T value) //смотрит первый элемент очереди
        {
            return tryPeek(out value);
        }

        private bool tryPeek(out T value) //пытается посмотреть первый элемент очереди
        {
            if (myQ.Count == 0)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = myQ.Peek();
                return true;
            }
        }

        internal void Merge(Revision main, Revision joinRef, Segment join) //нахдим последнее изменение переменной и записываем его
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

        public T[] ToArray()
        {
            T[] rval = null;
            Revision newRev = mainRev.Fork(delegate() { rval = myQ.ToArray(); });
            mainRev.Join(newRev);
            return rval;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Revision newRev = mainRev.Fork(delegate() { myQ.CopyTo(array, arrayIndex); });
            mainRev.Join(newRev);
        }
    }
}
