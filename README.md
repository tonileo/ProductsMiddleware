# ProductsMiddleware upute

## Podizanje rješenja lokalno
1.	Potrebno klonirati repozitorij
2.	Moguće da će biti ponuđeno vjerovati licencama pri pokretanju, što je potrebno odobrit.
3.	Promijeniti konfiguracijske podatke baze podataka na vaše. Unutar appsettings.json postaviti ime vaše lokalne sql baze podataka unutar ConnectionString-a. Naziv baze koju dobijete pri instalaciji sql servera je naziv koji treba biti u ConnectionStringu, kod mene je (default) ime sql servera „localhost\\MSSQLSERVER01“, ostalo sve može ostati isto
4.	Izrađene su migracije pa se baza jednostavno postavlja tako da se unutar Package Manager Console (Tools -> NuGet Package Manager -> Package Manager Console) upise „Update-Database“
5.	Build-ajte aplikaciju

## Dostupni endpoint-ovi
Svaki endpoint počinje s https://localhost:7177/api/. Detaljnije o svakom u sljedećem poglavlju.

AuthApi
* AuthApi/users
* AuthApi/login
* AuthApi/loginDifferentWay

AuthDatabase
* AuthDatabase/register
* AuthDatabase/login

ProductsApi
* ProductsApi
* ProductsApi/{id}
* ProductsApi/filter
* ProductsApi/search

## Autorizacija i autentifikacija

Do kada ne dođete do ProductsApi-a, možete preskočiti ovo poglavlje. Na ProductsApi/ i na ProductsApi/{id} je potrebna prethodna autentikacija kako bi ih mogli testirati.

Potrebno je napraviti registraciju unutar AuthDatabase/register, te zatim prijavu unutar AuthDatabase/login. Potrebno je registrirati jednog „user“ i jednog „admin“ korisnika. Detaljnije u AuthDatabase poglavlju. 
Nakon što se korisnik uspješno prijavi, u odgovoru dobije jwt token koji treba kopirati/spremiti. 

Na vrhu swagger stranice s desne strane nalazi se gumb „Authorize“. Klikom na gumb otvara se obrazac u koji prvo moramo upisat „Bearer [jwt]“. [jwt] je kopirani jwt koji smo dobili nakon prijave. Nakon toga možemo kliknuti close i testirati zaštićene endpoint-ove. Moguće i testirati Postman alatom gdje samo treba staviti kao header „Authorization“ i unutar vrijednosti staviti kao i prije „Bearer [jwt]“. 

Primjer:
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidGVzdDEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTcxOTE2NTAzNiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE3Ny8iLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MTc3LyJ9.ikbZ2RT_axJX3JBNUXzu8_ffCoHSwgq_VLvr0UPRfks

## Kako testirati aplikaciju

### AuthApi:
* Users (GET)– Dohvaća sve korisnike s https://dummyjson.com/users/, sluzi samo za testiranje

* Login (POST) – U tijelu se šalje username, password i expiresInMins (trajanje jwt tokena). Radi provjeru na https://dummyjson.com/auth/login, te vraća status 200 (ok) ako se username i password podudaraju. U suprotnom vraća jednu od  mogućih grešaka i njihov status. Username i password možete pronaći na  https://dummyjson.com/users/ ili koristeći Users endpoint gdje brže možemo  izabrati korisnika i lozinku. Nije implementirano, da se nakon prijave može pristupiti zaštićenim endpoint-ovima

Primjer: {
  "username": "oliviaw",
  "password": "oliviawpass",
  "expiresInMins": 30
}

* loginDifferentWay (POST) –  U tijelu se šalje username i password. Za razliku od prijašnjeg endpointa, radi direktnu provjeru na https://dummyjson.com/users/. Vraća status 200 (ok) ako se podudaraju username i password koji smo unijeli s podacima korisnika na https://dummyjson.com/users/.  U suprotnom vraća jednu od  mogućih grešaka i njihov status. Nije implementirano, da se nakon prijave može pristupiti zaštićenim endpoint-ovima

Primjer: {
  "username": "oliviaw",
  "password": "oliviawpass"
}


### AuthDatabase:
* Register (POST)– Implementirana metoda stvarne registracije kroz bazu podataka. Potrebno unijeti željeni username, password (mora biti barem 3 znaka) i uloga (admin ili user). Vraća status 200 (ok) s porukom „User successfully registered, now you can login!“. U suprotnom vraća jednu od  mogućih grešaka i njihov status.
	Primjer:
{
  "username": "test",
  "password": "123",
  "roles": [
    "admin"
  ]
}

* Login (POST) – Implementirana post metoda stvarne prijave kroz bazu podataka. Potrebno unijeti username i password koji smo odabrali u procesu registracije. Vraća status 200 (ok) s JWT tokenom. JWT token traje 30 min. Vrlo važno kopirati/spremiti taj token kako bih kasnije mogli testirati Autentifikaciju i Autorizaciju na sljedećim endpoint-ovima.

Primjer:
{
  "username": "test",
  "password": "123"
}

### ProductsApi:
* (Get) – POTREBNA AUTENTIKACIJA KORISNIKA S USER ULOGOM. Dohvaća listu proizvoda s slikom, nazivom, skraćenim opisom do 100 znakova (ako proizvod ima više od 100 znakova, znakovi nakon 100-og znaka se neće prikazati)
  
* {id} (GET) – POTREBNA AUTENTIKACIJA KORISNIKA S ADMIN ULOGOM. Pri unosu id-a nekog proizvoda, vraća detalje tog proizvoda. Mora se unijet id inače se metoda neće izvršit. 
 
* Filter (GET) – Omogućuje filtriranje proizvoda po kategoriji, najnižoj cijeni, najvišoj cijeni. Moguće filtrirat po samo po jednom svojstvu, a moguća i kombinacija svih. Filtriranje po kategoriji će vratiti sve proizvode s tom kategorijom. Filtriranje po najnižoj cijeni će prikazati sve proizvode čija cijena je veća od najniže cijene. Filtriranje po najvišoj cijeni će prikazati sve proizvode čija cijena je niža od najviše cijene. Implementiran cache koji će pri svakom ponovnom pozivu endpoint-a s istim parametrima, vratiti proizvode iz cache memorije umjesto dohvaćanja s servisa svaki puta.
 
* Search (GET) – Omogućuje pretragu proizvoda po imenu (Title) za uneseni tekst. Vraća listu proizvoda čije ime se podudara s pretragom. I na ovom endpoint-u implementiran cache.

Na svakom endpoint-u implementirano loggiranje
