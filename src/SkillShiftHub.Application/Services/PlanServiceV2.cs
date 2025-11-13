using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SkillShiftHub.Application.DTOs;
using SkillShiftHub.Application.Exceptions;
using SkillShiftHub.Application.Interfaces;
using SkillShiftHub.Domain.Entities;
using SkillShiftHub.Domain.Repositories;

namespace SkillShiftHub.Application.Services;

public class PlanServiceV2 : IPlanServiceV2
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
                "Introdução ao SQL (SELECT/WHERE/JOIN)",
                "Git & GitHub (workflow)",
                "HTML & CSS (fundamentos)",
                "Modelagem de Dados (conceitual e lógica)",
                "Noções de JavaScript (básico)",
                "Power BI - do zero ao relatório"
            },
            ["logistica"] = new[]
            {
                "Fundamentos de Logística (estoque, armazenagem, WMS)",
                "ERP para Logística (cadastros, ordens, integrações)",
                "Excel para Logística (procX, dashboards)",
                "Planejamento de Rotas & Custos",
                "Indicadores de Operações (OTIF, lead time)",
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
                "Gestão de Tempo & Produtividade Pessoal"
            }
        };

    private static readonly string[] CoreSoftSkills =
    {
        "Gestão de Tempo & Produtividade Pessoal",
        "Trabalho em Equipe & Colaboração",
        "Mentalidade de Aprendizado Contínuo",
        "Comunicação Escrita e Oral (prática)"
    };

    public PlanServiceV2(IUserRepository users, IStaticCatalogRepository catalog, IPlanRepository plans)
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
            var response = Map(existing, skillsCatalog, 2);
            return new PlanResult(response, false);
        }

        var skills = await _catalog.GetSkillsAsync();

        var suggested = SuggestSkills(user, skills);

        // Garantir que sempre temos pelo menos algumas skills
        if (suggested.Count == 0)
        {
            // Fallback: pegar as primeiras skills disponíveis
            suggested = skills.Take(6).ToList();
            
            if (suggested.Count == 0)
            {
                throw new ValidationAppException(new Dictionary<string, string[]>
                {
                    ["TargetArea"] = new[] { "Não foi possível gerar uma trilha. Nenhuma skill disponível no catálogo." }
                });
            }
        }

        var plan = new Plan
        {
            UserId = user.Id,
            Title = $"Trilha para migrar para {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.TargetArea)}"
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

        var mapped = Map(plan, skills, 2);
        return new PlanResult(mapped, true);
    }

    public async Task<PlanResponse> GetCurrentByUserAsync(Guid userId)
    {
        var plan = await _plans.GetCurrentByUserIdAsync(userId)
                   ?? throw new NotFoundAppException("Plan");
        var skills = await _catalog.GetSkillsAsync();
        return Map(plan, skills, 2);
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
        return Map(plan, skills, 2);
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

        // Education-awareness: reforça habilidades fundacionais se educação menor.
        if (IsLowerEducation(user.EducationLevel))
        {
            selectedNames.Insert(0, "Pensamento Computacional");
            selectedNames.Insert(1, "Gestão de Tempo & Produtividade Pessoal");
        }
        else if (IsHigherEducation(user.EducationLevel) && targetAreaKey == "tecnologia")
        {
            selectedNames.Insert(0, "Modelagem de Dados (conceitual e lógica)");
            selectedNames.Insert(1, "Power BI - do zero ao relatório");
        }

        if (!selectedNames.Any())
        {
            selectedNames.AddRange(CoreSoftSkills);
        }

        // Garantir equilíbrio entre habilidades técnicas e comportamentais.
        if (selectedNames.Count < 5)
        {
            selectedNames.AddRange(CoreSoftSkills);
        }

        var distinctNames = selectedNames
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var resolved = new List<Skill>();
        foreach (var name in distinctNames)
        {
            if (resolved.Count >= 6)
                break;

            if (normalized.TryGetValue(name, out var skill))
            {
                resolved.Add(skill);
            }
        }

        // Se ainda não temos skills suficientes, adicionar do catálogo geral
        if (resolved.Count < 4)
        {
            foreach (var skill in all.OrderBy(s => s.Name))
            {
                if (resolved.Any(existing => existing.Id == skill.Id))
                    continue;

                resolved.Add(skill);
                if (resolved.Count >= 6)
                    break;
            }
        }

        // Garantir que sempre retornamos pelo menos 3 skills
        if (resolved.Count == 0)
        {
            resolved.AddRange(all.Take(6));
        }

        return resolved.Take(6).ToList();
    }

    private static bool IsLowerEducation(string educationLevel) =>
        educationLevel.Contains("fundamental", StringComparison.OrdinalIgnoreCase) ||
        educationLevel.Contains("básico", StringComparison.OrdinalIgnoreCase) ||
        educationLevel.Contains("medio incompleto", StringComparison.OrdinalIgnoreCase);

    private static bool IsHigherEducation(string educationLevel) =>
        educationLevel.Contains("superior", StringComparison.OrdinalIgnoreCase) ||
        educationLevel.Contains("graduação", StringComparison.OrdinalIgnoreCase) ||
        educationLevel.Contains("pos", StringComparison.OrdinalIgnoreCase) ||
        educationLevel.Contains("pós", StringComparison.OrdinalIgnoreCase);

    private static string? ResolveAreaKey(string targetArea)
    {
        if (string.IsNullOrWhiteSpace(targetArea))
            return null;

        var value = targetArea.ToLowerInvariant().Trim();

        // Tecnologia: aceita variações como "tech", "ti", "informática", "desenvolvimento", "software", "dados", "programação"
        if (value.Contains("tecnolog") || value.Contains("tech") || value.Contains("ti ") || 
            value.Contains("informática") || value.Contains("informatica") || value.Contains("dados") || 
            value.Contains("program") || value.Contains("desenvolvimento") || value.Contains("software") ||
            value.Contains("analista") || value.Contains("dev"))
            return "tecnologia";
        
        // Logística: aceita variações como "log", "operacao", "operac", "supply", "suprimentos", "estoque"
        if (value.Contains("log") || value.Contains("operacao") || value.Contains("operac") || 
            value.Contains("supply") || value.Contains("suprimento") || value.Contains("estoque") ||
            value.Contains("armazem") || value.Contains("armazém") || value.Contains("distribuicao") ||
            value.Contains("distribuição"))
            return "logistica";
        
        // Atendimento: aceita variações como "atendimento", "venda", "comercial", "sac", "suporte", "relacionamento"
        if (value.Contains("atendimento") || value.Contains("venda") || value.Contains("comercial") || 
            value.Contains("sac") || value.Contains("suporte") || value.Contains("relacionamento") ||
            value.Contains("cliente") || value.Contains("customer"))
            return "atendimento";
        
        // Finanças: aceita variações como "finan", "credito", "contab", "financeiro", "contabilidade"
        if (value.Contains("finan") || value.Contains("credito") || value.Contains("crédito") || 
            value.Contains("contab") || value.Contains("financeiro") || value.Contains("contabilidade") ||
            value.Contains("banco") || value.Contains("investimento"))
            return "financas";

        return null;
    }

    private static PlanResponse Map(Plan plan, IReadOnlyList<Skill> skills, int version)
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
            Self: $"/api/v{version}/plans",
            ToggleItemTemplate: $"/api/v{version}/plans/{plan.Id}/items/{{order}}",
            CoursesTemplate: $"/api/v{version}/courses?skillId={{skillId}}"
        );

        return new PlanResponse(plan.Id, plan.Title, plan.CreatedAt, total, completed, items, links);
    }
}


