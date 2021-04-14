create or replace package PKG_XML_H_ZAP_CONTROL is

  -- Author  : NDV
  -- Created : 05.03.2015 10:10:54
  -- Purpose :


--Проверка на N_ZAP не null
procedure EMPT_N_ZAP(XML_H_ZAP_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--Проверка PR_NOV не null
procedure EMPT_PR_NOV(XML_H_ZAP_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
  --проверка PR_NOV
  procedure PR_NOV_EXISTS(XML_H_ZAP_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
end PKG_XML_H_ZAP_CONTROL;
/
create or replace package body PKG_XML_H_ZAP_CONTROL
is
---------------------------------------------------------------------------------------
--Проверка на N_ZAP не null
procedure EMPT_N_ZAP(XML_H_ZAP_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_ZAP_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_ZAP_TEMP||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''N_ZAP (Номер записи); ''
               WHERE (N_ZAP is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--Проверка PR_NOV не null
procedure EMPT_PR_NOV(XML_H_ZAP_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_ZAP_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_ZAP_TEMP||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PR_NOV (Признак исправленной записи); ''
               WHERE (PR_NOV is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
  --проверка PR_NOV
  procedure PR_NOV_EXISTS(XML_H_ZAP_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_ZAP_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_ZAP_TEMP||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PR_NOV (Признак исправленной записи) - ''||PR_NOV||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.PR_NOV t WHERE s.pr_nov = t.id_pr_nov )';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
end PKG_XML_H_ZAP_CONTROL;
/
