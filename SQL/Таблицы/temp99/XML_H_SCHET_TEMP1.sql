-- Create table
create table XML_H_SCHET_TEMP99
(
  schet_id     NUMBER not null,
  zglv_id      NUMBER,
  code         NUMBER(8),
  code_mo      VARCHAR2(6),
  year         NUMBER(4),
  month        NUMBER(2),
  nschet       VARCHAR2(15),
  dschet       DATE,
  plat         VARCHAR2(5),
  summav       NUMBER(15,2),
  coments      VARCHAR2(250),
  summap       NUMBER(15,2),
  sank_mek     NUMBER(15,2),
  sank_mee     NUMBER(15,2),
  sank_empk    NUMBER(15,2),
  err          VARCHAR2(300),
  err_prim     VARCHAR2(750),
  disp         VARCHAR2(3),
  summav_tfoms NUMBER(15,2)
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
comment on table XML_H_SCHET_TEMP99
  is '�������� ����� XML';
-- Add comments to the columns 
comment on column XML_H_SCHET_TEMP99.schet_id
  is '������������� �����';
comment on column XML_H_SCHET_TEMP99.zglv_id
  is '������� ���� �� ������� XML_H_ZGLV';
comment on column XML_H_SCHET_TEMP99.code
  is '��� ������ �����';
comment on column XML_H_SCHET_TEMP99.code_mo
  is '���������� ����� ����������� �����������';
comment on column XML_H_SCHET_TEMP99.year
  is '�������� ���';
comment on column XML_H_SCHET_TEMP99.month
  is '�������� �����';
comment on column XML_H_SCHET_TEMP99.nschet
  is '����� �����';
comment on column XML_H_SCHET_TEMP99.dschet
  is '���� ����������� �����';
comment on column XML_H_SCHET_TEMP99.plat
  is '����������. ���������� ����� ���';
comment on column XML_H_SCHET_TEMP99.summav
  is '����� ��, ������������ �� ������';
comment on column XML_H_SCHET_TEMP99.coments
  is '��������� ����';
comment on column XML_H_SCHET_TEMP99.summap
  is '�����, �������� � ������ ���';
comment on column XML_H_SCHET_TEMP99.sank_mek
  is '���������� ������� (���)';
comment on column XML_H_SCHET_TEMP99.sank_mee
  is '���������� ������� (���)';
comment on column XML_H_SCHET_TEMP99.sank_empk
  is '���������� ������� (����)';
comment on column XML_H_SCHET_TEMP99.err
  is '��� ������';
comment on column XML_H_SCHET_TEMP99.err_prim
  is '�������� ������';
comment on column XML_H_SCHET_TEMP99.disp
  is '��� ��������������� (V016)';
comment on column XML_H_SCHET_TEMP99.summav_tfoms
  is '����� �����';
-- Create/Recreate primary, unique and foreign key constraints 
alter table XML_H_SCHET_TEMP99
  add constraint PK_XML_H_SCHET_ID_TEMP99 primary key (SCHET_ID)
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