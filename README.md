# Blazor.FamilyTreeJS

This is a wrapper library for [FamilyTreeJS](https://balkan.app/FamilyTreeJS)
and it is only compatible for Blazor WASM.

FamilyTreeJS version: `1.9.18`

# Installation
Install from Nuget (not available yet).
```
dotnet add package Blazor.FamilyTreeJS --version <latest-version>
```

Register `Blazor.FamilyTreeJS` dependencies.
```cs
builder.Services.AddBlazorFamilyJS();
```

Configure `IJSRuntime`'s `JsonSerializerOptions`. This allows `System.Text.Json` to ignore `null` when
serializing to JSON and send that JSON to Javascript. Note this affects globally.
```cs
var app = builder
  .Build()
  .ConfigureIJSRuntimeJsonOptions();
```

If you have classes/records that inhereit from classes/records in this library, then you may
need to use the following extension method, `UseDerivedTypes<BaseType>([derive types])`.
This allows serializing your derived classes/records with all of their properties.
```cs
var app = builder
  .Build()
  .ConfigureIJSRuntimeJsonOptions();
  .UseDerivedTypes<Node>(typeof(NodeWithProfession))
  .UseDerivedTypes<NodeMenu>(typeof(CollapseNodeMenu), typeof(DrawLineNodeMenu));

public record NodeWithProfession : Node
{
  public string? Job { get; init; } = null;
}

public record CollapseNodeMenu : NodeMenu
{
  public Menu? Collapse { get; init; } = null;
}

public record DrawLineNodeMenu : NodeMenu
{
  public Menu? Draw { get; init; } = null;
}
```
