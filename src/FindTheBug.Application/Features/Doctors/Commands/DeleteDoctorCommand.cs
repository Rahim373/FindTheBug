using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Doctors.Commands;

public record DeleteDoctorCommand(Guid Id) : ICommand<bool>;
