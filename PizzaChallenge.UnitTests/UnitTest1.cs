using System;
using Xunit;

namespace PizzaChallenge.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void PlotPizza()
        {
            PizzaDefinition pizzaDefinition=new PizzaDefinition();
            //pizzaDefinition.Parse("Samples/a_example.in").Wait();
            pizzaDefinition.Parse("Samples/b_small.in").Wait();
            var requirements = pizzaDefinition.Requirements;
            var pizza = pizzaDefinition.Pizza;
            var pizzaPlotter = new PizzaPlotter();
            var result = pizzaPlotter.Plot(pizzaDefinition);
        }

        [Fact]
        public void PlotPizzaSlices()
        {
            PizzaDefinition pizzaDefinition = new PizzaDefinition();
            pizzaDefinition.Parse("Samples/a_example.in").Wait();
            var slicedPizza = pizzaDefinition.Pizza.Slice();
            var pizzaPlotter = new PizzaPlotter();
            var result = pizzaDefinition.Pizza.PlotSliceSteps();
        }
    }
}
