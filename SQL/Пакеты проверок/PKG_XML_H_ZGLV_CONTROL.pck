create or replace package PKG_XML_H_ZGLV_CONTROL is

  -- Author  : NDV
  -- Created : 05.03.2015 10:10:54
  -- Purpose :


--Проверка на версии не null
procedure EMPT_VERSION(H_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--Проверка DATA не null
procedure EMPT_DATA(H_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
  --проверка фамилии и фамилии представителя не null
  procedure EMPT_FILENAME(H_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
end PKG_XML_H_ZGLV_CONTROL;
/
create or replace package body PKG_XML_H_ZGLV_CONTROL
is
---------------------------------------------------------------------------------------
--Проверка на версии не null
procedure EMPT_VERSION(H_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_ZGLV SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_ZGLV_TBL_NAME||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''VERSION (Версия взаимодействия); ''
               WHERE (VERSION is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--Проверка DATA не null
procedure EMPT_DATA(H_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_ZGLV SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_ZGLV_TBL_NAME||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DATA (Дата); ''
               WHERE (DATA is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
  --проверка фамилии и фамилии представителя не null
  procedure EMPT_FILENAME(H_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_ZGLV SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_ZGLV_TBL_NAME||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''FILENAME (Имя файла); ''
               WHERE (FILENAME is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------


end PKG_XML_H_ZGLV_CONTROL;
/
