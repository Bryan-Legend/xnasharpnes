// created on 10/30/2004 at 09:54
using System;

public class Mapper
{
	NesCartridge mapperCartridge;
	NesEngine myEngine;
	
	public uint [] current_prg_rom_page;
	public uint [] current_chr_rom_page;
	
	//Mapper specific vars, there will be redundancy here, but for now they
	//are separated to make it easier to keep track.  These could easily be compressed
	//down to a handful of variables, since roms don't change mappers, but it's easier 
	//to debug this way.
	
	//Mapper #1:
	
	int mapper1_register8000BitPosition;
	int mapper1_registerA000BitPosition;
	int mapper1_registerC000BitPosition;
	int mapper1_registerE000BitPosition;
	int mapper1_register8000Value;
	int mapper1_registerA000Value;
	int mapper1_registerC000Value;
	int mapper1_registerE000Value;
	
	byte mapper1_mirroringFlag;
	byte mapper1_onePageMirroring;
	byte mapper1_prgSwitchingArea;
	byte mapper1_prgSwitchingSize;
	byte mapper1_vromSwitchingSize;
	
	byte mapper4_commandNumber;
	byte mapper4_prgAddressSelect;
	byte mapper4_chrAddressSelect;
	
	byte mapper5_prgBankSize;
	byte mapper5_chrBankSize;
	int mapper5_scanlineSplit;
	bool mapper5_splitIrqEnabled;
		
	//These are for Mappers 9 and 10
	byte latch1, latch2;
	int latch1data1, latch1data2;
	int latch2data1, latch2data2;

	byte mapper64_commandNumber;
	byte mapper64_prgAddressSelect;
	byte mapper64_chrAddressSelect;
		
	public bool timer_irq_enabled;
	public bool timer_reload_next;
	public uint timer_irq_count, timer_irq_reload;
	bool timer_zero_pulse;  //the single pulse timer
	
	public byte ReadChrRom(ushort address)
	{
		byte returnvalue = 0xff;
		
		if (address < 0x400)
		{
			returnvalue = mapperCartridge.chr_rom[current_chr_rom_page[0]][address];
		}
		else if (address < 0x800)
		{
			returnvalue = mapperCartridge.chr_rom[current_chr_rom_page[1]][address - 0x400];
		}
		else if (address < 0xC00)
		{
			returnvalue = mapperCartridge.chr_rom[current_chr_rom_page[2]][address - 0x800];
		}
		else if (address < 0x1000)
		{
			if (mapperCartridge.mapper == 9)
			{
				if ((address >= 0xfd0)&&(address <= 0xfdf))
				{
					latch1 = 0xfd;
					Switch4kChrRom(latch1data1, 1);
				}
				else if ((address >= 0xfe0)&&(address <= 0xfef))
				{
					latch1 = 0xfe;
					Switch4kChrRom(latch1data2, 1);
				}
			}
			else if (mapperCartridge.mapper == 10)
			{
				if ((address >= 0xfd0)&&(address <= 0xfdf))
				{
					latch1 = 0xfd;
					Switch4kChrRom(latch1data1, 0);
				}
				else if ((address >= 0xfe0)&&(address <= 0xfef))
				{
					latch1 = 0xfe;
					Switch4kChrRom(latch1data2, 0);
				}
			}
			returnvalue = mapperCartridge.chr_rom[current_chr_rom_page[3]][address - 0xC00];
		}
		else if (address < 0x1400)
		{
			returnvalue = mapperCartridge.chr_rom[current_chr_rom_page[4]][address - 0x1000];
		}
		else if (address < 0x1800)
		{
			returnvalue = mapperCartridge.chr_rom[current_chr_rom_page[5]][address - 0x1400];
		}
		else if (address < 0x1C00)
		{
			returnvalue = mapperCartridge.chr_rom[current_chr_rom_page[6]][address - 0x1800];
		}
		else
		{
			if (mapperCartridge.mapper == 9)
			{
				if ((address >= 0x1fd0)&&(address <= 0x1fdf))
				{
					latch1 = 0xfd;
					Switch4kChrRom(latch1data1, 1);
				}
				else if ((address >= 0x1fe0)&&(address <= 0x1fef))
				{
					latch1 = 0xfe;
					Switch4kChrRom(latch1data2, 1);
				}
			}
			else if (mapperCartridge.mapper == 10)
			{
				if ((address >= 0x1fd0)&&(address <= 0x1fdf))
				{
					latch2 = 0xfd;
					Switch4kChrRom(latch2data1, 1);
				}
				else if ((address >= 0x1fe0)&&(address <= 0x1fef))
				{
					latch2 = 0xfe;
					Switch4kChrRom(latch2data2, 1);
				}
			}
			
			returnvalue = mapperCartridge.chr_rom[current_chr_rom_page[7]][address - 0x1C00];
			
		}
		
		return returnvalue;
	}
	
	void Switch32kPrgRom(int start)
	{
		int i;
		switch (mapperCartridge.prg_rom_pages)
		{
			case (2): start = (start & 0x7); break;
			case (4): start = (start & 0xf); break;
			case (8): start = (start & 0x1f); break;
			case (16): start = (start & 0x3f); break;
			case (32): start = (start & 0x7f); break;
		}
		for (i = 0; i < 8; i++)
		{
			current_prg_rom_page[i] = (uint)(start + i);
		}
		/*
		Console.Write("[");
		for (i = 0; i < 8; i++)
			Console.Write("{0} ", (uint)(current_prg_rom_page[i]));
		Console.WriteLine("]");
		*/
	}

