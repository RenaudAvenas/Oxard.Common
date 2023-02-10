using System;
using System.Collections.Generic;

namespace Oxard.Common.Messaging
{
    public class Messenger : IMessenger
    {
        private readonly Dictionary<Type, IMessengerRegister> registers = new Dictionary<Type, IMessengerRegister>();
        private readonly object locker = new object();

        public void Register<TMessengerArgs>(Action<TMessengerArgs> handler, object trackingTarget = null)
            where TMessengerArgs : MessengerArgs
        {
            var messengerRegisterType = typeof(MessengerRegister<TMessengerArgs>);

            lock (this.locker)
            {
                if (!this.registers.ContainsKey(messengerRegisterType))
                {
                    this.registers.Add(messengerRegisterType, new MessengerRegister<TMessengerArgs>());
                }

                ((MessengerRegister<TMessengerArgs>)this.registers[messengerRegisterType]).AddHandler(handler, trackingTarget);
            }
        }

        public void Unregister<TMessengerArgs>(Action<TMessengerArgs> handler)
            where TMessengerArgs : MessengerArgs
        {
            var messengerRegisterType = typeof(MessengerRegister<TMessengerArgs>);

            lock (this.locker)
            {
                if (!this.registers.ContainsKey(messengerRegisterType))
                {
                    return;
                }

                ((MessengerRegister<TMessengerArgs>)this.registers[messengerRegisterType]).RemoveHandler(handler);
            }
        }

        public void Send<TMessengerArgs>(TMessengerArgs args)
            where TMessengerArgs : MessengerArgs
        {
            var messengerRegisterType = typeof(MessengerRegister<TMessengerArgs>);
            MessengerRegister<TMessengerArgs> messengerRegister;
            lock (this.locker)
            {
                if (!this.registers.ContainsKey(messengerRegisterType))
                {
                    return;
                }

                messengerRegister = (MessengerRegister<TMessengerArgs>)this.registers[messengerRegisterType];
            }

            messengerRegister.CallHandlers(args);
        }
    }
}