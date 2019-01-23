using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

        public int Area
        {
            get
            {
                var min = pizzaCells.Min();

                var max = pizzaCells.Max();

                return (max.Col - min.Col + 1) * (max.Row - min.Row + 1);
            }
        }
    }
}
