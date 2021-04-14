create or replace trigger medpom_client_roles_AUTO_ID
  before insert on medpom_client_roles
  for each row
begin
  if :NEW.id is null then
    select SEQ_MEDPOM_CLIENT_ROLES_ID.nextval into :NEW.id from dual;
  end if;
  end;
/
