namespace Blazor.FamilyTreeJS.Components.Interop;

public sealed class FamilyTreeJsModule : BaseJsModule
{
  protected override string ModulePath => $"{ModulePrefixPath}/js/{nameof(Components)}/{nameof(Interop)}/FamilyTree.js";

  public FamilyTreeJsModule(IJSRuntime jSRuntime) : base(jSRuntime)
  {}
}
