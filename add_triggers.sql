-- Script para adicionar triggers de geração automática de ID para SKILLS, COURSES e PLAN_ITEMS
-- Execute este script diretamente no banco de dados Oracle

-- Ajustar sequence SEQ_SKILLS para o próximo valor disponível
-- (baseado no máximo ID atual + 1)
DECLARE
    v_max_id NUMBER;
    v_current_seq NUMBER;
    v_increment NUMBER;
BEGIN
    -- Obter o máximo ID atual na tabela
    SELECT NVL(MAX("ID"), 0) INTO v_max_id FROM "SKILLS";
    
    -- Obter o valor atual da sequence (isso vai incrementar, mas vamos ajustar depois)
    SELECT SEQ_SKILLS.NEXTVAL INTO v_current_seq FROM DUAL;
    
    -- Se a sequence está atrás do máximo ID, ajustar
    IF v_current_seq <= v_max_id THEN
        -- Calcular quantos valores precisamos pular
        v_increment := v_max_id - v_current_seq + 1;
        -- Alterar o incremento temporariamente e avançar a sequence
        EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_SKILLS INCREMENT BY ' || v_increment;
        SELECT SEQ_SKILLS.NEXTVAL INTO v_current_seq FROM DUAL;
        -- Restaurar o incremento para 1
        EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_SKILLS INCREMENT BY 1';
    END IF;
END;
/

-- Ajustar sequence SEQ_COURSES para o próximo valor disponível
DECLARE
    v_max_id NUMBER;
    v_current_seq NUMBER;
    v_increment NUMBER;
BEGIN
    -- Obter o máximo ID atual na tabela
    SELECT NVL(MAX("ID"), 0) INTO v_max_id FROM "COURSES";
    
    -- Obter o valor atual da sequence (isso vai incrementar, mas vamos ajustar depois)
    SELECT SEQ_COURSES.NEXTVAL INTO v_current_seq FROM DUAL;
    
    -- Se a sequence está atrás do máximo ID, ajustar
    IF v_current_seq <= v_max_id THEN
        -- Calcular quantos valores precisamos pular
        v_increment := v_max_id - v_current_seq + 1;
        -- Alterar o incremento temporariamente e avançar a sequence
        EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_COURSES INCREMENT BY ' || v_increment;
        SELECT SEQ_COURSES.NEXTVAL INTO v_current_seq FROM DUAL;
        -- Restaurar o incremento para 1
        EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_COURSES INCREMENT BY 1';
    END IF;
END;
/

-- Trigger para SKILLS
CREATE OR REPLACE TRIGGER TRG_SKILLS_ID
BEFORE INSERT ON "SKILLS"
FOR EACH ROW
BEGIN
    IF :NEW."ID" IS NULL THEN
        SELECT SEQ_SKILLS.NEXTVAL INTO :NEW."ID" FROM DUAL;
    END IF;
END;
/

-- Ajustar sequence SEQ_PLAN_ITEMS para o próximo valor disponível
DECLARE
    v_max_id NUMBER;
    v_current_seq NUMBER;
    v_increment NUMBER;
BEGIN
    -- Obter o máximo ID atual na tabela
    SELECT NVL(MAX("ID"), 0) INTO v_max_id FROM "PLAN_ITEMS";
    
    -- Obter o valor atual da sequence (isso vai incrementar, mas vamos ajustar depois)
    SELECT SEQ_PLAN_ITEMS.NEXTVAL INTO v_current_seq FROM DUAL;
    
    -- Se a sequence está atrás do máximo ID, ajustar
    IF v_current_seq <= v_max_id THEN
        -- Calcular quantos valores precisamos pular
        v_increment := v_max_id - v_current_seq + 1;
        -- Alterar o incremento temporariamente e avançar a sequence
        EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_PLAN_ITEMS INCREMENT BY ' || v_increment;
        SELECT SEQ_PLAN_ITEMS.NEXTVAL INTO v_current_seq FROM DUAL;
        -- Restaurar o incremento para 1
        EXECUTE IMMEDIATE 'ALTER SEQUENCE SEQ_PLAN_ITEMS INCREMENT BY 1';
    END IF;
END;
/

-- Trigger para COURSES
CREATE OR REPLACE TRIGGER TRG_COURSES_ID
BEFORE INSERT ON "COURSES"
FOR EACH ROW
BEGIN
    IF :NEW."ID" IS NULL THEN
        SELECT SEQ_COURSES.NEXTVAL INTO :NEW."ID" FROM DUAL;
    END IF;
END;
/

-- Trigger para PLAN_ITEMS (já existe, mas vamos garantir que está atualizado)
CREATE OR REPLACE TRIGGER TRG_PLAN_ITEMS_ID
BEFORE INSERT ON "PLAN_ITEMS"
FOR EACH ROW
BEGIN
    IF :NEW."ID" IS NULL THEN
        SELECT SEQ_PLAN_ITEMS.NEXTVAL INTO :NEW."ID" FROM DUAL;
    END IF;
END;
/

