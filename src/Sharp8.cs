using System.Collections;

public class Sharp8
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

    public Sharp8()
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
            // 00E0 - clear screen
            case 0x0:
                display = new bool[64 * 32];
                Redraw();
                break;

            // 1NNN - jump
            case 0x1:
                pc = (ushort)(instr & 0x0FFF);
                break;

            // 6XNN - set register vx 
            // 7XNN - add value to register vx
            case 0x6 or 0x7:
                byte x = (byte)((instr & 0x0F00) >> 8);
                byte val = (byte)(instr & 0x00FF);

                if (op == 0x6)
                {
                    reg[x] = val;
                }
                else
                {
                    reg[x] += val;
                }
                break;

            // ANNN - set index register I
            case 0xA:
                I = (ushort) (instr & 0x0FFF);
                break;

            // DXYN - Draw N pixels tall sprite at V[X],V[Y]
            case 0xD:
                break;

            default:
                Console.WriteLine("Unimplemented opcode {0:X2}. Full instruction: {1:X2}", op, instr);
                Halt();
                break;
        }
    }

    private void Redraw()
    {

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