create or replace package PKG_XML_L_PERS_CONTROL is

  -- Author  : NDV
  -- Created : 05.03.2015 10:10:54
  -- Purpose : 


--�������� �� ��������� �� null
procedure EMPT_ID_PAC(L_PERS_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2); 
-----------------------------------------------------------------------------------------
--�������� ���� �� null
procedure EMPT_W(L_PERS_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2); 
  -----------------------------------------------------------------------------------------
  --�������� ������� � ������� ������������� �� null
  procedure EMPT_FAM(L_PERS_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2); 
  -----------------------------------------------------------------------------------------  
    --�������� ����� � ����� ������������� �� null
    procedure EMPT_IM(L_PERS_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);  
  -----------------------------------------------------------------------------------------
--�������� �������� � �������� ������������� �� null
  procedure EMPT_OT(L_PERS_TBL_NAME varchar2,
    L_ZGLV_TBL_NAME varchar2,
    H_ZGLV_TBL_NAME varchar2,
    H_SCHET_TBL_NAME varchar2,
    H_ZAP_TBL_NAME varchar2,
    H_PACIENT_TBL_NAME varchar2,
    H_SLUSH_TBL_NAME varchar2,
    err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� �� � �� ������������� �� null
procedure EMPT_DR(L_PERS_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);

end PKG_XML_L_PERS_CONTROL;
/
create or replace package body PKG_XML_L_PERS_CONTROL
is
---------------------------------------------------------------------------------------
--�������� �� ��������� �� null
procedure EMPT_ID_PAC(L_PERS_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
       dml_str := 'UPDATE '||L_PERS_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''ID_PAC (����� ������ � ��������); ''
               WHERE (ID_PAC is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� ���� �� null
procedure EMPT_W(L_PERS_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
 dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||L_PERS_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''W (��� ��������); ''
               WHERE (W is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;  
  -----------------------------------------------------------------------------------------
  --�������� ������� � ������� ������������� �� null
  procedure EMPT_FAM(L_PERS_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||L_PERS_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''FAM (������� ��������); ''
               WHERE ((FAM is null) and (FAM_P is null))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;  
  -----------------------------------------------------------------------------------------  
    --�������� ����� � ����� ������������� �� null
    procedure EMPT_IM(L_PERS_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||L_PERS_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''IM (��� ��������); ''
               WHERE ((IM is null) and (IM_P is null))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
--�������� �������� � �������� ������������� �� null
  procedure EMPT_OT(L_PERS_TBL_NAME varchar2,
    L_ZGLV_TBL_NAME varchar2,
    H_ZGLV_TBL_NAME varchar2,
    H_SCHET_TBL_NAME varchar2,
    H_ZAP_TBL_NAME varchar2,
    H_PACIENT_TBL_NAME varchar2,
    H_SLUSH_TBL_NAME varchar2,
    err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
 dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||L_PERS_TBL_NAME||' p SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''OT (�������� ��������); ''
               WHERE p.pers_id in (select lp.pers_id
                                    from '||H_ZGLV_TBL_NAME||' hz inner join '||L_ZGLV_TBL_NAME||' lz on hz.filename=lz.filename1
                                                            inner join '||H_SCHET_TBL_NAME||' sc on hz.zglv_id=sc.zglv_id
                                                            inner join '||H_ZAP_TBL_NAME||' z on sc.schet_id=z.schet_id
                                                            inner join '||H_PACIENT_TBL_NAME||' p on p.zap_id=z.zap_id
                                                            inner join '||H_SLUSH_TBL_NAME||' s on s.zap_id=z.zap_id
                                                            inner join '||L_PERS_TBL_NAME||' lp on p.id_pac=lp.id_pac and lp.zglv_id=lz.zglv_id
                                    where (lp.ot is null) and
                                          (lp.ot_p is null) and
                                          (s.os_sluch not like ''%2;%''))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� �� � �� ������������� �� null
procedure EMPT_DR(L_PERS_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_USL_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||L_PERS_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''DR (���� �������� ��������); ''
               WHERE ((DR is null) and (DR_P is null))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------

end PKG_XML_L_PERS_CONTROL;
/
