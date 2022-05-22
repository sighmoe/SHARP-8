using Util;
public class Opcodes
{
    
    public Dictionary<int,Action<DecodeState>> opcodes = new Dictionary<int, Action<DecodeState>>
    {
        { 0x0, (_) => { Op0(_); } },
        { 0x1, (_) => { Op1(_); } },
        { 0x2, (_) => { Op2(_); } },
        { 0x3, (_) => { Op3(_); } },
        { 0x4, (_) => { Op4(_); } },
        { 0x5, (_) => { Op5(_); } },
        { 0x6, (_) => { Op6(_); } },
        { 0x7, (_) => { Op7(_); } },
        { 0x8, (_) => { Op8(_); } },
        { 0x9, (_) => { Op9(_); } },
        { 0xA, (_) => { OpA(_); } },
        { 0xB, (_) => { OpB(_); } },
        { 0xC, (_) => { OpC(_); } },
        { 0xD, (_) => { OpD(_); } },
        { 0xE, (_) => { OpE(_); } },
        { 0xF, (_) => { OpF(_); } },
    };

    private static void Op0 (DecodeState ds) 
    {
        return;
    }
    private static void Op1 (DecodeState ds) 
    {
        return;
    }
    private static void Op2 (DecodeState ds) 
    {
        return;
    }
    private static void Op3 (DecodeState ds) 
    {
        return;
    }
    private static void Op4 (DecodeState ds) 
    {
        return;
    }
    private static void Op5 (DecodeState ds) 
    {
        return;
    }
    private static void Op6 (DecodeState ds) 
    {
        return;
    }
    private static void Op7 (DecodeState ds) 
    {
        return;
    }
    private static void Op8 (DecodeState ds) 
    {
        return;
    }
    private static void Op9 (DecodeState ds) 
    {
        return;
    }
    private static void OpA (DecodeState ds) 
    {
        return;
    }
    private static void OpB (DecodeState ds) 
    {
        return;
    }
    private static void OpC (DecodeState ds) 
    {
        return;
    }
    private static void OpD (DecodeState ds) 
    {
        return;
    }
    private static void OpE (DecodeState ds) 
    {
        return;
    }
    private static void OpF (DecodeState ds) 
    {
        return;
    }
}