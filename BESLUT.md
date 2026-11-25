# Beslut – grupp dromtrion

Här står allt vi har kommit överens om, så att vi inte krockar eller bygger olika saker.

**Så används filen**
- Nya beslut läggs till här och pushas. Skriv i Discord att filen är ändrad.
- Håller du inte med om något: säg till i Discord inom ett dygn. Säger ingen något gäller beslutet.
- Ändrar du en fil som någon annan äger: säg till i Discord först.

**När du jobbar med AI:** be den läsa repot direkt i stället för att klistra in kod, till exempel:
*"Läs https://raw.githubusercontent.com/Oliver-agg/grupp-dromtrion-clo26-inlamning-2-start/refs/heads/main/BESLUT.md och filerna under min uppgift. Fungerar inte detta klistra in all raw länkar som kan agera likadant som ovanstående."*

---

## Arbetssätt

1. **`git pull` innan du börjar jobba.**
2. **`dotnet build Projekt.slnx` innan du pushar.** Vi pushar aldrig kod som inte bygger.
3. **Vi jobbar direkt på `main`**, med små commits och ofta push. *Alternativ: en branch per uppgift. Valdes bort för att vi är nybörjare på git och vill undvika krångliga merger.*
4. **En uppgift per person i taget i kanban.** Den som tar ett kort sätter sig som assignee och flyttar det till In Progress.
5. **AI får användas.** Alla ska förstå och kunna förklara koden de pushar. AI-användning skrivs in i AI-deklarationen i README.
6. **Vi delar databasen i Atlas.** Radera aldrig databasen eller collections utan att säga till i Discord först.

---

## Vem äger vilka filer

| `Models/Recipe.cs`, `Chef.cs`, `Ingredient.cs`, `PlannedMeal.cs` | Filippa |
| `Models/Category.cs` | Daniella |
| `Models/CategoryWarning.cs`, `ShoppingListItem.cs` | Oliver |
| `Data/MongoContext.cs`, `MongoRecipeRepository.cs`, `MongoMealPlanRepository.cs` | Oliver |
| `Data/MongoCategoryRepository.cs`, `DatabaseSeeder.cs`, `ConnectionStringProvider.cs` | Daniella |
| `Services/IRecipeRepository.cs`, `IMealPlanRepository.cs`, `InMemoryRecipeRepository.cs` | Oliver |
| `Services/ICategoryRepository.cs`, `CategoryService.cs`, `RepositoryFactory.cs` | Daniella |
| `Services/RecipeCalculator.cs`, `RecipeBuilder.cs` (ny) | Filippa |
| `Services/MealPlanService.cs` | Delas – se beslut 9 |
| `UI/RecipeMenu.cs` | Filippa |
| `UI/CategoryMenu.cs` | Daniella |
| `UI/MealPlanMenu.cs`, `Menu.cs`, `ConsoleInput.cs` | Färdiga från läraren, rör bara om det behövs |
| `Program.cs` | Daniella (den som ändrar säger till först) |

---

## Beslut

### 1. Databas: MongoDB Atlas
Ett gemensamt kluster, databasen heter `recept`. Varje person har en egen databasanvändare.
*Alternativ: SQLite (fildatabas) – inte tillåtet, alla körde fildatabas i Hotel Grandioso. MongoDB lokalt med Docker – valdes bort för att slippa installera Docker.*

### 2. Id är heltal
Nästa nummer räknas fram i collection `counters` med `MongoContext.GetNextId`. Alla repositories använder samma metod.
*Alternativ: MongoDB:s egna långa id (ObjectId). Valdes bort för att menyerna redan frågar efter "nummer", och interfacen använder `int`.*

### 3. En gemensam databasanslutning
`MongoContext` ansluter, kontrollerar att databasen svarar och delar ut collections och nya id. `RepositoryFactory` skapar den en gång, och alla repositories delar på den.
*Alternativ: varje repository har en egen anslutning, som innan. Valdes bort: samma kod på flera ställen (DRY), och olika sätt att räkna fram id.*

### 4. Recipe
`Id`, `Name`, `Servings`, `Ingredients`, `CategoryId` (krävs), `ChefId`, `Note` (valfri, tom text = ingen anteckning).
**Förslag:** `ChefId` blir obligatoriskt och väljs i menyn när ett recept läggs till, eftersom beställningen säger att kocken är den som skapat receptet. (Filippa)
Ingredienser sparas inuti receptet och behöver inget eget Id.

### 5. Chef
**Förslag:** bara `Id` och `FirstName`. Beställningen säger att förnamn räcker. Övriga fält är personuppgifter som programmet inte använder (dataminimering, GDPR), och `Religion` är en känslig personuppgift.
Daniella motiverar fälten i "Fält och data". Filippa tar bort fälten i **ett** commit: `Chef.cs`, `RecipeMenu` (lägg till och lista kockar) och exempeldatan i `InMemoryRecipeRepository`, annars bygger inte projektet.

