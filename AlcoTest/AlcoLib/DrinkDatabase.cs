using SQLite;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AlcoLib
{
    public class DrinkDatabase
    {
        readonly SQLiteAsyncConnection database;

        public DrinkDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<ShotDrink>().Wait();
        }

        public Task<List<ShotDrink>> GetShotDrinksAsync()
        {
            return database.Table<ShotDrink>().ToListAsync();
        }

        public Task<ShotDrink> GetShotDrinkAsync(int id)
        {
            return database.Table<ShotDrink>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveShotDrinkAsync(ShotDrink drink)
        {
            if (drink.ID != 0)
            {
                return database.UpdateAsync(drink);
            }
            else
            {
                return database.InsertAsync(drink);
            }
        }

        public Task<int> DeleteShotDrinkAsync(ShotDrink drink)
        {
            return database.DeleteAsync(drink);
        }
    }
}
