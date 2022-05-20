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
    byte[] ram, reg;
    ushort pc, I;
    Stack<ushort> stack;
    byte[] vram;

    public Sharp8()
    {
        pc = 0x200;
        ram = new byte[4096];
        reg = new byte[16];
        stack = new Stack<ushort>();
        vram = new byte[Gui.DISPLAY_HEIGHT*Gui.DISPLAY_WIDTH];
    }

    public bool FdxCycle()
    {
        var vramChanged = false;
        ushort instr = (ushort)(ram[pc] << 8 | ram[pc + 1]);
        pc += 2;
        byte op = (byte)(instr >> 12);

        Console.WriteLine("Fdx cycle for instr: {0:X2}", instr);

        switch (op)
        {
            // 00E0 - clear screen
            // 00EE - return from subroutine
            case 0x0:
                if ((instr & 0x00FF) == 0x00E0)
                {
                    vram = new byte[64 * 32];
                    vramChanged = true;
                }
                else
                {
                    pc = stack.Pop();
                }
                break;

            // 1NNN - jump
            case 0x1:
                pc = (ushort)(instr & 0x0FFF);
                break;

            // 2NNN - call subroutine at memory location NNN
            case 0x2:
            {
                stack.Push(pc);
                pc = (ushort)(instr & 0x0FFF);
            }
            break;

            // 3XNN - skip if V[X] == NN
            // 4XNN - skip if V[X] != NN
            case 0x3 or 0x4:
            {
                byte x = (byte) ((instr & 0x0F00) >> 8);
                byte nn = (byte)(instr & 0x00FF);

                if ((op == 0x3 && reg[x] == nn) || (op == 0x4 && reg[x] != nn))
                {
                    pc += 2;
                }
                break;
            }

            // 5XY0 - skip if V[X] == V[Y]
            case 0x5:
            {
                uint x = (uint) ((instr & 0x0F00) >> 8);
                uint y = (uint) ((instr & 0x00F0) >> 4);

                if (reg[x] == reg[y])
                {
                    pc += 2;
                }
                break;
            }


            // 6XNN - set register vx 
            // 7XNN - add value to register vx
            case 0x6 or 0x7:
                byte loc = (byte)((instr & 0x0F00) >> 8);
                byte val = (byte)(instr & 0x00FF);

                if (op == 0x6)
                {
                    reg[loc] = val;
                }
                else
                {
                    reg[loc] += val;
                }
                break;
        
            case 0x8:
            {
                uint x = (uint) ((instr & 0x0F00) >> 8);
                uint y = (uint) ((instr & 0x00F0) >> 4);

                uint lsn = (uint) (instr & 0x000F);

                switch (lsn)
                {
                    // 8XY0 - V[Y] = V[X]
                    case 0x0:
                        reg[y] = reg[x];
                        break;

                    // 8XY1 - V[X] = V[X] OR V[Y]
                    case 0x1:
                        reg[x] = (byte) (reg[x] | reg[y]);
                        break;

                    // 8XY2 - V[X] = V[X] AND V[Y]
                    case 0x2:
                        reg[x] = (byte) (reg[x] & reg[y]);
                        break;

                    // 8XY3 - V[X] = V[X] XOR V[Y]
                    case 0x3:
                        reg[x] = (byte) (reg[x] ^ reg[y]);
                        break;

                    // 8XY4 - V[X] = V[X] + V[Y]
                    case 0x4:
                        if (reg[x] + reg[y] > 255) 
                        {
                            reg[0xF] = 1;
                        }
                        else
                        {
                            reg[0xF] = 0;
                        }
                        reg[x] = (byte) (reg[x] + reg[y]);
                        break;

                    // 8XY5 - V[X] = V[X] - V[Y]
                    case 0x5:
                        if (reg[x] > reg[y]) 
                        {
                            reg[0xF] = 1;
                        }
                        else
                        {
                            reg[0xF] = 0;
                        }
                        reg[x] = (byte) (reg[x] - reg[y]);
                        break;

                    // 8XY6 - V[X] = V[X] >> 1
                    case 0x6:
                        if ((reg[x] & 0x1) == 1)
                        {
                            reg[0xF] = 1;
                        }
                        else
                        {
                            reg[0xF] = 0;
                        }
                        reg[x] = (byte) (reg[x] >> 1);
                        break;

                    // 8XY7 - V[X] = V[Y] - V[X]
                    case 0x7:
                        if (reg[y] > reg[x]) 
                        {
                            reg[0xF] = 1;
                        }
                        else
                        {
                            reg[0xF] = 0;
                        }
                        reg[x] = (byte) (reg[y] - reg[x]);
                        break;

                    // 8XYE - V[X] = V[X] << 1
                    case 0xE:
                        if ((reg[x] & 0x80) > 0)
                        {
                            reg[0xF] = 1;
                        }
                        else
                        {
                            reg[0xF] = 0;
                        }
                    
                        reg[x] = (byte) (reg[x] << 1);
                        break;
                }
                break;
            }
            
            // 9XY0 - skip if V[X] == V[Y]
            case 0x9:
            {
                uint x = (uint) ((instr & 0x0F00) >> 8);
                uint y = (uint) ((instr & 0x00F0) >> 4);

                if (reg[x] != reg[y])
                {
                    pc += 2;
                }
                break;
            }

            // ANNN - set index register I
            case 0xA:
                I = (ushort) (instr & 0x0FFF);
                break;

            // DXYN - Draw N pixels tall sprite at V[X],V[Y]
            case 0xD:
            {
                uint x = (uint) ((instr & 0x0F00) >> 8);
                uint y = (uint) ((instr & 0x00F0) >> 4);
                uint n = (uint) (instr & 0x000F); 
                
                uint row = (uint) (reg[y] & 31);
                uint col = (uint) (reg[x] & 63);

                // clear flag register
                reg[0xF] = 0;

                for (int r = 0; r < n; r++)
                {
                    // Don't draw past bottom edge of screen
                    if (row+r >= 32)
                    {
                        continue;
                    }

                    byte bite = ram[I + r];
                    for (int c = 7; c >= 0; c--)
                    {
                        // Don't draw past right edge of screen
                        if (col+c >= 64)
                        {
                            continue;
                        }

                        byte bit = (byte) ((bite >> c) & 0x01);
                        uint pixel = (uint) (((row+r)*Gui.DISPLAY_WIDTH) + (col+(7-c)));
                        if (vram[pixel] == 1 && bit == 1)
                        {
                            reg[0xF] = 1;
                        }

                        vram[pixel] ^= bit;
                    }
                    vramChanged = true;
                }
                break;
            }

            case 0xF:
            {
                if ((instr & 0x00FF) == 0x001E)
                {
                    uint x = (uint) ((instr & 0x0F00) >> 8);
                    I += reg[x];
                }
                break;
            }

            default:
                Console.WriteLine("Unimplemented opcode {0:X2}. Full instruction: {1:X2}", op, instr);
                Halt();
                break;

        }
        return vramChanged;
    }

    public byte[] GetVram()
    {
        return vram;
    }

    private void LoadFont()
    {
        for (int i = 0; i < font.Length; i++)
        {
            ram[0x050 + i] = font[i];
        }
    }

    public void LoadRom(byte[] rom)
    {
        for (int i = 0; i < rom.Length; i++)
        {
            ram[0x200 + i] = rom[i];
        }
    }

    public void Halt()
    {
        Console.WriteLine("HALTING...");
        for (; ; ) { }
    }
}