using System;

namespace Greenergy.Messaging
{
    public interface IEmissionsConsumer
    {
        void Listen(Action<string> message);
    }
}
