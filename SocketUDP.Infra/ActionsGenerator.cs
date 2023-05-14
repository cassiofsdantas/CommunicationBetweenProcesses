using ProcessesTestRunner.Shared.Managers;
using ProcessesTestRunner.Shared.Models;
using System.Net;
using System.Net.Sockets;

namespace SocketUDP.Infra;

public static class ActionsGenerator
{
    private static Position receivePosition = null!;
    private static EndPoint serverEndPoint = null!;
    private static Position sendPosition = null!;
    private static Socket socket = null!;

    public static void Setup(Socket socket, EndPoint serverEndPoint, Position receivePosition, Position sendPosition)
    {
        ActionsGenerator.socket = socket;
        ActionsGenerator.serverEndPoint = serverEndPoint;
        ActionsGenerator.receivePosition = receivePosition;
        ActionsGenerator.sendPosition = sendPosition;
    }

    public static Func<TransitionDataModel> GetReceiveAction() => ReceiveAction;

    private static TransitionDataModel ReceiveAction()
    {
        byte[] data = new byte[CapacityManager.DataSize];

        socket.ReceiveFrom(data, ref serverEndPoint);
        receivePosition.Index += 2;

        return data;
    }

    public static Func<byte[]> GetReceiveActionCallback() => ReceiveActionCallback;

    private static byte[] ReceiveActionCallback()
    {
        byte[] data = new byte[CapacityManager.DataSize];

        socket.ReceiveFrom(data, ref serverEndPoint);

        return data;
    }

    public static Action<byte[]> GetSendAction() => SendAction;

    private static void SendAction(byte[] data)
    {
        socket.SendTo(data, serverEndPoint);
        sendPosition.Index += 2;
    }

    public static Action<byte[]> GetSendActionCallback() => SendActionCallback;

    private static void SendActionCallback(byte[] data)
    {
        socket.SendTo(data, serverEndPoint);
    }
}
