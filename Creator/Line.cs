// For Color
using Raylib_cs;
using System.Numerics;

class Line {
    public Vector2 a;
    public Vector2 b;
    public float Len { get { return Vector2.Distance(a, b); } }
    public float Angle { get {
            Vector2 Point = Vector2.Normalize(a - Center);
            return MathF.Atan2(Point.Y, Point.X);
        }
    }
    public float Angle_deg { get { return Single.RadiansToDegrees(Angle); } }
    public Vector2 Center { get { return (a + b) / 2; } }
    public Color c;

    public Line(Vector2 a, Vector2 b, Color c) {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    public Line(Color c) {
        this.a = Vector2.Zero;
        this.b = Vector2.Zero;
        this.c = c;
    }

    public Line(Line line) {
        this.a = line.a;
        this.b = line.b;
        this.c = line.c;
    }
    
    public Line() {
        this.a = Vector2.Zero;
        this.b = Vector2.Zero;
        this.c = Color.White;
    }
}
