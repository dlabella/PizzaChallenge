using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaChallenge
{
    public class PizzaOrder
    {
        private Pizza _pizza;
        private PizzaRequirements _requirements;
        public PizzaOrder()
        {

            _requirements = new PizzaRequirements();
        }

        public PizzaOrder(PizzaRequirements requirements, Pizza pizza)
        {

            _requirements = requirements;
            _pizza = pizza;
        }

        public Pizza Pizza
        {
            get { return _pizza; }
        }

        public PizzaRequirements Requirements
        {
            get { return _requirements; }
        }

        public async Task WriteResult(string file)
        {
            await WriteResult(_pizza, file);
        }

        public async Task WriteResult(Pizza pizza, string file)
        {
            StringBuilder sb = new StringBuilder();
            var slices = pizza.Cells.Items().GroupBy(x => x.Slice).OrderBy(x => x.Key);
            sb.AppendLine($"{slices.Count()}");
            foreach (var slice in slices)
            {
                var cellMin = slice.Min();
                var cellMax = slice.Max();
                sb.AppendLine($"{cellMin.Row} {cellMin.Col} {cellMax.Row} {cellMax.Col}");
            }
            await File.WriteAllTextAsync(file,sb.ToString());
        }


        public async Task ReadRequest(string file)
        {
            Logger.Log($"Parsing File {file}");
            using (StreamReader rdr = new StreamReader(file))
            {
                if (rdr.Peek() > 0)
                {
                    _requirements.Parse(await rdr.ReadLineAsync());
                }
                _pizza = new Pizza(_requirements);
                await _pizza.ParseAsync(rdr);
            }
        }
    }
}
