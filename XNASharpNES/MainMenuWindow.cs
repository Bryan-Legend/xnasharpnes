//// created on 2/4/2005 at 8:26 PM
//using System;
//using Gtk;
//using Gdk;
//using GtkSharp;
//using System.Threading;

//public class MainMenuWindow : Gtk.Window
//{
//    private Thread gameThread;
//    private bool gameIsRunning;
//    private NesEngine myEngine;
//    private ThreadStart myThreadCreator;
	
//    public MainMenuWindow (string defaultROM) : base ("SharpNES v0.3")
//    {
//        this.SetDefaultSize (300, 200);
//        this.DeleteEvent += new DeleteEventHandler (OnMyWindowDelete);
//        MenuBar mb = new MenuBar();
//        AccelGroup agrp = new AccelGroup();
//        this.AddAccelGroup(agrp);
		
//        Menu file_menu = new Menu();
//        MenuItem item = new MenuItem("_File");
//        item.Submenu = file_menu;
//        mb.Append(item);
//        item = new ImageMenuItem(Stock.Open, agrp);
//        item.Activated += Open_Activated;
//        file_menu.Append(item);
			
//        file_menu.Append(new SeparatorMenuItem());
		
//        item = new ImageMenuItem(Stock.Quit, agrp);
//        item.Activated += Quit_Activated;
//        file_menu.Append(item);

//        Menu game_menu = new Menu();
//        item = new MenuItem("_Game");
//        item.Submenu = game_menu;
//        mb.Append(item);
		
//        item = new MenuItem("Play/Pause");
//        item.AddAccelerator("activate", agrp, new AccelKey(Gdk.Key.space, 0, AccelFlags.Visible));
		
//        item.Activated += Play_Pause_Activated;
//        game_menu.Append(item);
		
//        /*
//        item = new MenuItem("Full Screen");
//        item.AddAccelerator("activate", agrp, new AccelKey(Gdk.Key.F, 0, AccelFlags.Visible));
//        item.Activated += Full_Screen_Activated;
//        game_menu.Append(item);
//        */
//        VBox v = new VBox();
//        v.PackStart(mb, false, false, 0);
//        this.Add (v);
		
//        this.ShowAll ();
						
//        //our game-specific bool
//        //FIXME: move this to a more sane place when I figure out where to put it
//        gameIsRunning = false;
//        myEngine = new NesEngine();
//        //If they gave us a ROM to run on the commandline, go ahead and start it up 
//        if (defaultROM != "")
//        {
//            Run_Cart(defaultROM);
//        }
//    }
	
//    void Run_Cart(string filename)
//    {
//        if (gameIsRunning)
//        {
//            myEngine.QuitEngine();
//            //while (!myEngine.hasQuit);
			
//            gameThread.Join();
//            gameIsRunning = false; 
//            myEngine.RestartEngine();
//        }
		
//        if (myEngine.LoadCart(filename))
//        {
//            myThreadCreator = new ThreadStart(myEngine.RunCart);
//            gameThread = new Thread(myThreadCreator);
//            gameThread.Start();
//            //myEngine.RunCart();
//            gameIsRunning = true;
//        }
//        else
//        {
//            MessageDialog md = new MessageDialog (this, 
//                                    DialogFlags.DestroyWithParent,
//                                    MessageType.Error, 
//                                    ButtonsType.Close, "Error loading file");
     
//            int result = md.Run ();
//            md.Destroy();
//            gameIsRunning = false;
//        }
//    }
	
//    void Open_Activated(object o, EventArgs e)
//    {
//        FileSelection fs = new FileSelection("Select the ROM file");
//        fs.Run();
//        fs.Hide();
//        //Console.WriteLine("Selected: " + fs.Filename);
//        //Console.WriteLine("Selected Entry: '" + fs.SelectionEntry.Text + "'");
//        if (fs.SelectionEntry.Text != "")
//        {
//            Run_Cart(fs.Filename);
//        }
//    }
	
//    void Play_Pause_Activated(object o, EventArgs e)
//    {
//        myEngine.TogglePause();
//    }
	
//    void Full_Screen_Activated(object o, EventArgs e)
//    {
//        myEngine.myPPU.myVideo.ToggleFullscreen();
//    }
	
//    void Quit_Activated(object o, EventArgs e)
//    {
//        if (!myEngine.isQuitting)
//        {
//            myEngine.QuitEngine();
//            //gameThread.Join();
//        }
//        Application.Quit ();
//    }
	
//    void OnMyWindowDelete (object o, DeleteEventArgs args)
//    {
//        if (!myEngine.isQuitting)
//        {
//            myEngine.QuitEngine();
//            //gameThread.Join();
//        }
//        Application.Quit ();
//    }
//}
