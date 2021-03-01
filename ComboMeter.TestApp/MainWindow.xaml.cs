using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComboMeter.Combos;
using ComboMeter.Configuration;
using ComboMeter.Notifications;
using ComboMeter.TestApp.Annotations;
using HunterPie.Core;
using HunterPie.Core.Settings;

namespace ComboMeter.TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ComboMeterWidget comboWidget;
        private readonly ComboService comboService;

        private int damage = 0;
        private readonly NotificationsWidget notificationsWidget;
        private readonly OverlaySimulator overlay;
        public SettingsViewModel SettingsVm { get; }

        public bool IsDesignMode
        {
            get => overlay.IsDesignMode;
            set
            {
                if (overlay.IsDesignMode && !value)
                {
                    ConfigService.Save();
                }
                overlay.IsDesignMode = value;
            }
        }

        public MainWindow()
        {
            typeof(ConfigManager).GetProperty(nameof(ConfigManager.Settings)).SetValue(null, new Config(), null);
            ConfigService.Load();
            SettingsVm = new SettingsViewModel();
            SettingsVm.LoadSettings();
            this.overlay = new OverlaySimulator();

            InitializeComponent();

            this.comboWidget = new ComboMeterWidget();
            overlay.Add(this.comboWidget);

            this.notificationsWidget = new NotificationsWidget();
            overlay.Add(this.notificationsWidget);

            this.comboService = new ComboService(new FakeThreshold(), notificationsWidget, comboWidget, 3000);
            this.LocationChanged += MainWindow_LocationChanged;

        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            //var location = this.PointToScreen(new Point());
            //this.comboWidget.Left = location.X + 30;
            //this.comboWidget.Top = location.Y + 100;

            //this.notificationsWidget.Left = location.X + 30;
            //this.notificationsWidget.Top = location.Y + 150;
        }

        private void HitClick(object sender, RoutedEventArgs e)
        {
            this.comboService.UpdateDamage(damage += 10);
        }

        private void DoneClick(object sender, RoutedEventArgs e)
        {
            this.comboService.WriteRecords();
            this.comboService.Stop();
            this.comboService.SetInitialDamage(this.damage);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FakeThreshold : IThresholdStrategy
    {
        public string GetGradeHeader(int damage)
        {
            return "test";
        }
    }
}
