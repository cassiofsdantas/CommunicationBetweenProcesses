using ProcessesTestRunner.Shared.Managers;
using ProcessesTestRunner.Shared.Models;
using System.IO.MemoryMappedFiles;

namespace MMAP.Infra;

public static class ActionsGenerator
{
    private static MemoryMappedViewAccessor accessor = null!;
    private static Position receivePosition = null!;
    private static Position sendPosition = null!;
    private static readonly byte[] baseData = new byte[CapacityManager.DataSize];

    public static void Setup(MemoryMappedViewAccessor accessor, Position receivePosition, Position sendPosition)
    {
        ActionsGenerator.accessor = accessor;
        ActionsGenerator.receivePosition = receivePosition;
        ActionsGenerator.sendPosition = sendPosition;
    }

    public static Func<TransitionDataModel> GetReceiveAction() => ReceiveAction;

    private static TransitionDataModel ReceiveAction()
    {
        byte[] data = new byte[CapacityManager.DataSize];

        accessor.ReadArray(0, data, 0, data.Length);

        receivePosition.Index += 2;

        ResetData();

        return data;
    }

    public static Func<byte[]> GetReceiveActionCallback() => ReceiveActionCallback;

    private static byte[] ReceiveActionCallback()
    {
        byte[] data = new byte[CapacityManager.DataSize];

        accessor.ReadArray(0, data, 0, data.Length);

        ResetData();

        return data;
    }

    private static void ResetData()
    {
        accessor.WriteArray(0, baseData, 0, baseData.Length);
    }
    public static Action<byte[]> GetSendAction() => SendAction;

    private static void SendAction(byte[] data)
    {
        accessor.WriteArray(0, data, 0, data.Length);
        sendPosition.Index += 2;
    }

    public static Action<byte[]> GetSendActionCallback() => SendActionCallback;

    private static void SendActionCallback(byte[] data)
    {
        accessor.WriteArray(0, data, 0, data.Length);
    }
}
