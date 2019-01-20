using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PizzaChallenge
{
    public class SliceSolver
    {
        PizzaDefinition _definition;
        
        public SliceSolver(PizzaDefinition definition)
        {
            _definition=definition;
        }
        public Solve()
        {
            var array = _definition.Pizza.GetPizzaArray();
            PizzaSlice currentSlice=new PizzaSlice();
            List<PizzaSlice> slices =new List<PizzaSlice>();
            currentSlice.StartCol=0;
            currentSlice.StartRow=0;
            
        }
    }
}
