using System;

namespace Oxard.Common.Messaging
{
    public interface IMessenger
    {
        void Register<TMessengerArgs>(Action<TMessengerArgs> handler, object trackingTarget = null)
            where TMessengerArgs : MessengerArgs;

        void Send<TMessengerArgs>(TMessengerArgs args)
            where TMessengerArgs : MessengerArgs;
    }
}