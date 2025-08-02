using Raylib_cs;
using System.Numerics;

class Simulation {
    const int Width = 800;
    const int Height = 600;

    const float LineWidth = 5.0f;

    List<Line> Lines;
    List<Particle> Particles;

    private void Draw() {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        foreach(Line line in Lines)
            Raylib.DrawLineEx(line.a, line.b, LineWidth, line.c);
        foreach(Particle particle in Particles)
            Raylib.DrawCircleV(particle.Position, particle.Radius, particle.c);

        Raylib.EndDrawing();
    }

    private void Update() {
        foreach (Particle part in Particles) {
            Parallel.ForEach<Particle>(Particles, part2 => {
                if(part == part2)
                    return;
                if (!Raylib.CheckCollisionCircles(part.Position, part.Radius, part2.Position, part2.Radius))
                    return;

                Vector2 RelativeVelocity = part2.Velocity - part.Velocity;
                Vector2 DistanceNormal = Vector2.Normalize(part2.Position - part.Position);

                float VelocityNormal =
                    + RelativeVelocity.X * DistanceNormal.X
                    + RelativeVelocity.Y * DistanceNormal.Y;

                const float e = 1.00f;
                float k = -(1 + e) * VelocityNormal / (1 / part.Radius + 1 / part2.Radius);

                Vector2 CollisionImpulse = DistanceNormal * k;
                part.Velocity  -= CollisionImpulse / part.Radius;
                part2.Velocity += CollisionImpulse / part2.Radius;
            });

            Vector2 VelocityNormal  = Vector2.Normalize(part.Velocity);
            float VelocityMagnitude = part.Velocity.Length();
            float VelocityAngle     = MathF.Atan2(VelocityNormal.Y, VelocityNormal.X);

            Parallel.ForEach<Line>(Lines, (line) => {
                if(!Raylib.CheckCollisionCircleLine(part.Position, part.Radius, line.a, line.b))
                    return;
                if(Raylib.CheckCollisionCircles(line.a, 5, part.Position, part.Radius) ||
                   Raylib.CheckCollisionCircles(line.b, 5, part.Position, part.Radius)) {
                    part.Velocity *= -1;
                    return;
                }
                
                float CombinedAngle = 2 * line.Angle - VelocityAngle;
                Vector2 ResultNormalized = new Vector2(MathF.Cos(CombinedAngle), MathF.Sin(CombinedAngle));
                part.Velocity = ResultNormalized * VelocityMagnitude;
            });

            part.Position += part.Velocity;
        }
    }

    public void Run() {
        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.InitWindow(Width, Height, "Simulation");
        Raylib.SetTargetFPS(60);

        while(!Raylib.WindowShouldClose()) {
            Update();
            Draw();
        }
    }

    public Simulation (FileInfo Source) {
        SourceLoader Loader = new JsonSourceLoader();
        Loader.Load(Source.FullName);
        Lines = Loader.Value.Lines;
        Particles = Loader.Value.Particles;


    }
}
