using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BkTravelApp
{
    public class Constant
    {
        public const string DataFilename = "BKTravelApp.db3";

        public const SQLite.SQLiteOpenFlags flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DataFilename);

    }
}
