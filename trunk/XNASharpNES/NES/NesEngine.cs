// created on 10/24/2004 at 11:36
using System;
using System.IO;
using System.Threading;
//using Tao.Sdl;

public class NesEngine
{
	//Universal constants
	//FIXME: this assumes NTSC
	public static uint Ticks_Per_Scanline = 113;

	//The connections to tie the system together
	public NesCartridge myCartridge;
	public ProcessorNes6502 my6502;
	public Mapper myMapper;
	public PPU myPPU;
	Joypad myJoypad;
	public bool isQuitting;
	public bool hasQuit;
	public bool isDebugging;
	public bool isSaveRAMReadonly;
	public bool isPaused;
	public bool fix_bgchange;
	public bool fix_spritehit;
	public bool fix_scrolloffset1;
	public bool fix_scrolloffset2;
	public bool fix_scrolloffset3;
	
	byte counter;
	byte [][] scratchRam;
	byte [] saveRam;
	
	string saveFilename;
	
	public uint numOfInstructions;
	
	public byte ReadMemory8(ushort address)
	{
		byte returnvalue = 0;
		
		if (address < 0x2000)
		{
			
			if (address < 0x800) {
				returnvalue = scratchRam[0][address];
			}
			else if (address < 0x1000) {
				returnvalue = scratchRam[1][address - 0x800];
				//Console.WriteLine("I need ram mirroring {0:x}", address);
			}
			
			else if (address < 0x1800) {
				returnvalue = scratchRam[2][address - 0x1000];
				//Console.WriteLine("I need ram mirroring {0:x}", address);
			}
			//else if (address < 0x2000) {
			else {
				returnvalue = scratchRam[3][address - 0x1800];
				//Console.WriteLine("I need ram mirroring {0:x}", address);
			}
			
			//returnvalue = scratchRam[address >> 11][address & 0x7ff];
		}
		else if (address < 0x6000) {
			switch (address)
			{
				case (0x2002): returnvalue = myPPU.Status_Register_Read(); break;
				case (0x2004): returnvalue = myPPU.SpriteRam_IO_Register_Read(); break;
				case (0x2007): returnvalue = myPPU.VRAM_IO_Register_Read(); break;
				//case (0x4015): returnvalue = myPPU.Sound_Signal_Register_Read(); break;
				case (0x4016): returnvalue = myJoypad.Joypad_1_Read(); break;
				case (0x4017): returnvalue = myJoypad.Joypad_2_Read(); break;
				//default: Console.WriteLine("UNKOWN READ: {0:x}", address); break;
			} 
		}
		else if (address < 0x8000) {
			returnvalue = saveRam[address - 0x6000];
			if (myCartridge.mapper == 5)
				returnvalue = 1;
		}
		else {
			returnvalue = myMapper.ReadPrgRom(address);
		}
		return returnvalue;
	}
	
	//This is optimized for the places the PC can be
	//Except, it doesn't seem to be faster
	public byte ReadMemory8PC(ushort address)
	{
		byte returnvalue = 0;
		/*
		if ((address >= 0x2000) && (address < 0x6000))
		{
			Console.WriteLine("ERROR: PC = {0:x}", address);
			isQuitting = true;
		}
		*/
		if (address < 0x800) {
			returnvalue = scratchRam[0][address];
		}
		else if (address < 0x1000) {
			returnvalue = scratchRam[1][address - 0x800];
		}
		
		else if (address < 0x1800) {
			returnvalue = scratchRam[2][address - 0x1000];
		}
		else if (address < 0x2000) {
			returnvalue = scratchRam[3][address - 0x1800];
		}
		
		else if (/*(address >= 0x6000)&&*/(address < 0x8000)) {
			returnvalue = saveRam[address - 0x6000];
		}
		//else if (address >= 0x8000){
		else {
			returnvalue = myMapper.ReadPrgRom((ushort)address);
		}
		return returnvalue;
	}
	
