-- Create table
create table XML_L_PERS_TEMP1
(
  pers_id   NUMBER not null,
  zglv_id   NUMBER not null,
  id_pac    VARCHAR2(36),
  fam       VARCHAR2(40),
  im        VARCHAR2(40),
  ot        VARCHAR2(40),
  w         NUMBER(1),
  dr        DATE,
  fam_p     VARCHAR2(40),
  im_p      VARCHAR2(40),
  ot_p      VARCHAR2(40),
  w_p       NUMBER(1),
  dr_p      DATE,
  mr        VARCHAR2(100),
  doctype   VARCHAR2(2),
  docser    VARCHAR2(10),
  docnum    VARCHAR2(20),
  snils     VARCHAR2(20),
  okatog    VARCHAR2(11),
  okatop    VARCHAR2(11),
  comentp   VARCHAR2(250),
  err       VARCHAR2(100),
  err_prim  VARCHAR2(1000),
  dost      VARCHAR2(25),
  dost_p    VARCHAR2(25),
  fam_tfoms VARCHAR2(40),
  im_tfoms  VARCHAR2(40),
  ot_tfoms  VARCHAR2(40),
  dr_tfoms  DATE,
  flag      VARCHAR2(2),
  rokato    VARCHAR2(5),
  renp      VARCHAR2(16),
  rqogrn    VARCHAR2(13),
  rdbeg     DATE
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
comment on table XML_L_PERS_TEMP1
  is '�������� ������������ ������ ��������� XML';
-- Add comments to the columns 
comment on column XML_L_PERS_TEMP1.pers_id
  is '������������� ��������';
comment on column XML_L_PERS_TEMP1.zglv_id
  is '������� ���� �� ������� XML_L_ZGLV';
comment on column XML_L_PERS_TEMP1.id_pac
  is '��� ������ � ��������';
comment on column XML_L_PERS_TEMP1.fam
  is '������� ��������';
comment on column XML_L_PERS_TEMP1.im
  is '��� ��������';
comment on column XML_L_PERS_TEMP1.ot
  is '�������� ��������';
comment on column XML_L_PERS_TEMP1.w
  is '��� ��������';
comment on column XML_L_PERS_TEMP1.dr
  is '���� ��������';
comment on column XML_L_PERS_TEMP1.fam_p
  is '�������  ������������� ��������';
comment on column XML_L_PERS_TEMP1.im_p
  is '���  ������������� ��������';
comment on column XML_L_PERS_TEMP1.ot_p
  is '��������  ������������� ��������';
comment on column XML_L_PERS_TEMP1.w_p
  is '���  ������������� ��������';
comment on column XML_L_PERS_TEMP1.dr_p
  is '���� ��������  ������������� ��������';
comment on column XML_L_PERS_TEMP1.mr
  is '����� ��������';
comment on column XML_L_PERS_TEMP1.doctype
  is '��� ���������, ��������������� �������� �������� ��� �������������';
comment on column XML_L_PERS_TEMP1.docser
  is '����� ���������, ��������������� �������� �������� ��� �������������';
comment on column XML_L_PERS_TEMP1.docnum
  is '����� ���������, ��������������� �������� �������� ��� �������������';
comment on column XML_L_PERS_TEMP1.snils
  is '�����';
comment on column XML_L_PERS_TEMP1.okatog
  is '��� ����� ���������� �� �����';
comment on column XML_L_PERS_TEMP1.okatop
  is '��� ����� ���������� �� �����';
comment on column XML_L_PERS_TEMP1.comentp
  is '��������� ����';
comment on column XML_L_PERS_TEMP1.err
  is '��� ������';
comment on column XML_L_PERS_TEMP1.err_prim
  is '�������� ������';
comment on column XML_L_PERS_TEMP1.fam_tfoms
  is '������� �������� �� ������ �����';
comment on column XML_L_PERS_TEMP1.im_tfoms
  is '��� �������� �� ������ �����';
comment on column XML_L_PERS_TEMP1.ot_tfoms
  is '�������� �������� �� ������ �����';
comment on column XML_L_PERS_TEMP1.dr_tfoms
  is '���� �������� �� ������ �����';
-- Create/Recreate primary, unique and foreign key constraints 
alter table XML_L_PERS_TEMP1
  add constraint PK_XML_L_PERS_ID_TEMP1 primary key (PERS_ID)
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