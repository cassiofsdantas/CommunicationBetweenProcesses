using ProcessesTestRunner.Shared.Managers;
using ProcessesTestRunner.Shared.Models;

namespace SharedFile.Infra;

public static class ActionsGenerator
{
    private static FileStream stream = null!;
    private static Position receivePosition = null!;
    private static Position sendPosition = null!;
    private static readonly byte[] baseData = new byte[CapacityManager.DataSize];

    public static void Setup(FileStream stream, Position receivePosition, Position sendPosition)
    {
        ActionsGenerator.stream = stream;
        ActionsGenerator.receivePosition = receivePosition;
        ActionsGenerator.sendPosition = sendPosition;
    }

    public static Func<TransitionDataModel> GetReceiveAction() => ReceiveAction;

    private static TransitionDataModel ReceiveAction()
    {
        byte[] data = new byte[CapacityManager.DataSize];

        stream.Position = 0;
        stream.Read(data, 0, data.Length);

        receivePosition.Index += 2;

        ResetData();

        data = data.Where(x => x != 0).ToArray();

        return data;
    }

    public static Func<byte[]> GetReceiveActionCallback() => ReceiveActionCallback;

    private static byte[] ReceiveActionCallback()
    {
        byte[] data = new byte[CapacityManager.DataSize];

        stream.Position = 0;
        stream.Read(data, 0, data.Length);

        ResetData();

        data = data.Where(x => x != 0).ToArray();

        return data;
    }

    private static void ResetData()
    {
        stream.Write(baseData, 0, baseData.Length);
        stream.SetLength(0);
        stream.Position = 0;
        stream.Flush();
    }
    public static Action<byte[]> GetSendAction() => SendAction;

    private static void SendAction(byte[] data)
    {
        stream.SetLength(0);
        stream.Position = 0;
        stream.Write(data, 0, data.Length);
        sendPosition.Index += 2;
        stream.Flush();
    }

    public static Action<byte[]> GetSendActionCallback() => SendActionCallback;

    private static void SendActionCallback(byte[] data)
    {
        stream.SetLength(0);
        stream.Position = 0;
        stream.Write(data, 0, data.Length);
        stream.Flush();
    }
}
