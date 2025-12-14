namespace Conkeron.Blop;

class Program
{
	#region Properties
	private static string RootDirectory { get; } = GetRootDirectory();
	private static string ContentDirectory { get; } = Path.Combine(RootDirectory, "content");
	private static string OutputDirectory { get; } = Path.Combine(RootDirectory, "output");
	#endregion

	static void Main(string[] args)
	{
		Directory.CreateDirectory(ContentDirectory);

		if (Directory.Exists(OutputDirectory))
			Directory.Delete(OutputDirectory, true);

		Directory.CreateDirectory(OutputDirectory);

		Console.WriteLine($"Root directory: {RootDirectory}");
		Console.WriteLine($"Game content directory: {ContentDirectory}");
		Console.WriteLine($"Game output directory: {OutputDirectory}");
		LevelInfo[] levelFiles = FindLevelFiles();


		string indexPath = Path.Combine(OutputDirectory, "index.html");
		File.WriteAllText(indexPath, """
		<!DOCTYPE html>
		<html lang="en">
			<head>
				<meta charset="UTF-8">
				<meta name="viewport" content="width=device-width, initial-scale=1.0">
				<title>Blop</title>
			</head>
			<body>
				<h1>Blop</h1>
				<p>Hello, nothing here yet, check back later!</p>
			</body>
		</html>
		""");
	}

	#region Helpers
	private static string GetRootDirectory()
	{
		string? directory = Environment.CurrentDirectory;
		while (directory is not null)
		{
			string gitDirectory = Path.Combine(directory, ".git");
			if (Directory.Exists(gitDirectory))
				return directory;

			directory = Path.GetDirectoryName(directory);
		}

		throw new InvalidOperationException("The generator must be ran from a git repository.");
	}
	private static LevelInfo[] FindLevelFiles()
	{
		string[] files = Directory.GetFiles(ContentDirectory, "*.level", SearchOption.AllDirectories);

		Console.WriteLine("\nSearching for level files...");
		foreach (string file in files)
		{
			string id = Path.GetFileNameWithoutExtension(file);
			string relative = Path.GetRelativePath(ContentDirectory, file);
			string directory = Path.GetDirectoryName(file) ?? throw new UnreachableException("A file must always be in some directory.");

			Console.WriteLine($"Level({id}): {relative} ({Path.GetDirectoryName(relative)})");

			string[] nestedLevels = Directory.GetFiles(directory, "*.level", SearchOption.AllDirectories);

			if (nestedLevels.Length is not 1 && nestedLevels[0] != file)
				throw new NotSupportedException($"Nested levels are not supported.");

		}

		LevelInfo[] levels = new LevelInfo[files.Length];

		Console.WriteLine("\nParsing level files...");
		for (int i = 0; i < files.Length; i++)
		{
			LevelInfo level = LevelInfoParser.Parse(files[i]);
			levels[i] = level;

			Console.WriteLine($"level({level.Id}): \"{level.Name}\", \"{level.Description}\"");
		}

		return levels;
	}
	#endregion
}
