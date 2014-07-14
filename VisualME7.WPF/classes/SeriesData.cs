using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualME7.WPF.classes
{
    public class SeriesData
    {
        public string SeriesDisplayName { get; set; }
        public string SeriesDescription { get; set; }

        public ObservableCollection<GraphVariable> Items { get; set; }
    }
}
