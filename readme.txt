        __  __    __  _     __ _                          __  __  __    
        \ \/ / /\ \ \/_\   / _\ |__   __ _ _ __ _ __   /\ \ \/__\/ _\   
         \  / /  \/ //_\\  \ \| '_ \ / _` | '__| '_ \ /  \/ /_\  \ \    
         /  \/ /\  /  _  \ _\ \ | | | (_| | |  | |_) / /\  //__  _\ \   
        /_/\_\_\ \/\_/ \_/ \__/_| |_|\__,_|_|  | .__/\_\ \/\__/  \__/   
                                               |_|                      
              the first NES emulator for the xbox 360 and XNA.

If you'd like to talk about this project my gamer tag is Lone Coder or you can
email me at bryanlivingston at gmail dot com

The project page for this project is at: http://code.google.com/p/xnasharpnes/

The latest release can be downloaded from:
http://code.google.com/p/xnasharpnes/downloads/list

You can get the latest source from our subversion repository at: 
http://code.google.com/p/xnasharpnes/source

This is a conversion of SharpNES by Jonathan Turner.  Converting it only took
a couple of hours.  http://jturner.tapetrade.net/sharpnes/index.html

You'll need to put your roms in the SharpNES360\bin\Xbox 360\Debug directory
and then run the emulator to deploy them to the xbox.

When the emulator runs it first pops up the prompt for storage devices, this
is so that it knows where to load and save your savegames.

It runs at 60% or 70% of normal speed while being debugged, but now runs at
full speed when not running with a debugger on the 360.

This requires a creators club account which runs $99 a year or $49 for four
months.  You'll also need the free Visual C# Studio Express and the XNA bits.


Controls:
	A = A
	B = X or B
	Start = Start
	Select = Back
	Up, Down, Left, Right = D-Pad or Left Thumbstick

	* With the first controller:
		Exit Emulator = RB (Right Shoulder Button) + Back
		Load Previous Rom = RB + Up or RB + Left
		Load Next Rom = RB + Down or RB + Right


What's Still Missing:
	Real Rom Loading Menu
	Sound (Might I suggest humming along or retro game music from the 360)
	Full Mapper Support


Supported Mappers:
	Mappers 1 (mostly), 2, 3, 4 (mostly), 7 (partial), 9 (mostly), 10, 
	  11 (partial), 22 (partial), 34, 64 (partial), 66


Change Log:
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


A Note from the Author:

	I'm not much interested in coding up the mappers or the sound and I
	will probably not work on a real rom menu or more gui options until
	CEGUI# works or some nice gui components are published.

	So if anyone would like to contribute on these, please step up as I'm
	probably done with this project for a while.


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