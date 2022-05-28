using Util;

public class Gui 
{

    private const uint WINDOW_HEIGHT = 600;
    private const uint WINDOW_WIDTH = 800;
    
        
    private float SCALING_FACTOR_X = WINDOW_WIDTH / Constants.DISPLAY_WIDTH;
    private float SCALING_FACTOR_Y = WINDOW_HEIGHT / Constants.DISPLAY_HEIGHT;

    private SFML.Graphics.Image buffer;
    private SFML.Graphics.Texture viewPort;
    private Sharp8 sharp8;

    public Gui(Sharp8 sharp8)
    {
        this.buffer = new SFML.Graphics.Image(WINDOW_HEIGHT, WINDOW_WIDTH);
        this.viewPort = new SFML.Graphics.Texture(WINDOW_HEIGHT, WINDOW_WIDTH);
        this.sharp8 = sharp8;
    }
    public void Start()
    {
        var mode = new SFML.Window.VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT);
        var window = new SFML.Graphics.RenderWindow(mode, "SHARP-8");
        window.KeyPressed += Window_KeyPressed;
        window.KeyReleased += Window_KeyReleased;

        var s8s = this.sharp8.GetSharp8State();
        // Start the game loop
        while (window.IsOpen)
        {
            window.DispatchEvents();
            this.sharp8.Cycle();
            if (s8s.VramChanged)
            {
                s8s.VramChanged = false;
                UpdatePixelBuffer(s8s.Vram);
                var sprite = new SFML.Graphics.Sprite(this.viewPort);
                sprite.Scale = new SFML.System.Vector2f(SCALING_FACTOR_X,SCALING_FACTOR_Y);
                window.Draw(sprite);
                
                // Finally, display the rendered frame on screen
                window.Display();
            }
        }
    }

    private void ProcessInput()
    {
        var s8s = this.sharp8.GetSharp8State();
        for (var i = 0; i < 0x10; i++)
        {
            if (s8s.K[i] != 0xFF)
            {
                s8s.V[s8s.KeyPressRegister] = s8s.K[s8s.KeyPressRegister];
                s8s.K[s8s.KeyPressRegister] = 0xFF;
                s8s.WaitForKey = false;
            }
        }
    }

    private void UpdatePixelBuffer(byte[] vram)
    {
        for (uint r = 0; r < Constants.DISPLAY_HEIGHT; r++)
        {
            for (uint c = 0; c < Constants.DISPLAY_WIDTH; c++)
            {
                var idx = (r * Constants.DISPLAY_WIDTH)+c;
                SFML.Graphics.Color color = vram[idx] == 1 ? SFML.Graphics.Color.White: SFML.Graphics.Color.Black;
                this.buffer.SetPixel(c,r,color);
            }
        }
        this.viewPort.Update(buffer);
    }

    private void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
    {
        var s8s = this.sharp8.GetSharp8State();
        var K = s8s.K;
        var keyPressRegister = s8s.KeyPressRegister;
        var window = (SFML.Window.Window)sender;
        if (e.Code == SFML.Window.Keyboard.Key.Escape)
        {
            window.Close();
        }
        else if (e.Code == SFML.Window.Keyboard.Key.Num1)
        {
            K[keyPressRegister] = 0x1;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.Num2)
        {
            K[keyPressRegister] = 0x2;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.Num3)
        {
            K[keyPressRegister] = 0x3;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.Num4)
        {
            K[keyPressRegister] = 0xC;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.Q)
        {
            K[keyPressRegister] = 0x4;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.W)
        {
            K[keyPressRegister] = 0x5;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.E)
        {
            K[keyPressRegister] = 0x6;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.R)
        {
            K[keyPressRegister] = 0xD;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.A)
        {
            K[keyPressRegister] = 0x7;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.S)
        {
            K[keyPressRegister] = 0x8;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.D)
        {
            K[keyPressRegister] = 0x9;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.F)
        {
            K[keyPressRegister] = 0xE;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.Z)
        {
            K[keyPressRegister] = 0xA;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.X)
        {
            K[keyPressRegister] = 0x0;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.C)
        {
            K[keyPressRegister] = 0xB;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.V)
        {
            K[keyPressRegister] = 0xF;
        }
    }
    
    private void Window_KeyReleased(object sender, SFML.Window.KeyEventArgs e)
    {
        var s8s = this.sharp8.GetSharp8State();
        var K = s8s.K;
        var keyPressRegister = s8s.KeyPressRegister;
        var window = (SFML.Window.Window)sender;
        if (e.Code == SFML.Window.Keyboard.Key.Num1)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.Num2)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.Num3)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.Num4)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.Q)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.W)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.E)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.R)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.A)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.S)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.D)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.F)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.Z)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.X)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.C)
        {
            K[keyPressRegister] = 0xFF;
        }
        else if (e.Code == SFML.Window.Keyboard.Key.V)
        {
            K[keyPressRegister] = 0xFF;
        }
    }
}