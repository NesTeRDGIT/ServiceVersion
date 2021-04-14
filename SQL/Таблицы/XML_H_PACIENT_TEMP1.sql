-- Create table
create table XML_H_PACIENT_TEMP1
(
  pacient_id   NUMBER not null,
  zap_id       NUMBER,
  id_pac       VARCHAR2(36),
  vpolis       NUMBER(1),
  spolis       VARCHAR2(10),
  npolis       VARCHAR2(20),
  smo          VARCHAR2(5),
  smo_ogrn     VARCHAR2(15),
  smo_ok       VARCHAR2(5),
  smo_nam      VARCHAR2(100),
  novor        VARCHAR2(9),
  err          VARCHAR2(500),
  err_prim     VARCHAR2(1000),
  st_okato     VARCHAR2(5),
  vnov_d       NUMBER(4),
  smo_tfoms    VARCHAR2(5),
  vpolis_tfoms NUMBER(1),
  spolis_tfoms VARCHAR2(25),
  npolis_tfoms VARCHAR2(20),
  flag         VARCHAR2(2),
  flag1        NUMBER(1),
  num_tf       NUMBER,
  zap_tf       NUMBER
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
comment on table XML_H_PACIENT_TEMP1
  is '�������� � �������� XML';
-- Add comments to the columns 
comment on column XML_H_PACIENT_TEMP1.pacient_id
  is 'ID ��������';
comment on column XML_H_PACIENT_TEMP1.zap_id
  is '������� ���� �� ������� XML_H_ZAP';
comment on column XML_H_PACIENT_TEMP1.id_pac
  is '��� ������ � ��������';
comment on column XML_H_PACIENT_TEMP1.vpolis
  is '��� ���������, ��������������� ���� ����������� �� ���';
comment on column XML_H_PACIENT_TEMP1.spolis
  is '����� ���������, ��������������� ���� ����������� �� ���';
comment on column XML_H_PACIENT_TEMP1.npolis
  is '����� ���������, ��������������� ���� ����������� �� ���';
comment on column XML_H_PACIENT_TEMP1.smo
  is '���������� ����� ���';
comment on column XML_H_PACIENT_TEMP1.smo_ogrn
  is '���� ���';
comment on column XML_H_PACIENT_TEMP1.smo_ok
  is '����� ���������� �����������';
comment on column XML_H_PACIENT_TEMP1.smo_nam
  is '������������ ���';
comment on column XML_H_PACIENT_TEMP1.novor
  is '������� ��������������';
comment on column XML_H_PACIENT_TEMP1.err
  is '��� ������';
comment on column XML_H_PACIENT_TEMP1.err_prim
  is '�������� ������';
comment on column XML_H_PACIENT_TEMP1.st_okato
  is '������ �����������';
comment on column XML_H_PACIENT_TEMP1.vnov_d
  is '��� ��� ��������';
comment on column XML_H_PACIENT_TEMP1.smo_tfoms
  is '���������� ����� ��� �� ������ �����';
comment on column XML_H_PACIENT_TEMP1.vpolis_tfoms
  is '��� ��������� ��� �� ������ �����';
comment on column XML_H_PACIENT_TEMP1.spolis_tfoms
  is '����� ��������� ��� �� ������ �����';
comment on column XML_H_PACIENT_TEMP1.npolis_tfoms
  is '����� ��������� ��� �� ������ �����';
comment on column XML_H_PACIENT_TEMP1.num_tf
  is '����� ����� �� ���';
comment on column XML_H_PACIENT_TEMP1.zap_tf
  is '����� ������ � ������� �� ���';
-- Create/Recreate primary, unique and foreign key constraints 
alter table XML_H_PACIENT_TEMP1
  add constraint PK_XML_H_PACIENT_TEMP1 primary key (PACIENT_ID)
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