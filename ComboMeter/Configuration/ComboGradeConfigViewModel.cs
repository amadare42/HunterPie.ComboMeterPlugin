using System.ComponentModel;
using System.Runtime.CompilerServices;
using ComboMeter.Properties;

namespace ComboMeter.Configuration
{
    public class ComboGradeConfigViewModel : INotifyPropertyChanged
    {
        private double avgHpMultiplier;
        private string header;

        public double AvgHpMultiplier
        {
            get => this.avgHpMultiplier;
            set
            {
                if (value.Equals(this.avgHpMultiplier)) return;
                this.avgHpMultiplier = value;
                OnPropertyChanged();
            }
        }

        public string Header
        {
            get => this.header;
            set
            {
                if (value == this.header) return;
                this.header = value;
                OnPropertyChanged();
            }
        }

        public ComboGradeConfigViewModel(double avgHpMultiplier, string header)
        {
            this.avgHpMultiplier = avgHpMultiplier;
            this.header = header;
        }

        public ComboGradeConfigViewModel()
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}