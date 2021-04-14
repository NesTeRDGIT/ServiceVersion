-- Create table
create table XML_H_SLUCH_TEMP99
(
  sluch_id    NUMBER not null,
  pacient_id  NUMBER,
  zap_id      NUMBER,
  idcase      NUMBER(11),
  usl_ok      NUMBER(2),
  vidpom      NUMBER(4),
  npr_mo      VARCHAR2(6),
  extr        NUMBER(2),
  lpu         VARCHAR2(6),
  lpu_1       VARCHAR2(8),
  podr        NUMBER(8),
  profil      NUMBER(3),
  det         NUMBER(1),
  nhistory    VARCHAR2(50),
  date_1      DATE,
  date_2      DATE,
  ds0         VARCHAR2(10),
  ds1         VARCHAR2(10),
  ds2         VARCHAR2(10),
  code_mes1   VARCHAR2(20),
  code_mes2   VARCHAR2(20),
  rslt        NUMBER(3),
  ishod       NUMBER(3),
  prvs        NUMBER(4),
  iddokt      VARCHAR2(25),
  os_sluch    NUMBER(1),
  idsp        NUMBER(2),
  ed_col      NUMBER(5,2),
  tarif       NUMBER(15,2),
  sumv        NUMBER(15,2),
  oplata      NUMBER(1),
  sump        NUMBER(15,2),
  refreason   NUMBER(3),
  sank_mek    NUMBER(15,2),
  sank_mee    NUMBER(15,2),
  sank_ekmp   NUMBER(15,2),
  comentsl    VARCHAR2(250),
  err         VARCHAR2(500),
  err_prim    VARCHAR2(1500),
  sank_it     NUMBER(15,2),
  for_pom     NUMBER(1),
  vers_spec   VARCHAR2(4),
  vnov_m      VARCHAR2(4),
  ds3         VARCHAR2(10),
  rslt_d      NUMBER(3),
  vid_hmp     VARCHAR2(9),
  metod_hmp   NUMBER(3),
  p_otk       NUMBER(1),
  tarif_tfoms NUMBER(15,2),
  sumv_tfoms  NUMBER(15,2),
  num_tf      NUMBER,
  idcase_tf   NUMBER
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
comment on table XML_H_SLUCH_TEMP99
  is '�������� � ������ XML';
-- Add comments to the columns 
comment on column XML_H_SLUCH_TEMP99.sluch_id
  is 'ID ������';
comment on column XML_H_SLUCH_TEMP99.pacient_id
  is '������� ���� �� ������� XML_H_PACIENT(���� �� ������������)';
comment on column XML_H_SLUCH_TEMP99.zap_id
  is '������� ���� �� ������� XML_H_ZAP';
comment on column XML_H_SLUCH_TEMP99.idcase
  is '����� ������ � ������� �������';
comment on column XML_H_SLUCH_TEMP99.usl_ok
  is '������� �������� ����������� ������';
comment on column XML_H_SLUCH_TEMP99.vidpom
  is '��� ������';
comment on column XML_H_SLUCH_TEMP99.npr_mo
  is '��� ��, ������������ �� ������� (�����������, ������������)';
comment on column XML_H_SLUCH_TEMP99.extr
  is '����������� (��������������)';
comment on column XML_H_SLUCH_TEMP99.lpu
  is '��� ��';
comment on column XML_H_SLUCH_TEMP99.lpu_1
  is '������������� ��';
comment on column XML_H_SLUCH_TEMP99.podr
  is '��� ���������';
comment on column XML_H_SLUCH_TEMP99.profil
  is '�������';
comment on column XML_H_SLUCH_TEMP99.det
  is '������� �������� �������';
comment on column XML_H_SLUCH_TEMP99.nhistory
  is '����� ������� �������/������ ������������� ��������';
comment on column XML_H_SLUCH_TEMP99.date_1
  is '���� ������ �������';
comment on column XML_H_SLUCH_TEMP99.date_2
  is '���� ��������� �������';
comment on column XML_H_SLUCH_TEMP99.ds0
  is '������� ���������';
comment on column XML_H_SLUCH_TEMP99.ds1
  is '������� ��������';
comment on column XML_H_SLUCH_TEMP99.ds2
  is '������� �������������� �����������';
comment on column XML_H_SLUCH_TEMP99.code_mes1
  is '��� ���';
comment on column XML_H_SLUCH_TEMP99.code_mes2
  is '��� ��� �������������� �����������';
comment on column XML_H_SLUCH_TEMP99.rslt
  is '��������� ���������/��������������';
comment on column XML_H_SLUCH_TEMP99.ishod
  is '����� �����������';
comment on column XML_H_SLUCH_TEMP99.prvs
  is '������������� �������� �����/�����. ���������� �����';
comment on column XML_H_SLUCH_TEMP99.iddokt
  is '��� �����, ���������� �����/������� �������';
comment on column XML_H_SLUCH_TEMP99.os_sluch
  is '������� "������ ������" ��� ����������� ��������� �� ����������� �������';
comment on column XML_H_SLUCH_TEMP99.idsp
  is '��� ������� ������ ����������� ������';
comment on column XML_H_SLUCH_TEMP99.ed_col
  is '���������� ������ ������ ����������� ������';
comment on column XML_H_SLUCH_TEMP99.tarif
  is '�����';
comment on column XML_H_SLUCH_TEMP99.sumv
  is '�����, ������������ � ������';
comment on column XML_H_SLUCH_TEMP99.oplata
  is '��� ������';
comment on column XML_H_SLUCH_TEMP99.sump
  is '�����, �������� � ������ ���';
comment on column XML_H_SLUCH_TEMP99.refreason
  is '��� ������� ������ (���������) ������';
comment on column XML_H_SLUCH_TEMP99.sank_mek
  is '���������� ������� (���)';
comment on column XML_H_SLUCH_TEMP99.sank_mee
  is '���������� ������� (���)';
comment on column XML_H_SLUCH_TEMP99.sank_ekmp
  is '���������� ������� (����)';
comment on column XML_H_SLUCH_TEMP99.comentsl
  is '��������� ����';
comment on column XML_H_SLUCH_TEMP99.err
  is '��� ������';
comment on column XML_H_SLUCH_TEMP99.err_prim
  is '�������� ������';
comment on column XML_H_SLUCH_TEMP99.sank_it
  is '����� ������� �� ������';
comment on column XML_H_SLUCH_TEMP99.for_pom
  is '������������� ���� �������� �������-���� ������. ����-������ V014 ';
comment on column XML_H_SLUCH_TEMP99.vers_spec
  is '��� ������������-������������� ��������������';
comment on column XML_H_SLUCH_TEMP99.vnov_m
  is '��� ��� ��������';
comment on column XML_H_SLUCH_TEMP99.ds3
  is '������� �������-��� �����������';
comment on column XML_H_SLUCH_TEMP99.rslt_d
  is '��������� ��������������� (V017)';
comment on column XML_H_SLUCH_TEMP99.p_otk
  is '����� �� ���������������';
comment on column XML_H_SLUCH_TEMP99.tarif_tfoms
  is '����� (������ �����)';
comment on column XML_H_SLUCH_TEMP99.sumv_tfoms
  is '����� (������ �����)';
comment on column XML_H_SLUCH_TEMP99.num_tf
  is '����� ����� �� ���';
comment on column XML_H_SLUCH_TEMP99.idcase_tf
  is '����� ������ � ������� �������  �� ���';
-- Create/Recreate primary, unique and foreign key constraints 
alter table XML_H_SLUCH_TEMP99
  add constraint PK_SLUCH_TEMP99 primary key (SLUCH_ID)
  using index 
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
-- Create/Recreate indexes 
create index IND_SLUCH_TEMP_PACIENT1 on XML_H_SLUCH_TEMP99 (PACIENT_ID)
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
create index IND_SLUCH_TEMP_ZAP1 on XML_H_SLUCH_TEMP99 (ZAP_ID)
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