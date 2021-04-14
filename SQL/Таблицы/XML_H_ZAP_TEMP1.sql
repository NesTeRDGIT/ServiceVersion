-- Create table
create table XML_H_ZAP_TEMP1
(
  zap_id   NUMBER not null,
  schet_id NUMBER,
  n_zap    NUMBER(8),
  pr_nov   NUMBER(1),
  err      VARCHAR2(25),
  err_prim VARCHAR2(350)
)
tablespace MEDPOM
  pctfree 10
  initrans 1
  maxtrans 255
  storage
  (
    initial 16K
    minextents 1
    maxextents unlimited
  );
-- Add comments to the table 
comment on table XML_H_ZAP_TEMP1
  is 'Сведения о записи ';
-- Add comments to the columns 
comment on column XML_H_ZAP_TEMP1.zap_id
  is 'ID записи';
comment on column XML_H_ZAP_TEMP1.schet_id
  is 'Внешний ключ на таблицу XML_H_SCHET';
comment on column XML_H_ZAP_TEMP1.n_zap
  is 'Номер позиции записи';
comment on column XML_H_ZAP_TEMP1.pr_nov
  is 'Признак исправленной записи';
comment on column XML_H_ZAP_TEMP1.err
  is 'Код ошибки';
comment on column XML_H_ZAP_TEMP1.err_prim
  is 'Описание ошибки';
-- Create/Recreate primary, unique and foreign key constraints 
alter table XML_H_ZAP_TEMP1
  add constraint PK_XML_H_ZAP_ID_TEMP1 primary key (ZAP_ID)
  using index 
  tablespace USERS
  pctfree 10
  initrans 2
  maxtrans 255
  storage
  (
    initial 64K
    minextents 1
    maxextents unlimited
  );
-- Create/Recreate indexes 
create index IND_ZAP_TEMP_SCHET1 on XML_H_ZAP_TEMP1 (SCHET_ID)
  tablespace MEDPOM
  pctfree 10
  initrans 2
  maxtrans 255
  storage
  (
    initial 64K
    minextents 1
    maxextents unlimited
  );