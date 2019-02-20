using System;
using System.Threading;

namespace PizzaChallenge
{
    class Program
    {
        public static CancellationTokenSource cts = new CancellationTokenSource();
        public static AutoResetEvent ars = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            //cts = new CancellationTokenSource();
            //RunExampleChallenge();
            //cts = new CancellationTokenSource();
            //RunSmallChallenge();
            //cts = new CancellationTokenSource();
            //RunMediumChallenge();
            cts = new CancellationTokenSource();
            RunBigChallenge();

            Console.WriteLine("Challenge completed!");
        }
        static void RunExampleChallenge()
        {
            Console.WriteLine("Running SMALL Challenge...");
            var stackSize = 10000000;
            Thread thread = new Thread(new ThreadStart(SolveExample), stackSize);
            thread.Start();
            Console.WriteLine("Press any key to terminate SMALL ...");
            var k = Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("Waiting");
            ars.WaitOne();
            Console.WriteLine("SMALL Challenge completed!");
        }
        static void SolveExample()
        {
            ars.Reset();
            PizzaOrder pizzaOrder = new PizzaOrder();
            pizzaOrder.ReadRequest("Samples/a_example.in").Wait();
            var pizzaSlicer = new PizzaSlicer(pizzaOrder);
            var result = pizzaSlicer.Slice(cts);
            Console.WriteLine("Pizza slice completed");
            pizzaOrder.WriteResult(result, "Results/a_example.out").Wait();
            ars.Set();
        }

        static void RunSmallChallenge()
        {
            Console.WriteLine("Running SMALL Challenge...");
            var stackSize = 10000000;
            Thread thread = new Thread(new ThreadStart(SolveSmall), stackSize);
            thread.Start();
            Console.WriteLine("Press any key to terminate SMALL ...");
            var k = Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("Waiting");
            ars.WaitOne();
            Console.WriteLine("SMALL Challenge completed!");
        }

        static void SolveSmall()
        {
            ars.Reset();
            PizzaOrder pizzaOrder = new PizzaOrder();
            pizzaOrder.ReadRequest("Samples/b_small.in").Wait();
            var pizzaSlicer = new PizzaSlicer(pizzaOrder);
            var result = pizzaSlicer.Slice(cts);
            Console.WriteLine("Pizza slice completed");
            pizzaOrder.WriteResult(result, "Results/b_small.out").Wait();
            ars.Set();
        }

        static void RunMediumChallenge()
        {
            Console.WriteLine("Running MEDIUM Challenge...");
            var stackSize = 10000000;
            Thread thread = new Thread(new ThreadStart(SolveMedium), stackSize);
            thread.Start();
            Console.WriteLine("Press any key to terminate MEDIUM ...");
            var k = Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("Waiting");
            ars.WaitOne();
            Console.WriteLine("MEDIUM Challenge completed!");
        }

        static void SolveMedium()
        {
            ars.Reset();
            PizzaOrder pizzaOrder = new PizzaOrder();
            pizzaOrder.ReadRequest("Samples/c_medium.in").Wait();
            var pizzaSlicer = new PizzaSlicer(pizzaOrder);
            var result = pizzaSlicer.Slice(cts);
            var validator = new ResultValidator(pizzaOrder.Requirements);
            if (validator.Validate(result))
            {
                Console.WriteLine("Pizza slice completed");
                pizzaOrder.WriteResult(result, "Results/c_medium.out").Wait();
            }
            ars.Set();
        }

        static void RunBigChallenge()
        {
            ars.Reset();
            var stackSize = 10000000;
            Thread thread = new Thread(new ThreadStart(SolveBig), stackSize);
            thread.Start();
            Console.WriteLine("Press any key to terminate BIG...");
            var k = Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("Waiting");
            ars.WaitOne();
            Console.WriteLine("BIG Challenge completed!");
        }

        static void SolveBig()
        {
            PizzaOrder pizzaOrder = new PizzaOrder();
            pizzaOrder.ReadRequest("Samples/d_big.in").Wait();
            var pizzaSlicer = new PizzaSlicer(pizzaOrder);
            var result = pizzaSlicer.Slice(cts);
            Console.WriteLine("Pizza slice completed");
            pizzaOrder.WriteResult(result, "Results/d_big.out").Wait();
            ars.Set();
        }
    }
}
