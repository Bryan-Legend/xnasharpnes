using System;

using XNASharpNES;

namespace SharpNES360
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SharpNES game = new SharpNES())
            {
                game.Run();
            }
        }
    }
}
