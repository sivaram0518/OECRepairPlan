using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RL.Backend.Commands;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class PlanProcedureUserController : ControllerBase
{
    private readonly ILogger<PlanProcedureUserController> _logger;
    private readonly RLContext _context;
    private readonly IMediator _mediator;

    public PlanProcedureUserController(ILogger<PlanProcedureUserController> logger, RLContext context, IMediator mediator)
    {
        _logger = logger;
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    [EnableQuery]
    public IEnumerable<PlanProcedureUser> Get()
    {
            return _context.PlanProcedureUsers;
    }

    [HttpPost("AssignUserToProcedure")]
    public async Task<IActionResult> AssignUserToProcedure(AssignUserToProcedureCommand command, CancellationToken token)
    {
        var response = await _mediator.Send(command, token);

        return response.ToActionResult();
    }

    [HttpPost("RemoveUserFromProcedure")]
    public async Task<IActionResult> RemoveUserFromProcedure(RemoveUserFromProcedureCommand command, CancellationToken token)
    {
        var response = await _mediator.Send(command, token);

        return response.ToActionResult();
    }

    [HttpPost("RemoveAllUsersFromProcedure")]
    public async Task<IActionResult> RemoveAllUsersFromProcedure(RemoveAllUsersFromProcedureCommand command, CancellationToken token)
    {
        var response = await _mediator.Send(command, token);

        return response.ToActionResult();
    }
}
