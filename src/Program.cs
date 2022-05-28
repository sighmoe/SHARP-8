using System.IO;

public class Program {
    public static void Main(string[] args) {
        byte[] rom = File.ReadAllBytes("/home/simo/development/chip8-roms/ZERO");
        Sharp8 sharp8 = new Sharp8();
        sharp8.LoadRom(rom);
        Gui gui = new Gui(sharp8);
        gui.Start();
    }
}