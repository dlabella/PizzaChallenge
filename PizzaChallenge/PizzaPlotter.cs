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
        private const string defaultColorId = "_"; 
        public PizzaPlotter()
        {
            _random = new Random();
            _queuedPlot = new StringBuilder();
            _colors = new Dictionary<string, string>();
            _colorsRgb = new Dictionary<string, Color>();
            _colors.Add(defaultColorId, "255,255,255");
            _colorsRgb.Add(defaultColorId, Color.White);
        }

        //public string Plot(PizzaOrder pizzaDefinition)
        //{
        //    StringBuilder html = new StringBuilder();
        //    html.AppendLine("<html>");
        //    html.AppendLine("<body>");
        //    PlotPizzaDefinition(html, pizzaDefinition);
        //    html.AppendLine("<p>");
        //    PlotPizzaTable(html, pizzaDefinition.Pizza);
        //    html.AppendLine("</body>");
        //    html.AppendLine("</html>");
        //    return html.ToString();
        //}

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

        //public string Plot(PizzaSlice slice)
        //{
        //    StringBuilder html = new StringBuilder();
        //    html.AppendLine("<html>");
        //    html.AppendLine("<body>");
        //    PlotPizzaSlice(html, slice.PizzaCells);
        //    html.AppendLine("</body>");
        //    html.AppendLine("</html>");
        //    return html.ToString();
        //}

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

        public Bitmap PlotToBitmap(PizzaSlices slices)
        {
            var rows = slices.Pizza.Rows;
            var cols = slices.Pizza.Columns;
            Bitmap bmp = new Bitmap(rows, cols);
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    var currentCell = slices.Pizza.Cells[row, col];
                    if (slices.ContainsCellId(currentCell.CellId))
                    {
                        var slice = slices.GetSliceFromCell(currentCell);
                        var color = GetColorRgb(slice.SliceId);
                        bmp.SetPixel(row, col, color);
                    }
                    else
                    {
                        bmp.SetPixel(row, col, Color.White);
                    }
                }
            }
            return bmp;
        }



        //public string Plot(IEnumerable<PizzaSlice> slices)
        //{

        //    StringBuilder html = new StringBuilder();
        //    html.AppendLine("<html>");
        //    html.AppendLine("<body>");
        //    foreach (var slice in slices)
        //    {
        //        PlotPizzaSlice(html, slice.PizzaCells);
        //    }
        //    html.AppendLine("</body>");
        //    html.AppendLine("</html>");
        //    return html.ToString();
        //}

        //public string Plot(IEnumerable<PizzaCell> cells)
        //{
        //    StringBuilder html = new StringBuilder();
        //    html.AppendLine("<html>");
        //    html.AppendLine("<body>");
        //    PlotPizzaSlice(html, cells);
        //    html.AppendLine("</body>");
        //    html.AppendLine("</html>");
        //    return html.ToString();
        //}

        //public void EnqueuePlot(Pizza pizza)
        //{
        //    if (pizza != null)
        //    {
        //        PlotPizzaTable(_queuedPlot, pizza);
        //        _queuedPlot.AppendLine("<p>");
        //        _queuedPlot.AppendLine("<p>");
        //    }
        //}

        //public string DequeuePlot()
        //{
        //    var html = new StringBuilder();
        //    html.AppendLine("<html>");
        //    html.AppendLine("<body>");
        //    html.AppendLine(_queuedPlot.ToString());
        //    html.AppendLine("</body>");
        //    html.AppendLine("</html>");
        //    return html.ToString();
        //}

        //private void PlotPizzaDefinition(StringBuilder sb, PizzaOrder definition)
        //{
        //    sb.AppendLine("****** SLICE DEFINITION *******<p>");
        //    sb.AppendLine($"Max Cells:       <b>{definition.Requirements.SliceMaxCells}</b><p>");
        //    sb.AppendLine($"Min Ingredients: <b>{definition.Requirements.SliceMinIngredients}</b><p>");
        //    sb.AppendLine("*******************************<p>");

        //}

        private void PlotPizzaTable(StringBuilder sb, Pizza pizza)
        {
            sb.AppendLine("<table>");
            for (var row = 0; row < pizza.Rows; row++)
            {
                PlotPizzaRow(sb, row, pizza);
            }
            sb.AppendLine("</table>");
        }

        //private void PlotPizzaSlice(StringBuilder sb, IEnumerable<PizzaCell> cells)
        //{
        //    sb.AppendLine("<table>");
        //    var minRow = cells.Min(x => x.Row);
        //    var maxRow = cells.Max(x => x.Row);
        //    for (var row = minRow; row <= maxRow; row++)
        //    {
        //        PlotPizzaSliceRow(sb, row, cells);
        //    }
        //    sb.AppendLine("</table>");
        //}

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
                    if (slices.ContainsCellId(currentCell.CellId))
                    {
                        var slice = slices.GetSliceFromCell(currentCell);
                        var color = GetColor(slice.SliceId);
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

        //private void PlotPizzaSliceRow(StringBuilder sb, int currentRow, IEnumerable<PizzaCell> cells)
        //{
        //    sb.AppendLine("<tr>");
        //    var minCol = cells.Min(x => x.Col);
        //    var maxCol = cells.Max(x => x.Col);
        //    for (var col = minCol; col <= maxCol; col++)
        //    {
        //        var cell = cells.First(x => x.Col == col && x.Row == currentRow);
        //        if (cell != null)
        //        {
        //            PlotPizzaCell(sb, cell);
        //        }
        //    }
        //    sb.AppendLine("</tr>");
        //}
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
            if (string.IsNullOrEmpty(sliceId))
            {
                return _colors[defaultColorId];
            }
            else if (_colors.ContainsKey(sliceId))
            {
                return _colors[sliceId];
            }
            else
            {
                var r = _random.Next(256);
                var g = _random.Next(256);
                var b = _random.Next(256);
                rgb = $"{r},{g},{b}";
                _colors.Add(sliceId, rgb);
                return rgb;
            }
        }

        private Color GetColorRgb(string sliceId)
        {
            Color rgb;
            if (string.IsNullOrEmpty(sliceId))
            {
                return _colorsRgb[defaultColorId];
            }
            else if (_colorsRgb.ContainsKey(sliceId))
            {
                return _colorsRgb[sliceId];
            }
            else
            {
                var r = _random.Next(256);
                var g = _random.Next(256);
                var b = _random.Next(256);
                rgb = Color.FromArgb(r, g, b);
                _colorsRgb.Add(sliceId, rgb);
                return rgb;
            }
        }
    }
}
