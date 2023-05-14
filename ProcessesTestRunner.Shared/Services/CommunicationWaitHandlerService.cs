namespace ProcessesTestRunner.Shared.Services;

internal class CommunicationWaitHandlerService
{
    public static readonly EventWaitHandle WaitCanReceiveEventWaitHandle = new(false, EventResetMode.ManualReset, nameof(WaitCanReceiveEventWaitHandle));
    public static readonly EventWaitHandle WaitCanSendEventWaitHandle = new(true, EventResetMode.ManualReset, nameof(WaitCanSendEventWaitHandle));
    public static readonly EventWaitHandle WaitReceiveCallbackEventWaitHandle = new(false, EventResetMode.ManualReset, nameof(WaitReceiveCallbackEventWaitHandle));
}
