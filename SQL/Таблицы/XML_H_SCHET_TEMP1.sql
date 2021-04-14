-- Create table
create table XML_H_SCHET_TEMP1
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
comment on table XML_H_SCHET_TEMP1
  is 'Описание счета XML';
-- Add comments to the columns 
comment on column XML_H_SCHET_TEMP1.schet_id
  is 'Идентификатор счета';
comment on column XML_H_SCHET_TEMP1.zglv_id
  is 'Внешний ключ на таблицу XML_H_ZGLV';
comment on column XML_H_SCHET_TEMP1.code
  is 'Код записи счета';
comment on column XML_H_SCHET_TEMP1.code_mo
  is 'Реестровый номер медицинской организации';
comment on column XML_H_SCHET_TEMP1.year
  is 'Отчетный год';
comment on column XML_H_SCHET_TEMP1.month
  is 'Отчетный месяц';
comment on column XML_H_SCHET_TEMP1.nschet
  is 'Номер счёта';
comment on column XML_H_SCHET_TEMP1.dschet
  is 'Дата выставления счета';
comment on column XML_H_SCHET_TEMP1.plat
  is 'Плательщик. Реестровый номер СМО';
comment on column XML_H_SCHET_TEMP1.summav
  is 'Сумма МО, выставленная на оплату';
comment on column XML_H_SCHET_TEMP1.coments
  is 'Служебное поле';
comment on column XML_H_SCHET_TEMP1.summap
  is 'Сумма, принятая к оплате СМО';
comment on column XML_H_SCHET_TEMP1.sank_mek
  is 'Финансовые санкции (МЭК)';
comment on column XML_H_SCHET_TEMP1.sank_mee
  is 'Финансовые санкции (МЭЭ)';
comment on column XML_H_SCHET_TEMP1.sank_empk
  is 'Финансовые санкции (ЭКМП)';
comment on column XML_H_SCHET_TEMP1.err
  is 'Код ошибки';
comment on column XML_H_SCHET_TEMP1.err_prim
  is 'Описание ошибки';
comment on column XML_H_SCHET_TEMP1.disp
  is 'Тип диспансеризации (V016)';
comment on column XML_H_SCHET_TEMP1.summav_tfoms
  is 'Сумма ТФОМС';
-- Create/Recreate primary, unique and foreign key constraints 
alter table XML_H_SCHET_TEMP1
  add constraint PK_XML_H_SCHET_ID_TEMP1 primary key (SCHET_ID)
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