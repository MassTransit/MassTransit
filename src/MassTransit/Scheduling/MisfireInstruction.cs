using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Scheduling
{
    public enum MisfireInstruction
    {
        Default,
        DoNothing,
        FireOnceNow
    }
}
