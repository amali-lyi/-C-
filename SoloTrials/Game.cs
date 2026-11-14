using System;
using System.Collections.Generic;
using System.Linq;
using SoloTrials.Models;

namespace SoloTrials
{
    public class Game
    {
        private const int PlayerDamage = 1;
        private const int EnemyChaseDistance = 6;
        private const int HudWidth = 70;

        private Player player;
        private InputHandler input;
        private int width = 20;
        private int height = 10;

        private List<Enemy> enemies = new List<Enemy>();
        private List<Coin> coins = new List<Coin>();
        private List<Bonus> bonuses = new List<Bonus>();
        private Random random = new Random();
        private string message = "";

        public Game()
        {
            player = new Player(3, 3);
            player.MaxLives = 5;
            player.X = 2;
            player.Y = 2;
            input = new InputHandler();
        }

        private void CreateMap()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 || x == width - 1 ||
                        y == 0 || y == height - 1)
                    {
                        map[y, x] = '#';
                    }
                    else
                    {
                        map[y, x] = ' ';
                    }
                }
            }

            for (int x = 1; x < width - 1; x++)
            {
                if (x < 22 || x > 27)
                {
                    map[5, x] = '#';
                    map[10, x] = '#';
                    map[15, x] = '#';
                }
            }

            map[height - 2, width - 2] = '*';
        }

        // ---------- Спавн ----------

        private List<(int x, int y)> GetFreeCells()
        {
            var cells = new List<(int x, int y)>();

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    if (map[y, x] == ' ' && !(x == player.X && y == player.Y))
                        cells.Add((x, y));
                }
            }

            return cells;
        }

        private (int x, int y) TakeRandomCell(List<(int x, int y)> cells)
        {
            int index = random.Next(cells.Count);
            var cell = cells[index];
            cells.RemoveAt(index);
            return cell;
        }

        private void SpawnEntities()
        {
            enemies.Clear();
            coins.Clear();
            bonuses.Clear();

            var cells = GetFreeCells();

            var enemyCells = cells
                .Where(c => Math.Abs(c.x - player.X) + Math.Abs(c.y - player.Y) > 8)
                .ToList();

            int enemyCount = 3 + currentLevel * 2;
            for (int i = 0; i < enemyCount && enemyCells.Count > 0; i++)
            {
                var cell = TakeRandomCell(enemyCells);
                cells.Remove(cell);
                enemies.Add(new Enemy(2 + currentLevel, 1, 50 * currentLevel, cell.x, cell.y));
            }

            int coinCount = 10 + currentLevel * 2;
            for (int i = 0; i < coinCount && cells.Count > 0; i++)
            {
                var cell = TakeRandomCell(cells);
                coins.Add(new Coin(cell.x, cell.y, 1));
            }

            BonusType[] types = { BonusType.Heal, BonusType.Heal, BonusType.ExtraLife, BonusType.ScoreBonus };
            foreach (var type in types)
            {
                if (cells.Count == 0)
                    break;

                var cell = TakeRandomCell(cells);
                int amount = type == BonusType.ScoreBonus ? 100 : 1;
                bonuses.Add(new Bonus(cell.x, cell.y, type, amount));
            }
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();
            SpawnEntities();
            bool running = true;

            while (running)
            {
                levelCompleted = false;

                while (running && !levelCompleted && player.IsAlive())
                {
                    Draw();

                ConsoleKey key = input.ReadInput();

                    int newX = player.X;
                    int newY = player.Y;
                    bool isMoveKey = true;

                    switch (key)
                    {
                        case ConsoleKey.W:
                        case ConsoleKey.UpArrow:
                            newY--;
                            break;

                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        if (player.Y < height - 1)
                            player.Y++;
                        break;

                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        if (player.X > 0)
                            player.X--;
                        break;

                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        if (player.X < width - 1)
                            player.X++;
                        break;

                        case ConsoleKey.Escape:
                            running = false;
                            isMoveKey = false;
                            break;

                        default:
                            isMoveKey = false;
                            break;
                    }

                    if (!running)
                        break;

                    if (!isMoveKey)
                        continue;

                    if (newX < 0 || newX >= width ||
                        newY < 0 || newY >= height ||
                        map[newY, newX] == '#')
                    {
                        continue;
                    }

                    message = "";

                    Enemy target = enemies.FirstOrDefault(e => e.IsAlive() && e.X == newX && e.Y == newY);

                    if (target != null)
                    {
                        target.TakeDamageFrom(player, PlayerDamage);

                        if (!target.IsAlive())
                        {
                            message = $"Enemy defeated! +{target.Reward} points";
                        }
                        else
                        {
                            message = $"You hit the enemy ({target.Health}/{target.MaxHealth}).";
                        }

                        enemies.RemoveAll(e => !e.IsAlive());
                    }
                    else
                    {
                        player.X = newX;
                        player.Y = newY;

                        CollectItems();

                        if (map[player.Y, player.X] == '*')
                        {
                            levelCompleted = true;
                        }
                    }

                    if (!levelCompleted && player.IsAlive())
                    {
                        EnemiesTurn();
                    }
                }

                if (!player.IsAlive())
                {
                    ShowGameOver();
                    break;
                }

                if (!running || !levelCompleted)
                    break;

                if (ShowLevelMenu())
                {
                    Level2 level2 = new Level2();

                    map = level2.Map;
                    width = level2.Width;
                    height = level2.Height;

                    player.X = 1;
                    player.Y = 1;

                    currentLevel = 2;
                    message = "";
                    Console.Clear();
                    SpawnEntities();
                }
                else
                {
                    break;
                }
            }

            Console.CursorVisible = true;
        }

        private void CollectItems()
        {
            foreach (var coin in coins.Where(c => c.X == player.X && c.Y == player.Y).ToList())
            {
                coin.Collect(player);
                message = $"Coin collected! +{coin.Value * Coin.ScorePerValue} points";
            }
            coins.RemoveAll(c => c.IsCollected);

            foreach (var bonus in bonuses.Where(b => b.X == player.X && b.Y == player.Y).ToList())
            {
                bonus.Apply(player);

                switch (bonus.Type)
                {
                    case BonusType.Heal:
                        message = "Bonus: health restored!";
                        break;
                    case BonusType.ExtraLife:
                        message = "Bonus: extra life!";
                        break;
                    case BonusType.ScoreBonus:
                        message = $"Bonus: +{bonus.Amount} points!";
                        break;
                }
            }
            bonuses.RemoveAll(b => b.IsCollected);
        }


        private bool IsCellFreeForEnemy(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return false;

            if (map[y, x] == '#')
                return false;

            return !enemies.Any(e => e.IsAlive() && e.X == x && e.Y == y);
        }

        private void EnemiesTurn()
        {
            foreach (var enemy in enemies.Where(e => e.IsAlive()).ToList())
            {
                if (!player.IsAlive())
                    return;

                int dx = player.X - enemy.X;
                int dy = player.Y - enemy.Y;

                if (Math.Abs(dx) + Math.Abs(dy) > EnemyChaseDistance)
                    continue;

                int stepX = Math.Sign(dx);
                int stepY = Math.Sign(dy);


                var attempts = new List<(int x, int y)>();
                if (Math.Abs(dx) >= Math.Abs(dy))
                {
                    if (stepX != 0) attempts.Add((enemy.X + stepX, enemy.Y));
                    if (stepY != 0) attempts.Add((enemy.X, enemy.Y + stepY));
                }
                else
                {
                    if (stepY != 0) attempts.Add((enemy.X, enemy.Y + stepY));
                    if (stepX != 0) attempts.Add((enemy.X + stepX, enemy.Y));
                }

                foreach (var (tx, ty) in attempts)
                {
                    if (tx == player.X && ty == player.Y)
                    {
                        int livesBefore = player.Lives;
                        enemy.Attack(player);
                        string hit = player.Lives < livesBefore
                            ? "An enemy hit you. You lost a life!"
                            : "An enemy hit you!";
                        message = (message + " " + hit).Trim();
                        break;
                    }

                    if (IsCellFreeForEnemy(tx, ty))
                    {
                        enemy.X = tx;
                        enemy.Y = ty;
                        break;
                    }
                }
            }
        }


        private char GetSymbolAt(int x, int y)
        {
            if (player.X == x && player.Y == y)
                return player.Symbol;

            foreach (var enemy in enemies)
                if (enemy.X == x && enemy.Y == y)
                    return enemy.Symbol;

            foreach (var bonus in bonuses)
                if (bonus.X == x && bonus.Y == y)
                    return bonus.Symbol;

            foreach (var coin in coins)
                if (coin.X == x && coin.Y == y)
                    return coin.Symbol;

            return map[y, x];
        }

        private void Draw()
        {
            Console.SetCursorPosition(0, 0);

            Console.WriteLine($"LEVEL {currentLevel}".PadRight(HudWidth));
            Console.WriteLine(
                $"Lives: {player.Lives}/{player.MaxLives} | HP: {player.Health}/{player.MaxHealth} | " +
                $"Coins: {player.Coins} | Score: {player.Score}".PadRight(HudWidth));

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write(GetSymbolAt(x, y));
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine(message.PadRight(HudWidth));
            Console.WriteLine("WASD / Arrows - move (walk into X to attack) | Esc - exit".PadRight(HudWidth));
            Console.WriteLine("* - finish | $ - coin | + - heal | L - life | B - bonus points | X - enemy".PadRight(HudWidth));
        }


        private bool ShowLevelMenu()
        {
            Console.Clear();

            if (currentLevel == 1)
            {
                Console.WriteLine("LEVEL 1 COMPLETED!");
                Console.WriteLine($"Score: {player.Score} | Coins: {player.Coins} | Lives: {player.Lives}");
                Console.WriteLine();
                Console.WriteLine("1. LEVEL 2");
                Console.WriteLine("0. Exit");

                while (true)
                {
                    ConsoleKey key = input.ReadInput();

                    if (key == ConsoleKey.D0 || key == ConsoleKey.NumPad0 ||
                        key == ConsoleKey.Escape)
                    {
                        return false;
                    }

                    if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
                    {
                        return true;
                    }
                }
            }

            Console.WriteLine("LEVEL 2 COMPLETED!");
            Console.WriteLine($"Final score: {player.Score} | Coins: {player.Coins} | Lives: {player.Lives}");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");

            input.ReadInput();

            return false;
        }

        private void ShowGameOver()
        {
            Console.Clear();
            Console.WriteLine("GAME OVER");
            Console.WriteLine();
            Console.WriteLine($"Level reached: {currentLevel}");
            Console.WriteLine($"Coins: {player.Coins}");
            Console.WriteLine($"Final score: {player.Score}");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");

            input.ReadInput();
        }
    }
}
