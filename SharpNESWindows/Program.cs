using System;

namespace XNASharpNES
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //if (args.GetLength(0) == 0)
            //{
            //    Console.WriteLine("Please specify a filename.");
            //    Environment.Exit(1);
            //}

            using (SharpNES game = new SharpNES())
            {
                //game.LoadCart(args[0]);
                game.Run();
            }
        }
    }
}