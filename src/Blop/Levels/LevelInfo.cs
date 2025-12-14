namespace Conkeron.Blop.Levels;

[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}()}}")]
public class LevelInfo
{
	#region Properties
	public required string Id { get; init; }
	public required string Path { get; init; }
	public required string Directory { get; init; }
	public required string Name { get; init; }
	public required string Description { get; init; }
	#endregion

	#region Helpers
	private string DebuggerDisplay() => $"{nameof(LevelInfo)} {{ {nameof(Id)} = ({Id}), {nameof(Name)} = ({Name}) }}";
	#endregion
}
