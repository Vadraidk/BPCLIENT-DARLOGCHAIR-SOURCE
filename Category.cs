using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules
{
    class Category
    {
        public string name;

        public Category(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return this.name.ToString();
        }
    }
}
