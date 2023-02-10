# Oxard.Common.Diagnostics

The aim of this project is to write some usefull class to detect memory leaks etc... at debug time.

## MemoryLeakHelper
Use MemoryLeakHelper to track instances and check how many times they appear in memory at a time.

The code bellow demonstrate to track a TrackedClass to see if its appear more than 2 times in memory.
```csharp
public class TrackedClass
{
	public TrackedClass()
	{
		MemoryLeakHelper.RegisterInstance(2);
	}
}
```

MemoryLeakHelper write the leak results in traces but you can catch the result with the MemoryLeakHelper.MemoryLeakReported event