### 6. Category
`Id`, `Name`, `MaxPerWeek` (tom = ingen gräns). Standardkategorierna läggs in automatiskt när databasen är tom. "Övrigt" kan inte tas bort, och namn får inte vara tomma eller finnas två gånger (finns i `CategoryService`).

### 7. Ta bort en kategori
Recepten i kategorin flyttas till "Övrigt" med `IRecipeRepository.MoveRecipesToCategory` (finns nu). Daniella kopplar in den i `CategoryService.Delete`.

### 8. Menyerna pratar bara med interface
Inga databasklasser i `UI/`. `RecipeMenu` får kategorierna via konstruktorn (`ICategoryService`), precis som repositoryt.

### 9. Veckoplan, varningar och inköpslista
- **Lagring:** `IMealPlanRepository` och `MongoMealPlanRepository` (finns nu). Där sparas planerade måltider och varningar som redan visats.
- **Veckan** börjar på måndag och slutar på söndag. Vi använder `ISOWeek`, som redan finns i `MealPlanService`.
- **Gränsen** gäller alla veckor och kontrolleras per vecka. Varning vid 80 % och ett annat meddelande vid 100 %.
- **Visade varningar sparas i databasen** (`CategoryWarning`), så att de inte visas igen samma vecka, inte ens efter omstart (önskemål 4).
- **Inköpslistan** räknas fram från veckoplanen varje gång och sparas inte. Samma ingrediensnamn (stora och små bokstäver räknas som samma) med samma enhet summeras.
- **Ordning i `MealPlanService.cs`**, eftersom tre personer arbetar där:
  1. **Filippa:** konstruktorn som tar emot repositories, raden i `Program.cs`, `GetWeekPlan`, och den del av `PlanMeal` som sparar måltiden och kontrollerar att receptet finns. Pushar.
  2. **Daniella:** varningarna, i en egen klass (till exempel `CategoryLimitChecker`) som `PlanMeal` anropar. Observer är ett valfritt mönster här. Pushar.
  3. **Oliver:** `GetShoppingList`. Pushar.

### 10. Mängder och enheter
**Förslag:** mängden måste vara större än 0. Enheten väljs från en fast lista: `dl, l, msk, tsk, krm, g, kg, st`, så att inköpslistan kan summera rätt. Kontrollen görs på ett ställe, i `RecipeBuilder`. (Filippa)

### 11. Builder
`RecipeBuilder` används både för exempelrecepten och när användaren lägger till ett recept. Exempelrecepten får då en riktig kategori och kock (i dag hamnar de på kategori 0, som inte finns). När det fungerar tas `InMemoryRecipeRepository` bort. (Filippa bygger Builder, Daniella byter seed-datan)

### 12. Designmönster
Factory (`RepositoryFactory`), Builder (`RecipeBuilder`) och Repository. Observer är valfritt för varningarna.

### 13. Datum
`PlannedMeal.Date` är `DateOnly`. Den som först sparar en måltid testar att datumet sparas och läses rätt i MongoDB, och säger till om det inte fungerar.

---

## Klart

- [x] Atlas-kluster och user-secrets (`DB_CONNECTION`)
- [x] `MongoRecipeRepository`: recept, kockar, sök, max 5 kockar
- [x] `MongoCategoryRepository` och `CategoryService` med regler
- [x] Standardkategorier och exempelrecept vid första start
- [x] `RepositoryFactory`
- [x] Kategori och anteckning när ett recept läggs till
- [x] Gemensam anslutning (`MongoContext`), `IMealPlanRepository` och `MongoMealPlanRepository`
- [x] `MoveRecipesToCategory`
- [x] `RecipeMenu` får kategorier via konstruktorn och visar kategorins namn
- [x] Städat: gamla rader i `Program.cs` och en felaktig `using` i `CategoryMenu`

---

## Läs-länkar per person


**Alla**
- `BESLUT.md`
- `Recept-Konsol/README.md`
- Beställningen: https://raw.githubusercontent.com/marcusjobb/YRGO-CLO26-HT26Kursmaterial/refs/heads/main/kurs-03-fordjupad-oop/04_csharp_hidden_gems/assignment/bestallning_recept.md

**Filippa**
- `Recept-Konsol/Models/Recipe.cs`, `Models/Chef.cs`, `Models/PlannedMeal.cs`
- `Recept-Konsol/UI/RecipeMenu.cs`
- `Recept-Konsol/Services/RecipeCalculator.cs`, `Services/MealPlanService.cs`, `Services/IMealPlanRepository.cs`

**Daniella**
- `Recept-Konsol/Services/CategoryService.cs`, `Services/RepositoryFactory.cs`, `Services/IMealPlanRepository.cs`
- `Recept-Konsol/Data/DatabaseSeeder.cs`
- `Recept-Konsol/Models/Category.cs`, `Models/CategoryWarning.cs`

**Oliver**
- `Recept-Konsol/Services/MealPlanService.cs`, `Services/IMealPlanRepository.cs`
- `Recept-Konsol/Models/ShoppingListItem.cs`
