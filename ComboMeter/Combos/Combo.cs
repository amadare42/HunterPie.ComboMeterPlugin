namespace ComboMeter.Combos
{
    public class Combo
    {
        public double Time { get; set; }
        public int Damage { get; set; }
        public double Dps { get; set; }

        public override string ToString() => Format();
        
        public string Format(bool newDmg = false, bool newTime = false, bool newDps = false) => $"{Damage}{Ex(newDmg)} dmg {Time:0.0}{Ex(newTime)} sec {Dps:0.0}{Ex(newDps)} DPS";
        public string FormatLong(bool newDmg = false, bool newTime = false, bool newDps = false) => $"Done {Damage}{Ex(newDmg)} dmg over {Time:0.0}{Ex(newTime)} seconds ({Dps:0.0}{Ex(newDps)} DPS)";

        private static string Ex(bool display) => display ? "(!)" : string.Empty;
    }
}