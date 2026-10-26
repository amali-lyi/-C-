using System;
using SoloTrials.Models;

namespace SoloTrials
{
    public class Game
    {
        private Player player;
        private InputHandler input;
        private int width = 20;
        private int height = 10;

        public Game()
        {
            player = new Player(1, 1);
            input = new InputHandler();
        }

        public void Run()
        {
            Console.CursorVisible = false;
            bool running = true;

            while (running)
            {
                Draw();

                ConsoleKey key = input.ReadInput();

                switch (key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        player.Y = Math.Max(0, player.Y - player.Speed);
                        break;

                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        player.Y = Math.Min(height - 1, player.Y + player.Speed);
                        break;

                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        player.X = Math.Max(0, player.X - player.Speed);
                        break;

                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        player.X = Math.Min(width - 1, player.X + player.Speed);
                        break;

                    case ConsoleKey.R:
                        player.Health = Math.Min(100, player.Health + 10);
                        break;

                    case ConsoleKey.F:
                        player.Money += 10;
                        break;

                    case ConsoleKey.Escape:
                        running = false;
                        break;
                }
            }

            Console.CursorVisible = true;
        }

        private void Draw()
        {
            Console.Clear();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (player.X == x && player.Y == y)
                        Console.Write(player.Symbol);
                    else
                        Console.Write('.');
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine($"Health: {player.Health}/100");
            Console.WriteLine($"Money: {player.Money}");
            Console.WriteLine($"Speed: {player.Speed}");
            Console.WriteLine();
            Console.WriteLine("WASD or arrows - move");
            Console.WriteLine("R - heal 10 health");
            Console.WriteLine("F - get 10 coins");
            Console.WriteLine("Esc - exit");
        }
    }
}