using System.Collections.Generic;
using System.Linq;

namespace PizzaChallenge
{
    public class PizzaSlice
    {
        public PizzaSlice()
        {
            Tomatoes=0;
            Mushrooms=0;
            PizzaCells = new List<PizzaCell>();
        }
        public PizzaSlice(int cellCount)
        {
            Tomatoes = 0;
            Mushrooms = 0;
            PizzaCells = new List<PizzaCell>(cellCount);
        }
       
        public int Tomatoes { get; set; }
        public int Mushrooms { get; set; }
        public int DistinctIngredients
        {
            get { return (Tomatoes > 0 && Mushrooms > 0) ? 2 : 1; }
        }

        public void AddCell(PizzaCell cell)
        {
            if (cell.Ingredient == 'T')
            {
                Tomatoes++;
            }
            else
            {
                Mushrooms++;
            }
            PizzaCells.Add(cell);
        }
        public List<PizzaCell> PizzaCells { get; set; }
        public string SliceId => $"{PizzaCells.Min().CellId}-{PizzaCells.Max().CellId}";

        public int Area => PizzaCells.Count;
        public bool Visited { get; set; }
        public PizzaCell NextPizzaCell { get; set; }
    }
}
