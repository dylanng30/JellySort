using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dylanng.Core.State
{
    public abstract class StateBase
    {
        public virtual void Enter(){}
        public virtual void Update(){}
        public virtual void Exit(){}
    }
}
