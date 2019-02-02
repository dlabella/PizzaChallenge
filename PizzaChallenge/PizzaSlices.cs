using System.Collections.Generic;
using System.Linq;

namespace PizzaChallenge
{
    public class PizzaSlices
    {
        HashSet<PizzaSlice> _slices = new HashSet<PizzaSlice>();
        HashSet<string> _invalidSlices = new HashSet<string>();
        HashSet<string> _cellKeys = new HashSet<string>();
        HashSet<string> _cellsInvalid = new HashSet<string>();

        public int Area => _cellKeys.Count;
        public Pizza Pizza { get; }
        public PizzaSlices(Pizza pizza)
        {
            Pizza = pizza;
        }

        public void ClearSlices()
        {
            _cellKeys.Clear();
            _slices.Clear();
        }

        public void AddInvalidCell(PizzaCell invalidCell)
        {
            _cellsInvalid.Add(invalidCell.CellId);
        }

        

        public bool IsInvalidCell(PizzaCell cell)
        {
            return _cellsInvalid.Contains(cell.CellId);
        }

        public bool IsInvalidSlice(PizzaSlice slice)
        {
            return _invalidSlices.Contains(slice.SliceId);
        }

        public void AddSlice(PizzaSlice slice)
        {
            foreach (var cell in slice.GetCells())
            {
                _cellKeys.Add(cell.CellId);
            }
            _slices.Add(slice);
        }

        public void AddInvalidSlice(PizzaSlice slice)
        {
            _invalidSlices.Add(slice.SliceId);
        }

        public void RemoveSlice(PizzaSlice slice)
        {
            foreach (var cell in slice.GetCells())
            {
                _cellKeys.Remove(cell.CellId);
            }
            _slices.Remove(slice);
        }

        public IEnumerable<PizzaSlice> Slices => _slices;

        public PizzaSlice GetSliceFromCell(PizzaCell cell)
        {
            return Slices.FirstOrDefault(x => x.ContainsCell(cell));
        }

        public bool ContainsCellId(string cellKey)
        {
            return _cellKeys.Contains(cellKey);
        }
    }
}
