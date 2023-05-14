using ProcessesTestRunner.Shared.Managers;
using ProcessesTestRunner.Shared.Models;
using System.Net.Sockets;

namespace SocketTCP.Infra;

public static class ActionsGenerator
{
    private static Position receivePosition = null!;
    private static Position sendPosition = null!;
    private static NetworkStream stream = null!;

    public static void Setup(TcpClient tcpClient, Position receivePosition, Position sendPosition)
    {
        stream = tcpClient.GetStream();
        ActionsGenerator.receivePosition = receivePosition;
        ActionsGenerator.sendPosition = sendPosition;
    }

    public static Func<TransitionDataModel> GetReceiveAction() => ReceiveAction;

    private static TransitionDataModel ReceiveAction()
    {
        byte[] data = new byte[CapacityManager.DataSize];

        stream.Read(data, 0, data.Length);

        receivePosition.Index += 2;

        return data;
    }

    public static Func<byte[]> GetReceiveActionCallback() => ReceiveActionCallback;

    private static byte[] ReceiveActionCallback()
    {
        byte[] data = new byte[CapacityManager.DataSize];

        stream.Read(data, 0, data.Length);

        return data;
    }

    public static Action<byte[]> GetSendAction() => SendAction;

    private static void SendAction(byte[] data)
    {
        stream.Write(data, 0, data.Length);
        sendPosition.Index += 2;
    }

    public static Action<byte[]> GetSendActionCallback() => SendActionCallback;

    private static void SendActionCallback(byte[] data)
    {
        stream.Write(data, 0, data.Length);
    }
}
