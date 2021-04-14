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
  is 'Сведения о случае XML';
-- Add comments to the columns 
comment on column XML_H_SLUCH_TEMP99.sluch_id
  is 'ID случая';
comment on column XML_H_SLUCH_TEMP99.pacient_id
  is 'Внешний ключ на таблицу XML_H_PACIENT(Пока не используется)';
comment on column XML_H_SLUCH_TEMP99.zap_id
  is 'Внешний ключ на таблицу XML_H_ZAP';
comment on column XML_H_SLUCH_TEMP99.idcase
  is 'Номер записи в реестре случаев';
comment on column XML_H_SLUCH_TEMP99.usl_ok
  is 'Условия оказания медицинской помощи';
comment on column XML_H_SLUCH_TEMP99.vidpom
  is 'Вид помощи';
comment on column XML_H_SLUCH_TEMP99.npr_mo
  is 'Код МО, направившего на лечение (диагностику, консультацию)';
comment on column XML_H_SLUCH_TEMP99.extr
  is 'Направление (госпитализация)';
comment on column XML_H_SLUCH_TEMP99.lpu
  is 'Код МО';
comment on column XML_H_SLUCH_TEMP99.lpu_1
  is 'Подразделение МО';
comment on column XML_H_SLUCH_TEMP99.podr
  is 'Код отделения';
comment on column XML_H_SLUCH_TEMP99.profil
  is 'Профиль';
comment on column XML_H_SLUCH_TEMP99.det
  is 'Признак детского профиля';
comment on column XML_H_SLUCH_TEMP99.nhistory
  is 'Номер истории болезни/талона амбулаторного пациента';
comment on column XML_H_SLUCH_TEMP99.date_1
  is 'Дата начала лечения';
comment on column XML_H_SLUCH_TEMP99.date_2
  is 'Дата окончания лечения';
comment on column XML_H_SLUCH_TEMP99.ds0
  is 'Диагноз первичный';
comment on column XML_H_SLUCH_TEMP99.ds1
  is 'Диагноз основной';
comment on column XML_H_SLUCH_TEMP99.ds2
  is 'Диагноз сопутствующего заболевания';
comment on column XML_H_SLUCH_TEMP99.code_mes1
  is 'Код МЭС';
comment on column XML_H_SLUCH_TEMP99.code_mes2
  is 'Код МЭС сопутствующего заболевания';
comment on column XML_H_SLUCH_TEMP99.rslt
  is 'Результат обращения/госпитализации';
comment on column XML_H_SLUCH_TEMP99.ishod
  is 'Исход заболевания';
comment on column XML_H_SLUCH_TEMP99.prvs
  is 'Специальность лечащего врача/врача. закрывшего талон';
comment on column XML_H_SLUCH_TEMP99.iddokt
  is 'Код врача, закрывшего талон/историю болезни';
comment on column XML_H_SLUCH_TEMP99.os_sluch
  is 'Признак "Особый случай" при регистрации обращения за медицинской помощью';
comment on column XML_H_SLUCH_TEMP99.idsp
  is 'Код способа оплаты медицинской помощи';
comment on column XML_H_SLUCH_TEMP99.ed_col
  is 'Количество единиц оплаты медицинской помощи';
comment on column XML_H_SLUCH_TEMP99.tarif
  is 'Тариф';
comment on column XML_H_SLUCH_TEMP99.sumv
  is 'Сумма, выставленная к оплате';
comment on column XML_H_SLUCH_TEMP99.oplata
  is 'Тип оплаты';
comment on column XML_H_SLUCH_TEMP99.sump
  is 'Сумма, принятая к оплате СМО';
comment on column XML_H_SLUCH_TEMP99.refreason
  is 'Код причины отказа (частичной) оплаты';
comment on column XML_H_SLUCH_TEMP99.sank_mek
  is 'Финансовые санкции (МЭК)';
comment on column XML_H_SLUCH_TEMP99.sank_mee
  is 'Финансовые санкции (МЭЭ)';
comment on column XML_H_SLUCH_TEMP99.sank_ekmp
  is 'Финансовые санкции (ЭКМП)';
comment on column XML_H_SLUCH_TEMP99.comentsl
  is 'Служебное поле';
comment on column XML_H_SLUCH_TEMP99.err
  is 'Код ошибки';
comment on column XML_H_SLUCH_TEMP99.err_prim
  is 'Описание ошибки';
comment on column XML_H_SLUCH_TEMP99.sank_it
  is 'Сумма санкций по случаю';
comment on column XML_H_SLUCH_TEMP99.for_pom
  is 'Классификатор форм оказания медицин-ской помощи. Спра-вочник V014 ';
comment on column XML_H_SLUCH_TEMP99.vers_spec
  is 'Код классификато-рамедицинских специальностей';
comment on column XML_H_SLUCH_TEMP99.vnov_m
  is 'Вес при рождении';
comment on column XML_H_SLUCH_TEMP99.ds3
  is 'Диагноз осложне-ния заболевания';
comment on column XML_H_SLUCH_TEMP99.rslt_d
  is 'Результат диспансеризации (V017)';
comment on column XML_H_SLUCH_TEMP99.p_otk
  is 'Отказ от диспансеризации';
comment on column XML_H_SLUCH_TEMP99.tarif_tfoms
  is 'Тариф (данные ТФОМС)';
comment on column XML_H_SLUCH_TEMP99.sumv_tfoms
  is 'Сумма (данные ТФОМС)';
comment on column XML_H_SLUCH_TEMP99.num_tf
  is 'Номер счета по МТР';
comment on column XML_H_SLUCH_TEMP99.idcase_tf
  is 'Номер записи в реестре случаев  по МТР';
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