using System.Collections.Generic;
using System.Linq;

namespace PizzaChallenge
{
    public class PizzaSlice
    {
        public PizzaSlice()
        {
            PizzaCells = new List<PizzaCell>();
        }
        public PizzaSlice(int cellCount)
        {
            PizzaCells = new List<PizzaCell>(cellCount);
        }
        public List<PizzaCell> PizzaCells { get; set; }
        public string SliceId => $"{PizzaCells.Min().CellId}-{PizzaCells.Max().CellId}";

        public int Area => PizzaCells.Count;
    }
}
