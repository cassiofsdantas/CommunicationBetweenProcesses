using ProcessesTestRunner.Shared.Models;

namespace ProcessesTestRunner.Shared.Services;

public static class SendService
{
    private static bool useContextSincronization;
    public static void Setup(bool useContextSincronization)
    {
        SendService.useContextSincronization = useContextSincronization;
    }

    private static void WaitReceiveCallback()
    {
        if (!useContextSincronization)
            return;

        CommunicationWaitHandlerService.WaitReceiveCallbackEventWaitHandle.WaitOne();
        CommunicationWaitHandlerService.WaitReceiveCallbackEventWaitHandle.Reset();
    }

    private static void WaitCanSendData()
    {
        if (!useContextSincronization)
            return;

        CommunicationWaitHandlerService.WaitCanSendEventWaitHandle.WaitOne();
        CommunicationWaitHandlerService.WaitCanSendEventWaitHandle.Reset();
    }

    private static void ReleaseCanReceiveData()
    {
        CommunicationWaitHandlerService.WaitCanReceiveEventWaitHandle.Set();
    }


    public static void SendData(Action<byte[]> sendAction, TransitionDataModel data, SendProcessWay sendProcessWay)
    {
        data = data with { SendProcessWay = sendProcessWay };
        SendData(sendAction, data);
    }

    public static void SendData(Action<byte[]> sendAction, byte[] data)
    {
        WaitCanSendData();

        sendAction.Invoke(data);

        ReleaseCanReceiveData();
    }

    public static void SendDataAndWaitCallbackConfirmation(Action<byte[]> sendAction, Func<byte[]> receiveAction, TransitionDataModel data, SendProcessWay sendProcessWay)
    {
        SendData(sendAction, data, sendProcessWay);

        WaitReceiveCallback();

        var result = ReceiveService.ReceiveData(receiveAction);

        var sendProcessWayCallback = sendProcessWay is SendProcessWay.Server ? SendProcessWay.Client : SendProcessWay.Server;

        if (!data.Compare(result) || ((TransitionDataModel)result).SendProcessWay != sendProcessWayCallback)
        {
            throw new InvalidDataException("Invalid data returned.");
        }
    }
}
