using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioned_Memory
{
    class VersionedStack<T> : Versioned<Stack<T>>
    {
        private Stack<T> myStack;

        Revision mainRev;

        public VersionedStack()
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

        public void CopyTo(T[] array, Int32 index) //копирует стек в массив начиная с определенного индекса массива
        {
            Revision newRev = mainRev.Fork(delegate() { myStack.CopyTo(array, index); });
            mainRev.Join(newRev);
        }

        public bool Contains(T item)
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