using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interfaces;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace TestyClient
{
    public partial class TestyClient : Form
    {
        public TestyClient()
        {
            InitializeComponent();
        }

        private async void btnRunTest_Click(object sender, EventArgs e)
        {
            var serviceUri = new Uri(@"fabric:/PartitionTest/StatefulTest");

            // Base the partition key off one of the authentication parameters, such as the 
            // *last* digit in AccountId, AcceptorId, or Account Token.  Just fake it for the test.
            ServicePartitionKey pKey = new ServicePartitionKey((long)keySelector.Value);

            try
            {
                // Resolve an instance of our stateful service on our requested partition and make the call.
                IStatefulTest1 testStateful = ServiceProxy.Create<IStatefulTest1>(serviceUri, pKey);
                richTextBox1.Text = await testStateful.TestEntryPoint();
            }
            catch (System.Fabric.FabricException ex)
            {
                richTextBox1.Text = @"Error: " + ex.Message;
            }
        }
    }
}
