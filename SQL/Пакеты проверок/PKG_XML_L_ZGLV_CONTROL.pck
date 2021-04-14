create or replace package PKG_XML_L_ZGLV_CONTROL is

  -- Author  : NDV
  -- Created : 05.03.2015 10:10:54
  -- Purpose :


--Проверка на версии не null
procedure EMPT_VERSION(L_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--Проверка DATA не null
procedure EMPT_DATA(L_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
  --проверка фамилии и фамилии представителя не null
  procedure EMPT_FILENAME(L_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --проверка FILENAME1 не null
    procedure EMPT_FILENAME1(L_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
end PKG_XML_L_ZGLV_CONTROL;
/
create or replace package body PKG_XML_L_ZGLV_CONTROL
is
---------------------------------------------------------------------------------------
--Проверка на версии не null
procedure EMPT_VERSION(L_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||L_ZGLV_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''VERSION (Версия формата); ''
               WHERE (VERSION is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--Проверка DATA не null
procedure EMPT_DATA(L_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||L_ZGLV_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DATA (Дата файла); ''
               WHERE (DATA is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
  --проверка фамилии и фамилии представителя не null
  procedure EMPT_FILENAME(L_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||L_ZGLV_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''FILENAME (Имя файла пациента); ''
               WHERE (FILENAME is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --проверка FILENAME1 не null
    procedure EMPT_FILENAME1(L_ZGLV_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||L_ZGLV_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''FILENAME1 (Имя файла с услугами); ''
               WHERE (FILENAME1 is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------

end PKG_XML_L_ZGLV_CONTROL;
/
