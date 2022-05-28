using Util;
public class Opcodes
{
    private static Random rng = new Random();

    public static Dictionary<int, Action<DecodeState, Sharp8State>> OpTable = new Dictionary<int, Action<DecodeState, Sharp8State>>
    {
        { 0x0, (_1,_2) => { Op0(_1,_2); } },
        { 0x1, (_1,_2) => { Op1(_1,_2); } },
        { 0x2, (_1,_2) => { Op2(_1,_2); } },
        { 0x3, (_1,_2) => { Op3(_1,_2); } },
        { 0x4, (_1,_2) => { Op4(_1,_2); } },
        { 0x5, (_1,_2) => { Op5(_1,_2); } },
        { 0x6, (_1,_2) => { Op6(_1,_2); } },
        { 0x7, (_1,_2) => { Op7(_1,_2); } },
        { 0x8, (_1,_2) => { Op8(_1,_2); } },
        { 0x9, (_1,_2) => { Op9(_1,_2); } },
        { 0xA, (_1,_2) => { OpA(_1,_2); } },
        { 0xB, (_1,_2) => { OpB(_1,_2); } },
        { 0xC, (_1,_2) => { OpC(_1,_2); } },
        { 0xD, (_1,_2) => { OpD(_1,_2); } },
        { 0xE, (_1,_2) => { OpE(_1,_2); } },
        { 0xF, (_1,_2) => { OpF(_1,_2); } },
    };

    // 00E0 - clear screen
    // 00EE - return from subroutine
    private static void Op0(DecodeState ds, Sharp8State s8s)
    {
        if (ds.NN == 0xE0)
        {
            s8s.Vram = new byte[Constants.DISPLAY_WIDTH * Constants.DISPLAY_HEIGHT];
            s8s.VramChanged = true;
        }
        else if (ds.NN == 0xEE)
        {
            s8s.Pc = s8s.CallStack.Pop();
        }
        else
        {
            s8s.Pc = ds.NNN;
        }
    }

    // 1NNN - jump to address NNN
    private static void Op1(DecodeState ds, Sharp8State s8s)
    {
        s8s.Pc = ds.NNN;
    }

    // 2NNN - call subroutine at memory location NNN
    private static void Op2(DecodeState ds, Sharp8State s8s)
    {
        s8s.CallStack.Push(s8s.Pc);
        s8s.Pc = ds.NNN;
    }

    // 3XNN - skip if V[X] == NN
    private static void Op3(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var X = ds.X;
        var NN = ds.NN;

        if (V[X] == NN)
        {
            s8s.Pc += 2;
        }
    }

    // 4XNN - skip if V[X] != NN
    private static void Op4(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var X = ds.X;
        var NN = ds.NN;
        if (V[X] != NN)
        {
            s8s.Pc += 2;
        }
    }

    // 5XY0 - skip if V[X] == V[Y]
    private static void Op5(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var X = ds.X;
        var Y = ds.Y;
        if (V[X] == V[Y])
        {
            s8s.Pc += 2;
        }
    }

    // 6XNN - set register vx 
    private static void Op6(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var X = ds.X;
        var NN = ds.NN;
        s8s.V[X] = NN;
    }

    // 7XNN - add value to register vx
    private static void Op7(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var X = ds.X;
        var NN = ds.NN;
        s8s.V[X] += NN;
    }
    private static void Op8(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var X = ds.X;
        var Y = ds.Y;
        var N = ds.N;
        UpdateFlags(ds,s8s);
        s8s.V[X] = N switch
        {
            // 8XY0 - V[X] = V[Y]
            0x0 => V[Y],

            // 8XY1 - V[X] = V[X] OR V[Y]
            0x1 => (byte)(V[X] | V[Y]),

            // 8XY2 - V[X] = V[X] AND V[Y]
            0x2 => (byte)(V[X] & V[Y]),

            // 8XY3 - V[X] = V[X] XOR V[Y]
            0x3 => (byte)(V[X] ^ V[Y]),

            // 8XY4 - V[X] = V[X] + V[Y]
            0x4 => (byte)(V[X] + V[Y]),

            // 8XY5 - V[X] = V[X] - V[Y]
            0x5 => (byte)(V[X] - V[Y]),

            // 8XY6 - V[X] = V[X] >> 1
            0x6 => (byte)(V[X] >> 1),

            // 8XY7 - V[X] = V[Y] - V[X]
            0x7 => (byte)(V[Y] - V[X]),

            // 8XYE - V[X] = V[X] << 1 
            0xE => (byte)(V[X] << 1),

            _ => Unimplemented(ds, s8s),
        };
    }

    // 9XY0 - skip if V[X] != V[Y]
    private static void Op9(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var X = ds.X;
        var Y = ds.Y;

        if (V[X] != V[Y])
        {
            s8s.Pc += 2;
        }
        return;
    }

