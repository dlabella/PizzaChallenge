using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private readonly SliceStatistics _statistics;
        private readonly SliceStatistics _statisticsBuff;
        private readonly PizzaPlotter _plotter;
        public PizzaSlicer(PizzaOrder definition)
        {
            _definition = definition;
            _pizza = _definition.Pizza;
            _requirements = _definition.Requirements;
            _statistics = new SliceStatistics();
            _statisticsBuff = new SliceStatistics();
            _plotter=new PizzaPlotter();
        }

        public Pizza Slice()
        {
            var slices = new PizzaSlices(_pizza);
            var cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    var buff = _statistics.SlicesProcessed - _statisticsBuff.SlicesProcessed;
                    _statisticsBuff.SlicesProcessed = _statistics.SlicesProcessed;
                    System.Diagnostics.Debug.WriteLine($"Slices per seconds {buff}");
                    System.Diagnostics.Debug.WriteLine($"Area Filled {_statistics.AreaFilled} of {_pizza.Area}");
                    Thread.Sleep(1000);
                }
            });

            SliceInternal(_pizza, slices);
            cts.Cancel();

            int sliceIndex = 0;
            foreach (var slice in slices.Slices)
            {
                foreach (var cell in slice.PizzaCells)
                {
                    cell.Slice = sliceIndex;
                }
                sliceIndex++;
            }
            return _pizza;
        }


        private bool SliceInternal(Pizza pizza, PizzaSlices slices)
        {
            var newSlices = GetSlices(pizza, slices, _requirements.SliceMaxCells);
            foreach (var slice in newSlices.Slices)
            {
                _statistics.SlicesProcessed++;
                slices.AddSlice(slice);
                _statistics.AreaFilled = slices.Area;
                if (slices.Area == pizza.Area)
                {
                    return true;
                }

                if (SliceInternal(pizza, slices))
                {
                    return true;
                }
                slices.RemoveSlice(slice);
            };

            return false;
        }

        public PizzaCell GetFirstCellNotInSlice(Pizza pizza, PizzaSlices slices)
        {
            return pizza.Cells.Items().FirstOrDefault(x => !slices.ContainsCellId(x.CellId) && x.Slice!=-1);
        }

        private PizzaSlices GetSlices(Pizza pizza, PizzaSlices currentSlices, int sliceMaxCells)
        {
            var returnValue = new PizzaSlices(pizza);

            var cellStart = GetFirstCellNotInSlice(pizza, currentSlices);
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
                    var currentCell = pizza.Cells[row, col];
                    if (currentSlices.ContainsCellId(currentCell.CellId))
                    {
                        break;
                    }
                    var cellCount = GetCellCount(cellStart, row, col);
                    if (cellCount > 1 && cellCount <= sliceMaxCells)
                    {
                        var slice = GetSlice(cellStart, currentCell);
                        if (SliceMeetsRequirements(slice))
                        {
                            returnValue.AddSlice(slice);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            if (returnValue.Area == 0)
            {
                cellStart.Slice=-1;
            }
            return returnValue;
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
