namespace SoloTrials.Models
{
    public class Player : GameModel
    {
        public int Coins { get; set; } = 0;
        public int Score { get; private set; } = 0;
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Lives { get; private set; }
        public int MaxLives { get; set; }

        public Player(int maxHealth, int lives = 3) : base(0, 0, '@')
        {
            Health = maxHealth;
            MaxHealth = maxHealth;
            Lives = lives;
            MaxLives = lives;
        }
        public bool IsAlive()
        {
            return Lives > 0;
        }
        public void TakeDamage(int damage)
        {
            if (!IsAlive() || damage <= 0)
                return;

            Health -= damage;

            if (Health <= 0)
            {
                Lives--;
                Health = Lives > 0 ? MaxHealth : 0;
            }
        }
        public void Heal(int amount)
        {
            if (!IsAlive() || amount <= 0)
                return;

            Health += amount;
            if (Health > MaxHealth)
                Health = MaxHealth;
        }
        public void AddLife(int count = 1)
        {
            if (count <= 0)
                return;

            Lives += count;
            if (Lives > MaxLives)
                Lives = MaxLives;
        }
        public void AddScore(int points)
        {
            if (points > 0)
                Score += points;
        }
    }
}