using Raylib_cs;
using System.Numerics;

class Creator {
    enum Selected {
        Particle,
        Line
    }

    FileInfo Source;
    SourceSaver Saver;
    const int Width = 800;
    const int Height = 600;
    const int TextSize = 25;

    const float LineSidesWidth = 15.0f;
    const float CursorWidth = 7.0f;
    const float LineWidth = 5.0f;

    List<Line> Lines;
    List<Particle> Particles;

    Rectangle MenuButtonRect = new Rectangle(0, 0, 100, 40);

    bool DisplayConfiguration = false;

    Selected? SelectedType = null;
    int SelectedElement = 0;

    float
        LClickSpan = 0,
        RClickSpan = 0;
    Vector2
        OrigMousePos = Vector2.Zero,
        CurrMousePos = Vector2.Zero;

    // Before it gets pushed to the main list
    Line? tempLine = null;
    Particle? tempParticle = null;

    private void DrawColorPicker(Vector2 Position, Vector2 MousePosition, ref Color c) {
        Vector2 Size = new Vector2(140, 150),
            SegmentSize = new Vector2(40, 140),
            SegmentOffset = new Vector2(5, 5);
        float SegmentSizeWithOffsetX = SegmentSize.X + SegmentOffset.X;
        
        Raylib.DrawRectangleV(Position, Size, Color.Black);

        Vector2 Segment1 = Position + SegmentOffset + Vector2.UnitX * SegmentSizeWithOffsetX * 0;
        Raylib.DrawRectangleGradientV((int)Segment1.X, (int)Segment1.Y, (int)SegmentSize.X,
                                      (int)SegmentSize.Y, Color.Red, Color.Black);

        Vector2 Segment2 = Position + SegmentOffset + Vector2.UnitX * SegmentSizeWithOffsetX * 1;
        Raylib.DrawRectangleGradientV((int)Segment2.X, (int)Segment2.Y, (int)SegmentSize.X,
                                      (int)SegmentSize.Y, Color.Green, Color.Black);
        
        Vector2 Segment3 = Position + SegmentOffset + Vector2.UnitX * SegmentSizeWithOffsetX * 2;
        Raylib.DrawRectangleGradientV((int)Segment3.X, (int)Segment3.Y, (int)SegmentSize.X,
                                      (int)SegmentSize.Y, Color.Blue, Color.Black);

        int PosR = (int)(SegmentSize.Y - (c.R / 255.0f) * SegmentSize.Y),
            PosG = (int)(SegmentSize.Y - (c.G / 255.0f) * SegmentSize.Y),
            PosB = (int)(SegmentSize.Y - (c.B / 255.0f) * SegmentSize.Y);

        Vector2 SelectionSize = new Vector2(40, 4);
        Raylib.DrawRectangleV(Segment1 + Vector2.UnitY * PosR, SelectionSize, Color.White);
        Raylib.DrawRectangleV(Segment2 + Vector2.UnitY * PosG, SelectionSize, Color.White);
        Raylib.DrawRectangleV(Segment3 + Vector2.UnitY * PosB, SelectionSize, Color.White);
        
        if(Raylib.IsMouseButtonDown(MouseButton.Left)) {
            Vector2 v1 = MousePosition - Segment1,
                v2 = MousePosition - Segment2,
                v3 = MousePosition - Segment3;

            if(v1.X < SegmentSize.X && v1.Y < SegmentSize.Y && 0 < v1.X && 0 < v1.Y)
                c.R = (byte)(255.0f - (v1.Y / SegmentSize.Y * 255.0f));

            if(v2.X < SegmentSize.X && v2.Y < SegmentSize.Y && 0 < v2.X && 0 < v2.Y)
                c.G = (byte)(255.0f - (v2.Y / SegmentSize.Y * 255.0f));

            if(v3.X < SegmentSize.X && v3.Y < SegmentSize.Y && 0 < v3.X && 0 < v3.Y)
                c.B = (byte)(255.0f - (v3.Y / SegmentSize.Y * 255.0f));
        }
    }

