using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers;

public class RemoveUserFromProcedureCommandHandler : IRequestHandler<RemoveUserFromProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public RemoveUserFromProcedureCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(RemoveUserFromProcedureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validation = ValidateRequest(request);
            if (validation is not null)
                return validation;

            var assignment = await _context.PlanProcedureUsers
                .FirstOrDefaultAsync(ppu => ppu.PlanId == request.PlanId && 
                                           ppu.ProcedureId == request.ProcedureId && 
                                           ppu.UserId == request.UserId);

            if (assignment is null)
                return ApiResponse<Unit>.Succeed(new Unit());

            _context.PlanProcedureUsers.Remove(assignment);
            await _context.SaveChangesAsync();

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }

    private ApiResponse<Unit>? ValidateRequest(RemoveUserFromProcedureCommand request)
    {
        if (request.PlanId < 1)
            return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
        if (request.ProcedureId < 1)
            return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
        if (request.UserId < 1)
            return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));

        return null;
    }
}
