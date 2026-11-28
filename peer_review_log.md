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

### P1: rubrik

- **Typ:** 
- **Hittad av:** 
- **Datum:** 
- **Steg för att återskapa:** 
- **Förväntat:**
- **Faktiskt:**
- **Fel-meddelande (om något):** 
- **Åtgärd:**
- **Verifierad av:** vem som kontrollerade att det fungerar, och hur.
- **Status:** öppen / löst / accepterat (och varför, om accepterat). 

---

## Sammanfattning

- Antal ärenden totalt:
- Antal lösta:
- Antal accepterade (kvar, med motivering):
- Vad tog längst tid, och vad skulle ni göra annorlunda nästa gång:
