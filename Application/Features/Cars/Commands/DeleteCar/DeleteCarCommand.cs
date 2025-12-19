using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Commands.DeleteCar;

public class DeleteCarCommand(int id) : IRequest<int>
{
    public int Id { get; } = id;
}
