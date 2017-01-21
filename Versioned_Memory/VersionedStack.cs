using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
    class VersionedStack<T> : Versioned<T>
    {
        private Stack<T> myStack;

        List<Revision> revisions;

        Revision mainRev;

        internal SortedDictionary<int, Stack<T>> versions;

        public VersionedStack() : base()
        {
            myStack = new Stack<T>();

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, Stack<T>>();

            Set(myStack);
        }

        public VersionedStack(IEnumerable<T> collection)
        {

            myStack = new Stack<T>(collection);
            
            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, Stack<T>>();
            Set(myStack);
        }

        public VersionedStack(int capacity)
        {

            myStack = new Stack<T>(capacity);

            Segment parent = new Segment(null);
            mainRev = new Revision(parent, new Segment(parent));

            this.versions = new SortedDictionary<int, Stack<T>>();
            Set(myStack);
        }

        public void Clear()
        {
            Revision newRev = mainRev.Fork(delegate() { myStack.Clear(); Set(myStack); });
            newRev.Join(mainRev);
        }

        internal void Set(Stack<T> v) { Set(Revision.currentRevision.Value, v); } //добавляет стек в текущую ревизию

        protected void Set(Revision r, Stack<T> value)
        {
            Stack<T> v;
            if (versions.TryGetValue(r.current.version, out v) == false)
            {
                r.current.written.Add(this);
            }
            versions[r.current.version] = value;
        }

        internal Stack<T> Get() { return Get(Revision.currentRevision.Value); } //возвращает стек текущей ревизии

        protected Stack<T> Get(Revision r) 
        {
            Segment s = r.current;
            Stack<T> value;
            while (versions.TryGetValue(s.version, out value) == false)
            {
                s = s.parent;
            }
            return value;
        }

        public Stack<T> Elements() //dвозвращает весь стек
        {
            return myStack;
        }


        public void Push(T item) //добавляет элемент в конец
        {
            Revision newRev = mainRev.Fork(delegate() { myStack.Push(item); Set(myStack); });
            mainRev.Join(newRev);
        }

        public bool Pop(out T value) //берет последний элемент очереди и удаляет
        {
            T v = default(T);
            bool rval = true;
            Revision newRev = mainRev.Fork(delegate()
            {
                rval = tryGet(out v);
                if (rval)
                {
                    Set(myStack);
                }
            });

            if (rval)
            {
                mainRev.Join(newRev);
            }

            value = v;
            return rval;
        }

        private bool tryGet(out T value) //пытаемся всять последний элемент
        {
            if (myStack.Count == 0)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = myStack.Pop();
                return true;
            }
        }

        public bool Peek(out T value) //смотрит первый элемент очереди
        {
            return tryPeek(out value);
        }

        private bool tryPeek(out T value) //пытается посмотреть верхний элемент стека
        {
            if (myStack.Count == 0)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = myStack.Peek();
                return true;
            }
        }

        public T[] ToArray() //копирует стек в новый массив
        {
            T[] array = null;

            Revision newRev = mainRev.Fork(delegate() { array = myStack.ToArray(); });
            mainRev.Join(newRev);

            return array;
        }

        public void CopyTo( T[] array, Int32 index) //копирует стек в массив начиная с определенного индекса массива
        {
            Revision newRev = mainRev.Fork(delegate() { myStack.CopyTo(array, index); });
            mainRev.Join(newRev);
        }

        public bool Contains( T item)
        {
            bool iscontain = false;

            Revision newRev = mainRev.Fork(delegate() { iscontain = myStack.Contains(item); });
            mainRev.Join(newRev);

            return iscontain;
        }

        public Stack<T>.Enumerator GetEnumerator()
        {
            Stack<T>.Enumerator Enum = default(Stack<T>.Enumerator);

            Revision newRev = mainRev.Fork(delegate() { Enum = myStack.GetEnumerator(); });
            mainRev.Join(newRev);

            return Enum;
        }

        public void TrimExcess()
        {
            Revision newRev = mainRev.Fork(delegate() { myStack.TrimExcess(); });
            mainRev.Join(newRev);
        }

        internal void Merge(Revision main, Revision joinRef, Segment join) //нахдим последнее изменение переменной и записываем его
        {
            Segment s = joinRef.current;
            Stack<T> v;
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
