namespace Blazor.FamilyTreeJS.Components.Interop.Options;

/// <summary>
/// Toolbar options. See https://balkan.app/FamilyTreeJS/API/interfaces/FamilyTree.options#toolbar
/// </summary>
/// <param name="Layout">Show different layouts.</param>
/// <param name="Zoom">Show zoom buttons (+/-).</param>
/// <param name="Fit">Show fit button (fitting the entire tree in view).</param>
/// <param name="ExpandAll">Show expand all button.</param>
/// <param name="FullScreen">Show full screen button.</param>
public sealed record Toolbar(
  bool? Layout = null,
  bool? Zoom = null,
  bool? Fit = null,
  bool? ExpandAll = null,
  bool? FullScreen = null
);
