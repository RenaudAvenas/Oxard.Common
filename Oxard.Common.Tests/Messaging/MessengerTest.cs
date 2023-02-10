using Oxard.Common.Messaging;
using NFluent;
using NUnit.Framework;
using System;

namespace Oxard.Common.Tests.Messaging
{
    [TestFixture]
    public class MessengerTest
    {
        [Test]
        public void WhenSomeoneSendMessageThenRegisteredHandlerAreCalledWithMessengerArgs()
        {
            var messenger = new Messenger();
            bool firstLambdaPassed = false;
            bool secondLambdaPassed = false;

            var args = new MessengerArgsFake();

            messenger.Register<MessengerArgsFake>(t => firstLambdaPassed = ReferenceEquals(args, t), this);
            messenger.Register<MessengerArgsFake>(t => secondLambdaPassed = ReferenceEquals(args, t), this);

            messenger.Send(args);

            Check.That(firstLambdaPassed).IsTrue();
            Check.That(secondLambdaPassed).IsTrue();
        }

        [Test]
        public void WhenSomeoneSendMessageWithNoRecieverThenCodeDoesNotThrowsException()
        {
            var messenger = new Messenger();
            Check.ThatCode(() => messenger.Send(new MessengerArgsFake())).DoesNotThrow();
        }

        [Test]
        public void WhenDirectlyCallMessengerRegisterCallMethodThenCodeDoesNotThrowsException()
        {
            var messengerRegister = new MessengerRegister<MessengerArgsFake>();
            Check.ThatCode(() => messengerRegister.CallHandlers(new MessengerArgsFake())).DoesNotThrow();
        }

        [Test]
        public void WhenRegisterAndUnregisterThenNoEventRecieved()
        {
            var messenger = new Messenger();

            int eventRaisedCount = 0;

            var regsiteredMethod = new Action<MessengerArgsFake>(m => eventRaisedCount++);

            messenger.Register(regsiteredMethod, this);

            messenger.Send(new MessengerArgsFake());

            messenger.Unregister(regsiteredMethod);

            messenger.Send(new MessengerArgsFake());

            Check.That(eventRaisedCount).IsEqualTo(1);
        }
    }
}