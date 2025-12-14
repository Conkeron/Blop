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
			Directory.Delete(OutputDirectory);

		Directory.CreateDirectory(OutputDirectory);

		Console.WriteLine($"Root directory: {RootDirectory}");
		Console.WriteLine($"Game content directory: {ContentDirectory}");
		Console.WriteLine($"Game output directory: {OutputDirectory}");

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
	#endregion
}
