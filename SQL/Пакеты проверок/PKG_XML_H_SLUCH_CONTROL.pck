create or replace package PKG_XML_H_SLUCH_CONTROL is

  -- Author  : NDV
  -- Created : 05.03.2015 10:10:54
  -- Purpose : 


---------------------------------------------------------------------------------------
--�������� �� IDCASE �� null
procedure EMPT_IDCASE(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� USL_OK �� null
procedure EMPT_USL_OK(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
  --�������� VIDPOM �� null
  procedure EMPT_VIDPOM(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------  
    --�������� FOR_POM b rslt_d �� null
    procedure EMPT_FOR_POM(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  ------------asdasdasd-----------------------------------------------------------------------------
--�������� LPU �� null
  procedure EMPT_LPU(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EMPT_LPU_1(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� PODR �� null
  procedure EMPT_PODR(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� PROFIL �� null
  procedure EMPT_PROFIL(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� EMPT_NHISTORY �� null
  procedure EMPT_NHISTORY(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� DET �� null
  procedure EMPT_DET(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EMPT_DATE_1(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------


--�������� DATE_2
procedure EMPT_DATE_2(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� DS1
procedure EMPT_DS1(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
--�������� usl_ok �� null
  procedure EMPT_usl_ok1(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
--�������� RSLT �� null
  procedure EMPT_RSLT(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
--�������� ISHOD �� null
  procedure EMPT_ISHOD(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� PRVS �� null
  procedure EMPT_PRVS(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� IDDOKT �� null
  procedure EMPT_IDDOKT(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� IDSP �� null
  procedure EMPT_IDSP(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� SUMV �� null
  procedure EMPT_SUMV(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

  procedure EXIST_USL_OK(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
--�������� '||H_SLUCH_TBL_NAME||' �� null
  procedure EXIST_VIDPOM(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� USL_OK �� null
  procedure EXIST_FORPOM(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_NPR_MO(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_LPU(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� EXIST_EXTR �� null
  procedure EXIST_EXTR(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_PROFIL(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_DET(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_DATE(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
 
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_DS0(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


--�������� LPU �� null
  procedure EXIST_DS1(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS2(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS3(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS01(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS11(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS21(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS31(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_code_mes11(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_code_RSLT(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_ISHOD(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_PRVS(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_PRVS2(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_IDSP(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_RSLT_D(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_SUMV(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_NPR_MO1(H_SLUCH_TBL_NAME varchar2,H_USL_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_SUM_USL(H_SLUCH_TBL_NAME varchar2,H_USL_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
   procedure EXIST_PERIOD(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2,P_MON integer);---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_RSLT_USL(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_ISHOD_USL(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_LPU1(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_code_mes1(H_SLUCH_TBL_NAME varchar2,H_USL_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);

end PKG_XML_H_SLUCH_CONTROL;
/
create or replace package body PKG_XML_H_SLUCH_CONTROL
is
---------------------------------------------------------------------------------------
--�������� �� IDCASE �� null
procedure EMPT_IDCASE(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
  declare dml_str varchar2(10000);
  begin 
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''IDCASE (����� ������ � ������); ''
               WHERE (IDCASE is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� USL_OK �� null
procedure EMPT_USL_OK(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''USL_OK (������� �������� ���������); ''
               WHERE (USL_OK is null) AND (S.rslt_d IS NULL)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;  
  -----------------------------------------------------------------------------------------
  --�������� VIDPOM �� null
  procedure EMPT_VIDPOM(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''VIDPOM (��� ���������); ''
               WHERE (VIDPOM is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;  
  -----------------------------------------------------------------------------------------  
    --�������� FOR_POM b rslt_d �� null
    procedure EMPT_FOR_POM(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''FOR_POM (����� �������� ���������); ''
               WHERE (FOR_POM is null) AND (S.rslt_d IS NULL)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  ------------asdasdasd-----------------------------------------------------------------------------
--�������� LPU �� null
  procedure EMPT_LPU(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''LPU (��� ��); ''
               WHERE (LPU is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EMPT_LPU_1(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''LPU_1 (������������� ��); ''
               WHERE (LPU_1 is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� PODR �� null
  procedure EMPT_PODR(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PODR (��������� ��); ''
               WHERE (PODR is null) AND (S.rslt_d IS NULL)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;

  end;
  end;
-----------------------------------------------------------------------------------------
--�������� PROFIL �� null
  procedure EMPT_PROFIL(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PROFIL (�������); ''
               WHERE (PROFIL is null) AND S.rslt_d IS NULL';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� EMPT_NHISTORY �� null
  procedure EMPT_NHISTORY(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''NHISTORY (����� ������� �������/������������ �����); ''
               WHERE (NHISTORY is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� DET �� null
  procedure EMPT_DET(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DET (������� �������� �������); ''
               WHERE (DET is null) AND S.rslt_d IS NULL';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EMPT_DATE_1(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DATE_1 (���� �������� ������ ������); ''
               WHERE (DATE_1 is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------


--�������� DATE_2
procedure EMPT_DATE_2(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DATE_2 (���� �������� ������ ������); ''
               WHERE (DATE_2 is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� DS1
procedure EMPT_DS1(H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DS1 (�������� �������); ''
               WHERE (DS1 is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
--�������� usl_ok �� null
  procedure EMPT_usl_ok1(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE_MES1 (��� ���); ''
               WHERE (usl_ok in (1, 2)) and (code_mes1 is null) and (s.metod_hmp is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
--�������� RSLT �� null
  procedure EMPT_RSLT(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''RSLT (��������� ���������/��������������); ''
               WHERE (RSLT is null) AND S.rslt_d IS NULL';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
--�������� ISHOD �� null
  procedure EMPT_ISHOD(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''ISHOD (����� �����������); ''
               WHERE (ISHOD is null) AND S.rslt_d IS NULL';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� PRVS �� null
  procedure EMPT_PRVS(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PRVS (������������� �������� �����); ''
               WHERE (PRVS is null) AND S.rslt_d IS NULL';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� IDDOKT �� null
  procedure EMPT_IDDOKT(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''IDDOKT (��� �����); ''
               WHERE (IDDOKT is null) AND S.rslt_d IS NULL';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� IDSP �� null
  procedure EMPT_IDSP(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''IDSP (������ ������ ���������); ''
               WHERE (IDSP is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� SUMV �� null
  procedure EMPT_SUMV(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''SUMV (�����, ������������ � ������); ''
               WHERE (SUMV is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� '||H_SLUCH_TBL_NAME||' �� null
  procedure EXIST_USL_OK(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin

     -- err := 'H_SLUCH_USL_OK; ';
   --   err_prim := '������������ ���������� ';
    --  err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''USL_OK (��� ������� �������� ����������� ������)-''||USL_OK||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V006 t WHERE s.USL_OK = t.IDUMP and (s.date_2 between t.datebeg and nvl(t.dateend, sysdate))) and s.USL_OK is not null';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� '||H_SLUCH_TBL_NAME||' �� null
  procedure EXIST_VIDPOM(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''VIDPOM (��� ���� ����������� ������)-''||VIDPOM||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V008 t WHERE s.VIDPOM = t.IDVMP and t.actual=1 and (s.date_2 between t.datebeg and nvl(t.dateend, sysdate))) AND (s.VIDPOM is not null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� USL_OK �� null
  procedure EXIST_FORPOM(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      --err := 'H_SLUCH_FORPOM; ';
      --err_prim := '������������ ���������� ';
      --err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''FOR_POM (��� ����� �������� ����������� ������)-''||FOR_POM||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V014 t WHERE s.FOR_POM = t.IDFRMMP  and (s.date_2 between t.datebeg and nvl(t.dateend, sysdate))) and s.FOR_POM is not null';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_NPR_MO(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
begin
     -- err := 'H_SLUCH_NPR_MO; ';
    -- err_prim := '������������ ���������� ';
    --  err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''NPR_MO (���������� ����� ��, ������������ �� �������)-''||NPR_MO||''; ''
                 WHERE (s.NPR_MO IS NOT NULL) AND
                       (NOT EXISTS (SELECT * FROM nsi.F003 t WHERE trim(to_char(s.NPR_MO)) = trim(to_char(t.MCOD)) ))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_LPU(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_LPU; ';
     -- err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''LPU (���������� ����� ��)-''||LPU||''; ''
                 WHERE (s.LPU IS NOT NULL) AND (NOT EXISTS (SELECT * FROM nsi.F003 t WHERE s.LPU = t.MCOD ))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� EXIST_EXTR �� null
  procedure EXIST_EXTR(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      --err := 'H_SLUCH_EXTR; ';
     -- err_prim := '������������ ���������� ';
    --  err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''EXTR (��� ���� ��������������)-''||EXTR||''; ''
                 WHERE (S.USL_OK IN (1)) AND NOT EXISTS (SELECT * FROM nsi.EXTR t WHERE s.EXTR = t.id_extr )';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_PROFIL(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
--err := 'H_SLUCH_PROFIL; ';
  --    err_prim := '������������ ���������� ';
   --   err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PROFIL (��� �������)-''||PROFIL||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V002 t WHERE s.PROFIL = t.IDPR  and (s.date_2 between t.datebeg and nvl(t.dateend, sysdate))) and S.PROFIL is not null';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_DET(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_DET; ';
     -- err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DET (������� �������� �������)-''||DET||''; ''
                 WHERE DET NOT IN (0, 1)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_DATE(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      --err := 'H_SLUCH_DATE; ';
     -- err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE  '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''(���� ��������� ������ ������ ���� ������)-''||DATE_1||''>''||DATE_2||''; ''
                 WHERE DATE_1>DATE_2';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--�������� USL_OK �� null
  procedure EXIST_DS0(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_DS; ';
     -- err_prim := '������������ ���������� ';
    --  err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DS0 (������� ���������)-''||DS0||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.MKB10 t WHERE s.DS0 = t.MKB  and (s.date_2 between t.date_b and nvl(t.date_e, sysdate))) and (s.DS0 is not null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


--�������� LPU �� null
  procedure EXIST_DS1(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      --err := 'H_SLUCH_DS; ';
     -- err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DS1 (������� ��������)-''||DS1||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.MKB10 t WHERE s.DS1 = t.MKB and (s.date_2 between t.date_b and nvl(t.date_e, sysdate))) and (s.DS1 is not null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS2(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_DS; ';
      --err_prim := '������������ ���������� ';
      --err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DS2 (������� �������������)-''||DS2||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.MKB10 t WHERE s.DS2 = t.MKB and (s.date_2 between t.date_b and nvl(t.date_e, sysdate))) and (s.DS2 is not null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS3(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      --err := 'H_SLUCH_DS; ';
     -- err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
             'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DS3 (������� ����������)-''||DS3||''; ''
                WHERE NOT EXISTS (SELECT * FROM nsi.MKB10 t WHERE s.DS3 = t.MKB and (s.date_2 between t.date_b and nvl(t.date_e, sysdate))) and (s.DS3 is not null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS01(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_DS; ';
    -- err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DS0 (��� ���������������� �������� �� ������� ����������)-''||DS0||''; ''
                 WHERE EXISTS (SELECT * FROM nsi.MKB10 t WHERE s.DS0 = t.MKB and (s.date_2 between t.date_b and nvl(t.date_e, sysdate))) and (s.DS0 is not null) and
                       (length(to_char(trim(s.ds0)))=3 and (select count(*) from nsi.mkb10 m10 where m10.mkb like trim(s.ds0)||''%'')>1 ) ';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS11(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      --err := 'H_SLUCH_DS; ';
    --  err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DS1 (��� ��������� �������� �� ������� ����������)-''||DS1||''; ''
                 WHERE EXISTS (SELECT * FROM nsi.MKB10 t WHERE s.DS1 = t.MKB and (s.date_2 between t.date_b and nvl(t.date_e, sysdate))) and (s.DS1 is not null) and
                       (length(to_char(trim(s.ds1)))=3 and (select count(*) from nsi.mkb10 m10 where m10.mkb like trim(s.ds1)||''%'')>1 ) ';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS21(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      --err := 'H_SLUCH_DS; ';
     -- err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DS2 (��� �������������� �������� �� ������� ����������)-''||DS2||''; ''
                 WHERE EXISTS (SELECT * FROM nsi.MKB10 t WHERE s.DS2 = t.MKB and (s.date_2 between t.date_b and nvl(t.date_e, sysdate))) and (s.DS2 is not null) and
                       (length(to_char(trim(s.ds2)))=3 and (select count(*) from nsi.mkb10 m10 where m10.mkb like trim(s.ds2)||''%'')>1 ) ';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_DS31(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_DS; ';
     -- err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DS3 (��� �������� ���������� ����������� �� ������� ����������)-''||DS3||''; ''
                 WHERE EXISTS (SELECT * FROM nsi.MKB10 t WHERE s.DS3 = t.MKB and (s.date_2 between t.date_b and nvl(t.date_e, sysdate))) and (s.DS3 is not null) and
                       (length(to_char(trim(s.ds3)))=3 and (select count(*) from nsi.mkb10 m10 where m10.mkb like trim(s.ds3)||''%'')>1 ) ';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_code_mes11(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_code_mes1; ';
     -- err_prim := '������������ ���������� ';
    --  err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''code_mes1 (��� ���)-''||code_mes1||''; ''
                 WHERE (usl_ok in (1, 2)) and
                       (s.metod_hmp is null) and
                       NOT EXISTS (SELECT * FROM nsi.KSG t WHERE s.code_mes1 = t.ID_KSG )';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_code_RSLT(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
 --err := 'H_SLUCH_RSLT; ';
   --   err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''RSLT (��� ���������� ���������)-''||RSLT||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V009 t WHERE s.RSLT = t.IDRMP  and (s.date_2 between t.datebeg and nvl(t.dateend, sysdate))) and s.rslt is not null';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_ISHOD(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_ISHOD; ';
   ---   err_prim := '������������ ���������� ';
   --   err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''ISHOD (��� ������ �����������)-''||ISHOD||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V012 t WHERE s.ISHOD = t.IDIZ  and (s.date_2 between t.datebeg and nvl(t.dateend, sysdate))) and s.ISHOD is not null';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_PRVS(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
--err := 'H_SLUCH_PRVS; ';
 --     err_prim := '������������ ���������� ';
   --   err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PRVS (��� ������������� ����� V015)-''||PRVS||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V015 t WHERE s.PRVS = t.CODE  and (s.date_2 between t.datebeg and nvl(t.dateend, sysdate))) AND UPPER(VERS_SPEC)=''V015'' and s.PRVS is not null';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_PRVS2(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_PRVS; ';
    --  err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PRVS (��� ������������� ����� V004)-''||PRVS||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V004 t WHERE s.PRVS = t.IDMSP  and (s.date_2 between t.datebeg and nvl(t.dateend, sysdate))) AND UPPER(NVL(VERS_SPEC, ''V004''))=''V004'' and s.PRVS is not null';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_IDSP(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      --err := 'H_SLUCH_IDSP; ';
   --   err_prim := '������������ ���������� ';
   --   err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''IDSP (��� ������� ������)-''||IDSP||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V010 t WHERE s.IDSP = t.IDSP  and (s.date_2 between t.datebeg and nvl(t.dateend, sysdate))) AND S.IDSP IS NOT NULL';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_RSLT_D(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
--err := 'H_SLUCH_RSLT_D; ';
   --   err_prim := '������������ ���������� ';
   --   err_type := '����������� ������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''RSLT_D (��������� ���������������)-''||RSLT_D||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V017 t WHERE s.rslt_d = t.IDDR  and (s.date_2 between t.datebeg and nvl(t.dateend, sysdate))) AND S.rslt_d IS NOT NULL';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_SUMV(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_SUMV; ';
    --  err_prim := '����� �� ������ ����� 0';
    --  err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''; ''
               WHERE (SUMV=0) and (s.usl_ok<>4)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_NPR_MO1(H_SLUCH_TBL_NAME varchar2,H_USL_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
--err := 'H_SLUCH_NPR_MO; ';
      --err_prim := '�� ������� ����������� �� ';
    --  err_type := '������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''; ''
   WHERE (nvl(s.extr, 0)<>2) and
         (nvl(s.for_pom, 0) not in (1, 2)) and
         (npr_mo is null) and
         exists (select t.kod_r
                   from (select ls.kod_r from nsi.lpu l inner join nsi.lpu_s ls on l.mcod=ls.mkod
                          where fond is null and
                                date_e is null and
                                mcod not in (''0352006'', ''2148003'', ''0352005'', ''0352002'', ''7101002'', ''7101003'')
                         ) t where trim(s.lpu)=trim(t.kod_r)) AND
         exists (select s.sluch_id
                   from '||H_USL_TBL_NAME||' t
                   where s.sluch_id=t.sluch_id and
                         trim(t.code_usl) not in (/*''311036'', ''311136'', ''321035'', ''321135'', ''601035'', ''602035'', ''311051'', */ ''321043'')) AND

         ((nvl(s.os_sluch, ''0'') not like  ''%7%'') and
          (nvl(s.os_sluch, ''0'') not like  ''%8%''))';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_SUM_USL(H_SLUCH_TBL_NAME varchar2,H_USL_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
--err := 'H_SLUCH_SUM_USL; ';
   --   err_prim := '����� �� ������� ����������; ';
   --   err_type := '����������� ������. ';
      dml_str :=
--            'UPDATE XML_H_SCHET_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''; ''
                 WHERE EXISTS (SELECT * FROM (select s.sluch_id, sum(U.SUMV_USL)-s.SumV AS DEFF
                                                from '||H_SLUCH_TBL_NAME||' s inner join '||H_USL_TBL_NAME||' U on s.sluch_id = U.SLUCH_ID
                                                group by s.sluch_id, s.SumV) T WHERE S.sluch_id = T.sluch_id AND DEFF<>0)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_PERIOD(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2,P_MON integer)
  as
  begin 
declare dml_str varchar2(30000);
     begin
       --err := 'H_SLUCH_PERIOD; ';
   -- err_prim := '������ �� ������ � �������� ������; ';
   --  err_type := '������. ';
    dml_str :=
--            'UPDATE XML_H_SCHET_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
         'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''; ''
             WHERE extract(month from s.date_2)<>'||p_MON;
    EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
    COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_RSLT_USL(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
       --err := 'H_SLUCH_RSLT_USL; ';
      --err_prim := '������������ ���������� ';
     -- err_type := '������. ';
      dml_str :=
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err,
                                           err_prim = err_prim||:err_type||:err_prim||''RSLT_USL (��� ���������� ��������� �� ������������� ������� �������� ��)-''||RSLT||'' - ''||USL_OK||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V009 t WHERE s.RSLT = t.IDRMP  and
                                                                 (s.date_2 between t.datebeg and nvl(t.dateend, sysdate)) and
                                                                 (s.usl_ok=t.dl_uslov)) and
                       (s.rslt is not null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_ISHOD_USL(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_ISHOD_USL; ';
      --err_prim := '������������ ���������� ';
      --err_type := '������. ';
      dml_str :=
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err,
                                           err_prim = err_prim||:err_type||:err_prim||''RSLT_USL (��� ���������� ��������� �� ������������� ������� �������� ��)-''||ISHOD||'' - ''||USL_OK||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.V012 t WHERE s.ISHOD = t.idiz  and
                                                                 (s.date_2 between t.datebeg and nvl(t.dateend, sysdate)) and
                                                                 (s.usl_ok=t.dl_uslov)) and
                       (s.ishod is not null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_LPU1(H_SLUCH_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
     -- err := 'H_SLUCH_LPU1; ';
    --  err_prim := '������������ ���������� ';
     -- err_type := '����������� ������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err,
                                           err_prim = err_prim||:err_type||:err_prim||''LPU_1 (������������� ��); ''
               WHERE NOT EXISTS (SELECT * FROM nsi.t001 t WHERE s.lpu = t.mcod  and
                                                                (s.date_2 between t.d_start and nvl(t.data_e, sysdate)) and
                                                                (s.LPU_1=t.nom_podr)) and
                    (LPU_1 is not null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--�������� LPU �� null
  procedure EXIST_code_mes1(H_SLUCH_TBL_NAME varchar2,H_USL_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
      --err := 'H_SLUCH_CODE_USL; ';
    --  err_prim := '������������ ���������� ';
    --  err_type := '����������� ������. ';
      dml_str :=
--            'UPDATE XML_H_SLUCH_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_SLUCH_TBL_NAME||' s SET err = err|| :err,
                                           err_prim = err_prim||:err_type||:err_prim||''USL_OK (�������������� ������� �������� ����������� ������ � ���� ������); ''
               WHERE NOT EXISTS (SELECT *
                                   FROM '||H_USL_TBL_NAME||' u inner join nsi.tarif t on u.code_usl=t.id_tarif and u.date_in between t.date_b and nvl(t.date_e, sysdate)
                                   WHERE s.usl_ok = t.ps) and
                    (usl_ok is not null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



end PKG_XML_H_SLUCH_CONTROL;
/
