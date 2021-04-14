-- Create table
create table XML_L_ZGLV_TEMP99
(
  zglv_id      NUMBER not null,
  version      VARCHAR2(5),
  data         DATE,
  filename     VARCHAR2(26),
  filename1    VARCHAR2(26),
  err          VARCHAR2(35),
  err_prim     VARCHAR2(350),
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
comment on table XML_L_ZGLV_TEMP99
  is '��������� ����� � ������������� ������� XML';
-- Add comments to the columns 
comment on column XML_L_ZGLV_TEMP99.zglv_id
  is 'ID ���������';
comment on column XML_L_ZGLV_TEMP99.version
  is '������ ������� ��������������';
comment on column XML_L_ZGLV_TEMP99.data
  is '���� ������������ �����';
comment on column XML_L_ZGLV_TEMP99.filename
  is '��� ����� ��� ����������';
comment on column XML_L_ZGLV_TEMP99.filename1
  is '��� �����, � ������� ������ ������ ����, ��� ����������';
comment on column XML_L_ZGLV_TEMP99.err
  is '��� ������';
comment on column XML_L_ZGLV_TEMP99.err_prim
  is '�������� ������';
comment on column XML_L_ZGLV_TEMP99.comment_zglv
  is '����������';
-- Create/Recreate primary, unique and foreign key constraints 
alter table XML_L_ZGLV_TEMP99
  add constraint PK_XML_L_ZGLV_ID_TEMP99 primary key (ZGLV_ID)
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