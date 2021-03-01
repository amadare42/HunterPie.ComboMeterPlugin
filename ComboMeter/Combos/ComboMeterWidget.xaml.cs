using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using ComboMeter.Configuration;
using HunterPie.Core.Settings;
using HunterPie.GUI;
using HunterPie.GUI.Helpers;
using HunterPie.Utils;

namespace ComboMeter.Combos
{
    /// <summary>
    /// Interaction logic for ComboMeterWidget.xaml
    /// </summary>
    public partial class ComboMeterWidget : Widget, IComboMeter
    {
        public override IWidgetSettings Settings => ConfigService.Current.ComboMeterWidget;

        private readonly Stopwatch sw = new Stopwatch();
        private int timeout = 0;
        private volatile int damage;
        private volatile bool isChanged;

        public static readonly DependencyProperty DamageTextProperty = DependencyProperty.Register(
            "DamageText", typeof(string), typeof(ComboMeterWidget), new PropertyMetadata(default(string)));

        public string DamageText
        {
            get { return (string) GetValue(DamageTextProperty); }
            set { SetValue(DamageTextProperty, value); }
        }

        public ComboMeterWidget()
        {
            WidgetHasContent = true;
            InitializeComponent();
            SetWindowFlags();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            if (DesignUtils.IsInDesignMode)
            {
                DamageText = "42";
            }
        }

        public void SetVisibility(bool visibility)
        {
            if (WidgetHasContent != visibility)
            {
                WidgetHasContent = visibility;
                Dispatcher.Invoke(ChangeVisibility);
            }
        }

        public void ResetTimer(int timeout, int damage)
        {
            SetVisibility(true);

            sw.Restart();

            this.timeout = timeout;
            this.damage = damage;
            this.isChanged = true;
        }

        public void Stop() => SetVisibility(false);

        private void CompositionTarget_Rendering(object sender, System.EventArgs e)
        {
            // no update needed
            if (!this.WidgetHasContent) return;

            // update timer
            var elapsed = this.sw.ElapsedMilliseconds;
            TimerArc.EndAngle = Arc.ConvertPercentageIntoAngle(1 - elapsed / (float)timeout);

            // update text if needed
            if (!isChanged) return;
            this.isChanged = false;
            DamageText = damage.ToString();
        }

        public override void EnterWidgetDesignMode()
        {
            base.EnterWidgetDesignMode();
            RemoveWindowTransparencyFlag();
        }

        public override void LeaveWidgetDesignMode()
        {
            ApplyWindowTransparencyFlag();
            base.LeaveWidgetDesignMode();
        }

        public override void SaveSettings()
        {
            base.SaveSettings();
            ConfigService.Save();
        }

        public override void ScaleWidget(double newScaleX, double newScaleY)
        {
            if (newScaleX <= 0.2)
                return;

            Container.LayoutTransform = new ScaleTransform(newScaleX, newScaleY);
            DefaultScaleX = newScaleX;
            DefaultScaleY = newScaleY;
        }
    }
}
