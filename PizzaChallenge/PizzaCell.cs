using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaChallenge
{
    public class PizzaCell
    {
        public PizzaCell(int row , int col, char ingredient)
        {
            Row=row;
            Col=col;
            Ingredient=ingredient;
        }
        public int Row { get;}
        public int Col { get;}
        public char Ingredient { get;}
        public int? Slice { get; set; }
    }
}
