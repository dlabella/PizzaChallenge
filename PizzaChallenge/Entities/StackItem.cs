using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaChallenge.Entities
{
    public class StackItem
    {
        public List<PizzaSlice> Slices { get;set;}
        public StackItem()
        {
            Slices=new List<PizzaSlice>();
        }
        public StackItem(List<PizzaSlice> slices)
        {
            Slices = slices;
        }
    }
}
