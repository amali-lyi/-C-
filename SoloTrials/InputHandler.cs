using System;

namespace SoloTrials
{
    public class InputHandler
    {
        public ConsoleKey ReadInput()
        {
            return Console.ReadKey(true).Key;
        }
    }
}
