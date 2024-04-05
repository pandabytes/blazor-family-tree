namespace Blazor.FamilyTreeJS.Interop.CallbackInterops;

public class FuncCallbackInterop<TResult> : BaseCallbackInterop
{
  private class JSInteropFuncWrapper
  {
    private readonly Func<TResult> _callback;

    public JSInteropFuncWrapper(Func<TResult> callback) => _callback = callback;

    [JSInvokable]
    public TResult Invoke() => _callback.Invoke();
  }

  public sealed override bool IsAsync
  {
    get
    {
      var resultType = typeof(TResult);
      return resultType == typeof(Task) || 
             (resultType.IsGenericType && 
              resultType.GetGenericTypeDefinition() == typeof(Task<>));
    }
  }

  /// <summary>
  /// For derived classes.
  /// </summary>
  protected FuncCallbackInterop() {}

  public FuncCallbackInterop(Func<TResult> callback)
    => DotNetRef = DotNetObjectReference.Create(new JSInteropFuncWrapper(callback));
}

public sealed class FuncCallbackInterop<T, TResult> : FuncCallbackInterop<TResult>
{
  private class JSInteropFuncWrapper
  {
    private readonly Func<T, TResult> _callback;

    public JSInteropFuncWrapper(Func<T, TResult> callback) => _callback = callback;

    [JSInvokable]
    public TResult Invoke(T arg) => _callback.Invoke(arg);
  }

  public FuncCallbackInterop(Func<T, TResult> callback)
    => DotNetRef = DotNetObjectReference.Create(new JSInteropFuncWrapper(callback));
}

public sealed class FuncCallbackInterop<T1, T2, TResult> : FuncCallbackInterop<TResult>
{
  private class JSInteropFuncWrapper
  {
    private readonly Func<T1, T2, TResult> _callback;

    public JSInteropFuncWrapper(Func<T1, T2, TResult> callback) => _callback = callback;

    [JSInvokable]
    public TResult Invoke(T1 arg1, T2 arg2) => _callback.Invoke(arg1, arg2);
  }

  public FuncCallbackInterop(Func<T1, T2, TResult> callback)
    => DotNetRef = DotNetObjectReference.Create(new JSInteropFuncWrapper(callback));
}
