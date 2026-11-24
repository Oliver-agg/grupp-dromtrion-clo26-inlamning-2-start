# Recept, konsol

Konsolprogram. **Gränssnittet är färdigt.** Ni bygger det som ligger bakom: databas, repository och logik. Se avsnittet [Så är koden uppbyggd](#så-är-koden-uppbyggd).

## Så kör du

1. Gör en `dotnet user-secrets init` och sätt `DB_CONNECTION` (se nedan).
2. Kör: `dotnet run --project Recept-Konsol`

Vid första start skapar programmet databasen och seedar den, om den är tom.

## Connectionstring (user-secrets)

Connectionstringen sparas i en secrets-fil på er dator. Den ligger aldrig i koden eller i repot.

```bash
dotnet user-secrets init --project Recept-Konsol
dotnet user-secrets set "DB_CONNECTION" "<er connectionstring>" --project Recept-Konsol
```

Kontrollera med `dotnet user-secrets list --project Recept-Konsol`. Kommandot visar värdet, så dela inte skärmbilden.
Läraren kör er kod mot sin egen Docker-server och sätter connectionstringen själv. Er kod måste därför bygga och seeda hela databasen vid första start.

---


Ett hushåll sparar recept med ingredienser, planerar veckans måltider, får en inköpslista och får varning när en kategori används för ofta. Beställningen finns i uppgiftsdokumentet `bestallning_recept.md`.

## Minsta menyval (måste finnas)

```
1. Lista recept
2. Lägg till recept (namn, portioner, ingredienser)
3. Visa ett recept
4. Ta bort recept
5. Sök recept på ingrediens
6. Lägg till kock
0. Avsluta
```

## Minimal modell (måste byggas ut)

- `Recept`: `Id`, `Namn`, `Portioner`
- `Ingrediens`: `Namn`, `Mangd`, `Enhet`
- `Kock`: `Id`, `Fornamn`, `Efternamn`, `Fodelsedatum`, `HairColor`, `HeightCm`, `Religion` (den som skapat receptet)

Lägg till: kategorier, veckoplan, inköpslista, gräns per kategori, och det som beställaren vill ha.

---

## Så är koden uppbyggd

Programmet går att köra direkt, utan databas. Menyerna i `UI/` är färdiga och ska inte behöva ändras, förutom där en kommentar säger det.

| Mapp | Innehåll | Vad ni gör |
|------|----------|------------|
| `UI/` | Menyer och inmatning. Pratar bara med interface i `Services/`. | Lägger till fält i formulären när ni bygger ut modellerna. |
| `Models/` | Minimala modeller, bara det som README:n kräver. | Bygger ut med fälten beställningen kräver, och tar bort de som inte behövs. |
| `Services/` | Interface och klasser som menyn anropar. | Bygger allt som kastar `NotImplementedException`. |
| `Data/` | Tom. | Er databaskoppling. |

### Vad fungerar redan?

- **Menyval 1–7** (recept och kockar) fungerar mot `InMemoryRecipeRepository`. Den är en låtsasdatabas: data ligger i listor och försvinner när programmet stängs.
- **Kategorier, veckoplan och inköpslista** visar demodata från `CategoryService` och `MealPlanService`: hårdkodade listor som inte sparas. Allt som ändrar något (lägga till eller ta bort kategori, sätta gräns, planera måltid) kastar `NotImplementedException`, och då visas *"Inte implementerat ännu"*.
- **Demodatan visar hur sidorna ska se ut** när ni är klara, men den är hårdkodad. Ersätt allt som är märkt `DEMO` med riktig logik.
- **Två saker är fel med flit:** omräkning av portioner returnerar originalmängderna, och mängder visas som `2,50 dl` i stället för `2,5 dl` (önskemål 1). Båda rättas i `RecipeCalculator`.

Sök efter `Du behöver bygga ut detta mer` i koden för att hitta alla ställen ni ska bygga vidare på.

### Byt låtsasdatabasen mot en riktig

1. Skriv en klass, till exempel `MongoRecipeRepository` eller `EfRecipeRepository`, som implementerar `IRecipeRepository`.
2. Låt den skapa databasen och seeda exempeldata vid första start, om den är tom.
3. Byt raden `new InMemoryRecipeRepository()` i `Program.cs` mot er klass.
4. Ta bort `InMemoryRecipeRepository.cs` när ni inte behöver den längre.

Felaktig inmatning som upptäcks i era services: kasta `ArgumentException` med ett meddelande på svenska. Menyn visar meddelandet i stället för att krascha.

### Svenska namn i README, engelska i koden

| README | Koden |
|--------|-------|
| `Recept` (`Namn`, `Portioner`) | `Recipe` (`Name`, `Servings`) |
| `Ingrediens` (`Namn`, `Mangd`, `Enhet`) | `Ingredient` (`Name`, `Amount`, `Unit`) |
| `Kock` (`Fornamn`, `Efternamn`, `Fodelsedatum`, `HairColor`, `HeightCm`, `Religion`) | `Chef` (`FirstName`, `LastName`, `BirthDate`, `HairColor`, `HeightCm`, `Religion`) |

