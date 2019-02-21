using PizzaChallenge.Services;

namespace PizzaChallenge.Entities
{
    public class PizzaRequirements
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int SliceMinIngredients { get; set; }
        public int SliceMaxCells { get; set; }

        public void Parse(string requirements)
        {
            Rows = -1;
            Columns = -1;
            SliceMinIngredients = -1;
            SliceMaxCells = -1;

            var data = requirements.Trim().Split(new char[] { ' ' });
            Logger.Log($"Parsing Requirements {data}");
            if (int.TryParse(data[0], out var parsedRows))
            {
                Rows = parsedRows;
            }
            if (int.TryParse(data[1], out var parsedColumns))
            {
                Columns = parsedColumns;
            }
            if (int.TryParse(data[2], out var parsedSliceMinIngredients))
            {
                SliceMinIngredients = parsedSliceMinIngredients;
            }
            if (int.TryParse(data[3], out var parsedSliceMaxCells))
            {
                SliceMaxCells = parsedSliceMaxCells;
            }
            if (Rows<0 || Columns<0 || SliceMinIngredients<0 || SliceMaxCells < 0)
            {
                throw new System.Exception("Invalid Requirements");
            }
            Logger.Log($"Parsed: Rows {Rows}, Columns {Columns}, SliceMinIngredients {SliceMinIngredients}, SliceMaxCells {SliceMaxCells}");
        }
    }
}