	void Switch16kPrgRom(int start, int area)
	{
		int i;
		switch (mapperCartridge.prg_rom_pages)
		{
			case (2): start = (start & 0x7); break;
			case (4): start = (start & 0xf); break;
			case (8): start = (start & 0x1f); break;
			case (16): start = (start & 0x3f); break;
			case (32): start = (start & 0x7f); break;
		}

		for (i = 0; i < 4; i++)
		{
			current_prg_rom_page[4*area + i] = (uint)(start + i);
		}
		/*
		Console.Write("[");
		for (i = 0; i < 8; i++)
			Console.Write("{0} ", (uint)(current_prg_rom_page[i]));
		Console.WriteLine("]");
		*/
	}

	void Switch8kPrgRom(int start, int area)
	{
		int i;
		switch (mapperCartridge.prg_rom_pages)
		{
			case (2): start = (start & 0x7); break;
			case (4): start = (start & 0xf); break;
			case (8): start = (start & 0x1f); break;
			case (16): start = (start & 0x3f); break;
			case (32): start = (start & 0x7f); break;
		}
		for (i = 0; i < 2; i++)
		{
			current_prg_rom_page[2*area + i] = (uint)(start + i);
		}
	}
	void Switch8kChrRom(int start)
	{
		int i;
		switch (mapperCartridge.chr_rom_pages)
		{
			case (2): start = (start & 0xf); break;
			case (4): start = (start & 0x1f); break;
			case (8): start = (start & 0x3f); break;
			case (16): start = (start & 0x7f); break;
			case (32): start = (start & 0xff); break;
		}
		for (i = 0; i < 8; i++)
		{
			current_chr_rom_page[i] = (uint)(start + i);
		}
	}
	void Switch4kChrRom(int start, int area)
	{
		int i;
		switch (mapperCartridge.chr_rom_pages)
		{
			case (2): start = (start & 0xf); break;
			case (4): start = (start & 0x1f); break;
			case (8): start = (start & 0x3f); break;
			case (16): start = (start & 0x7f); break;
			case (32): start = (start & 0xff); break;
		}
		for (i = 0; i < 4; i++)
		{
			current_chr_rom_page[4*area + i] = (uint)(start + i);
		}
	}
	void Switch2kChrRom(int start, int area)
	{
		int i;
		switch (mapperCartridge.chr_rom_pages)
		{
			case (2): start = (start & 0xf); break;
			case (4): start = (start & 0x1f); break;
			case (8): start = (start & 0x3f); break;
			case (16): start = (start & 0x7f); break;
			case (32): start = (start & 0xff); break;
		}
		for (i = 0; i < 2; i++)
		{
			current_chr_rom_page[2*area + i] = (uint)(start + i);
		}
	}
	void Switch1kChrRom(int start, int area)
	{
		switch (mapperCartridge.chr_rom_pages)
		{
			case (2): start = (start & 0xf); break;
			case (4): start = (start & 0x1f); break;
			case (8): start = (start & 0x3f); break;
			case (16): start = (start & 0x7f); break;
			case (32): start = (start & 0xff); break;
		}
		current_chr_rom_page[area] = (uint)(start);
	}
	
