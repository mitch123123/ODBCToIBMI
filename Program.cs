using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Reflection;
using System.Text;
using IBM.Data.Db2;
using TestDb2App;
using TestDb2App.DTO;
using TestDb2App.Helpers;
class Program
{

    public static string ODBCconnectionString ;
    static void Main()
    {
        try
        {
             ODBCconnectionString = ConnectionHelper.GetDB2ConnectionString();
            Console.WriteLine("🔒 DB2 Connection String Loaded Securely!");
            Console.WriteLine(ODBCconnectionString); // Debugging
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        TestODBCInsert();
        TestODBCUpdate();
        TestODBCFetch();
        TestODBCDelete();

        //testselect(DB2CoreconnectionString);
        //testupdate("000905",DB2CoreconnectionString);
        //testinsert(DB2CoreconnectionString);
        //testdelete("000903",DB2CoreconnectionString);
    }

    public static void TestODBCInsert()
    {
        F101 newCustomer = new F101()
        {
            ABSTAT = "A",
            ABPRFX = "C",
            ABNO = "123456",
            ABALPH = "John Doe",
            ABALNM = "Doe",
            ABAREA = 415,
            ABPHON = 1234567,
            ABEXT1 = "1001",
            ABARA2 = 212,
            ABPHN2 = 7654321
        };
        ODBCMethods.ODBCInsertCust("devmjf", "f101", newCustomer);//can be full or partial data
        ODBCMethods.ODBCInsertAnonymous("devmjf", "f101", new
        {
            ABPRFX = "C",
            ABNO = "654321",
            ABALPH = "Doe John ",
            ABALNM = "john",
            ABAREA = 415,
            ABPHON = 1234567
        });
    }

    public static void TestODBCUpdate()
    {
        F101 updateCustomer = new F101()
        {
            ABALPH = "John Updated",
            ABALNM = "Doe Updated"
        };
        Dictionary<string, string> whereConditions = new Dictionary<string, string>
        {
            { "ABNO", "123456" }
        };
        ODBCMethods.ODBCUpdateCust("devmjf", "f101", updateCustomer, whereConditions);
        ODBCMethods.ODBCUpdateAnonymous("devmjf", "f101", new
        {
            ABSTAT = "B",
            ABZIP = "98765",
            ABAREA = 415
        }, new Dictionary<string, string>
        {
            { "ABNO", "654321" },
            { "ABSTAT", "A" }
        });
    }
    public static void TestODBCFetch()
    {
        var customers = ODBCMethods.ODBCFetchWDictionary("ABNO, ABALPH, ABALNM", new Dictionary<string, string>
            {{ "ABNO", "123456" }}, "devmjf", "f101");

        // 🛠 Process fetched results
        foreach (var row in customers)
        {
            foreach (var column in row)
            {
                Console.WriteLine($"{column.Key}: {column.Value ?? "NULL"}");
            }
            Console.WriteLine("----------------------------");
        }//fields wanting to fethch, and keys used in where condition
        var customers2 = ODBCMethods.ODBCFetchAnonymous("ABNO, ABALPH, ABALNM", new
        {
            ABNO = "123456"
        }, "devmjf", "f101");

        //  Print fetched results
        
        foreach (var row in customers)
        {
            Console.WriteLine("\n🔹 New Row:");

            foreach (var column in row)
            {
                Console.WriteLine($"{column.Key}: {column.Value ?? "NULL"}"); // Handle NULL values
            }
        }
    }
    public static void TestODBCDelete()
    {
        ODBCMethods.ODBCDelete("devmjf", "f101", new
        {
            ABNO = "123456"
        });
    }


}
