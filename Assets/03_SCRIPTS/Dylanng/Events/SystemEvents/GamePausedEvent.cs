using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dylanng.Core;
using Dylanng.Core.Base.Interfaces;

namespace Dylanng.Events.SystemEvents
{
    public struct GamePausedEvent : IEvent
    {
        public bool IsPaused;
    }
}
