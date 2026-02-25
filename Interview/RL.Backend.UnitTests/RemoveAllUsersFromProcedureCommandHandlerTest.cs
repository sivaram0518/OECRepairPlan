using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.PlanProcedureUsers;
using RL.Backend.Exceptions;

namespace RL.Backend.UnitTests;

[TestClass]
public class RemoveAllUsersFromProcedureCommandHandlerTest
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task InvalidPlanId_ReturnsBadRequest(int planId)
    {
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveAllUsersFromProcedureCommandHandler(context);
        var request = new RemoveAllUsersFromProcedureCommand { PlanId = planId, ProcedureId = 1 };

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
        var sut = new RemoveAllUsersFromProcedureCommandHandler(context);
        var request = new RemoveAllUsersFromProcedureCommand { PlanId = 1, ProcedureId = procedureId };

        var result = await sut.Handle(request, new CancellationToken());

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task NoAssignments_ReturnsSuccess()
    {
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveAllUsersFromProcedureCommandHandler(context);
        var request = new RemoveAllUsersFromProcedureCommand { PlanId = 1, ProcedureId = 1 };

        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "T" });
        await context.SaveChangesAsync();

        var result = await sut.Handle(request, new CancellationToken());

        result.Value.Should().BeOfType<MediatR.Unit>();
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    public async Task RemovesAssignments_ReturnsSuccess()
    {
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveAllUsersFromProcedureCommandHandler(context);
        var request = new RemoveAllUsersFromProcedureCommand { PlanId = 1, ProcedureId = 1 };

        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "T" });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser { PlanId = 1, ProcedureId = 1, UserId = 1 });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser { PlanId = 1, ProcedureId = 1, UserId = 2 });
        await context.SaveChangesAsync();

        var pre = await context.PlanProcedureUsers.CountAsync(ppu => ppu.PlanId == 1 && ppu.ProcedureId == 1);
        pre.Should().Be(2);

        var result = await sut.Handle(request, new CancellationToken());

        result.Value.Should().BeOfType<MediatR.Unit>();
        result.Succeeded.Should().BeTrue();

        var post = await context.PlanProcedureUsers.CountAsync(ppu => ppu.PlanId == 1 && ppu.ProcedureId == 1);
        post.Should().Be(0);
    }
}
