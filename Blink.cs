using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.exploit
{
    class Blink : Module
    {
        public Blink() : base(Categories.Exploit, "Blink", "Allows to blink")
        {

        }

        public override void onActivate()
        {
            
        }

        public override void onDeactivate()
        {
            
        }

        public override void onRender()
        {
            
        }

        public override void onUpdate()
        {
            
        }

        public bool shouldInterceptUpdate()
        {
            return isEnabled();
        }
    }
}
