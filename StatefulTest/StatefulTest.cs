using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using StatefulService = Microsoft.ServiceFabric.Services.Runtime.StatefulService;

namespace StatefulTest
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class StatefulTest : StatefulService, IStatefulTest1
    {
        public StatefulTest(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see http://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(this.CreateServiceRemotingListener, "rpcStatefulTestPrimaryEndpoint", false)
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(this, "-StatefulTest- Started on Node: {0} using Low Partition Key: {1}", this.Context.NodeContext.NodeName, GetCurrentPartitionLowKey().Result);
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }

        public Task<string> TestEntryPoint()
        {
            var msg = string.Format(
                    "TestEntryPoint():\n NodeName: {0}\n PartitionLowKey: {1}\n NodeInstanceId: {2}\n ReplicaId: {3}\n",
                    this.Context.NodeContext.NodeName,
                    GetCurrentPartitionLowKey().Result,
                    this.Context.NodeContext.NodeInstanceId,
                    this.Context.ReplicaId);

            ServiceEventSource.Current.ServiceMessage(this, msg.Replace("\n", " -"));
            return Task.FromResult(msg);
        }


        /// <summary>
        /// Retrieve current partition information based on our Context.PartitionId.  Returns the Low Key for our partition.
        /// </summary>
        /// <returns></returns>
        private async Task<long> GetCurrentPartitionLowKey()
        {
            var serviceUri = new Uri(@"fabric:/PartitionTest/StatefulTest");
            ServicePartitionList partitionList = await new FabricClient().QueryManager.GetPartitionListAsync(serviceUri);

            Partition currentPartition = partitionList.Single(part => part.PartitionInformation.Id == this.Context.PartitionId);
            Debug.Assert(currentPartition.PartitionInformation.Kind == ServicePartitionKind.Int64Range);
            return ((Int64RangePartitionInformation)currentPartition.PartitionInformation).LowKey;    // lame up-cast required.
        }
    }
}
