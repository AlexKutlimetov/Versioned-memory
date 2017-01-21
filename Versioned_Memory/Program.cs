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

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            Task t1 = Task.Factory.StartNew(()=>
            {
               
                for (int i = 0; i < 10; i++)
                {
                    
                    int ts = rnd.Next(0, 500);
                    Thread.Sleep(ts);
                    q.Enqueue(i);
                    Console.WriteLine("Add");
                }
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

                    Thread.Sleep(1256);
                    q.Clear();
                    Console.WriteLine("Delete");
               // }
            });

            t1.Wait();
            t2.Wait();

            Console.WriteLine("Total elements in queue: "+ q.Elements().Count);
            foreach (int el in q.Elements())
            {
                Console.WriteLine(el.ToString());
            }

        }


    }

    
}
