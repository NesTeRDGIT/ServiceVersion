create or replace view v_xml_errors_den as
select distinct sc.code_mo as "Код МО",
case when ((lp.fam is not null) and (lp.fam_p is not null)) then nvl(lp.fam, '')||'/'||nvl(lp.fam_p, '')
  else  nvl(lp.fam, '')||nvl(lp.fam_p, '') end as "Фамилия пациента/представителя",
case when ((lp.im is not null) and (lp.im_p is not null)) then nvl(lp.im, '')||'/'||nvl(lp.im_p, '')
  else  nvl(lp.im, '')||nvl(lp.im_p, '') end as "Имя пациента/представителя",
    case when ((lp.ot is not null) and (lp.ot_p is not null)) then nvl(lp.ot, '')||'/'||nvl(lp.ot_p, '')
  else  nvl(lp.ot, '')||nvl(lp.ot_p, '') end as Ot,
        case when ((lp.dr is not null) and (lp.dr_p is not null)) then nvl(lp.dr, '')||'/'||nvl(lp.dr_p, '')
  else  nvl(lp.dr, '')||nvl(lp.dr_p, '') end as "ДР пациента/представителя",

        p.id_pac as "Код пациента",
        p.vpolis as "Тип полиса",
        p.spolis as "Серия полиса",
        p.npolis as "Номер полиса",
        s.lpu_1 as "№ подразделения",
        s.nhistory ,
        s.usl_ok as "Условие оказания МП",
        s.iddokt as "Код врача, закрывшего случай",
        trim(hz.err_prim)||
        trim(sc.err_prim)||
        trim(p.err_prim)||
        trim(s.err_prim)||
        trim(u.err_prim)||
        trim(lp.err_prim)
         as "Ошибка"
  from xml_h_zglv_temp99 hz inner join xml_l_zglv_temp99 lz on hz.filename=lz.filename1
                     inner join xml_h_schet_temp99 sc on hz.zglv_id=sc.zglv_id
                     inner join xml_h_zap_temp99 z on sc.schet_id=z.schet_id
                     inner join xml_h_pacient_temp99 p on p.zap_id=z.zap_id
                     inner join xml_h_sluch_temp99 s on s.zap_id=z.zap_id
                     inner join xml_h_usl_temp99 u on u.sluch_id=s.sluch_id
                     inner join xml_l_pers_temp99 lp on p.id_pac=lp.id_pac and lp.zglv_id=lz.zglv_id
   where (hz.err is not null) or
        (sc.err is not null) or
        (p.err is not null) or
        (s.err is not null) or
        (u.err is not null) or
        (lz.err is not null) or
        (lp.err is not null)/* and
        hz.zglv_id>4421*/;
