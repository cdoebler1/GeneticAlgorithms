using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace OilSelector
{
    public class OilDatabase
    {

        readonly SQLiteAsyncConnection database;

        public OilDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<ViscosityItem>().Wait();
        }

        public Task<List<ViscosityItem>> GetItemsAsync()
        {
            return database.Table<ViscosityItem>().ToListAsync();
        }

        public Task<List<ViscosityItem>> GetItemsInRangeAtTempAsync(double lowRange, double highRange, string temp)
        {
            return database.QueryAsync<ViscosityItem>("SELECT * FROM OilViscosity WHERE C = " + temp + " AND Viscosity > " + lowRange.ToString() + " AND Viscosity < " + highRange.ToString() + " ORDER BY Viscosity ASC");
        }

        public Task<ViscosityItem> GetItemAsync(int id)
        {
            return database.Table<ViscosityItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(ViscosityItem item)
        {
            if (item.ID != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(ViscosityItem item)
        {
            return database.DeleteAsync(item);
        }
    }
}
