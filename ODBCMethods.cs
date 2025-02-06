using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Program;
using TestDb2App;
using TestDb2App.DTO;

namespace TestDb2App
{
    public class ODBCMethods
    {
        public static bool ODBCUpdateAnonymous(string library, string table, object data, object filters, bool debug = false)
        {
            if (data == null || filters == null)
            {
                Console.WriteLine("❌ Error: Update conditions cannot be null.");
                return   false; ;
            }

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ODBCconnectionString))
                {
                    conn.Open();

                    Type type = data.GetType();
                    PropertyInfo[] properties = type.GetProperties();

                    StringBuilder setClause = new StringBuilder();
                    List<OdbcParameter> parameters = new List<OdbcParameter>();

                    int counter = 0;
                    foreach (var prop in properties)
                    {
                        object value = prop.GetValue(data);
                        if (value != null)
                        {
                            if (counter > 0)
                                setClause.Append(", ");

                            setClause.Append($"{prop.Name.ToUpper()} = ?");
                            parameters.Add(new OdbcParameter($"?", value));
                            counter++;
                        }
                    }

                    Type filterType = filters.GetType();
                    PropertyInfo[] filterProperties = filterType.GetProperties();

                    StringBuilder whereClause = new StringBuilder();
                    int whereCounter = 0;
                    foreach (var prop in filterProperties)
                    {
                        object value = prop.GetValue(filters);
                        if (value != null)
                        {
                            if (whereCounter > 0)
                                whereClause.Append(" AND ");

                            whereClause.Append($"{prop.Name.ToUpper()} = ?");
                            parameters.Add(new OdbcParameter($"?", value));
                            whereCounter++;
                        }
                    }

                    string sql = $"UPDATE {library}.{table} SET {setClause} WHERE {whereClause}";

                    using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());

