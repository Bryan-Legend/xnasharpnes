// created on 10/30/2004 at 09:56

public enum MIRRORING {HORIZONTAL, VERTICAL, FOUR_SCREEN, ONE_SCREEN};
public class NesCartridge
{
	public byte [][] prg_rom;
	public byte [][] chr_rom;
	public MIRRORING mirroring;
	public bool trainer_present;
	public bool save_ram_present;
	public bool is_vram;
	public byte mapper;
	
	public byte prg_rom_pages;
	public byte chr_rom_pages;
	public uint mirroringBase; //For one screen mirroring
}

