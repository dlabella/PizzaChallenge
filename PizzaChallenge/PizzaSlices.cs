using System.Collections.Generic;
using System.Linq;

namespace PizzaChallenge
{
    public class PizzaSlices
    {
        List<PizzaSlice> _slices = new List<PizzaSlice>();
        HashSet<string> _slicesId = new HashSet<string>();
        public int Area { get;set;}
        public Pizza Pizza { get; }

        public PizzaSlices(Pizza pizza)
        {
            Pizza = pizza;
        }

        public void AddSlice(PizzaSlice slice, int? sliceIndex = null)
        {
            if (!_slicesId.Contains(slice.SliceId))
            {
                Area+=slice.Area;
                _slicesId.Add(slice.SliceId);
                _slices.Add(slice);
                slice.PizzaCells.ForEach(item => item.Slice = sliceIndex);
            }
        }

        public void RemoveSlice(PizzaSlice slice)
        {
            _slices.Remove(slice);
            Area -= slice.Area;
            _slicesId.Remove(slice.SliceId);
            slice.PizzaCells.ForEach(item => item.Slice = null);
        }

        public IEnumerable<PizzaSlice> Slices => _slices.OrderByDescending(x => x.Area);
    }
}
