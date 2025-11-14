using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SkillPath.Application.DTOs;
using SkillPath.Application.Interfaces;
using SkillPath.Application.Services;
using SkillPath.Domain.Entities;
using SkillPath.Domain.Repositories;
using Xunit;

namespace SkillPath.Tests;

public class PlanServiceTests
{
    [Fact]
    public async Task GenerateOrGetCurrentAsync_ShouldCreateBasicPlan()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Teste",
            Email = "teste@example.com",
            PasswordHash = "x",
            CurrentJob = "Operador de Caixa",
            TargetArea = "Tecnologia",
            EducationLevel = "Médio"
        };

        var skills = new List<Skill>
        {
            new Skill { Id = 1, Name = "Lógica de Programação (iniciante)", Description = "" },
            new Skill { Id = 2, Name = "Excel Básico ao Intermediário", Description = "" },
            new Skill { Id = 3, Name = "Introdução ao SQL (SELECT/WHERE/JOIN)", Description = "" },
            new Skill { Id = 4, Name = "Pensamento Computacional", Description = "" }
        };

        var userRepo = new Mock<IUserRepository>();
        userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var catalog = new Mock<IStaticCatalogRepository>();
        catalog.Setup(c => c.GetSkillsAsync()).ReturnsAsync(skills);

        var planRepo = new Mock<IPlanRepository>();
        planRepo.Setup(r => r.GetCurrentByUserIdAsync(user.Id)).ReturnsAsync((Plan?)null);

        Plan? savedPlan = null;
        planRepo.Setup(r => r.AddAsync(It.IsAny<Plan>()))
            .Callback<Plan>(p => savedPlan = p)
            .Returns(Task.CompletedTask);

        var service = new PlanService(userRepo.Object, catalog.Object, planRepo.Object);

        var result = await service.GenerateOrGetCurrentAsync(new GeneratePlanRequest(user.Id));

        Assert.NotNull(savedPlan);
        Assert.True(result.Created);
        Assert.True(result.Plan.TotalItems >= 3);
        Assert.Contains(result.Plan.Items, i => i.Skill.Contains("Excel"));
        Assert.All(result.Plan.Items, i => Assert.Null(i.CompletedAt));
    }

    [Fact]
    public async Task GenerateOrGetCurrentAsync_ShouldLimitToSixSkills()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Teste",
            Email = "teste2@example.com",
            PasswordHash = "x",
            CurrentJob = "Operador de Caixa",
            TargetArea = "Tecnologia",
            EducationLevel = "Médio"
        };

        var skills = Enumerable.Range(1, 10)
            .Select(i => new Skill
            {
                Id = i,
                Name = i switch
                {
                    1 => "Lógica de Programação (iniciante)",
                    2 => "Excel Básico ao Intermediário",
                    3 => "Introdução ao SQL (SELECT/WHERE/JOIN)",
                    4 => "Git & GitHub (workflow)",
                    5 => "HTML & CSS (fundamentos)",
                    6 => "Pensamento Computacional",
                    7 => "Gestão de Tempo & Produtividade Pessoal",
                    _ => $"Skill Extra {i}"
                },
                Description = "Desc"
            })
            .ToList();

        var userRepo = new Mock<IUserRepository>();
        userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var catalog = new Mock<IStaticCatalogRepository>();
        catalog.Setup(c => c.GetSkillsAsync()).ReturnsAsync(skills);

        var planRepo = new Mock<IPlanRepository>();
        planRepo.Setup(r => r.GetCurrentByUserIdAsync(user.Id)).ReturnsAsync((Plan?)null);
        planRepo.Setup(r => r.AddAsync(It.IsAny<Plan>())).Returns(Task.CompletedTask);

        var service = new PlanService(userRepo.Object, catalog.Object, planRepo.Object);

        var result = await service.GenerateOrGetCurrentAsync(new GeneratePlanRequest(user.Id));

        Assert.InRange(result.Plan.TotalItems, 3, 6);
    }

    [Fact]
    public async Task ToggleItemCompletion_ShouldUpdateProgressAndStatus()
    {
        var userId = Guid.NewGuid();
        var plan = new Plan
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };

        plan.Items.Add(new PlanItem { Id = 1, PlanId = plan.Id, SkillId = 1, Order = 1, IsCompleted = false });
        plan.Items.Add(new PlanItem { Id = 2, PlanId = plan.Id, SkillId = 2, Order = 2, IsCompleted = false });

        var skills = new List<Skill>
        {
            new Skill { Id = 1, Name = "Lógica de Programação (iniciante)", Description = "" },
            new Skill { Id = 2, Name = "Excel Básico ao Intermediário", Description = "" }
        };

        var userRepo = new Mock<IUserRepository>();
        var catalog = new Mock<IStaticCatalogRepository>();
        catalog.Setup(c => c.GetSkillsAsync()).ReturnsAsync(skills);

        var planRepo = new Mock<IPlanRepository>();
        planRepo.Setup(r => r.GetByIdAsync(plan.Id)).ReturnsAsync(plan);
        planRepo.Setup(r => r.UpdateAsync(plan)).Returns(Task.CompletedTask);

        var service = new PlanService(userRepo.Object, catalog.Object, planRepo.Object);

        var toggled = await service.ToggleItemCompletionAsync(userId, plan.Id, 1);

        var toggledItem = toggled.Items.First(i => i.Order == 1);
        Assert.True(toggledItem.IsCompleted);
        Assert.NotNull(toggledItem.CompletedAt);
        Assert.Equal(1, toggled.CompletedItems);
        Assert.True(toggled.ProgressPercent > 0);
    }
}
