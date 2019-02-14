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
        private readonly HashSet<string> _invalidCells;
        private List<Stack<PizzaSlice>> _sliceStack;

        public PizzaSlicer(PizzaOrder definition)
        {
            _definition = definition;
            _pizza = _definition.Pizza;
            _requirements = _definition.Requirements;
            _statistics = new SliceStatistics();
            _statisticsBuff = new SliceStatistics();
            _plotter = new PizzaPlotter();

            _invalidCells = new HashSet<string>();
            _sliceStack = new List<Stack<PizzaSlice>>();
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

            var slices = new PizzaSlices(_pizza);
            var result = SliceInternal(slices);

            foreach (var slice in slices.Slices)
            {
                foreach (var cell in slice.GetCells())
                {
                    cell.Slice = slice.SliceNum;
                }
            }

            return _pizza;
        }


        private bool SliceInternal(PizzaSlices slices, PizzaCell parentCell = null)
        {
            int sliceIdx=0;
            PizzaSlice currentSlice = null;
            do
            {
                var cellStart = GetFirstCellNotInSlice();
                if (cellStart == null)
                {
                    //
                }

                var validSlices = GetValidSlices(slices, cellStart).OrderByDescending(x => x.Area);
                if (validSlices.Any())
                {
                    var items = new Stack<PizzaSlice>(validSlices);
                    _sliceStack.Add(items);
                }

                if (_sliceStack[_sliceStack.Count-1].TryPop(out var slice))
                {
                    sliceIdx++;
                    slice.SliceNum=sliceIdx;
                    currentSlice = slice;
                    slices.AddSlice(slice);
                    foreach (var cellToSet in slice.GetCells())
                    {
                        cellToSet.Slice = sliceIdx;
                    }
                    if (slices.Area == _pizza.Area)
                    {
                        return true;
                    }
                }
                else
                {
                    foreach (var sliceToClear in _sliceStack[_sliceStack.Count-1])
                    {
                        foreach(var cellToClear in sliceToClear.GetCells())
                        {
                            cellToClear.Slice=null;
                        }
                    }
                    _sliceStack.RemoveAt(_sliceStack.Count-1);
                }
            } while (_sliceStack.Count>0);
            return true;
        }

        private PizzaCell GetFirstCellNotInSlice()
        {
            return _pizza.Cells.Items().FirstOrDefault(x => x.Slice==null);
        }

        private IEnumerable<PizzaSlice> GetValidSlices(PizzaSlices slices, PizzaCell cellStart)
        {
            var availableSlices = GetAvailableSlices(cellStart);
            foreach (var slice in availableSlices)
            {
                if (SliceMeetsRequirements(slices, slice))
                {
                    yield return slice;
                }
            }
        }

        private IEnumerable<PizzaSlice> GetAvailableSlices(PizzaCell cellStart)
        {
            var sliceMaxCells = _requirements.SliceMaxCells;
            var maxRow = Math.Min(cellStart.Row + sliceMaxCells, _pizza.Rows - 1);
            var minRow = Math.Max(cellStart.Row - sliceMaxCells, 0);
            var maxCol = Math.Min(cellStart.Col + sliceMaxCells, _pizza.Columns - 1);
            var minCol = Math.Max(cellStart.Col - sliceMaxCells, 0);

            for (var row = cellStart.Row - minRow; row <= maxRow; row++)
            {
                for (var col = cellStart.Col - minCol; col <= maxCol; col++)
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
