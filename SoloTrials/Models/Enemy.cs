namespace SoloTrials.Models
{
    public class Enemy : GameModel
    {
        public int Reward { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Damage { get; set; }

        public Enemy(int maxHealth, int damage, int reward) : base(0, 0, 'X')
        {
            MaxHealth = maxHealth;
            Health = maxHealth;
            Damage = damage;
            Reward = reward;
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
            if(IsAlive())
            {
                Health -= damage;
                if(Health < 0)
                    Health = 0;
            }
        }
        public void Attack(Player player)
        {
            if(IsAlive())
            {
                player.TakeDamage(Damage);
            }
        }
    }
}
