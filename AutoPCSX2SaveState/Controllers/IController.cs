using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPCSX2SaveState.Controllers
{
    interface IController
    {
        double GetIdleTime();
        void Poll();
    }
}
