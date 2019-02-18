using System;
using Xunit;

namespace PizzaChallenge.UnitTests
{
    public class UnitTest1
    {

        [Fact]
        public void PizzaCellComparableWorks()
        {
            PizzaCell cellA;
            PizzaCell cellB;

            cellA = new PizzaCell(0, 1, 'T');
            cellB = new PizzaCell(1, 0, 'T');
            Assert.True(cellA.CompareTo(cellB) < 0);

            cellA = new PizzaCell(1, 0, 'T');
            cellB = new PizzaCell(0, 1, 'T');
            Assert.True(cellA.CompareTo(cellB) > 0);

            cellA = new PizzaCell(1, 1, 'T');
            cellB = new PizzaCell(1, 1, 'T');
            Assert.True(cellA.CompareTo(cellB) == 0);
        }
        
    }
}
