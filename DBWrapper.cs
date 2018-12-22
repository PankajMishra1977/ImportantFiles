using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ItpMicroController.Entities;

namespace MyNameSpace
{
    public class DatabaseWrapper
    {
        //private static string ConnectionString = "Data Source=database.db;Version=3;New=True;Compress=True;";
        private static string _connectionString = string.Empty;

        public static string GetConnectionString()
        {
            return _connectionString;
        }

        public static void GenerateDatabase()
        {
            String appFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Sample");
            const string databaseFileName = "Sample.db";
            String completeDatabasePath = Path.Combine(appFolderPath, databaseFileName);

            _connectionString = string.Format("Data Source={0};Version=3;New=True;Compress=True;", completeDatabasePath);    

            // if directory is not there, create directory and file
            if (!Directory.Exists(appFolderPath))
            {
                Directory.CreateDirectory(appFolderPath);
                LogWriter.WriteToLog(string.Format("Creating Directory {0} for database ItpMicroControllerDb.db", appFolderPath));
            }

            if (!File.Exists(completeDatabasePath))
            {
                LogWriter.WriteToLog("Creating Tables for database");
                CreateTables();
            }
        }

       

        public static void ExecuteQuery(string queryString)
        {
            using (var sqliteConn = new SQLiteConnection(GetConnectionString()))
            {
                sqliteConn.Open();

                using (SQLiteCommand sqliteCmd = sqliteConn.CreateCommand())
                {
                    sqliteCmd.CommandText = queryString;
                    sqliteCmd.ExecuteNonQuery();
                }
            }
        }
       
        public static bool TableExists(SQLiteCommand command, string tableName)
        {
            command.CommandText = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}';", tableName);
            var rowCount = command.ExecuteNonQuery();
            return rowCount != -1;
        }

        public static void CreateTables()
        {
            using (var sqlite_conn = new SQLiteConnection(GetConnectionString()))
            {
                sqlite_conn.Open();
                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    if (!TableExists(sqlite_cmd, "Sample"))
                    {
                        // Vehicle Table
                        sqlite_cmd.CommandText = "CREATE TABLE SampleTbl (Id integer primary key, Name varchar(100));";
                        sqlite_cmd.ExecuteNonQuery();
                    }

                    
                    // Customer Table
                    if (!TableExists(sqlite_cmd, "Relational1"))
                    {
                        sqlite_cmd.CommandText = "CREATE TABLE Relational1(" +
                                                 "Id INTEGER PRIMARY KEY, " +
                                                 "Name  varchar(100);"
                        sqlite_cmd.ExecuteNonQuery();
                    }

                    if (!TableExists(sqlite_cmd, "Relational2"))
                    {
                        // MicroController Table
                        sqlite_cmd.CommandText = "CREATE TABLE Relational2(" +
                                                 "Id INTEGER PRIMARY KEY," +
                                                 "Name varchar(100), " +
                                                 "RelationalId INTEGER, " +
                                                 "FOREIGN KEY(RelationalId) REFERENCES Relational1(Id));";

                        sqlite_cmd.ExecuteNonQuery();
                    }
                    
                }
            }

            LogWriter.WriteToLog("Created Tables for database");
        }

        public static int SaveCustomer(Customer customer
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(GetConnectionString()))
            {
                sqlite_conn.Open();
               
                using (var sqlCommand = new SQLiteCommand(sqlite_conn))
                {
                    sqlCommand.CommandText = "INSERT INTO [Customer] (Name, CountryName, Address, PostCode, Phone, EmailAddress) " +
                                             "VALUES(@param1, @param2, @param3, @param4, @param5, @param6)";
                    sqlCommand.CommandType = CommandType.Text;

                    sqlCommand.Parameters.Add(new SQLiteParameter("@param1", customer.Name));
                    sqlCommand.Parameters.Add(new SQLiteParameter("@param2",  customer.CountryName));
                    sqlCommand.Parameters.Add(new SQLiteParameter("@param3",  customer.Address));
                    sqlCommand.Parameters.Add(new SQLiteParameter("@param4",  customer.PostCode));
                    sqlCommand.Parameters.Add(new SQLiteParameter("@param5",  customer.Phone));
                    sqlCommand.Parameters.Add(new SQLiteParameter("@param6", customer.EmailAddress));

                    sqlCommand.ExecuteNonQuery();

                    // Get the Last Row
                    sqlCommand.CommandText = "select last_insert_rowid()";
                    
                    var lastRowId64 = (Int64)sqlCommand.ExecuteScalar();
                    return (int)lastRowId64;
                }
            }
        }

        public static void SaveCountry(string countryName)
        {
            var queryStr = string.Format("INSERT INTO Country (Name) VALUES ('{0}');", countryName);
            ExecuteQuery(queryStr);
        }

        public static void DeleteCountry(string countryName)
        {
            var queryStr = string.Format("DELETE FROM Country Where Name = '{0}';", countryName);
            ExecuteQuery(queryStr);
        }

        public static void UpdateCountry(int existingId, string countryName)
        {
            var queryStr = string.Format("Update Country Set Name = '{0}' Where Id = '{1}';", countryName, existingId);
            ExecuteQuery(queryStr);
        }
    }
}
