using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ComboMeter.Notifications
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : UserControl
    {
        public event Action<Notification, bool> ShouldBeShownChanged;

        public static readonly DependencyProperty ShouldBeShownProperty = DependencyProperty.Register(
            "ShouldBeShown", typeof(bool), typeof(Notification), new PropertyMetadata(default(bool)));

        public bool ShouldBeShown
        {
            get { return (bool) GetValue(ShouldBeShownProperty); }
            set
            {
                SetValue(ShouldBeShownProperty, value);
                ShouldBeShownChanged?.Invoke(this, value);
            }
        }

        private readonly DispatcherTimer timer;

        public Notification()
        {
            InitializeComponent();
            ShouldBeShown = true;
        }

        public Notification(NotificationModel model) : this(model.Header, model.Text, model.Timeout)
        {
        }

        public Notification(string header, string text, int time = 5000)
        {
            Header = header;
            Text = text;
            InitializeComponent();
            ShouldBeShown = true;

            timer = new DispatcherTimer(DispatcherPriority.Render, Dispatcher);
            timer.Tick += Hide;
            timer.Interval = TimeSpan.FromMilliseconds(time);
            timer.Start();
        }

        private void Hide(object sender, EventArgs args)
        {
            timer.Stop();
            Dispatcher.Invoke(() => ShouldBeShown = false);
        }

        public string Header { get; }
        public string Text { get; }
    }
}
