using System.Collections.Generic;
using System.Linq;

namespace PizzaChallenge
{
    public class SliceSolver
    {
        PizzaDefinition _definition;

        public SliceSolver(PizzaDefinition definition)
        {
            _definition = definition;
        }
        public void Solve()
        {
            List<PizzaSlice> _pizzaSlices = new List<PizzaSlice>();
            int startCol=0;
            int startRow=0;
            for (var i = _definition.Requirements.SliceMaxCells; i > startCol; i--)
            { 
                for (var j = startRow; j<_definition.Requirements.SliceMaxCells;j++)
                {
                    var slice = BuildSlice(startRow, startCol, j, i);
                    if (slice.PizzaCells.GroupBy(x => x.Ingredient).Count() > 1)
                    {
                        _pizzaSlices.Add(slice);
                        break;
                    }
                }
            }
        }

        private PizzaSlice BuildSlice(int startRow, int startCol, int endRow, int endCol)
        {
            PizzaSlice slice=new PizzaSlice();
            for(var i = startRow; i < endRow; i++)
            {
                for(var j = startCol; j < endCol; j++)
                {
                    slice.PizzaCells.Add(_definition.Pizza.GetPizzaCell(i,j));
                }
            }
            return slice;
        }
    }
}
