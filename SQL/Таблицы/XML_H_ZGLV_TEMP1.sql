-- Create table
create table XML_H_ZGLV_TEMP1
(
  zglv_id      NUMBER not null,
  version      VARCHAR2(5),
  data         DATE,
  filename     VARCHAR2(26),
  err          VARCHAR2(60),
  err_prim     VARCHAR2(500),
  comment_zglv VARCHAR2(500)
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
comment on table XML_H_ZGLV_TEMP1
  is 'Заголовок файла XML';
-- Add comments to the columns 
comment on column XML_H_ZGLV_TEMP1.zglv_id
  is 'ID заголовка';
comment on column XML_H_ZGLV_TEMP1.version
  is 'Версия формата взаимодействия';
comment on column XML_H_ZGLV_TEMP1.data
  is 'Дата формирования файла';
comment on column XML_H_ZGLV_TEMP1.filename
  is 'Имя файла без расширения';
comment on column XML_H_ZGLV_TEMP1.err
  is 'Код ошибки';
comment on column XML_H_ZGLV_TEMP1.err_prim
  is 'Описание ошибки';
comment on column XML_H_ZGLV_TEMP1.comment_zglv
  is 'Комментарий';
-- Create/Recreate primary, unique and foreign key constraints 
alter table XML_H_ZGLV_TEMP1
  add constraint PK_XML_H_ZGLV_ID_TEMP1 primary key (ZGLV_ID)
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