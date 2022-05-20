public class Gui 
{

    public static uint WINDOW_HEIGHT = 600;
    public static uint WINDOW_WIDTH = 800;
    
    public static uint DISPLAY_HEIGHT = 32;
    public static uint DISPLAY_WIDTH = 64;
    
    private float SCALING_FACTOR_X = WINDOW_WIDTH / DISPLAY_WIDTH;
    private float SCALING_FACTOR_Y = WINDOW_HEIGHT / DISPLAY_HEIGHT;

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
        var window = new SFML.Graphics.RenderWindow(mode, "SFML works!");
        window.KeyPressed += Window_KeyPressed;

        // Start the game loop
        while (window.IsOpen)
        {
            // Process events
            window.DispatchEvents();
            var vramChanged = this.sharp8.FdxCycle();
            if (vramChanged)
            {
                UpdatePixelBuffer(sharp8.GetVram());
                var sprite = new SFML.Graphics.Sprite(this.viewPort);
                sprite.Scale = new SFML.System.Vector2f(SCALING_FACTOR_X,SCALING_FACTOR_Y);
                window.Draw(sprite);
                
                // Finally, display the rendered frame on screen
                window.Display();
            }
        }
    }

    private void UpdatePixelBuffer(byte[] vram)
    {
        for (uint r = 0; r < DISPLAY_HEIGHT; r++)
        {
            for (uint c = 0; c < DISPLAY_WIDTH; c++)
            {
                var idx = (r*DISPLAY_WIDTH)+c;
                SFML.Graphics.Color color = vram[idx] == 1 ? SFML.Graphics.Color.White: SFML.Graphics.Color.Black;
                this.buffer.SetPixel(c,r,color);
            }
        }
        this.viewPort.Update(buffer);
    }

    private void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
    {
        var window = (SFML.Window.Window)sender;
        if (e.Code == SFML.Window.Keyboard.Key.Escape)
        {
            window.Close();
        }
        else if (e.Code == SFML.Window.Keyboard.Key.A)
        {
            for (uint r = 0; r < WINDOW_HEIGHT; r++)
            {
                for (uint c = 0; c < WINDOW_WIDTH; c++)
                {
                    this.buffer.SetPixel(c,r,SFML.Graphics.Color.Yellow);
                }
            }
            this.viewPort.Update(buffer);
        }
        else if (e.Code == SFML.Window.Keyboard.Key.B)
        {
            this.buffer.SetPixel(1,1,SFML.Graphics.Color.Blue);
            this.viewPort.Update(buffer);
        }
    }
}