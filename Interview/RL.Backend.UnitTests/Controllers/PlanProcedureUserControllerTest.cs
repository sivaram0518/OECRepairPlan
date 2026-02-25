using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Models;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests;

[TestClass]
public class PlanProcedureUserControllerTest
{
    [TestMethod]
    public async Task Get_ReturnsAllPlanProcedureUsers()
    {
        var context = DbContextHelper.CreateContext();

        context.PlanProcedureUsers.Add(new PlanProcedureUser { PlanId = 1, ProcedureId = 1, UserId = 1 });
        context.PlanProcedureUsers.Add(new PlanProcedureUser { PlanId = 2, ProcedureId = 2, UserId = 2 });
        await context.SaveChangesAsync();

        var mediator = new Mock<IMediator>();
        var logger = new Mock<ILogger<RL.Backend.Controllers.PlanProcedureUserController>>();

        var controller = new RL.Backend.Controllers.PlanProcedureUserController(logger.Object, context, mediator.Object);

        var result = controller.Get().ToList();

        result.Should().HaveCount(2);
    }

    [TestMethod]
    public async Task AssignUserToProcedure_ReturnsOk_OnSuccess()
    {
        var context = DbContextHelper.CreateContext();
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<AssignUserToProcedureCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ApiResponse<Unit>.Succeed(new Unit()));

        var logger = new Mock<ILogger<RL.Backend.Controllers.PlanProcedureUserController>>();
        var controller = new RL.Backend.Controllers.PlanProcedureUserController(logger.Object, context, mediator.Object);

        var result = await controller.AssignUserToProcedure(new AssignUserToProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = 1 }, CancellationToken.None);

        result.Should().BeOfType<OkResult>();
    }

    [TestMethod]
    public async Task AssignUserToProcedure_ReturnsBadRequest_OnFailure()
    {
        var context = DbContextHelper.CreateContext();
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<AssignUserToProcedureCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ApiResponse<Unit>.Fail(new Exception("bad")));

        var logger = new Mock<ILogger<RL.Backend.Controllers.PlanProcedureUserController>>();
        var controller = new RL.Backend.Controllers.PlanProcedureUserController(logger.Object, context, mediator.Object);

        var result = await controller.AssignUserToProcedure(new AssignUserToProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = 1 }, CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [TestMethod]
    public async Task RemoveUserFromProcedure_ReturnsOk_OnSuccess()
    {
        var context = DbContextHelper.CreateContext();
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<RemoveUserFromProcedureCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ApiResponse<Unit>.Succeed(new Unit()));

        var logger = new Mock<ILogger<RL.Backend.Controllers.PlanProcedureUserController>>();
        var controller = new RL.Backend.Controllers.PlanProcedureUserController(logger.Object, context, mediator.Object);

        var result = await controller.RemoveUserFromProcedure(new RemoveUserFromProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = 1 }, CancellationToken.None);

        result.Should().BeOfType<OkResult>();
    }

    [TestMethod]
    public async Task RemoveAllUsersFromProcedure_ReturnsOk_OnSuccess()
    {
        var context = DbContextHelper.CreateContext();
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<RemoveAllUsersFromProcedureCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ApiResponse<Unit>.Succeed(new Unit()));

        var logger = new Mock<ILogger<RL.Backend.Controllers.PlanProcedureUserController>>();
        var controller = new RL.Backend.Controllers.PlanProcedureUserController(logger.Object, context, mediator.Object);

        var result = await controller.RemoveAllUsersFromProcedure(new RemoveAllUsersFromProcedureCommand { PlanId = 1, ProcedureId = 1 }, CancellationToken.None);

        result.Should().BeOfType<OkResult>();
    }
}
