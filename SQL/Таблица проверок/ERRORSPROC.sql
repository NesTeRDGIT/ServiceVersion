prompt PL/SQL Developer import file
prompt Created on 9 Апрель 2015 г. by user_close_sad
set feedback off
set define off
prompt Creating ERRORSPROC...
create table ERRORSPROC
(
  id        NUMBER not null,
  name_err  NVARCHAR2(60),
  name_proc NVARCHAR2(60),
  type_err  NVARCHAR2(60),
  owner     NVARCHAR2(60),
  state     NUMBER(1)
)
tablespace MEDPOM
  pctfree 10
  pctused 40
  initrans 1
  maxtrans 255
  storage
  (
    initial 64K
    minextents 1
    maxextents unlimited
  );
comment on table ERRORSPROC
  is 'Нестеренок Д.В. Часть сервиса';
comment on column ERRORSPROC.id
  is 'ID';
comment on column ERRORSPROC.name_err
  is 'Имя ошибки';
comment on column ERRORSPROC.name_proc
  is 'Имя фукции в БД';
comment on column ERRORSPROC.type_err
  is 'Тип ошибки';
comment on column ERRORSPROC.owner
  is 'Таблица для которой проверка';
comment on column ERRORSPROC.state
  is 'Активна проверка или нет';
alter table ERRORSPROC
  add constraint ID primary key (ID)
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

