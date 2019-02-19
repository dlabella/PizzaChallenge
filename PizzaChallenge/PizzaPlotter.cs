using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PizzaChallenge
{
    public class PizzaPlotter
    {
        public Dictionary<string, string> _colors;
        public Dictionary<string, Color> _colorsRgb;
        private Random _random;
        private readonly StringBuilder _queuedPlot;

        public PizzaPlotter()
        {
            _random = new Random();
            _queuedPlot = new StringBuilder();
            _colors = new Dictionary<string, string>();
            _colorsRgb = new Dictionary<string, Color>();
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

        public string Plot(PizzaSlices slices)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<body>");
            PlotPizzaSlices(html, slices);
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        private void PlotPizzaTable(StringBuilder sb, Pizza pizza)
        {
            sb.AppendLine("<table>");
            for (var row = 0; row < pizza.Rows; row++)
            {
                PlotPizzaRow(sb, row, pizza);
            }
            sb.AppendLine("</table>");
        }

        private void PlotPizzaSlices(StringBuilder sb, PizzaSlices slices)
        {
            sb.AppendLine("<table>");
            var rows = slices.Pizza.Rows;
            var cols = slices.Pizza.Columns;

            for (var row = 0; row < rows; row++)
            {
                sb.Append("<tr>");
                for (var col = 0; col < cols; col++)
                {
                    var currentCell = slices.Pizza.Cells[row, col];
                    if (currentCell.Slice != null && currentCell.Slice>=0){
                        var color = GetColor(currentCell.Slice.Value.ToString());
                        PlotPizzaCell(sb, currentCell, color);
                    }
                    else
                    {
                        PlotPizzaCell(sb, currentCell, string.Empty);
                    }
                }
                sb.Append("</tr>");
            }

            sb.AppendLine("</table>");
        }

        private void PlotPizzaRow(StringBuilder sb, int currentRow, Pizza pizza)
        {
            sb.AppendLine("<tr>");
            for (var col = 0; col < pizza.Columns; col++)
            {
                var cell = pizza.Cells[currentRow, col];
                var color = GetColor(cell.Slice.ToString());
                PlotPizzaCell(sb, cell, color);
            }
            sb.AppendLine("</tr>");
        }

        private void PlotPizzaCell(StringBuilder sb, PizzaCell cell, string color)
        {

            sb.AppendLine($"<td bgcolor=rgb({color})>{cell.Ingredient}</td>");
        }

        private string GetColor(string sliceId)
        {
            string rgb = "";
            if (string.IsNullOrEmpty(sliceId) || sliceId=="-1")
            {
                rgb = "255,255,255";
            }
            else if (_colors.ContainsKey(sliceId ?? "0"))
            {
                rgb = _colors[sliceId ?? "0"];
            }
            else
            {
                var r = _random.Next(256);
                var g = _random.Next(256);
                var b = _random.Next(256);
                rgb = $"{r},{g},{b}";
                _colors.Add(sliceId ?? "0", rgb);
            }
            return rgb;
        }
    }
}
