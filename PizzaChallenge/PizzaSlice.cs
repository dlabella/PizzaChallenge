using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaChallenge
{
    public class PizzaSlice
    {
        private List<PizzaCell> pizzaCells;
        public PizzaSlice()
        {
            PizzaCells = new List<PizzaCell>();
        }

        public List<PizzaCell> PizzaCells { get => pizzaCells; set => pizzaCells = value; }
    }
}
