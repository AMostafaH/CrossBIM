using CrossBIMLib.ReadDXF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossBIM.HelperClasses
{
    public class HelperStorey
    {
        public HelperStorey()
        {
            BeamsList = new List<DXFBeam>();
            ColumnsList = new List<DXFColumn>();
            AllSlabsList = new List<DXFSlab>();
            AllDropsList = new List<DXFSlab>();
        }

        public List<DXFBeam> BeamsList { get; set; }
        public List<DXFColumn> ColumnsList { get; set; }
        public List<DXFSlab> AllSlabsList { get; set; }
        public List<DXFSlab> AllDropsList { get; set; }
    }
}
