using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaChallenge
{
    public class PizzaSlicer
    {
        private readonly PizzaOrder _definition;
        private readonly PizzaRequirements _requirements;
        private readonly Pizza _pizza;
        

        public PizzaSlicer(PizzaOrder definition)
        {
            _definition = definition;
            _pizza = _definition.Pizza;
            _requirements = _definition.Requirements;
        }

        public Pizza Slice()
        {
            var statistics = new SlicingStatistics();
            var cts = new CancellationTokenSource();
            var results = new ConcurrentBag<Pizza>();
            Task.Run(() =>
            {
                var slicesBuff = 0;
                var totalCells = _pizza.Rows * _pizza.Columns;
                while (!cts.IsCancellationRequested)
                {
                    var slicesProc = statistics.SlicesProcessed - slicesBuff;
                    slicesBuff = statistics.SlicesProcessed;
                    System.Diagnostics.Debug.WriteLine($"Slices processed per second: {slicesProc}");
                    System.Diagnostics.Debug.WriteLine($"Percent completed ap: {statistics.CellsInSlice} of {totalCells}");
                    Thread.Sleep(1000);
                }
            });

            SliceInternal(_pizza, results, statistics);
            cts.Cancel();
            return results.FirstOrDefault();
        }


        private bool SliceInternal(Pizza sourcePizza, ConcurrentBag<Pizza> results, SlicingStatistics statistics, CancellationTokenSource cts = null)
        {
            if (cts == null)
            {
                cts = new CancellationTokenSource();
            }
            var slices = GetSlices(sourcePizza, _requirements.SliceMaxCells);
            if (slices == null || slices.Count == 0)
            {
                if (sourcePizza.Cells.Items().All(x => x.Slice != null))
                {
                    results.Add(sourcePizza);
                    return true;
                }
                return false;
            }

            Parallel.ForEach(slices, (slice, state) =>
            {
                if (!cts.IsCancellationRequested)
                {
                    var newPizza = sourcePizza.Clone() as Pizza;
                    newPizza.AddSlice(slice);
                    statistics.SlicesProcessed += slice.PizzaCells.Count;
                    statistics.CellsInSlice = newPizza.CellsInSlice;
                    var solved = SliceInternal(newPizza, results, statistics, cts);
                    if (solved)
                    {
                        cts.Cancel();
                        state.Stop();
                    }
                }
            });

            return false;
        }

        private static Pizza GetBestSolution(Pizza sourcePizza, Pizza bestSolution)
        {
            var currentEmptyCount = sourcePizza.Cells.Items().Count(x => x.Slice == null);
            var bestEmptyCount = bestSolution.Cells.Items().Count(x => x.Slice == null);
            if (currentEmptyCount < bestEmptyCount)
            {
                bestSolution = sourcePizza;
            }

            return bestSolution;
        }

        //private List<PizzaSlice> GetFilteredSlices(Pizza pizza, int sliceMaxCells)
        //{
        //    var slices = GetSlices(pizza, sliceMaxCells);
        //    var result = new List<PizzaSlice>(slices.Count);
        //    if (slices == null)
        //    {
        //        return null;
        //    }

        //    foreach(var slice in slices)
        //    {
        //        var groups = slice.PizzaCells.GroupBy(x=>x.Ingredient);
        //        if (groups.Count()>= _pizza.DistinctIngredientsCount)
        //        {
        //            if (groups.All(x=>x.Count()>= _requirements.SliceMinIngredients))
        //            {
        //                result.Add(slice);
        //            }
        //        }
        //    }
        //    return result;
        //}

        private List<PizzaSlice> GetSlices(Pizza pizza, int sliceMaxCells)
        {
            var returnValue = new List<PizzaSlice>();

            var cellStart = pizza.GetFirstCellNotInSlice();
            if (cellStart == null)
            {
                return returnValue;
            }

            var maxRow = Math.Min(cellStart.Row + sliceMaxCells, pizza.Rows - 1);
            var maxCol = Math.Min(cellStart.Col + sliceMaxCells, pizza.Columns - 1);

            for (var row = maxRow; row >= cellStart.Row; row--)
            {
                for (var col = maxCol; col >= cellStart.Col; col--)
                {
                    if (pizza.Cells[row, col].Slice != null)
                    {
                        break;
                    }
                    var cellCount = GetCellCount(cellStart, row, col);
                    if (cellCount > 1 && cellCount <= sliceMaxCells)
                    {
                        var slice = GetSlice(cellStart, pizza.Cells[row, col]);
                        if (SliceMeetsRequirements(slice))
                        {
                            returnValue.Add(slice);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return returnValue.OrderByDescending(x => x.Area).ToList();
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
            var pizzaSlice = new PizzaSlice();
            for (var row = cellStart.Row; row <= pizzaCell.Row; row++)
            {
                for (var col = cellStart.Col; col <= pizzaCell.Col; col++)
                {
                    pizzaSlice.PizzaCells.Add(_pizza.Cells[row, col]);
                }
            }

            return pizzaSlice;
        }
    }

}
