namespace SoloTrials.Models
{
    public class Coin : GameModel
    {
        public const int ScorePerValue = 10;

        public int Value { get; set; }
        public bool IsCollected { get; private set; }

        public Coin(int x, int y, int value = 1) : base(x, y, '$')
        {
            Value = value;
        }

        public void Collect(Player player)
        {
            if (IsCollected)
                return;

            IsCollected = true;
            player.Coins += Value;
            player.AddScore(Value * ScorePerValue);
        }
    }
}