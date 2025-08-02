using System.Text;
using System.Text.Json;

struct SourceTuple {
    public List<Line> Lines;
    public List<Particle> Particles;
    public SourceTuple() {
        Lines = new List<Line>();
        Particles = new List<Particle>();
    }
}

interface SourceLoader {
    public SourceTuple Value { get; protected set; }
    public SourceTuple Load(string FileSource);
}

class JsonSourceLoader : SourceLoader {
    public SourceTuple Value { get; set; }
    public SourceTuple Load(string FileSource) {
        try {
            StreamReader reader = new StreamReader(FileSource, Encoding.UTF8);
            String JsonSource = reader.ReadToEnd();
            reader.Close();
            JsonSerializerOptions options = new() { IncludeFields = true };

            this.Value = JsonSerializer.Deserialize<SourceTuple>(JsonSource, options);
            return this.Value;
        } catch(JsonException ex) {
            throw new FileLoadException($"Json Parse Error: {ex.Message}");
        }
    }
}
