using System.IO;

public class Program {
    public static void Main(string[] args) {
        byte[] rom = File.ReadAllBytes("chip8-roms/test_opcode.ch8");
        Sharp8 sharp8 = new Sharp8();
        sharp8.LoadRom(rom);
        Gui gui = new Gui(sharp8);
        gui.Start();
    }
}