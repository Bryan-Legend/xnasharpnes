//// project created on 10/22/2004 at 18:36
//using System;
//using System.Runtime.InteropServices;
////using Gtk;


//class MainClass
//{
//    public static void Main(string[] args)
//    {
//        Application.Init ();
//        //If they gave us a rom name on the commandline, run it.
//        if (args.GetLength(0) == 0)
//        {
//            new MainMenuWindow ("");
//        }
//        else
//        {
//            new MainMenuWindow (args[0]);
//        }
//        Application.Run ();
		
//        /*
//        EngineBase myEngine = new NesEngine();
		
//        if (args.GetLength(0) == 0)
//        {
//            Console.WriteLine("Please specify a filename.");
//            Environment.Exit(1);
//        }
		
//        myEngine.LoadCart(args[0], args[0]);
//        myEngine.RunCart();
//        */
//    }
//}