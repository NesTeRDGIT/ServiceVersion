-- Create table
create table XML_H_USL_TEMP1
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
comment on table XML_H_USL_TEMP1
  is 'Сведения об услуге XML';
-- Add comments to the columns 
comment on column XML_H_USL_TEMP1.usl_id
  is 'ID услуги';
comment on column XML_H_USL_TEMP1.sluch_id
  is 'Внешний ключ на таблицу XML_H_SLUCH';
comment on column XML_H_USL_TEMP1.idserv
  is 'Номер записи в реестре услуг';
comment on column XML_H_USL_TEMP1.lpu
  is 'Код МО';
comment on column XML_H_USL_TEMP1.lpu_1
  is 'Подразделение МО';
comment on column XML_H_USL_TEMP1.podr
  is 'Код отделения';
comment on column XML_H_USL_TEMP1.profil
  is 'Профиль';
comment on column XML_H_USL_TEMP1.det
  is 'Признак детского стационара';
comment on column XML_H_USL_TEMP1.date_in
  is 'Дата начала окончания услуги';
comment on column XML_H_USL_TEMP1.date_out
  is 'Дата окончания оказания услуги';
comment on column XML_H_USL_TEMP1.ds
  is 'Диагноз';
comment on column XML_H_USL_TEMP1.code_usl
  is 'Код услуги';
comment on column XML_H_USL_TEMP1.kol_usl
  is 'Количество услуги (кратность услуги)';
comment on column XML_H_USL_TEMP1.tarif
  is 'Тариф';
comment on column XML_H_USL_TEMP1.sumv_usl
  is 'Стоимость, медицинской услуги, выставленная к оплате (руб)';
comment on column XML_H_USL_TEMP1.prvs
  is 'Специальность медработника, выполнившего услугу';
comment on column XML_H_USL_TEMP1.code_md
  is 'Код медицинского работника, оказавшего медицинскую услугу';
comment on column XML_H_USL_TEMP1.comentu
  is 'Служебное поле';
comment on column XML_H_USL_TEMP1.err
  is 'Код ошибки';
comment on column XML_H_USL_TEMP1.err_prim
  is 'Описание ошибки';
comment on column XML_H_USL_TEMP1.fond
  is 'Признак оплаты услуги 1 - в Фондодержании; 2 - вне Фондодержания';
comment on column XML_H_USL_TEMP1.tarif_tfoms
  is 'Тариф (данные ТФОМС)';
comment on column XML_H_USL_TEMP1.sumv_usl_tfoms
  is 'Стоимость (данные ТФОМС)';
comment on column XML_H_USL_TEMP1.vid_vme
  is 'Вид медицинского вмешательства(V001)';
-- Create/Recreate primary, unique and foreign key constraints 
alter table XML_H_USL_TEMP1
  add constraint PK_XML_H_USL_ID_TEMP1 primary key (USL_ID)
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
create index IND_USL_TEMP_1 on XML_H_USL_TEMP1 (SLUCH_ID)
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