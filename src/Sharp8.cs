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
    byte[] display;

    public Sharp8()
    {
        pc = 0x200;
        memory = new byte[4096];
        reg = new byte[16];
        stack = new Stack<ushort>();
        display = new byte[64 * 32];
    }

    private void FdxCycle()
    {
        ushort instr = (ushort)(memory[pc] << 8 | memory[pc + 1]);
        pc += 2;
        byte op = (byte)(instr >> 12);

        Console.WriteLine("Fdx cycle for instr: {0:X2}", instr);

        switch (op)
        {
            // 00E0 - clear screen
            case 0x0:
                display = new byte[64 * 32];
                Redraw();
                break;

            // 1NNN - jump
            case 0x1:
                pc = (ushort)(instr & 0x0FFF);
                break;

            // 6XNN - set register vx 
            // 7XNN - add value to register vx
            case 0x6 or 0x7:
                byte r = (byte)((instr & 0x0F00) >> 8);
                byte val = (byte)(instr & 0x00FF);

                if (op == 0x6)
                {
                    reg[r] = val;
                }
                else
                {
                    reg[r] += val;
                }
                break;

            // ANNN - set index register I
            case 0xA:
                I = (ushort) (instr & 0x0FFF);
                break;

            // DXYN - Draw N pixels tall sprite at V[X],V[Y]
            case 0xD:
                int x = (byte) ((instr & 0x0F00) >> 8);
                int y = (byte) ((instr & 0x00F0) >> 4);
                byte n = (byte) (instr & 0x000F); 
                
                byte row = (byte) (reg[y] & 32);
                byte col = (byte) (reg[x] & 64);

                // clear flag register
                reg[0xF] = 0;

                for (int i = 0; i < n; i++)
                {
                    // Don't draw past bottom edge of screen
                    if (row+i >= 32)
                    {
                        continue;
                    }

                    byte bite = memory[I + i];
                    for (int j = 7; j >= 0; j--)
                    {
                        // Don't draw past right edge of screen
                        if (col+j >= 64)
                        {
                            continue;
                        }

                        byte bit = (byte) ((bite >> j) & 0x01);
                        int pixel = ((row+i)*64) + ((col+j));
                        if (display[pixel] == 1 && bit == 1)
                        {
                            reg[0xF] = 1;
                        }

                        display[pixel] ^= bit;
                    }
                    Redraw();
                }
                break;

            default:
                Console.WriteLine("Unimplemented opcode {0:X2}. Full instruction: {1:X2}", op, instr);
                Halt();
                break;
        }
    }

    private void Redraw()
    {
        Console.WriteLine("CANVAS:");
        for (int r = 0; r < 32; r++)
        {
            for (int c = 0; c < 64; c++)
            {
                Console.Write(display[r*64+c]);
            }
            Console.WriteLine();
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