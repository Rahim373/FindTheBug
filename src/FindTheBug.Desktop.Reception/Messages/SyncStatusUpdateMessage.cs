using FindTheBug.Desktop.Reception.Services.CloudSync;

namespace FindTheBug.Desktop.Reception.Messages;

public record SyncStatusUpdateMessage(SyncStateEnum state);
