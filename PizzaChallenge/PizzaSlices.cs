using System.Collections.Generic;
using System.Linq;

namespace PizzaChallenge
{
    public class PizzaSlices
    {
        List<PizzaSlice> _slices = new List<PizzaSlice>();
        HashSet<string> _sliceIds = new HashSet<string>();
        public int Area { get; set; }
        public Pizza Pizza { get; }

        public PizzaSlices(Pizza pizza)
        {
            Pizza = pizza;
        }

        public void AddSlice(PizzaSlice slice, int? sliceIndex = null)
        {
            if (!_sliceIds.Contains(slice.SliceId))
            {
                Area += slice.Area;
                _slices.Add(slice);
                _sliceIds.Add(slice.SliceId);
                slice.PizzaCells.ForEach(item => item.Slice = sliceIndex);
            }
        }

        public void RemoveSlice(PizzaSlice slice)
        {
            _slices.Remove(slice);
            _sliceIds.Remove(slice.SliceId);
            Area -= slice.Area;
            slice.PizzaCells.ForEach(item => item.Slice = null);
        }

        public IEnumerable<PizzaSlice> Slices => _slices.OrderByDescending(x => x.Area);
    }
}
