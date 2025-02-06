using IBM.Data.Db2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDb2App
{
    internal class DB2CoreMethods
    {
        public static void testupdate(string id, string connectionString)
        {
            using (DB2Connection conn = new DB2Connection(connectionString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connected to DB2!");

                    string query = $"UPDATE devmjf/f101 SET ABALPH = 'mitchtest' WHERE id = {id}";

                    using (DB2Command cmd = new DB2Command(query, conn))
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Rows updated: {rowsAffected}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        public static void testselect(string connectionString)
        {
            using (DB2Connection conn = new DB2Connection(connectionString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connected to DB2!");

                    string query = "SELECT * FROM devmjf/f101 FETCH FIRST 5 ROWS ONLY";

                    using (DB2Command cmd = new DB2Command(query, conn))
                    {
                        using (DB2DataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    Console.Write($"{reader.GetName(i)}: {reader[i]} ");
                                }
                                Console.WriteLine();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        public static void testinsert(string connectionString)
        {
            using (DB2Connection conn = new DB2Connection(connectionString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connected to DB2!");

                    string query = "INSERT INTO devmjf/f101 (column1, column2) VALUES ('Value1', 'Value2')";

                    using (DB2Command cmd = new DB2Command(query, conn))
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Rows inserted: {rowsAffected}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        public static void testdelete(string id, string connectionString)
        {
            using (DB2Connection conn = new DB2Connection(connectionString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connected to DB2!");

                    string query = $"Delete from devmjf/f101 where abno = {id}";


                    using (DB2Command cmd = new DB2Command(query, conn))
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Rows inserted: {rowsAffected}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
