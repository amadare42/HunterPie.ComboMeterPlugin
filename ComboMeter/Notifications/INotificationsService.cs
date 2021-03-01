namespace ComboMeter.Notifications
{
    public interface INotificationsService
    {
        void AddNotification(string header, string text, int timeout);
    }
}