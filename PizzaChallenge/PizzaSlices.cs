using System.Collections.Generic;
using System.Linq;

namespace PizzaChallenge
{
    public class PizzaSlices
    {
        List<PizzaSlice> _slices = new List<PizzaSlice>();
        public int Area { get; set; }
        public Pizza Pizza { get; }

        public PizzaSlices(Pizza pizza)
        {
            Pizza = pizza;
        }

        public void AddSlice(PizzaSlice slice, int? sliceIndex = null)
        {
            _slices.Add(slice);
            Area += slice.Area;
            slice.PizzaCells.ForEach(item => item.Slice = sliceIndex);
        }

        public void RemoveSlice(PizzaSlice slice)
        {
            if (_slices.Remove(slice))
            {
                Area -= slice.Area;
                slice.PizzaCells.ForEach(item => item.Slice = null);
            }
        }

        public IEnumerable<PizzaSlice> Slices => _slices.OrderByDescending(x => x.Area);
    }
}
