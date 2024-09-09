using System;

namespace Support;

public abstract class ADisposable : IDisposable
{
    public bool WasDisposed { get; private set; }
    ~ADisposable() => Dispose(false);
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    private void Dispose(bool manualDispose)
    {
        if (WasDisposed) { return; }
        if (manualDispose) { DisposeManaged(); }
        DisposeUnmanaged();
        WasDisposed = true;
    }
    protected virtual void DisposeManaged() { }
    protected virtual void DisposeUnmanaged() { }
}

