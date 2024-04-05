# Blazor.FamilyTreeJS

This is a wrapper library for [FamilyTreeJS](https://balkan.app/FamilyTreeJS).

FamilyTreeJS version: `1.09.12`

requireJS version: `2.3.6`

# Installation

Add the following lines in your `wwwroot/index.html`.
```html
<!-- This is the core FamilyTree library. -->
<script src="_content/Blazor.FamilyTreeJS/FamilyTree.js"></script>

<!-- This is the interop layer that communicates between JS and C#.
     We use requirejs to load this JS file.
-->
<script data-main="_content/Blazor.FamilyTreeJS/family-tree-interop.js"
        src="_content/Blazor.FamilyTreeJS/requirejs.js">
```

Register `Blazor.FamilyTreeJS` dependencies.
```cs
builder.Services.AddBlazorFamilyJS();
```

Configure `IJSRuntime`'s `JsonSerializerOptions`. This allows `System.Text.Json` to ignore `null` when
serializing to JSON and send that JSON to Javascript.
```cs
var app = builder.Build();

app.Services.ConfigureIJSRuntimeJsonOptions();
```

If you have classes/records that inhereit from classes/records in this library, then you must use
the following extension method(s), `Use<type>DerivedTypes()`. This allows serializing
your derived classes/records with all of their properties.
```cs
var app = builder.Build();

app.Services
  .ConfigureIJSRuntimeJsonOptions()
  .UseNodeMenuDerivedTypes(typeof(CustomNodeMenu));

public record CustomNodeMenu : NodeMenu
{
  public Menu? Custom { get; init; } = null;
}
```
