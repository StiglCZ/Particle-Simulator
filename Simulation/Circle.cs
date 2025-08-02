using System.Numerics;

// For Color
using Raylib_cs;

class Particle {
    public Vector2 Position, Velocity;
    public float Radius;
    public Color c;

    public bool OthersInteract, WallsInteract;

    public Particle(Vector2 Position, float Radius,
                  Vector2? Velocity, Color? color,
                  bool OthersInteract = false,
                  bool WallsInteract = true) {
        this.OthersInteract = OthersInteract;
        this.WallsInteract = WallsInteract;
        this.Position = Position;
        this.Radius = Radius;
        this.Velocity = Velocity ?? Vector2.Zero;
        this.c = color ?? Color.White;
    }

    [System.Text.Json.Serialization.JsonConstructor]
    public Particle() {
        
    }
};