    Vector2 ConfSize = new Vector2(200, Height);
    
    private void DrawConfiguration() {
        if(!DisplayConfiguration ||
           SelectedType == null)
            return;

        Vector2 MousePosition = Raylib.GetMousePosition();
        Rectangle
            mainRect = new Rectangle(Vector2.Zero, ConfSize);
        Raylib.DrawRectangleRec(mainRect, Color.DarkGray);

        if(SelectedType == Selected.Line) {
            Raylib.DrawText("Line Config", 0, 0, TextSize, Color.SkyBlue);
            // Do not use CsLo, too complex

            DrawColorPicker(new Vector2(20, 50), MousePosition, ref Lines[SelectedElement / 2].c);
            //var v = mainRect.ToString().SelectMany(x => new string[] { x.ToString().ToLower(), x.ToString().ToUpper() });
            // Configure color

        } else {
            Particle p = Particles[SelectedElement];
            Raylib.DrawText("Particle Config", 0, 0, TextSize, Color.SkyBlue);
            Raylib.DrawText("Change color:", 0, 30, TextSize, Color.White);
            DrawColorPicker(new Vector2(20, 60), MousePosition, ref Particles[SelectedElement].c);
            Raylib.DrawText($"Radius: {p.Radius}", 0, 220, TextSize, Color.White);
            Raylib.DrawText($"Interacts w.:", 0, 250, TextSize, Color.White);

            Rectangle
                chBox1 = new Rectangle(100, 280, 20, 20),
                chBox2 = new Rectangle(100, 300, 20, 20);
            Raylib.DrawText($"Lines:", 20, 280, TextSize - 5, Color.White);
            Raylib.DrawRectangleLinesEx(chBox1, 3.0f, Color.White);
            if(p.WallsInteract) {
                Raylib.DrawLineEx(chBox1.Position,
                                  chBox1.Position + chBox1.Size,
                                  3.0f, Color.White);
                Raylib.DrawLineEx(chBox1.Position + Vector2.UnitX * chBox1.Size.X,
                                  chBox1.Position + chBox1.Size - Vector2.UnitX * chBox1.Size.Y,
                                  3.0f, Color.White);
            }
            
            
                
            Raylib.DrawText($"Others:", 20, 300, TextSize - 5, Color.White);
            Raylib.DrawRectangleLinesEx(chBox2, 3.0f, Color.White);

            if(p.OthersInteract) {
                Raylib.DrawLineEx(chBox2.Position,
                                  chBox2.Position + chBox2.Size,
                                  3.0f, Color.White);
                Raylib.DrawLineEx(chBox2.Position + Vector2.UnitX * chBox2.Size.X,
                                  chBox2.Position + chBox2.Size - Vector2.UnitX * chBox2.Size.Y,
                                  3.0f, Color.White);
            }
            
            if(Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                if(Raylib.CheckCollisionPointRec(MousePosition, chBox2))
                    p.OthersInteract = !p.OthersInteract;
                if(Raylib.CheckCollisionPointRec(MousePosition, chBox1))
                    p.WallsInteract = !p.WallsInteract;
            }

            float velRadius = 85;
            Vector2 velCircle = new Vector2(100, 450);
            Vector2 up = Vector2.UnitY, le = Vector2.UnitX;

            Raylib.DrawText($"Velocity:", 0, 325, TextSize, Color.White);
            
            Raylib.DrawCircleV(velCircle, velRadius, Color.Gray);
            Raylib.DrawLineV(velCircle - up * velRadius, velCircle + up * velRadius, Color.White);
            Raylib.DrawLineV(velCircle - le * velRadius, velCircle + le * velRadius, Color.White);

            Vector2 Button = new Vector2(velCircle.X - 25, velCircle.Y + 95);
            Raylib.DrawRectangleV(Button, new Vector2(50, 20), Color.SkyBlue);
            Raylib.DrawText("Zero", (int)Button.X, (int)Button.Y, 15, Color.White);

            Raylib.DrawCircleV(velCircle + p.Velocity * 10, 5.0f, Color.White);
            if(Raylib.IsMouseButtonDown(MouseButton.Left)) {
                if(Raylib.CheckCollisionPointCircle(MousePosition, velCircle, velRadius)) {
                    p.Velocity = (MousePosition - velCircle) / 10;
                }
                
                if(Raylib.CheckCollisionPointRec(MousePosition, new Rectangle(Button, new Vector2(50, 20)))) {
                    p.Velocity = Vector2.Zero;
                }
            }
            Raylib.DrawText(p.Velocity.ToString("00.000"), 0, (int)Button.Y + 30, TextSize - 5, Color.White);
        }
    }

