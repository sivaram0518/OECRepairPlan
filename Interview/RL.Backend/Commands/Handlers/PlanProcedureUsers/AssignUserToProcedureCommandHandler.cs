using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers;

public class AssignUserToProcedureCommandHandler : IRequestHandler<AssignUserToProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public AssignUserToProcedureCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(AssignUserToProcedureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validation = ValidateRequest(request);
            if (validation is not null)
                return validation;

            var plan = await _context.Plans.FirstOrDefaultAsync(p => p.PlanId == request.PlanId);
            var procedure = await _context.Procedures.FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);

            var entityValidation = EnsureEntitiesExist(plan, procedure, user, request);
            if (entityValidation is not null)
                return entityValidation;

            // Verify that the procedure is part of the plan
            var planProcedure = await _context.PlanProcedures
                .FirstOrDefaultAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId);
            
            if (planProcedure is null)
                return ApiResponse<Unit>.Fail(new BadRequestException("Procedure is not part of this plan"));

            // Check if user already assigned to this procedure in this plan
            var existingAssignment = await _context.PlanProcedureUsers
                .FirstOrDefaultAsync(ppu => ppu.PlanId == request.PlanId && 
                                           ppu.ProcedureId == request.ProcedureId && 
                                           ppu.UserId == request.UserId);

            if (existingAssignment is not null)
                return ApiResponse<Unit>.Succeed(new Unit());

            var planProcedureUser = new PlanProcedureUser
            {
                PlanId = request.PlanId,
                ProcedureId = request.ProcedureId,
                UserId = request.UserId
            };

            _context.PlanProcedureUsers.Add(planProcedureUser);
            await _context.SaveChangesAsync();

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }

    private ApiResponse<Unit>? ValidateRequest(AssignUserToProcedureCommand request)
    {
        if (request.PlanId < 1)
            return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
        if (request.ProcedureId < 1)
            return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
        if (request.UserId < 1)
            return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));

        return null;
    }

    private ApiResponse<Unit>? EnsureEntitiesExist(Plan? plan, Procedure? procedure, User? user, AssignUserToProcedureCommand request)
    {
        if (plan is null)
            return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));
        if (procedure is null)
            return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));
        if (user is null)
            return ApiResponse<Unit>.Fail(new NotFoundException($"UserId: {request.UserId} not found"));

        return null;
    }
}
