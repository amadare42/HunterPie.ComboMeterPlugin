using System.Linq;
using ComboMeter.Configuration;
using HunterPie.Core;

namespace ComboMeter.Combos
{
    public class ThresholdStrategy : IThresholdStrategy
    {
        private readonly Game game;

        public ThresholdStrategy(Game game)
        {
            this.game = game;
        }

        public string GetGradeHeader(int damage)
        {
            var avgHp = this.game.Monsters.Where(m => !string.IsNullOrEmpty(m.Id))
                .Average(m => m.MaxHealth);

            var suitable = ConfigService.Current.ComboGrades.Select(g => new {g, hp = g.AvgHpMultiplier * avgHp})
                .Where(t => t.hp < damage)
                .OrderByDescending(t => t.hp)
                .FirstOrDefault();

            return suitable?.g.Header.Replace("{Target}", suitable.hp.ToString("0.0"));
        }
    }
}