using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace PizzaChallenge
{
    public class Pizza : ICloneable
    {
        private PizzaRequirements _requirements;
        private PizzaCell[,] _pizzaTable;
        private int _rowIdx;
        private int _colIdx;
        private int _distincIngredients;
        private int _sliceCount;
        private static PizzaPlotter _plotter = new PizzaPlotter();

        public Pizza(PizzaRequirements requirements)
        {
            _requirements = requirements;
            _pizzaTable = new PizzaCell[_requirements.Rows, _requirements.Columns];
            _sliceCount = 0;
        }

        public PizzaCell GetPizzaCell(int row, int col)
        {
            return _pizzaTable[row, col];
        }

        public PizzaCell GetFirstCellNotInSlice()
        {
            for (var row = 0; row < _requirements.Rows; row++)
            {
                for (var col = 0; col < _requirements.Columns; col++)
                {
                    if (_pizzaTable[row, col].Slice == null)
                    {
                        return _pizzaTable[row, col];
                    }
                }
            }
            return null;
        }

        public int Rows => _pizzaTable.GetLength(0);
        public int Columns => _pizzaTable.GetLength(1);

        public Pizza Slice()
        {
            return SliceInternal(this);
        }

        private Pizza SliceInternal(Pizza sourcePizza)
        {
            _plotter.EnqueuePlot(this);
            var slices = GetFilteredSlices();
            if (slices == null || slices.Count == 0)
            {
                if (_pizzaTable.Items().Any(x => x.Slice == null))
                {
                    return null;
                }
                else
                {
                    _plotter.EnqueuePlot(this);
                    return this;
                }
            }
            foreach (var slice in slices)
            {
                var newPizza = sourcePizza.CloneWithNewSlice(slice);
                var solution = newPizza.SliceInternal(newPizza);
                if (solution != null)
                {
                    return solution;
                }
            }

            return null;
        }

        public string PlotSliceSteps()
        {
            return _plotter.DequeuePlot();
        }

        private Pizza CloneWithNewSlice(PizzaSlice slice)
        {
            var returnValue = this.Clone() as Pizza;

            returnValue.AddSlice(slice);

            return returnValue;
        }

        private void AddSlice(PizzaSlice slice)
        {
            this._sliceCount++;
            foreach (var cell in slice.PizzaCells)
            {
                this._pizzaTable[cell.Row, cell.Col].Slice = this._sliceCount;
            }
        }

        public async Task ParseAsync(StreamReader reader)
        {
            _rowIdx = 0;
            _colIdx = 0;
            Logger.Log($"Parsing Pizza");
            var ingredients = new HashSet<char>();
            while (reader.Peek() > 0)
            {
                var row = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(row))
                {
                    continue;
                }
                if (row.Length != _requirements.Columns)
                {
                    throw new System.Exception($"invalid definition '{row}' at row:{_rowIdx}, col: {_colIdx}");
                }
                foreach (var c in row)
                {
                    ingredients.Add(c);
                    AddCell(_rowIdx, _colIdx, c);
                    _colIdx++;
                }

                _rowIdx++;
                _colIdx = 0;
            }
            _distincIngredients = ingredients.Count;
            Logger.Log($"Pizza Parse done");
        }

        public List<PizzaSlice> GetSlices()
        {
            var maxRows = _pizzaTable.GetLength(0);
            var maxCols = _pizzaTable.GetLength(1);

            var returnValue = new List<PizzaSlice>();

            var cellStart = GetFirstCellNotInSlice();

            if(cellStart == null)
            {
                return null;
            }

            //Vertical
            for (var row = cellStart.Row; row < maxRows; row++)
            {
                //Horizontal
                for (var col = cellStart.Col; col < maxCols; col++)
                {
                    if (row == cellStart.Row && col == cellStart.Col)
                    {
                        continue;
                    }
                    var distanceCol = (col - cellStart.Col) + 1;
                    var distanceRow = (row - cellStart.Row) + 1;

                    var sliceArea = distanceCol * distanceRow;

                    if (sliceArea <= _requirements.SliceMaxCells)
                    {
                        returnValue.Add(GetSlice(cellStart, _pizzaTable[row, col]));
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return returnValue.OrderByDescending(x => x.Area).ToList();
        }

        public List<PizzaSlice> GetFilteredSlices()
        {
            var slices = GetSlices();
            if (slices == null){
                return null;
            }

            return slices.
                Where(slice => slice.PizzaCells.Select(x => x.Ingredient).Distinct().Count() >= (_requirements.SliceMinIngredients * _distincIngredients)).ToList();
        }

        private PizzaSlice GetSlice(PizzaCell cellStart, PizzaCell pizzaCell)
        {
            var pizzaSlice = new PizzaSlice();
            for (var row = cellStart.Row; row <= pizzaCell.Row; row++)
            {
                //Horizontal
                for (var col = cellStart.Col; col <= pizzaCell.Col; col++)
                {
                    pizzaSlice.PizzaCells.Add(_pizzaTable[row, col]);
                }
            }

            return pizzaSlice;
        }

        private void AddCell(int row, int col, char ingredient)
        {
            _pizzaTable[row, col] = new PizzaCell(row, col, ingredient);
        }

        public object Clone()
        {
            var returnValue = new Pizza(this._requirements);
            returnValue._sliceCount = this._sliceCount;
            returnValue._rowIdx = this._rowIdx;
            returnValue._colIdx = this._colIdx;
            returnValue._distincIngredients = this._distincIngredients;
            returnValue._pizzaTable = new PizzaCell[this.Rows, this.Columns];

            for(var row = 0; row < this.Rows; row++)
            {
                for (var column = 0; column < this.Columns; column++)
                {
                    var currentCell = this._pizzaTable[row, column];
                    returnValue._pizzaTable[row, column] = 
                        new PizzaCell(row, column, currentCell.Ingredient, currentCell.Slice);
                }
            }

            return returnValue;
        }

        //private void BuildGraph(PizzaNode[,] pizzaNodeArray)
        //{
        //    int rowCount = pizzaNodeArray.GetLength(0);
        //    int colCount = pizzaNodeArray.GetLength(1);
        //    for (var row = 0; row < rowCount; row++)
        //    {
        //        for(var col=0;col< colCount; col++)
        //        {
        //            if (col > 0)
        //            {
        //                pizzaNodeArray[row, col].Left = pizzaNodeArray[row, col-1];
        //            }
        //            if (col < colCount - 1)
        //            {
        //                pizzaNodeArray[row, col].Right = pizzaNodeArray[row, col + 1];
        //            }
        //            if (row > 0)
        //            {
        //                pizzaNodeArray[row, col].Top = pizzaNodeArray[row-1, col];
        //            }
        //            if (row < rowCount-1)
        //            {
        //                pizzaNodeArray[row, col].Bottom = pizzaNodeArray[row + 1, col];
        //            }
        //        }
        //    }
        //}
    }
}