	//NOTE: I'm replacing ReadMemory16, if there are changes
	//to ReadMemory please be sure to FIXME also
	/*
	public ushort ReadMemory16(ushort address)
	{
		byte data_1 = ReadMemory8(address);
		byte data_2 = ReadMemory8((ushort)(address+1));
		//Because of initialization order, we copy the code from Nes6502 to here
		ushort data = (ushort)data_2;
		data = (ushort)(data << 8);
		data += (ushort)data_1;
		
		return data;
	}
	*/
	
	public ushort ReadMemory16(ushort address)
	{
		byte returnvalue = 0;
		byte data_1;
		byte data_2;

		//FIXME: We assume it is not 0x2000-0x6000 because that
		//doesn't make any sense
		
		//FIXME: We also assume no boundaries are crossed
		//This may not be true, and may cause segfaults
		
		if (address < 0x2000)
		{
			if (address < 0x800) {
				data_1 = scratchRam[0][address];
				data_2 = scratchRam[0][address+1];
			}
			else if (address < 0x1000) {
				data_1 = scratchRam[1][address - 0x800];
				data_2 = scratchRam[1][address - 0x800 + 1];
			}
			
			else if (address < 0x1800) {
				data_1 = scratchRam[2][address - 0x1000];
				data_2 = scratchRam[2][address - 0x1000 + 1];
			}
			else {
				data_1 = scratchRam[3][address - 0x1800];
				data_2 = scratchRam[3][address - 0x1800 + 1];
			}
		}
		else if (address < 0x8000) {
			data_1 = saveRam[address - 0x6000];
			data_2 = saveRam[address - 0x6000 + 1];
			//FIXME: At some point I need to fix mapper 5
			
			//if (myCartridge.mapper == 5)
			//	returnvalue = 1;
			
		}
		else {
			data_1 = myMapper.ReadPrgRom(address);
			data_2 = myMapper.ReadPrgRom((ushort)(address + 1));
		}
		
		ushort data = (ushort)((data_2 << 8) + data_1);
		
		return data;
	}
	
	public byte WriteMemory8(ushort address, byte data)
	{
		if (address < 0x800) {
			scratchRam[0][address] = data; 
		}
		else if (address < 0x1000) {
			scratchRam[1][address - 0x800] = data; 
		}
		else if (address < 0x1800) {
			scratchRam[2][address - 0x1000] = data; 
		}
		else if (address < 0x2000) {
			scratchRam[3][address - 0x1800] = data; 
		}
		else if (address < 0x4000) {
			//address = (ushort)((address - 0x2000) % 8);
			switch (address)
			{
				case (0x2000): myPPU.Control_Register_1_Write(data); break;
				case (0x2001): myPPU.Control_Register_2_Write(data); break;
				case (0x2003): myPPU.SpriteRam_Address_Register_Write(data); break;
				case (0x2004): myPPU.SpriteRam_IO_Register_Write(data); break;
				case (0x2005): myPPU.VRAM_Address_Register_1_Write(data); break;
				case (0x2006): myPPU.VRAM_Address_Register_2_Write(data); break;
				case (0x2007): myPPU.VRAM_IO_Register_Write(data); break;
				//default: Console.WriteLine("UNKOWN CONTROL WRITE: {0}", address); break;
			}
		}
		else if (address < 0x6000)
		{
			switch (address) 
			{
				case (0x4014): myPPU.SpriteRam_DMA_Begin(data); break;
				case (0x4016): myJoypad.Joypad_Write(data); break;
				//default: Console.WriteLine("UNKOWN WRITE: {0:x}", address); break;
			}
			if (myCartridge.mapper == 5)
				myMapper.WritePrgRom(address, data);
		}
		else if (address < 0x8000)
		{
			if (!isSaveRAMReadonly)
				saveRam[address - 0x6000] = data;
			
			if (myCartridge.mapper == 34)
				myMapper.WritePrgRom(address, data);
		
		}
		else
		{
			myMapper.WritePrgRom(address, data);
		}
		return 1;
	}
	
