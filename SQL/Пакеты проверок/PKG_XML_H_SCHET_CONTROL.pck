create or replace package PKG_XML_H_SCHET_CONTROL is

  -- Author  : NDV
  -- Created : 05.03.2015 10:10:54
  -- Purpose :


procedure EMPT_CODE(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--Проверка CODE_MO не null
procedure EMPT_CODE_MO(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
  --проверка YEAR не null
  procedure EMPT_YEAR(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
  --проверка MONTH не null
  procedure EMPT_MONTH(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --проверка NSCHET не null
  procedure EMPT_NSCHET(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --проверка DSCHET не null
  procedure EMPT_DSCHET(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --проверка PLAT не null
  procedure EMPT_PLAT(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --проверка SUMMAV не null
  procedure EMPT_SUMMAV(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
    -----------------------------------------------------------------------------------------
    --проверка H_SCHET_CODE Задвоенное значение поля
  procedure CODE_DOUBLE(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --проверка CODE_MO со справочникам
  procedure CODE_MO_EXISTS(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --проверка plat = 75000 не null
  procedure PLAT_TFOMS(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --проверка schet_id !!!!!!!!!!!!!!!
  procedure schet_id(XML_H_SCHET_TEMP varchar2,XML_H_ZAP_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --проверка zglv_id
  procedure ZGLV_ID_EXIST(XML_H_SCHET_TEMP varchar2,XML_H_ZGLV_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --проверка SUM_SCHET
  procedure SUM_SCHET(XML_H_SCHET_TEMP varchar2,xml_h_zap_TEMP varchar2,xml_h_sluch_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------

end PKG_XML_H_SCHET_CONTROL;
/
create or replace package body PKG_XML_H_SCHET_CONTROL
is
---------------------------------------------------------------------------------------
--Проверка на CODE не null
procedure EMPT_CODE(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE (Код записи счета); ''
               WHERE (CODE is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--Проверка CODE_MO не null
procedure EMPT_CODE_MO(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE_MO (Реестровый номер МО); ''
               WHERE (CODE_MO is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
  --проверка YEAR не null
  procedure EMPT_YEAR(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''YEAR (Отчетный год); ''
               WHERE (YEAR is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
  --проверка MONTH не null
  procedure EMPT_MONTH(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''MONTH (Отчетный месяц); ''
               WHERE (MONTH is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --проверка NSCHET не null
  procedure EMPT_NSCHET(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''NSCHET (Номер счета); ''
               WHERE (NSCHET is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --проверка DSCHET не null
  procedure EMPT_DSCHET(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DSCHET (Дата выставления счета); ''
               WHERE (DSCHET is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --проверка PLAT не null
  procedure EMPT_PLAT(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PLAT (Реестровый номер ТФОМС); ''
               WHERE (PLAT is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --проверка SUMMAV не null
  procedure EMPT_SUMMAV(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''SUMMAV (Сумма к оплате); ''
               WHERE (SUMMAV is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --проверка H_SCHET_CODE Задвоенное значение поля
  procedure CODE_DOUBLE(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_SCHET_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE (Код записи счета) - ''||CODE||''; ''
                 WHERE CODE||CODE_MO IN (select CODE||CODE_MO
                               from '||XML_H_SCHET_TEMP||'
                               group by CODE||CODE_MO
                               having count(*)>1)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --проверка CODE_MO со справочникам
  procedure CODE_MO_EXISTS(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_SCHET_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE_MO (Код МО) - ''||CODE_MO||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.F003 t WHERE s.code_mo = t.mcod )';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --проверка plat = 75000 не null
  procedure PLAT_TFOMS(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
           dml_str :=
--            'UPDATE XML_H_SCHET_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PLAT (Реестровый номер ТФОМС) - ''||PLAT||''; ''
                 WHERE PLAT=''75000''';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --проверка schet_id !!!!!!!!!!!!!!!
  procedure schet_id(XML_H_SCHET_TEMP varchar2,XML_H_ZAP_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SCHET_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim
                 WHERE NOT EXISTS (SELECT * FROM '||XML_H_ZAP_TEMP||' t WHERE s.schet_id = t.schet_id )';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --проверка zglv_id
  procedure ZGLV_ID_EXIST(XML_H_SCHET_TEMP varchar2,XML_H_ZGLV_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
         dml_str :=
--            'UPDATE XML_H_SCHET_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim
                 WHERE NOT EXISTS (SELECT * FROM '||XML_H_ZGLV_TEMP||' t WHERE s.zglv_id = t.zglv_id )';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --проверка SUM_SCHET
  procedure SUM_SCHET(XML_H_SCHET_TEMP varchar2,xml_h_zap_TEMP varchar2,xml_h_sluch_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_SCHET_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim
                 WHERE EXISTS (SELECT * FROM (select sc.schet_id, sum(sl.sumv)-sc.summav AS DEFF
                                                from '||XML_H_SCHET_TEMP||' sc inner join '||xml_h_zap_TEMP||' z on sc.schet_id = z.schet_id
                                                                          inner join '||xml_h_sluch_TEMP||' sl on z.zap_id = sl.zap_id
                                                group by sc.schet_id, sc.summav
                                              ) T WHERE S.SCHET_ID = T.SCHET_ID AND DEFF<>0)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------


end PKG_XML_H_SCHET_CONTROL;
/
