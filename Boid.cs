using SFML.Graphics;
using SFML.System;

public class Boid
{
    private ConvexShape BodyShape;
    private Vector2f Velocity;
    private Vector2f Acceleration;
    private float DistanceFromOther;

    public Boid(Vector2f position, Vector2f velocity)
    {
        BodyShape = new ConvexShape(3);
        BodyShape.SetPoint(0, new Vector2f(10, 0));
        BodyShape.SetPoint(1, new Vector2f(0, 30));
        BodyShape.SetPoint(2, new Vector2f(20, 30));
        BodyShape.FillColor = Color.Red;
        BodyShape.Origin = BodyShape.GetGeometricCenter();
        BodyShape.Position = position;
        Velocity = velocity;
    }

    public void Update(List<Boid> allBoids)
    {
        Vector2f averageBoidPos = GetAverageBoidsPos(allBoids);
        Vector2f difference = averageBoidPos - GetPosition();
        Acceleration = difference; // Changing this value (100) changes the speed at which each boid travels to the average position of all boids
        Velocity += Acceleration;
        LimitVelocity();
        Acceleration = new Vector2f(0, 0);


        BodyShape.Position += Velocity;
        BodyShape.Rotation = GetAngleFromVelocity();
    }

    private Vector2f GetAverageBoidsPos(List<Boid> allBoids)
    {
        float sumPosX = 0;
        float sumPosY = 0;
        foreach (Boid boid in allBoids)
        {
            if (boid == this)
            {
                continue;
            }
            Vector2f boidPos = boid.GetPosition();
            sumPosX += boidPos.X;
            sumPosY += boidPos.Y;

        }
        float nOfBoids = allBoids.Count - 1;
        return new Vector2f(sumPosX / nOfBoids, sumPosY / nOfBoids);
    }

    public void Draw(RenderWindow window)
    {
        window.Draw(BodyShape);
    }

    private void Accelerate(Vector2f force)
    {
        Acceleration += force;
    }

    private float GetAngleFromVelocity()
    {
        float angleInRadians = MathF.Atan2(Velocity.X, -Velocity.Y);
        float angleInDegrees = angleInRadians * 180f / MathF.PI;
        return angleInDegrees;
    }

    private float MaxSpeed = 10f;

    private void LimitVelocity()
    {
        float speed = MathF.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);
        if (speed > MaxSpeed)
        {
            Velocity = new Vector2f(
                Velocity.X / speed * MaxSpeed,
                Velocity.Y / speed * MaxSpeed
            );
        }
    }
    public Vector2f GetPosition()
    {
        return BodyShape.Position;
    }
}
