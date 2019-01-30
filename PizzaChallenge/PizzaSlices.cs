using System.Collections.Generic;
using System.Linq;

namespace PizzaChallenge
{
    public class PizzaSlices
    {
        private readonly Pizza _pizza;
        List<PizzaSlice> _slices = new List<PizzaSlice>();
        HashSet<string> _cellKeys = new HashSet<string>();
        HashSet<string> _invalidSlices = new HashSet<string>();
        public int Area => _cellKeys.Count;
        public Pizza Pizza { get; }
        public PizzaSlices(Pizza pizza)
        {
            Pizza = pizza;
        }
        public void AddSlice(PizzaSlice slice)
        {
            foreach (var cell in slice.PizzaCells)
            {
                _cellKeys.Add(cell.CellId);
            }
            _slices.Add(slice);
        }

        public void AddInvalidSlice(PizzaSlice slice)
        {
            var max = slice.PizzaCells.Max();
            var min = slice.PizzaCells.Min();
            _invalidSlices.Add($"{min.CellId}-{max.CellId}");
        }

        public bool IsInvalidSlice(PizzaSlice slice)
        {
            var max = slice.PizzaCells.Max();
            var min = slice.PizzaCells.Min();
            return _invalidSlices.Contains($"{min.CellId}-{max.CellId}");
        }

        public void RemoveSlice(PizzaSlice slice)
        {
            foreach (var cell in slice.PizzaCells)
            {
                _cellKeys.Remove(cell.CellId);
            }
            _slices.Remove(slice);
        }

        public IEnumerable<PizzaSlice> Slices => _slices.OrderByDescending(x=>x.Area);

        public PizzaSlice GetSliceFromCell(PizzaCell cell)
        {
            return Slices.FirstOrDefault(x => x.PizzaCells.Any(y => y == cell));
        }

        public bool ContainsCellId(string cellKey)
        {
            return _cellKeys.Contains(cellKey);
        }
    }
}
