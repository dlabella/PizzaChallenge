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
        private int _rowIdx;
        private int _colIdx;
        private int _area;

        public Pizza(PizzaRequirements requirements)
        {
            _requirements = requirements;
            Cells = new PizzaCell[_requirements.Rows, _requirements.Columns];
            _area=_requirements.Rows*_requirements.Columns;
        }
        public int Area=>_area;
        public int DistinctIngredientsCount { get; private set; }

        public PizzaCell[,] Cells { get; private set; }

        public int Rows => Cells.GetLength(0);
        public int Columns => Cells.GetLength(1);

        //public void AddSlice(PizzaSlice slice)
        //{
        //    this._sliceCount++;
        //    foreach (var cell in slice.PizzaCells)
        //    {
        //        this.Cells[cell.Row, cell.Col].Slice = this._sliceCount;
        //    }
        //}

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
            DistinctIngredientsCount = ingredients.Count;
            Logger.Log($"Pizza Parse done");
        }

        private void AddCell(int row, int col, char ingredient)
        {
            Cells[row, col] = new PizzaCell(row, col, ingredient);
        }
        //public PizzaCell GetFirstCellNotInSlice()
        //{
        //    for (var row = 0; row < Rows; row++)
        //    {
        //        for (var col = 0; col < Columns; col++)
        //        {
        //            if (Cells[row, col].Slice == null)
        //            {
        //                return Cells[row, col];
        //            }
        //        }
        //    }
        //    return null;
        //}
        public object Clone()
        {
            var returnValue = new Pizza(this._requirements);
            returnValue._rowIdx = this._rowIdx;
            returnValue._colIdx = this._colIdx;
            returnValue.DistinctIngredientsCount = this.DistinctIngredientsCount;
            returnValue.Cells = new PizzaCell[this.Rows, this.Columns];

            for(var row = 0; row < this.Rows; row++)
            {
                for (var column = 0; column < this.Columns; column++)
                {
                    var currentCell = this.Cells[row, column];
                    returnValue.Cells[row, column] = 
                        new PizzaCell(row, column, currentCell.Ingredient, currentCell.Slice);
                }
            }

            return returnValue;
        }
    }
}
