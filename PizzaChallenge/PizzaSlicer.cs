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
        private readonly PizzaSlicesAvailability _slicesAvailability;
        DateTime _targetStatisticsTime;
        private const int _slideSeconds = 3;

        public PizzaSlicer(PizzaOrder definition)
        {
            _definition = definition;
            _pizza = _definition.Pizza;
            _requirements = _definition.Requirements;
            _statistics = new SliceStatistics();
            _slicesAvailability = new PizzaSlicesAvailability(_requirements,_pizza);
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
            var next = true;
            var previous = false;
            var startRow = 0;
            var startCol = 0;

            PizzaSlice currentSlice = null;
            while (!cts.IsCancellationRequested)
            {
                PrintStatistics();

                if (next)
                {
                    previous = !ProcessNext(slices, sliceStack, startRow, startCol);
                }

                if (previous)
                {
                    ProcessPrevious(slices, sliceStack, currentSlice);
                    next = false;
                }

                if (sliceStack.Count > 0)
                {
                    ComputeSlice(slices, sliceStack, ref sliceIndex, ref startRow, ref startCol, ref currentSlice);
                    next = true;
                }

                _statistics.AreaFilled = slices.Area;
                if (slices.Area == _pizza.Area)
                {
                    return true;
                }
            }
            return false;
        }

        private static void ComputeSlice(PizzaSlices slices, List<StackItem> sliceStack, ref int sliceIndex, ref int startRow, ref int startCol, ref PizzaSlice currentSlice)
        {
            var slice = sliceStack[sliceStack.Count - 1].Slices.FirstOrDefault(x => !x.Visited);
            if (slice != null)
            {
                currentSlice = slice;
                slice.Visited = true;
                slices.AddSlice(slice, sliceIndex++);

                startRow = Math.Max(0, slice.NextPizzaCell.Row);
                startCol = Math.Max(0, slice.NextPizzaCell.Col);
            }
        }

        private void ProcessPrevious(PizzaSlices slices, List<StackItem> sliceStack, PizzaSlice currentSlice)
        {
            if (currentSlice != null)
            {
                slices.RemoveSlice(currentSlice);
            }
            while (sliceStack[sliceStack.Count - 1].Slices.All(x => x.Visited))
            {
                PopStack(slices, sliceStack);
            }
        }

        private bool ProcessNext(PizzaSlices slices, List<StackItem> sliceStack, int startRow, int startCol)
        {
            bool sliceOk;
            var startCell = GetFirstCellNotInSlice(startRow, startCol, slices);
            sliceOk = false;
            if (startCell != null)
            {
                var availableSlices = _slicesAvailability.GetAvailableSlices(startCell);
                if (availableSlices.Any())
                {
                    sliceOk = true;
                    sliceStack.Add(new StackItem(availableSlices.OrderByDescending(x => x.Area).ToList()));
                }
            }

            return sliceOk;
        }

        private void PopStack(PizzaSlices slices, List<StackItem> sliceStack)
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
                    var cell = _pizza.Cells[row, col];
                    if (cell.Slice == null)
                    {
                        return cell;
                    }
                }
                startCol = 0;
            }
            return null;
        }
    }
}
