using CommunityToolkit.Mvvm.Messaging;
using FindTheBug.Desktop.Reception.Messages;

namespace FindTheBug.Desktop.Reception.Services.CloudSync;

public class SyncState()
{
    SyncStateEnum state = SyncStateEnum.NotStarted;

    internal void CompleteSync()
    {
        state = SyncStateEnum.Success;
        WeakReferenceMessenger.Default.Send(new SyncStatusUpdateMessage(state));
    }

    internal void FailSync()
    {
        state = SyncStateEnum.Fail;
        WeakReferenceMessenger.Default.Send(new SyncStatusUpdateMessage(state));
    }

    internal bool IsSyncing()
    {
        return state == SyncStateEnum.InProgress;
    }

    internal void StartSync()
    {
        state = SyncStateEnum.InProgress;
        WeakReferenceMessenger.Default.Send(new SyncStatusUpdateMessage(state));
    }
}