namespace Util
{
    class Constants
    {
        public const uint DISPLAY_HEIGHT = 32;
        public const uint DISPLAY_WIDTH = 64;
    }
    
    public struct DecodeState
    {
        public byte Op { get; init; }
        public byte X { get; init; }
        public byte Y { get; init; }
        public byte N { get; init; }
        public byte NN { get; init; }
        public ushort NNN { get; init; }
    }
    
    public class Sharp8State
    {
        public byte[] Ram { get; set; }
        public byte[] Vram { get; set; }
        public byte[] V { get; set; }
        public ushort Pc { get; set; }
        public ushort I { get; set; }
        public Stack<ushort> CallStack { get; set; }
        public byte DelayTimer { get; set; }
        public ushort SoundTimer { get; set; }
        public long Cycles { get; set; }
        public bool WaitForKey { get; set; }
        public byte KeyPressRegister { get; set; }
        public bool VramChanged { get; set; }
    }
}