using ProcessesTestRunner.Shared.Identifiers;
using ProcessesTestRunner.Shared.Managers;
using ProcessesTestRunner.Shared.Mock;
using ProcessesTestRunner.Shared.Models;
using ProcessesTestRunner.Shared.Services;
using System.IO.MemoryMappedFiles;

namespace MMAP.Infra;

public static class ProcessStarter
{
    public static readonly EventWaitHandle WaitCanStartServerEventWaitHandle = new(false, EventResetMode.ManualReset, "MMAP.Infra" + nameof(WaitCanStartServerEventWaitHandle));
    public static readonly EventWaitHandle WaitCanStartClientEventWaitHandle = new(false, EventResetMode.ManualReset, "MMAP.Infra" + nameof(WaitCanStartClientEventWaitHandle));

    public static readonly EventWaitHandle WaitSetupServerEventWaitHandle = new(false, EventResetMode.ManualReset, "MMAP.Infra" + nameof(WaitSetupServerEventWaitHandle));

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
        using var mmf = MemoryMappedFile.CreateFromFile(
        DirectoryManager.GetFilePath(DirectoryManager.BasePath, FileManager.MMAP_FileName),
        FileMode.OpenOrCreate,
        MemoryMappedIdentifiers.MMAPSpaceIdentifier,
        CapacityManager.DataSize);

        // Criar uma visão de acesso ao arquivo
        using var accessor = mmf.CreateViewAccessor();

        WaitSetupServerEventWaitHandle.Set();

        var receivePosition = new Position() { Index = 1 };
        var sendPosition = new Position() { Index = 0 };

        ActionsGenerator.Setup(accessor, receivePosition, sendPosition);
        ReceiveService.Setup(true);
        SendService.Setup(true);

        var holdMockData = GetMockData.GetTransitionDataModelGenerated();

        var totalAmount = holdMockData.Count();
        while (receivePosition.Index < totalAmount && sendPosition.Index < totalAmount)
        {
            SendService.SendDataAndWaitCallbackConfirmation(ActionsGenerator.GetSendAction(), ActionsGenerator.GetReceiveActionCallback(), holdMockData.ElementAt(sendPosition.Index), SendProcessWay.Server);
            ReceiveService.ReceiveDataAndSendCallbackConfirmation(ActionsGenerator.GetReceiveAction(), ActionsGenerator.GetSendActionCallback());
        }
    }

    public static void StartClient()
    {
        WaitSetupServerEventWaitHandle.WaitOne();
        WaitSetupServerEventWaitHandle.Reset();

        using MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(MemoryMappedIdentifiers.MMAPSpaceIdentifier);

        // Criar uma visão de acesso ao arquivo
        using var accessor = mmf.CreateViewAccessor();

        var receivePosition = new Position() { Index = 0 };
        var sendPosition = new Position() { Index = 1 };

        ActionsGenerator.Setup(accessor, receivePosition, sendPosition);
        ReceiveService.Setup(true);
        SendService.Setup(true);

        var holdMockData = GetMockData.GetTransitionDataModelGenerated();

        var totalAmount = holdMockData.Count();
        while (receivePosition.Index < totalAmount && sendPosition.Index < totalAmount)
        {
            ReceiveService.ReceiveDataAndSendCallbackConfirmation(ActionsGenerator.GetReceiveAction(), ActionsGenerator.GetSendActionCallback());
            SendService.SendDataAndWaitCallbackConfirmation(ActionsGenerator.GetSendAction(), ActionsGenerator.GetReceiveActionCallback(), holdMockData.ElementAt(sendPosition.Index), SendProcessWay.Client);
        }
    }
}
