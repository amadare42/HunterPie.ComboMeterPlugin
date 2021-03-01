using System;
using System.Timers;
using ComboMeter.Configuration;
using ComboMeter.Notifications;
using HunterPie.Logger;

namespace ComboMeter.Combos
{
    public class ComboService
    {
        private readonly IThresholdStrategy threshold;
        private readonly INotificationsService notifications;
        private readonly IComboMeter meter;
        private readonly Timer timer;

        private int comboStart;
        private int lastDamage;
        private DateTime startTimestamp;
        private bool isInCombo;
        
        private Combo maxDmgCombo;
        private Combo maxDpsCombo;
        private Combo maxTimeCombo;

        public ComboService(IThresholdStrategy threshold, INotificationsService notifications, IComboMeter meter, int timeout)
        {
            this.threshold = threshold;
            this.notifications = notifications;
            this.meter = meter;

            this.timer = new Timer(timeout);
            this.timer.Elapsed += Timer_Elapsed;
            this.timer.AutoReset = false;
        }

        public void UpdateTimeout(int timeout) => this.timer.Interval = timeout;

        public void SetInitialDamage(int damage) => this.comboStart = damage;

        public void Stop()
        {
            this.timer.Stop();
            this.meter.Stop();
            this.ClearRecords();
        }

        public void ClearRecords()
        {
            this.maxDpsCombo = null;
            this.maxDmgCombo = null;
            this.maxTimeCombo = null;
        }

        public void WriteRecords()
        {
            if (this.maxDpsCombo != null)
            {
                var msg = $"Longest: {this.maxTimeCombo}\nMax dmg: {this.maxDmgCombo}\nMax DPS: {this.maxDpsCombo}";
                this.notifications.AddNotification("Expedition records",
                    msg, ConfigService.Current.RecordsNotificationTime);
                Debugger.Log(msg);
            }
        }

        public void UpdateDamage(int memberDamage)
        {
            // restart timer
            this.timer.Stop();
            this.timer.Start();

            // begin combo
            if (!this.isInCombo)
            {
                this.isInCombo = true;
                // we need to include current attack
                this.comboStart = memberDamage - (memberDamage - this.lastDamage);
                this.startTimestamp = DateTime.Now;
            }

            this.meter.ResetTimer((int)this.timer.Interval, memberDamage - this.comboStart);
            this.lastDamage = memberDamage;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.isInCombo = false;
            var dmg = this.lastDamage - this.comboStart;
            var elapsed = (DateTime.Now - this.startTimestamp).TotalSeconds;
            if (dmg == 0)
            {
                return;
            }

            var header = this.threshold.GetGradeHeader(dmg);
            if (header == null) return;

            var combo = new Combo
            {
                Damage = dmg,
                Dps = dmg / elapsed,
                Time = elapsed
            };

            var newDmg = this.maxDmgCombo == null || this.maxDmgCombo.Damage < combo.Damage;
            if (newDmg) this.maxDmgCombo = combo;

            var newTime = this.maxTimeCombo == null || this.maxTimeCombo.Time < combo.Time;
            if (newTime) this.maxTimeCombo = combo;

            var newDps = this.maxDpsCombo == null || this.maxDpsCombo.Dps < combo.Dps;
            if (newDps) this.maxDpsCombo = combo;

            this.notifications.AddNotification(header, combo.FormatLong(newDmg, newTime, newDps), ConfigService.Current.ComboNotificationTime);
            this.meter.Stop();
        }
    }
}