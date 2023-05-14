using ProcessesTestRunner.Shared.Models;

namespace ProcessesTestRunner.Shared.Services;

public static class ReceiveService
{
    private static bool useContextSincronization;
    public static void Setup(bool useContextSincronization)
    {
        ReceiveService.useContextSincronization = useContextSincronization;
    }


    private static void ReleaseCanSendData()
    {
        CommunicationWaitHandlerService.WaitCanSendEventWaitHandle.Set();
    }

    private static void WaitCanReceiveData()
    {
        if (!useContextSincronization)
            return;

        CommunicationWaitHandlerService.WaitCanReceiveEventWaitHandle.WaitOne();
        CommunicationWaitHandlerService.WaitCanReceiveEventWaitHandle.Reset();
    }

    private static void ReleaseWaitReceiveCallback()
    {
        CommunicationWaitHandlerService.WaitReceiveCallbackEventWaitHandle.Set();
    }


    public static byte[] ReceiveData(Func<byte[]> receiveAction)
    {
        WaitCanReceiveData();

        var result = receiveAction.Invoke();

#if DEBUG
        var sendProcessWayCallback = ((TransitionDataModel)result).SendProcessWay is SendProcessWay.Server ? SendProcessWay.Client : SendProcessWay.Server;
        Console.WriteLine($"{nameof(ReceiveData)};{sendProcessWayCallback};{result};");
#endif

        ReleaseCanSendData();

        return result;
    }

    public static TransitionDataModel ReceiveDataAndSendCallbackConfirmation(Func<TransitionDataModel> receiveAction, Action<byte[]> sendAction)
    {
        WaitCanReceiveData();

        var result = receiveAction.Invoke();

        var sendProcessWayCallback = result.SendProcessWay is SendProcessWay.Server ? SendProcessWay.Client : SendProcessWay.Server;

#if DEBUG
        Console.WriteLine($"{nameof(ReceiveDataAndSendCallbackConfirmation)};{sendProcessWayCallback};{result};");
#endif

        ReleaseCanSendData();

        SendService.SendData(sendAction, result, sendProcessWayCallback);

        ReleaseWaitReceiveCallback();

        return result;
    }
}
