using System.Collections.Generic;
using System.Linq;

namespace PizzaChallenge
{
    public class PizzaSlicer
    {
        private readonly PizzaDefinition _definition;
        private readonly PizzaRequirements _requirements;
        private readonly Pizza _pizza;

        public PizzaSlicer(PizzaDefinition definition)
        {
            _definition = definition;
            _pizza = _definition.Pizza;
            _requirements = _definition.Requirements;
        }

        public Pizza Slice()
        {
            var bestSolution = _pizza.Clone() as Pizza;
            var result = SliceInternal(_pizza, ref bestSolution);
            return result??bestSolution;
        }

        private Pizza SliceInternal(Pizza sourcePizza, ref Pizza bestSolution)
        {
            var slices = GetFilteredSlices(sourcePizza, _requirements.SliceMaxCells);
            if (slices == null || slices.Count == 0)
            {
                if (sourcePizza.Cells.Items().Any(x => x.Slice == null))
                {
                    var currentEmptyCount = sourcePizza.Cells.Items().Count(x => x.Slice == null);
                    var bestEmptyCount = bestSolution.Cells.Items().Count(x => x.Slice == null);
                    if (currentEmptyCount < bestEmptyCount)
                    {
                        bestSolution = sourcePizza;
                    }
                    return null;
                }
                else
                {
                    return sourcePizza;
                }
            }
            foreach (var slice in slices)
            {
                var newPizza = sourcePizza.Clone() as Pizza;
                newPizza.AddSlice(slice);
                var solution = SliceInternal(newPizza, ref bestSolution);
                if (solution != null)
                {
                    return solution;
                }
            }

            return null;
        }

        private List<PizzaSlice> GetFilteredSlices(Pizza pizza, int sliceMaxCells)
        {
            var slices = GetSlices(pizza, sliceMaxCells);
            if (slices == null)
            {
                return null;
            }

            return slices.
                Where(slice => slice.PizzaCells.Select(x => x.Ingredient).Distinct().Count() >= (_requirements.SliceMinIngredients * _pizza.DistinctIngredientsCount)).ToList();
        }
        
        private List<PizzaSlice> GetSlices(Pizza pizza, int sliceMaxCells)
        {
            var returnValue = new List<PizzaSlice>();

            var cellStart = pizza.GetFirstCellNotInSlice();

            if (cellStart == null)
            {
                return null;
            }

            for (var row = cellStart.Row; row < pizza.Rows; row++)
            {
                for (var col = cellStart.Col; col < pizza.Columns; col++)
                {
                    var cellCount = GetCellCount(cellStart, row, col);
                    if (cellCount > 1 && cellCount <= sliceMaxCells)
                    {
                        returnValue.Add(GetSlice(cellStart, pizza.Cells[row, col]));
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return returnValue.OrderByDescending(x => x.Area).ToList();
        }

        private int GetCellCount(PizzaCell cellStart, int row, int col)
        {
            var distanceCol = (col - cellStart.Col) + 1;
            var distanceRow = (row - cellStart.Row) + 1;

            return distanceCol * distanceRow;
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
