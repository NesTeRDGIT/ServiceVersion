-- Create table
create table XML_H_SANK_SMO
(
  sluch_id NUMBER,
  sank_id  NUMBER,
  s_code   VARCHAR2(36),
  s_sum    NUMBER(15,2),
  s_tip    NUMBER(1),
  s_osn    NUMBER(3),
  s_com    VARCHAR2(250),
  s_ist    NUMBER(1) default 1
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