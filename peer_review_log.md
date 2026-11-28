# Peer review-logg

> Den här filen ligger i **gruppens** repo och uppdateras under hela testningen och granskningen.
> Allt som krånglar ska in här, även det som verkar litet: glitchar, quirks, buggar, verktygsproblem och allt som tog tid att lösa.
> Den som hittar något skriver in det. Den som fixar det skriver in hur det lösts. Ingen fix är klar förrän den står här.

---

## Granskning

| Fält | Värde |
|------|-------|
| Granskad grupp (ni) | |
| Granskare (grupp, personer) | |
| Datum för granskningen | |
| Granskaren har bjudits in som medarbetare (ja/nej) | |
| Granskningen klar och godkänd av den granskade gruppen (ja/nej, datum) | |

**Regel:** en annan grupp måste granska det slutliga resultatet. Två grupper får inte granska varandra.

---

## Ärenden

Ett ärende per problem. Ett nytt ärende för varje nytt problem. Numrera i ordning: `P1`, `P2`, `P3` och så vidare.

| ID | Datum | Typ | Vem hittade | Vad hände (steg för steg) | Förväntat | Status | Hur löstes det |
|----|-------|-----|-------------|---------------------------|-----------|--------|----------------|
| P1 | 2026-11-27 | bugg | Filippa | `RecipeBuilder` byggde inte: `private interface _categoryId` i stället för `private int`, och två rader i `ByChef` låg i fel block ("unreachable code"). | Bygget går igenom utan fel | löst | Rättade typen till `int` och flyttade klammern i `ByChef`. Hände före commit. |
| P2 | 2026-11-27 | quirk | Filippa | Kockarna i Atlas hade fält som inte längre finns i `Chef` (efternamn, religion med mera). De kunde ge fel vid läsning, och personuppgifter låg kvar i databasen. | Collectionen `chefs` stämmer med nya `Chef` (bara Id och FirstName) | löst | Tömde collectionen `chefs` i Atlas. Kocken Kim lades in på nytt vid start, bara med förnamn. |
| P3 | 2026-11-27 | krångel | Filippa | Med giltig sträng gav inloggningen "Unable to authenticate using sasl protocol mechanism SCRAM-SHA-1". Atlas godkände inte inloggningen. Orsaken är inte fastställd. | Inloggningen fungerar | löst | Skapade en ny databasanvändare i Atlas med ett enkelt lösenord (bara bokstäver och siffror) och satte om `DB_CONNECTION`. |
| P4 | 2026-11-27 | krångel | Filippa | Programmet startade inte: "The connection string is not valid". Lösenordet saknades i strängen i user-secrets. | Programmet ansluter till databasen | löst | Satte om `DB_CONNECTION` med en komplett sträng (enkla citattecken i PowerShell). |

**Typer:**
- **Glitch:** något som bara händer ibland.
- **Quirk:** något som fungerar men oväntat, eller som inte borde vara så.
- **Bugg:** något som är fel enligt kraven.
- **Krångel:** verktyg, installation, inloggning, Git, databas, miljö. Även sådant som inte är kodens fel men tog tid.

---

## Detaljer per ärende

Kopiera blocket nedan för varje ärende. Fyll i så mycket som ni kan. Ett kort svar är bättre än ingen logg.

### P1: RecipeBuilder byggde inte

- **Typ:** bugg
- **Hittad av:** Filippa
- **Datum:** 2026-11-27
- **Steg för att återskapa:**  Kör `dotnet build Projekt.slnx` med `private interface _categoryId;` i `RecipeBuilder`.
- **Förväntat:** Bygget går igenom.
- **Faktiskt:** Fyra felmeddelanden (CS0118, CS0119, CS0161 och en varning, CS0162).
- **Fel-meddelande (om något):** `'RecipeBuilder._categoryId' is a type but is used like a variable`, och `not all code paths return a value`.
- **Orsak (om ni vet den):** Två skrivfel när koden skrevs in: `interface` i stället för `int`, och en slutklammer på fel ställe i `ByChef`.
- **Åtgärd:** Rättade typen och flyttade klammern. Fixat före commit, så det finns ingen commit-hash.
- **Verifierad av:** vem som kontrollerade att det fungerar, och hur. Filippa, `dotnet build` utan fel.
- **Status:** öppen / löst / accepterat (och varför, om accepterat). löst

