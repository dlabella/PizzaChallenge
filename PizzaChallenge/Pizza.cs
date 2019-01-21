using System.IO;
using System.Threading.Tasks;

namespace PizzaChallenge
{
    public class Pizza
    {
        private PizzaRequirements _requirements;
        private PizzaNode[,] _pizzaTable;
        private int _rowIdx;
        private int _colIdx;
        public Pizza(PizzaRequirements requirements)
        {
            _requirements = requirements;
            _pizzaTable = new PizzaNode[_requirements.Rows, _requirements.Columns];

        }
        public PizzaCell GetPizzaCell(int row, int col)
        {
            return _pizzaTable[row,col].PizzaCell;
        }

        public PizzaNode GetRootPizzaNode()
        {
            return _pizzaTable[0,0];
        }

        public async Task ParseAsync(StreamReader reader)
        {
            _rowIdx=0;
            _colIdx=0;
            Logger.Log($"Parsing Pizza");
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
                    AddCell(_rowIdx, _colIdx, c);
                    _colIdx++;
                }
                _rowIdx++;
                _colIdx=0;
            }
            Logger.Log($"Pizza Parse done");
        }


        private void AddCell(int row, int col, char ingredient)
        {
            _pizzaTable[row,col] = new PizzaNode(row, col, ingredient);
        }

        private void BuildGraph(PizzaNode[,] pizzaNodeArray)
        {
            int rowCount = pizzaNodeArray.GetLength(0);
            int colCount = pizzaNodeArray.GetLength(1);
            for (var row = 0; row < rowCount; row++)
            {
                for(var col=0;col< colCount; col++)
                {
                    if (col > 0)
                    {
                        pizzaNodeArray[row, col].Left = pizzaNodeArray[row, col-1];
                    }
                    if (col < colCount - 1)
                    {
                        pizzaNodeArray[row, col].Right = pizzaNodeArray[row, col + 1];
                    }
                    if (row > 0)
                    {
                        pizzaNodeArray[row, col].Top = pizzaNodeArray[row-1, col];
                    }
                    if (row < rowCount-1)
                    {
                        pizzaNodeArray[row, col].Bottom = pizzaNodeArray[row + 1, col];
                    }
                }
            }
        }
    }
}
