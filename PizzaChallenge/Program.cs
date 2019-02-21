using PizzaChallenge.Entities;
using PizzaChallenge.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaChallenge
{
    class Program
    {
        private static readonly string[] challenges = { "a_example" , "b_small", "c_medium", "d_big" };
        
        static void Main(string[] args)
        {
            foreach(var challenge in challenges)
            {
                Console.WriteLine($"Running {challenge} challenge");
                RunChallenge(challenge);
            }

            Console.WriteLine("Challenge completed!");
        }

        private static void RunChallenge(string challengeFile)
        {
            Console.WriteLine("Press any key to end ...");
            var cts = new CancellationTokenSource();

            var task = Task.Run(() =>{
                PizzaOrder pizzaOrder = new PizzaOrder();
                pizzaOrder.ReadRequest($"Samples/{challengeFile}.in").Wait();
                var validator = new ResultValidator(pizzaOrder.Requirements);
                var result = new PizzaSlicer(pizzaOrder).Slice(cts);
                Console.WriteLine("Pizza sliced!!!");
                if (validator.Validate(result))
                {
                    pizzaOrder.WriteResult(result, $"Results/{challengeFile}.out").Wait();
                    Console.WriteLine("Pizza slice completed");
                }
                else
                {
                    Console.WriteLine("Ups! something went wrong sliced pizza is not valid!");
                }
            });
            
            Console.ReadKey();
            cts.Cancel();
            task.Wait();
        }
    }
}
