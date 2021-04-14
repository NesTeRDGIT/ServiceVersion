mkdir ssl
openssl genrsa -des3 -out ssl\root.ru.key 2048
openssl req -x509 -new -config set.txt -nodes -key ssl\root.ru.key -sha256 -days 3600 -out ssl\root.ru.pem