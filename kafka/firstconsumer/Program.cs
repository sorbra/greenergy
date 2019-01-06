using System;
using System.Threading;
using Greenergy.Messaging;

namespace firstclient
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };
            new EmissionsConsumer().Consume(cts.Token);
        }
    }
}