using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualME7.WPF.classes;

namespace VisualME7.WPF.viewmodel
{
    public class DashboardViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<SeriesData> Series { get; set; }
        public DelegateCommand AddSeriesCommand { get; set; }

        public DashboardViewModel()
        {
            AddSeriesCommand = new DelegateCommand(x => AddSeries());

            ObservableCollection<GraphVariable> data = new ObservableCollection<GraphVariable>();
            data = new ObservableCollection<GraphVariable>();
            data.Add(new GraphVariable() { Name = "RPM", VarInt = 0 });

            Series = new ObservableCollection<SeriesData>();
            Series.Add(new SeriesData() { SeriesDisplayName = "RPMS", Items = data });
        }

        private void AddSeries()
        {
            ObservableCollection<GraphVariable> data = new ObservableCollection<GraphVariable>();

            data.Add(new GraphVariable() { Name = "RPM", VarInt = 600 });
            data.Add(new GraphVariable() { Name = "RPM", VarInt = 600 });
            data.Add(new GraphVariable() { Name = "RPM", VarInt = 600 });
            data.Add(new GraphVariable() { Name = "RPM", VarInt = 600 });
            data.Add(new GraphVariable() { Name = "RPM", VarInt = 600 });
            data.Add(new GraphVariable() { Name = "RPM", VarInt = 600 });

            Series.Add(new SeriesData() { SeriesDisplayName = "New Series", Items = data });
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion
    }
}
