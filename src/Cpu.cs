using System.Collections;

public class Cpu
{
    byte[] font = 
    {
        0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
        0x20, 0x60, 0x20, 0x20, 0x70, // 1
        0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
        0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
        0x90, 0x90, 0xF0, 0x10, 0x10, // 4
        0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
        0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
        0xF0, 0x10, 0x20, 0x40, 0x40, // 7
        0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
        0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
        0xF0, 0x90, 0xF0, 0x90, 0x90, // A
        0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
        0xF0, 0x80, 0x80, 0x80, 0xF0, // C
        0xE0, 0x90, 0x90, 0x90, 0xE0, // D
        0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
        0xF0, 0x80, 0xF0, 0x80, 0x80  // F
    };


    byte delayTimer, soundTimer;
    byte[] memory, reg;
    ushort pc, I;
    Stack<ushort> stack;
    bool[] display;

    public Cpu()
    {
        pc = 0x200;
        memory = new byte[4096];
        reg = new byte[16];
        stack = new Stack<ushort>();
        display = new bool[64 * 32];
    }

    private void FdxCycle()
    {
        ushort instr = (ushort)(memory[pc] << 8 | memory[pc + 1]);
        pc += 2;

        byte op = (byte)(instr >> 12);

        switch (op)
        {
            default:
                Console.WriteLine("Unimplemented opcode {0:X2}. Full instruction: {1:X2}", op, instr);
                Halt();
                break;
        }
    }

    private void LoadFont()
    {
        for (int i = 0; i < font.Length; i++)
        {
            memory[0x050 + i] = font[i];
        }
    }

    public void LoadRom(byte[] rom)
    {
        for (int i = 0; i < rom.Length; i++)
        {
            memory[0x200 + i] = rom[i];
        }
    }

    public void Start()
    {
        for (; ; )
        {
            FdxCycle();
        }
    }

    public void Halt()
    {
        Console.WriteLine("HALTING...");
        for (; ; ) { }
    }
}