using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace AlcoLib
{
    public class ProfileDatabase
    {


            readonly SQLiteAsyncConnection database;

            public ProfileDatabase(string dbPath)
            {
                database = new SQLiteAsyncConnection(dbPath);
                database.CreateTableAsync<Profile>().Wait();
            }

            public Task<List<Profile>> GetProfilesAsync()
            {
                return database.Table<Profile>().ToListAsync();
            }

            public Task<Profile> GetProfileAsync(int id)
            {
                return database.Table<Profile>()
                                .Where(i => i.ID == id)
                                .FirstOrDefaultAsync();
            }

            public Task<int> SaveProfileAsync(Profile profile)
            {
                if (profile.ID != 0)
                {
                    return database.UpdateAsync(profile);
                }
                else
                {
                    return database.InsertAsync(profile);
                }
            }

            public Task<int> DeleteProfileAsync(Profile profile)
            {
                return database.DeleteAsync(profile);
            }

        public void DeleteProfilesAsync()
        {
            foreach (var item in GetProfilesAsync().Result)
            {
                DeleteProfileAsync(item);
            }
        }

        
    }
}
