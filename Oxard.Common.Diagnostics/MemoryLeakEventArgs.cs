using System;

namespace Oxard.Common.Diagnostics;

public delegate void MemoryLeakEventHandler(object? sender, MemoryLeakEventArgs args);

public class MemoryLeakEventArgs : EventArgs
{
    public MemoryLeakEventArgs(MemoryLeakHelper.MemoryLeakReport report)
    {
        Report = report;
    }

    public MemoryLeakHelper.MemoryLeakReport Report { get; }
}