using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BkTravelApp.Data
{
    public class BktravelDB
    {
        SQLiteAsyncConnection _database;

        async Task Init()
        {
            if (_database is not null)
            {
                return;
            }
            _database = new SQLiteAsyncConnection(
                Constant.DatabasePath,
                Constant.flags);
            await _database.CreateTableAsync<Models.Travel>();
        }
        public async Task<List<Models.Travel>> GetTravelsAsync()
        {
            await Init();
            var traveldata = await _database.Table<Models.Travel>()
                .ToListAsync();
            return traveldata;
        }
        public async Task<Models.Travel> GetTravelAsync(int tId)
        {
            await Init();
            return await _database
                .Table<Models.Travel>()
                .Where(x => x.ID == tId)
                .FirstOrDefaultAsync();
        }
        public async Task<int> SaveTravelAsync(Models.Travel sTravel)
        {
            await Init();
            if (sTravel.ID != 0)
            {
                return await _database.UpdateAsync(sTravel);
            }
            else
            {
                return await _database.InsertAsync(sTravel);
            }
        }
        public async Task<int> DeleteTravelAsync(Models.Travel dTravel)
        {
            await Init();
            return await _database.DeleteAsync(dTravel);
        }
    }
}
