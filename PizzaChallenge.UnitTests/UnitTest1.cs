using System;
using Xunit;

namespace PizzaChallenge.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            PizzaDefinition pizzaDefinition=new PizzaDefinition();
            //pizzaDefinition.Parse("Samples/a_example.in").Wait();
            pizzaDefinition.Parse("Samples/b_small.in").Wait();
            var requirements = pizzaDefinition.Requirements;
            var pizza = pizzaDefinition.Pizza;
            var pizzaPlotter = new PizzaPlotter();
            var result = pizzaPlotter.Plot(pizzaDefinition);
        }
    }
}
