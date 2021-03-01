using System.Windows.Controls;

namespace ComboMeter.Configuration
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        public SettingsControl(SettingsViewModel vm) : this()
        {
            this.DataContext = vm;
        }
    }
}
