namespace ComboMeter.Combos
{
    public interface IComboMeter
    {
        void ResetTimer(int timeout, int damage);
        void Stop();
    }
}