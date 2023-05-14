using ProcessesTestRunner.Shared.Managers;
using ProcessesTestRunner.Shared.Mock;
using ProcessesTestRunner.Shared.Models;
using ProcessesTestRunner.Shared.Services;

namespace SharedFile.Infra;

public static class ProcessStarter
{
    public static readonly EventWaitHandle WaitCanStartServerEventWaitHandle = new(false, EventResetMode.ManualReset, "SharedFile.Infra" + nameof(WaitCanStartServerEventWaitHandle));
    public static readonly EventWaitHandle WaitCanStartClientEventWaitHandle = new(false, EventResetMode.ManualReset, "SharedFile.Infra" + nameof(WaitCanStartClientEventWaitHandle));

    public static readonly EventWaitHandle WaitSetupServerEventWaitHandle = new(false, EventResetMode.ManualReset, "SharedFile.Infra" + nameof(WaitSetupServerEventWaitHandle));

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
        var path = DirectoryManager.GetFilePath(DirectoryManager.BasePath, FileManager.SharedFileFileName);

        using FileStream stream = new(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        var receivePosition = new Position() { Index = 1 };
        var sendPosition = new Position() { Index = 0 };

        ActionsGenerator.Setup(stream, receivePosition, sendPosition);
        ReceiveService.Setup(true);
        SendService.Setup(true);

        var holdMockData = GetMockData.GetTransitionDataModelGenerated();

        var totalAmount = holdMockData.Count();
        while (receivePosition.Index < totalAmount && sendPosition.Index < totalAmount)
        {
            SendService.SendDataAndWaitCallbackConfirmation(ActionsGenerator.GetSendAction(), ActionsGenerator.GetReceiveActionCallback(), holdMockData.ElementAt(sendPosition.Index), SendProcessWay.Server);
            ReceiveService.ReceiveDataAndSendCallbackConfirmation(ActionsGenerator.GetReceiveAction(), ActionsGenerator.GetSendActionCallback());
        }

        stream.Close();
        stream.Dispose();
    }

    public static void StartClient()
    {
        var path = DirectoryManager.GetFilePath(DirectoryManager.BasePath, FileManager.SharedFileFileName);

        using FileStream stream = new(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        var receivePosition = new Position() { Index = 0 };
        var sendPosition = new Position() { Index = 1 };

        ActionsGenerator.Setup(stream, receivePosition, sendPosition);
        ReceiveService.Setup(true);
        SendService.Setup(true);

        var holdMockData = GetMockData.GetTransitionDataModelGenerated();

        var totalAmount = holdMockData.Count();
        while (receivePosition.Index < totalAmount && sendPosition.Index < totalAmount)
        {
            ReceiveService.ReceiveDataAndSendCallbackConfirmation(ActionsGenerator.GetReceiveAction(), ActionsGenerator.GetSendActionCallback());
            SendService.SendDataAndWaitCallbackConfirmation(ActionsGenerator.GetSendAction(), ActionsGenerator.GetReceiveActionCallback(), holdMockData.ElementAt(sendPosition.Index), SendProcessWay.Client);
        }

        stream.Close();
        stream.Dispose();
    }
}