### P2: Gamla kockar i Atlas med fält som inte längre finns

- **Typ:** quirk
- **Hittad av:** Filippa
- **Datum:** 2026-11-27
- **Steg för att återskapa:**  Förenkla `Chef` till Id och FirstName och kör mot en databas som redan har kockar med de gamla fälten.
- **Förväntat:** `chefs` innehåller bara Id och FirstName.
- **Faktiskt:** De gamla dokumenten hade kvar efternamn, religion med mera.
- **Fel-meddelande (om något):** Inget. Åtgärden gjordes innan något fel hann visas.
- **Orsak (om ni vet den):** Databasen seedades med den gamla `Chef`-klassen, och en ändrad klass ändrar inte data som redan ligger i Atlas.
- **Åtgärd:** Tömde collectionen `chefs` i Atlas. Vid nästa start seedades kocken Kim om, bara med förnamn.
- **Verifierad av:** vem som kontrollerade att det fungerar, och hur. Filippa, genom att lista kockar (val 7) och titta på dokumenten i Atlas.
- **Status:** öppen / löst / accepterat (och varför, om accepterat). löst

### P3: Autentitisering mot Atlas misslyckades

- **Typ:** krångel
- **Hittad av:** Filippa
- **Datum:** 2026-11-27
- **Steg för att återskapa:**  Kör programmet med en giltig sträng, med min IP-adress godkänd under Network Access i Atlas.
- **Förväntat:** Inloggning och anslutning.
- **Faktiskt:** Programmet visade "Kunde inte ansluta till databasen".
- **Fel-meddelande (om något):** `Unable to authenticate using sasl protocol mechanism SCRAM-SHA-1.`
- **Orsak (om ni vet den):** Okänd. Användarnamn och lösenord godkändes inte av Atlas.
- **Åtgärd:** Skapade en ny databasanvändare med läs- och skrivrättigheter och ett lösenord med bara bokstäver och siffror, och satte om `DB_CONNECTION`.
- **Verifierad av:** vem som kontrollerade att det fungerar, och hur. Filippa, programmet startade och kockarna kunde listas.
- **Status:** öppen / löst / accepterat (och varför, om accepterat). löst

### P4: Connectionstringen är ogiltig

- **Typ:** krångel
- **Hittad av:** Filippa
- **Datum:** 2026-11-27
- **Steg för att återskapa:** Sätt `DB_CONNECTION` i user-secrets med ett tomt lösenord (`användare:@kluster`) och kör `dotnet run --project Recept-Konsol`.
- **Förväntat:** Programmet ansluter till Atlas.
- **Faktiskt:** Programmet kraschade i `MongoContext`.
- **Fel-meddelande (om något):** `MongoConfigurationException: The connection string is not valid.`
- **Orsak (om ni vet den):** ** Lösenordet saknades i strängen, så MongoDB-drivrutinen kunde inte tolka den.
- **Åtgärd:** Satte om `DB_CONNECTION` med en komplett sträng. Ingen kodändring.
- **Verifierad av:** vem som kontrollerade att det fungerar, och hur. Filippa, genom att starta programmet och se menyn.
- **Status:** öppen / löst / accepterat (och varför, om accepterat). löst

---

## Sammanfattning

- Antal ärenden totalt: 4
- Antal lösta: 4
- Antal accepterade (kvar, med motivering): 0
- Vad tog längst tid, och vad skulle ni göra annorlunda nästa gång:
