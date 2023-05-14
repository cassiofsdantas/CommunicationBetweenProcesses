using ProcessesTestRunner.Shared.Identifiers;
using ProcessesTestRunner.Shared.Managers;
using ProcessesTestRunner.Shared.Mock;
using ProcessesTestRunner.Shared.Models;
using ProcessesTestRunner.Shared.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketUDP.Infra;

public static class ProcessStarter
{
    public static readonly EventWaitHandle WaitCanStartServerEventWaitHandle = new(false, EventResetMode.ManualReset, "SocketUDP.Infra" + nameof(WaitCanStartServerEventWaitHandle));
    public static readonly EventWaitHandle WaitCanStartClientEventWaitHandle = new(false, EventResetMode.ManualReset, "SocketUDP.Infra" + nameof(WaitCanStartClientEventWaitHandle));

    public static readonly EventWaitHandle WaitSetupServerEventWaitHandle = new(false, EventResetMode.ManualReset, "SocketUDP.Infra" + nameof(WaitSetupServerEventWaitHandle));

    public static void WaitCanStart(bool isServer)
    {
        if (isServer)
        {
            WaitCanStartServerEventWaitHandle.Set();
        }
        else
        {
            WaitCanStartClientEventWaitHandle.Set();
        }

        WaitHandle.WaitAll(new WaitHandle[] { WaitCanStartServerEventWaitHandle, WaitCanStartClientEventWaitHandle });
        Task.Delay(100).ContinueWith(t =>
        {
            WaitCanStartServerEventWaitHandle.Reset();
            WaitCanStartClientEventWaitHandle.Reset();
        });
    }

    public static void StartServer()
    {
        static void WaitClientHandshake(Socket socket, ref EndPoint endPoint)
        {
            byte[] buffer = new byte[CapacityManager.DataSize];
            socket.ReceiveFrom(buffer, ref endPoint);
        }

        static void SendHandshake(Socket socket, EndPoint endPoint)
        {
            byte[] message = Encoding.UTF8.GetBytes("Handshake from server!");
            socket.SendTo(message, endPoint);
        }

        IPAddress ipAddress = MemoryMappedIdentifiers.SocketIp;
        int port = MemoryMappedIdentifiers.SocketPort;

        using Socket serverSocket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        serverSocket.Bind(new IPEndPoint(ipAddress, port));

        EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

        WaitSetupServerEventWaitHandle.Set();

        WaitClientHandshake(serverSocket, ref clientEndPoint);

        SendHandshake(serverSocket, clientEndPoint);

        var receivePosition = new Position() { Index = 1 };
        var sendPosition = new Position() { Index = 0 };

        ActionsGenerator.Setup(serverSocket, clientEndPoint, receivePosition, sendPosition);
        ReceiveService.Setup(false);
        SendService.Setup(true);

        var holdMockData = GetMockData.GetTransitionDataModelGenerated();

        var totalAmount = holdMockData.Count();
        while (receivePosition.Index < totalAmount && sendPosition.Index < totalAmount)
        {
            SendService.SendDataAndWaitCallbackConfirmation(ActionsGenerator.GetSendAction(), ActionsGenerator.GetReceiveActionCallback(), holdMockData.ElementAt(sendPosition.Index), SendProcessWay.Server);
            ReceiveService.ReceiveDataAndSendCallbackConfirmation(ActionsGenerator.GetReceiveAction(), ActionsGenerator.GetSendActionCallback());
        }

        serverSocket.Dispose();
    }

    public static void StartClient()
    {
        static void WaitServerHandshake(Socket clientSocket, EndPoint serverEndPoint)
        {
            byte[] buffer = new byte[CapacityManager.DataSize];
            clientSocket.ReceiveFrom(buffer, ref serverEndPoint);
        }

        static void SendHandshake(Socket clientSocket, EndPoint serverEndPoint)
        {
            byte[] message = Encoding.UTF8.GetBytes("Handshake from client!");
            clientSocket.SendTo(message, serverEndPoint);
        }

        IPAddress serverIpAddress = MemoryMappedIdentifiers.SocketIp;
        int serverPort = MemoryMappedIdentifiers.SocketPort;

        WaitSetupServerEventWaitHandle.WaitOne();
        WaitSetupServerEventWaitHandle.Reset();

        using Socket clientSocket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        EndPoint serverEndPoint = new IPEndPoint(serverIpAddress, serverPort);

        SendHandshake(clientSocket, serverEndPoint);

        WaitServerHandshake(clientSocket, serverEndPoint);

        var receivePosition = new Position() { Index = 0 };
        var sendPosition = new Position() { Index = 1 };

        ActionsGenerator.Setup(clientSocket, serverEndPoint, receivePosition, sendPosition);
        ReceiveService.Setup(false);
        SendService.Setup(true);

        var holdMockData = GetMockData.GetTransitionDataModelGenerated();

        var totalAmount = holdMockData.Count();
        while (receivePosition.Index < totalAmount && sendPosition.Index < totalAmount)
        {
            ReceiveService.ReceiveDataAndSendCallbackConfirmation(ActionsGenerator.GetReceiveAction(), ActionsGenerator.GetSendActionCallback());
            SendService.SendDataAndWaitCallbackConfirmation(ActionsGenerator.GetSendAction(), ActionsGenerator.GetReceiveActionCallback(), holdMockData.ElementAt(sendPosition.Index), SendProcessWay.Client);
        }

        clientSocket.Dispose();
    }
}
