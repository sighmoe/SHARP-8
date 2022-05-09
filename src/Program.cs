using System.IO;

public class Program {
    public static void Main(string[] args) {
        byte[] rom = File.ReadAllBytes("/home/simo/development/SHARP-8/rom/chip8-roms/programs/ibm_logo.ch8");
        Cpu cpu = new Cpu();
        cpu.LoadRom(rom);
        cpu.Start();
    }
}