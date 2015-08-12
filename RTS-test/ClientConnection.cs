using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;

namespace RTS_test
{
    class ClientConnection
    {
        NetPeerConfiguration config = new NetPeerConfiguration("RTS-test");
        NetClient client;
        NetConnection connection;

        private Thread receiveThread;

        public ClientConnection()
        {
            client = new NetClient(config);
        }

        public void connect(string ip, int port)
        {
            connection = client.Connect(ip, port);

            if(connection.Status == NetConnectionStatus.Connected)
            {
                receiveThread = new Thread(new ThreadStart(receive));
                receiveThread.Start();
            }
            
        }

        public NetSendResult send(byte[] data)
        {
            NetOutgoingMessage msg = client.CreateMessage();
            return connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void receive()
        {
            NetBuffer buffer = new NetBuffer();

            bool keepGoing = true;
            while (keepGoing)
            {
                NetIncomingMessage message;
                while ((message = client.ReadMessage()) != null)
                {
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                            Console.WriteLine(buffer.ReadString());
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            Console.WriteLine("New status: " + client.Status + " (Reason: " + buffer.ReadString() + ")");
                            if(client.Status != NetPeerStatus.Running)
                            {
                                keepGoing = false;
                                break;
                            }
                            break;

                        case NetIncomingMessageType.Data:
                            // Handle data in buffer here
                            break;
                    }
                }
            }
        }
    }
}