	public byte ReadPrgRom(ushort address)
	{
		byte returnvalue = 0xff;
		
		if (address < 0x9000)
		{
			returnvalue = mapperCartridge.prg_rom[current_prg_rom_page[0]][address - 0x8000];
		}
		else if (address < 0xA000)
		{
			returnvalue = mapperCartridge.prg_rom[current_prg_rom_page[1]][address - 0x9000];
		}
		else if (address < 0xB000)
		{
			returnvalue = mapperCartridge.prg_rom[current_prg_rom_page[2]][address - 0xA000];
		}
		else if (address < 0xC000)
		{
			returnvalue = mapperCartridge.prg_rom[current_prg_rom_page[3]][address - 0xB000];
		}
		else if (address < 0xD000)
		{
			returnvalue = mapperCartridge.prg_rom[current_prg_rom_page[4]][address - 0xC000];
		}
		else if (address < 0xE000)
		{
			returnvalue = mapperCartridge.prg_rom[current_prg_rom_page[5]][address - 0xD000];
		}
		else if (address < 0xF000)
		{
			returnvalue = mapperCartridge.prg_rom[current_prg_rom_page[6]][address - 0xE000];
		}
		else
		{
			returnvalue = mapperCartridge.prg_rom[current_prg_rom_page[7]][address - 0xF000];
		}
		
		return returnvalue;
	}
	public void WriteChrRom(ushort address, byte data)
	{
		if (mapperCartridge.is_vram == true)
		{
			if (address < 0x400)
			{
				mapperCartridge.chr_rom[current_chr_rom_page[0]][address] = data;
			}
			else if (address < 0x800)
			{
				mapperCartridge.chr_rom[current_chr_rom_page[1]][address - 0x400] = data;
			}
			else if (address < 0xC00)
			{
				mapperCartridge.chr_rom[current_chr_rom_page[2]][address - 0x800] = data;
			}
			else if (address < 0x1000)
			{
				mapperCartridge.chr_rom[current_chr_rom_page[3]][address - 0xC00] = data; 
			}
			else if (address < 0x1400)
			{
				mapperCartridge.chr_rom[current_chr_rom_page[4]][address - 0x1000] = data;
			}
			else if (address < 0x1800)
			{
				mapperCartridge.chr_rom[current_chr_rom_page[5]][address - 0x1400] = data;
			}
			else if (address < 0x1C00)
			{
				mapperCartridge.chr_rom[current_chr_rom_page[6]][address - 0x1800] = data;
			}
			else
			{
				mapperCartridge.chr_rom[current_chr_rom_page[7]][address - 0x1C00] = data;
			}
			
		}
	}	
	public void WritePrgRom(ushort address, byte data)
	{
		int i;
		
		//Console.WriteLine("Mapper: {0}  Address: 0x{1:x}  Data: {2:x}  PC: {3:x}", mapperCartridge.mapper, address, data, myEngine.my6502.pc_register);
		
		if (mapperCartridge.mapper == 1)
		{
			//Using Mapper #1
		
			if ((address >= 0x8000) && (address <= 0x9FFF))
			{
				if ((data & 0x80) == 0x80)
				{
					//Reset
					mapper1_register8000BitPosition = 0;
					mapper1_register8000Value = 0;
					mapper1_mirroringFlag = 0;
					mapper1_onePageMirroring = 1;
					mapper1_prgSwitchingArea = 1;
					mapper1_prgSwitchingSize = 1;
					mapper1_vromSwitchingSize = 0;
				}
				else
				{
					mapper1_register8000Value += (data & 0x1) << mapper1_register8000BitPosition;
					mapper1_register8000BitPosition++;
					if (mapper1_register8000BitPosition == 5)
					{
						mapper1_mirroringFlag = (byte)(mapper1_register8000Value & 0x1);
						if (mapper1_mirroringFlag == 0)
						{
							mapperCartridge.mirroring = MIRRORING.VERTICAL;
						}
						else
						{
							mapperCartridge.mirroring = MIRRORING.HORIZONTAL;
						}
						mapper1_onePageMirroring = (byte)((mapper1_register8000Value >> 1) & 0x1);
						if (mapper1_onePageMirroring == 0)
						{
							mapperCartridge.mirroring = MIRRORING.ONE_SCREEN;
							mapperCartridge.mirroringBase = 0x2000;
						}
						mapper1_prgSwitchingArea = (byte)((mapper1_register8000Value >> 2) & 0x1);
						mapper1_prgSwitchingSize = (byte)((mapper1_register8000Value >> 3) & 0x1);
						mapper1_vromSwitchingSize = (byte)((mapper1_register8000Value >> 4) & 0x1);
						mapper1_register8000BitPosition = 0;
						mapper1_register8000Value = 0;
						mapper1_registerA000BitPosition = 0;
						mapper1_registerA000Value = 0;
						mapper1_registerC000BitPosition = 0;
						mapper1_registerC000Value = 0;
						mapper1_registerE000BitPosition = 0;
						mapper1_registerE000Value = 0;
					}
				}
			}
			else if ((address >= 0xA000) && (address <= 0xBFFF))
			{
				if ((data & 0x80) == 0x80)
				{
					//Reset
					mapper1_registerA000BitPosition = 0;
					mapper1_registerA000Value = 0;
				}
				else
				{
					mapper1_registerA000Value += (data & 0x1) << mapper1_registerA000BitPosition;
					mapper1_registerA000BitPosition++;
					if (mapper1_registerA000BitPosition == 5)
					{
						if (mapperCartridge.chr_rom_pages > 0)
						{
							if (mapper1_vromSwitchingSize == 1)
							{
								Switch4kChrRom(mapper1_registerA000Value * 4, 0);
							}
							else
							{
								Switch8kChrRom((mapper1_registerA000Value >> 1) * 8);
							}
						}
						mapper1_registerA000BitPosition = 0;
						mapper1_registerA000Value = 0;
					}
				}
			}
			else if ((address >= 0xC000) && (address <= 0xDFFF))
			{
				if ((data & 0x80) == 0x80)
				{
					//Reset
					mapper1_registerC000BitPosition = 0;
					mapper1_registerC000Value = 0;
				}
				else
				{
					mapper1_registerC000Value += (data & 0x1) << mapper1_registerC000BitPosition;
					mapper1_registerC000BitPosition++;
					if (mapper1_registerC000BitPosition == 5)
					{
						if (mapperCartridge.chr_rom_pages > 0)
						{
							if (mapper1_vromSwitchingSize == 1)
							{
								Switch4kChrRom(mapper1_registerC000Value * 4, 1);
							}
						}
						mapper1_registerC000BitPosition = 0;
						mapper1_registerC000Value = 0;
					}
				}
			}
			else if ((address >= 0xE000) && (address <= 0xFFFF))
			{
				if ((data & 0x80) == 0x80)
				{
					//Reset
					mapper1_registerE000BitPosition = 0;
					mapper1_registerE000Value = 0;
					mapper1_registerA000BitPosition = 0;
					mapper1_registerA000Value = 0;
					mapper1_registerC000BitPosition = 0;
					mapper1_registerC000Value = 0;
					mapper1_register8000BitPosition = 0;
					mapper1_register8000Value = 0;
				}
				else
				{
					mapper1_registerE000Value += (data & 0x1) << mapper1_registerE000BitPosition;
					mapper1_registerE000BitPosition++;
					if (mapper1_registerE000BitPosition == 5)
					{
						if (mapper1_prgSwitchingSize == 1)
						{
							if (mapper1_prgSwitchingArea == 1)
							{
								// Switch bank at 0x8000 and reset 0xC000
								Switch16kPrgRom(mapper1_registerE000Value * 4, 0);
								Switch16kPrgRom((mapperCartridge.prg_rom_pages - 1) * 4, 1);
							}
							else
							{
								// Switch bank at 0xC000 and reset 0x8000
								Switch16kPrgRom(mapper1_registerE000Value * 4, 1);
								Switch16kPrgRom(0, 0);
							}
						}
						else
						{
							//Full 32k switch
							Switch32kPrgRom((mapper1_registerE000Value >> 1) * 8);
						}
						mapper1_registerE000BitPosition = 0;
						mapper1_registerE000Value = 0;
					}
				}
			}
				
			//End Mapper #1	
		}
		else if (mapperCartridge.mapper == 2)
		{
			//Using Mapper #2
			
			if ((address >= 0x8000) && (address <= 0xFFFF))
			{
				Switch16kPrgRom(data * 4, 0);
			}
			
			//End Mapper #2
		}
		else if (mapperCartridge.mapper == 3)
		{
			//Using Mapper #3

			if ((address >= 0x8000) && (address <= 0xFFFF))
			{
				Switch8kChrRom(data * 8);
			}	
			
			//End Mapper #3
		}
		else if (mapperCartridge.mapper == 4)
		{
			//Using Mapper #4
			
			if (address == 0x8000)
			{
				mapper4_commandNumber = (byte)(data & 0x7);
				mapper4_prgAddressSelect = (byte)(data & 0x40);
				mapper4_chrAddressSelect = (byte)(data & 0x80);
				
			}
			else if (address == 0x8001)
			{
				if (mapper4_commandNumber == 0)
				{
					data = (byte)(data - (data % 2));
					if (mapper4_chrAddressSelect == 0)
					{
						Switch2kChrRom(data, 0);
					}
					else
					{
						Switch2kChrRom(data, 2);
					}
				}
				else if (mapper4_commandNumber == 1)
				{
					data = (byte)(data - (data % 2));
					if (mapper4_chrAddressSelect == 0)
					{
						Switch2kChrRom(data, 1);
					}
					else
					{
						Switch2kChrRom(data, 3);
					}
				}
				else if (mapper4_commandNumber == 2)
				{
					data = (byte)(data & (mapperCartridge.chr_rom_pages * 8 - 1));
					if (mapper4_chrAddressSelect == 0)
					{
						Switch1kChrRom(data, 4);
					}
					else
					{
						Switch1kChrRom(data, 0);
					}
				}
				else if (mapper4_commandNumber == 3)
				{
					if (mapper4_chrAddressSelect == 0)
					{
						Switch1kChrRom(data, 5);
					}
					else
					{
						Switch1kChrRom(data, 1);
					}
				}
				else if (mapper4_commandNumber == 4)
				{
					if (mapper4_chrAddressSelect == 0)
					{
						Switch1kChrRom(data, 6);
					}
					else
					{
						Switch1kChrRom(data, 2);
					}
				}
				else if (mapper4_commandNumber == 5)
				{
					if (mapper4_chrAddressSelect == 0)
					{
						Switch1kChrRom(data, 7);
					}
					else
					{
						Switch1kChrRom(data, 3);
					}
				}
				else if (mapper4_commandNumber == 6)
				{

					if (mapper4_prgAddressSelect == 0)
					{
						Switch8kPrgRom(data * 2, 0);
					}
					else
					{
						Switch8kPrgRom(data * 2, 2);
					}
					if (mapper4_prgAddressSelect == 0)
					{
						Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 4, 2);
					}
					else
					{
						Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 4, 0);
					}
				}
				else if (mapper4_commandNumber == 7)
				{
					Switch8kPrgRom(data * 2, 1);
					if (mapper4_prgAddressSelect == 0)
					{
						Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 4, 2);
					}
					else
					{
						Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 4, 0);
					}
					
				}
				
			}
			else if (address == 0xA000)
			{
				if ((data & 1) == 1)
				{
					mapperCartridge.mirroring = MIRRORING.HORIZONTAL;
				}
				else
				{
					mapperCartridge.mirroring = MIRRORING.VERTICAL;
				}
				//Console.WriteLine("Changed Mirroring: {0}", 
				//	mapperCartridge.mirroring);
			}
			else if (address == 0xA001)
			{
				if (data == 0)
					myEngine.isSaveRAMReadonly = true;
				else
					myEngine.isSaveRAMReadonly = false;
			}
			else if (address == 0xC000)
			{
				timer_irq_reload = data;
				if (data == 0)
				{
					timer_zero_pulse = true;
				}
				timer_reload_next = true;
			}
			else if (address == 0xC001)
			{
				timer_irq_count = 0;
			}
			else if (address == 0xE000)
			{
				timer_irq_enabled = false;
			}
			else if (address == 0xE001)
			{
				timer_irq_enabled = true;
			}
			
			else
			{
				//Console.WriteLine("Mapper: {0}  Address: 0x{1:x}  Data: {2:x}", mapperCartridge.mapper, address, data);
			}
			
			//End Mapper #4
		}				
		else if (mapperCartridge.mapper == 5)
		{
			//Using Mapper #5
			//FIXME: is this actually 0x5101?
			if (address == 0x5100)
			{
				mapper5_prgBankSize = (byte)(data & 0x3);
				//Console.WriteLine("Prg Bank Size: {0}", mapper5_prgBankSize);
				//0 = 32k, 1 = 16k, 2 = 16k/8k, 3 = 8k
			}
			else if (address == 0x5101)
			{
				mapper5_chrBankSize = (byte)(data & 0x3);
				//Console.WriteLine("Chr Bank Size: {0}", mapper5_chrBankSize);
				//0 = 8k, 1 = 4k, 2 = 2k, 3 = 1k
			}
			else if (address == 0x5105)
			{
				//Console.WriteLine("Nametable Switch?!");
				//myEngine.myPPU.backgroundAddress = 0x0000;
			}
			else if (address == 0x5114)
			{
				//FIXME: Add switch to WRAM
				if (mapper5_prgBankSize == 3)
				{
					Switch8kPrgRom((data & 0x7f) * 2, 0);
				}
			}
			else if (address == 0x5115)
			{
				//FIXME: Add switch to WRAM
				if (mapper5_prgBankSize == 1)
				{
					Switch16kPrgRom((data & 0x7e) * 2, 0);
				}
				else if (mapper5_prgBankSize == 2)
				{
					Switch16kPrgRom((data & 0x7e) * 2, 0);
				}
				else if (mapper5_prgBankSize == 3)
				{
					Switch8kPrgRom((data & 0x7f) * 2, 1);
				}
			}
			else if (address == 0x5116)
			{
				//FIXME: Add switch to WRAM
				if (mapper5_prgBankSize == 2)
				{
					Switch8kPrgRom((data & 0x7f) * 2, 2);
				}
				else if (mapper5_prgBankSize == 3)
				{
					Switch8kPrgRom((data & 0x7f) * 2, 2);
				}
			}
			else if (address == 0x5117)
			{
				//FIXME: Add switch to WRAM
				if (mapper5_prgBankSize == 0)
				{
					Switch32kPrgRom((data & 0x7c) * 2);
				}
				else if (mapper5_prgBankSize == 1)
				{
					Switch16kPrgRom((data & 0x7e) * 2, 1);
				}
				else if (mapper5_prgBankSize == 2)
				{
					Switch8kPrgRom((data & 0x7f) * 2, 3);
				}
				else if (mapper5_prgBankSize == 3)
				{
					Switch8kPrgRom((data & 0x7f) * 2, 3);
				}
				
			}
			else if (address == 0x5120)
			{
				if (mapper5_chrBankSize == 3)
				{
					Switch1kChrRom(data, 0);
				}
			}
			else if (address == 0x5121)
			{
				if (mapper5_chrBankSize == 3)
				{
					Switch1kChrRom(data, 1);
				}
				else if (mapper5_chrBankSize == 2)
				{
					//Switch2kChrRom(data * 2, 0);
					Switch2kChrRom(data, 0);
				}
			}
			else if (address == 0x5122)
			{
				if (mapper5_chrBankSize == 3)
				{
					Switch1kChrRom(data, 2);
				}
			}
			else if (address == 0x5123)
			{
				if (mapper5_chrBankSize == 3)
				{
					Switch1kChrRom(data, 3);
				}
				else if (mapper5_chrBankSize == 2)
				{
					//Switch2kChrRom(data * 2, 1);
					Switch2kChrRom(data, 1);
				}
				else if (mapper5_chrBankSize == 1)
				{
					//Switch4kChrRom(data * 4, 0);
					Switch4kChrRom(data, 0);
				}
			}
			else if (address == 0x5124)
			{
				if (mapper5_chrBankSize == 3)
				{
					Switch1kChrRom(data, 4);
				}
			}
			else if (address == 0x5125)
			{
				if (mapper5_chrBankSize == 3)
				{
					Switch1kChrRom(data, 5);
				}
				else if (mapper5_chrBankSize == 2)
				{
					//Switch2kChrRom(data * 2, 2);
					Switch2kChrRom(data, 2);
				}
			}
			else if (address == 0x5126)
			{
				if (mapper5_chrBankSize == 3)
				{
					Switch1kChrRom(data, 6);
				}
			}
			else if (address == 0x5127)
			{
				if (mapper5_chrBankSize == 3)
				{
					Switch1kChrRom(data, 7);
				}
				else if (mapper5_chrBankSize == 2)
				{
					//Switch2kChrRom(data * 2, 3);
					Switch2kChrRom(data, 3);
				}
				else if (mapper5_chrBankSize == 1)
				{
					//Switch4kChrRom(data * 4, 1);
					Switch4kChrRom(data, 1);
				}
				else if (mapper5_chrBankSize == 0)
				{
					//Switch8kChrRom(data * 8);
					Switch8kChrRom(data);
				}
			}
			else if (address == 0x5128)
			{
				//FIXME: Nametables only?
				//Switch2kChrRom(data * 2, 0);
				Switch1kChrRom(data, 0);
				Switch1kChrRom(data, 4);
			}
			else if (address == 0x5129)
			{
				//FIXME: Nametables only?
				//Switch2kChrRom(data * 2, 1);
				if (mapper5_chrBankSize == 3)
				{
					Switch1kChrRom(data, 1);
					Switch1kChrRom(data, 5);
				}
				else if (mapper5_chrBankSize == 2)
				{
					Switch2kChrRom(data, 0);
					Switch2kChrRom(data, 2);
				}
			}
			else if (address == 0x512a)
			{
				//FIXME: Nametables only?
				//Switch2kChrRom(data * 2, 2);
				if (mapper5_chrBankSize == 3)
				{
					Switch1kChrRom(data, 2);
					Switch1kChrRom(data, 6);
				}
			}
			else if (address == 0x512b)
			{
				//FIXME: Nametables only?
				//Switch2kChrRom(data * 2, 3);
				if (mapper5_chrBankSize == 3)
				{
					Switch1kChrRom(data, 3);
					Switch1kChrRom(data, 7);
				}
				else if (mapper5_chrBankSize == 2)
				{
					Switch2kChrRom(data, 1);
					Switch2kChrRom(data, 3);
				}
				else if (mapper5_chrBankSize == 1)
				{
					Switch4kChrRom(data, 0);
					Switch4kChrRom(data, 1);
				}
				else if (mapper5_chrBankSize == 0)
				{
					Switch8kChrRom(data);
				}
			}
			//End Mapper #5
		}				
		else if (mapperCartridge.mapper == 7)
		{
			//Using Mapper #7
					
			if ((address >= 0x8000) && (address <= 0xFFFF))
			{
				Switch32kPrgRom((data & 0xf) * 8);
				if ((data & 0x10) == 0x10)
				{
					mapperCartridge.mirroring = MIRRORING.ONE_SCREEN;
					mapperCartridge.mirroringBase = 0x2400;
				}
				else
				{
					mapperCartridge.mirroring = MIRRORING.ONE_SCREEN;
					mapperCartridge.mirroringBase = 0x2000;
				}
			}
			//End Mapper #7
		}
		else if (mapperCartridge.mapper == 9)
		{
			//Using Mapper #9
			if ((address >= 0xa000) && (address <= 0xafff))
			{
				Switch8kPrgRom(data * 2, 0);
			}
			else if ((address >= 0xB000) && (address <= 0xCFFF))
			{
				Switch4kChrRom(data * 4, 0);
			}
			else if ((address >= 0xD000) && (address <= 0xDFFF))
			{
				latch1data1 = data * 4;
			}
			else if ((address >= 0xE000) && (address <= 0xEFFF))
			{
				latch1data2 = data * 4;
			}
			else if ((address >= 0xF000) && (address <= 0xFFFF))
			{
				if ((data & 1) == 1)
				{
					mapperCartridge.mirroring = MIRRORING.HORIZONTAL;
				}
				else
				{
					mapperCartridge.mirroring = MIRRORING.VERTICAL;
				}
			}
			//End Mapper #9
		}
		else if (mapperCartridge.mapper == 10)
		{
			//Using Mapper #10
			if ((address >= 0xa000) && (address <= 0xafff))
			{
				Switch16kPrgRom(data * 4, 0);
			}
			else if ((address >= 0xB000) && (address <= 0xBFFF))
			{
				
				if (latch1 == 0xfd)
				{
					Switch4kChrRom(data * 4, 0);
				}
			
				latch1data1 = data * 4;
			}
			else if ((address >= 0xC000) && (address <= 0xCFFF))
			{
				
				if (latch1 == 0xfe)
				{
					Switch4kChrRom(data * 4, 0);
				}
				
				latch1data2 = data * 4;
			}
			else if ((address >= 0xD000) && (address <= 0xDFFF))
			{
				
				if (latch2 == 0xfd)
				{
					Switch4kChrRom(data * 4, 1);
				}
				
				latch2data1 = data * 4;
			}
			else if ((address >= 0xE000) && (address <= 0xEFFF))
			{
				
				if (latch2 == 0xfe)
				{
					Switch4kChrRom(data * 4, 1);
				}
				
				latch2data2 = data * 4;
			}
			else if ((address >= 0xF000) && (address <= 0xFFFF))
			{
				if ((data & 1) == 1)
				{
					mapperCartridge.mirroring = MIRRORING.HORIZONTAL;
				}
				else
				{
					mapperCartridge.mirroring = MIRRORING.VERTICAL;
				}
			}
			//End Mapper #10
		}
		else if (mapperCartridge.mapper == 11)
		{
			//Using Mapper #11
					
			if ((address >= 0x8000) && (address <= 0xFFFF))
			{
				int prg_switch;
				int chr_switch;
				
				prg_switch = (data & 0xf);
				chr_switch = (data >> 4);
				
				Switch32kPrgRom(prg_switch * 8);
				Switch8kChrRom(chr_switch * 8);
			}
			//End Mapper #11
		}
		else if (mapperCartridge.mapper == 22)
		{
			//Using Mapper #22
			if (address == 0x8000)
			{
				Switch8kPrgRom(data * 2, 0);
			}
			else if (address == 0x9000)
			{
				switch (data & 0x3)
				{
					case (0): mapperCartridge.mirroring = MIRRORING.VERTICAL;
						break;
					case (1): mapperCartridge.mirroring = MIRRORING.VERTICAL;
						break;
					case (2): mapperCartridge.mirroring = MIRRORING.ONE_SCREEN;
						mapperCartridge.mirroringBase = 0x2400; break;
					case (3): mapperCartridge.mirroring = MIRRORING.ONE_SCREEN;
						mapperCartridge.mirroringBase = 0x2000; break;
				}
			}
			else if (address == 0xA000)
			{
				Switch8kPrgRom(data * 2, 1);
			}
			else if (address == 0xB000)
			{
				Switch1kChrRom((data >> 1), 0);
			}
			else if (address == 0xB001)
			{
				Switch1kChrRom((data >> 1), 1);
			}
			else if (address == 0xC000)
			{
				Switch1kChrRom((data >> 1), 2);
			}
			else if (address == 0xC001)
			{
				Switch1kChrRom((data >> 1), 3);
			}
			else if (address == 0xD000)
			{
				Switch1kChrRom((data >> 1), 4);
			}
			else if (address == 0xD001)
			{
				Switch1kChrRom((data >> 1), 5);
			}
			else if (address == 0xE000)
			{
				Switch1kChrRom((data >> 1), 6);
			}
			else if (address == 0xE001)
			{
				Switch1kChrRom((data >> 1), 7);
			}
			//End Mapper #22
		}
					
		else if (mapperCartridge.mapper == 34)
		{
			//Using Mapper #34
			if (address == 0x7ffd)
			{
				Switch32kPrgRom(data * 8);
			}
			else if (address == 0x7ffe)
			{
				Switch4kChrRom(data * 4, 0);
			}
			else if (address == 0x7fff)
			{
				Switch4kChrRom(data * 4, 1);
			}
			else if (address >= 0x8000)
			{
				Switch32kPrgRom(data * 8);
			}
			//End Mapper #34
		}
		else if (mapperCartridge.mapper == 64)
		{
			//Using Mapper #64
					
			if (address == 0x8000)
			{
				mapper64_commandNumber = data;
				mapper64_prgAddressSelect = (byte)(data & 0x40);
				mapper64_chrAddressSelect = (byte)(data & 0x80);
			}
			else if (address == 0x8001)
			{
				if ((mapper64_commandNumber & 0xf) == 0)
				{
					//Swap 2 1k chr roms
					data = (byte)(data - (data % 2));
					if (mapper64_chrAddressSelect == 0)
					{
						Switch2kChrRom(data, 0);
					}
					else
					{
						Switch2kChrRom(data, 2);
					}
				}
				else if ((mapper64_commandNumber & 0xf) == 1)
				{
					//Swap 2 1k chr roms
					data = (byte)(data - (data % 2));
					if (mapper64_chrAddressSelect == 0)
					{
						Switch2kChrRom(data, 1);
					}
					else
					{
						Switch2kChrRom(data, 3);
					}
				}
				else if ((mapper64_commandNumber & 0xf) == 2)
				{
					//Swap 1k chr rom
					if (mapper64_chrAddressSelect == 0)
					{
						Switch1kChrRom(data, 4);
					}
					else
					{
						Switch1kChrRom(data, 0);
					}
				}
				else if ((mapper64_commandNumber & 0xf) == 3)
				{
					//Swap 1k chr rom
					if (mapper64_chrAddressSelect == 0)
					{
						Switch1kChrRom(data, 5);
					}
					else
					{
						Switch1kChrRom(data, 1);
					}
				}
				else if ((mapper64_commandNumber & 0xf) == 4)
				{
					//Swap 1k chr rom
					if (mapper64_chrAddressSelect == 0)
					{
						Switch1kChrRom(data, 6);
					}
					else
					{
						Switch1kChrRom(data, 2);
					}
				}
				else if ((mapper64_commandNumber & 0xf) == 5)
				{
					//Swap 1k chr rom
					if (mapper64_chrAddressSelect == 0)
					{
						Switch1kChrRom(data, 7);
					}
					else
					{
						Switch1kChrRom(data, 3);
					}
				}
				else if ((mapper64_commandNumber & 0xf) == 6)
				{
					if (mapper64_prgAddressSelect == 0)
					{
						Switch8kPrgRom(data * 2, 0);
					}
					else
					{
						Switch8kPrgRom(data * 2, 1);
					}
				}
				else if ((mapper64_commandNumber & 0xf) == 7)
				{
					if (mapper64_prgAddressSelect == 0)
					{
						Switch8kPrgRom(data * 2, 1);
					}
					else
					{
						Switch8kPrgRom(data * 2, 2);
					}
				}
				else if ((mapper64_commandNumber & 0xf) == 8)
				{
					Switch1kChrRom(data, 1);
				}
				else if ((mapper64_commandNumber & 0xf) == 9)
				{
					Switch1kChrRom(data, 3);
				}
				else if ((mapper64_commandNumber & 0xf) == 0xf)
				{
					if (mapper64_prgAddressSelect == 0)
					{
						Switch8kPrgRom(data * 2, 2);
					}
					else
					{
						Switch8kPrgRom(data * 2, 0);
					}
				}
			}
			else if (address == 0xA000)
			{
				if ((data & 1) == 1)
				{
					mapperCartridge.mirroring = MIRRORING.VERTICAL;
				}
				else
				{
					mapperCartridge.mirroring = MIRRORING.HORIZONTAL;
				}
				
			}
	
			//End Mapper #64
			
		}
		
		else if (mapperCartridge.mapper == 66)
		{
			//Using Mapper #66
					
			if ((address >= 0x8000) && (address <= 0xFFFF))
			{
				int prg_switch;
				int chr_switch;
				
				chr_switch = (data & 0x3);
				prg_switch = (data >> 4) & 0x3;
				
				Switch32kPrgRom(prg_switch * 8);
				Switch8kChrRom(chr_switch * 8);
				
			}
			//End Mapper #66
		}
		else if (mapperCartridge.mapper == 71)
		{
			//Using Mapper #71
			if ((address >= 0xC000)&&(address <= 0xFFFF))
			{
				Switch16kPrgRom(data * 4, 0);
			}
		}
	}
	
	public void TickTimer()
	{
		if (myEngine.myPPU.currentScanline < 240)
		{
			if ((timer_reload_next)&&(timer_irq_enabled))
			{
				//Console.WriteLine("Timer reloaded with: {0} in scanline: {1}", timer_irq_reload, myEngine.myPPU.currentScanline);
				timer_irq_count = timer_irq_reload;
				timer_reload_next = false;
			}
			else
			{
				if (timer_irq_enabled)
				{
					if (timer_irq_count == 0)
					{
						//Count down complete, fire our irq
						//Special case: if we're doing a single pulse because of a zero
						//being written to C000
						if (timer_irq_reload > 0)
						{
							//if (myEngine.my6502.interrupt_flag == 0)
							{
								myEngine.my6502.Push16(myEngine.my6502.pc_register);
								myEngine.my6502.PushStatus();
								myEngine.my6502.pc_register = myEngine.ReadMemory16(0xFFFE);
								myEngine.my6502.interrupt_flag = 1;
							}
							timer_irq_enabled = false;
						}
						else if (timer_zero_pulse)
						{
							myEngine.my6502.Push16(myEngine.my6502.pc_register);
							myEngine.my6502.PushStatus();
							myEngine.my6502.pc_register = myEngine.ReadMemory16(0xFFFE);
							timer_zero_pulse = false;
						}
						//Make sure that we also carry in the timer
						timer_reload_next = true;
						//timer_irq_count = timer_irq_reload;
					}
					else
					{
						if ((myEngine.myPPU.backgroundVisible) || ( myEngine.myPPU.spritesVisible))
							timer_irq_count = timer_irq_count - 1;
					}
				}
			}
		}
	}
	
	public void SetUpMapperDefaults()
	{
		uint i;
		
		Switch32kPrgRom(0);
		Switch8kChrRom(0);
		if (mapperCartridge.prg_rom_pages == 1)
		{
			Switch16kPrgRom(0, 1);
		}
		if (mapperCartridge.mapper == 1)
		{
			mapper1_register8000BitPosition = 0;
			mapper1_register8000Value = 0;
			mapper1_mirroringFlag = 0;
			mapper1_onePageMirroring = 1;
			mapper1_prgSwitchingArea = 1;
			mapper1_prgSwitchingSize = 1;
			mapper1_vromSwitchingSize = 0;
			Switch16kPrgRom((mapperCartridge.prg_rom_pages - 1) * 4, 1);
		}
		else if (mapperCartridge.mapper == 2)
		{
			Switch16kPrgRom((mapperCartridge.prg_rom_pages - 1) * 4, 1);
		}
		else if (mapperCartridge.mapper == 4)
		{
			mapper4_prgAddressSelect = 0;
			mapper4_chrAddressSelect = 0;
			timer_zero_pulse = false;
			
			Switch16kPrgRom((mapperCartridge.prg_rom_pages - 1) * 4, 1);
		}
		else if (mapperCartridge.mapper == 5)
		{
			Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 2, 0);
			Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 2, 1);
			Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 2, 2);
			Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 2, 3);
			mapper5_splitIrqEnabled = false;
		}
		else if (mapperCartridge.mapper == 9)
		{
			latch1 = 0xfe;
			Switch32kPrgRom((mapperCartridge.prg_rom_pages - 1) * 4 - 4);
			Switch8kPrgRom(0, 0);
			Switch8kChrRom(0);
		}
		else if (mapperCartridge.mapper == 10)
		{
			Switch16kPrgRom((mapperCartridge.prg_rom_pages - 1) * 4, 1);
			latch1 = 0xfe;
			latch2 = 0xfe;
		}
		else if (mapperCartridge.mapper == 22)
		{
			Switch16kPrgRom((mapperCartridge.prg_rom_pages - 1) * 4, 1);
			Switch8kChrRom(0);
		}
		else if (mapperCartridge.mapper == 64)
		{
			Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 2, 0);
			Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 2, 1);
			Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 2, 2);
			Switch8kPrgRom((mapperCartridge.prg_rom_pages * 4) - 2, 3);
		}
		else if (mapperCartridge.mapper == 71)
		{
			Switch16kPrgRom((mapperCartridge.prg_rom_pages - 1) * 4, 1);
		}
	}
	
	public Mapper(NesEngine theEngine, ref NesCartridge theCartridge)
	{
		myEngine = theEngine;
		mapperCartridge = theCartridge;
		current_prg_rom_page = new uint[8];
		current_chr_rom_page = new uint[8];
		timer_irq_enabled = false;
	}
}
