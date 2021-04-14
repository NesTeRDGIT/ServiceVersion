openssl req -new -sha256 -nodes -out site.root.ru.csr -newkey rsa:2048 -keyout site.root.ru.key -config server.csr.cnf

openssl x509 -req -in site.root.ru.csr -CA ssl\root.ru.pem -CAkey ssl\root.ru.key -CAcreateserial -out site.root.ru.crt -days 3650 -sha256 -extfile v3.ext