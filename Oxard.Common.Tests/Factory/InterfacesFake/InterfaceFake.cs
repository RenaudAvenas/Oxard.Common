using System;

namespace Oxard.Common.Tests.Factory.InterfacesFake
{
    public interface IInterfaceFake
    {
    }

    public interface IInterface1 : IInterfaceFake
    {

    }

    public interface IInterface2 : IInterfaceFake
    {

    }

    public class ObjectForInterface1 : IInterface1
    {
        public ObjectForInterface1()
        {
            if (OnCreated != null)
            {
                OnCreated();
            }
        }

        public static Action OnCreated { get; set; }
    }

    internal class ObjectForInterface2 : IInterface2
    {
        public ObjectForInterface2()
        {
            if (OnCreated != null)
            {
                OnCreated();
            }
        }

        public static Action OnCreated { get; set; }
    }
}
