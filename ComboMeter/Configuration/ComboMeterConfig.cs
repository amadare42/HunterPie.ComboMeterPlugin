using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace ComboMeter.Configuration
{
    public class ComboMeterConfig
    {
        public static List<ComboGradeConfig> DefaultGrades => new List<ComboGradeConfig>
        {
            new ComboGradeConfig("[S] C-c-combo! ({Target})", .05),
            new ComboGradeConfig("[A] Impressive! ({Target})", .035),
            new ComboGradeConfig("[B] Good! ({Target})", .02),
            new ComboGradeConfig("[D] Combo done", .0)
        };

        public List<ComboGradeConfig> ComboGrades { get; set; } 

        public int ComboTimeout { get; set; } = 3500;
        
        public int RecordsNotificationTime { get; set; } = 15000;

        public int ComboNotificationTime { get; set; } = 7000;

        public WidgetSettings NotificationsWidget { get; set; } = new WidgetSettings
        {
            Position = new[] {1370, 680}
        };

        public WidgetSettings ComboMeterWidget { get; set; } = new WidgetSettings
        {
            Position = new[] {1170, 680}
        };
    }

    public class ComboGradeConfig
    {
        public double AvgHpMultiplier { get; set; }
        public string Header { get; set; }

        public ComboGradeConfig(string header, double avgHpMultiplier)
        {
            AvgHpMultiplier = avgHpMultiplier;
            Header = header;
        }

        public ComboGradeConfig()
        {
        }
    }
}