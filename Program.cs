using SFML.Graphics;
using SFML.System;
using SFML.Window;

const int ScreenWidth = 800;
const int ScreenHeight = 600;

// Window
RenderWindow window = new RenderWindow(
    new VideoMode(new Vector2u(ScreenWidth, ScreenHeight)),
    "SFML.NET"
);

window.Closed += (Sender, e) => window.Close();
View view = new(new FloatRect(new Vector2f(0, 0), new Vector2f(ScreenWidth, ScreenHeight)));
window.SetView(view);
window.SetFramerateLimit(60);
RectangleShape background = new RectangleShape(new Vector2f(ScreenWidth, ScreenHeight))
{
    FillColor = Color.White,
    Position = new Vector2f(0, 0)
};

// Events
window.Resized += (_, e) => { window.SetView(GetLetterboxView(view, e.Size.X, e.Size.Y)); };

// Objects
Bird bird = new(new Vector2f(400, 400));

// Main loop
while (window.IsOpen)
{

    window.DispatchEvents();
    window.Clear(Color.Black);

    // Draw
    window.Draw(background);
    bird.UpdateAndDraw(window);

    window.Display();
}

View GetLetterboxView(View view, uint windowWidth, uint windowHeight)
{
    float windowRatio = (float)windowWidth / windowHeight;
    float viewRatio = view.Size.X / view.Size.Y;

    float sizeX = 1f;
    float sizeY = 1f;
    float posX = 0f;
    float posY = 0f;

    bool horizontalSpacing = windowRatio >= viewRatio;

    if (horizontalSpacing)
    {
        sizeX = viewRatio / windowRatio;
        posX = (1f - sizeX) / 2f;
    }
    else
    {
        sizeY = windowRatio / viewRatio;
        posY = (1f - sizeY) / 2f;
    }

    view.Viewport = new FloatRect(new Vector2f(posX, posY), new Vector2f(sizeX, sizeY));
    return view;
}