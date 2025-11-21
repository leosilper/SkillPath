using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillPath.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Criar sequences primeiro
        migrationBuilder.CreateSequence(
            name: "SEQ_SKILLS",
            startValue: 1L,
            incrementBy: 1);

        migrationBuilder.CreateSequence(
            name: "SEQ_COURSES",
            startValue: 1L,
            incrementBy: 1);

        migrationBuilder.CreateSequence(
            name: "SEQ_PLAN_ITEMS",
            startValue: 1L,
            incrementBy: 1);

        // Criar tabela SKILLS
        migrationBuilder.CreateTable(
            name: "SKILLS",
            columns: table => new
            {
                ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                NAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                DESCRIPTION = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SKILLS", x => x.ID);
            });

        // Criar tabela USER_APP
        migrationBuilder.CreateTable(
            name: "USER_APP",
            columns: table => new
            {
                ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                NAME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                EMAIL = table.Column<string>(type: "NVARCHAR2(160)", maxLength: 160, nullable: false),
                PASSWORD_HASH = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                CURRENT_JOB = table.Column<string>(type: "NVARCHAR2(160)", maxLength: 160, nullable: false),
                TARGET_AREA = table.Column<string>(type: "NVARCHAR2(160)", maxLength: 160, nullable: false),
                EDUCATION_LEVEL = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_USER_APP", x => x.ID);
            });

        // Criar tabela COURSES
        migrationBuilder.CreateTable(
            name: "COURSES",
            columns: table => new
            {
                ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                NAME = table.Column<string>(type: "NVARCHAR2(220)", maxLength: 220, nullable: false),
                PROVIDER = table.Column<string>(type: "NVARCHAR2(160)", maxLength: 160, nullable: false),
                URL = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false),
                SKILL_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_COURSES", x => x.ID);
                table.ForeignKey(
                    name: "FK_COURSES_SKILLS_SKILL_ID",
                    column: x => x.SKILL_ID,
                    principalTable: "SKILLS",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
            });

        // Criar tabela PLANS
        migrationBuilder.CreateTable(
            name: "PLANS",
            columns: table => new
            {
                ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                USER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                TITLE = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PLANS", x => x.ID);
                table.ForeignKey(
                    name: "FK_PLANS_USER_APP_USER_ID",
                    column: x => x.USER_ID,
                    principalTable: "USER_APP",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
            });

        // Criar tabela PLAN_ITEMS
        migrationBuilder.CreateTable(
            name: "PLAN_ITEMS",
            columns: table => new
            {
                ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                PLAN_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                SKILL_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                ORDER_NUMBER = table.Column<int>(type: "NUMBER(10)", nullable: false),
                IS_COMPLETED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                COMPLETED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PLAN_ITEMS", x => x.ID);
                table.ForeignKey(
                    name: "FK_PLAN_ITEMS_PLANS_PLAN_ID",
                    column: x => x.PLAN_ID,
                    principalTable: "PLANS",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PLAN_ITEMS_SKILLS_SKILL_ID",
                    column: x => x.SKILL_ID,
                    principalTable: "SKILLS",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
            });

        // Criar trigger para PLAN_ITEMS
        migrationBuilder.Sql(@"
            CREATE OR REPLACE TRIGGER TRG_PLAN_ITEMS_ID
            BEFORE INSERT ON ""PLAN_ITEMS""
            FOR EACH ROW
            BEGIN
                IF :NEW.""ID"" IS NULL THEN
                    SELECT SEQ_PLAN_ITEMS.NEXTVAL INTO :NEW.""ID"" FROM DUAL;
                END IF;
            END;");

        // Insert data - Skills
        migrationBuilder.InsertData(
            table: "SKILLS",
            columns: new[] { "ID", "NAME", "DESCRIPTION" },
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

        // Insert data - Courses
        migrationBuilder.InsertData(
            table: "COURSES",
            columns: new[] { "ID", "NAME", "PROVIDER", "URL", "SKILL_ID" },
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

        // Criar índices
        migrationBuilder.CreateIndex(
            name: "IX_COURSES_SKILL_ID",
            table: "COURSES",
            column: "SKILL_ID");

        migrationBuilder.CreateIndex(
            name: "IX_PLAN_ITEMS_PLAN_ID_ORDER",
            table: "PLAN_ITEMS",
            columns: new[] { "PLAN_ID", "ORDER_NUMBER" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PLAN_ITEMS_SKILL_ID",
            table: "PLAN_ITEMS",
            column: "SKILL_ID");

        migrationBuilder.CreateIndex(
            name: "IX_PLANS_USER_ID",
            table: "PLANS",
            column: "USER_ID");

        migrationBuilder.CreateIndex(
            name: "IX_PLANS_USER_ID_CREATED_AT",
            table: "PLANS",
            columns: new[] { "USER_ID", "CREATED_AT" });

        migrationBuilder.CreateIndex(
            name: "IX_USER_APP_EMAIL",
            table: "USER_APP",
            column: "EMAIL",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TRIGGER TRG_PLAN_ITEMS_ID");
        
        migrationBuilder.DropTable(name: "COURSES");
        migrationBuilder.DropTable(name: "PLAN_ITEMS");
        migrationBuilder.DropTable(name: "PLANS");
        migrationBuilder.DropTable(name: "SKILLS");
        migrationBuilder.DropTable(name: "USER_APP");

        migrationBuilder.DropSequence(name: "SEQ_PLAN_ITEMS");
        migrationBuilder.DropSequence(name: "SEQ_COURSES");
        migrationBuilder.DropSequence(name: "SEQ_SKILLS");
    }
}
