create or replace trigger medpom_exist_method_AUTO_ID
  before insert on medpom_exist_method
  for each row
begin
  if :NEW.id is null then
    select SEQ_medpom_client_users_ID.nextval into :NEW.id from dual;
  end if;
  end;
/
