using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaChallenge
{
    public class StackItem
    {
        PizzaCell StartCell { get;set;}
        List<PizzaSlice> Slices { get;set;}
        public StackItem()
        {
            Slices=new List<PizzaSlice>();
        }
        public StackItem(PizzaCell startCell, List<PizzaSlice> slices)
        {
            StartCell = startCell;
            Slices = slices;
        }
    }
}
