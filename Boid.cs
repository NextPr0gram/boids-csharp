using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

public class Boid
{
    private readonly ConvexShape bodyShape;
    private Vector2f velocity;
    private Vector2f acceleration;
    private readonly float maxForce;
    private readonly float maxSpeed;
    private readonly float radius;

    public Boid(Vector2f position, Vector2f velocity)
    {
        bodyShape = new ConvexShape(3);
        bodyShape.SetPoint(0, new Vector2f(0f, -8f));
        bodyShape.SetPoint(1, new Vector2f(-5f, 8f));
        bodyShape.SetPoint(2, new Vector2f(5f, 8f));
        bodyShape.FillColor = Color.Red;
        bodyShape.OutlineColor = Color.White;
        bodyShape.OutlineThickness = 1f;
        bodyShape.Origin = new Vector2f(0f, 0f);
        bodyShape.Position = position;

        this.velocity = velocity;
        acceleration = new Vector2f(0f, 0f);

        radius = 2f;
        maxSpeed = 2f;
        maxForce = 0.03f;
    }

    public void Update(List<Boid> allBoids, Vector2u windowSize)
    {
        Flock(allBoids);

        velocity += acceleration;
        velocity = Limit(velocity, maxSpeed);

        bodyShape.Position += velocity;
        acceleration = new Vector2f(0f, 0f);

        Borders(windowSize);

        bodyShape.Rotation = GetAngleFromVelocity();
    }

    public void Draw(RenderWindow window)
    {
        window.Draw(bodyShape);
    }

    public Vector2f GetPosition()
    {
        return bodyShape.Position;
    }

    private void ApplyForce(Vector2f force)
    {
        acceleration += force;
    }

    private void Flock(List<Boid> allBoids)
    {
        Vector2f sep = Separate(allBoids);
        Vector2f ali = Align(allBoids);
        Vector2f coh = Cohesion(allBoids);

        sep *= 1.5f;
        ali *= 1.0f;
        coh *= 1.0f;

        ApplyForce(sep);
        ApplyForce(ali);
        ApplyForce(coh);
    }

    private Vector2f Seek(Vector2f target)
    {
        Vector2f desired = target - bodyShape.Position;
        desired = Normalize(desired);
        desired *= maxSpeed;

        Vector2f steer = desired - velocity;
        steer = Limit(steer, maxForce);
        return steer;
    }

    private Vector2f Separate(List<Boid> allBoids)
    {
        float desiredSeparation = 25f;
        Vector2f steer = new Vector2f(0f, 0f);
        int count = 0;

        foreach (Boid other in allBoids)
        {
            if (other == this)
            {
                continue;
            }

            float d = Distance(bodyShape.Position, other.GetPosition());

            if (d > 0f && d < desiredSeparation)
            {
                Vector2f diff = bodyShape.Position - other.GetPosition();
                diff = Normalize(diff);
                diff /= d;
                steer += diff;
                count++;
            }
        }

        if (count > 0)
        {
            steer /= count;
        }

        if (Magnitude(steer) > 0f)
        {
            steer = Normalize(steer);
            steer *= maxSpeed;
            steer -= velocity;
            steer = Limit(steer, maxForce);
        }

        return steer;
    }

    private Vector2f Align(List<Boid> allBoids)
    {
        float neighborDist = 50f;
        Vector2f sum = new Vector2f(0f, 0f);
        int count = 0;

        foreach (Boid other in allBoids)
        {
            if (other == this)
            {
                continue;
            }

            float d = Distance(bodyShape.Position, other.GetPosition());

            if (d > 0f && d < neighborDist)
            {
                sum += other.velocity;
                count++;
            }
        }

        if (count > 0)
        {
            sum /= count;
            sum = Normalize(sum);
            sum *= maxSpeed;

            Vector2f steer = sum - velocity;
            steer = Limit(steer, maxForce);
            return steer;
        }

        return new Vector2f(0f, 0f);
    }

    private Vector2f Cohesion(List<Boid> allBoids)
    {
        float neighborDist = 50f;
        Vector2f sum = new Vector2f(0f, 0f);
        int count = 0;

        foreach (Boid other in allBoids)
        {
            if (other == this)
            {
                continue;
            }

            float d = Distance(bodyShape.Position, other.GetPosition());

            if (d > 0f && d < neighborDist)
            {
                sum += other.GetPosition();
                count++;
            }
        }

        if (count > 0)
        {
            sum /= count;
            return Seek(sum);
        }

        return new Vector2f(0f, 0f);
    }

    private void Borders(Vector2u windowSize)
    {
        Vector2f pos = bodyShape.Position;

        if (pos.X < -radius)
        {
            pos.X = windowSize.X + radius;
        }

        if (pos.Y < -radius)
        {
            pos.Y = windowSize.Y + radius;
        }

        if (pos.X > windowSize.X + radius)
        {
            pos.X = -radius;
        }

        if (pos.Y > windowSize.Y + radius)
        {
            pos.Y = -radius;
        }

        bodyShape.Position = pos;
    }

    private float GetAngleFromVelocity()
    {
        float angleRadians = MathF.Atan2(velocity.Y, velocity.X);
        return angleRadians * 180f / MathF.PI + 90f;
    }

    private static float Magnitude(Vector2f vector)
    {
        return MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
    }

    private static Vector2f Normalize(Vector2f vector)
    {
        float mag = Magnitude(vector);

        if (mag == 0f)
        {
            return new Vector2f(0f, 0f);
        }

        return vector / mag;
    }

    private static Vector2f Limit(Vector2f vector, float limit)
    {
        float mag = Magnitude(vector);

        if (mag > limit && mag > 0f)
        {
            vector = Normalize(vector);
            vector *= limit;
        }

        return vector;
    }

    private static float Distance(Vector2f a, Vector2f b)
    {
        return Magnitude(a - b);
    }
}
