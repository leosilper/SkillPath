using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillPath.Infrastructure.Data;

#nullable disable

namespace SkillPath.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "7.0.3")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("SkillPath.Domain.Entities.Course", b =>
            {
                b.Property<int>("Id")
                    .HasColumnType("int");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(220)
                    .HasColumnType("nvarchar(220)");

                b.Property<string>("Provider")
                    .IsRequired()
                    .HasMaxLength(160)
                    .HasColumnType("nvarchar(160)");

                b.Property<int>("SkillId")
                    .HasColumnType("int");

                b.Property<string>("Url")
                    .IsRequired()
                    .HasMaxLength(400)
                    .HasColumnType("nvarchar(400)");

                b.HasKey("Id");

                b.HasIndex("SkillId");

                b.ToTable("Courses");

                b.HasData(
                    new
                    {
                        Id = 1,
                        Name = "Lógica de Programação com C# e Jogos",
                        Provider = "Alura",
                        SkillId = 1,
                        Url = "https://example.com/logica-iniciante"
                    },
                    new
                    {
                        Id = 2,
                        Name = "Excel Essencial: do zero às tabelas dinâmicas",
                        Provider = "Udemy",
                        SkillId = 2,
                        Url = "https://example.com/excel-basico"
                    },
                    new
                    {
                        Id = 3,
                        Name = "SQL para Análise de Dados",
                        Provider = "Data Academy",
                        SkillId = 3,
                        Url = "https://example.com/sql-fundamentos"
                    },
                    new
                    {
                        Id = 4,
                        Name = "Git & GitHub na Prática",
                        Provider = "DIO",
                        SkillId = 4,
                        Url = "https://example.com/git-workflow"
                    },
                    new
                    {
                        Id = 5,
                        Name = "HTML e CSS para Iniciantes",
                        Provider = "Origamid",
                        SkillId = 5,
                        Url = "https://example.com/html-css"
                    },
                    new
                    {
                        Id = 6,
                        Name = "Pensamento Computacional para Todos",
                        Provider = "Coursera",
                        SkillId = 6,
                        Url = "https://example.com/pensamento-computacional"
                    },
                    new
                    {
                        Id = 7,
                        Name = "Excel Avançado Aplicado à Logística",
                        Provider = "FGV Online",
                        SkillId = 7,
                        Url = "https://example.com/excel-logistica"
                    },
                    new
                    {
                        Id = 8,
                        Name = "Operations Management Basics",
                        Provider = "Coursera",
                        SkillId = 8,
                        Url = "https://example.com/fund-logistica"
                    },
                    new
                    {
                        Id = 9,
                        Name = "ERP Logístico na Prática",
                        Provider = "SENAI",
                        SkillId = 9,
                        Url = "https://example.com/erp-logistica"
                    },
                    new
                    {
                        Id = 10,
                        Name = "KPIs de Operações: Monitoramento e Melhoria",
                        Provider = "LinkedIn Learning",
                        SkillId = 10,
                        Url = "https://example.com/kpis-operacoes"
                    },
                    new
                    {
                        Id = 11,
                        Name = "Roteirização e Custos Logísticos",
                        Provider = "LogSchool",
                        SkillId = 11,
                        Url = "https://example.com/rotas-custos"
                    },
                    new
                    {
                        Id = 12,
                        Name = "Power BI: Dashboards Profissionais",
                        Provider = "Microsoft Learn",
                        SkillId = 12,
                        Url = "https://example.com/powerbi-basico"
                    },
                    new
                    {
                        Id = 13,
                        Name = "Atendimento ao Cliente com Escuta Ativa",
                        Provider = "SEBRAE",
                        SkillId = 13,
                        Url = "https://example.com/atendimento-escuta"
                    },
                    new
                    {
                        Id = 14,
                        Name = "Gestão de Conflitos no Atendimento",
                        Provider = "Udemy",
                        SkillId = 14,
                        Url = "https://example.com/resolucao-conflitos"
                    },
                    new
                    {
                        Id = 15,
                        Name = "CRM e Jornada do Cliente",
                        Provider = "RD Station",
                        SkillId = 15,
                        Url = "https://example.com/crm-jornada"
                    },
                    new
                    {
                        Id = 16,
                        Name = "Vendas Consultivas com SPIN Selling",
                        Provider = "Fundação Dom Cabral",
                        SkillId = 16,
                        Url = "https://example.com/vendas-consultivas"
                    },
                    new
                    {
                        Id = 17,
                        Name = "Excel para Forecast de Vendas",
                        Provider = "G4 Educação",
                        SkillId = 17,
                        Url = "https://example.com/excel-comercial"
                    },
                    new
                    {
                        Id = 18,
                        Name = "Escrita Corporativa Objetiva",
                        Provider = "PUC Minas",
                        SkillId = 18,
                        Url = "https://example.com/escrita-profissional"
                    },
                    new
                    {
                        Id = 19,
                        Name = "Matemática Financeira Aplicada",
                        Provider = "Coursera",
                        SkillId = 19,
                        Url = "https://example.com/matematica-financeira"
                    },
                    new
                    {
                        Id = 20,
                        Name = "Excel Financeiro com TIR e VPL",
                        Provider = "Udemy",
                        SkillId = 20,
                        Url = "https://example.com/excel-financeiro"
                    },
                    new
                    {
                        Id = 21,
                        Name = "Introdução a Crédito e Cobrança",
                        Provider = "Serasa Experian",
                        SkillId = 21,
                        Url = "https://example.com/credito-risco"
                    },
                    new
                    {
                        Id = 22,
                        Name = "Leitura de Demonstrativos Financeiros",
                        Provider = "B3 Educação",
                        SkillId = 22,
                        Url = "https://example.com/analise-demonstrativos"
                    },
                    new
                    {
                        Id = 23,
                        Name = "Power BI para Finanças",
                        Provider = "FIAP",
                        SkillId = 23,
                        Url = "https://example.com/powerbi-financeiro"
                    },
                    new
                    {
                        Id = 24,
                        Name = "Educação Financeira Pessoal",
                        Provider = "Banco Central do Brasil",
                        SkillId = 24,
                        Url = "https://example.com/financas-pessoais"
                    },
                    new
                    {
                        Id = 25,
                        Name = "Produtividade e Prioridades",
                        Provider = "GetAbstract",
                        SkillId = 25,
                        Url = "https://example.com/gestao-tempo"
                    },
                    new
                    {
                        Id = 26,
                        Name = "Colaboração e Trabalho em Equipe",
                        Provider = "Google Skillshop",
                        SkillId = 26,
                        Url = "https://example.com/trabalho-equipe"
                    },
                    new
                    {
                        Id = 27,
                        Name = "Lifelong Learning na Prática",
                        Provider = "LinkedIn Learning",
                        SkillId = 27,
                        Url = "https://example.com/aprendizado-continuo"
                    },
                    new
                    {
                        Id = 28,
                        Name = "Comunicação Escrita e Oral para Profissionais",
                        Provider = "SENAC",
                        SkillId = 28,
                        Url = "https://example.com/comunicacao-pratica"
                    },
                    new
                    {
                        Id = 29,
                        Name = "Modelagem de Dados Essencial",
                        Provider = "Data University",
                        SkillId = 29,
                        Url = "https://example.com/modelagem-dados"
                    },
                    new
                    {
                        Id = 30,
                        Name = "JavaScript para Iniciantes",
                        Provider = "freeCodeCamp",
                        SkillId = 30,
                        Url = "https://example.com/js-basico"
                    });
            });

        modelBuilder.Entity("SkillPath.Domain.Entities.Plan", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.HasIndex("UserId", "CreatedAt");

                b.ToTable("Plans");
            });

        modelBuilder.Entity("SkillPath.Domain.Entities.PlanItem", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CompletedAt")
                    .HasColumnType("datetime2");

                b.Property<bool>("IsCompleted")
                    .HasColumnType("bit");

                b.Property<int>("Order")
                    .HasColumnType("int");

                b.Property<Guid>("PlanId")
                    .HasColumnType("uniqueidentifier");

                b.Property<int>("SkillId")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("PlanId", "Order")
                    .IsUnique();

                b.HasIndex("SkillId");

                b.ToTable("PlanItems");
            });

        modelBuilder.Entity("SkillPath.Domain.Entities.Skill", b =>
            {
                b.Property<int>("Id")
                    .HasColumnType("int");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasMaxLength(400)
                    .HasColumnType("nvarchar(400)");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.HasKey("Id");

                b.ToTable("Skills");

                b.HasData(
                    new
                    {
                        Id = 1,
                        Description = "Fundamentos de algoritmos, estruturas condicionais e resolução de problemas.",
                        Name = "Lógica de Programação (iniciante)"
                    },
                    new
                    {
                        Id = 2,
                        Description = "Construção de planilhas, fórmulas, gráficos e tabelas dinâmicas.",
                        Name = "Excel Básico ao Intermediário"
                    },
                    new
                    {
                        Id = 3,
                        Description = "Consultas básicas, filtros e relacionamentos entre tabelas.",
                        Name = "Introdução ao SQL (SELECT/WHERE/JOIN)"
                    },
                    new
                    {
                        Id = 4,
                        Description = "Trabalho com versionamento distribuído, branches e pull requests.",
                        Name = "Git & GitHub (workflow)"
                    },
                    new
                    {
                        Id = 5,
                        Description = "Estruturação de páginas, semântica e princípios de responsividade.",
                        Name = "HTML & CSS (fundamentos)"
                    },
                    new
                    {
                        Id = 6,
                        Description = "Decomposição, padrões e criação de soluções sistemáticas.",
                        Name = "Pensamento Computacional"
                    },
                    new
                    {
                        Id = 7,
                        Description = "Funções avançadas, análises e visualização de dados logísticos.",
                        Name = "Excel para Logística (procX, dashboards)"
                    },
                    new
                    {
                        Id = 8,
                        Description = "Processos essenciais de cadeia de suprimentos e controle de estoque.",
                        Name = "Fundamentos de Logística (estoque, armazenagem, WMS)"
                    },
                    new
                    {
                        Id = 9,
                        Description = "Integração de processos logísticos em sistemas corporativos.",
                        Name = "ERP para Logística (cadastros, ordens, integrações)"
                    },
                    new
                    {
                        Id = 10,
                        Description = "Monitoramento e análise de métricas operacionais críticas.",
                        Name = "Indicadores de Operações (OTIF, lead time)"
                    },
                    new
                    {
                        Id = 11,
                        Description = "Estratégias para otimizar deslocamentos e reduzir custos logísticos.",
                        Name = "Planejamento de Rotas & Custos"
                    },
                    new
                    {
                        Id = 12,
                        Description = "Criação de dashboards, modelagem simples e publicação de relatórios.",
                        Name = "Power BI - do zero ao relatório"
                    },
                    new
                    {
                        Id = 13,
                        Description = "Técnicas para relacionamento empático e profissional com clientes.",
                        Name = "Comunicação & Atendimento (escuta ativa)"
                    },
                    new
                    {
                        Id = 14,
                        Description = "Estratégias práticas para tratar objeções e situações difíceis.",
                        Name = "Resolução de Conflitos"
                    },
                    new
                    {
                        Id = 15,
                        Description = "Gestão de relacionamento com clientes e acompanhamento de funil.",
                        Name = "Noções de CRM (pipeline, cadência)"
                    },
                    new
                    {
                        Id = 16,
                        Description = "Métodos estruturados para diagnóstico e geração de valor em vendas.",
                        Name = "Vendas Consultivas (SPIN/GPCT)"
                    },
                    new
                    {
                        Id = 17,
                        Description = "Construção de projeções, metas e acompanhamento de resultados de vendas.",
                        Name = "Excel para Comerciais - previsões e metas"
                    },
                    new
                    {
                        Id = 18,
                        Description = "Comunicação escrita objetiva e alinhada ao contexto corporativo.",
                        Name = "Escrita Profissional (e-mails claros)"
                    },
                    new
                    {
                        Id = 19,
                        Description = "Juros simples e compostos, descontos e aplicações práticas.",
                        Name = "Matemática Financeira Básica"
                    },
                    new
                    {
                        Id = 20,
                        Description = "Ferramentas de análise financeira e cenários em planilhas.",
                        Name = "Excel Financeiro (projeções)"
                    },
                    new
                    {
                        Id = 21,
                        Description = "Conceitos de avaliação de crédito, risco e concessão responsável.",
                        Name = "Introdução a Crédito & Risco"
                    },
                    new
                    {
                        Id = 22,
                        Description = "Leitura de DRE, Balanço Patrimonial e indicadores básicos.",
                        Name = "Análise de Demonstrativos (noções)"
                    },
                    new
                    {
                        Id = 23,
                        Description = "Visualização de indicadores financeiros e storytelling com dados.",
                        Name = "Power BI Financeiro"
                    },
                    new
                    {
                        Id = 24,
                        Description = "Organização financeira, metas e renegociação de dívidas.",
                        Name = "Finanças Pessoais - Orçamento e dívida"
                    },
                    new
                    {
                        Id = 25,
                        Description = "Priorização, planejamento semanal e técnicas de foco.",
                        Name = "Gestão de Tempo & Produtividade Pessoal"
                    },
                    new
                    {
                        Id = 26,
                        Description = "Princípios de colaboração, feedback e alinhamento entre pares.",
                        Name = "Trabalho em Equipe & Colaboração"
                    },
                    new
                    {
                        Id = 27,
                        Description = "Construção de planos de desenvolvimento e aprendizado ativo.",
                        Name = "Mentalidade de Aprendizado Contínuo"
                    },
                    new
                    {
                        Id = 28,
                        Description = "Clareza, objetividade e storytelling em apresentações e textos.",
                        Name = "Comunicação Escrita e Oral (prática)"
                    },
                    new
                    {
                        Id = 29,
                        Description = "Diagramas, normalização e boas práticas de modelagem.",
                        Name = "Modelagem de Dados (conceitual e lógica)"
                    },
                    new
                    {
                        Id = 30,
                        Description = "Sintaxe essencial, manipulação de DOM e eventos.",
                        Name = "Noções de JavaScript (básico)"
                    });
            });

        modelBuilder.Entity("SkillPath.Domain.Entities.User", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("CurrentJob")
                    .IsRequired()
                    .HasMaxLength(160)
                    .HasColumnType("nvarchar(160)");

                b.Property<string>("EducationLevel")
                    .IsRequired()
                    .HasMaxLength(120)
                    .HasColumnType("nvarchar(120)");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasMaxLength(160)
                    .HasColumnType("nvarchar(160)");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(120)
                    .HasColumnType("nvarchar(120)");

                b.Property<string>("PasswordHash")
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnType("nvarchar(256)");

                b.Property<string>("TargetArea")
                    .IsRequired()
                    .HasMaxLength(160)
                    .HasColumnType("nvarchar(160)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.HasKey("Id");

                b.HasIndex("Email")
                    .IsUnique();

                b.ToTable("USER_APP");
            });

        modelBuilder.Entity("SkillPath.Domain.Entities.Course", b =>
            {
                b.HasOne("SkillPath.Domain.Entities.Skill", "Skill")
                    .WithMany("Courses")
                    .HasForeignKey("SkillId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Skill");
            });

        modelBuilder.Entity("SkillPath.Domain.Entities.Plan", b =>
            {
                b.HasOne("SkillPath.Domain.Entities.User", null)
                    .WithMany("Plans")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

        modelBuilder.Entity("SkillPath.Domain.Entities.PlanItem", b =>
            {
                b.HasOne("SkillPath.Domain.Entities.Plan", null)
                    .WithMany("Items")
                    .HasForeignKey("PlanId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("SkillPath.Domain.Entities.Skill", null)
                    .WithMany()
                    .HasForeignKey("SkillId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

        modelBuilder.Entity("SkillPath.Domain.Entities.Plan", b =>
            {
                b.Navigation("Items");
            });

        modelBuilder.Entity("SkillPath.Domain.Entities.Skill", b =>
            {
                b.Navigation("Courses");
            });

        modelBuilder.Entity("SkillPath.Domain.Entities.User", b =>
            {
                b.Navigation("Plans");
            });
#pragma warning restore 612, 618
    }
}




