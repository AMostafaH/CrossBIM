using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossBIM.ViewModelClasses
{
    public class ViewModelStoreys
    {
        public ViewModelStoreys()
        {
            DXFLayersNames = new List<string>();
        }
        public string DXFFilePath { get; set; }
        public double Height { get; set; }
        public double Elevation { get; set; }

        public int NumOfRepetitions { get; set; }
        public int BeamsNumberOfCategories { get; set; }
        public int SlabsNumberOfCategories { get; set; }
        public List<string> DXFLayersNames { get; set; }
    }
}
