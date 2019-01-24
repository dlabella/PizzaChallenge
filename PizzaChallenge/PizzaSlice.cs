using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PizzaChallenge
{
    public class PizzaSlice
    {
        public PizzaSlice()
        {
            PizzaCells = new List<PizzaCell>();
        }

        public List<PizzaCell> PizzaCells { get; set; }

        public int Area
        {
            get
            {
                var min = PizzaCells.Min();

                var max = PizzaCells.Max();

                return (max.Col - min.Col + 1) * (max.Row - min.Row + 1);
            }
        }
    }
}
