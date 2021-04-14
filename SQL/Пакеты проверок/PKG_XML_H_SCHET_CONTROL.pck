create or replace package PKG_XML_H_SCHET_CONTROL is

  -- Author  : NDV
  -- Created : 05.03.2015 10:10:54
  -- Purpose :


procedure EMPT_CODE(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� CODE_MO �� null
procedure EMPT_CODE_MO(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
  --�������� YEAR �� null
  procedure EMPT_YEAR(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
  --�������� MONTH �� null
  procedure EMPT_MONTH(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --�������� NSCHET �� null
  procedure EMPT_NSCHET(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --�������� DSCHET �� null
  procedure EMPT_DSCHET(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --�������� PLAT �� null
  procedure EMPT_PLAT(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --�������� SUMMAV �� null
  procedure EMPT_SUMMAV(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
    -----------------------------------------------------------------------------------------
    --�������� H_SCHET_CODE ���������� �������� ����
  procedure CODE_DOUBLE(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --�������� CODE_MO �� ������������
  procedure CODE_MO_EXISTS(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --�������� plat = 75000 �� null
  procedure PLAT_TFOMS(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --�������� schet_id !!!!!!!!!!!!!!!
  procedure schet_id(XML_H_SCHET_TEMP varchar2,XML_H_ZAP_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --�������� zglv_id
  procedure ZGLV_ID_EXIST(XML_H_SCHET_TEMP varchar2,XML_H_ZGLV_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
    --�������� SUM_SCHET
  procedure SUM_SCHET(XML_H_SCHET_TEMP varchar2,xml_h_zap_TEMP varchar2,xml_h_sluch_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------

end PKG_XML_H_SCHET_CONTROL;
/
create or replace package body PKG_XML_H_SCHET_CONTROL
is
---------------------------------------------------------------------------------------
--�������� �� CODE �� null
procedure EMPT_CODE(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE (��� ������ �����); ''
               WHERE (CODE is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� CODE_MO �� null
procedure EMPT_CODE_MO(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE_MO (���������� ����� ��); ''
               WHERE (CODE_MO is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
  --�������� YEAR �� null
  procedure EMPT_YEAR(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''YEAR (�������� ���); ''
               WHERE (YEAR is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
  --�������� MONTH �� null
  procedure EMPT_MONTH(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''MONTH (�������� �����); ''
               WHERE (MONTH is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --�������� NSCHET �� null
  procedure EMPT_NSCHET(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''NSCHET (����� �����); ''
               WHERE (NSCHET is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --�������� DSCHET �� null
  procedure EMPT_DSCHET(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DSCHET (���� ����������� �����); ''
               WHERE (DSCHET is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --�������� PLAT �� null
  procedure EMPT_PLAT(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PLAT (���������� ����� �����); ''
               WHERE (PLAT is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --�������� SUMMAV �� null
  procedure EMPT_SUMMAV(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_SCHET SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''SUMMAV (����� � ������); ''
               WHERE (SUMMAV is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --�������� H_SCHET_CODE ���������� �������� ����
  procedure CODE_DOUBLE(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_SCHET_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE (��� ������ �����) - ''||CODE||''; ''
                 WHERE CODE||CODE_MO IN (select CODE||CODE_MO
                               from '||XML_H_SCHET_TEMP||'
                               group by CODE||CODE_MO
                               having count(*)>1)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --�������� CODE_MO �� ������������
  procedure CODE_MO_EXISTS(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
     dml_str :=
--            'UPDATE XML_H_SCHET_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''CODE_MO (��� ��) - ''||CODE_MO||''; ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.F003 t WHERE s.code_mo = t.mcod )';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --�������� plat = 75000 �� null
  procedure PLAT_TFOMS(XML_H_SCHET_TEMP varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
declare dml_str varchar2(10000);
     begin
           dml_str :=
--            'UPDATE XML_H_SCHET_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||XML_H_SCHET_TEMP||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''PLAT (���������� ����� �����) - ''||PLAT||''; ''
                 WHERE PLAT=''75000''';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
    --�������� schet_id !!!!!!!!!!!!!!!
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
    --�������� zglv_id
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
    --�������� SUM_SCHET
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
