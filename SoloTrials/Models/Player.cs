namespace SoloTrials.Models
{
    public class Player : GameModel
    {
        public int Coins { get; set; } = 0;
        public int Health { get; set; }
        public int MaxHealth { get; set; }

        public Player(int maxHealth) : base(0, 0, '*')
        {
            Health = maxHealth;
            MaxHealth = maxHealth;
        }
        public bool IsAlive()
        {
            if (Health <= 0)
                return false;
            else
                return true;
        }
        public void TakeDamage(int damage)
        {
            if (IsAlive())
            {
                Health -= damage;
                if (Health < 0)
                    Health = 0;
            }
        }
    }
}