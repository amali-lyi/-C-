namespace SoloTrials.Models
{
    public class Player
    // updated player class 23 oktober...
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }

        public int Health { get; set; }
        public int Money { get; set; }
        public int Speed { get; set; }
        

        public Player(int x, int y)
        {
            X = x;
            Y = y;
            Symbol = '*';

            Health = 100;
            Money = 0;
            Speed = 1;
        }
    }
}