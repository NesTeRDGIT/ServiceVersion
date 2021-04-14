prompt PL/SQL Developer import file
prompt Created on 8 Февраль 2016 г. by ndv
set feedback off
set define off
prompt Creating MEDPOM_CLIENT_ROLES...
create table MEDPOM_CLIENT_ROLES
(
  id           NUMBER not null,
  role_name    VARCHAR2(50),
  role_comment VARCHAR2(250)
)
;
comment on column MEDPOM_CLIENT_ROLES.id
  is 'ID';
comment on column MEDPOM_CLIENT_ROLES.role_name
  is 'Наименование роли';
comment on column MEDPOM_CLIENT_ROLES.role_comment
  is 'Комментарии к роли';
alter table MEDPOM_CLIENT_ROLES
  add constraint PK primary key (ID);

prompt Creating MEDPOM_EXIST_METHOD...
create table MEDPOM_EXIST_METHOD
(
  name   VARCHAR2(50),
  coment VARCHAR2(250),
  id     NUMBER not null
)
;
alter table MEDPOM_EXIST_METHOD
  add constraint PK_MEDPOM_EXIST_MET primary key (ID);
alter table MEDPOM_EXIST_METHOD
  add constraint UK_MEDPOM_EXIST_MET unique (NAME);

prompt Creating MEDPOM_CLIENT_CLAIMS...
create table MEDPOM_CLIENT_CLAIMS
(
  role_id   NUMBER not null,
  claims_id NUMBER not null
)
;
alter table MEDPOM_CLIENT_CLAIMS
  add constraint MEDPOM_CLIENT_CLAIMS_U unique (ROLE_ID, CLAIMS_ID);
alter table MEDPOM_CLIENT_CLAIMS
  add constraint MEDPOM_CLIENT_CLAIMS_C foreign key (CLAIMS_ID)
  references MEDPOM_EXIST_METHOD (ID) on delete cascade;
alter table MEDPOM_CLIENT_CLAIMS
  add constraint MEDPOM_CLIENT_CLAIMS_R foreign key (ROLE_ID)
  references MEDPOM_CLIENT_ROLES (ID) on delete cascade;

prompt Creating MEDPOM_CLIENT_USERS...
create table MEDPOM_CLIENT_USERS
(
  id   NUMBER not null,
  pass VARCHAR2(40) not null,
  name VARCHAR2(40) not null
)
;
alter table MEDPOM_CLIENT_USERS
  add constraint MEDPOM_CLIENT_USERS_ID primary key (ID);
alter table MEDPOM_CLIENT_USERS
  add constraint MEDPOM_CLIENT_USERS_NAME unique (NAME);

prompt Creating MEDPOM_CLIENT_US_ROL...
create table MEDPOM_CLIENT_US_ROL
(
  user_id NUMBER not null,
  role_id NUMBER not null
)
;
comment on column MEDPOM_CLIENT_US_ROL.user_id
  is 'ID юзера';
comment on column MEDPOM_CLIENT_US_ROL.role_id
  is 'ID роли';
alter table MEDPOM_CLIENT_US_ROL
  add constraint MEDPOM_CLIENT_US_ROL_PK primary key (USER_ID, ROLE_ID);
alter table MEDPOM_CLIENT_US_ROL
  add constraint ROLE_ID foreign key (ROLE_ID)
  references MEDPOM_CLIENT_ROLES (ID);
alter table MEDPOM_CLIENT_US_ROL
  add constraint USER_ID foreign key (USER_ID)
  references MEDPOM_CLIENT_USERS (ID);

