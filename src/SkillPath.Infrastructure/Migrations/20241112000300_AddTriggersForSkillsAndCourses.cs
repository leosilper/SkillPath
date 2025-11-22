using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillPath.Infrastructure.Migrations;

public partial class AddTriggersForSkillsAndCourses : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Ajustar sequence SEQ_SKILLS para o próximo valor disponível
        migrationBuilder.Sql(@"
            DECLARE
                v_max_id NUMBER;
                v_current_seq NUMBER;
                v_increment NUMBER;
            BEGIN
                SELECT NVL(MAX(""ID""), 0) INTO v_max_id FROM ""SKILLS"";
                SELECT SEQ_SKILLS.NEXTVAL INTO v_current_seq FROM DUAL;
                IF v_current_seq <= v_max_id THEN
                    v_increment := v_max_id - v_current_seq + 1;
                    EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_SKILLS INCREMENT BY ' || v_increment;
                    SELECT SEQ_SKILLS.NEXTVAL INTO v_current_seq FROM DUAL;
                    EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_SKILLS INCREMENT BY 1';
                END IF;
            END;");

        // Ajustar sequence SEQ_COURSES para o próximo valor disponível
        migrationBuilder.Sql(@"
            DECLARE
                v_max_id NUMBER;
                v_current_seq NUMBER;
                v_increment NUMBER;
            BEGIN
                SELECT NVL(MAX(""ID""), 0) INTO v_max_id FROM ""COURSES"";
                SELECT SEQ_COURSES.NEXTVAL INTO v_current_seq FROM DUAL;
                IF v_current_seq <= v_max_id THEN
                    v_increment := v_max_id - v_current_seq + 1;
                    EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_COURSES INCREMENT BY ' || v_increment;
                    SELECT SEQ_COURSES.NEXTVAL INTO v_current_seq FROM DUAL;
                    EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_COURSES INCREMENT BY 1';
                END IF;
            END;");

        // Criar trigger para SKILLS
        migrationBuilder.Sql(@"
            CREATE OR REPLACE TRIGGER TRG_SKILLS_ID
            BEFORE INSERT ON ""SKILLS""
            FOR EACH ROW
            BEGIN
                IF :NEW.""ID"" IS NULL THEN
                    SELECT SEQ_SKILLS.NEXTVAL INTO :NEW.""ID"" FROM DUAL;
                END IF;
            END;");

        // Ajustar sequence SEQ_PLAN_ITEMS para o próximo valor disponível
        migrationBuilder.Sql(@"
            DECLARE
                v_max_id NUMBER;
                v_current_seq NUMBER;
                v_increment NUMBER;
            BEGIN
                SELECT NVL(MAX(""ID""), 0) INTO v_max_id FROM ""PLAN_ITEMS"";
                SELECT SEQ_PLAN_ITEMS.NEXTVAL INTO v_current_seq FROM DUAL;
                IF v_current_seq <= v_max_id THEN
                    v_increment := v_max_id - v_current_seq + 1;
                    EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_PLAN_ITEMS INCREMENT BY ' || v_increment;
                    SELECT SEQ_PLAN_ITEMS.NEXTVAL INTO v_current_seq FROM DUAL;
                    EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_PLAN_ITEMS INCREMENT BY 1';
                END IF;
            END;");

        // Criar trigger para COURSES
        migrationBuilder.Sql(@"
            CREATE OR REPLACE TRIGGER TRG_COURSES_ID
            BEFORE INSERT ON ""COURSES""
            FOR EACH ROW
            BEGIN
                IF :NEW.""ID"" IS NULL THEN
                    SELECT SEQ_COURSES.NEXTVAL INTO :NEW.""ID"" FROM DUAL;
                END IF;
            END;");

        // Garantir que o trigger para PLAN_ITEMS está atualizado
        migrationBuilder.Sql(@"
            CREATE OR REPLACE TRIGGER TRG_PLAN_ITEMS_ID
            BEFORE INSERT ON ""PLAN_ITEMS""
            FOR EACH ROW
            BEGIN
                IF :NEW.""ID"" IS NULL THEN
                    SELECT SEQ_PLAN_ITEMS.NEXTVAL INTO :NEW.""ID"" FROM DUAL;
                END IF;
            END;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TRIGGER TRG_PLAN_ITEMS_ID");
        migrationBuilder.Sql("DROP TRIGGER TRG_COURSES_ID");
        migrationBuilder.Sql("DROP TRIGGER TRG_SKILLS_ID");
    }
}

