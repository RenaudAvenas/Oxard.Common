using System.Diagnostics;
using System.Text;

namespace Oxard.Common.Diagnostics;

public static class MemoryLeakHelper
{
    private const int CheckInterval = 2000;
    private static readonly Dictionary<Type, List<MemoryLeak>> Instances = new Dictionary<Type, List<MemoryLeak>>();
    private static readonly Dictionary<Type, int> NumberAuthorizedInstances = new Dictionary<Type, int>();
    private static readonly List<Type> TypesToCheck = new List<Type>();

    static MemoryLeakHelper()
    {
        var timer = new Timer(TimeSpan.FromMilliseconds(CheckInterval), CheckMemoryLeak);
        timer.Start();
    }

    public static event MemoryLeakEventHandler? MemoryLeakReported;

    public static void RegisterInstance(this object instance, int numberOfAuthorizedInstances = 1)
    {
        var type = instance.GetType();
        NumberAuthorizedInstances[type] = numberOfAuthorizedInstances;

        if (!Instances.ContainsKey(type))
            Instances[type] = new List<MemoryLeak>();

        Instances[type].Add(new MemoryLeak(new WeakReference(instance)));
        TypesToCheck.Add(type);
    }

    private static Dictionary<Type, List<MemoryLeak>> CleanAndSnapshot()
    {
        Dictionary<Type, List<MemoryLeak>> snapshot = new Dictionary<Type, List<MemoryLeak>>();

        foreach (var instanceKvp in Instances)
        {
            var instanceList = instanceKvp.Value;
            for (int i = instanceList.Count - 1; i >= 0; i--)
            {
                instanceList[i].Freeze();
                if (!instanceList[i].IsAlive)
                    instanceList.RemoveAt(i);
                else if (snapshot.ContainsKey(instanceKvp.Key))
                    snapshot[instanceKvp.Key].Add(instanceList[i]);
                else if (TypesToCheck.Contains(instanceKvp.Key) && !snapshot.ContainsKey(instanceKvp.Key))
                    snapshot[instanceKvp.Key] = new List<MemoryLeak> { instanceList[i] };
            }
        }

        return snapshot;
    }

    private static void CheckMemoryLeak()
    {
        var snaphot = CleanAndSnapshot();

        var report = new MemoryLeakReport();

        foreach (var instanceKvp in snaphot)
        {
            if (instanceKvp.Value.Count > NumberAuthorizedInstances[instanceKvp.Key])
                report.AddMemoryLeakReport(instanceKvp.Key, instanceKvp.Value, NumberAuthorizedInstances[instanceKvp.Key]);

            foreach (var memoryLeak in instanceKvp.Value)
                memoryLeak.Free();
        }

        if (!report.IsEmpty)
        {
            Trace.WriteLine(report);
            MemoryLeakReported?.Invoke(null, new MemoryLeakEventArgs(report));
        }
    }

    public class MemoryLeak
    {
        private static int instanceIdentifier = 0;
        private readonly WeakReference weakReference;
        private object? frozenTarget;

        public MemoryLeak(WeakReference weakReference)
        {
            this.weakReference = weakReference;
            Id = ++instanceIdentifier;
        }

        public int Id { get; }

        public DateTime CreationTime { get; set; }

        public bool IsAlive => weakReference.IsAlive;

        public object? Target => weakReference.Target;

        public void Freeze()
        {
            frozenTarget = weakReference.Target;
        }

        public void Free()
        {
            frozenTarget = null;
        }
    }

    public class MemoryLeakReport
    {
        private readonly List<ReportLine> reportLines = new List<ReportLine>();

        public bool IsEmpty { get; set; } = true;

        public void AddMemoryLeakReport(Type type, List<MemoryLeak> instances, int maxAuthorizedInstanceNumber)
        {
            IsEmpty = false;
            reportLines.Add(new ReportLine(type, instances, maxAuthorizedInstanceNumber));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("Memory leaks report :");
            foreach (var reportLine in reportLines)
                builder.Append($"{Environment.NewLine}{reportLine}");

            return builder.ToString();
        }

        private class ReportLine
        {
            private readonly Type type;
            private readonly List<MemoryLeak> instances;
            private readonly int maxAuthorizedInstanceNumber;

            public ReportLine(Type type, List<MemoryLeak> instances, int maxAuthorizedInstanceNumber)
            {
                this.type = type;
                this.instances = instances;
                this.maxAuthorizedInstanceNumber = maxAuthorizedInstanceNumber;
            }

            public override string ToString()
            {
                return $"\t- {instances.Count} instances of type {type} exists instead of {maxAuthorizedInstanceNumber}";
            }
        }
    }
}