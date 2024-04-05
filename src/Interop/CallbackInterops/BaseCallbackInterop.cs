namespace Blazor.FamilyTreeJS.Interop.CallbackInterops;

/// <summary>
/// Taken from https://remibou.github.io/How-to-send-callback-to-JS-Interop-in-Blazor/
/// </summary>
public abstract class BaseCallbackInterop : IDisposable
{
  private bool _disposed;

  // We have this property to help JS know this is the CallbackInterop instance
  #pragma warning disable CA1822
  public bool IsCallbackInterop => true;
  #pragma warning restore CA1822

  // We don't want to expose private class JSInterop<T>Wrapper hence we return it as object
  public object? DotNetRef { get; protected set; }

  /// <summary>
  /// Return true if the callback returns a <see cref="Task"/>
  /// in which indicates that it should be "awaited". <see cref="Task"/> 
  /// is equivalent to Promise in JS. This helps JS code know to whether
  /// await the the callback or not.
  /// </summary>
  public abstract bool IsAsync { get; }

  protected BaseCallbackInterop()
  {
    _disposed = false;
    DotNetRef = null;
  }

  public void Dispose()
  {
    if (_disposed)
    {
      return;
    }

    var disposable = DotNetRef as IDisposable;
    disposable?.Dispose();

    DotNetRef = null;
    _disposed = true;

    GC.SuppressFinalize(this);
  }
}
