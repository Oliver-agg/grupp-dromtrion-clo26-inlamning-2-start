using Recept.Data;

namespace Recept.Services;

// Factory skapar rätt repository så att resten av programmet bara känner till interfacet. 
public static class RepositoryFactory 
{
    public static IRecipeRepository Create() =>
        new MongoRecipeRepository(ConnectionStringProvider.GetConnectionString());
public static ICategoryService CreateCategoryService()
    {
        var repository = new MongoCategoryRepository(ConnectionStringProvider.GetConnectionString());
        DatabaseSeeder.SeedCategoriesIfEmpty(repository);
        return new CategoryService(repository);
    }
}
