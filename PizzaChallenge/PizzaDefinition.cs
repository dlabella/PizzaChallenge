using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PizzaChallenge
{
    public class PizzaDefinition
    {
        private Pizza _pizza;
        private PizzaRequirements _requirements;
        public PizzaDefinition()
        {
            
            _requirements=new PizzaRequirements();
        }

        public Pizza Pizza
        {
            get { return _pizza;}
        }

        public PizzaRequirements Requirements
        {
            get { return _requirements;}
        }

        public async Task Parse(string file)
        {
            Logger.Log($"Parsing File {file}");
            using(StreamReader rdr = new StreamReader(file))
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
