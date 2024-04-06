namespace Blazor.FamilyTreeJS.Components.Interop.Options;

/// <summary>
/// Represent a node menu with
/// an event handler to handle
/// when this node menu is clicked on.
/// </summary>
/// <param name="Text">Text of the node menu.</param>
/// <param name="Icon">Icon of the node menu</param>
/// <param name="OnClick">
/// First argument is the node id.
/// Second argument is unused, hence it's always null.
/// </param>
public record Menu(
  string Text,
  string? Icon = null,
  ActionCallbackInterop<string, object?>? OnClick = null
);

public record NodeMenu
{
  public Menu? Add { get; init; } = null;

  public Menu? Details { get; init; } = null;

  public Menu? Edit { get; init; } = null;

  public Menu? Remove { get; init; } = null;
}
