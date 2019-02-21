using PizzaChallenge.Extensions;
using PizzaChallenge.Services;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaChallenge.Entities
{
    public class PizzaOrder
    {
        public PizzaOrder()
        {

            Requirements = new PizzaRequirements();
        }

        public PizzaOrder(PizzaRequirements requirements, Pizza pizza)
        {

            Requirements = requirements;
            Pizza = pizza;
        }

        public Pizza Pizza { get; internal set; }

        public PizzaRequirements Requirements{get;internal set; }

        public async Task WriteResult(string file)
        {
            await WriteResult(Pizza, file);
        }

        public async Task WriteResult(Pizza pizza, string file)
        {
            StringBuilder sb = new StringBuilder();
            var slices = pizza.Cells.All().Where(x => x.Slice != -1 && x.Slice != null).GroupBy(x => x.Slice);
            sb.AppendLine($"{slices.Count()}");
            foreach (var slice in slices)
            {
                var cellMin = slice.Min();
                var cellMax = slice.Max();
                sb.AppendLine($"{cellMin.Row} {cellMin.Col} {cellMax.Row} {cellMax.Col}");
            }
            var finfo = new FileInfo(file);
            if (!finfo.Directory.Exists)
            {
                finfo.Directory.Create();
            }
            await File.WriteAllTextAsync(file, sb.ToString());
        }

        public async Task ReadRequest(string file)
        {
            Logger.Log($"Parsing File {file}");
            using (StreamReader rdr = new StreamReader(file))
            {
                if (rdr.Peek() > 0)
                {
                    Requirements.Parse(await rdr.ReadLineAsync());
                }
                Pizza = new Pizza(Requirements);
                await Pizza.ParseAsync(rdr);
            }
        }
    }
}
