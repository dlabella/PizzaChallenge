using System;
using System.Collections.Generic;

namespace PizzaChallenge
{
    public class PizzaSlice
    {
        private PizzaCell _cellStart;
        private PizzaCell _cellEnd;
        private HashSet<PizzaCell> _pizzaCells;
        public PizzaSlice()
        {
            _pizzaCells = new HashSet<PizzaCell>();
            _cellStart = null;
            _cellEnd = null;
        }

        public string SliceId {get;set; }
        public int SliceNum { get; set; }

        public void AddCell(PizzaCell cell)
        {
            if (_cellStart == null)
            {
                _cellStart = cell;
                _cellEnd = cell;
            }
            else if (_cellStart.Row >= cell.Row && _cellStart.Col >= cell.Col)
            {
                _cellStart = cell;
            }
            else if (_cellEnd.Row <= cell.Row && _cellEnd.Col <= cell.Col)
            {
                _cellEnd = cell;
            }
            _pizzaCells.Add(cell);
            SliceId = GetSliceId();
        }

        public bool ContainsCell(PizzaCell cell)
        {
            return _pizzaCells.Contains(cell);
        }

        private string GetSliceId()
        {
            if (_cellStart != null && _cellEnd != null)
            {
                return $"{_cellStart.CellId}-{_cellEnd.CellId}";
            }
            return "-";
        }

        public IEnumerable<PizzaCell> GetCells()
        {
            return _pizzaCells;
        }

        public int Area
        {
            get
            {
                if (_cellStart != null && _cellEnd != null)
                {
                    return (_cellEnd.Col - _cellStart.Col + 1) * (_cellEnd.Row - _cellStart.Row + 1);
                }
                return -1;
            }
        }
    }
}