	public byte WriteMemory16(ushort address, ushort data)
	{
		Console.WriteLine("** ERROR: WriteMemory16 was used **");
		
		return 255;
	}
	
	public void TogglePause()
	{
		if (isPaused)
		{
			isPaused = false;
            //myPPU.myVideo.SetWindowTitle("NES Window");
		}
		else
		{
			isPaused = true;
            //myPPU.myVideo.SetWindowTitle("[paused]");
		}
	}
	
	public void CheckForEvents()
	{
        //Sdl.SDL_Event myEvent;
		
        //while (Sdl.SDL_PollEvent(out myEvent) == 1)
        //{
        //    if (myEvent.type == Sdl.SDL_QUIT)
        //    {
        //        //QuitEngine();
        //    }
        //    else if (myEvent.type == Sdl.SDL_KEYDOWN)
        //    {
        //        if (myEvent.key.keysym.sym == (int)Sdl.SDLKey.SDLK_SPACE)
        //        {
        //            TogglePause();
        //        }
        //        else if (myEvent.key.keysym.sym == (int)Sdl.SDLKey.SDLK_f)
        //        {
        //            myPPU.myVideo.ToggleFullscreen();
        //        }
        //        //Console.WriteLine("Game Paused");
        //        //Console.WriteLine("KeyDown: {0}", myEvent.key.keysym.sym);
        //    }
        //    /*
        //    else if (myEvent.type == Sdl.SDL_MOUSEBUTTONDOWN)
        //    {
        //        isDebugging = true;
        //    }
        //    else if (myEvent.type == Sdl.SDL_MOUSEBUTTONUP)
        //    {
        //        isDebugging = false;
        //    }
        //    */
        //}
	}
	
	public void InitializeEngine()
	{
		myCartridge = new NesCartridge();
		my6502 = new ProcessorNes6502(this);
		myMapper = new Mapper(this, ref myCartridge);
		myPPU = new PPU(this);
		myJoypad = new Joypad();
		
		scratchRam = new byte[4][];
		scratchRam[0] = new byte[0x800];
		scratchRam[1] = new byte[0x800];
		scratchRam[2] = new byte[0x800];
		scratchRam[3] = new byte[0x800];
		saveRam = new byte[0x2000];
		
		isSaveRAMReadonly = false;
		isDebugging = false;
		isQuitting = false;
		isPaused = false;
		hasQuit = false;
		fix_bgchange = false;
		fix_spritehit = false;
		fix_scrolloffset1 = false;
		fix_scrolloffset2 = false;
		fix_scrolloffset3 = false;
	}
	
	public void RestartEngine()
	{
		isSaveRAMReadonly = false;
		isDebugging = false;
		isQuitting = false;
		isPaused = false;
		hasQuit = false;
		fix_bgchange = false;
		fix_spritehit = false;
		fix_scrolloffset1 = false;
		fix_scrolloffset2 = false;
		fix_scrolloffset3 = false;
		myPPU.RestartPPU();
        //myPPU.myVideo.SetWindowTitle("NES Window");
	}
	
	public void QuitEngine()
	{
		isQuitting = true;
	}

	public bool RenderNextScanline()
	{
		//This is a tie-in function to the PPU
		//If we decide later that the CPU should have direct visibility, this
		//won't be needed

        bool renderResult = myPPU.RenderNextScanline();
		
		if (renderResult)
		{
			//We entered VBlank and executeNMIonVBlank is set
			//Console.WriteLine("Entering --VBLANK--");
			my6502.Push16(my6502.pc_register);
			my6502.PushStatus();
			my6502.pc_register = ReadMemory16(0xFFFA);
		}
		
		if (myCartridge.mapper == 4)
		{
			myMapper.TickTimer();
		}

        return renderResult;	
	}
	
