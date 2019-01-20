using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaChallenge
{
    public class PizzaSlice
    {
        public int StartRow;
        public int StartCol;

        public List<PizzaCell> PizzaCells;
        public PizzaSlice()
        {
            PizzaCells = new List<PizzaCell>();
        }
    }
}
