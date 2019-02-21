using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PizzaChallenge
{
    public class PizzaSlicer
    {
        private readonly PizzaOrder _definition;
        private readonly PizzaRequirements _requirements;
        private readonly Pizza _pizza;
        private Pizza _bestSlicedPizza;
        private readonly SliceStatistics _statistics;
        private readonly PizzaCell[,] _pizzaCells;

        DateTime _targetStatisticsTime;
        private const int _slideSeconds = 3;

        public PizzaSlicer(PizzaOrder definition)
        {
            _definition = definition;
            _pizza = _definition.Pizza;
            _bestSlicedPizza = null;
            _requirements = _definition.Requirements;
            _statistics = new SliceStatistics();
            _pizzaCells = _pizza.Cells;
        }

        private void SlideStatisticsTime()
        {
            _targetStatisticsTime = DateTime.Now.AddSeconds(_slideSeconds);
        }

        public Pizza Slice(CancellationTokenSource cts)
        {
            var slices = new PizzaSlices(_pizza);

            SlideStatisticsTime();

            SliceInternal(cts);

            cts.Cancel();

            PrintStatistics(true);

            return _pizza;
        }

        private void PrintStatistics(bool force = false)
        {
            if (_targetStatisticsTime < DateTime.Now || force)
            {
                Console.WriteLine($"Area Filled {_statistics.AreaFilled}");
                Console.WriteLine($"Best Area Filled {_statistics.BestAreaFilled}");
                SlideStatisticsTime();
            }
        }

        private bool SliceInternal(CancellationTokenSource cts)
        {
            var slices = new PizzaSlices(_pizza);
            var sliceStack = new List<StackItem>();
            int sliceIndex = 0;
            var forward = true;
            var backward = false;
            var startRow = 0;
            var startCol = 0;
            int bestAreaSlice = 0;

            PizzaSlice currentSlice = null;
            while (!cts.IsCancellationRequested)
            {
                PrintStatistics();

                if (forward)
                {
                    var startCell = GetFirstCellNotInSlice(startRow, startCol, slices);
                    backward = true;
                    if (startCell != null)
                    {
                        var availableSlices = GetAvailableSlicesAsync(startCell);
                        if (availableSlices.Any())
                        {
                            backward = false;
                            sliceStack.Add(new StackItem(availableSlices.OrderByDescending(x => x.Area).ToList()));
                        }
                    }
                }

                if (backward)
                {
                    if (bestAreaSlice < slices.Area)
                    {
                        bestAreaSlice = slices.Area;
                        _statistics.BestAreaFilled = bestAreaSlice;
                    }

                    if (currentSlice != null)
                    {
                        slices.RemoveSlice(currentSlice);
                    }
                    while (sliceStack[sliceStack.Count - 1].Slices.All(x => x.Visited))
                    {
                        MoveStackBack(slices, sliceStack);
                    }
                    forward = false;
                }

                if (sliceStack.Count > 0)
                {
                    var slice = sliceStack[sliceStack.Count - 1].Slices.FirstOrDefault(x => !x.Visited);
                    if (slice != null)
                    {
                        currentSlice = slice;
                        slice.Visited = true;
                        slices.AddSlice(slice, sliceIndex++);

                        startRow = slice.NextPizzaCell.Row;
                        startCol = slice.NextPizzaCell.Col;
                    }
                    forward = true;
                }

                _statistics.AreaFilled = slices.Area;
                if (slices.Area == _pizza.Area)
                {
                    _bestSlicedPizza = _pizza;
                    return true;
                }
            }
            return false;
        }

        private void MoveStackBack(PizzaSlices slices, List<StackItem> sliceStack)
        {
            sliceStack.RemoveAt(sliceStack.Count - 1);

            foreach (var item in sliceStack[sliceStack.Count - 1].Slices)
            {
                item.PizzaCells.ForEach(x => x.Slice = null);
                slices.RemoveSlice(item);
            }
        }

        public PizzaCell GetFirstCellNotInSlice(int startRow, int startCol, PizzaSlices slices)
        {
            for (var row = startRow; row < _pizza.Rows; row++)
            {
                for (var col = startCol; col < _pizza.Columns; col++)
                {
                    if (_pizzaCells[row, col].Slice == null)
                    {
                        return _pizzaCells[row, col];
                    }
                }
                startCol = 0;
            }
            return null;
        }

        private List<PizzaSlice> GetAvailableSlicesAsync(PizzaCell cellStart)
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


            GetLeftBottomSlices(cellStart, maxRow, maxCol).ForEach(x => retVal.AddSlice(x));
            GetBottomRightSlices(cellStart, maxRow, minCol).ForEach(x => retVal.AddSlice(x));
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

        private List<PizzaSlice> GetBottomRightSlices(PizzaCell cellStart, int maxRow, int minCol)
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

        private List<PizzaSlice> GetLeftBottomSlices(PizzaCell cellStart, int maxRow, int maxCol)
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
            return IsCellAvailable(_pizzaCells[row, col]);
        }

        private bool GetValidSlice(PizzaCell cellStart, int row, int col, out PizzaSlice slice)
        {
            var cellCount = GetCellCount(cellStart, row, col);
            if (cellCount > 1 && cellCount <= _requirements.SliceMaxCells)
            {
                var localSlice = GetSlice(cellStart, _pizzaCells[row, col]);
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
            var groups = slice.PizzaCells.GroupBy(x => x.Ingredient);
            if (groups.Count() >= _pizza.DistinctIngredientsCount)
            {
                if (groups.All(x => x.Count() >= _requirements.SliceMinIngredients))
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
                    pizzaSlice.PizzaCells.Add(_pizza.Cells[row, col]);
                }
            }
            pizzaSlice.NextPizzaCell = _pizzaCells[cellStart.Row, maxCol];
            return pizzaSlice;
        }
    }

}
