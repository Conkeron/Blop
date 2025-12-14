namespace Conkeron.Blop.Levels;

public class LevelInfoParser
{
	#region Properties
	private string Id { get; }
	private string Path { get; }
	private string Directory { get; }
	private int Line { get; set; }
	#endregion

	#region Constructors
	private LevelInfoParser(string path)
	{
		Path = path;
		Id = System.IO.Path.GetFileNameWithoutExtension(path);
		Directory = System.IO.Path.GetDirectoryName(path) ?? throw new ArgumentException($"The level file should be in some directory.", nameof(path));
	}
	#endregion

	#region Functions
	public static LevelInfo Parse(string path)
	{
		LevelInfoParser parser = new(path);
		LevelInfo info = parser.Parse();

		return info;
	}
	#endregion

	#region Methods
	private LevelInfo Parse()
	{
		Dictionary<string, object> map = ReadMap();

		return new()
		{
			Id = Id,
			Path = Path,
			Directory = Directory,
			Name = (string)map["name"],
			Description = (string)map["description"],
		};
	}
	private Dictionary<string, object> ReadMap()
	{
		using (StreamReader reader = new(Path, Encoding.UTF8))
		{
			Dictionary<string, object> map = ReadMap(reader);
			return map;
		}
	}
	private Dictionary<string, object> ReadMap(StreamReader reader)
	{
		Dictionary<string, object> map = [];

		Line = 0;
		string? line;
		while ((line = reader.ReadLine()) is not null)
		{
			Line++;

			if (string.IsNullOrWhiteSpace(line))
				continue;

			ReadOnlySpan<char> remaining = line;
			string key = ReadKey(ref remaining);
			object value = ReadValue(ref remaining);

			remaining = SkipSpace(remaining);
			if (remaining.Length > 0)
				Error($"There was unparsed text left for the ({key}) key.");

			map.Add(key, value);
		}

		return map;
	}
	private string ReadKey(ref ReadOnlySpan<char> remaining)
	{
		int colon = remaining.IndexOf(':');
		if (colon < 0)
			Error("Expected a colon ':' to separate the key and the value.");

		string key = remaining[..colon].Trim().ToString();
		if (string.IsNullOrWhiteSpace(key))
			Error("The key cannot be empty.");

		if (colon == remaining.Length)
			Error($"Expected a value after the ({key}) key.");

		remaining = SkipSpace(remaining[(colon + 1)..]);
		return key;
	}
	private object ReadValue(ref ReadOnlySpan<char> remaining)
	{
		if (remaining.Length is 0)
			Error("There was no text left to parse as the value.");

		if (remaining[0] is '"')
			return ReadStringValue(ref remaining);

		Error($"Unknown value type ({remaining})");
		return default;
	}
	#endregion

	#region Value methods
	private string ReadStringValue(ref ReadOnlySpan<char> remaining)
	{
		if (remaining.Length is 0 || remaining[0] is not '"')
			Error("A string value must being with a quote mark '\"'.");

		remaining = remaining[1..];

		StringBuilder builder = new();

		while (remaining.Length > 0)
		{
			if (remaining[0] is '"')
			{
				remaining = remaining[1..];
				return builder.ToString();
			}

			builder.Append(remaining[0]);
			remaining = remaining[1..];
		}

		Error("Unclosed string value.");
		return default;
	}
	#endregion

	#region Helpers
	[DoesNotReturn]
	private void Error(string error)
	{
		throw new InvalidOperationException($"Error in parsing level file ({Id}, {Line}): {error}");
	}
	private static ReadOnlySpan<char> SkipSpace(ReadOnlySpan<char> remaining)
	{
		while (remaining.Length > 0)
		{
			if (char.IsWhiteSpace(remaining[0]) is false)
				break;

			remaining = remaining[1..];
		}

		return remaining;
	}
	#endregion
}
