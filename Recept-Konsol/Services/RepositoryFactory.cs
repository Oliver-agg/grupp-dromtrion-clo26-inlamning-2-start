using Recept.Data;

namespace Recept.Services;

// Factory skapar rätt repository så att resten av programmet bara känner till interfacet.
// Alla repositories delar på samma anslutning (MongoContext), som skapas första gången den behövs.
public static class RepositoryFactory
{
    private static MongoContext? _context;

    private static MongoContext Context =>
        _context ??= new MongoContext(ConnectionStringProvider.GetConnectionString());

    public static IRecipeRepository Create() =>
        new MongoRecipeRepository(Context);

    public static ICategoryService CreateCategoryService()
    {
        var repository = new MongoCategoryRepository(Context);
        DatabaseSeeder.SeedCategoriesIfEmpty(repository);
        return new CategoryService(repository);
    }

    public static IMealPlanRepository CreateMealPlanRepository() =>
        new MongoMealPlanRepository(Context);
}
