using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillShiftHub.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Skills",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Skills", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                CurrentJob = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                TargetArea = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                EducationLevel = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Courses",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                Provider = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                Url = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                SkillId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Courses", x => x.Id);
                table.ForeignKey(
                    name: "FK_Courses_Skills_SkillId",
                    column: x => x.SkillId,
                    principalTable: "Skills",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Plans",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Plans", x => x.Id);
                table.ForeignKey(
                    name: "FK_Plans_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PlanItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SkillId = table.Column<int>(type: "int", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PlanItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_PlanItems_Plans_PlanId",
                    column: x => x.PlanId,
                    principalTable: "Plans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PlanItems_Skills_SkillId",
                    column: x => x.SkillId,
                    principalTable: "Skills",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "Skills",
            columns: new[] { "Id", "Name", "Description" },
            values: new object[,]
            {
                { 1, "Lógica de Programação (iniciante)", "Fundamentos de algoritmos, estruturas condicionais e resolução de problemas." },
                { 2, "Excel Básico ao Intermediário", "Construção de planilhas, fórmulas, gráficos e tabelas dinâmicas." },
                { 3, "Introdução ao SQL (SELECT/WHERE/JOIN)", "Consultas básicas, filtros e relacionamentos entre tabelas." },
                { 4, "Git & GitHub (workflow)", "Trabalho com versionamento distribuído, branches e pull requests." },
                { 5, "HTML & CSS (fundamentos)", "Estruturação de páginas, semântica e princípios de responsividade." },
                { 6, "Pensamento Computacional", "Decomposição, padrões e criação de soluções sistemáticas." },
                { 7, "Excel para Logística (procX, dashboards)", "Funções avançadas, análises e visualização de dados logísticos." },
                { 8, "Fundamentos de Logística (estoque, armazenagem, WMS)", "Processos essenciais de cadeia de suprimentos e controle de estoque." },
                { 9, "ERP para Logística (cadastros, ordens, integrações)", "Integração de processos logísticos em sistemas corporativos." },
                { 10, "Indicadores de Operações (OTIF, lead time)", "Monitoramento e análise de métricas operacionais críticas." },
                { 11, "Planejamento de Rotas & Custos", "Estratégias para otimizar deslocamentos e reduzir custos logísticos." },
                { 12, "Power BI - do zero ao relatório", "Criação de dashboards, modelagem simples e publicação de relatórios." },
                { 13, "Comunicação & Atendimento (escuta ativa)", "Técnicas para relacionamento empático e profissional com clientes." },
                { 14, "Resolução de Conflitos", "Estratégias práticas para tratar objeções e situações difíceis." },
                { 15, "Noções de CRM (pipeline, cadência)", "Gestão de relacionamento com clientes e acompanhamento de funil." },
                { 16, "Vendas Consultivas (SPIN/GPCT)", "Métodos estruturados para diagnóstico e geração de valor em vendas." },
                { 17, "Excel para Comerciais - previsões e metas", "Construção de projeções, metas e acompanhamento de resultados de vendas." },
                { 18, "Escrita Profissional (e-mails claros)", "Comunicação escrita objetiva e alinhada ao contexto corporativo." },
                { 19, "Matemática Financeira Básica", "Juros simples e compostos, descontos e aplicações práticas." },
                { 20, "Excel Financeiro (projeções)", "Ferramentas de análise financeira e cenários em planilhas." },
                { 21, "Introdução a Crédito & Risco", "Conceitos de avaliação de crédito, risco e concessão responsável." },
                { 22, "Análise de Demonstrativos (noções)", "Leitura de DRE, Balanço Patrimonial e indicadores básicos." },
                { 23, "Power BI Financeiro", "Visualização de indicadores financeiros e storytelling com dados." },
                { 24, "Finanças Pessoais - Orçamento e dívida", "Organização financeira, metas e renegociação de dívidas." },
                { 25, "Gestão de Tempo & Produtividade Pessoal", "Priorização, planejamento semanal e técnicas de foco." },
                { 26, "Trabalho em Equipe & Colaboração", "Princípios de colaboração, feedback e alinhamento entre pares." },
                { 27, "Mentalidade de Aprendizado Contínuo", "Construção de planos de desenvolvimento e aprendizado ativo." },
                { 28, "Comunicação Escrita e Oral (prática)", "Clareza, objetividade e storytelling em apresentações e textos." },
                { 29, "Modelagem de Dados (conceitual e lógica)", "Diagramas, normalização e boas práticas de modelagem." },
                { 30, "Noções de JavaScript (básico)", "Sintaxe essencial, manipulação de DOM e eventos." }
            });

        migrationBuilder.InsertData(
            table: "Courses",
            columns: new[] { "Id", "Name", "Provider", "Url", "SkillId" },
            values: new object[,]
            {
                { 1, "Lógica de Programação com C# e Jogos", "Alura", "https://example.com/logica-iniciante", 1 },
                { 2, "Excel Essencial: do zero às tabelas dinâmicas", "Udemy", "https://example.com/excel-basico", 2 },
                { 3, "SQL para Análise de Dados", "Data Academy", "https://example.com/sql-fundamentos", 3 },
                { 4, "Git & GitHub na Prática", "DIO", "https://example.com/git-workflow", 4 },
                { 5, "HTML e CSS para Iniciantes", "Origamid", "https://example.com/html-css", 5 },
                { 6, "Pensamento Computacional para Todos", "Coursera", "https://example.com/pensamento-computacional", 6 },
                { 7, "Excel Avançado Aplicado à Logística", "FGV Online", "https://example.com/excel-logistica", 7 },
                { 8, "Operations Management Basics", "Coursera", "https://example.com/fund-logistica", 8 },
                { 9, "ERP Logístico na Prática", "SENAI", "https://example.com/erp-logistica", 9 },
                { 10, "KPIs de Operações: Monitoramento e Melhoria", "LinkedIn Learning", "https://example.com/kpis-operacoes", 10 },
                { 11, "Roteirização e Custos Logísticos", "LogSchool", "https://example.com/rotas-custos", 11 },
                { 12, "Power BI: Dashboards Profissionais", "Microsoft Learn", "https://example.com/powerbi-basico", 12 },
                { 13, "Atendimento ao Cliente com Escuta Ativa", "SEBRAE", "https://example.com/atendimento-escuta", 13 },
                { 14, "Gestão de Conflitos no Atendimento", "Udemy", "https://example.com/resolucao-conflitos", 14 },
                { 15, "CRM e Jornada do Cliente", "RD Station", "https://example.com/crm-jornada", 15 },
                { 16, "Vendas Consultivas com SPIN Selling", "Fundação Dom Cabral", "https://example.com/vendas-consultivas", 16 },
                { 17, "Excel para Forecast de Vendas", "G4 Educação", "https://example.com/excel-comercial", 17 },
                { 18, "Escrita Corporativa Objetiva", "PUC Minas", "https://example.com/escrita-profissional", 18 },
                { 19, "Matemática Financeira Aplicada", "Coursera", "https://example.com/matematica-financeira", 19 },
                { 20, "Excel Financeiro com TIR e VPL", "Udemy", "https://example.com/excel-financeiro", 20 },
                { 21, "Introdução a Crédito e Cobrança", "Serasa Experian", "https://example.com/credito-risco", 21 },
                { 22, "Leitura de Demonstrativos Financeiros", "B3 Educação", "https://example.com/analise-demonstrativos", 22 },
                { 23, "Power BI para Finanças", "FIAP", "https://example.com/powerbi-financeiro", 23 },
                { 24, "Educação Financeira Pessoal", "Banco Central do Brasil", "https://example.com/financas-pessoais", 24 },
                { 25, "Produtividade e Prioridades", "GetAbstract", "https://example.com/gestao-tempo", 25 },
                { 26, "Colaboração e Trabalho em Equipe", "Google Skillshop", "https://example.com/trabalho-equipe", 26 },
                { 27, "Lifelong Learning na Prática", "LinkedIn Learning", "https://example.com/aprendizado-continuo", 27 },
                { 28, "Comunicação Escrita e Oral para Profissionais", "SENAC", "https://example.com/comunicacao-pratica", 28 },
                { 29, "Modelagem de Dados Essencial", "Data University", "https://example.com/modelagem-dados", 29 },
                { 30, "JavaScript para Iniciantes", "freeCodeCamp", "https://example.com/js-basico", 30 }
            });

        migrationBuilder.CreateIndex(
            name: "IX_Courses_SkillId",
            table: "Courses",
            column: "SkillId");

        migrationBuilder.CreateIndex(
            name: "IX_PlanItems_PlanId_Order",
            table: "PlanItems",
            columns: new[] { "PlanId", "Order" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PlanItems_SkillId",
            table: "PlanItems",
            column: "SkillId");

        migrationBuilder.CreateIndex(
            name: "IX_Plans_UserId",
            table: "Plans",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Plans_UserId_CreatedAt",
            table: "Plans",
            columns: new[] { "UserId", "CreatedAt" });

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Courses");

        migrationBuilder.DropTable(
            name: "PlanItems");

        migrationBuilder.DropTable(
            name: "Plans");

        migrationBuilder.DropTable(
            name: "Skills");

        migrationBuilder.DropTable(
            name: "Users");
    }
}




