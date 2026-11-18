namespace SoloTrials.Models
{
    public enum BonusType
    {
        Heal,
        ExtraLife,
        ScoreBonus
    }

    public class Bonus : GameModel
    {
        public BonusType Type { get; }
        public int Amount { get; set; }
        public bool IsCollected { get; private set; }

        public Bonus(int x, int y, BonusType type, int amount = 1) : base(x, y, GetSymbol(type))
        {
            Type = type;
            Amount = amount;
        }
        private static char GetSymbol(BonusType type)
        {
            switch (type)
            {
                case BonusType.Heal: return '+';
                case BonusType.ExtraLife: return 'L';
                default: return 'B';
            }
        }
        public void Apply(Player player)
        {
            if (IsCollected)
                return;

            IsCollected = true;

            switch (Type)
            {
                case BonusType.Heal:
                    player.Heal(Amount);
                    break;
                case BonusType.ExtraLife:
                    player.AddLife(Amount);
                    break;
                case BonusType.ScoreBonus:
                    player.AddScore(Amount);
                    break;
            }
        }
    }
}