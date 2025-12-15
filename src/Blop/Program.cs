namespace Conkeron.Blop;

class Program
{
	#region Properties
	private static string RootDirectory { get; } = GetRootDirectory();
	private static string ContentDirectory { get; } = Path.Combine(RootDirectory, "content");
	private static string SystemDirectory { get; } = Path.Combine(ContentDirectory, "system");
	private static string OutputDirectory { get; } = Path.Combine(RootDirectory, "output");
	#endregion

	static void Main(string[] args)
	{
		Directory.CreateDirectory(ContentDirectory);

		if (Directory.Exists(OutputDirectory))
			Directory.Delete(OutputDirectory, true);

		Directory.CreateDirectory(OutputDirectory);
		Directory.CreateDirectory(SystemDirectory);

		Console.WriteLine($"Root directory: {RootDirectory}");
		Console.WriteLine($"Game content directory: {ContentDirectory}");
		Console.WriteLine($"Game content directory: {SystemDirectory}");
		Console.WriteLine($"Game output directory: {OutputDirectory}");

		string html = GenerateHtml();
		string indexPath = Path.Combine(OutputDirectory, "index.html");
		File.WriteAllText(indexPath, html);
		Console.WriteLine($"Final index.html size: {html.Length / 1024d:n2} KB");
	}

	#region Helpers
	private static string GenerateHtml()
	{
		string templatePath = Path.Combine(SystemDirectory, "main_template.html");
		string stylePath = Path.Combine(SystemDirectory, "style.css");
		string enginePath = Path.Combine(SystemDirectory, "engine.js");

		string template = File.ReadAllText(templatePath);
		string style = File.ReadAllText(stylePath);
		string engine = File.ReadAllText(enginePath);
		string gameData = GetGameDataJson();

		template = template
			.Replace("${game-title}", "Blop")
			.Replace("${game-description}", "A blog hopping game.")
			.Replace("${game-data}", EmbedCode(gameData))
			.Replace("${system-style}", EmbedCode(style))
			.Replace("${system-engine}", EmbedCode(engine));

		return template;
	}
	private static string GetGameDataJson()
	{
		return """
		{
			"id": "blop",
			"title": "Blop",
			"description": "A blog hopping game.",
			"version": "gamejam",
			"levels": [
				{
					"id": "introduction",
					"title": "Introduction",
					"description": "An introduction to Blop."
				}
			]
		}
		""";
	}
	private static string EmbedCode(string text)
	{
		string[] lines = text.TrimEnd().Split('\r', '\n');
		for (int i = 0; i < lines.Length; i++)
			lines[i] = "\t\t\t" + lines[i];

		return string.Concat(
			Environment.NewLine,
			string.Join(Environment.NewLine, lines),
			Environment.NewLine, "\t\t"
		);
	}
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
