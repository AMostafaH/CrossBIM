using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossBIM.ViewModelClasses
{
    public class ViewModelIsoFooting
    {
        
        public ViewModelIsoFooting()
        {
            IsoFootingLayersName = new List<string>();
            IsoFootingTopLevel = new List<double>();
            IsoFootingDepth = new List<double>();

            StripsLayerName = new List<string>();
            StripsTopLevel = new List<double>();
            StripsWidth = new List<double>();
            StripsDepth = new List<double>();
        }
        public double Elevation { get; set; }
        public string DXFFilePath { get; set; }
        public int FootingNumberOfCategories { get; set; }
        public int StripsNumberOfCategories { get; set; }
        public List<string> IsoFootingLayersName { get; set; }
        public List<double> IsoFootingTopLevel { get; set; }
        public List<double> IsoFootingDepth { get; set; }
        public double PCOffset { get; set; }
        public double PCDepth { get; set; }

        public List<string> StripsLayerName { get; set; }
        public List<double> StripsTopLevel { get; set; }
        public List<double> StripsWidth { get; set; }
        public List<double> StripsDepth { get; set; }

        public string BaseColumnLayerName { get; set; }
        public double BaseColumnLowLevel { get; set; }
        public double BaseColumnHeight { get; set; }

    }
}
