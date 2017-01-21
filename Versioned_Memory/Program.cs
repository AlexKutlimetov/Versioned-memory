using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;


namespace Versioned_Memory
{
    class Program
    {
        static void Main(string[] args)
        {

            VersionedQ<int> q = new VersionedQ<int>();
            VersionedHashSet<int> h = new VersionedHashSet<int>();
            HashSet<int> h1 = new HashSet<int>();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            Task t1 = Task.Factory.StartNew(() =>
            {

                for (int i = 0; i < 10; i++)
                {

                    int ts = rnd.Next(0, 500);
                    Thread.Sleep(ts);
                    h.Add(i);
                    Console.WriteLine("Add1");

                }
                Thread.Sleep(1256);
                h1.Add(1);
                h.ExceptWith(h1);
                Console.WriteLine("Delete1");
                //Thread.Sleep(1256);
            });

            Task t2 = Task.Factory.StartNew(() =>
            {

                //for (int i = 1; i < 11; i++)
                //{
                //int ts = rnd.Next(0, 1000);
                //Thread.Sleep(ts);
                //q.Enqueue(i * 10);
                //int res;
                //q.Dequeue(out res);

                for (int i = 0; i < 10; i++)
                {

                    int ts = rnd.Next(0, 500);
                    Thread.Sleep(ts);
                    h.Add(10 + i);
                    Console.WriteLine("Add2");
                }
                //Thread.Sleep(1256);
                //h.Clear();
                //Console.WriteLine("Delete");
                // }
            });

            //t1.Wait();
            //t2.Wait();
            var continuation = Task.WhenAll(t1, t2);
            try
            {
                continuation.Wait();
            }
            catch (AggregateException)
            {
            }

            if (t1.Status == TaskStatus.RanToCompletion && t2.Status == TaskStatus.RanToCompletion)
                {

                    Console.WriteLine("Total elements in queue: " + h.Elements().Count);
                    foreach (int el in h.Elements())
                    {
                        Console.WriteLine(el.ToString());
                    }
    
                }
          
        }

    }

    
}
