using System.IO;

public class Program {
    public static void Main(string[] args) {
        byte[] rom = File.ReadAllBytes("/home/simo/development/SHARP-8/rom/chip8-roms/programs/ibm_logo.ch8");
        Sharp8 sharp8 = new Sharp8();
        sharp8.LoadRom(rom);
        sharp8.Start();
    }
}