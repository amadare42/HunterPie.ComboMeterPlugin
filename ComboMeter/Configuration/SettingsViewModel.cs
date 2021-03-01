using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ComboMeter.Properties;
using HunterPie.Settings;
using HunterPie.UI.Infrastructure;

namespace ComboMeter.Configuration
{
    public class SettingsViewModel : INotifyPropertyChanged, ISettings
    {
        private int timeout;
        public int Timeout
        {
            get => timeout;
            set
            {
                if (value == timeout) return;
                timeout = value;
                OnPropertyChanged();
            }
        }

        private int notificationTimeout;
        public int NotificationTimeout
        {
            get => notificationTimeout;
            set
            {
                if (value == notificationTimeout) return;
                notificationTimeout = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            AddGradeCommand = new ArglessRelayCommand(AddGrade);
            RemoveGradeCommand = new RelayCommand(g => RemoveGrade(g as ComboGradeConfigViewModel));
        }


        public ObservableCollection<ComboGradeConfigViewModel> Grades { get; } = new ObservableCollection<ComboGradeConfigViewModel>();

        public ICommand AddGradeCommand { get; }
        public ICommand RemoveGradeCommand { get; }


        private void AddGrade() => Grades.Add(new ComboGradeConfigViewModel
        {
            AvgHpMultiplier = this.Grades.LastOrDefault()?.AvgHpMultiplier ?? .1f
        });

        private void RemoveGrade(ComboGradeConfigViewModel grade) => Grades.Remove(grade);

        public bool IsSettingsChanged { get; } = true;

        public void LoadSettings()
        {
            ConfigService.Load();
            this.Timeout = ConfigService.Current.ComboTimeout;
            this.NotificationTimeout = ConfigService.Current.RecordsNotificationTime;
            this.Grades.Clear();
            foreach (var grade in ConfigService.Current.ComboGrades)
            {
                this.Grades.Add(new ComboGradeConfigViewModel(grade.AvgHpMultiplier, grade.Header));
            }
        }

        public void SaveSettings()
        {
            ConfigService.Current.ComboTimeout = this.Timeout;
            ConfigService.Current.RecordsNotificationTime = this.NotificationTimeout;
            ConfigService.Current.ComboGrades = this.Grades.Select(g => new ComboGradeConfig(g.Header, g.AvgHpMultiplier))
                .ToList();
            ConfigService.Save();
        }

        public string ValidateSettings()
        {
            if (Grades.Any(g => string.IsNullOrEmpty(g.Header)))
            {
                return "All grades should have header!";
            }

            return null;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}