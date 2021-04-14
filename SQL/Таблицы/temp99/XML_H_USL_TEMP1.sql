-- Create table
create table XML_H_USL_TEMP99
(
  usl_id         NUMBER not null,
  sluch_id       NUMBER,
  idserv         NVARCHAR2(36),
  lpu            VARCHAR2(6),
  lpu_1          VARCHAR2(8),
  podr           NUMBER(8),
  profil         NUMBER(3),
  det            NUMBER(1),
  date_in        DATE,
  date_out       DATE,
  ds             VARCHAR2(10),
  code_usl       VARCHAR2(20),
  kol_usl        NUMBER(6,2),
  tarif          NUMBER(15,2),
  sumv_usl       NUMBER(15,2),
  prvs           NUMBER(9),
  code_md        VARCHAR2(25),
  comentu        VARCHAR2(250),
  err            VARCHAR2(500),
  err_prim       VARCHAR2(1000),
  refreason      NUMBER(3),
  idserv_tf      NUMBER,
  num_tf         NUMBER,
  idcase_tf      NUMBER,
  fond           NUMBER(1),
  tarif_tfoms    NUMBER(15,2),
  sumv_usl_tfoms NUMBER(15,2),
  vid_vme        VARCHAR2(15)
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
comment on table XML_H_USL_TEMP99
  is '�������� �� ������ XML';
-- Add comments to the columns 
comment on column XML_H_USL_TEMP99.usl_id
  is 'ID ������';
comment on column XML_H_USL_TEMP99.sluch_id
  is '������� ���� �� ������� XML_H_SLUCH';
comment on column XML_H_USL_TEMP99.idserv
  is '����� ������ � ������� �����';
comment on column XML_H_USL_TEMP99.lpu
  is '��� ��';
comment on column XML_H_USL_TEMP99.lpu_1
  is '������������� ��';
comment on column XML_H_USL_TEMP99.podr
  is '��� ���������';
comment on column XML_H_USL_TEMP99.profil
  is '�������';
comment on column XML_H_USL_TEMP99.det
  is '������� �������� ����������';
comment on column XML_H_USL_TEMP99.date_in
  is '���� ������ ��������� ������';
comment on column XML_H_USL_TEMP99.date_out
  is '���� ��������� �������� ������';
comment on column XML_H_USL_TEMP99.ds
  is '�������';
comment on column XML_H_USL_TEMP99.code_usl
  is '��� ������';
comment on column XML_H_USL_TEMP99.kol_usl
  is '���������� ������ (��������� ������)';
comment on column XML_H_USL_TEMP99.tarif
  is '�����';
comment on column XML_H_USL_TEMP99.sumv_usl
  is '���������, ����������� ������, ������������ � ������ (���)';
comment on column XML_H_USL_TEMP99.prvs
  is '������������� ������������, ������������ ������';
comment on column XML_H_USL_TEMP99.code_md
  is '��� ������������ ���������, ���������� ����������� ������';
comment on column XML_H_USL_TEMP99.comentu
  is '��������� ����';
comment on column XML_H_USL_TEMP99.err
  is '��� ������';
comment on column XML_H_USL_TEMP99.err_prim
  is '�������� ������';
comment on column XML_H_USL_TEMP99.fond
  is '������� ������ ������ 1 - � �������������; 2 - ��� �������������';
comment on column XML_H_USL_TEMP99.tarif_tfoms
  is '����� (������ �����)';
comment on column XML_H_USL_TEMP99.sumv_usl_tfoms
  is '��������� (������ �����)';
comment on column XML_H_USL_TEMP99.vid_vme
  is '��� ������������ �������������(V001)';
-- Create/Recreate primary, unique and foreign key constraints 
alter table XML_H_USL_TEMP99
  add constraint PK_XML_H_USL_ID_TEMP99 primary key (USL_ID)
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
create index IND_USL_TEMP_1 on XML_H_USL_TEMP99 (SLUCH_ID)
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