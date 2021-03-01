using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ComboMeter.Combos;
using ComboMeter.Configuration;
using ComboMeter.Notifications;
using HunterPie.Core;
using HunterPie.GUI;
using HunterPie.Logger;
using HunterPie.Plugins;
using HunterPie.Settings;

namespace ComboMeter
{
    // TODO: visual settings are disabled until https://github.com/Haato3o/HunterPie/pull/151 is released
    public class ComboMeterPlugin : IPlugin//, ISettingsOwner
    {
        private NotificationsWidget notificationsWidget;
        private ComboMeterWidget comboMeterWidget;
        private ComboService comboService;
        private bool initialDamageIsSet = false;

        public ComboMeterPlugin()
        {
            ConfigService.ConfigUpdated += ConfigService_ConfigUpdated;
        }

        public string Name { get; set; } = "ComboMeter";
        public string Description { get; set; }
        public Game Context { get; set; }

        public void Initialize(Game context)
        {
            this.Context = context;

            CreateWidgets();
            this.initialDamageIsSet = false;
            this.comboService = new ComboService(new ThresholdStrategy(context), notificationsWidget, comboMeterWidget, ConfigService.Current.ComboTimeout);

            foreach (var member in Context.Player.PlayerParty.Members)
            {
                member.OnSpawn += Member_OnSpawn;
                member.OnDamageChange += Member_OnDamageChange;
            }

            // if HunterPie launched when game is running, we need to set initial damage
            if (!context.Player.InPeaceZone)
            {
                var me = context.Player.PlayerParty.Members.FirstOrDefault(m => m.IsMe);
                if (me != null) this.comboService.SetInitialDamage(me.Damage);
            }

            context.Player.OnPeaceZoneEnter += Player_OnPeaceZoneEnter;
            context.Player.OnPeaceZoneLeave += Player_OnPeaceZoneLeave;
        }

        public void Unload()
        {
            foreach (var member in Context.Player.PlayerParty.Members)
            {
                member.OnSpawn -= Member_OnSpawn;
                member.OnDamageChange -= Member_OnDamageChange;
            }
            Context.Player.OnPeaceZoneEnter -= Player_OnPeaceZoneEnter;
            Context.Player.OnPeaceZoneLeave -= Player_OnPeaceZoneLeave;
            this.Context = null;

            this.comboService.Stop();
            this.comboService = null;
            DestroyWidgets();
        }

        private void CreateWidgets()
        {
            ConfigService.Load();
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.notificationsWidget = new NotificationsWidget();
                this.comboMeterWidget = new ComboMeterWidget();

                Overlay.RegisterWidget(this.notificationsWidget);
                Overlay.RegisterWidget(this.comboMeterWidget);
            });
        }

        private void DestroyWidgets()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Overlay.UnregisterWidget(comboMeterWidget);
                Overlay.UnregisterWidget(notificationsWidget);
                this.comboMeterWidget.Close();
                this.notificationsWidget.Close();

                this.comboMeterWidget = null;
                this.notificationsWidget = null;
            });
        }

        private void Player_OnPeaceZoneLeave(object source, EventArgs args) => this.comboService.ClearRecords();

        private void Player_OnPeaceZoneEnter(object source, EventArgs args)
        {
            this.comboService.WriteRecords();
            this.comboService.Stop();
        }

        private void ConfigService_ConfigUpdated(ComboMeterConfig obj) => this.comboService?.UpdateTimeout(obj.ComboTimeout);

        private void Member_OnDamageChange(object source, HunterPie.Core.Events.PartyMemberEventArgs args)
        {
            if (this.initialDamageIsSet && source is Member member && member.IsMe && member.Damage != 0) 
                comboService.UpdateDamage(member.Damage);
        }

        private void Member_OnSpawn(object source, HunterPie.Core.Events.PartyMemberEventArgs args)
        {
            if (source is Member member && member.IsMe)
            {
                comboService.SetInitialDamage(member.Damage);
                this.initialDamageIsSet = true;
            }
        }


        public IEnumerable<ISettingsTab> GetSettings(ISettingsBuilder build)
        {
            var vm = new SettingsViewModel();
            return build.AddTab(new SettingsControl(vm), vm)
                .Value();
        }
    }
}