prompt Loading MEDPOM_CLIENT_ROLES...
insert into MEDPOM_CLIENT_ROLES (id, role_name, role_comment)
values (21, '423', '234');
insert into MEDPOM_CLIENT_ROLES (id, role_name, role_comment)
values (1, 'SUPER_ADMIN', 'Может все');
prompt 2 records loaded
prompt Loading MEDPOM_EXIST_METHOD...
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('DelPack', 'УДАЛЕНИЕ ПАКЕТА', 102);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSecureCard', null, 101);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('SaveProcessArch', null, 121);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_DeleteUsers_Role', null, 22);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSVOD_SMP_TEMP1', null, 23);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSVOD_SMP_TEMP100', null, 24);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSVOD_DISP_ITOG', null, 25);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSVOD_SMP_ITOG', null, 26);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSVOD_VMP_ITOG', null, 27);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('SetTimeoutServer', null, 28);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetTimeoutServer', null, 29);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_GetMethod', null, 30);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_AddMethod', null, 31);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_DeleteMethod', null, 32);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_UpdateMethod', null, 33);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_GetRoles', null, 34);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_AddRoles', null, 35);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_DeleteRoles', null, 36);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_UpdateRoles', null, 37);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_GetRolesClaims', null, 38);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_AddClaims', null, 39);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_DeleteClaims', null, 40);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_UpdateClaims', null, 41);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_GetUsers', null, 42);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_AddUsers', null, 43);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_DeleteUsers', null, 44);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_UpdateUsers', null, 45);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_GetUsers_Roles', null, 46);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Roles_AddUsers_Role', null, 47);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('ClearEventLogEntry', null, 48);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetChekingList', null, 49);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('SetChekingList', null, 50);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetProcedureFromPack', null, 51);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetParam', null, 52);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('ExecuteCheckAv', null, 53);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('LoadChekListFromBD', null, 54);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('ClearJornal', null, 55);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('SetSettingTransfer', null, 56);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSettingTransfer', null, 57);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetTableTransfer', null, 58);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('ClearFileManagerList', null, 59);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('SetPriority', null, 60);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('SetUserPriv', null, 61);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('CheckUserPriv', null, 62);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetUserPriv', null, 63);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSumReestr', null, 64);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetNotReestr', null, 65);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSumReestrDetail', null, 66);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSVOD_SMO_TEMP1', null, 67);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSVOD_SMO_TEMP100', null, 68);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSVOD_DISP_TEMP100', null, 69);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSVOD_DISP_TEMP1', null, 70);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSVOD_VMP_TEMP1', null, 71);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSVOD_VMP_TEMP100', null, 72);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetFileManagerList', null, 73);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetJournalList', null, 74);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetJournalListByFilter', null, 75);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('SettingsFolder', null, 76);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('SettingConnect', null, 77);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSettingsFolder', null, 78);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSettingConnect', null, 79);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('isConnenct', null, 80);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetTableServer', null, 81);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('SaveProperty', null, 82);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('LoadProperty', null, 83);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('StopProccess', null, 84);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('StartProccess', null, 85);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetSchemaColection', null, 86);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('SettingSchemaColection', null, 87);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('ArchiveInviterStatus', null, 88);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('FilesInviterStatus', null, 89);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('FLKInviterStatus', null, 90);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('Ping', null, 91);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('SetMouth', null, 92);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetMouth', null, 93);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetFolderLocal', null, 94);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetFilesLocal', null, 95);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetLocalDisk', null, 96);
insert into MEDPOM_EXIST_METHOD (name, coment, id)
values ('GetEventLogEntry', null, 97);
prompt 79 records loaded
prompt Loading MEDPOM_CLIENT_CLAIMS...
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 22);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 23);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 24);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 25);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 26);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 27);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 28);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 29);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 30);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 31);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 32);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 33);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 34);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 35);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 36);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 37);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 38);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 39);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 40);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 41);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 42);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 43);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 44);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 45);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 46);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 47);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 48);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 49);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 50);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 51);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 52);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 53);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 54);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 55);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 56);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 57);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 58);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 59);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 60);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 61);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 62);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 63);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 64);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 65);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 66);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 67);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 68);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 69);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 70);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 71);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 72);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 73);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 74);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 75);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 76);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 77);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 78);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 79);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 80);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 81);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 82);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 83);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 84);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 85);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 86);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 87);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 88);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 89);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 90);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 91);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 92);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 93);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 94);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 95);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 96);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 97);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 101);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 102);
insert into MEDPOM_CLIENT_CLAIMS (role_id, claims_id)
values (1, 121);
prompt 79 records loaded
prompt Loading MEDPOM_CLIENT_USERS...
insert into MEDPOM_CLIENT_USERS (id, pass, name)
values (1, '091991', 'DEN');
insert into MEDPOM_CLIENT_USERS (id, pass, name)
values (2, '123456', 'NDV');
prompt 2 records loaded
prompt Loading MEDPOM_CLIENT_US_ROL...
insert into MEDPOM_CLIENT_US_ROL (user_id, role_id)
values (1, 1);
insert into MEDPOM_CLIENT_US_ROL (user_id, role_id)
values (2, 1);
prompt 2 records loaded
set feedback on
set define on
prompt Done.