                        if (debug)
                        {
                            Console.WriteLine($"📝 SQL Query: {sql}");
                            Console.WriteLine("🛠 Parameters: " + string.Join(", ", parameters));
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"✅ Update successful! {rowsAffected} row(s) updated.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message);
                return false;
            }
        }
        public static bool ODBCUpdateCust(string library, string table, F101 customer, Dictionary<string, string> keys)
        {
            if (customer == null || keys == null || keys.Count == 0)
            {
                Console.WriteLine("❌ Error: Customer object or WHERE conditions cannot be null.");
                return false;
            }

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ODBCconnectionString))
                {
                    conn.Open();
                    Console.WriteLine("✅ Connected to DB2 on AS/400 successfully!");

                    // Use reflection to get field names and values
                    Type type = customer.GetType();
                    PropertyInfo[] properties = type.GetProperties();

                    StringBuilder setClause = new StringBuilder();
                    List<OdbcParameter> parameters = new List<OdbcParameter>();

                    int counter = 0;
                    foreach (var prop in properties)
                    {
                        object value = prop.GetValue(customer);
                        if (value != null)  // Ignore null values (don't update unchanged fields)
                        {
                            if (counter > 0)
                            {
                                setClause.Append(", ");
                            }

                            setClause.Append($"{prop.Name} = ?");
                            parameters.Add(new OdbcParameter($"?", value));

                            counter++;
                        }
                    }

                    //  Ensure at least one field is being updated
                    if (parameters.Count == 0)
                    {
                        Console.WriteLine("❌ Error: No fields to update.");
                        return false;
                    }

                    // Build WHERE clause
                    StringBuilder whereClause = new StringBuilder();
                    int whereCounter = 0;
                    foreach (var key in keys)
                    {
                        if (whereCounter > 0)
                        {
                            whereClause.Append(" AND ");
                        }
                        whereClause.Append($"{key.Key} = ?");
                        parameters.Add(new OdbcParameter($"?", key.Value));
                        whereCounter++;
                    }

                    // Construct the final UPDATE SQL statement
                    string sql = $"UPDATE {library}.{table} SET {setClause} WHERE {whereClause}";

                    using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                    {
                        // Add all parameters dynamically
                        cmd.Parameters.AddRange(parameters.ToArray());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"✅ Update successful! {rowsAffected} row(s) updated.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message);
                return false;
            }
        }
        public static bool ODBCInsertAnonymous(string library, string table, object data, bool debug = false)
        {
            if (data == null)
            {
                Console.WriteLine("❌ Error: Data object is null.");
                return false;
            }

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ODBCconnectionString))
                {
                    conn.Open();
                    Console.WriteLine("✅ Connected to DB2 on AS/400 successfully!");

                    //  Use reflection to get field names and values dynamically
                    Type type = data.GetType();
                    PropertyInfo[] properties = type.GetProperties();

                    StringBuilder columns = new StringBuilder();
                    StringBuilder values = new StringBuilder();
                    List<OdbcParameter> parameters = new List<OdbcParameter>();

                    int counter = 0;
                    foreach (var prop in properties)
                    {
                        object value = prop.GetValue(data);
                        if (value != null)  // Ignore null values (don't insert unchanged fields)
                        {
                            if (counter > 0)
                            {
                                columns.Append(", ");
                                values.Append(", ");
                            }

                            columns.Append(prop.Name); // Column name
                            values.Append("?"); // Parameter placeholder

                            parameters.Add(new OdbcParameter($"?", value)); // Add parameter
                            counter++;
                        }
                    }

                    //  Ensure at least one field is being inserted
                    if (parameters.Count == 0)
                    {
                        Console.WriteLine("❌ Error: No fields to insert.");
                        return false;
                    }

                    //  Construct the INSERT SQL statement dynamically
                    string sql = $"INSERT INTO {library}.{table} ({columns}) VALUES ({values})";

                    using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                    {
                        //  Add all parameters dynamically
                        cmd.Parameters.AddRange(parameters.ToArray());
                        if (debug)
                        {
                            Console.WriteLine($"📝 SQL Query: {sql}");
                            Console.WriteLine("🛠 Parameters: " + string.Join(", ", parameters));
                        }
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"✅ Insert successful! {rowsAffected} row(s) added.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message);
                return false;
            }
        }

        public static bool ODBCInsertCust(string library, string table, F101 customer)
        {
            if (customer == null)
            {
                Console.WriteLine("❌ Error: Customer object is null.");
                return false;
            }

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ODBCconnectionString))
                {
                    conn.Open();
                    Console.WriteLine("✅ Connected to DB2 on AS/400 successfully!");

                    //  Use reflection to dynamically get field names and values
                    Type type = customer.GetType();
                    PropertyInfo[] properties = type.GetProperties();

                    StringBuilder columns = new StringBuilder();
                    StringBuilder values = new StringBuilder();
                    List<OdbcParameter> parameters = new List<OdbcParameter>();

                    int counter = 0;
                    foreach (var prop in properties)
                    {
                        object value = prop.GetValue(customer);
                        if (value != null)  // Ignore null values
                        {
                            if (counter > 0)
                            {
                                columns.Append(", ");
                                values.Append(", ");
                            }

                            columns.Append(prop.Name); // Column name
                            values.Append("?"); // Parameter placeholder

                            // Convert to correct type if needed
                            parameters.Add(new OdbcParameter($"?", value));
                            counter++;
                        }
                    }

                    //  Construct the INSERT SQL statement dynamically
                    string sql = $"INSERT INTO {library}.{table} ({columns}) VALUES ({values})";

                    using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                    {
                        //  Add all parameters
                        cmd.Parameters.AddRange(parameters.ToArray());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"✅ Insert successful! {rowsAffected} row(s) added.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message);
                return false;
            }
        }

        public static List<Dictionary<string, object>> ODBCFetchWDictionary(string fields, Dictionary<string, string> keys, string library, string file)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ODBCconnectionString))
                {
                    conn.Open();

                    // 🛠 Dynamically build the query
                    StringBuilder sql = new StringBuilder($"SELECT {fields} FROM {library}.{file}");

                    // 🛠 Add WHERE conditions if parameters are provided
                    if (keys.Count > 0)
                    {
                        sql.Append(" WHERE ");
                        int counter = 0;
                        foreach (var key in keys)
                        {
                            if (counter > 0)
                                sql.Append(" AND ");

                            sql.Append($"{key.Key} = ?");
                            counter++;
                        }
                    }

                    // 🛠 Append "FETCH FIRST 10 ROWS ONLY"
                    sql.Append(" FETCH FIRST 10 ROWS ONLY");

                    using (OdbcCommand cmd = new OdbcCommand(sql.ToString(), conn))
                    {
                        // 🛠 Add parameters dynamically to prevent SQL injection
                        foreach (var key in keys)
                        {
                            cmd.Parameters.AddWithValue($"?", key.Value);
                        }

                        using (OdbcDataReader reader = cmd.ExecuteReader())
                        {
                            // 🛠 Get column names dynamically
                            int columnCount = reader.FieldCount;
                            List<string> columnNames = new List<string>();

                            for (int i = 0; i < columnCount; i++)
                            {
                                columnNames.Add(reader.GetName(i));
                            }

                            // 🛠 Read rows dynamically into dictionary objects
                            while (reader.Read())
                            {
                                Dictionary<string, object> row = new Dictionary<string, object>();

                                for (int i = 0; i < columnCount; i++)
                                {
                                    row[columnNames[i]] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }

                                results.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message);
            }

            return results;
        }

        public static List<Dictionary<string, object>> ODBCFetchAnonymous(string fields, object filters, string library, string file,int? limit = 10, bool debug = false)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ODBCconnectionString))
                {
                    conn.Open();

                    // 🛠 Dynamically build the query
                    StringBuilder sql = new StringBuilder($"SELECT {fields} FROM {library}.{file}");

                    List<OdbcParameter> parameters = new List<OdbcParameter>();

                    // 🛠 Use reflection to extract WHERE conditions dynamically
                    if (filters != null)
                    {
                        Type filterType = filters.GetType();
                        PropertyInfo[] filterProperties = filterType.GetProperties();

                        if (filterProperties.Length > 0)
                        {
                            sql.Append(" WHERE ");
                            int counter = 0;

                            foreach (var prop in filterProperties)
                            {
                                object value = prop.GetValue(filters);
                                if (value != null)
                                {
                                    if (counter > 0)
                                        sql.Append(" AND ");

                                    sql.Append($"{prop.Name} = ?");
                                    parameters.Add(new OdbcParameter($"?", value));
                                    counter++;
                                }
                            }
                        }
                    }

                    // 🛠 Append "FETCH FIRST 10 ROWS ONLY"
                    sql.Append($" FETCH FIRST {limit.Value} ROWS ONLY");

                    using (OdbcCommand cmd = new OdbcCommand(sql.ToString(), conn))
                    {
                        // 🛠 Add parameters dynamically to prevent SQL injection
                        cmd.Parameters.AddRange(parameters.ToArray());
                        if (debug)
                        {
                            Console.WriteLine($"📝 SQL Query: {sql}");
                            Console.WriteLine("🛠 Parameters: " + string.Join(", ", parameters));
                        }
                        using (OdbcDataReader reader = cmd.ExecuteReader())
                        {
                            // 🛠 Get column names dynamically
                            int columnCount = reader.FieldCount;
                            List<string> columnNames = new List<string>();

                            for (int i = 0; i < columnCount; i++)
                            {
                                columnNames.Add(reader.GetName(i));
                            }

                            // 🛠 Read rows dynamically into dictionary objects
                            while (reader.Read())
                            {
                                Dictionary<string, object> row = new Dictionary<string, object>();

                                for (int i = 0; i < columnCount; i++)
                                {
                                    row[columnNames[i]] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }

                                results.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message);
            }

            return results;
        }
        public static bool ODBCDelete(string library, string table, object filters)
        {
            if (filters == null)
            {
                Console.WriteLine("❌ Error: WHERE conditions cannot be null.");
                return false;
            }

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ODBCconnectionString))
                {
                    conn.Open();
                    Console.WriteLine("✅ Connected to DB2 on AS/400 successfully!");

                    // 🛠 Use reflection to dynamically extract WHERE conditions
                    Type filterType = filters.GetType();
                    PropertyInfo[] filterProperties = filterType.GetProperties();

                    if (filterProperties.Length == 0)
                    {
                        Console.WriteLine("❌ Error: No WHERE conditions provided.");
                        return false;
                    }

                    StringBuilder whereClause = new StringBuilder();
                    List<OdbcParameter> parameters = new List<OdbcParameter>();
                    int counter = 0;

                    foreach (var prop in filterProperties)
                    {
                        object value = prop.GetValue(filters);
                        if (value != null)
                        {
                            if (counter > 0)
                                whereClause.Append(" AND ");

                            whereClause.Append($"{prop.Name} = ?");
                            parameters.Add(new OdbcParameter($"?", value));
                            counter++;
                        }
                    }

                    // 🛠 Construct the DELETE SQL statement dynamically
                    string sql = $"DELETE FROM {library}.{table} WHERE {whereClause}";

                    using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                    {
                        // 🛠 Add parameters dynamically to prevent SQL injection
                        cmd.Parameters.AddRange(parameters.ToArray());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"✅ Delete successful! {rowsAffected} row(s) deleted.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message);
                return false;
            }
        }
    }
}
