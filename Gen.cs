using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetics
{
    abstract class Gen
    {
        protected uint _fitness;
        public uint Fitness
        {
            get { return _fitness; }
            set { _fitness = value; }
        }
    }
}
