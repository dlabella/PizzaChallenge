using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaChallenge
{
    public class PizzaCell : IComparable
    {
        public PizzaCell(int row, int col, char ingredient, int? slice = null)
        {
            Row = row;
            Col = col;
            Ingredient = ingredient;
            Slice = slice;
        }
        public int Row { get; }
        public int Col { get; }
        public char Ingredient { get; }
        public int? Slice { get; set; }

        public int CompareTo(object obj)
        {
            var pizzaCell = obj as PizzaCell;

            if(pizzaCell.Row + pizzaCell.Col > this.Row + this.Col)
            {
                return -1;
            }
            else if (pizzaCell.Row + pizzaCell.Col == this.Row + this.Col)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
