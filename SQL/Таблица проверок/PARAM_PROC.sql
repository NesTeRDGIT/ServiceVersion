prompt PL/SQL Developer import file
prompt Created on 9 Апрель 2015 г. by user_close_sad
set feedback off
set define off
prompt Creating PARAM_PROC...
create table PARAM_PROC
(
  id_proc      NUMBER not null,
  name         NVARCHAR2(60),
  datatype     NVARCHAR2(60),
  comments     NVARCHAR2(120),
  defaultvalue NVARCHAR2(60),
  type_value   NVARCHAR2(60)
)
;
comment on table PARAM_PROC
  is 'Часть сервиса Нестеренок';
comment on column PARAM_PROC.id_proc
  is 'Внешний ключ на PROC';
comment on column PARAM_PROC.name
  is 'Имя параметра';
comment on column PARAM_PROC.datatype
  is 'ТИП ДАННЫХ';
comment on column PARAM_PROC.comments
  is 'Коментарий';
comment on column PARAM_PROC.defaultvalue
  is 'Значение';
comment on column PARAM_PROC.type_value
  is 'Тип параметра(Константа value, или привязка)';
alter table PARAM_PROC
  add constraint ID_PROC foreign key (ID_PROC)
  references ERRORSPROC (ID) on delete cascade;

