using System;
using System.Collections.Generic;
using System.Linq;
using SkillPath.Application.DTOs;
using SkillPath.Application.Exceptions;
using SkillPath.Application.Interfaces;
using SkillPath.Domain.Entities;
using SkillPath.Domain.Repositories;

namespace SkillPath.Application.Services;

public class PlanService : IPlanService
{
    private readonly IUserRepository _users;
    private readonly IStaticCatalogRepository _catalog;
    private readonly IPlanRepository _plans;

    private static readonly IReadOnlyDictionary<string, string[]> AreaSkillMap =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["tecnologia"] = new[]
            {
                "Lógica de Programação (iniciante)",
                "Excel Básico ao Intermediário",
                "Introdução ao SQL (SELECT/WHERE/JOIN)",
                "Git & GitHub (workflow)",
                "HTML & CSS (fundamentos)",
                "Pensamento Computacional"
            },
            ["logistica"] = new[]
            {
                "Excel para Logística (procX, dashboards)",
                "Fundamentos de Logística (estoque, armazenagem, WMS)",
                "ERP para Logística (cadastros, ordens, integrações)",
                "Indicadores de Operações (OTIF, lead time)",
                "Planejamento de Rotas & Custos",
                "Power BI - do zero ao relatório"
            },
            ["atendimento"] = new[]
            {
                "Comunicação & Atendimento (escuta ativa)",
                "Resolução de Conflitos",
                "Noções de CRM (pipeline, cadência)",
                "Vendas Consultivas (SPIN/GPCT)",
                "Excel para Comerciais - previsões e metas",
                "Escrita Profissional (e-mails claros)"
            },
            ["financas"] = new[]
            {
                "Matemática Financeira Básica",
                "Excel Financeiro (projeções)",
                "Introdução a Crédito & Risco",
                "Análise de Demonstrativos (noções)",
                "Power BI Financeiro",
                "Finanças Pessoais - Orçamento e dívida"
            }
        };

    private static readonly string[] CoreSoftSkills =
    {
        "Gestão de Tempo & Produtividade Pessoal",
        "Trabalho em Equipe & Colaboração",
        "Mentalidade de Aprendizado Contínuo",
        "Comunicação Escrita e Oral (prática)"
    };

    public PlanService(IUserRepository users, IStaticCatalogRepository catalog, IPlanRepository plans)
    {
        _users = users;
        _catalog = catalog;
        _plans = plans;
    }

    public async Task<PlanResult> GenerateOrGetCurrentAsync(GeneratePlanRequest request)
    {
        var user = await _users.GetByIdAsync(request.UserId)
                   ?? throw new NotFoundAppException("User");

        var existing = await _plans.GetCurrentByUserIdAsync(user.Id);
        if (existing is not null)
        {
            var skillsCatalog = await _catalog.GetSkillsAsync();
            var response = Map(existing, skillsCatalog);
            return new PlanResult(response, false);
        }

        var skills = await _catalog.GetSkillsAsync();

        var suggested = SuggestSkills(user, skills);

        if (suggested.Count == 0)
        {
            throw new ValidationAppException(new Dictionary<string, string[]>
            {
                ["TargetArea"] = new[] { "Não foi possível gerar uma trilha para a área informada." }
            });
        }

        var plan = new Plan
        {
            UserId = user.Id,
            Title = $"Trilha para migrar para {user.TargetArea}"
        };

        int order = 1;
        foreach (var skill in suggested)
        {
            plan.Items.Add(new PlanItem
            {
                SkillId = skill.Id,
                Order = order++,
                IsCompleted = false,
                CompletedAt = null
            });
        }

        await _plans.AddAsync(plan);

        var mapped = Map(plan, skills);
        return new PlanResult(mapped, true);
    }

    public async Task<PlanResponse> GetCurrentByUserAsync(Guid userId)
    {
        var plan = await _plans.GetCurrentByUserIdAsync(userId)
                   ?? throw new NotFoundAppException("Plan");
        var skills = await _catalog.GetSkillsAsync();
        return Map(plan, skills);
    }

    public async Task<PlanResponse> ToggleItemCompletionAsync(Guid userId, Guid planId, int order)
    {
        var plan = await _plans.GetByIdAsync(planId)
                   ?? throw new NotFoundAppException("Plan");

        if (plan.UserId != userId)
            throw new ConflictAppException("Plano informado não pertence ao usuário autenticado.");

        var item = plan.Items.FirstOrDefault(i => i.Order == order)
                   ?? throw new NotFoundAppException("PlanItem");
        item.IsCompleted = !item.IsCompleted;
        item.CompletedAt = item.IsCompleted ? DateTime.UtcNow : null;

        await _plans.UpdateAsync(plan);

        var skills = await _catalog.GetSkillsAsync();
        return Map(plan, skills);
    }

    public async Task DeleteAsync(Guid userId, Guid planId)
    {
        var plan = await _plans.GetByIdAsync(planId)
                   ?? throw new NotFoundAppException("Plan");

        if (plan.UserId != userId)
            throw new ConflictAppException("Plano informado não pertence ao usuário autenticado.");

        await _plans.DeleteAsync(plan);
    }

    private static List<Skill> SuggestSkills(User user, IReadOnlyList<Skill> all)
    {
        var normalized = all.ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);

        var targetAreaKey = ResolveAreaKey(user.TargetArea);

        var selectedNames = new List<string>();

        if (targetAreaKey is not null && AreaSkillMap.TryGetValue(targetAreaKey, out var mappedSkills))
        {
            selectedNames.AddRange(mappedSkills);
        }

        // Ajustes simples baseados na profissão atual para reforçar lacunas.
        if (user.CurrentJob.Contains("caixa", StringComparison.OrdinalIgnoreCase) &&
            (targetAreaKey?.Equals("tecnologia", StringComparison.OrdinalIgnoreCase) ?? false))
        {
            selectedNames.Insert(0, "Lógica de Programação (iniciante)");
            selectedNames.Insert(1, "Excel Básico ao Intermediário");
            selectedNames.Insert(2, "Introdução ao SQL (SELECT/WHERE/JOIN)");
        }

        if (selectedNames.Count < 3)
        {
            selectedNames.AddRange(CoreSoftSkills);
        }

        var distinctNames = selectedNames
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(6)
            .ToList();

        var resolvedSkills = new List<Skill>();
        foreach (var name in distinctNames)
        {
            if (normalized.TryGetValue(name, out var skill))
            {
                resolvedSkills.Add(skill);
            }
        }

        if (resolvedSkills.Count < 3)
        {
            foreach (var skill in all.OrderBy(s => s.Name))
            {
                if (resolvedSkills.Any(existing => existing.Id == skill.Id))
                    continue;

                resolvedSkills.Add(skill);
                if (resolvedSkills.Count >= 3)
                    break;
            }
        }

        return resolvedSkills.Take(6).ToList();
    }

    private static string? ResolveAreaKey(string targetArea)
    {
        var value = targetArea.ToLowerInvariant();

        if (value.Contains("tecnolog") || value.Contains("dados") || value.Contains("program"))
            return "tecnologia";
        if (value.Contains("log") || value.Contains("operacao") || value.Contains("operac") || value.Contains("supply"))
            return "logistica";
        if (value.Contains("atendimento") || value.Contains("venda") || value.Contains("comercial") || value.Contains("sac"))
            return "atendimento";
        if (value.Contains("finan") || value.Contains("credito") || value.Contains("contab"))
            return "financas";

        return null;
    }

    private static PlanResponse Map(Plan plan, IReadOnlyList<Skill> skills)
    {
        var skillLookup = skills.ToDictionary(s => s.Id, s => s);

        var items = plan.Items
            .OrderBy(i => i.Order)
            .Select(item =>
            {
                skillLookup.TryGetValue(item.SkillId, out var skill);
                return new PlanItemResponse(
                    item.SkillId,
                    item.Order,
                    skill?.Name ?? $"Skill {item.SkillId}",
                    skill?.Description ?? string.Empty,
                    item.IsCompleted,
                    item.CompletedAt
                );
            })
            .ToList();

        var total = items.Count;
        var completed = items.Count(i => i.IsCompleted);

        var links = new PlanLinks(
            Self: "/api/v1/plans",
            ToggleItemTemplate: $"/api/v1/plans/{plan.Id}/items/{{order}}",
            CoursesTemplate: "/api/v1/courses?skillId={skillId}"
        );

        return new PlanResponse(plan.Id, plan.Title, plan.CreatedAt, total, completed, items, links);
    }
}
