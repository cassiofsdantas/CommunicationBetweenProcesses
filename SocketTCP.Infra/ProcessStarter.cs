using ProcessesTestRunner.Shared.Identifiers;
using ProcessesTestRunner.Shared.Mock;
using ProcessesTestRunner.Shared.Models;
using ProcessesTestRunner.Shared.Services;
using System.Net;
using System.Net.Sockets;

namespace SocketTCP.Infra;

public static class ProcessStarter
{
    public static readonly EventWaitHandle WaitCanStartServerEventWaitHandle = new(false, EventResetMode.ManualReset, "SocketTCP.Infra" + nameof(WaitCanStartServerEventWaitHandle));
    public static readonly EventWaitHandle WaitCanStartClientEventWaitHandle = new(false, EventResetMode.ManualReset, "SocketTCP.Infra" + nameof(WaitCanStartClientEventWaitHandle));

    public static readonly EventWaitHandle WaitSetupServerEventWaitHandle = new(false, EventResetMode.ManualReset, "SocketTCP.Infra" + nameof(WaitSetupServerEventWaitHandle));
    public static readonly EventWaitHandle WaitClientServerDisconnectEventWaitHandle = new(false, EventResetMode.ManualReset, "SocketTCP.Infra" + nameof(WaitClientServerDisconnectEventWaitHandle));

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
    }

    public static void StartServer()
    {
        IPAddress ipAddress = MemoryMappedIdentifiers.SocketIp;
        int port = MemoryMappedIdentifiers.SocketPort;

        // Cria um socket TCP e o associa ao IP e porta definidos
        TcpListener serverSocket = new(ipAddress, port);
        serverSocket.Start();

        WaitSetupServerEventWaitHandle.Set();

        using TcpClient clientSocket = serverSocket.AcceptTcpClient();

        var receivePosition = new Position() { Index = 1 };
        var sendPosition = new Position() { Index = 0 };

        ActionsGenerator.Setup(clientSocket, receivePosition, sendPosition);
        ReceiveService.Setup(false);
        SendService.Setup(true);

        var holdMockData = GetMockData.GetTransitionDataModelGenerated();

        var totalAmount = holdMockData.Count();
        while (receivePosition.Index < totalAmount && sendPosition.Index < totalAmount)
        {
            SendService.SendDataAndWaitCallbackConfirmation(ActionsGenerator.GetSendAction(), ActionsGenerator.GetReceiveActionCallback(), holdMockData.ElementAt(sendPosition.Index), SendProcessWay.Server);
            ReceiveService.ReceiveDataAndSendCallbackConfirmation(ActionsGenerator.GetReceiveAction(), ActionsGenerator.GetSendActionCallback());
        }

        WaitClientServerDisconnectEventWaitHandle.WaitOne();
        WaitClientServerDisconnectEventWaitHandle.Reset();

        clientSocket.Dispose();

        serverSocket.Stop();
        serverSocket.Server.Dispose();
    }

    public static void StartClient()
    {
        WaitSetupServerEventWaitHandle.WaitOne();
        WaitSetupServerEventWaitHandle.Reset();

        IPAddress serverIpAddress = MemoryMappedIdentifiers.SocketIp;
        int serverPort = MemoryMappedIdentifiers.SocketPort;

        // Cria um socket TCP e se conecta ao servidor
        using TcpClient clientSocket = new();

        clientSocket.Connect(serverIpAddress, serverPort);

        var receivePosition = new Position() { Index = 0 };
        var sendPosition = new Position() { Index = 1 };

        ActionsGenerator.Setup(clientSocket, receivePosition, sendPosition);
        ReceiveService.Setup(false);
        SendService.Setup(true);

        var holdMockData = GetMockData.GetTransitionDataModelGenerated();

        var totalAmount = holdMockData.Count();
        while (receivePosition.Index < totalAmount && sendPosition.Index < totalAmount)
        {
            ReceiveService.ReceiveDataAndSendCallbackConfirmation(ActionsGenerator.GetReceiveAction(), ActionsGenerator.GetSendActionCallback());
            SendService.SendDataAndWaitCallbackConfirmation(ActionsGenerator.GetSendAction(), ActionsGenerator.GetReceiveActionCallback(), holdMockData.ElementAt(sendPosition.Index), SendProcessWay.Client);
        }

        clientSocket.Close();
        clientSocket.Dispose();
        WaitClientServerDisconnectEventWaitHandle.Set();
    }
}
