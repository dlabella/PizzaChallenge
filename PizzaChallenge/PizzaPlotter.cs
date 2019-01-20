using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaChallenge
{
    public class PizzaPlotter
    {
        public PizzaPlotter()
        {

        }
        public string Plot(PizzaDefinition pizzaDefinition)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<body>");
            PlotPizzaDefinition(html, pizzaDefinition);
            html.AppendLine("<p>");
            PlotPizzaTable(html, pizzaDefinition);
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }
        private void PlotPizzaDefinition(StringBuilder sb, PizzaDefinition definition)
        {
            sb.AppendLine("****** SLICE DEFINITION *******<p>");
            sb.AppendLine($"Max Cells:       <b>{definition.Requirements.SliceMaxCells}</b><p>");
            sb.AppendLine($"Min Ingredients: <b>{definition.Requirements.SliceMinIngredients}</b><p>");
            sb.AppendLine("*******************************<p>");

        }
        private void PlotPizzaTable(StringBuilder sb, PizzaDefinition definition)
        {
            sb.AppendLine("<table>");
            for(var row=0;row<definition.Requirements.Rows; row++)
            {
                PlotPizzaRow(sb,row, definition);
            }
            sb.AppendLine("</table>");
        }
        private void PlotPizzaRow(StringBuilder sb,int currrentRow, PizzaDefinition definition)
        {
            sb.AppendLine("<tr>");
            for (var col = 0; col < definition.Requirements.Columns; col++)
            {
                PlotPizzaCell(sb, definition.Pizza.GetPizzaCell(currrentRow, col));
            }
            sb.AppendLine("</tr>");
        }
        private void PlotPizzaCell(StringBuilder sb, PizzaCell cell)
        {
            sb.AppendLine($"<td>{cell.Ingredient}</td>");
        }
    }
}