	public bool LoadCart(string filename)
	{
		byte [] nesHeader = new byte[16];
		int i;
		
		//DEBUG
		counter = 1;
		
		try {
			using(FileStream reader = File.OpenRead(filename))
			{
				reader.Read(nesHeader, 0, 16);
				
				//Allocate space, because of mappers we don't use the 16k default
				int prg_roms = nesHeader[4]*4;
				myCartridge.prg_rom_pages = nesHeader[4];
				//Console.WriteLine("Number of PRG pages: {0}", myCartridge.prg_rom_pages);
				myCartridge.prg_rom = new byte[prg_roms][];
				for (i = 0; i < (prg_roms); i++)
				{
					//Console.WriteLine("Create PRG Page: {0}", i);
					myCartridge.prg_rom[i] = new byte[4096];
					reader.Read(myCartridge.prg_rom[i], 0, 4096);
				}
				//Console.WriteLine("CHR ROM: {0}", nesHeader[5]);
				
				//Console.WriteLine("Zelda fix: {0}", fix_zelda);
				//Allocate space, because of mappers we don't use the 16k default
				int chr_roms = nesHeader[5]*8;
				myCartridge.chr_rom_pages = nesHeader[5];
				//Console.WriteLine("Number of CHR pages: {0}", myCartridge.chr_rom_pages);
				if (myCartridge.chr_rom_pages != 0)
				{
					myCartridge.chr_rom = new byte[chr_roms][];
					for (i = 0; i < (chr_roms); i++)
					{
						myCartridge.chr_rom[i] = new byte[1024];
						reader.Read(myCartridge.chr_rom[i], 0, 1024);
					}
					myCartridge.is_vram = false;
				}
				else
				{
					//If we have 0 CHR pages, we're dealing with VRAM instead of VROM
					//So make enough space by providing the minimum for 0x0000 and 0x1000
					//and set the toggle that allows us to to write to the video memory
					myCartridge.chr_rom = new byte[8][];
					for (i = 0; i < 8; i++)
					{
						myCartridge.chr_rom[i] = new byte[1024];
					}
					myCartridge.is_vram = true;
				}
				
				if ((nesHeader[6] & 0x1) == 0x0)
				{
					myCartridge.mirroring = MIRRORING.HORIZONTAL;
				}
				else
				{
					myCartridge.mirroring = MIRRORING.VERTICAL;
				}
				
				if ((nesHeader[6] & 0x2) == 0x0)
				{
					//Console.WriteLine("No Save RAM");
					myCartridge.save_ram_present = false;
				}
				else
				{
					//Console.WriteLine("Save RAM enabled");
					myCartridge.save_ram_present = true;
				}
				
				if ((nesHeader[6] & 0x4) == 0x0)
				{
					myCartridge.trainer_present = false;
				}
				else
				{
					myCartridge.trainer_present = true;
				}
				
				if ((nesHeader[6] & 0x8) != 0x0)
				{
					myCartridge.mirroring = MIRRORING.FOUR_SCREEN;
				}
				
				if (nesHeader[7] == 0x44)
				{
					//!DiskDude! garbage ignore
					myCartridge.mapper = (byte)(nesHeader[6] >> 4);
				}
				else
				{					
					myCartridge.mapper = (byte)((nesHeader[6] >> 4) + (nesHeader[7] & 0xf0));
				}
				if ((nesHeader[6] == 0x23) && (nesHeader[7] == 0x64))
					myCartridge.mapper = 2;
				
				//Console.WriteLine("Mirroring: {0}", myCartridge.mirroring);
				//Console.WriteLine("Mapper: {0}", myCartridge.mapper);
				
				//ID the rom, and enable fixes when necessary
				
				if ((myCartridge.prg_rom[prg_roms - 1][0xfeb] == 'Z') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfec] == 'E') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfed] == 'L') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfee] == 'D') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfef] == 'A'))
				{
					fix_bgchange = true;
					//Console.WriteLine("Workaround: BG Change");
				}
				
				if ((myCartridge.prg_rom[prg_roms - 1][0xfe0] == 'B') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfe1] == 'B') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfe2] == '4') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfe3] == '7') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfe4] == '9') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfe5] == '5') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfe6] == '6') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfe7] == '-') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfe8] == '1') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfe9] == '5') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfea] == '4') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfeb] == '4') &&
					(myCartridge.prg_rom[prg_roms - 1][0xfec] == '0'))
				{
					fix_scrolloffset1 = true;
					//Console.WriteLine("Workaround: Scroll Offset #1");
				}
				if ((myCartridge.prg_rom[0][0x9] == 0xfc) &&
					(myCartridge.prg_rom[0][0xa] == 0xfc) &&
					(myCartridge.prg_rom[0][0xb] == 0xfc) &&
					(myCartridge.prg_rom[0][0xc] == 0x40) &&
					(myCartridge.prg_rom[0][0xd] == 0x40) &&
					(myCartridge.prg_rom[0][0xe] == 0x40) &&
					(myCartridge.prg_rom[0][0xf] == 0x40))
				{
					fix_scrolloffset2 = true;
					//Console.WriteLine("Workaround: Scroll Offset #2");
				}
				if ((myCartridge.prg_rom[0][0x75] == 0x11) &&
					(myCartridge.prg_rom[0][0x76] == 0x12) &&
					(myCartridge.prg_rom[0][0x77] == 0x13) &&
					(myCartridge.prg_rom[0][0x78] == 0x14) &&
					(myCartridge.prg_rom[0][0x79] == 0x07) && 
					(myCartridge.prg_rom[0][0x7a] == 0x03) && 
					(myCartridge.prg_rom[0][0x7b] == 0x03) && 
					(myCartridge.prg_rom[0][0x7c] == 0x03) && 
					(myCartridge.prg_rom[0][0x7d] == 0x03)  
					)
				{
					fix_scrolloffset3 = true;
					//Console.WriteLine("Workaround: Scroll Offset #3");
				}
			
				
				//This should help with Dragon Strike
				//if (myCartridge.mapper == 4)
				//	fix_spritehit = true;
					
				myMapper.SetUpMapperDefaults();
				
				//my6502.RunProcessor();
				
				//myPPU.DumpVRAM();
			}
		}
		catch (FileNotFoundException e) {
			//Console.Error.WriteLine("File \"{0}\" does not exist.", filename);
			return false;
		}
		catch (Exception e) {
			//Console.Error.WriteLine(e);
			return false;
		}

        if (myCartridge.save_ram_present)
        {
            //If we have save RAM, try to load it
            saveFilename = filename.Remove(filename.Length - 3, 3);
            saveFilename = saveFilename.Insert(saveFilename.Length, "sav");
            //Console.WriteLine("SaveRAM enabled: {0}", saveFilename);
        }
				
		return true;
	}

    public string SaveRamDirectory = String.Empty;

    public void SaveRam()
    {
        if (myCartridge.save_ram_present)
        {
            //If we have save RAM, try to save it
            try
            {
                using (FileStream writer = File.OpenWrite(Path.Combine(SaveRamDirectory, saveFilename)))
                {
                    writer.Write(saveRam, 0, 0x2000);
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("SaveRAM could not be saved.");
            }
        }
    }

    public void LoadRam()
    {
        if (myCartridge.save_ram_present)
        {
            try
            {
                using (FileStream reader = File.OpenRead(Path.Combine(SaveRamDirectory, saveFilename)))
                {
                    reader.Read(saveRam, 0, 0x2000);
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("No SaveRAM found.");
                //Ignore it, we'll make our own.
            }
        }
    }

    public void StartCart()
    {
        my6502.pc_register = ReadMemory16(0xFFFC);
        myPPU.myVideo.StartVideo();
    }

	public void RunCart()
	{
		try{
			my6502.RunProcessor();
		}
		catch (ThreadAbortException tae) {
            SaveRam();
        }
	}

    public void StopCart()
    {
        SaveRam();
        hasQuit = true;
        myPPU.myVideo.CloseVideo();
    }
	
	public NesEngine()
	{
		InitializeEngine();
		
        //myPPU.myVideo.StartVideo();
	}
}