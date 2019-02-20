﻿using System;
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
        private readonly SliceStatistics _statistics;
        private int _sliceIndex;
        private readonly PizzaCell[,] _pizzaCells;
        DateTime _targetStatisticsTime;
        private const int _slideSeconds = 3;

        public PizzaSlicer(PizzaOrder definition)
        {
            _definition = definition;
            _pizza = _definition.Pizza;
            _requirements = _definition.Requirements;
            _statistics = new SliceStatistics();
            _sliceIndex = 0;
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

            //SliceInternal(slices, cts);
            DoSlice(cts);

            cts.Cancel();

            PrintStatistics(true);

            return _pizza;
        }

        private void PrintStatistics(bool force = false)
        {
            if (_targetStatisticsTime < DateTime.Now || force)
            {
                Console.WriteLine($"Area Filled {_statistics.AreaFilled}");

                SlideStatisticsTime();
            }
        }

        private bool DoSlice(CancellationTokenSource cts)
        {
            var slices = new PizzaSlices(_pizza);
            var sliceStack = new List<List<PizzaSlice>>();
            int sliceIndex = 0;
            var forward = true;
            var shouldMoveBack = false;
            PizzaSlice currentSlice = null;
            while (!cts.IsCancellationRequested)
            {
                PrintStatistics();

                if (forward)
                {
                    var startCell = GetFirstCellNotInSlice(slices);

                    if (startCell != null)
                    {
                        var availableSlices = GetAvailableSlices(startCell);
                        if (availableSlices.Any())
                        {
                            sliceStack.Add(availableSlices.OrderByDescending(x => x.Area).ToList());
                        }
                        else
                        {
                            shouldMoveBack = true;
                        }
                    }
                    else
                    {
                        shouldMoveBack = true;
                    }
                }

                if (shouldMoveBack)
                {
                    if (currentSlice != null)
                    {
                        slices.RemoveSlice(currentSlice);
                    }
                    while (sliceStack[sliceStack.Count - 1].All(x => x.Visited))
                    {
                        MoveStackBack(slices, sliceStack);
                    }
                    forward = false;
                }

                if (sliceStack.Count > 0)
                {
                    var slice = sliceStack[sliceStack.Count - 1].FirstOrDefault(x => !x.Visited);
                    if (slice != null)
                    {
                        currentSlice = slice;
                        slice.Visited = true;
                        slices.AddSlice(slice, sliceIndex++);
                    }
                    forward = true;
                    shouldMoveBack = false;
                }

                _statistics.AreaFilled = slices.Area;
                if (_pizza.Cells.Items().Count(x => x.Slice >= 0) == _pizza.Area)
                {
                    return true;
                }
            }
            return false;
        }

        private static void MoveStackBack(PizzaSlices slices, List<List<PizzaSlice>> sliceStack)
        {
            sliceStack.RemoveAt(sliceStack.Count - 1);

            foreach (var item in sliceStack[sliceStack.Count - 1])
            {
                item.PizzaCells.ForEach(x =>
                {
                    x.Slice = null;
                });
                slices.RemoveSlice(item);
            }
        }

        private static void ClearVisitedSlices(PizzaSlices slices, List<List<PizzaSlice>> sliceStack)
        {
            sliceStack[sliceStack.Count - 1].ForEach(x =>
            {
                if (x.Visited)
                {
                    slices.RemoveSlice(x);
                }
            });
        }

        private bool SliceInternal(PizzaSlices slices, CancellationTokenSource cts)
        {
            PrintStatistics();

            if (cts.IsCancellationRequested) { return true; }

            var startCell = GetFirstCellNotInSlice(slices);

            if (startCell == null) { return false; }

            foreach (var slice in GetAvailableSlices(startCell))
            {
                if (cts.IsCancellationRequested) { return true; }
                _sliceIndex++;
                slices.AddSlice(slice, _sliceIndex);
                _statistics.AreaFilled = slices.Area;
                if (slices.Area == _pizza.Area)
                {
                    return true;
                }

                if (SliceInternal(slices, cts))
                {
                    return true;
                }

                slices.RemoveSlice(slice);
            };

            return false;
        }

        public PizzaCell GetFirstCellNotInSlice(PizzaSlices slices)
        {
            for (var row = 0; row < _pizza.Rows; row++)
            {
                for (var col = 0; col < _pizza.Columns; col++)
                {
                    if (_pizzaCells[row, col].Slice == null)
                    {
                        return _pizzaCells[row, col];
                    }
                }
            }
            return null;
        }

        private IEnumerable<PizzaSlice> GetAvailableSlices(PizzaCell cellStart)
        {
            var retVal = new PizzaSlices(_pizza);
            if (cellStart == null)
            {
                return retVal.Slices;
            }

            var maxRow = Math.Min(cellStart.Row + _requirements.SliceMaxCells, _pizza.Rows);
            var maxCol = Math.Min(cellStart.Col + _requirements.SliceMaxCells, _pizza.Columns);
            var minRow = Math.Max(cellStart.Row - _requirements.SliceMaxCells, 0);
            var minCol = Math.Max(cellStart.Col - _requirements.SliceMaxCells, 0);

            var lbRow = maxRow;
            var lbCol = maxCol;

            var brRow = maxRow;
            var brCol = maxCol;

            var tlRow = minRow;
            var tlCol = minCol;

            var trRow = minRow;
            var trCol = maxCol;

            for (var row = cellStart.Row; row < lbRow; row++)
            {
                for (var col = cellStart.Col; col < lbCol; col++)
                {
                    if (!IsCellAvailable(row, col))
                    {
                        lbCol = col;
                        break;
                    }
                    else if (GetValidSlice(cellStart, row, col, out var slice))
                    {
                        retVal.AddSlice(slice);
                    }
                }
            }

            for (var row = cellStart.Row; row < brRow; row++)
            {
                for (var col = cellStart.Col; col > brCol; col--)
                {
                    if (!IsCellAvailable(row, col))
                    {
                        brRow = col;
                        break;
                    }
                    else if (GetValidSlice(cellStart, row, col, out var slice))
                    {
                        retVal.AddSlice(slice);
                    }
                }
            }

            for (var row = cellStart.Row; row > tlRow; row--)
            {
                for (var col = cellStart.Col; col > tlCol; col--)
                {
                    if (!IsCellAvailable(row, col))
                    {
                        tlCol = col;
                        break;
                    }
                    else if (GetValidSlice(cellStart, row, col, out var slice))
                    {
                        retVal.AddSlice(slice);
                    }
                }
            }

            for (var row = cellStart.Row; row > trRow; row--)
            {
                for (var col = cellStart.Col; col < trCol; col++)
                {
                    if (!IsCellAvailable(row, col))
                    {
                        trCol = col;
                        break;
                    }
                    else if (GetValidSlice(cellStart, row, col, out var slice))
                    {
                        retVal.AddSlice(slice);
                    }
                }
            }

            if (retVal.Area == 0)
            {
                cellStart.Slice = -1;
            }

            return retVal.Slices;
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

            return pizzaSlice;
        }
    }

}
