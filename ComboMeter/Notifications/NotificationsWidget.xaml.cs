using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media;
using ComboMeter.Configuration;
using HunterPie.Core.Settings;
using HunterPie.GUI;

namespace ComboMeter.Notifications
{
    /// <summary>
    /// Interaction logic for NotificationsWidget.xaml
    /// </summary>
    public partial class NotificationsWidget : Widget, INotificationsService
    {
        public override IWidgetSettings Settings => ConfigService.Current.NotificationsWidget;

        private readonly Stack<NotificationModel> notificationsQueue = new Stack<NotificationModel>();

        public ObservableCollection<Notification> Notifications { get; }

        public NotificationsWidget() : base()
        {
            Notifications = new ObservableCollection<Notification>();
            WidgetHasContent = true;
            InitializeComponent();
            SetWindowFlags();
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

        public void AddNotification(string header, string text, int timeout)
        {
            var model = new NotificationModel(header, text, timeout);
            if (Notifications.Count < 3)
            {
                AddImmediateNotification(model);
            }
            else
            {
                notificationsQueue.Push(model);
            }
        }

        private void AddImmediateNotification(NotificationModel model)
        {
            Dispatcher.Invoke(() =>
            {
                var notif = new Notification(model);
                notif.ShouldBeShownChanged += Notif_ShouldBeShownChanged;
                Notifications.Add(notif);
            });
        }

        private async void Notif_ShouldBeShownChanged(Notification notif, bool isVisible)
        {
            // wait for animation to end
            await Task.Delay(300);
            Notifications.Remove(notif);
            notif.ShouldBeShownChanged -= Notif_ShouldBeShownChanged;

            if (notificationsQueue.Count > 0)
            {
                var model = notificationsQueue.Pop();
                AddImmediateNotification(model);
            }
        }
    }
}
