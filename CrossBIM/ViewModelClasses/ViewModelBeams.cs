using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossBIM.ViewModelClasses
{
    public class ViewModelBeams
    {
        public ViewModelBeams()
        {
            BeamsLayersName = new List<string>();
            BeamsTopLevel = new List<double>();
            BeamsDepth = new List<double>();
            BeamsWidth = new List<double>();
        }

        public List<string> BeamsLayersName { get; set; }
        public List<double> BeamsTopLevel { get; set; }
        public List<double> BeamsDepth { get; set; }
        public List<double> BeamsWidth { get; set; }
    }
}
