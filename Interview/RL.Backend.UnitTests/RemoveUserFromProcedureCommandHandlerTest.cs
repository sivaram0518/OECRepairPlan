using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.PlanProcedureUsers;
using RL.Backend.Exceptions;
using RL.Data;

namespace RL.Backend.UnitTests;

[TestClass]
public class RemoveUserFromProcedureCommandHandlerTest
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task InvalidPlanId_ReturnsBadRequest(int planId)
    {
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveUserFromProcedureCommandHandler(context);
        var request = new RemoveUserFromProcedureCommand { PlanId = planId, ProcedureId = 1, UserId = 1 };

        var result = await sut.Handle(request, new CancellationToken());

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveUserFromProcedureCommandHandler(context);
        var request = new RemoveUserFromProcedureCommand { PlanId = 1, ProcedureId = procedureId, UserId = 1 };

        var result = await sut.Handle(request, new CancellationToken());

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task InvalidUserId_ReturnsBadRequest(int userId)
    {
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveUserFromProcedureCommandHandler(context);
        var request = new RemoveUserFromProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = userId };

        var result = await sut.Handle(request, new CancellationToken());

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task NotAssigned_ReturnsSuccess()
    {
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveUserFromProcedureCommandHandler(context);
        var request = new RemoveUserFromProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = 1 };

        // No assignment added -> should be idempotent success
        var result = await sut.Handle(request, new CancellationToken());

        result.Value.Should().BeOfType<MediatR.Unit>();
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    public async Task RemovesAssignment_ReturnsSuccess()
    {
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveUserFromProcedureCommandHandler(context);
        var request = new RemoveUserFromProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = 1 };

        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser { PlanId = 1, ProcedureId = 1, UserId = 1 });
        await context.SaveChangesAsync();

        var pre = await context.PlanProcedureUsers.CountAsync(ppu => ppu.PlanId == 1 && ppu.ProcedureId == 1 && ppu.UserId == 1);
        pre.Should().Be(1);

        var result = await sut.Handle(request, new CancellationToken());

        result.Value.Should().BeOfType<MediatR.Unit>();
        result.Succeeded.Should().BeTrue();

        var post = await context.PlanProcedureUsers.CountAsync(ppu => ppu.PlanId == 1 && ppu.ProcedureId == 1 && ppu.UserId == 1);
        post.Should().Be(0);
    }
}
