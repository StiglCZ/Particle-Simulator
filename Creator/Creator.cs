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

    private void DrawConfiguration() {
        if(!DisplayConfiguration ||
           SelectedType == null)
            return;

        Rectangle
            mainRect = new Rectangle(Vector2.Zero, 200, Height);
        Raylib.DrawRectangleRec(mainRect, Color.DarkGray);
        
        if(SelectedType == Selected.Line) {
            Raylib.DrawText("Line Config", 0, 0, TextSize, Color.SkyBlue);
            
            // Configure color
            
        } else {
            Raylib.DrawText("Particle Config", 0, 0, TextSize, Color.SkyBlue);

            // Configure everything else
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
        if(DisplayConfiguration) return; // Handle elsewhere
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
            }

            if(Raylib.CheckCollisionCircles(OrigMousePos, CursorWidth, line.b, LineSidesWidth)) {
                SelectedElement = i * 2 + 1;
                SelectedType = Selected.Line;
                Raylib.SetMousePosition((int)line.b.X, (int)line.b.Y);
            }
        }
        
        for(int i =0; i < Particles.Count && SelectedType == null; i++) {
            Particle particle = Particles[i];
            if(Raylib.CheckCollisionCircles(OrigMousePos, CursorWidth, particle.Position, particle.Radius + 5.0f)) {
                SelectedElement = i;
                SelectedType = Selected.Particle;
                Raylib.SetMousePosition((int)particle.Position.X, (int)particle.Position.Y);
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
