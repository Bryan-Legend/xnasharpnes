        __  __    __  _     __ _                          __  __  __    
        \ \/ / /\ \ \/_\   / _\ |__   __ _ _ __ _ __   /\ \ \/__\/ _\   
         \  / /  \/ //_\\  \ \| '_ \ / _` | '__| '_ \ /  \/ /_\  \ \    
         /  \/ /\  /  _  \ _\ \ | | | (_| | |  | |_) / /\  //__  _\ \   
        /_/\_\_\ \/\_/ \_/ \__/_| |_|\__,_|_|  | .__/\_\ \/\__/  \__/   
                                               |_|                      
              the first NES emulator for the Xbox 360 and XNA


The project page for this project is at:
	http://code.google.com/p/xnasharpnes/

The latest release can be downloaded from:
	http://code.google.com/p/xnasharpnes/downloads/list

You can get the latest source from our subversion repository at: 
	http://code.google.com/p/xnasharpnes/source

The development mailing list for the project is at:
	http://groups.google.com/group/xnasharpnes


You'll need to put your roms in the SharpNES360\bin\Xbox 360\Debug directory
and then run the emulator to deploy them to the xbox.

When the emulator runs it first pops up the prompt for storage devices, this
is so that it knows where to load and save your savegames.

It runs at 60% or 70% of normal speed while being debugged, but runs at full
speed when not running with a debugger on the 360.

This requires a creators club account which runs $99 a year or $49 for four
months.  You'll also need the free Visual C# Studio Express and the XNA bits.


Controls:
	A = A
	B = X or B
	Start = Start
	Select = Back
	Up, Down, Left, Right = D-Pad or Left Thumbstick

	* With the first controller:
		Return to Rom Menu = RB (Right Shoulder Button) + Back
		Exit Emu from Rom Menu = Back


Supported Mappers:
	Mappers 1 (mostly), 2, 3, 4 (mostly), 7 (partial), 9 (mostly), 10, 
	  11 (partial), 22 (partial), 34, 64 (partial), 66


What's Still Missing:
	Sound (Might I suggest humming along or retro game music from the 360)
	Full Mapper Support
	NES State Saving and Loading


A Note About Sound:
	Josh Dersch, author of DotNES (another NES emu written in C#,
	http://yahozna.dhs.org/projects/NES/index.html) has given us permission
	to use his sound rendering classes when he releases the source code.

	Unfortunatly, they won't do us any good.  The Audio API's in XNA do not
	allow for writing to sound buffers.  THIS MEANS THAT NO EMU OR MEDIA
	PLAYER CAN HAVE SOUND SUPPORT ON XNA IN ITS CURRENT STATE.  I've talked
	with MS about this and they said it is not intentionally blocked but is
	just a side effect of using the XACT system.  It's something that they
	didn't get to, which is understandable considering how fast they built
	XNA for us.  Please feel free to complain loudly to xna@microsoft.com.


Change Log:
	Release 3 ()
		Rom Loading Menu - Contributed by Thomas Aylesworth		
			Font classes from XNA Extras by Gary Kacmarcik at
			http://blogs.msdn.com/garykac/articles/749188.aspx
		
		Several bug fixes by Kerry McCullough

		New logo designed by Matt Turnbull

		Windows executables are now included in the release.  No need
		to compile it yourself to run on windows.

		New mailing list setup for dev talk at 
		http://groups.google.com/group/xnasharpnes

	Release 2 (1-28-2007)
		Second controller fixed (Yea for Luigi fans!)
		Made thumbstick work as an alternate control
		Battery saves are stored to disk
			(Be sure to exit the emu or it won't save)
		Crude rom navigation
		Emu remembers which rom was last loaded and will load it up
		Minor performance improvements
		Emu runs at full speed when not being debugged

	Release 1 (1-26-2007)
		Initial Release


Contributors:
	Bryan Livingston - Lone Coder - Developer & Project Manager
	Thomas Aylesworth - SwampThingTom - Developer
	Kerry McCullough - SubOneND - Developer
	Matt Turnbull - BigBaconAndEggs - Logo Designer
	Jonathan Turner - Original Sharp NES author
		http://jturner.tapetrade.net/sharpnes/index.html


Shoutouts:
	Jonathan Turner, Dilvie of http://KnobTweakers.net, Sambena, Fenrx, 
	Chia Pet of Borg and Acius of http://www.SparkArts.org/, Victor of
	http://FlatRedBall.com, Shawn Hargreaves (Allegro rocked!), The crew
	at http://NinjaBee.com and Greg Squire of http://UtahIndieGames.org


My other sites and projects:
	http://CoolText.com
	http://GlobalCombat.com
	http://designer.CoolText.com
	http://ZennyExchange.com


Special Plug:
	http://www.SparkArts.org/
	The first annual SparkArts Digital Arts Festival will be held in Salt
	Lake City, USA, in October, 2007.  This is pretty much going to be a
	demo party with a variety of different contests including a game dev
	contest.  Please check it out and consider attending.