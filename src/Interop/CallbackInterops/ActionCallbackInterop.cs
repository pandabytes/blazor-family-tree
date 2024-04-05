namespace Blazor.FamilyTreeJS.Interop.CallbackInterops;

public abstract class ActionCallbackInterop : BaseCallbackInterop
{
  public sealed override bool IsAsync => false;
}

public sealed class ActionCallbackInterop<T> : ActionCallbackInterop
{
  private class JSInteropActionWrapper
  {
    private readonly Action<T> _callback;

    public JSInteropActionWrapper(Action<T> callback) => _callback = callback;

    [JSInvokable]
    public void Invoke(T arg) => _callback.Invoke(arg);
  }

  public ActionCallbackInterop(Action<T> callback)
    => DotNetRef = DotNetObjectReference.Create(new JSInteropActionWrapper(callback));
}

public sealed class ActionCallbackInterop<T1, T2> : ActionCallbackInterop
{
  private class JSInteropActionWrapper
  {
    private readonly Action<T1, T2> _callback;

    public JSInteropActionWrapper(Action<T1, T2> callback) => _callback = callback;

    [JSInvokable]
    public void Invoke(T1 arg1, T2 arg2) => _callback.Invoke(arg1, arg2);
  }

  public ActionCallbackInterop(Action<T1, T2> callback)
    => DotNetRef = DotNetObjectReference.Create(new JSInteropActionWrapper(callback));
}
