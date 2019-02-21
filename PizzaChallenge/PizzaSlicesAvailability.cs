using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PizzaChallenge
{
    public class PizzaSlicesAvailability
    {
        PizzaRequirements _requirements;
        Pizza _pizza;
        public PizzaSlicesAvailability(PizzaRequirements requirements, Pizza pizza)
        {
            _requirements = requirements;
            _pizza = pizza;
        }

        public List<PizzaSlice> GetAvailableSlices(PizzaCell cellStart)
        {
            var retVal = new PizzaSlices(_pizza);
            if (cellStart == null)
            {
                return retVal.Slices.ToList();
            }

            var maxRow = Math.Min(cellStart.Row + _requirements.SliceMaxCells, _pizza.Rows);
            var maxCol = Math.Min(cellStart.Col + _requirements.SliceMaxCells, _pizza.Columns);
            var minRow = Math.Max(cellStart.Row - _requirements.SliceMaxCells, 0);
            var minCol = Math.Max(cellStart.Col - _requirements.SliceMaxCells, 0);

            GetRightBottomSlices(cellStart, maxRow, maxCol).ForEach(x => retVal.AddSlice(x));
            GetLeftBottomSlices(cellStart, maxRow, minCol).ForEach(x => retVal.AddSlice(x));
            GetTopLeftSlices(cellStart, minRow, minCol).ForEach(x => retVal.AddSlice(x));
            GetTopRightSlices(cellStart, minRow, maxCol).ForEach(x => retVal.AddSlice(x));

            if (retVal.Area == 0)
            {
                cellStart.Slice = -1;
            }

            return retVal.Slices.ToList();
        }

        private List<PizzaSlice> GetTopRightSlices(PizzaCell cellStart, int minRow, int maxCol)
        {
            var retVal = new List<PizzaSlice>();
            for (var row = cellStart.Row; row > minRow; row--)
            {
                for (var col = cellStart.Col; col < maxCol; col++)
                {
                    if (!IsCellAvailable(row, col))
                    {
                        maxCol = col;
                        break;
                    }
                    else if (GetValidSlice(cellStart, row, col, out var slice))
                    {
                        retVal.Add(slice);
                    }
                }
            }

            return retVal;
        }

        private List<PizzaSlice> GetTopLeftSlices(PizzaCell cellStart, int minRow, int minCol)
        {
            var retVal = new List<PizzaSlice>();
            for (var row = cellStart.Row; row > minRow; row--)
            {
                for (var col = cellStart.Col; col > minCol; col--)
                {
                    if (!IsCellAvailable(row, col))
                    {
                        minCol = col;
                        break;
                    }
                    else if (GetValidSlice(cellStart, row, col, out var slice))
                    {
                        retVal.Add(slice);
                    }
                }
            }
            return retVal;
        }

        private List<PizzaSlice> GetLeftBottomSlices(PizzaCell cellStart, int maxRow, int minCol)
        {
            var retVal = new List<PizzaSlice>();
            for (var row = cellStart.Row; row < maxRow; row++)
            {
                for (var col = cellStart.Col; col > minCol; col--)
                {
                    if (!IsCellAvailable(row, col))
                    {
                        minCol = col;
                        break;
                    }
                    else if (GetValidSlice(cellStart, row, col, out var slice))
                    {
                        retVal.Add(slice);
                    }
                }
            }
            return retVal;
        }

        private List<PizzaSlice> GetRightBottomSlices(PizzaCell cellStart, int maxRow, int maxCol)
        {
            var retVal = new List<PizzaSlice>();
            for (var row = cellStart.Row; row < maxRow; row++)
            {
                for (var col = cellStart.Col; col < maxCol; col++)
                {
                    if (!IsCellAvailable(row, col))
                    {
                        maxCol = col;
                        break;
                    }
                    else if (GetValidSlice(cellStart, row, col, out var slice))
                    {
                        retVal.Add(slice);
                    }
                }
            }
            return retVal;
        }

        private bool IsCellAvailable(PizzaCell cell)
        {
            return (cell.Slice == null || cell.Slice == -1);
        }

        private bool IsCellAvailable(int row, int col)
        {
            return IsCellAvailable(_pizza.Cells[row, col]);
        }

        private bool GetValidSlice(PizzaCell cellStart, int row, int col, out PizzaSlice slice)
        {
            var cellCount = GetCellCount(cellStart, row, col);
            if (cellCount > 1 && cellCount <= _requirements.SliceMaxCells)
            {
                var localSlice = GetSlice(cellStart, _pizza.Cells[row, col]);
                if (SliceMeetsRequirements(localSlice))
                {
                    slice = localSlice;
                    return true;
                }
            }
            slice = null;
            return false;
        }

        private bool SliceMeetsRequirements(PizzaSlice slice)
        {
            if (slice.DistinctIngredients >= _pizza.DistinctIngredientsCount)
            {
                if (slice.Mushrooms >= _requirements.SliceMinIngredients &&
                    slice.Tomatoes >= _requirements.SliceMinIngredients)
                {
                    return true;
                }
            }
            return false;
        }

        private int GetCellCount(PizzaCell cellStart, int row, int col)
        {
            return ((col - cellStart.Col) + 1) * ((row - cellStart.Row) + 1);
        }

        private PizzaSlice GetSlice(PizzaCell cellStart, PizzaCell pizzaCell)
        {
            var maxCol = pizzaCell.Col;
            var maxRow = pizzaCell.Row;
            int cellCount = (maxRow - cellStart.Row) * (maxCol - cellStart.Col);
            var pizzaSlice = new PizzaSlice(cellCount);
            for (var row = cellStart.Row; row <= maxRow; row++)
            {
                for (var col = cellStart.Col; col <= maxCol; col++)
                {
                    pizzaSlice.AddCell(_pizza.Cells[row, col]);
                }
            }
            pizzaSlice.NextPizzaCell = _pizza.Cells[cellStart.Row, maxCol];
            return pizzaSlice;
        }

    }
}
