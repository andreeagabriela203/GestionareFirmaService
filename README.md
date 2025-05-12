# Sistem de gestionare al unei Firme service de dispozitive electronice

## Descriere

Aceast proiect presupune realizarea unei baze de date pentru gestionarea unei firme de service de dispozitive electronice. Scopul este de a ține evidența angajaților, serviciilor oferite, dispozitivelor reparate și pieselor necesare pentru efectuarea reparațiilor.
Baza de date include tabele pentru gestionarea angajaților, clienților, dispozitivelor, pieselor necesare pentru reparații, servicii oferite și activitatea firmei.

## Functionalități
- Gestionare angajați/ clienți/ dispozitive/ piese de schimb/ servicii - adăugarea, editarea și eliminarea acestora;
- Generarea rapoartelor referitoare la activitățile firmei privind veniturile, numărul de reparații și costuri;
- Vizualizarea dispozitivelor reparate sau pe cele a căror reparație întârzie.


## Structura Proiectului

### Clase(Model) pentru: Angajați, Clienți, Dispozitive, Servicii și Piese
### Controllere specifice claselor enumerate care se ocupă de legătura dintre interfață și date.
### Interfețe(View)

## Structura bazei de date
Tabelele împreună cu atributele specifice și tipul lor de date.

### Angajați:
 -Nume	nvarchar(50)
 -Prenume	nvarchar(50)
 -Adresa	nvarchar(50)
 -DataNastere	datetime
 -DataAngajare	datetime
 -Salariu	int
 -CNP 	     int
### Clienți:
 -Nume	nvarchar(50)
 -Prenume	nvarchar(50)
 -AdresaEmail	nvarchar(50)
 -NrTelefon	int
### Dispozitive:
 -ClientID	int
 -TipDispozitiv	nvarchar(50)
 -Marca	nvarchar(50)
 -Model	nvarchar(50)
 -Problema	nvarchar(50)
 -Status	nchar(10)
### Servicii:
 -Denumire	nvarchar(50)
 -DurataEstimata	nvarchar(50)
 -CostServiciu	int
### Piese:
 -TipPiesa	nvarchar(50)
 -TipDispozitiv	nvarchar(50)
 -Model	nvarchar(50)
 -Marca	nvarchar(50)
 -Cost	int
 -Stoc	int
### Angajați-Servicii: tabel de legătură
 -AngajatID	int
 -DispozitivID	int
 -PiesaID	int
 -Cost	int
 -Data_Incepere	datetime
 -DataFinalizare	datetime


## Interfața 
- **Vizualizarea și gestionarea datelor pentru Angajați/ Dispozitive/ Clienți/ Piese/ Servicii**
- **Vizualizarea statusului dispozitivelor** 
- **Statistici**


## Tehnologii folosite
- **SQL Server**
- **C#**

