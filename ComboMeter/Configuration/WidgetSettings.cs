using HunterPie.Core.Settings;

namespace ComboMeter.Configuration
{
    public class WidgetSettings : IWidgetSettings
    {
        public bool Initialize { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public int[] Position { get; set; }
        public float Opacity { get; set; } = 1;
        public double Scale { get; set; } = 1;
        public bool StreamerMode { get; set; } = false;

        public WidgetSettings()
        {
        }
    }
}