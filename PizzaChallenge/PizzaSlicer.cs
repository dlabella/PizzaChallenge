using System;
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
        private readonly SliceStatistics _statistics;
        private readonly SliceStatistics _statisticsBuff;
        private readonly PizzaPlotter _plotter;
        private readonly PizzaSlices _pizzaSlices;

        public PizzaSlicer(PizzaOrder definition)
        {
            _definition = definition;
            _pizza = _definition.Pizza;
            _requirements = _definition.Requirements;
            _statistics = new SliceStatistics();
            _statisticsBuff = new SliceStatistics();
            _plotter = new PizzaPlotter();
            _pizzaSlices=new PizzaSlices(_pizza);
        }

        public Pizza Slice()
        {
            var cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    var buff = _statistics.SlicesProcessed - _statisticsBuff.SlicesProcessed;
                    _statisticsBuff.SlicesProcessed = _statistics.SlicesProcessed;
                    System.Diagnostics.Debug.WriteLine($"Slices per seconds {buff}");
                    System.Diagnostics.Debug.WriteLine($"Current Slice {_statistics.CurrentSliceId}");
                    System.Diagnostics.Debug.WriteLine($"Area Filled {_statistics.AreaFilled} of {_pizza.Area}");
                    System.Diagnostics.Debug.WriteLine($"Best Area Filled {_statistics.BestAreaFilled} of {_pizza.Area}");
                    Thread.Sleep(1000);
                }
            });

            var result = SliceInternal(cts);

            int sliceIndex = 0;
            foreach (var slice in _pizzaSlices.Slices)
            {
                foreach (var cell in slice.GetCells())
                {
                    cell.Slice = sliceIndex;
                }
                sliceIndex++;
            }

            return _pizza;
        }


        private bool SliceInternal(CancellationTokenSource cts = null)
        {
            var cellStart = GetFirstCellNotInSlice();
            var newSlices = GetValidSlices(cellStart).OrderBy(x=>x.Area);
            foreach (var slice in newSlices)
            {
                _pizzaSlices.AddSlice(slice);
                _statistics.CurrentSliceId = slice.SliceId;
                _statistics.AreaFilled = _pizzaSlices.Area;
                if (_pizzaSlices.Area == _pizza.Area)
                {
                    return true;
                }
                if (cts != null && cts.IsCancellationRequested)
                {
                    return false;
                }
                if (SliceInternal(cts))
                {
                    return true;
                }
                _pizzaSlices.RemoveSlice(slice);
            };

            return false;
        }

        private PizzaCell GetFirstCellNotInSlice()
        {
            return _pizza.Cells.Items().FirstOrDefault(x => !_pizzaSlices.ContainsCellId(x.CellId) && !_pizzaSlices.IsInvalidCell(x));
        }

        private IEnumerable<PizzaSlice> GetValidSlices(PizzaCell cellStart)
        {
            
            var availableSlices = GetAvailableSlices(cellStart);
            if (!availableSlices.Any())
            {
                _pizzaSlices.AddInvalidCell(cellStart);
                yield return null;
            }

            foreach (var slice in availableSlices)
            {
                if (SliceMeetsRequirements(_pizzaSlices, slice))
                {
                    yield return slice;
                }
            }
        }

        private IEnumerable<PizzaSlice> GetAvailableSlices(PizzaCell cellStart)
        {
            if (cellStart == null)
            {
                yield return null;
            }
            var sliceMaxCells = _requirements.SliceMaxCells;
            var maxRow = Math.Min(cellStart.Row + sliceMaxCells, _pizza.Rows - 1);
            var maxCol = Math.Min(cellStart.Col + sliceMaxCells, _pizza.Columns - 1);

            for (var row = cellStart.Row; row <= maxRow; row++)
            {
                for (var col = cellStart.Col; col <= maxCol; col++)
                {
                    var cellCount = GetCellCount(cellStart, row, col);
                    if (cellCount > 1 && cellCount <= sliceMaxCells)
                    {
                        _statistics.SlicesProcessed++;
                        yield return GetSlice(cellStart, _pizza.Cells[row, col]);
                    }
                }
            }
        }

        private bool SliceMeetsRequirements(PizzaSlices currentSlices, PizzaSlice slice)
        {
            if (slice.Area <= 1 || slice.GetCells().Any(cell => currentSlices.ContainsCellId(cell.CellId)))
            {
                return false;
            }

            var groups = slice.GetCells().GroupBy(x => x.Ingredient);
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
                    pizzaSlice.AddCell(_pizza.Cells[row, col]);
                }
            }

            return pizzaSlice;
        }
    }

}
