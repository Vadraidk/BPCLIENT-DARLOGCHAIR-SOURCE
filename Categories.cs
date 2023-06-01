using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules
{
    class Categories
    {
        public static readonly Category Combat = new Category("Combat");
        public static readonly Category Player = new Category("Player");
        public static readonly Category Movement = new Category("Movement");
        public static readonly Category Render = new Category("Render");
        public static readonly Category Exploit = new Category("Exploit");
        public static readonly Category Misc = new Category("Misc");

        public static void init()
        {
            Modules.instance.registerCategory(Combat);
            Modules.instance.registerCategory(Player);
            Modules.instance.registerCategory(Movement);
            Modules.instance.registerCategory(Render);
            Modules.instance.registerCategory(Exploit);
            Modules.instance.registerCategory(Misc);
        }
    }
}
