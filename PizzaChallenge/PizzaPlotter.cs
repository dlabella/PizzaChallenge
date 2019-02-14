using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            GenearateColors();
        }
        private void GenearateColors()
        {
            int i = 1;
            _colors.Add("0", "rgb(255,255,255)");
            for (var j = 0; j <= 50; j++)
            {
                for (var k = 0; k < 100; k++)
                {
                    var r = _random.Next(100) * 2;
                    var g = _random.Next(100) * 2;
                    var b = _random.Next(100) * 2;
                    _colors.Add(i.ToString(), $"rgb({r},{g},{b})");
                    i++;
                }
            }
        }
        public void SetStyles(StringBuilder sb)
        {
            sb.AppendLine("<style>");
            int i = 1;
            for (var j = 0; j <= 50; j++)
            {
                for (var k = 0; k < 100; k++)
                {
                    sb.Append($".c-{i} {{background-color:{_colors[i.ToString()]});}} ");
                    sb.Append($".c-{i}-d {{background-color:{_colors[i.ToString()]}); border-style:dotted;}} ");
                    i++;
                }
            }
            sb.AppendLine("</style>");
        }

        public string Plot(Pizza pizza)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            SetStyles(html);
            html.AppendLine("<body>");
            PlotPizzaTable(html, pizza);
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        public string Plot(PizzaSlices slices)
        {
            AssignSliceDate(slices);

            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            //SetStyles(html);
            html.AppendLine("<body>");
            PlotPizzaSlices(html, slices);
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            UnassignSliceDate(slices);

            return html.ToString();
        }

        private void AssignSliceDate(PizzaSlices slices)
        {
            foreach (var slice in slices.Slices)
            {
                foreach (var cell in slice.GetCells())
                {
                    cell.Slice = slice.SliceNum;
                }
            }
        }

        private void UnassignSliceDate(PizzaSlices slices)
        {
            foreach (var slice in slices.Slices)
            {
                foreach (var cell in slice.GetCells())
                {
                    cell.Slice = null;
                }
            }
        }

        public void Plot(PizzaSlices slices, string file)
        {

            AssignSliceDate(slices);

            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            SetStyles(html);
            html.AppendLine("<body>");
            PlotPizzaSlices(html, slices);
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            File.WriteAllText(file, html.ToString());

            UnassignSliceDate(slices);
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
                    PlotPizzaCell(sb, currentCell, currentCell.Slice ?? 0);
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
                //PlotPizzaCell(sb, cell, $"c-{cell.Slice}");
                PlotPizzaCell(sb, cell, cell.Slice ?? 0);
            }
            sb.AppendLine("</tr>");
        }

        private void PlotPizzaCell(StringBuilder sb, PizzaCell cell, String classStyle)
        {
            sb.AppendLine($"<td><div class=\"{classStyle}\">{cell.Ingredient}</div></td>");
        }
        private void PlotPizzaCell(StringBuilder sb, PizzaCell cell, int slice)
        {
            sb.AppendLine($"<td><div style=\"background-color={_colors[slice.ToString()]}\">{cell.Ingredient}</div></td>");
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
