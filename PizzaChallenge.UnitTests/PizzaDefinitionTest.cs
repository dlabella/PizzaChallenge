using System;
using System.Collections.Generic;
using Xunit;

namespace PizzaChallenge.UnitTests
{
    public class PizzaDefinitionTest
    {
        [Fact]
        public void GetFirstCellNotInSliceTest()
        {
            PizzaDefinition pizzaDefinition = new PizzaDefinition();
            pizzaDefinition.Parse("Samples/b_small.in").Wait();

            var cell = pizzaDefinition.Pizza.GetFirstCellNotInSlice();
            Assert.True(cell.Slice == null);
            Assert.True(cell.Row == 0 && cell.Col == 0);
        }

        [Fact]
        public void SpectSecondSliceTest()
        {
            PizzaDefinition pizzaDefinition = new PizzaDefinition();
            pizzaDefinition.Parse("Samples/b_small.in").Wait();

            var cell = pizzaDefinition.Pizza.GetFirstCellNotInSlice();
            cell.Slice = 0;
            var cell2 = pizzaDefinition.Pizza.GetFirstCellNotInSlice();
            Assert.Null(cell2.Slice);
            Assert.Equal(0, cell2.Row);
            Assert.Equal(1, cell2.Col);
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(3, 4)]
        [InlineData(4, 7)]
        public void ExpectedSlicesTest(int maxCells, int sliceCount)
        {
            PizzaDefinition pizzaDefinition = new PizzaDefinition();
            pizzaDefinition.Parse($"Samples/c_mini_{maxCells}.in").Wait();

            List<PizzaSlice> slices = pizzaDefinition.Pizza.GetSlices();
            Assert.Equal(sliceCount, slices.Count);
        }

        [Theory]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(4, 2)]
        public void ExpectedFilteredSlicesTest(int maxCells, int sliceCount)
        {
            PizzaDefinition pizzaDefinition = new PizzaDefinition();
            pizzaDefinition.Parse($"Samples/c_mini_{maxCells}.in").Wait();

            List<PizzaSlice> slices = pizzaDefinition.Pizza.GetFilteredSlices();
            Assert.Equal(sliceCount, slices.Count);
        }
    }
}