prompt Loading ERRORSPROC...
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (0, 'Версия', 'PKG_XML_H_ZGLV_CONTROL.EMPT_VERSION', 'Критическая ошибка', 'ZGLV', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (1, 'EMPT_DATA', 'PKG_XML_H_ZGLV_CONTROL.EMPT_DATA', 'Критическая ошибка', 'ZGLV', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (2, 'EMPT_FILENAME', 'PKG_XML_H_ZGLV_CONTROL.EMPT_FILENAME', 'Критическая ошибка', 'ZGLV', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (3, 'SUM_SCHET', 'PKG_XML_H_SCHET_CONTROL.SUM_SCHET', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (4, 'EMPT_MONTH', 'PKG_XML_H_SCHET_CONTROL.EMPT_MONTH', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (5, 'EMPT_NSCHET', 'PKG_XML_H_SCHET_CONTROL.EMPT_NSCHET', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (6, 'EMPT_DSCHET', 'PKG_XML_H_SCHET_CONTROL.EMPT_DSCHET', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (7, 'EMPT_PLAT', 'PKG_XML_H_SCHET_CONTROL.EMPT_PLAT', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (8, 'EMPT_SUMMAV', 'PKG_XML_H_SCHET_CONTROL.EMPT_SUMMAV', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (9, 'EMPT_CODE', 'PKG_XML_H_SCHET_CONTROL.EMPT_CODE', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (10, 'EMPT_CODE_MO', 'PKG_XML_H_SCHET_CONTROL.EMPT_CODE_MO', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (11, 'EMPT_YEAR', 'PKG_XML_H_SCHET_CONTROL.EMPT_YEAR', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (12, 'CODE_DOUBLE', 'PKG_XML_H_SCHET_CONTROL.CODE_DOUBLE', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (13, 'CODE_MO_EXISTS', 'PKG_XML_H_SCHET_CONTROL.CODE_MO_EXISTS', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (14, 'PLAT_TFOMS', 'PKG_XML_H_SCHET_CONTROL.PLAT_TFOMS', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (15, 'schet_id', 'PKG_XML_H_SCHET_CONTROL.schet_id', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (16, 'ZGLV_ID_EXIST', 'PKG_XML_H_SCHET_CONTROL.ZGLV_ID_EXIST', 'Критическая ошибка', 'SCHET', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (17, 'EMPT_N_ZAP', 'PKG_XML_H_ZAP_CONTROL.EMPT_N_ZAP', 'Критическая ошибка', 'ZAP', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (18, 'EMPT_PR_NOV', 'PKG_XML_H_ZAP_CONTROL.EMPT_PR_NOV', 'Критическая ошибка', 'ZAP', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (19, 'PR_NOV_EXISTS', 'PKG_XML_H_ZAP_CONTROL.PR_NOV_EXISTS', 'Критическая ошибка', 'ZAP', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (20, 'EMPT_IDSERV', 'PKG_XML_H_USL_CONTROL.EMPT_IDSERV', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (21, 'EMPT_LPU', 'PKG_XML_H_USL_CONTROL.EMPT_LPU', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (22, 'EMPT_LPU_1', 'PKG_XML_H_USL_CONTROL.EMPT_LPU_1', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (23, 'EMPT_PODR', 'PKG_XML_H_USL_CONTROL.EMPT_PODR', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (24, 'EMPT_PROFIL', 'PKG_XML_H_USL_CONTROL.EMPT_PROFIL', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (25, 'EMPT_DET', 'PKG_XML_H_USL_CONTROL.EMPT_DET', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (26, 'EMPT_DATE_IN', 'PKG_XML_H_USL_CONTROL.EMPT_DATE_IN', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (27, 'EMPT_DATE_OUT', 'PKG_XML_H_USL_CONTROL.EMPT_DATE_OUT', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (28, 'EMPT_DS', 'PKG_XML_H_USL_CONTROL.EMPT_DS', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (29, 'EMPT_CODE_USL', 'PKG_XML_H_USL_CONTROL.EMPT_CODE_USL', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (30, 'EMPT_KOL_USL', 'PKG_XML_H_USL_CONTROL.EMPT_KOL_USL', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (31, 'EMPT_TARIF', 'PKG_XML_H_USL_CONTROL.EMPT_TARIF', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (32, 'EMPT_SUMV_USL', 'PKG_XML_H_USL_CONTROL.EMPT_SUMV_USL', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (33, 'EMPT_PRVS', 'PKG_XML_H_USL_CONTROL.EMPT_PRVS', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (34, 'EMPT_CODE_MD', 'PKG_XML_H_USL_CONTROL.EMPT_CODE_MD', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (35, 'DOUBLE_USL_ID', 'PKG_XML_H_USL_CONTROL.DOUBLE_USL_ID', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (36, 'EXIST_LPU', 'PKG_XML_H_USL_CONTROL.EXIST_LPU', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (37, 'EXIST_PROFIL', 'PKG_XML_H_USL_CONTROL.EXIST_PROFIL', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (38, 'EXIST_DET', 'PKG_XML_H_USL_CONTROL.EXIST_DET', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (39, 'CHECK_DATE_IN_DATE_OUT', 'PKG_XML_H_USL_CONTROL.CHECK_DATE_IN_DATE_OUT', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (40, 'EXIST_DS', 'PKG_XML_H_USL_CONTROL.EXIST_DS', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (41, 'EXIST_CODE_USL', 'PKG_XML_H_USL_CONTROL.EXIST_CODE_USL', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (42, 'EXIST_CODE_USL2', 'PKG_XML_H_USL_CONTROL.EXIST_CODE_USL2', null, 'USL', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (43, 'EMPT_DATE_1', 'PKG_XML_H_SLUCH_CONTROL.EMPT_DATE_1', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (44, 'EMPT_DATE_2', 'PKG_XML_H_SLUCH_CONTROL.EMPT_DATE_2', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (45, 'EMPT_DET', 'PKG_XML_H_SLUCH_CONTROL.EMPT_DET', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (46, 'EMPT_DS1', 'PKG_XML_H_SLUCH_CONTROL.EMPT_DS1', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (47, 'EMPT_FOR_POM', 'PKG_XML_H_SLUCH_CONTROL.EMPT_FOR_POM', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (48, 'EMPT_IDCASE', 'PKG_XML_H_SLUCH_CONTROL.EMPT_IDCASE', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (49, 'EMPT_IDDOKT', 'PKG_XML_H_SLUCH_CONTROL.EMPT_IDDOKT', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (50, 'EMPT_IDSP', 'PKG_XML_H_SLUCH_CONTROL.EMPT_IDSP', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (51, 'EMPT_ISHOD', 'PKG_XML_H_SLUCH_CONTROL.EMPT_ISHOD', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (52, 'EMPT_LPU', 'PKG_XML_H_SLUCH_CONTROL.EMPT_LPU', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (53, 'EMPT_LPU_1', 'PKG_XML_H_SLUCH_CONTROL.EMPT_LPU_1', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (54, 'EMPT_NHISTORY', 'PKG_XML_H_SLUCH_CONTROL.EMPT_NHISTORY', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (55, 'EMPT_PODR', 'PKG_XML_H_SLUCH_CONTROL.EMPT_PODR', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (56, 'EMPT_PROFIL', 'PKG_XML_H_SLUCH_CONTROL.EMPT_PROFIL', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (57, 'EMPT_PRVS', 'PKG_XML_H_SLUCH_CONTROL.EMPT_PRVS', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (58, 'EMPT_RSLT', 'PKG_XML_H_SLUCH_CONTROL.EMPT_RSLT', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (59, 'EMPT_SUMV', 'PKG_XML_H_SLUCH_CONTROL.EMPT_SUMV', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (60, 'EMPT_USL_OK', 'PKG_XML_H_SLUCH_CONTROL.EMPT_USL_OK', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (61, 'EMPT_USL_OK1', 'PKG_XML_H_SLUCH_CONTROL.EMPT_USL_OK1', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (62, 'EMPT_VIDPOM', 'PKG_XML_H_SLUCH_CONTROL.EMPT_VIDPOM', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (63, 'EXIST_CODE_MES1', 'PKG_XML_H_SLUCH_CONTROL.EXIST_CODE_MES1', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (64, 'EXIST_CODE_MES11', 'PKG_XML_H_SLUCH_CONTROL.EXIST_CODE_MES11', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (65, 'EXIST_CODE_RSLT', 'PKG_XML_H_SLUCH_CONTROL.EXIST_CODE_RSLT', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (66, 'EXIST_DATE', 'PKG_XML_H_SLUCH_CONTROL.EXIST_DATE', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (67, 'EXIST_DET', 'PKG_XML_H_SLUCH_CONTROL.EXIST_DET', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (68, 'EXIST_DS0', 'PKG_XML_H_SLUCH_CONTROL.EXIST_DS0', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (69, 'EXIST_DS01', 'PKG_XML_H_SLUCH_CONTROL.EXIST_DS01', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (70, 'EXIST_DS1', 'PKG_XML_H_SLUCH_CONTROL.EXIST_DS1', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (71, 'EXIST_DS11', 'PKG_XML_H_SLUCH_CONTROL.EXIST_DS11', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (72, 'EXIST_DS2', 'PKG_XML_H_SLUCH_CONTROL.EXIST_DS2', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (73, 'EXIST_DS21', 'PKG_XML_H_SLUCH_CONTROL.EXIST_DS21', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (74, 'EXIST_DS3', 'PKG_XML_H_SLUCH_CONTROL.EXIST_DS3', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (75, 'EXIST_DS31', 'PKG_XML_H_SLUCH_CONTROL.EXIST_DS31', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (76, 'EXIST_EXTR', 'PKG_XML_H_SLUCH_CONTROL.EXIST_EXTR', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (77, 'EXIST_FORPOM', 'PKG_XML_H_SLUCH_CONTROL.EXIST_FORPOM', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (78, 'EXIST_IDSP', 'PKG_XML_H_SLUCH_CONTROL.EXIST_IDSP', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (79, 'EXIST_ISHOD', 'PKG_XML_H_SLUCH_CONTROL.EXIST_ISHOD', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (80, 'EXIST_ISHOD_USL', 'PKG_XML_H_SLUCH_CONTROL.EXIST_ISHOD_USL', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (81, 'EXIST_LPU', 'PKG_XML_H_SLUCH_CONTROL.EXIST_LPU', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (82, 'EXIST_LPU1', 'PKG_XML_H_SLUCH_CONTROL.EXIST_LPU1', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (83, 'EXIST_NPR_MO1', 'PKG_XML_H_SLUCH_CONTROL.EXIST_NPR_MO1', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (84, 'EXIST_NPR_MO', 'PKG_XML_H_SLUCH_CONTROL.EXIST_NPR_MO', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (85, 'EXIST_PERIOD', 'PKG_XML_H_SLUCH_CONTROL.EXIST_PERIOD', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (86, 'EXIST_PROFIL', 'PKG_XML_H_SLUCH_CONTROL.EXIST_PROFIL', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (87, 'EXIST_PRVS', 'PKG_XML_H_SLUCH_CONTROL.EXIST_PRVS', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (88, 'EXIST_PRVS2', 'PKG_XML_H_SLUCH_CONTROL.EXIST_PRVS2', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (89, 'EXIST_RSLT_D', 'PKG_XML_H_SLUCH_CONTROL.EXIST_RSLT_D', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (90, 'EXIST_RSLT_USL', 'PKG_XML_H_SLUCH_CONTROL.EXIST_RSLT_USL', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (91, 'EXIST_SUMV', 'PKG_XML_H_SLUCH_CONTROL.EXIST_SUMV', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (92, 'EXIST_SUM_USL', 'PKG_XML_H_SLUCH_CONTROL.EXIST_SUM_USL', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (93, 'EXIST_VIDPOM', 'PKG_XML_H_SLUCH_CONTROL.EXIST_VIDPOM', null, 'SLUCH', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (94, 'EXIST_NOVOR', 'PKG_XML_H_PAC_CONTROL.EXIST_NOVOR', 'Ошибка', 'PACIENT', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (95, 'EMPT_NOVOR', 'PKG_XML_H_PAC_CONTROL.EMPT_NOVOR', 'Ошибка', 'PACIENT', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (96, 'EMPT_NPOLIS', 'PKG_XML_H_PAC_CONTROL.EMPT_NPOLIS', 'Ошибка', 'PACIENT', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (97, 'EMPT_SMO', 'PKG_XML_H_PAC_CONTROL.EMPT_SMO', 'Ошибка', 'PACIENT', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (98, 'EMPT_VPOLIS', 'PKG_XML_H_PAC_CONTROL.EMPT_VPOLIS', 'Ошибка', 'PACIENT', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (99, 'EMPT_ZAP', 'PKG_XML_H_PAC_CONTROL.EMPT_ZAP', 'Ошибка', 'PACIENT', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (100, 'EXIST_NPOLIS', 'PKG_XML_H_PAC_CONTROL.EXIST_NPOLIS', 'Ошибка', 'PACIENT', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (101, 'EXIST_SMO', 'PKG_XML_H_PAC_CONTROL.EXIST_SMO', 'Ошибка', 'PACIENT', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (102, 'EXIST_SPOLIS', 'PKG_XML_H_PAC_CONTROL.EXIST_SPOLIS', 'Ошибка', 'PACIENT', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (103, 'EXIST_VPOLIS', 'PKG_XML_H_PAC_CONTROL.EXIST_VPOLIS', 'Ошибка', 'PACIENT', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (104, 'EMPT_ID_PAC', 'PKG_XML_H_PAC_CONTROL.EMPT_ID_PAC', 'Ошибка', 'PACIENT', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (105, 'Версия', 'PKG_XML_L_ZGLV_CONTROL.EMPT_VERSION', 'Ошибка', 'L_ZGLV', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (106, 'EMPT_FILENAME', 'PKG_XML_L_ZGLV_CONTROL.EMPT_FILENAME', 'Ошибка', 'L_ZGLV', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (107, 'EMPT_FILENAME1', 'PKG_XML_L_ZGLV_CONTROL.EMPT_FILENAME1', 'Ошибка', 'L_ZGLV', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (108, 'EMPT_DATA', 'PKG_XML_L_ZGLV_CONTROL.EMPT_DATA', 'Ошибка', 'L_ZGLV', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (109, 'EMPT_ID_PAC', 'PKG_XML_L_PERS_CONTROL.EMPT_ID_PAC', '1', 'PERS', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (110, 'EMPT_W', 'PKG_XML_L_PERS_CONTROL.EMPT_W', '1', 'PERS', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (111, 'EMPT_FAM', 'PKG_XML_L_PERS_CONTROL.EMPT_FAM', '1', 'PERS', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (112, 'EMPT_IM', 'PKG_XML_L_PERS_CONTROL.EMPT_IM', '1', 'PERS', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (113, 'EMPT_OT', 'PKG_XML_L_PERS_CONTROL.EMPT_OT', '1', 'PERS', 1);
insert into ERRORSPROC (id, name_err, name_proc, type_err, owner, state)
values (114, 'EMPT_DR', 'PKG_XML_L_PERS_CONTROL.EMPT_DR', '1', 'PERS', 1);
prompt 115 records loaded
set feedback on
set define on
prompt Done.
