using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;


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

            Random rnd = new Random((int)Stopwatch.GetTimestamp());
            Task t1 = Task.Factory.StartNew(()=>
            {
               
                for (int i = 0; i < 10; i++)
                {
                    
                    int ts = rnd.Next(100, 200);
                    Thread.Sleep(ts);
                    q.Enqueue(i);
                }
            });

            Task t2 = Task.Factory.StartNew(() =>
            {

                for (int i = 1; i < 11; i++)
                {
                    int ts = rnd.Next(10, 150);
                    Thread.Sleep(ts);
                    q.Enqueue(i * 10);
                }
            });

            t1.Wait();
            t2.Wait();

            Console.WriteLine("Total elements in queue: "+ q.Elements().Count);
            foreach (int el in q.Elements())
            {
                Console.WriteLine(el.ToString());
            }

            //Console.Read();

            //...................
        }


    }*/

    class Program
    {
        static void Main(string[] args)
        {
            VersionedStack<int> q = new VersionedStack<int>();

            Random rnd = new Random((int)Stopwatch.GetTimestamp());
            Task t1 = Task.Factory.StartNew(() =>
            {

                for (int i = 0; i < 10; i++)
                {

                    int ts = rnd.Next(100, 200);
                    Thread.Sleep(ts);
                    q.Push(i);
                    Console.WriteLine("{0} add 1", i);
                }
            });

            Task t2 = Task.Factory.StartNew(() =>
            {

                for (int i = 1; i < 11; i++)
                {
                    int ts = rnd.Next(10, 150);
                    Thread.Sleep(ts);
                    q.Push(i * 10);
                    Console.WriteLine("{0} add 2", i*10);
                }
            });

            t1.Wait();
            t2.Wait();

            Console.WriteLine("Total elements in queue: " + q.Elements().Count);
            foreach (int el in q.Elements())
            {
                Console.WriteLine(el.ToString());
            }
        }
    }

    
}
