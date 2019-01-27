using System;
using Xunit;

namespace PizzaChallenge.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void SolveExampleRequest()
        {
            PizzaOrder pizzaOrder = new PizzaOrder();
            pizzaOrder.ReadRequest("Samples/a_example.in").Wait();
            var pizzaSlicer = new PizzaSlicer(pizzaOrder);
            var result = pizzaSlicer.Slice();
            var plottedResult = new PizzaPlotter().Plot(result);
            pizzaOrder.WriteResult(result, "Results/a_example.out").Wait();
        }

        [Fact]
        public void SolveSmallRequest()
        {
            PizzaOrder pizzaOrder = new PizzaOrder();
            pizzaOrder.ReadRequest("Samples/b_small.in").Wait();
            var pizzaSlicer = new PizzaSlicer(pizzaOrder);
            var result = pizzaSlicer.Slice();
            var plottedResult = new PizzaPlotter().Plot(result);
            pizzaOrder.WriteResult(result, "Results/b_small.out").Wait();
        }

        [Fact]
        public void SolveMediumRequest()
        {
            PizzaOrder pizzaOrder = new PizzaOrder();
            pizzaOrder.ReadRequest("Samples/c_medium.in").Wait();
            var pizzaSlicer = new PizzaSlicer(pizzaOrder);
            var result = pizzaSlicer.Slice();
            var plottedResult = new PizzaPlotter().Plot(result);
            pizzaOrder.WriteResult(result, "Results/c_mediu,.out").Wait();
        }
    }
}
