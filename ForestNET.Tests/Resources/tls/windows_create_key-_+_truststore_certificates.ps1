# manually add pfx file to user certificates -> trusted root certification authorities

if (Test-Path server -PathType Container) {
    rm server -r -fo
}

if (Test-Path client -PathType Container) {
    rm client -r -fo
}

mkdir server
cd server

#Server
Write-Host "SERVER PFX WITH PW"
openssl req -new -x509 -keyout ca-key-server-with-pw.pem -out ca-cert-server-with-pw.pem -subj "/CN=forestNET Server PW/OU=server_pw.forestany.de/O=forestany/L=Bochum/ST=NRW/C=DE" -passout pass:12345678
openssl rsa -in ca-key-server-with-pw.pem -out ca-key-server-with-pw.pem -passin pass:12345678
openssl req -new -key ca-key-server-with-pw.pem -out ca-sign-request-server-with-pw.csr -subj "/CN=forestNET Server PW/OU=server_pw.forestany.de/O=forestany/L=Hagen/ST=NRW/C=DE"
openssl x509 -req -days 365 -in ca-sign-request-server-with-pw.csr -signkey ca-key-server-with-pw.pem -out ca-cert-server-with-pw.crt
openssl pkcs12 -inkey ca-key-server-with-pw.pem -in ca-cert-server-with-pw.pem -export -out cert-server-with-pw.pfx -passin pass:12345678 -passout pass:123456

Write-Host "SERVER PFX WITH PW"
openssl req -new -x509 -keyout ca-key-server-no-pw.pem -out ca-cert-server-no-pw.pem -subj "/CN=forestNET Server NOPW/OU=server_no_pw.forestany.de/O=forestany/L=Bochum/ST=NRW/C=DE" -passout pass:12345678
openssl rsa -in ca-key-server-no-pw.pem -out ca-key-server-no-pw.pem -passin pass:12345678
openssl req -new -key ca-key-server-no-pw.pem -out ca-sign-request-server-no-pw.csr -subj "/CN=forestNET Server NOPW/OU=server_no_pw.forestany.de/O=forestany/L=Hagen/ST=NRW/C=DE"
openssl x509 -req -days 365 -in ca-sign-request-server-no-pw.csr -signkey ca-key-server-no-pw.pem -out ca-cert-server-no-pw.crt
openssl pkcs12 -inkey ca-key-server-no-pw.pem -in ca-cert-server-no-pw.pem -export -out cert-server-no-pw.pfx -passin pass:12345678 -passout pass:

cd ..
mkdir client
cd client

#Client
Write-Host "CLIENT PFX WITH PW"
openssl req -new -x509 -keyout ca-key-client-with-pw.pem -out ca-cert-client-with-pw.pem -subj "/CN=forestNET Client PW/OU=client_pw.forestany.de/O=forestany/L=Bochum/ST=NRW/C=DE" -passout pass:12345678
openssl rsa -in ca-key-client-with-pw.pem -out ca-key-client-with-pw.pem -passin pass:12345678
openssl req -new -key ca-key-client-with-pw.pem -out ca-sign-request-client-with-pw.csr -subj "/CN=forestNET Client PW/OU=client_pw.forestany.de/O=forestany/L=Hagen/ST=NRW/C=DE"
openssl x509 -req -days 365 -in ca-sign-request-client-with-pw.csr -signkey ca-key-client-with-pw.pem -out ca-cert-client-with-pw.crt
openssl pkcs12 -inkey ca-key-client-with-pw.pem -in ca-cert-client-with-pw.pem -export -out cert-client-with-pw.pfx -passin pass:12345678 -passout pass:123456

Write-Host "CLIENT PFX WITH PW"
openssl req -new -x509 -keyout ca-key-client-no-pw.pem -out ca-cert-client-no-pw.pem -subj "/CN=forestNET Client NOPW/OU=client_no_pw.forestany.de/O=forestany/L=Bochum/ST=NRW/C=DE" -passout pass:12345678
openssl rsa -in ca-key-client-no-pw.pem -out ca-key-client-no-pw.pem -passin pass:12345678
openssl req -new -key ca-key-client-no-pw.pem -out ca-sign-request-client-no-pw.csr -subj "/CN=forestNET Client NOPW/OU=client_no_pw.forestany.de/O=forestany/L=Hagen/ST=NRW/C=DE"
openssl x509 -req -days 365 -in ca-sign-request-client-no-pw.csr -signkey ca-key-client-no-pw.pem -out ca-cert-client-no-pw.crt
openssl pkcs12 -inkey ca-key-client-no-pw.pem -in ca-cert-client-no-pw.pem -export -out cert-client-no-pw.pfx -passin pass:12345678 -passout pass:

cd ..