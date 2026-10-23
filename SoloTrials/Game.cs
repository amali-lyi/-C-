using System;
using SoloTrials.Models;

namespace SoloTrials
{
    public class Game
    {
        private Player player;
        private InputHandler input;
        private int width = 50;
        private int height = 20;
        private char[,] map;
        private bool levelCompleted = false;

        public Game()
        {
            player = new Player(2, 2);
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

        public void Run()
        {
            Console.CursorVisible = false;
            bool running = true;

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

            if (levelCompleted)
            {
                ShowLevelMenu();
            }

            Console.CursorVisible = true;
            Console.Clear();
        }

        private void Draw()
        {
            Console.SetCursorPosition(0, 0);

            Console.WriteLine("LEVEL 1");

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

        private void ShowLevelMenu()
        {
            Console.Clear();
            Console.WriteLine("LEVEL 1 COMPLETED!");
            Console.WriteLine();
            Console.WriteLine("1. LEVEL 2");
            Console.WriteLine("0. Exit");
            Console.WriteLine();
            Console.WriteLine("LEVEL 2 is not available yet.");

            while (true)
            {
                ConsoleKey key = input.ReadInput();

                if (key == ConsoleKey.D0 || key == ConsoleKey.NumPad0 ||
                    key == ConsoleKey.Escape)
                {
                    break;
                }

                if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
                {
                    Console.Clear();
                    Console.WriteLine("LEVEL 2 is not available yet.");
                    Console.WriteLine("Press any key to return to the menu.");
                    input.ReadInput();

                    Console.Clear();
                    Console.WriteLine("LEVEL 1 COMPLETED!");
                    Console.WriteLine();
                    Console.WriteLine("1. LEVEL 2");
                    Console.WriteLine("0. Exit");
                    Console.WriteLine();
                    Console.WriteLine("LEVEL 2 is not available yet.");
                }
            }
        }
    }
}