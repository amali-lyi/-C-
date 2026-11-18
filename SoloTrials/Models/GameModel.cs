namespace SoloTrials.Models
{
    public class GameModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }
        public GameModel(int x, int y, char symbol)
        {
            X = x;
            Y = y;
            Symbol = symbol;
        }
    }
}
