namespace Blazor.FamilyTreeJS.Interop.CallbackInterops;

/// <summary>
/// Note when using this class, we copy 
/// <see cref="EventCallback"/> because
/// <see cref="EventCallback"/> is a struct.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class EventCallbackInterop<T> : BaseCallbackInterop
{
  private class JSInteropFuncWrapper
  {
    private readonly EventCallback<T> _callback;

    public JSInteropFuncWrapper(EventCallback<T> callback) => _callback = callback;

    // Keep name as Invoke (instead of usually InvokeAsync)
    // so that JS only needs to know to call "Invoke"
    [JSInvokable]
    public async Task Invoke(T arg) => await _callback.InvokeAsync(arg);
  }

  public override bool IsAsync => true;

  public EventCallbackInterop(EventCallback<T> callback)
    => DotNetRef = DotNetObjectReference.Create(new JSInteropFuncWrapper(callback));
}
