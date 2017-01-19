using System;
using System.Collections;


namespace Versioned_Memory
{
    class Program
    {
        static void Main(string[] args)
        {

            VersionedQ<int> q = new VersionedQ<int>();

            q.Enqueue(1);
            q.Enqueue(2);
            q.Enqueue(3);

            foreach (int el in q) {
                Console.WriteLine(el.ToString());
            }

        }
    }
}
