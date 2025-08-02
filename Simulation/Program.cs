class Program {
    public static void Main(string[] args) {
        FileInfo[] sources = args
            .Where(arg => File.Exists(arg))
            .Select(arg => new FileInfo(arg))
            .ToArray();
        
        if(sources.Length < 1)
            throw new FileLoadException($"No source was provided in [{string.Join(',', args)}]");

        Simulation simulation = new Simulation(sources.First());
        simulation.Run();
    }
}
