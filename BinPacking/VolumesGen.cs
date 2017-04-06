using System.Collections.Generic;
using Genetics.GeneticsAlgorithms;

namespace Genetics.BinPacking
{
    /*
     * Gen type used for bin backing algorithm
     */
    class VolumesGen : Gen
    {
        public List<int> Volumes { get; set; }

        public VolumesGen(List<int> volumes)
        {
            Volumes = new List<int>(volumes);
        }
    }
}
