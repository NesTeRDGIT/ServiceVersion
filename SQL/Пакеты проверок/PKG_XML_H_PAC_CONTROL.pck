create or replace package PKG_XML_H_PAC_CONTROL is

  -- Author  : NDV
  -- Created : 05.03.2015 10:10:54
  -- Purpose : 


---------------------------------------------------------------------------------------
--�������� �� ��������� �� null
procedure EMPT_ID_PAC(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� VPOLIS �� null
procedure EMPT_VPOLIS(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
  --�������� NPOLIS �� null
  procedure EMPT_NPOLIS(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------  
    --�������� NOVOR �� null
    procedure EMPT_NOVOR(H_PAC_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
  -----------------------------------------------------------------------------------------
--�������� SMO �� null
  procedure EMPT_SMO(H_PAC_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� --��������� �����
procedure EXIST_VPOLIS(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� ����� ������
procedure EXIST_SPOLIS(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� ����� ������
procedure EXIST_NPOLIS(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--��������� ���
procedure EXIST_SMO(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
-----------------------------------------------------------------------------------------
--�������� novor
procedure EXIST_NOVOR(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);
--�������� ZAP
procedure EMPT_ZAP(H_PAC_TBL_NAME varchar2,H_ZAP_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2);

end PKG_XML_H_PAC_CONTROL;
/
create or replace package body PKG_XML_H_PAC_CONTROL
is
---------------------------------------------------------------------------------------
--�������� �� ��������� �� null
procedure EMPT_ID_PAC(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin
  declare dml_str varchar2(10000);
  begin 
dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP s sSET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''ID_PAC (��� ������ � ��������); ''
               WHERE (ID_PAC is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� VPOLIS �� null
procedure EMPT_VPOLIS(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
 dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP s sSET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''VPOLIS (��� ����); ''
               WHERE (VPOLIS is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;  
  -----------------------------------------------------------------------------------------
  --�������� NPOLIS �� null
  procedure EMPT_NPOLIS(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP s sSET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''NPOLIS (����� ����); ''
               WHERE (NPOLIS is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;  
  -----------------------------------------------------------------------------------------  
    --�������� NOVOR �� null
    procedure EMPT_NOVOR(H_PAC_TBL_NAME varchar2,H_SLUCH_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
  dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP s sSET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' p SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''NOVOR (������� �������������); ''
               WHERE (NOVOR is null) and EXISTS (SELECT * FROM '||H_SLUCH_TBL_NAME||' t WHERE p.zap_id=t.zap_id and t.rslt_d is null)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
  -----------------------------------------------------------------------------------------
--�������� SMO �� null
  procedure EMPT_SMO(H_PAC_TBL_NAME varchar2, err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(30000);
     begin
       dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP s sSET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''������ � ��� � ���������� �����������; ''
               WHERE ((SMO is null) AND (SMO_OGRN is null) AND (SMO_OK is null) AND (SMO_NAM is null))';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� --��������� �����
procedure EXIST_VPOLIS(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP s SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''VPOLIS (��� ���� ������ �����������); ''
                 WHERE NOT EXISTS (SELECT * FROM nsi.F008 t WHERE s.VPOLIS = t.iddoc )';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� ����� ������
procedure EXIST_SPOLIS(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP s SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''SPOLIS (������������ ����� ������ ��� ���������� ���� ����); ''
                 WHERE (s.VPOLIS in (2, 3) and s.SPOLIS is not null) or (s.VPOLIS in (1) and s.SPOLIS is null)';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� ����� ������
procedure EXIST_NPOLIS(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP s SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''NPOLIS (������������ ����� ������ ������ ��� ���������� ���� ����); ''
                 WHERE ((s.VPOLIS = 3) and (length(trim(s.NPOLIS)) <> 16)) or ((s.VPOLIS = 2) and (length(trim(s.NPOLIS)) <> 9))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--��������� ���
procedure EXIST_SMO(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
           dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP s SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''SMO (���������� ����� ��� �� ������ � �����������); ''
                 WHERE (s.smo is not null) and (NOT EXISTS (SELECT * FROM nsi.F002 t WHERE to_char(s.smo) = to_char(t.smocod) ))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
      dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP s SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''�������������� ���� ��� � ����� ���������� �����������; ''
                 WHERE ((s.smo_ogrn is not null) and (s.smo_ok is not null)) and (NOT EXISTS (SELECT * FROM nsi.F002 t WHERE s.smo_ogrn = t.ogrn  and s.smo_ok = t.tf_okato))';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------
--�������� novor
procedure EXIST_NOVOR(H_PAC_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''NOVOR (������������ ������� �������������); ''
               WHERE length(trim(novor)) not in (1, 8, 9)';

      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------


--�������� ZAP
procedure EMPT_ZAP(H_PAC_TBL_NAME varchar2,H_ZAP_TBL_NAME varchar2,err varchar2, err_prim varchar2, err_type varchar2)
  as
  begin 
declare dml_str varchar2(10000);
     begin
      dml_str :=
--            'UPDATE XML_H_PACIENT_TEMP SET err = err|| :err, err_prim = err_prim||:err_prim
            'UPDATE '||H_PAC_TBL_NAME||' s SET err = err|| :err, err_prim = err_prim||:err_type||:err_prim||''; ''
                 WHERE NOT EXISTS (SELECT * FROM '||H_ZAP_TBL_NAME||' t WHERE s.zap_id = t.zap_id )';
      EXECUTE IMMEDIATE dml_str USING err, err_type, err_prim;
      COMMIT;
  end;
  end;
-----------------------------------------------------------------------------------------

end PKG_XML_H_PAC_CONTROL;
/
