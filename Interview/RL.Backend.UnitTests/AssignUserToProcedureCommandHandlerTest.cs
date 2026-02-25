using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.PlanProcedureUsers;
using RL.Backend.Exceptions;

namespace RL.Backend.UnitTests;

[TestClass]
public class AssignUserToProcedureCommandHandlerTest
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task InvalidPlanId_ReturnsBadRequest(int planId)
    {
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToProcedureCommandHandler(context);
        var request = new AssignUserToProcedureCommand { PlanId = planId, ProcedureId = 1, UserId = 1 };

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
        var sut = new AssignUserToProcedureCommandHandler(context);
        var request = new AssignUserToProcedureCommand { PlanId = 1, ProcedureId = procedureId, UserId = 1 };

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
        var sut = new AssignUserToProcedureCommandHandler(context);
        var request = new AssignUserToProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = userId };

        var result = await sut.Handle(request, new CancellationToken());

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task PlanNotFound_ReturnsNotFound()
    {
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToProcedureCommandHandler(context);
        var request = new AssignUserToProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = 1 };

        // Add procedure and user but not plan
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "T" });
        context.Users.Add(new Data.DataModels.User { UserId = 1 });
        await context.SaveChangesAsync();

        var result = await sut.Handle(request, new CancellationToken());

        result.Exception.Should().BeOfType<NotFoundException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task ProcedureNotFound_ReturnsNotFound()
    {
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToProcedureCommandHandler(context);
        var request = new AssignUserToProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = 1 };

        // Add plan and user but not procedure
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Users.Add(new Data.DataModels.User { UserId = 1 });
        await context.SaveChangesAsync();

        var result = await sut.Handle(request, new CancellationToken());

        result.Exception.Should().BeOfType<NotFoundException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task UserNotFound_ReturnsNotFound()
    {
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToProcedureCommandHandler(context);
        var request = new AssignUserToProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = 1 };

        // Add plan and procedure but not user
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "T" });
        await context.SaveChangesAsync();

        var result = await sut.Handle(request, new CancellationToken());

        result.Exception.Should().BeOfType<NotFoundException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task ProcedureNotInPlan_ReturnsBadRequest()
    {
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToProcedureCommandHandler(context);
        var request = new AssignUserToProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = 1 };

        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "T" });
        context.Users.Add(new Data.DataModels.User { UserId = 1 });
        await context.SaveChangesAsync();

        var result = await sut.Handle(request, new CancellationToken());

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task AlreadyAssigned_ReturnsSuccess()
    {
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToProcedureCommandHandler(context);
        var request = new AssignUserToProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = 1 };

        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "T" });
        context.Users.Add(new Data.DataModels.User { UserId = 1 });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = 1, ProcedureId = 1 });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser { PlanId = 1, ProcedureId = 1, UserId = 1 });
        await context.SaveChangesAsync();

        var result = await sut.Handle(request, new CancellationToken());

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    public async Task CreatesAssignment_ReturnsSuccess()
    {
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToProcedureCommandHandler(context);
        var request = new AssignUserToProcedureCommand { PlanId = 1, ProcedureId = 1, UserId = 1 };

        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "T" });
        context.Users.Add(new Data.DataModels.User { UserId = 1 });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = 1, ProcedureId = 1 });
        await context.SaveChangesAsync();

        var result = await sut.Handle(request, new CancellationToken());

        var dbAssignment = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu => ppu.PlanId == 1 && ppu.ProcedureId == 1 && ppu.UserId == 1);
        dbAssignment.Should().NotBeNull();

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();
    }
}