prompt Loading PARAM_PROC...
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (0, 'L_ZGLV_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_ZGLV');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (0, 'err', 'Varchar2', null, 'XML_H_ZGLV_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (0, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (0, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (1, 'L_ZGLV_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_ZGLV');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (1, 'err', 'Varchar2', null, 'XML_H_ZGLV_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (1, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (1, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (2, 'L_ZGLV_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_ZGLV');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (2, 'err', 'Varchar2', null, 'XML_H_ZGLV_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (2, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (2, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (3, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (3, 'XML_H_ZAP_TEMP', 'Varchar2', null, null, 'TABLE_NAME_ZAP');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (3, 'xml_h_sluch_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (3, 'err', 'Varchar2', null, 'H_SCHET_SUM_SLUCH; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (3, 'err_prim', 'Varchar2', null, 'Сумма по случаям отличается от суммы по счету; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (3, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (4, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (4, 'err', 'Varchar2', null, 'H_SCHET_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (4, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (4, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (5, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (5, 'err', 'Varchar2', null, 'H_SCHET_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (5, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (5, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (6, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (6, 'err', 'Varchar2', null, 'H_SCHET_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (6, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (6, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (7, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (7, 'err', 'Varchar2', null, 'H_SCHET_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (7, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (7, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (8, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (8, 'err', 'Varchar2', null, 'H_SCHET_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (8, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (8, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (9, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (9, 'err', 'Varchar2', null, null, 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (9, 'err_prim', 'Varchar2', null, null, 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (9, 'err_type', 'Varchar2', null, null, 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (10, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (10, 'err', 'Varchar2', null, null, 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (10, 'err_prim', 'Varchar2', null, null, 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (10, 'err_type', 'Varchar2', null, null, 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (11, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (11, 'err', 'Varchar2', null, 'H_SCHET_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (11, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (11, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (12, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (12, 'err', 'Varchar2', null, 'H_SCHET_CODE;', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (12, 'err_prim', 'Varchar2', null, 'Задвоенное значение поля', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (12, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (13, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (13, 'err', 'Varchar2', null, 'H_SCHET_CODE_MO; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (13, 'err_prim', 'Varchar2', null, 'Код МО не найден в справочнике.', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (13, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (14, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (14, 'err', 'Varchar2', null, 'H_SCHET_PLAT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (14, 'err_prim', 'Varchar2', null, 'Некорректное значение в поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (14, 'err_type', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (15, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (15, 'XML_H_ZAP_TEMP', 'Varchar2', null, null, 'TABLE_NAME_ZAP');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (15, 'err', 'Varchar2', null, 'H_SCHET_EMPTY_ZAP; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (15, 'err_prim', 'Varchar2', null, 'Нет соответствующей записи для счета; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (15, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (16, 'XML_H_SCHET_TEMP', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (16, 'XML_H_ZGLV_TEMP', 'Varchar2', null, null, 'TABLE_NAME_ZGLV');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (16, 'err', 'Varchar2', null, 'H_SCHET_EMPTY_ZGLV; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (16, 'err_prim', 'Varchar2', null, 'Нет соответствующего заголовка для счета; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (16, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (17, 'XML_H_ZAP_TEMP', 'Varchar2', null, null, 'TABLE_NAME_ZAP');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (17, 'err', 'Varchar2', null, 'XML_H_ZAP_EMPT', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (17, 'err_prim', 'Varchar2', null, 'Не заполнены обязательные поля; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (17, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (18, 'XML_H_ZAP_TEMP', 'Varchar2', null, null, 'TABLE_NAME_ZAP');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (18, 'err', 'Varchar2', null, 'XML_H_ZAP_EMPT', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (18, 'err_prim', 'Varchar2', null, 'Не заполнены обязательные поля; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (18, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (19, 'XML_H_ZAP_TEMP', 'Varchar2', null, null, 'TABLE_NAME_ZAP');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (19, 'err', 'Varchar2', null, 'XML_H_ZAP_EMPT', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (19, 'err_prim', 'Varchar2', null, 'Не заполнены обязательные поля; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (19, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (20, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (20, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (20, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (20, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (21, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (21, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (21, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (21, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (22, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (22, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (22, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (22, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (23, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (23, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (23, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (23, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (23, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (24, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (24, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (24, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (24, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (24, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (25, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (25, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (25, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (25, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (25, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (26, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (26, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (26, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (26, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (27, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (27, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (27, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (27, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (28, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (28, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (28, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (28, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (28, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (29, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (29, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (29, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (29, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (30, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (30, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (30, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (30, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (30, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (31, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (31, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (31, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (31, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (32, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (32, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (32, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (32, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (33, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (33, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (33, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (33, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (34, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (34, 'ERR', 'Varchar2', null, 'H_USL_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (34, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (34, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (35, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (35, 'H_USL_TBL_NAME_MAIN', 'Varchar2', null, 'ЛЯЛЯ', 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (35, 'ERR', 'Varchar2', null, 'H_USL_DOUB; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (35, 'ERR_PRIM', 'Varchar2', null, 'Услуга дублирована; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (35, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (36, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (36, 'ERR', 'Varchar2', null, 'H_USL_LPU;', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (36, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (36, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (37, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (37, 'ERR', 'Varchar2', null, 'H_USL_PROFIL; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (37, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (37, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (38, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (38, 'ERR', 'Varchar2', null, 'H_USL_DET; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (38, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (38, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (39, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (39, 'ERR', 'Varchar2', null, 'H_USL_DATE; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (39, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (39, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (40, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (40, 'ERR', 'Varchar2', null, 'H_USL_DS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (40, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (40, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (41, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (41, 'ERR', 'Varchar2', null, 'H_USL_CODE_USL; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (41, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (41, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (42, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (42, 'ERR', 'Varchar2', null, 'H_USL_CODE_USL; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (42, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (42, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (43, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (43, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (43, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (43, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (44, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (44, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (44, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (44, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (45, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (45, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (45, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (45, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (46, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (46, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (46, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (46, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (47, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (47, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (47, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (47, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (48, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (48, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (48, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (48, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (49, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (49, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (49, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (49, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (50, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (50, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (50, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (50, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (51, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (51, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (51, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (51, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (52, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (52, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (52, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (52, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (53, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (53, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (53, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (53, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (54, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (54, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (54, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (54, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (55, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (55, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (55, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (55, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (56, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (56, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (56, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (56, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (57, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (57, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (57, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (57, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (58, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (58, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (58, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (58, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (59, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (59, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (59, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (59, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (60, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (60, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (60, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (60, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (61, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (61, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (61, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (61, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (62, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (62, 'ERR', 'Varchar2', null, 'H_SLUCH_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (62, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (62, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (63, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (63, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (63, 'ERR', 'Varchar2', null, 'H_SLUCH_CODE_USL; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (63, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (63, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (64, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (64, 'ERR', 'Varchar2', null, 'H_SLUCH_code_mes1; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (64, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (64, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (65, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (65, 'ERR', 'Varchar2', null, 'H_SLUCH_RSLT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (65, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (65, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (66, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (66, 'ERR', 'Varchar2', null, 'H_SLUCH_DATE; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (66, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (66, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (67, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (67, 'ERR', 'Varchar2', null, 'H_SLUCH_DET; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (67, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (67, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (68, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (68, 'ERR', 'Varchar2', null, 'H_SLUCH_DS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (68, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (68, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (69, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (69, 'ERR', 'Varchar2', null, 'H_SLUCH_DS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (69, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (69, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (70, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (70, 'ERR', 'Varchar2', null, 'H_SLUCH_DS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (70, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (70, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (71, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (71, 'ERR', 'Varchar2', null, 'H_SLUCH_DS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (71, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (71, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (72, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (72, 'ERR', 'Varchar2', null, 'H_SLUCH_DS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (72, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (72, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (73, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (73, 'ERR', 'Varchar2', null, 'H_SLUCH_DS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (73, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (73, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (74, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (74, 'ERR', 'Varchar2', null, 'H_SLUCH_DS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (74, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (74, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (75, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (75, 'ERR', 'Varchar2', null, 'H_SLUCH_DS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (75, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (75, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (76, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (76, 'ERR', 'Varchar2', null, 'H_SLUCH_EXTR; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (76, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (76, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (77, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (77, 'ERR', 'Varchar2', null, 'H_SLUCH_FORPOM; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (77, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (77, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (78, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (78, 'ERR', 'Varchar2', null, 'H_SLUCH_IDSP; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (78, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (78, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (79, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (79, 'ERR', 'Varchar2', null, 'H_SLUCH_ISHOD; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (79, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (79, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (80, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (80, 'ERR', 'Varchar2', null, 'H_SLUCH_ISHOD_USL; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (80, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (80, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (81, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (81, 'ERR', 'Varchar2', null, 'H_SLUCH_LPU; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (81, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (81, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (82, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (82, 'ERR', 'Varchar2', null, 'H_SLUCH_LPU1; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (82, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (82, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (83, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (83, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (83, 'ERR', 'Varchar2', null, 'H_SLUCH_NPR_MO; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (83, 'ERR_PRIM', 'Varchar2', null, 'Не указана направившая МО ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (83, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (84, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (84, 'ERR', 'Varchar2', null, 'H_SLUCH_NPR_MO; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (84, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (84, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (85, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (85, 'ERR', 'Varchar2', null, 'H_SLUCH_PERIOD; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (85, 'ERR_PRIM', 'Varchar2', null, 'Случай не входит в отчетный период; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (85, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (85, 'P_MON', 'Int32', null, '3', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (86, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (86, 'ERR', 'Varchar2', null, 'H_SLUCH_PROFIL; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (86, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (86, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (87, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (87, 'ERR', 'Varchar2', null, 'H_SLUCH_PRVS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (87, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (87, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (88, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (88, 'ERR', 'Varchar2', null, 'H_SLUCH_PRVS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (88, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (88, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (89, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (89, 'ERR', 'Varchar2', null, 'H_SLUCH_RSLT_D; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (89, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (89, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (90, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (90, 'ERR', 'Varchar2', null, 'H_SLUCH_RSLT_USL; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (90, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (90, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (91, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (91, 'ERR', 'Varchar2', null, 'H_SLUCH_SUMV; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (91, 'ERR_PRIM', 'Varchar2', null, 'Сумма по случаю равна 0', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (91, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (92, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (92, 'H_USL_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_USL');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (92, 'ERR', 'Varchar2', null, 'H_SLUCH_SUM_USL; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (92, 'ERR_PRIM', 'Varchar2', null, 'Сумма по услугам отличается; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (92, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (93, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (93, 'ERR', 'Varchar2', null, 'H_SLUCH_VIDPOM; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (93, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (93, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (94, 'H_PAC_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (94, 'ERR', 'Varchar2', null, 'H_PACIENT_NOVOR; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (94, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (94, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (95, 'H_PAC_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (95, 'H_SLUCH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (95, 'ERR', 'Varchar2', null, 'H_PACIENT_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (95, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (95, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (96, 'H_PAC_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (96, 'ERR', 'Varchar2', null, 'H_PACIENT_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (96, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (96, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (97, 'H_PAC_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (97, 'ERR', 'Varchar2', null, 'H_PACIENT_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (97, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (97, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (98, 'H_PAC_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (98, 'ERR', 'Varchar2', null, 'H_PACIENT_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (98, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (98, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (99, 'H_PAC_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (99, 'H_ZAP_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_ZAP');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (99, 'ERR', 'Varchar2', null, 'H_PACIENT_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (99, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (99, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (100, 'H_PAC_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (100, 'ERR', 'Varchar2', null, 'H_PACIENT_NPOLIS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (100, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (100, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (101, 'H_PAC_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (101, 'ERR', 'Varchar2', null, 'H_PACIENT_SMO; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (101, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (101, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (102, 'H_PAC_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (102, 'ERR', 'Varchar2', null, 'H_PACIENT_SPOLIS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (102, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (102, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (103, 'H_PAC_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (103, 'ERR', 'Varchar2', null, 'H_PACIENT_VPOLIS; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (103, 'ERR_PRIM', 'Varchar2', null, 'Некорректное заполнение ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (103, 'ERR_TYPE', 'Varchar2', null, 'Ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (104, 'H_PAC_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (104, 'ERR', 'Varchar2', null, 'H_PACIENT_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (104, 'ERR_PRIM', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (104, 'ERR_TYPE', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (105, 'L_ZGLV_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_L_ZGLV');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (105, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (105, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (105, 'err', 'Varchar2', null, 'XML_L_ZGLV_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (106, 'L_ZGLV_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_L_ZGLV');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (106, 'err', 'Varchar2', null, 'XML_L_ZGLV_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (106, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (106, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (107, 'L_ZGLV_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_L_ZGLV');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (107, 'err', 'Varchar2', null, 'XML_L_ZGLV_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (107, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (107, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (108, 'L_ZGLV_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_L_ZGLV');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (108, 'err', 'Varchar2', null, 'XML_L_ZGLV_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (108, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (108, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (109, 'L_PERS_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_L_PERS');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (109, 'err', 'Varchar2', null, 'XML_L_PERS_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (109, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (109, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (110, 'L_PERS_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_L_PERS');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (110, 'err', 'Varchar2', null, 'XML_L_PERS_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (110, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (110, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (111, 'L_PERS_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_L_PERS');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (111, 'err', 'Varchar2', null, 'XML_L_PERS_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (111, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (111, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (112, 'L_PERS_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_L_PERS');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (112, 'err', 'Varchar2', null, 'XML_L_PERS_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (112, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (112, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (113, 'L_PERS_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_L_PERS');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (113, 'L_ZGLV_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_L_ZGLV');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (113, 'H_ZGLV_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_ZGLV');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (113, 'H_SCHET_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SCHET');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (113, 'H_ZAP_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_ZAP');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (113, 'H_PACIENT_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_PACIENT');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (113, 'H_SLUSH_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_SLUCH');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (113, 'err', 'Varchar2', null, 'XML_L_PERS_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (113, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (113, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (114, 'L_PERS_TBL_NAME', 'Varchar2', null, null, 'TABLE_NAME_L_PERS');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (114, 'err', 'Varchar2', null, 'XML_L_PERS_EMPT; ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (114, 'err_prim', 'Varchar2', null, 'Не заполнено обязательное поле ', 'value');
insert into PARAM_PROC (id_proc, name, datatype, comments, defaultvalue, type_value)
values (114, 'err_type', 'Varchar2', null, 'Критическая ошибка. ', 'value');
prompt 482 records loaded
set feedback on
set define on
prompt Done.