    // ANNN - set index register I
    private static void OpA(DecodeState ds, Sharp8State s8s)
    {
        s8s.I = ds.NNN;
    }
    
    // BNNN - PC = V[0] + NNN
    private static void OpB(DecodeState ds, Sharp8State s8s)
    {
        s8s.Pc = (ushort) (s8s.V[0] + ds.NNN);
    }

    // CXNN - V[X] = rand AND NN
    private static void OpC(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var X = ds.X;
        var NN = ds.NN;
        s8s.V[X] = (byte)(rng.NextInt64(256) & NN);
    }

    // DXYN - Draw N pixels tall sprite at V[X],V[Y]
    private static void OpD(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var X = ds.X;
        var Y = ds.Y;
        var N = ds.N;
        var ram = s8s.Ram;
        var vram = s8s.Vram;
        var I = s8s.I;

        var row = V[Y] & 31;
        var col = V[X] & 63;

        // clear flag register
        s8s.V[0xF] = 0;

        for (var r = 0; r < N; r++)
        {
            byte pixel = ram[I + r];
            for (int c = 0; c < 8; c++)
            {
                var cc = (col + c) % Constants.DISPLAY_WIDTH;
                var rr = (row + r) % Constants.DISPLAY_HEIGHT;
                var loc = (rr * Constants.DISPLAY_WIDTH) + cc;
                if ((pixel & (0x80 >> c)) != 0)
                {
                    if (vram[loc] == 1)
                    {
                        s8s.V[0xF] = 1;
                    }
                    s8s.Vram[loc] ^= 1;
                }
            }
        }
        s8s.VramChanged = true;
    }
    private static void OpE(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var K = s8s.K;
        var NN = ds.NN;
        var X = ds.X;
        s8s.Pc += NN switch
        {
            0xA1 => (ushort)(K[V[X]] == 0xFF ? 2 : 0),
            0x9E => (ushort)(K[V[X]] != 0xFF ? 2 : 0),
            _ => 0,
        };
    }
    private static void OpF(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var K = s8s.K;
        var ram = s8s.Ram;
        var I = s8s.I;
        var NN = ds.NN;
        var X = ds.X;
        switch (NN)
        {
            case 0x07:
                {
                    s8s.V[X] = s8s.DelayTimer;
                    break;
                }
            case 0x0A:
                {
                    s8s.Pc -= 2;
                    s8s.KeyPressRegister = X;
                    s8s.WaitForKey = true;
                    break;
                }
            case 0x15:
                {
                    s8s.DelayTimer = V[X];
                    break;
                }
            case 0x18:
                {
                    s8s.SoundTimer = V[X];
                    break;
                }
            case 0x1E:
                {
                    s8s.I += V[X];
                    break;
                }
            case 0x29:
                {
                    var digit = V[X];
                    s8s.I = (ushort) (5*digit);
                    break;
                }
            case 0x33:
                {
                    s8s.Ram[I] = (byte)(V[X] / 100);
                    s8s.Ram[I + 1] = (byte)((V[X] % 100) / 10);
                    s8s.Ram[I + 2] = (byte)(V[X] % 10);
                    break;
                }
            case 0x55:
                {
                    for (var i = 0; i <= X; i++)
                    {
                        s8s.Ram[I + i] = V[i];
                    }
                    break;
                }
            case 0x65:
                {
                    for (var i = 0; i <= X; i++)
                    {
                        s8s.V[i] = ram[I + i];
                    }
                    break;
                }
            default:
                {
                    Unimplemented(ds, s8s);
                    break;
                }
        }
    }

    public static byte Unimplemented(DecodeState ds, Sharp8State s8s)
    {
        Console.WriteLine("Unimplemented opcode {0:X2}. NNN: {1:X2}", ds.Op, ds.NNN);
        Console.WriteLine("HALTING...");
        for (; ; ) { }
        return 0x00;
    }

    private static void UpdateFlags(DecodeState ds, Sharp8State s8s)
    {
        var V = s8s.V;
        var X = ds.X;
        var Y = ds.Y;
        s8s.V[0xF] = ds.Op switch
        {
            0x8 => ds.N switch
            {
                0x4 => (byte)(V[X] + V[Y] > 255 ? 0x1 : 0x0),
                0x5 => (byte)(V[X] >= V[Y] ? 0x1 : 0x0),
                0x6 => (byte)((V[X] & 0x1) == 1 ? 0x1 : 0x0),
                0x7 => (byte)(V[Y] >= V[X] ? 0x1 : 0x0),
                0xE => (byte)((V[X] & 0x80) > 0 ? 0x1 : 0x0),
                _ => V[0xF]
            },
            _ => Unimplemented(ds, s8s),
        };
    }
}