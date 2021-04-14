create or replace package PKG_XML_H_USL_CONTROL is

  -- Author  : NDV
  -- Created : 05.03.2015 10:10:54
  -- Purpose : 


--�������� �� IDSERV �� null
procedure EMPT_IDSERV(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� LPU �� null
procedure EMPT_LPU(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2); 
  -----------------------------------------------------------------------------------------
  --�������� LPU_1 �� null
  procedure EMPT_LPU_1(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------  
    --�������� PODR �� null
    procedure EMPT_PODR(H_USL_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  ------------asdasdasd-----------------------------------------------------------------------------
--�������� SMO �� null
  procedure EMPT_PROFIL(H_USL_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� DET
procedure EMPT_DET(H_USL_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� EMPT_DATE_IN
procedure EMPT_DATE_IN(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� DATE_OUT
procedure EMPT_DATE_OUT(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� DS
procedure EMPT_DS(H_USL_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� CODE_USL
procedure EMPT_CODE_USL(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� KOL_USL
procedure EMPT_KOL_USL(H_USL_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� TARIF
procedure EMPT_TARIF(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� SUMV_USL
procedure EMPT_SUMV_USL(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� PRVS
procedure EMPT_PRVS(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� CODE_MD
procedure EMPT_CODE_MD(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� DOUBLE_usl_id
procedure DOUBLE_usl_id(H_USL_TBL_NAME varchar2,H_USL_TBL_NAME_MAIN varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� LPU
procedure EXIST_LPU(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------

--��������� EXIST_PROFIL
procedure EXIST_PROFIL(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� EXIST_DET
procedure EXIST_DET(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� ���� 
procedure CHECK_DATE_IN_DATE_OUT(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� EXIST_DS
procedure EXIST_DS(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� EXIST_CODE_USL
procedure EXIST_CODE_USL(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� EXIST_CODE_USL2
procedure EXIST_CODE_USL2(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------

end PKG_XML_H_USL_CONTROL;
/
create or replace package body PKG_XML_H_USL_CONTROL
is
---------------------------------------------------------------------------------------
--�������� �� IDSERV �� null
procedure EMPT_IDSERV(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
  declare dml_str varchar2(10000);
  begin 
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''IDSERV (����� ������ �� ������); ''
               WHERE (IDSERV is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� LPU �� null
procedure EMPT_LPU(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE  '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''LPU (��� ��); ''
               WHERE (LPU is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;  
  -----------------------------------------------------------------------------------------
  --�������� LPU_1 �� null
  procedure EMPT_LPU_1(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
 dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''LPU_1 (��� ������������� ��); ''
               WHERE (LPU_1 is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;  
  -----------------------------------------------------------------------------------------  
    --�������� PODR �� null
    procedure EMPT_PODR(H_USL_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PODR (��� ��������� ��); ''
               WHERE (PODR is null) and NOT EXISTS (SELECT * FROM '||H_SLUCH_TBL_NAME||' t WHERE (s.sluch_id = t.sluch_id) and (t.rslt_d is not null))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  ------------asdasdasd-----------------------------------------------------------------------------
--�������� SMO �� null
  procedure EMPT_PROFIL(H_USL_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PROFIL (�������); ''
               WHERE (PROFIL is null) and NOT EXISTS (SELECT * FROM '||H_SLUCH_TBL_NAME||' t WHERE (s.sluch_id = t.sluch_id) and (t.rslt_d is not null))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� DET
procedure EMPT_DET(H_USL_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DET (������� �������� �������); ''
               WHERE (DET is null)  and NOT EXISTS (SELECT * FROM '||H_SLUCH_TBL_NAME||' t WHERE (s.sluch_id = t.sluch_id) and (t.rslt_d is not null))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� EMPT_DATE_IN
procedure EMPT_DATE_IN(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
           dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DATE_IN (���� ������ �������� ������); ''
               WHERE (DATE_IN is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� DATE_OUT
procedure EMPT_DATE_OUT(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DATE_OUT (���� ������������ �������� ������); ''
               WHERE (DATE_OUT is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� DS
procedure EMPT_DS(H_USL_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DS (�������); ''
               WHERE (DS is null)  and NOT EXISTS (SELECT * FROM '||H_SLUCH_TBL_NAME||' t WHERE (s.sluch_id = t.sluch_id) and (t.rslt_d is not null))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� CODE_USL
procedure EMPT_CODE_USL(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE_USL (��� ������); ''
               WHERE (CODE_USL is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� KOL_USL
procedure EMPT_KOL_USL(H_USL_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
       dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''KOL_USL (���������� �����); ''
               WHERE (KOL_USL is null)  and NOT EXISTS (SELECT * FROM '||H_SLUCH_TBL_NAME||' t WHERE (s.sluch_id = t.sluch_id) and (t.rslt_d is not null))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� TARIF
procedure EMPT_TARIF(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''TARIF (�����); ''
               WHERE (TARIF is null) ';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� SUMV_USL
procedure EMPT_SUMV_USL(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''SUMV_USL (����� ������); ''
               WHERE (SUMV_USL is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;

  end;
  end;
-----------------------------------------------------------------------------------------
--��������� PRVS
procedure EMPT_PRVS(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PRVS (������������� ������������); ''
               WHERE (PRVS is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� CODE_MD
procedure EMPT_CODE_MD(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE_MD (��� ������������); ''
               WHERE (CODE_MD is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� DOUBLE_usl_id
procedure DOUBLE_usl_id(H_USL_TBL_NAME varchar2,H_USL_TBL_NAME_MAIN varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''IDSERV (����� ������)-''||IDSERV||''; ''
                    WHERE (usl_id IN (SELECT t1.usl_id
                                       FROM '||H_USL_TBL_NAME_MAIN||' t2, '||H_USL_TBL_NAME_MAIN||' t1
                                       WHERE t1.sluch_id     = t2.sluch_id AND
                                             trim(t1.lpu)    = trim(t2.lpu) AND
                                             trim(t1.podr)   = trim(t2.podr) AND
                                             trim(t1.profil) = trim(t2.profil) AND
                                             t1.date_in      = t2.date_in AND
                                             t1.date_out     = t2.date_out AND
                                             t1.code_usl     = t2.code_usl AND
                                             t1.usl_id       > t2.usl_id AND
                                             t1.code_usl not in (''311036'', ''311136'', ''321035'', ''321135''))
                           )';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� LPU
procedure EXIST_LPU(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
          dml_str :=
--            'UPDATE XML_H_USL_TEMP S SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''LPU (��� ��)-''||LPU||''; ''
                 WHERE (s.LPU IS NOT NULL) AND (NOT EXISTS (SELECT * FROM nsi.F003 t WHERE s.LPU = t.MCOD ))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------

--��������� EXIST_PROFIL
procedure EXIST_PROFIL(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
 dml_str :=
--            'UPDATE XML_H_USL_TEMP S SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PROFIL (��� �������)-''||PROFIL||''; ''
                 WHERE (s.PROFIL IS NOT NULL) AND (NOT EXISTS (SELECT * FROM nsi.V002 t WHERE s.PROFIL = t.IDPR  and (s.date_out between t.datebeg and nvl(t.dateend, sysdate))))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� EXIST_DET
procedure EXIST_DET(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_USL_TEMP S SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DET (������� �������� �������)-''||DET||''; ''
                 WHERE DET NOT IN (0, 1)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� ���� 
procedure CHECK_DATE_IN_DATE_OUT(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
 dml_str :=
--            'UPDATE XML_H_USL_TEMP S SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''���� ������� -''||DATE_IN||''>''||DATE_OUT||''; ''
                 WHERE DATE_IN>DATE_OUT';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� EXIST_DS
procedure EXIST_DS(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP S SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DS (�������)-''||DS||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.MKB10 t WHERE s.DS = t.MKB  and (s.date_out between t.date_b and nvl(t.date_e, sysdate))) and (s.DS is not null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� EXIST_CODE_USL
procedure EXIST_CODE_USL(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP S SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE_USL (��� ������)-''||CODE_USL||''; ''
                 WHERE NOT EXISTS (SELECT * FROM NSI.TARIF t WHERE TRIM(s.CODE_USL) = TRIM(T.ID_TARIF) AND (NVL(S.DATE_OUT, SYSDATE) BETWEEN T.date_b AND NVL(T.date_e, SYSDATE))) and (s.CODE_USL is not null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� EXIST_CODE_USL2
procedure EXIST_CODE_USL2(H_USL_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP S SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_USL_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''� �� ������ ������ �� ����� ����������� �������� -''||CODE_USL||''; ''
                 WHERE EXISTS (select id_tarif from nsi.OPERATION_LEVEL_2B3 o where o.id_tarif=s.code_usl ) and
                       EXISTS (select lpu from nsi.MEDCARE_LEVEL m where substr(trim(upper(m.lev)), 1, 2) in (''1�'', ''1�'', ''2�'') and m.lpu=s.lpu and s.lpu not in (''750001'', ''750002'')
                               and month_end is null)  and
                        (s.CODE_USL is not null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------


end PKG_XML_H_USL_CONTROL;
/
