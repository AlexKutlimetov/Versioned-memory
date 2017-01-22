using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace Versioned_Memory
{
    //class Program
    //{

    //    static void Main(string[] args)
    //    {

    //        VersionedQ<int> q = new VersionedQ<int>();

    //        //q.Enqueue(1);
    //        //q.Enqueue(2);
    //        //q.Enqueue(3);

    //        //foreach (int el in q)
    //        //{
    //        //    Console.WriteLine(el.ToString());
    //        //}

    //        //q.Clear();

    //        Console.WriteLine(q.Count.ToString());

    //        List<int> l = new List<int>();
    //        l.Add(1);
    //        l.Add(2);
    //        l.Add(3);
    //        l.Add(4);
    //        l.Add(5);
    //        l.Add(6);
    //        l.Add(7);
    //        l.Add(8);
    //        l.Add(9);
    //        l.Add(10);

    //        int j = 0;
    //        int k = 100;

    //        for (int i = 0; i < 10; i++)
    //        {
    //            Thread t1 = new Thread(new ParameterizedThreadStart(add_elem));
    //            Thread t2 = new Thread(() => { q.Enqueue(k); });

    //            t1.Start();
    //            t2.Start();

                

    //            j++;
    //            k++;
    //            Random rnd = new Random();
    //            Thread.Sleep(rnd.Next(0, 100));
    //        }

    //        foreach (int el in q)
    //        {
    //            Console.WriteLine(el.ToString());
    //        }

    //    }

    //    public static void add_elem(object q, object elem)
    //    {
    //        q.Enqueue(elem);
    //        Random rnd = new Random();
    //        Thread.Sleep(rnd.Next(0, 100));
    //    }
    //}

    /*
    class Program
    {
        static void Main(string[] args)
        {

            VersionedQ<int> q = new VersionedQ<int>();
<<<<<<< HEAD

            Random rnd = new Random((int)Stopwatch.GetTimestamp());
            Task t1 = Task.Factory.StartNew(()=>
=======
            VersionedHashSet<int> h = new VersionedHashSet<int>();
            HashSet<int> h1 = new HashSet<int>();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            Task t1 = Task.Factory.StartNew(() =>
>>>>>>> 6893d3ee9033ede67c411abe5ad6a90c19e63b79
            {

                for (int i = 0; i < 10; i++)
                {
<<<<<<< HEAD
                    
                    int ts = rnd.Next(100, 200);
                    Thread.Sleep(ts);
                    q.Enqueue(i);
=======

                    int ts = rnd.Next(0, 500);
                    Thread.Sleep(ts);
                    h.Add(i);
                    Console.WriteLine("Add1");

>>>>>>> 6893d3ee9033ede67c411abe5ad6a90c19e63b79
                }
                Thread.Sleep(1256);
                h1.Add(1);
                h.ExceptWith(h1);
                Console.WriteLine("Delete1");
                //Thread.Sleep(1256);
            });

            Task t2 = Task.Factory.StartNew(() =>
            {

<<<<<<< HEAD
                for (int i = 1; i < 11; i++)
                {
                    int ts = rnd.Next(10, 150);
                    Thread.Sleep(ts);
                    q.Enqueue(i * 10);
                }
            });
=======
                //for (int i = 1; i < 11; i++)
                //{
                //int ts = rnd.Next(0, 1000);
                //Thread.Sleep(ts);
                //q.Enqueue(i * 10);
                //int res;
                //q.Dequeue(out res);
>>>>>>> 6893d3ee9033ede67c411abe5ad6a90c19e63b79

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

<<<<<<< HEAD
            //Console.Read();

            //...................
        }
=======
            if (t1.Status == TaskStatus.RanToCompletion && t2.Status == TaskStatus.RanToCompletion)
                {
>>>>>>> 6893d3ee9033ede67c411abe5ad6a90c19e63b79

                    Console.WriteLine("Total elements in queue: " + h.Elements().Count);
                    foreach (int el in h.Elements())
                    {
                        Console.WriteLine(el.ToString());
                    }
    
                }
          
        }

    }*/

    class Program
    {
        static void Main(string[] args)
        {
            VersionedQ<int> q = new VersionedQ<int>();

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            Task t1 = Task.Factory.StartNew(() =>
            {

                for (int i = 0; i < 10; i++)
                {

                    int ts = rnd.Next(100, 200);
                    Thread.Sleep(ts);
                    q.Enqueue(i);
                    Console.WriteLine("{0} add 1", i);
                }
            });

            Task t2 = Task.Factory.StartNew(() =>
            {

                for (int i = 1; i < 11; i++)
                {
                    int ts = rnd.Next(10, 150);
                    Thread.Sleep(ts);
                    //q.Enqueue(i * 10);
                    int a;
                    q.Dequeue(out a);
                   // Console.WriteLine("{0} add 2", i * 10);
                    Console.WriteLine("Delete");
                }

                //Thread.Sleep(700);
                //q.Clear();
                //Console.WriteLine("Clear");

            });

            t1.Wait();
            t2.Wait();

            Console.WriteLine("Total elements in queue: " + q.Elements().Count);
            foreach (int el in q.Elements())
            {
                Console.WriteLine(el.ToString()); //test
            }
            //Stack<int> numbers = new Stack<int>();
            //numbers.Push(1);
            //numbers.Push(2);
            //foreach (var n in numbers.ToArray())
            //{
            //    Console.WriteLine(n.ToString());
            //}
        }
    }

    
}
