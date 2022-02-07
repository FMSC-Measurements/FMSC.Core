using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace FMSC.Core.Databases
{
    public enum SQLiteDataType
    {
        INTEGER,
        NUMERIC,
        REAL,
        TEXT,
        BLOB
    }

    /// <summary>
    /// SqliteDatabase Manager
    /// </summary>
    public class SQLiteDatabase
    {
        //Properties
        public String ConnectionString { get; protected set; }
        public String FileName { get; protected set; }


        /// <summary>
        ///     Default Constructor for SQLiteDatabase Class.
        /// </summary>
        public SQLiteDatabase()
        {
            FileName = ":memory:";
            ConnectionString = $"Data Source={FileName};";
        }

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="inputFile">Database Object</param>
        public SQLiteDatabase(SQLiteDatabase database)
        {
            ConnectionString = database.ConnectionString;
        }

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="inputFile">The File containing the DB</param>
        public SQLiteDatabase(String inputFile)
        {
            string tmpFN = inputFile.ToLower();
            if (tmpFN == "memory" || tmpFN == "mem" || tmpFN == ":mem:")
                FileName = ":memory:";
            else
                FileName = inputFile;

            ConnectionString = $"URI=file:{FileName}; New=False;";
        }

        /// <summary>
        ///     Single Param Constructor for specifying advanced connection options.
        /// </summary>
        /// <param name="connectionOpts">A dictionary containing all desired options and their values</param>
        public SQLiteDatabase(Dictionary<String, String> connectionOpts)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<String, String> row in connectionOpts)
            {
                sb.AppendFormat("{0}={1}; ", row.Key, row.Value);
            }

            ConnectionString = sb.ToString().Trim();
        }

        /// <summary>
        /// Creates a new SQLiteConnection
        /// </summary>
        /// <returns>SQLiteConnection</returns>
        public SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
        /// <summary>
        /// Creates and opens a new SQLiteConnection
        /// </summary>
        /// <returns>SQLiteConnection</returns>
        public SQLiteConnection CreateAndOpenConnection()
        {
            return CreateConnection().OpenAndReturn();
        }

        /// <summary>
        /// Creates a new SQLiteCommand
        /// </summary>
        /// <param name="connection">Connection to database</param>
        /// <param name="query">SQL Query</param>
        /// <param name="transaction">Optional SQL Transaction</param>
        /// <returns>SQLiteCommand</returns>
        public SQLiteCommand CreateCommand(SQLiteConnection connection, String query, SQLiteTransaction transaction = null)
        {
            return new SQLiteCommand(query, connection, transaction);
        }

        
        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="query">Query to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable GetDataTable(String query)
        {
            DataTable dt = new DataTable();

            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                using (SQLiteCommand cmd = CreateCommand(conn, query))
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                }

                conn.Close();
            }

            return dt;
        }

        /// <summary>
        ///  Execute the command and return the number of rows inserted/updated affected by it.
        /// </summary>
        /// <param name="query">The SQL to be run.</param>
        /// <param name="connection">Connection to database</param>
        /// <param name="transaction">Optional SQL Transaction</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(String query, SQLiteConnection connection = null, SQLiteTransaction transaction = null)
        {
            int re = -1;
            bool closeConn = false;

            if (connection == null)
            {
                connection = CreateAndOpenConnection();
                closeConn = true;
            }

            using (SQLiteCommand cmd = CreateCommand(connection, query, transaction))
            {
                re = cmd.ExecuteNonQuery();
            }

            if (closeConn)
            {
                connection.Close();
                connection.Dispose();
            }

            return re;
        }

        /// <summary>
        /// Execute the command and return the first column of the first row of the resultset
        /// (if present), or null if no resultset was returned.
        /// </summary>
        /// <param name="query">The query to run.</param>
        /// <param name="connection">Connection to database</param>
        /// <param name="transaction">Optional SQL Transaction</param>
        /// <returns>A String.</returns>
        public object ExecuteScalar(String query, SQLiteConnection connection = null, SQLiteTransaction transaction = null)
        {
            object re;

            bool closeConn = false;

            if (connection == null)
            {
                connection = CreateAndOpenConnection();
                closeConn = true;
            }

            using (SQLiteCommand cmd = CreateCommand(connection, query, transaction))
            {
                re = cmd.ExecuteScalar();
            }

            if (closeConn)
            {
                connection.Close();
                connection.Dispose();
            }

            return re;
        }


        /// <summary>
        /// Retrieves a SQLiteDataReader.
        /// </summary>
        /// <param name="query">The query to run.</param>
        /// <param name="connection">Connection to database</param>
        /// <returns>SQLiteDataReader.</returns>
        public SQLiteDataReader ExecuteReader(String query, SQLiteConnection connection)
        {
            using (SQLiteCommand cmd = CreateCommand(connection, query))
            {
                return cmd.ExecuteReader();
            }
        }


        /// <summary>
        /// Update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <param name="connection">Connection to database</param>
        /// <param name="transaction">Transaction the update is a part of</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public int Update(String tableName, Dictionary<String, String> data, String where = null, SQLiteConnection connection = null, SQLiteTransaction transaction = null)
        {
            StringBuilder vals = new StringBuilder();

            if (data.Count > 0)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    if (val.Value == null || val.Value.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                        vals.AppendFormat(" {0} = null,", val.Key.ToString());
                    else
                        vals.AppendFormat(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }

                vals = vals.Remove(vals.Length - 1, 1);

                return ExecuteNonQuery(
                    $"update {tableName} set {vals.ToString()}{(String.IsNullOrEmpty(where) ? String.Empty : $" where {where}")};",
                    connection,
                    transaction
                );
            }

            return 0;
        }

        /// <summary>
        /// Updates rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <param name="connection">Connection to database</param>
        /// <param name="transaction">Transaction the update is a part of</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public int Update(String tableName, Dictionary<String, Object> data, String where = null, SQLiteConnection connection = null, SQLiteTransaction transaction = null)
        {
            int count = 0;

            if (data.Count > 0)
            {
                bool hasTransaction = transaction != null;

                bool closeConn = false;
                if (connection == null)
                {
                    connection = CreateAndOpenConnection();
                    closeConn = true;
                }

                string tmp;
                StringBuilder cv = new StringBuilder();

                using (SQLiteCommand cmd = hasTransaction ? new SQLiteCommand(null, connection, transaction) : new SQLiteCommand(connection))
                {
                    foreach (KeyValuePair<String, Object> param in data)
                    {
                        tmp =$"@v{count}";
                        cv.AppendFormat(" {0} = {1},", param.Key, tmp);

                        if (param.Value == null)
                            cmd.Parameters.AddWithValue(tmp, DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue(tmp, param.Value);
                        count++;
                    }

                    cv = cv.Remove(cv.Length - 1, 1);

                    cmd.CommandText = $"update {tableName} set {cv.ToString()}{(String.IsNullOrEmpty(where) ? String.Empty : $" where {where}")};";

                    count = cmd.ExecuteNonQuery();

                    if (closeConn)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }

            return count;
        }


        /// <summary>
        /// Delets rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <param name="connection">Connection to database</param>
        /// <param name="transaction">Transaction the update is a part of</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public int Delete(String tableName, String where, SQLiteConnection connection = null, SQLiteTransaction transaction = null)
        {
            return ExecuteNonQuery($"delete from {tableName} where {where};", connection, transaction);
        }


        /// <summary>
        /// Inserts data into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="parameters">A dictionary containing column names and their values.</param>
        /// <param name="connection">Connection to database</param>
        /// <param name="transaction">Transaction the update is a part of</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public int Insert(String tableName, Dictionary<String, String> parameters, SQLiteConnection connection = null, SQLiteTransaction transaction = null)
        {
            StringBuilder columns = new StringBuilder();
            StringBuilder values = new StringBuilder();

            foreach (KeyValuePair<String, String> val in parameters)
            {
                columns.AppendFormat(" {0},", val.Key.ToString());

                if (val.Value == null || val.Value.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                    values.Append(" null,");
                else
                    values.AppendFormat(" '{0}',", val.Value);
            }

            columns = columns.Remove(columns.Length - 1, 1);
            values = values.Remove(values.Length - 1, 1);

            return ExecuteNonQuery(
                $"insert into {tableName}({columns}) values({values});",
                connection,
                transaction);
        }

        /// <summary>
        /// Insert data into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing column names and their values.</param>
        /// <param name="connection">Connection to database</param>
        /// <param name="transaction">Transaction the update is a part of</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public int Insert(String tableName, Dictionary<String, Object> data, SQLiteConnection connection = null, SQLiteTransaction transaction = null)
        {
            int count = 0;

            if (data.Count > 0)
            {
                bool hasTransaction = transaction != null;
                
                bool closeConn = false;
                if (connection == null)
                {
                    connection = CreateAndOpenConnection();
                    closeConn = true;
                }

                string tmp;
                StringBuilder cv = new StringBuilder();

                using (SQLiteCommand cmd = new SQLiteCommand(null, connection, transaction))
                {
                    StringBuilder columns = new StringBuilder();
                    StringBuilder values = new StringBuilder();

                    foreach (KeyValuePair<String, Object> param in data)
                    {
                        columns.AppendFormat(" {0},", param.Key);

                        tmp = $"@v{count}";
                        values.AppendFormat(" {0},", tmp);

                        if (param.Value == null)
                            cmd.Parameters.AddWithValue(tmp, DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue(tmp, param.Value);
                        count++;
                    }

                    columns = columns.Remove(columns.Length - 1, 1);
                    values = values.Remove(values.Length - 1, 1);

                    cmd.CommandText = $"insert into {tableName}({columns}) values({values});";

                    count = cmd.ExecuteNonQuery();

                    if (closeConn)
                        connection.Close();
                }
            }

            return count;
        }


        /// <summary>
        ///     Creates a new table with a primary key
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <param name="data">Values for table</param>
        /// <param name="primaryKey">Name of primary key</param>
        /// <param name="connection">Connection to database</param>
        /// <param name="transaction">Optional SQL Transaction</param>
        /// <returns></returns>
        public int CreateTable(String tableName, Dictionary<String, SQLiteDataType> data, String primaryKey = null, SQLiteConnection connection = null, SQLiteTransaction transaction = null)
        {
            bool keyfound = (primaryKey == null);

            StringBuilder sql = new StringBuilder();

            sql.AppendFormat("create table if not exists {0}(", tableName);

            foreach (KeyValuePair<String, SQLiteDataType> kvp in data)
            {
                if (!keyfound && kvp.Key == primaryKey)
                {
                    sql.AppendFormat("{0} {1} PRIMARY KEY, ", kvp.Key, kvp.Value);
                    keyfound = true;
                    continue;
                }

                sql.AppendFormat("{0} {1}, ", kvp.Key, kvp.Value);
            }

            sql = sql.Remove(sql.Length - 2, 2);
            sql.AppendFormat(");");

            return ExecuteNonQuery(sql.ToString(), connection, transaction);
        }

        /// <summary>
        ///     Drops table from database
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <param name="connection">Connection to database</param>
        /// <param name="transaction">Optional SQL Transaction</param>
        /// <returns>if sucessfull</returns>
        public int DropTable(String tableName, SQLiteConnection connection = null, SQLiteTransaction transaction = null)
        {
            return ExecuteNonQuery($"drop table if exists {tableName}", connection, transaction);
        }

        /// <summary>
        ///    Checks whether a sql table exists 
        /// </summary>
        /// <param name="tablename">Name of sql table</param>
        /// <returns>If Table exists</returns>
        public bool TableExists(string tablename)
        {
            bool exists = false;

            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                using (SQLiteDataReader reader = ExecuteReader(
                    $"SELECT name FROM sqlite_master WHERE name ='{tablename}';", conn))
                {
                    if (reader.HasRows)
                        exists = true;

                    reader.Close();
                }

                conn.Close();
            }

            return exists;
        }



        /// <summary>
        /// Deletes ALL data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public void ClearDB()
        {
            DataTable tables = GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");

            foreach (DataRow table in tables.Rows)
            {
                ClearTable(table["NAME"].ToString());
            }
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <param name="connection">Connection to database</param>
        /// <param name="transaction">Optional SQL Transaction</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public void ClearTable(String table, SQLiteConnection connection = null, SQLiteTransaction transaction = null)
        {
            ExecuteNonQuery($"delete from {table};", connection, transaction);
        }

        /// <summary>
        /// Backs up Database.
        /// </summary>
        /// <param name="filename">Name of File to backup to.</param>
        /// <returns></returns>
        public void Backup(string filename)
        {
            SQLiteDatabase _NewDb = new SQLiteDatabase(filename);

            Backup(_NewDb);
        }

        /// <summary>
        /// Backs up Database.
        /// </summary>
        /// <param name="db">Database class object to backup to.</param>
        /// <returns></returns>
        public void Backup(SQLiteDatabase db)
        {
            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                conn.BackupDatabase(db.CreateConnection(), "main", "main", -1, null, 0);

                conn.Close();
            }
        }
    }
}
