using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaChallenge
{
    public class PizzaNode
    {
        public PizzaNode(int row, int col, char ingredient)
        {
            PizzaCell=new PizzaCell(row,col,ingredient);
        }
        public PizzaCell PizzaCell { get;}
        public PizzaNode Top { get;set;}
        public PizzaNode Left { get;set;}
        public PizzaNode Bottom { get;set;}
        public PizzaNode Right { get;set;}
    }
}
