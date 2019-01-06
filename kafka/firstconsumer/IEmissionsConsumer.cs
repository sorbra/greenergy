using System;
using System.Threading;

namespace Greenergy.Messaging
{
    public interface IEmissionsConsumer
    {
        void Consume(CancellationToken cts);
    }
}
