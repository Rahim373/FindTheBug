using FindTheBug.Domain.Entities;

namespace FindTheBug.Desktop.Reception.Messages;

public record UserLoggedInMessage(User? user);
