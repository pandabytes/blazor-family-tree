# Blazor.FamilyTreeJS

This is a wrapper library for [FamilyTreeJS](https://balkan.app/FamilyTreeJS)
and it is only compatible for Blazor WASM.

Current FamilyTreeJS version packaged in this library: `1.9.36`.

[Here](https://familytreejs.balkan.app/JS/List) is the release note list of FamilyTreeJS.

See samples on [Github page](https://pandabytes.github.io/blazor-family-tree/) (CURRENTLY NOT WORKING!).

# Installation
Install from [Nuget](https://www.nuget.org/packages/Blazor.FamilyTreeJS).
```
dotnet add package Blazor.FamilyTreeJS --version <latest-version>
```

## Depenencies
Register `Blazor.FamilyTreeJS` dependencies.
```cs
builder.Services.AddBlazorFamilyJS();
```

## IJSRuntime configuration
Configure `IJSRuntime`'s `JsonSerializerOptions`. This allows `System.Text.Json` to ignore `null` when
serializing to JSON and send that JSON to Javascript. Note this affects globally.
```cs
var app = builder
  .Build()
  .ConfigureIJSRuntimeJsonOptions();
```

## Enable C# callback interop with Javascript
This library depends on the library `Blazor.Core` in which provides the feature to help
serialize/deserialize C# callback to Javascript. To ensure `Blazor.FamilyTreeJS` work
correctly, you must call `RegisterAttachReviverAsync()` from the `Blazor.Core` library.
```cs
var webHost = builder
  .Build()
  .ConfigureIJSRuntimeJsonOptions();

await webHost.Services.RegisterAttachReviverAsync();
await webHost.RunAsync();
```

## Serialize/deserialize derived types
If you have classes/records that inhereit from classes/records in this library, then you may
need to use the following extension method, `UseDerivedTypes<BaseType>([derive types])`.
This allows serializing your derived classes/records with all of their properties.
```cs
var app = builder
  .Build()
  .ConfigureIJSRuntimeJsonOptions();
  .UseDerivedTypes<NodeMenu>(typeof(CollapseNodeMenu), typeof(DrawLineNodeMenu));

public record CollapseNodeMenu : NodeMenu
{
  public Menu? Collapse { get; init; } = null;
}

public record DrawLineNodeMenu : NodeMenu
{
  public Menu? Draw { get; init; } = null;
}
```

# Usage
Simplest usage is to use the `DefaultFamilyTree` component and provide a tree id.
This component uses the provided `Node` record as the type for the nodes stored in
the family tree.
```razor
<DefaultFamilyTree TreeId="my-tree" />
```

This library is heavily driven by the `FamilyTreeOptions` class so almost
every control and UI are specified via this class. This C# class mirrors this
Typescript [options](https://balkan.app/FamilyTreeJS/API/interfaces/FamilyTree.options) interface.
```razor
<DefaultFamilyTree TreeId="my-tree"
                   Options=@new() { FamilyTreeOptions: new(Mode = "dark") } />
```

If you want to provide your own node with custom properties then you must have
your custom node type inherits `BaseNode`, and use the `FamilyTree<TNode>` component instead.
```razor
public record NodeWithProfession : BaseNode
{
  public string? Job { get; init; }
}

<FamilyTree TNode="NodeWithProfession"
            TreeId="my-tree"
            Options=@new() { FamilyTreeOptions: new(Mode = "dark") } />
```

## Define custom input element
An input element is a HTML element of a node that the user can view and/or edit. For example, the `Node` record has the property `Name`. The equivalent input element of
this property is `<input  type="text" <other_attributes />`. This property can be viewed
when the node is clicked and can be edited when the user navigates to the "Edit details"
of the node.

To provide your own custom HTML for an input element, you can reference to one of the
provided custom input element in this library, such as
[ReadOnlyTextbox](https://github.com/pandabytes/blazor-family-tree/blob/master/src/Components/Interop/Elements/ReadOnlyTextBox.cs).

After you define your custom input element, you have to "register" it with a unique type string. This type
must be unique across all family tree instances. If the same type is defined more than once, an
exception will be thrown.
```razor
public record NodeWithProfession : BaseNode
{
  public string? Job { get; init; }
}

<FamilyTree TNode="NodeWithProfession"
            TreeId="my-tree"
            Options=@new(
              // Skip other properties for brevity
              FamilyTreeOptions: new(
                EditForm: new(
                  Elements: new List<EditFormElement>()
                  {
                    // Here you reference your custom input
                    new(Type: "customInput", Label: "Job", Binding: "job"),
                  }
                )
              ),
              NonFamilyTreeOptions: new(
                CustomInputElements: new Dictionary<string, InputElementCallback<CustomNode>>
                {
                  // Register your custom input here
                  // "customInput" is the type - this must be unique across all family tree instances
                  { "customInput", CustomInput.Callback<NodeWithProfession> }
                }
              )
            ) />
```

Please refer to the [sample project](https://github.com/pandabytes/blazor-family-tree/tree/master/samples/Blazor.FamilyTreeJS.Sample/Pages)
for more examples.

## Using `FamilyTreeStaticModule`
To use the static methods defined in `FamilyTreeJS`, you would have to call them via this class
`FamilyTreeStaticModule`. This class is simply a wrapper class for the static methods defined
in `FamilyTreeJS`. 

To use it, you would have to manually register it in your DI container. Then you would have request
an instance of it and call `.ImportAsync()` to import the JavaScript module. Due to this class having
an asynchronous initilization, it is recommended to regsiter it as a singleton so that you don't
have to deal with when to call `ImportAsync()` when requesting it.
```cs
builder.Services
  .AddBlazorFamilyJS()
  // Register it as a singleton
  .AddSingleton<FamilyTreeStaticModule>()
  .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var webHost = builder.Build();

// Get the module object and import the JS module
var familyTreeStaticModule = webHost.Services.GetRequiredService<FamilyTreeStaticModule>();
await familyTreeStaticModule.ImportAsync();

await webHost.RunAsync();
```