    private void DrawMenuButton() {
        if(SelectedType == null || DisplayConfiguration) return;

        Raylib.DrawRectangleV(MenuButtonRect.Position, MenuButtonRect.Size, Color.Black);
        Raylib.DrawRectangleLinesEx(MenuButtonRect, 4.0f, Color.LightGray);
        Raylib.DrawText("Config", 5, 5, TextSize, Color.White);
    }

    private void Draw() {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        foreach(Line line in Lines) {
            Raylib.DrawCircleV(line.a, LineSidesWidth, line.c);
            Raylib.DrawCircleV(line.b, LineSidesWidth, line.c);
            Raylib.DrawLineEx(line.a, line.b, LineWidth, line.c);
        }

        foreach(Particle particle in Particles) {
            Raylib.DrawCircleV(particle.Position, particle.Radius, particle.c);
        }

        if(tempLine != null) {
            Raylib.DrawCircleV(tempLine.a, LineSidesWidth, tempLine.c);
            Raylib.DrawCircleV(tempLine.b, LineSidesWidth, tempLine.c);
            Raylib.DrawLineEx(tempLine.a, tempLine.b, LineWidth, tempLine.c);
        } else if(tempParticle != null) {
            Raylib.DrawCircleV(tempParticle.Position, tempParticle.Radius, tempParticle.c);
        }

        if(SelectedType != null) {
            if(SelectedType == Selected.Particle) {
                Particle particle = Particles[SelectedElement];
                Raylib.DrawCircleLinesV(particle.Position, particle.Radius, Color.White);
            } else if(SelectedType == Selected.Line) {
                int Point = SelectedElement & 0x01;
                Line line = Lines[(SelectedElement - Point) / 2];
                //if(Point == 0) Raylib.DrawCircleLinesV(line.a, LineSidesWidth, Color.Red);
                //else Raylib.DrawCircleLinesV(line.b, LineSidesWidth, Color.Red);
                Raylib.DrawCircleLinesV(line.a, LineSidesWidth, Color.Red);
                Raylib.DrawCircleLinesV(line.b, LineSidesWidth, Color.Red);
            }
        }

        DrawMenuButton();
        DrawConfiguration();

        Raylib.EndDrawing();
    }

    private void HandleCheckSave() {
        if(Raylib.IsKeyPressed(KeyboardKey.S) && Raylib.IsKeyDown(KeyboardKey.LeftControl)) {
            Saver.Value = new SourceTuple() {
                Lines = Lines,
                Particles = Particles,
            };

            Console.WriteLine("Saving!");
            Saver.Save(Source.FullName);
        }
    }

