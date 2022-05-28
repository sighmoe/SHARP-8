using Util;

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

    private Sharp8State s8s;
    public Sharp8()
    {
        s8s = new Sharp8State
        {
            Ram = new byte[4096],
            Vram = new byte[Constants.DISPLAY_HEIGHT * Constants.DISPLAY_WIDTH],
            V = new byte[16],
            K = new byte[16],
            Pc = 0x200,
            I = 0x0000,
            CallStack = new Stack<ushort>(),
            DelayTimer = 0,
            SoundTimer = 0,
            Cycles = 0,
            WaitForKey = false,
            VramChanged = false,
        };

        for (var i = 0; i < 0x10; i++)
        {
            s8s.K[i] = 0xFF;
        } 
        LoadFont();
    }

    public ushort Fetch()
    {
        var ram = s8s.Ram;
        var pc = s8s.Pc;
        var instr = ram[pc] << 8 | ram[pc + 1];
        s8s.Pc += 2;
        return (ushort) instr;
    }

    public DecodeState Decode(ushort instr)
    {
        return new DecodeState
        {
            Op = (byte)((instr & 0xF000) >> 12),
            X = (byte)((instr & 0x0F00) >> 8),
            Y = (byte)((instr & 0x00F0) >> 4),
            N = (byte)(instr & 0x000F),
            NN = (byte)(instr & 0x00FF),
            NNN = (ushort)(instr & 0x0FFF),
        };
    }

    public void Execute(DecodeState ds)
    {
        Opcodes.OpTable[ds.Op](ds,s8s); 
    }

    public void Cycle()
    {
        // Console.WriteLine("SHARP-8 CPU State:\n {0}", s8s);
        var instr = Fetch();
        var ds = Decode(instr);
        Console.WriteLine("PC: {0:X2}\ninstr: {1:X2}\n", s8s.Pc-2, instr);
        Execute(ds);
        if(s8s.DelayTimer > 0)
        {
            s8s.DelayTimer--;
        }
        
        if(s8s.SoundTimer > 0)
        {
            s8s.SoundTimer--;
            if(s8s.SoundTimer == 1)
            {
                Console.WriteLine("BEEP!\n");
            }  
        }
        s8s.Cycles += 1;
    }
        
    public Sharp8State GetSharp8State()
    {
        return s8s;
    }

    private void LoadFont()
    {
        for (var i = 0; i < font.Length; i++)
        {
            s8s.Ram[i] = font[i];
        }
    }

    public void LoadRom(byte[] rom)
    {
        for (var i = 0; i < rom.Length; i++)
        {
            s8s.Ram[0x200 + i] = rom[i];
        }
    }
}