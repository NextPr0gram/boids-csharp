using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Bird
{
    private ConvexShape BodyShape;
    private Vector2f Position { get; set; }

    public Bird(Vector2f position)
    {
        BodyShape = new ConvexShape(3);
        BodyShape.SetPoint(0, new Vector2f(0, 0));
        BodyShape.SetPoint(1, new Vector2f(100, 0));
        BodyShape.SetPoint(2, new Vector2f(50, 150));
        BodyShape.FillColor = Color.Red;
        BodyShape.Origin = new Vector2f(50, 75);
        BodyShape.Position = position;
    }

    public void UpdateAndDraw(RenderWindow window)
    {
        BodyShape.Rotation += 1;
        window.Draw(BodyShape);
    }
}