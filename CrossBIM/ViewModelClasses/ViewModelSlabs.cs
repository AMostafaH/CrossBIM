using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossBIM.ViewModelClasses
{
    public class ViewModelSlabs
    {
        public ViewModelSlabs()
        {
            SlabsLayersName = new List<string>();
            SlabsTopLevel = new List<double>();
            Slabsthickness = new List<double>();
        }

        public List<string> SlabsLayersName { get; set; }
        public List<double> SlabsTopLevel { get; set; }
        public List<double> Slabsthickness { get; set; }

        public string OpeningLayerName { get; set; }
        public string DropLayerName { get; set; }
        public double DropThickness { get; set; }
        public double DropTopLevel { get; set; }
    }
}
