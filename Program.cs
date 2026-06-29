using SFML.Graphics;
using SFML.System;
using SFML.Window;

const int ScreenWidth = 1600;
const int ScreenHeight = 900;
const int MaxVelocity = 2;

// Window and view
RenderWindow window = new RenderWindow(
    new VideoMode(new Vector2u(ScreenWidth, ScreenHeight)),
    "SFML.NET"
);
window.Closed += (Sender, e) => window.Close();
View view = new(new FloatRect(new Vector2f(0, 0), new Vector2f(ScreenWidth, ScreenHeight)));
window.SetView(view);
window.SetFramerateLimit(60);
RectangleShape viewBackground = new RectangleShape(new Vector2f(ScreenWidth, ScreenHeight))
{
    FillColor = Color.White,
    Position = new Vector2f(0, 0)
};

// Events
window.Resized += (_, e) => { window.SetView(GetLetterboxView(view, e.Size.X, e.Size.Y)); };

// Objects

List<Boid> BoidList = new();
Random random = new Random();

for (int i = 0; i < 100; i++)
{
    BoidList.Add(new Boid(
        new Vector2f(random.Next(0, ScreenWidth), random.Next(0, ScreenHeight)),
        new Vector2f(random.Next(-MaxVelocity, MaxVelocity), random.Next(-MaxVelocity, MaxVelocity))
    ));
}

// Main loop
while (window.IsOpen)
{

    window.DispatchEvents();
    window.Clear(Color.Black);

    // Update

    // Draw
    window.Draw(viewBackground);
    foreach (Boid bird in BoidList)
    {
        bird.Update(BoidList, new Vector2u(ScreenWidth, ScreenHeight));
        bird.Draw(window);
    }

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
