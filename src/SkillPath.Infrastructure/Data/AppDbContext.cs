using Microsoft.EntityFrameworkCore;
using SkillPath.Domain.Entities;

namespace SkillPath.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanItem> PlanItems => Set<PlanItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.Name)
            .HasMaxLength(120);

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasMaxLength(160);

        modelBuilder.Entity<User>()
            .Property(u => u.PasswordHash)
            .HasMaxLength(256);

        modelBuilder.Entity<User>()
            .Property(u => u.CurrentJob)
            .HasMaxLength(160);

        modelBuilder.Entity<User>()
            .Property(u => u.TargetArea)
            .HasMaxLength(160);

        modelBuilder.Entity<User>()
            .Property(u => u.EducationLevel)
            .HasMaxLength(120);

        modelBuilder.Entity<User>()
            .ToTable("USER_APP");

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Plan>()
            .Property(p => p.Title)
            .HasMaxLength(200);

        modelBuilder.Entity<Plan>()
            .HasMany(p => p.Items)
            .WithOne()
            .HasForeignKey(pi => pi.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Plan>()
            .HasIndex(p => new { p.UserId, p.CreatedAt });

        modelBuilder.Entity<PlanItem>()
            .HasIndex(pi => new { pi.PlanId, pi.Order })
            .IsUnique();

        modelBuilder.Entity<PlanItem>()
            .Property(pi => pi.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Skill>()
            .Property(s => s.Name)
            .HasMaxLength(200);

        modelBuilder.Entity<Skill>()
            .Property(s => s.Description)
            .HasMaxLength(400);

        modelBuilder.Entity<Course>()
            .Property(c => c.Name)
            .HasMaxLength(220);

        modelBuilder.Entity<Course>()
            .Property(c => c.Provider)
            .HasMaxLength(160);

        modelBuilder.Entity<Course>()
            .Property(c => c.Url)
            .HasMaxLength(400);

        var skills = new[]
        {
            new Skill { Id = 1, Name = "Lógica de Programação (iniciante)", Description = "Fundamentos de algoritmos, estruturas condicionais e resolução de problemas." },
            new Skill { Id = 2, Name = "Excel Básico ao Intermediário", Description = "Construção de planilhas, fórmulas, gráficos e tabelas dinâmicas." },
            new Skill { Id = 3, Name = "Introdução ao SQL (SELECT/WHERE/JOIN)", Description = "Consultas básicas, filtros e relacionamentos entre tabelas." },
            new Skill { Id = 4, Name = "Git & GitHub (workflow)", Description = "Trabalho com versionamento distribuído, branches e pull requests." },
            new Skill { Id = 5, Name = "HTML & CSS (fundamentos)", Description = "Estruturação de páginas, semântica e princípios de responsividade." },
            new Skill { Id = 6, Name = "Pensamento Computacional", Description = "Decomposição, padrões e criação de soluções sistemáticas." },
            new Skill { Id = 7, Name = "Excel para Logística (procX, dashboards)", Description = "Funções avançadas, análises e visualização de dados logísticos." },
            new Skill { Id = 8, Name = "Fundamentos de Logística (estoque, armazenagem, WMS)", Description = "Processos essenciais de cadeia de suprimentos e controle de estoque." },
            new Skill { Id = 9, Name = "ERP para Logística (cadastros, ordens, integrações)", Description = "Integração de processos logísticos em sistemas corporativos." },
            new Skill { Id = 10, Name = "Indicadores de Operações (OTIF, lead time)", Description = "Monitoramento e análise de métricas operacionais críticas." },
            new Skill { Id = 11, Name = "Planejamento de Rotas & Custos", Description = "Estratégias para otimizar deslocamentos e reduzir custos logísticos." },
            new Skill { Id = 12, Name = "Power BI - do zero ao relatório", Description = "Criação de dashboards, modelagem simples e publicação de relatórios." },
            new Skill { Id = 13, Name = "Comunicação & Atendimento (escuta ativa)", Description = "Técnicas para relacionamento empático e profissional com clientes." },
            new Skill { Id = 14, Name = "Resolução de Conflitos", Description = "Estratégias práticas para tratar objeções e situações difíceis." },
            new Skill { Id = 15, Name = "Noções de CRM (pipeline, cadência)", Description = "Gestão de relacionamento com clientes e acompanhamento de funil." },
            new Skill { Id = 16, Name = "Vendas Consultivas (SPIN/GPCT)", Description = "Métodos estruturados para diagnóstico e geração de valor em vendas." },
            new Skill { Id = 17, Name = "Excel para Comerciais - previsões e metas", Description = "Construção de projeções, metas e acompanhamento de resultados de vendas." },
            new Skill { Id = 18, Name = "Escrita Profissional (e-mails claros)", Description = "Comunicação escrita objetiva e alinhada ao contexto corporativo." },
            new Skill { Id = 19, Name = "Matemática Financeira Básica", Description = "Juros simples e compostos, descontos e aplicações práticas." },
            new Skill { Id = 20, Name = "Excel Financeiro (projeções)", Description = "Ferramentas de análise financeira e cenários em planilhas." },
            new Skill { Id = 21, Name = "Introdução a Crédito & Risco", Description = "Conceitos de avaliação de crédito, risco e concessão responsável." },
            new Skill { Id = 22, Name = "Análise de Demonstrativos (noções)", Description = "Leitura de DRE, Balanço Patrimonial e indicadores básicos." },
            new Skill { Id = 23, Name = "Power BI Financeiro", Description = "Visualização de indicadores financeiros e storytelling com dados." },
            new Skill { Id = 24, Name = "Finanças Pessoais - Orçamento e dívida", Description = "Organização financeira, metas e renegociação de dívidas." },
            new Skill { Id = 25, Name = "Gestão de Tempo & Produtividade Pessoal", Description = "Priorização, planejamento semanal e técnicas de foco." },
            new Skill { Id = 26, Name = "Trabalho em Equipe & Colaboração", Description = "Princípios de colaboração, feedback e alinhamento entre pares." },
            new Skill { Id = 27, Name = "Mentalidade de Aprendizado Contínuo", Description = "Construção de planos de desenvolvimento e aprendizado ativo." },
            new Skill { Id = 28, Name = "Comunicação Escrita e Oral (prática)", Description = "Clareza, objetividade e storytelling em apresentações e textos." },
            new Skill { Id = 29, Name = "Modelagem de Dados (conceitual e lógica)", Description = "Diagramas, normalização e boas práticas de modelagem." },
            new Skill { Id = 30, Name = "Noções de JavaScript (básico)", Description = "Sintaxe essencial, manipulação de DOM e eventos." }
        };

        modelBuilder.Entity<Skill>().HasData(skills);

        var courses = new[]
        {
            new Course { Id = 1, SkillId = 1, Name = "Lógica de Programação com C# e Jogos", Provider = "Alura", Url = "https://example.com/logica-iniciante" },
            new Course { Id = 2, SkillId = 2, Name = "Excel Essencial: do zero às tabelas dinâmicas", Provider = "Udemy", Url = "https://example.com/excel-basico" },
            new Course { Id = 3, SkillId = 3, Name = "SQL para Análise de Dados", Provider = "Data Academy", Url = "https://example.com/sql-fundamentos" },
            new Course { Id = 4, SkillId = 4, Name = "Git & GitHub na Prática", Provider = "DIO", Url = "https://example.com/git-workflow" },
            new Course { Id = 5, SkillId = 5, Name = "HTML e CSS para Iniciantes", Provider = "Origamid", Url = "https://example.com/html-css" },
            new Course { Id = 6, SkillId = 6, Name = "Pensamento Computacional para Todos", Provider = "Coursera", Url = "https://example.com/pensamento-computacional" },
            new Course { Id = 7, SkillId = 7, Name = "Excel Avançado Aplicado à Logística", Provider = "FGV Online", Url = "https://example.com/excel-logistica" },
            new Course { Id = 8, SkillId = 8, Name = "Operations Management Basics", Provider = "Coursera", Url = "https://example.com/fund-logistica" },
            new Course { Id = 9, SkillId = 9, Name = "ERP Logístico na Prática", Provider = "SENAI", Url = "https://example.com/erp-logistica" },
            new Course { Id = 10, SkillId = 10, Name = "KPIs de Operações: Monitoramento e Melhoria", Provider = "LinkedIn Learning", Url = "https://example.com/kpis-operacoes" },
            new Course { Id = 11, SkillId = 11, Name = "Roteirização e Custos Logísticos", Provider = "LogSchool", Url = "https://example.com/rotas-custos" },
            new Course { Id = 12, SkillId = 12, Name = "Power BI: Dashboards Profissionais", Provider = "Microsoft Learn", Url = "https://example.com/powerbi-basico" },
            new Course { Id = 13, SkillId = 13, Name = "Atendimento ao Cliente com Escuta Ativa", Provider = "SEBRAE", Url = "https://example.com/atendimento-escuta" },
            new Course { Id = 14, SkillId = 14, Name = "Gestão de Conflitos no Atendimento", Provider = "Udemy", Url = "https://example.com/resolucao-conflitos" },
            new Course { Id = 15, SkillId = 15, Name = "CRM e Jornada do Cliente", Provider = "RD Station", Url = "https://example.com/crm-jornada" },
            new Course { Id = 16, SkillId = 16, Name = "Vendas Consultivas com SPIN Selling", Provider = "Fundação Dom Cabral", Url = "https://example.com/vendas-consultivas" },
            new Course { Id = 17, SkillId = 17, Name = "Excel para Forecast de Vendas", Provider = "G4 Educação", Url = "https://example.com/excel-comercial" },
            new Course { Id = 18, SkillId = 18, Name = "Escrita Corporativa Objetiva", Provider = "PUC Minas", Url = "https://example.com/escrita-profissional" },
            new Course { Id = 19, SkillId = 19, Name = "Matemática Financeira Aplicada", Provider = "Coursera", Url = "https://example.com/matematica-financeira" },
            new Course { Id = 20, SkillId = 20, Name = "Excel Financeiro com TIR e VPL", Provider = "Udemy", Url = "https://example.com/excel-financeiro" },
            new Course { Id = 21, SkillId = 21, Name = "Introdução a Crédito e Cobrança", Provider = "Serasa Experian", Url = "https://example.com/credito-risco" },
            new Course { Id = 22, SkillId = 22, Name = "Leitura de Demonstrativos Financeiros", Provider = "B3 Educação", Url = "https://example.com/analise-demonstrativos" },
            new Course { Id = 23, SkillId = 23, Name = "Power BI para Finanças", Provider = "FIAP", Url = "https://example.com/powerbi-financeiro" },
            new Course { Id = 24, SkillId = 24, Name = "Educação Financeira Pessoal", Provider = "Banco Central do Brasil", Url = "https://example.com/financas-pessoais" },
            new Course { Id = 25, SkillId = 25, Name = "Produtividade e Prioridades", Provider = "GetAbstract", Url = "https://example.com/gestao-tempo" },
            new Course { Id = 26, SkillId = 26, Name = "Colaboração e Trabalho em Equipe", Provider = "Google Skillshop", Url = "https://example.com/trabalho-equipe" },
            new Course { Id = 27, SkillId = 27, Name = "Lifelong Learning na Prática", Provider = "LinkedIn Learning", Url = "https://example.com/aprendizado-continuo" },
            new Course { Id = 28, SkillId = 28, Name = "Comunicação Escrita e Oral para Profissionais", Provider = "SENAC", Url = "https://example.com/comunicacao-pratica" },
            new Course { Id = 29, SkillId = 29, Name = "Modelagem de Dados Essencial", Provider = "Data University", Url = "https://example.com/modelagem-dados" },
            new Course { Id = 30, SkillId = 30, Name = "JavaScript para Iniciantes", Provider = "freeCodeCamp", Url = "https://example.com/js-basico" }
        };

        modelBuilder.Entity<Course>().HasData(courses);
    }
}
