
using System;
using System.Collections.Generic;
using System.Linq;
using SoloTrials.Models;
using SoloTrials.World;

namespace SoloTrials
{
    public class Game
    {
        private const int PlayerDamage = 1;
        private const int EnemyChaseDistance = 6;
        private const int HudWidth = 70;

        private Player player;
        private InputHandler input;
        private int width = 50;
        private int height = 20;
        private char[,] map;
        private bool levelCompleted = false;
        private int currentLevel = 1;

        private List<Enemy> enemies = new List<Enemy>();
        private List<Coin> coins = new List<Coin>();
        private List<Bonus> bonuses = new List<Bonus>();
        private Random random = new Random();
        private string message = "";

        public Game()
        {
            player = new Player(3, 3);
            player.MaxLives = 5;

            input = new InputHandler();
            map = new char[height, width];

            CreateMap();
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

        private List<(int x, int y)> GetFreeCells()
        {
            var cells = new List<(int x, int y)>();

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    if (map[y, x] == ' ' &&
                        !(x == player.X && y == player.Y))
                    {
                        cells.Add((x, y));
                    }
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
                .Where(c => Math.Abs(c.x - player.X) +
                            Math.Abs(c.y - player.Y) > 8)
                .ToList();

            int enemyCount = 3 + currentLevel * 2;

            for (int i = 0; i < enemyCount && enemyCells.Count > 0; i++)
            {
                var cell = TakeRandomCell(enemyCells);
                cells.Remove(cell);

                enemies.Add(new Enemy(
                    2 + currentLevel,
                    1,
                    50 * currentLevel,
                    cell.x,
                    cell.y));
            }

            int coinCount = 10 + currentLevel * 2;

            for (int i = 0; i < coinCount && cells.Count > 0; i++)
            {
                var cell = TakeRandomCell(cells);
                coins.Add(new Coin(cell.x, cell.y, 1));
            }

            BonusType[] types =
            {
                BonusType.Heal,
                BonusType.Heal,
                BonusType.ExtraLife,
                BonusType.ScoreBonus
            };

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

                while (running && !levelCompleted)
                {
                    Draw();

                    ConsoleKey key = input.ReadInput();

                    int newX = player.X;
                    int newY = player.Y;

                    switch (key)
                    {
                        case ConsoleKey.W:
                        case ConsoleKey.UpArrow:
                            newY--;
                            break;

                        case ConsoleKey.S:
                        case ConsoleKey.DownArrow:
                            newY++;
                            break;

                        case ConsoleKey.A:
                        case ConsoleKey.LeftArrow:
                            newX--;
                            break;

                        case ConsoleKey.D:
                        case ConsoleKey.RightArrow:
                            newX++;
                            break;

                        case ConsoleKey.Escape:
                            running = false;
                            break;
                    }

                    if (running &&
                        newX >= 0 && newX < width &&
                        newY >= 0 && newY < height &&
                        map[newY, newX] != '#')
                    {
                        player.X = newX;
                        player.Y = newY;

                        if (map[player.Y, player.X] == '*')
                        {
                            levelCompleted = true;
                        }
                    }
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
                    SpawnEntities();
                }
                else
                {
                    break;
                }
            }

            Console.CursorVisible = true;
            Console.Clear();
        }

        private void Draw()
        {
            Console.SetCursorPosition(0, 0);

            Console.WriteLine($"LEVEL {currentLevel}");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (player.X == x && player.Y == y)
                    {
                        Console.Write(player.Symbol);
                    }
                    else
                    {
                        Console.Write(map[y, x]);
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("WASD / Arrows - move | Esc - exit");
            Console.WriteLine("* - finish level");
        }

        private bool ShowLevelMenu()
        {
            Console.Clear();

            if (currentLevel == 1)
            {
                Console.WriteLine("LEVEL 1 COMPLETED!");
                Console.WriteLine();
                Console.WriteLine("1. LEVEL 2");
                Console.WriteLine("0. Exit");

                while (true)
                {
                    ConsoleKey key = input.ReadInput();

                    if (key == ConsoleKey.D0 ||
                        key == ConsoleKey.NumPad0 ||
                        key == ConsoleKey.Escape)
                    {
                        return false;
                    }

                    if (key == ConsoleKey.D1 ||
                        key == ConsoleKey.NumPad1)
                    {
                        return true;
                    }
                }
            }

            Console.WriteLine("LEVEL 2 COMPLETED!");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");

            input.ReadInput();

            return false;
        }
    }
}
