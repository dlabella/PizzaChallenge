using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PizzaChallenge
{
    public class PizzaPlotter
    {
        public Dictionary<int, string> _colors;
        private Random _random;
        private StringBuilder _queuedPlot;
        public PizzaPlotter()
        {
            _random = new Random();
            _queuedPlot = new StringBuilder();
            _colors = new Dictionary<int, string>();
        }

        public string Plot(PizzaOrder pizzaDefinition)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<body>");
            PlotPizzaDefinition(html, pizzaDefinition);
            html.AppendLine("<p>");
            PlotPizzaTable(html, pizzaDefinition.Pizza);
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        public string Plot(Pizza pizza)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<body>");
            PlotPizzaTable(html, pizza);
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }
        public string Plot(PizzaSlice slice)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<body>");
            PlotPizzaSlice(html, slice.PizzaCells);
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        public string Plot(IEnumerable<PizzaCell> cells)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<body>");
            PlotPizzaSlice(html, cells);
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        public void EnqueuePlot(Pizza pizza)
        {
            if (pizza != null)
            {
                PlotPizzaTable(_queuedPlot, pizza);
                _queuedPlot.AppendLine("<p>");
                _queuedPlot.AppendLine("<p>");
            }
        }

        public string DequeuePlot()
        {
            var html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<body>");
            html.AppendLine(_queuedPlot.ToString());
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        private void PlotPizzaDefinition(StringBuilder sb, PizzaOrder definition)
        {
            sb.AppendLine("****** SLICE DEFINITION *******<p>");
            sb.AppendLine($"Max Cells:       <b>{definition.Requirements.SliceMaxCells}</b><p>");
            sb.AppendLine($"Min Ingredients: <b>{definition.Requirements.SliceMinIngredients}</b><p>");
            sb.AppendLine("*******************************<p>");

        }

        private void PlotPizzaTable(StringBuilder sb, Pizza pizza)
        {
            sb.AppendLine("<table>");
            for(var row=0;row< pizza.Rows; row++)
            {
                PlotPizzaRow(sb,row, pizza);
            }
            sb.AppendLine("</table>");
        }

        private void PlotPizzaSlice(StringBuilder sb, IEnumerable<PizzaCell> cells)
        {
            sb.AppendLine("<table>");
            var minRow = cells.Min(x=>x.Row);
            var maxRow = cells.Max(x => x.Row);
            for (var row = minRow; row <= maxRow; row++)
            {
                PlotPizzaSliceRow(sb, row, cells);
            }
            sb.AppendLine("</table>");
        }

        private void PlotPizzaSliceRow(StringBuilder sb,int currentRow, IEnumerable<PizzaCell> cells)
        {
            sb.AppendLine("<tr>");
            var minCol = cells.Min(x => x.Col);
            var maxCol = cells.Max(x => x.Col);
            for (var col = minCol; col <= maxCol; col++)
            {
                var cell = cells.First(x=>x.Col==col && x.Row==currentRow );
                if (cell != null) {
                    PlotPizzaCell(sb, cell);
                }
            }
            sb.AppendLine("</tr>");
        }
        private void PlotPizzaRow(StringBuilder sb,int currentRow, Pizza pizza)
        {
            sb.AppendLine("<tr>");
            for (var col = 0; col < pizza.Columns; col++)
            {
                PlotPizzaCell(sb, pizza.Cells[currentRow, col]);
            }
            sb.AppendLine("</tr>");
        }

        private void PlotPizzaCell(StringBuilder sb, PizzaCell cell)
        {
            string rgb = "";
            if (cell.Slice == null)
            {
                rgb = "255,255,255";
            }
            else if (_colors.ContainsKey(cell.Slice??0))
            {
                rgb = _colors[cell.Slice??0];
            }
            else
            {
                var r = _random.Next(256);
                var g = _random.Next(256);
                var b = _random.Next(256);
                rgb = $"{r},{g},{b}";
                _colors.Add(cell.Slice??0, rgb);
            }
            sb.AppendLine($"<td bgcolor=rgb({rgb})>{cell.Ingredient}</td>");
        }
    }
}