    private void LPressed() {
        Vector2 MousePosition = Raylib.GetMousePosition();
        // TODO: Check if not in menu!!! return
        if(DisplayConfiguration) {
            if (MousePosition.X > ConfSize.X || MousePosition.Y > ConfSize.Y) {
                DisplayConfiguration = false;
            }else return; // Handle elsewhere
        } 
        if(SelectedType != null && MousePosition.X < 100) {
            DisplayConfiguration = true;
            return;
        }

        SelectedType = null;
        SelectedElement = 0;
        CurrMousePos = OrigMousePos = MousePosition;

        for(int i =0; i < Lines.Count && SelectedType == null; i++) {
            Line line = Lines[i];

            if(Raylib.CheckCollisionCircles(OrigMousePos, CursorWidth, line.a, LineSidesWidth)) {
                SelectedElement = i * 2;
                SelectedType = Selected.Line;
                Raylib.SetMousePosition((int)line.a.X, (int)line.a.Y);
                
                OrigMousePos = line.a;
            }

            if(Raylib.CheckCollisionCircles(OrigMousePos, CursorWidth, line.b, LineSidesWidth)) {
                SelectedElement = i * 2 + 1;
                SelectedType = Selected.Line;
                Raylib.SetMousePosition((int)line.b.X, (int)line.b.Y);
                
                OrigMousePos = line.b;
            }
        }

        for(int i =0; i < Particles.Count && SelectedType == null; i++) {
            Particle particle = Particles[i];
            if(Raylib.CheckCollisionCircles(OrigMousePos, CursorWidth, particle.Position, particle.Radius + 5.0f)) {
                SelectedElement = i;
                SelectedType = Selected.Particle;
                Raylib.SetMousePosition((int)particle.Position.X, (int)particle.Position.Y);
                
                OrigMousePos = particle.Position;
            }
        }
    }

    private void LHeld() {
        if(DisplayConfiguration) return;

        LClickSpan += Raylib.GetFrameTime();
        CurrMousePos = Raylib.GetMousePosition();

        if(SelectedType == Selected.Particle) {
            Particles[SelectedElement].Position = CurrMousePos;
        } else if(SelectedType == Selected.Line) {
            int Point = SelectedElement & 0x01;
            Line line = Lines[(SelectedElement - Point) / 2];
            if(Point == 0) line.a = CurrMousePos;
            else line.b = CurrMousePos;
        }
    }

    private void LReleased() {
        LClickSpan = 0;
        if((OrigMousePos - CurrMousePos).Length() < 20) {
            DisplayConfiguration = true;
            return;
        }
            
        OrigMousePos = Vector2.Zero;
        CurrMousePos = Vector2.Zero;
    }

    private void HandleLeftClick() {
        if(Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            LPressed();
        } else if(Raylib.IsMouseButtonDown(MouseButton.Left)) {
            LHeld();
        } else if(Raylib.IsMouseButtonReleased(MouseButton.Left)){
            LReleased();
        }
    }

    private void RHeld() {
        RClickSpan += Raylib.GetFrameTime();
        CurrMousePos = Raylib.GetMousePosition();

        if(tempLine == null) {
            if((CurrMousePos - OrigMousePos).Length() > 20) {
                tempParticle = null;
                tempLine = new Line(OrigMousePos, CurrMousePos, Color.White);
                return;
            }

            tempParticle!.Position = CurrMousePos;
            tempParticle!.Radius += Raylib.GetMouseWheelMove();
        } else tempLine!.b = CurrMousePos;
    }

    private void RReleased() {
        RClickSpan = 0;
        OrigMousePos = Vector2.Zero;
        CurrMousePos = Vector2.Zero;

        if(tempLine == null){
            Particles.Add(new Particle(tempParticle!));
            tempParticle = null;
        }
        else {
            Lines.Add(new Line(tempLine!));
            tempLine = null;
        }
    }

    private void HandleRightClick() {
        if(Raylib.IsMouseButtonPressed(MouseButton.Right)) {
            CurrMousePos = OrigMousePos = Raylib.GetMousePosition();
            tempParticle = new Particle(OrigMousePos, 30.0f, null, Color.Red);
        } else if(Raylib.IsMouseButtonDown(MouseButton.Right)) {
            RHeld();
        } else if(Raylib.IsMouseButtonReleased(MouseButton.Right)) {
            RReleased();
        }
    }

    private void Update() {
        HandleCheckSave();
        HandleLeftClick();
        HandleRightClick();

        if(SelectedType == Selected.Particle)
            Particles[SelectedElement].Radius += Raylib.GetMouseWheelMove();
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

    public Creator(FileInfo source, SourceLoader loader, SourceSaver saver) {
        Lines = loader.Value.Lines ?? new List<Line>();
        Particles = loader.Value.Particles ?? new List<Particle>();

        Saver = saver;
        Source = source;
    }
}
