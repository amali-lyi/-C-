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
                        if (player.Y > 0)
                            player.Y--;
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
            Console.WriteLine("WASD or arrows - move");
            Console.WriteLine("Esc - exit");
        }
    }
}
