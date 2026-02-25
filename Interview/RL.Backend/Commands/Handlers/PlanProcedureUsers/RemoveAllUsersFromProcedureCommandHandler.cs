using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers;

public class RemoveAllUsersFromProcedureCommandHandler : IRequestHandler<RemoveAllUsersFromProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public RemoveAllUsersFromProcedureCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(RemoveAllUsersFromProcedureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

            var assignments = await _context.PlanProcedureUsers
                .Where(ppu => ppu.PlanId == request.PlanId && ppu.ProcedureId == request.ProcedureId)
                .ToListAsync();

            if (assignments.Any())
            {
                _context.PlanProcedureUsers.RemoveRange(assignments);
                await _context.SaveChangesAsync();
            }

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}
