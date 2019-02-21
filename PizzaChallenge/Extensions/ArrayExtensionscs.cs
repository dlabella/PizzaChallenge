using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaChallenge.Extensions
{
    public static class ArrayExtensionscs
    {
        public static IEnumerable<T> All<T>(this T[,] array)
        {
            var maxRows = array.GetLength(0);
            var maxCols = array.GetLength(1);
            for (var row=0; row < maxRows; row++)
            {
                for(var col = 0; col < maxCols; col++)
                {
                    yield return array[row, col];
                }
            }
        }
    }
}
