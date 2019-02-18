using System.Linq;

namespace PizzaChallenge
{
    public class ResultValidator
    {
        private readonly PizzaRequirements _requirements;
        public ResultValidator(PizzaRequirements requirements)
        {
            _requirements = requirements;
        }

        public bool Validate(Pizza pizza)
        {
            var slices = pizza.Cells.Items().Where(x=>x.Slice!=-1 && x.Slice!=null).GroupBy(x => x.Slice);
            foreach (var slice in slices)
            {
                var t = 0;
                var m = 0;
                var c = 0;
                foreach (var cell in slice)
                {
                    if (cell.Ingredient == 'T')
                    {
                        t++;
                    }
                    if (cell.Ingredient == 'M')
                    {
                        m++;
                    }
                    c++;
                }
                if ( c > _requirements.SliceMaxCells || t < _requirements.SliceMinIngredients || m < _requirements.SliceMinIngredients)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
