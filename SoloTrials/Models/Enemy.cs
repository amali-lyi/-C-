namespace SoloTrials.Models
{
    public class Enemy : GameModel
    {
        public int Reward { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Damage { get; set; }
        private bool _rewardGiven;

        public Enemy(int maxHealth, int damage, int reward, int x = 0, int y = 0) : base(x, y, 'X')
        {
            MaxHealth = maxHealth;
            Health = maxHealth;
            Damage = damage;
            Reward = reward;
        }
        public bool IsAlive()
        {
            return Health > 0;
        }
        public void TakeDamage(int damage)
        {
            if (IsAlive() && damage > 0)
            {
                Health -= damage;
                if (Health < 0)
                    Health = 0;
            }
        }
        public void Attack(Player player)
        {
            if (IsAlive())
            {
                player.TakeDamage(Damage);
            }
        }
        public void TakeDamageFrom(Player player, int damage)
        {
            TakeDamage(damage);

            if (!IsAlive() && !_rewardGiven)
            {
                _rewardGiven = true;
                player.Coins += Reward;
                player.AddScore(Reward);
            }
        }
    }
}