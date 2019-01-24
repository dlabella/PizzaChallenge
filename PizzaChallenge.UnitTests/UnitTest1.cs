using System;
using Xunit;

namespace PizzaChallenge.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void PlotPizza()
        {
            PizzaOrder pizzaDefinition=new PizzaOrder();
            //pizzaDefinition.Parse("Samples/a_example.in").Wait();
            pizzaDefinition.ReadRequest("Samples/b_small.in").Wait();
            var requirements = pizzaDefinition.Requirements;
            var pizza = pizzaDefinition.Pizza;
            var pizzaPlotter = new PizzaPlotter();
            var result = pizzaPlotter.Plot(pizzaDefinition);
        }

        [Fact]
        public void PlotPizzaSlices()
        {
            PizzaOrder pizzaOrder = new PizzaOrder();
            pizzaOrder.ReadRequest("Samples/b_small.in").Wait();
            var pizzaSlicer = new PizzaSlicer(pizzaOrder);
            var result = pizzaSlicer.Slice();
            var plottedResult = new PizzaPlotter().Plot(result);
            pizzaOrder.WriteResult(result, "Results/b_small.out").Wait();
        }
    }
}
