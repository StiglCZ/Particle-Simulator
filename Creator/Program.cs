class Program {
    public static void Main(string[] args) {
        if(args.Length < 1) {
            Console.WriteLine("Provide a file in first argument!");
            Environment.Exit(1);
        }
        
        SourceTuple src = new();
        FileInfo sourceFile = new FileInfo(args[0]);

        SourceLoader loader = new JsonSourceLoader();
        SourceSaver saver = new JsonSourceSaver();
        
        if(sourceFile.Exists) {
            src = loader.Load(sourceFile.FullName);
        }
        // Write the exact same config to verify the file is rw
        
        saver.Value = src;
        saver.Save(sourceFile.FullName);
        loader.Load(sourceFile.FullName);
        
        Creator creator = new Creator(sourceFile, loader, saver);
        creator.Run();

    }
}
