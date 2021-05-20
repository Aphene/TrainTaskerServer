using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Collections;
using System.Xml;
using System.Data.SqlTypes;
using System.Reflection;

namespace AphAsyncHandler
{
    public class RTDB
    {
        //  public static string connectionString = "Provider=SQLOLEDB;Data Source=JUAR-SQL-AG;Initial Catalog=RapidToteData4;user id=RT;password=1s1p4ss!!;";
        //  public static string connectionString = "Provider=SQLOLEDB;Data Source=172.20.20.27;Initial Catalog=RapidToteData4;user id=RT;password=1s1p4ss!!;";
        //public static string connectionString = "Provider=SQLOLEDB;Data Source=172.20.20.17;Initial Catalog=RapidToteData4;user id=sa;password=1s1p4ss;";
        //  public static string connectionString = "Provider=SQLOLEDB;Data Source=172.20.20.112;Initial Catalog=ISIBet;user id=sa;password=1s1p4ss!!;";

        public static string connectionString = "Provider=SQLOLEDB;Data Source=ZAPSERVERWIN200\\SQLEXPRESS;Initial Catalog=TrainTasker;user id=sa;password=1s1p4ss;";


        public static Queue<OleDbConnection> queue = new Queue<OleDbConnection>();

        public static OleDbConnection getConnection(string database)
        {
            //    if (queue.Count == 0)
            //    {
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = getConnectionString(database);
            connection.Open();
            return connection;



        }

        public static string getConnectionString(string database)
        {
            string s = connectionString;
            database = database.ToLower().Replace("sportsbet", "sportsbet2");

            if (database != "") s = s.Replace("RapidToteData4", database);
            return s;

        }

        public static void returnConnection(OleDbConnection connection)
        {
            connection.Close();

        }

        public static string select(string sql, string database)
        {
            string reply = "";
            List<List<string>> list = lowSelect(sql, database);
            for (int i = 0; i < list.Count; ++i)
            {
                for (int j = 0; j < list[i].Count; ++j)
                {
                    reply += list[i][j] + "|";
                }
                reply += "\r\n";
            }
            //replyBox.Text = encrypt.EncryptData(replyBox.Text);
            return reply;
        }




        public static List<Dictionary<string, string>> listSelect(string sql, string database)
        {
            List<Dictionary<string, string>> lst = new List<Dictionary<string, string>>();


            List<List<string>> rows = lowSelect(sql, database);
            if (rows.Count == 0) return lst;
            List<string> names = rows[0];


            for (int j = 1; j < rows.Count; ++j)
            {
                List<string> values = rows[j];
                Dictionary<string, string> h = new Dictionary<string, string>();
                for (int i = 0; i < names.Count; ++i)
                {
                    h.Add(names[i], values[i]);
                }
                lst.Add(h);
            }

            return lst;
        }

        public static Dictionary<string, string> hSelect(string sql, string database)
        {
            Dictionary<string, string> h = new Dictionary<string, string>();
            //      try
            //      {
            List<List<string>> rows = lowSelect(sql, database);
            if (rows.Count == 0) return h;
            List<string> names = rows[0];
            List<string> values = rows[1];

            for (int i = 0; i < names.Count; ++i)
            {
                h.Add(names[i], values[i]);
            }
            //       }
            //       catch (Exception err)
            //       {
            //           Console.Write(err.Message);
            //      }
            return h;
        }

        public static bool exists(string table, string field, string value, string database)
        {
            string sql = String.Format("SELECT {0} FROM {1} WHERE {2} = '{3}'", field, table, field, value);
            List<List<string>> list = lowSelect(sql, database);
            return list.Count > 0;
        }

        public static int selectCount(string table, string where, string database)
        {
            if (where.ToUpper().IndexOf("WHERE") < 0) where = where + " WHERE " + where;
            List<List<string>> list = lowSelect("SELECT * FROM " + table + where, database);
            if (list.Count == 0) return 0;
            return list.Count - 1;

        }

        public static List<object> selectObList(string sql, object ob, string database)
        {
            List<object> objectList = new List<object>();
            List<List<object>> listlist = lowSelectObs(sql, database);
            for (int i = 2; i < listlist.Count; ++i)
            {
                // Hashtable h = new Hashtable();
                Dictionary<string, string> table = new Dictionary<string, string>();
                for (int j = 0; j < listlist[0].Count; ++j)
                {
                    // h.Add(listlist[0][j] as string, listlist[i][j]);
                    table.Add(listlist[0][j] as string, listlist[i][j].ToString());
                }
                Record record = (Record)Activator.CreateInstance(ob.GetType());
                record.parse(record, table);
                objectList.Add(record);
                //List<object> valueList = new List<object>();

                //foreach (FieldInfo prop in ob.GetType().GetFields())
                //{
                //    string name = prop.Name;
                //    valueList.Add(h[name]);

                //}
                //try
                //{
                //    object newOb = Activator.CreateInstance(ob.GetType(), valueList);
                //    objectList.Add(newOb);
                //}
                //catch(Exception err)
                //{
                //    throw new Exception("Record ("+ob.GetType().Name+") structure does not match DB Table");

                //}

            }
            return objectList;
        }

        //public static Table selectTable(string sql, string database)
        //{
        //    List<List<object>> listlist = lowSelectObs(sql, database);
        //    if (listlist.Count == 0) return null;
        //    Table table = new Table(listlist);
        //    return table;
        //}

        public static List<List<object>> lowSelectObs(string sql, string database)
        {

            OleDbDataReader reader = null;
            OleDbCommand command = new OleDbCommand(sql);
            command.Connection = getConnection(database);


            //     try
            //    {
            reader = command.ExecuteReader();
            //   }
            //    catch (Exception err)
            //    {
            //        command.Connection.Close();
            //        reply(err.Message+"  "+sql);                
            //    }
            List<List<object>> list = new List<List<object>>();

            int index = 0;

            while (reader.Read())
            {
                List<object> names = new List<object>();
                List<object> types = new List<object>();
                List<object> table = new List<object>();
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    if (list.Count == 0)
                    {
                        names.Add(reader.GetName(i));
                        types.Add(reader.GetValue(i).GetType().Name);
                    }
                    table.Add(reader.GetValue(i));
                }
                if (list.Count == 0)
                {
                    list.Add(names);
                    list.Add(types);
                }
                list.Add(table);
            }
            returnConnection(command.Connection);

            return list;
        }

        public static object selectValue(string sql, string database)
        {
            List<List<object>> list = RTDB.lowSelectObs(sql, database);
            if (list.Count == 0) return null;
            return list[2][0];
        }

        public static decimal selectDecimal(string sql, string database)
        {
            List<List<string>> list = RTDB.lowSelect(sql, database);
            if (list.Count < 2) return 0;
            string s = list[1][0];
            try
            {
                return decimal.Parse(s);
            }
            catch (Exception err)
            {
                return 0M;
            }

        }

        public static int selectInt(string sql, string database)
        {
            List<List<string>> list = RTDB.lowSelect(sql, database);
            if (list.Count < 2) return 0;
            string s = list[1][0];
            try
            {
                decimal d = decimal.Parse(s);
                return (int)d;
            }
            catch (Exception err)
            {
                return 0;
            }

        }

        public static List<List<string>> lowSelect(string sql, string database)
        {

            OleDbDataReader reader = null;
            OleDbCommand command = new OleDbCommand(sql);
            command.Connection = getConnection(database);


            //     try
            //    {
            reader = command.ExecuteReader();
            //   }
            //    catch (Exception err)
            //    {
            //        command.Connection.Close();
            //        reply(err.Message+"  "+sql);                
            //    }
            List<List<string>> list = new List<List<string>>();

            int index = 0;

            while (reader.Read())
            {
                List<string> names = new List<string>();
                List<string> table = new List<string>();
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    if (list.Count == 0) names.Add(reader.GetName(i));
                    table.Add(reader.GetValue(i).ToString().Trim());
                }
                if (list.Count == 0) list.Add(names);
                list.Add(table);
            }
            returnConnection(command.Connection);

            return list;
        }

        //      public void deleteDatabase()
        //      {

        //         deleteTable("League");
        //         deleteTable("Game");
        //         deleteTable("Line");
        //     }

        //public static string backupXML(Hashtable h2)
        //{
        //    Hashtable h = new Hashtable();
        //    h.Add("Table", "Patient");
        //    string xml = selectXML(h, "RTServer");
        //    System.IO.TextWriter tr = new System.IO.StreamWriter("c:\\backup.xml");
        //    tr.Write(xml);
        //    tr.Close();
        //    return "Ok";
        //}


        public static int deleteTable(string table, string database)
        {
            string sql = "DELETE FROM " + table;
            return nonQuery(sql, database);
        }

        public static int deleteFromTable(string table, string where, string database)
        {
            string sql = "DELETE FROM " + table;
            if (where != "")
            {
                if (where.ToLower().IndexOf("where") < 0) where = " WHERE " + where;
                sql += " " + where;
            }

            return nonQuery(sql, database);
        }

        //public static string selectXML(Hashtable h, string database)
        //{

        //    string key1 = ""; string key2 = ""; string value1 = ""; string value2 = "";
        //    string order = ""; string orderValue = "";

        //    if (h.Contains("Key1")) key1 = h["Key1"].ToString();
        //    if (h.Contains("Key2")) key2 = h["Key2"].ToString();
        //    if (h.Contains("Value1")) value1 = h["Value1"].ToString();
        //    if (h.Contains("Value2")) value2 = h["Value2"].ToString();
        //    if (h.Contains("Order")) order = h["Order"].ToString();

        //    string table = h["Table"].ToString();

        //    string sql = String.Format("SELECT * FROM {0} ", table);
        //    if (key1 != "") sql += String.Format(" WHERE {0} = '{1}'", key1, value1);
        //    if (key2 != "") sql += String.Format(" AND {0} = '{1}'", key2, value2);

        //    if (order != "") sql += String.Format(" ORDER BY {0} ", order);
        //    //      ISIHttpHandler.log(sql);
        //    List<Dictionary<string, string>> list = listSelect(sql, database);

        //    string xml = "<XML>\r\n";
        //    for (int i = 0; i < list.Count; ++i)
        //    {
        //        xml += "<Record>\r\n";
        //        foreach (KeyValuePair<string, string> kvp in list[i])
        //        {
        //            xml += "<" + kvp.Key + ">" + kvp.Value + "</" + kvp.Key + ">";

        //        }
        //        xml += "\r\n</Record>\r\n";
        //    }
        //    xml += "<Result>Ok</Result>\r\n";
        //    xml += "</XML>";
        //    return xml;

        //}






        //public static string updateXML(Hashtable h, string database)
        //{
        //    string key1 = ""; string key2 = ""; string value1 = ""; string value2 = "";
        //    if (h.Contains("Key1")) key1 = h["Key1"].ToString();
        //    if (h.Contains("Key2")) key2 = h["Key2"].ToString();
        //    if (h.Contains("Value1")) value1 = h["Value1"].ToString();
        //    if (h.Contains("Value2")) value2 = h["Value2"].ToString();
        //    //       string database="";
        //    //      if (h.Contains("Database"))
        //    //      {
        //    //         database = h["Database"].ToString();
        //    //          h.Remove("Database");
        //    //      }

        //    string table = h["Table"].ToString();
        //    h.Remove("Command");
        //    h.Remove("random");
        //    h.Remove("Key1");
        //    h.Remove("Key2");
        //    h.Remove("Value1");
        //    h.Remove("Value2");
        //    h.Remove("Table");
        //    h.Remove("ID");
        //    string sql = "";
        //    if (key1 != "") sql += String.Format(" WHERE {0} = '{1}'", key1, value1);
        //    if (key2 != "") sql += String.Format(" AND {0} = '{1}'", key2, value2);
        //    int rows = updateTable(table, h, sql, database);
        //    return "<XML><Result>" + rows.ToString() + "</Result></XML>";
        //}

        //public static string insertMap(Hashtable h, string database)
        //{
        //    if (h["XML"] == null)
        //        Console.Write("WTF");
        //    string xml = h["XML"].ToString();
        //    XmlDocument xmlDoc = new XmlDocument();
        //    xmlDoc.LoadXml(xml);
        //    XmlNode root = xmlDoc.DocumentElement;
        //    XmlNode name = root.FirstChild;
        //    string nam = name.InnerText;
        //    XmlNode line = name.NextSibling;
        //    int count = 0;
        //    while (line != null)
        //    {
        //        XmlNode lineName = line.FirstChild;
        //        XmlNode lineIndex = lineName.NextSibling;
        //        XmlNode lineDays = lineIndex.NextSibling;
        //        Dictionary<string, string> d = new Dictionary<string, string>();
        //        d.Add("Name", lineName.InnerText);
        //        d.Add("Index", lineIndex.InnerText);
        //        if (lineDays != null) d.Add("Days", lineDays.InnerText);
        //        insert(nam, d, database);
        //        ++count;
        //        line = line.NextSibling;
        //    }

        //    return count.ToString();

        //}

        //public static string insertXML(Hashtable h, string database)
        //{
        //    //  string database = "";
        //    //   if (h.Contains("Database"))
        //    //   {
        //    //      database = h["Database"].ToString();
        //    //       h.Remove("Database");
        //    //   }
        //    string table = h["Table"].ToString();
        //    h.Remove("Command");
        //    h.Remove("random");
        //    h.Remove("Table");
        //    h.Remove("ID");
        //    int rows = insertIntoTable(table, h, database);
        //    return "<XML><Result>" + rows.ToString() + "</Result></XML>";

        //}


        //public static string deleteXML(Hashtable h, string database)
        //{


        //    string key1 = ""; string key2 = ""; string value1 = ""; string value2 = "";
        //    if (h.Contains("Key1")) key1 = h["Key1"].ToString();
        //    if (h.Contains("Key2")) key2 = h["Key2"].ToString();
        //    if (h.Contains("Value1")) value1 = h["Value1"].ToString();
        //    if (h.Contains("Value2")) value2 = h["Value2"].ToString();
        //    string table = h["Table"].ToString();
        //    h.Remove("Command");
        //    h.Remove("random");
        //    h.Remove("Key1");
        //    h.Remove("Key2");
        //    h.Remove("Value1");
        //    h.Remove("Value2");
        //    h.Remove("Table");
        //    h.Remove("ID");
        //    string sql = "";
        //    if (key1 != "") sql += String.Format(" WHERE {0} = '{1}'", key1, value1);
        //    if (key2 != "") sql += String.Format(" AND {0} = '{1}'", key2, value2);
        //    int rows = deleteFromTable(table, sql, database);
        //    return "<XML><Result>" + rows.ToString() + "</Result></XML>";
        //}

        public static int update(Dictionary<string, string> d, string table, string where, string database)
        {
            if (where.ToUpper().IndexOf("WHERE") < 0) where = "WHERE " + where;
            Hashtable h = new Hashtable();
            foreach (KeyValuePair<string, string> kvp in d)
            {
                h.Add(kvp.Key, kvp.Value);
            }
            return updateTable(table, h, where, database);

        }

        public static int updateTable(string table, Hashtable hashtable, string indexName, string indexValue, string database)
        {
            return updateTable(table, hashtable, " WHERE " + indexName + " = '" + indexValue + "'", database);
        }

        public static string DateTimeToString(DateTime dt)
        {
            return new SqlDateTime(dt).ToSqlString().ToString();
        }

        public static int updateTable(string table, Hashtable hashtable, string where, string database)
        {
            string sql = "UPDATE " + table + " SET ";

            string val = "";

            DateTime dt = DateTime.Now;


            IDictionaryEnumerator en = hashtable.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Value != null)
                {
                    if (en.Key.ToString() != "ID")
                    {
                        string value = en.Value.ToString();
                        string typeName = en.GetType().Name;  // liar
                        if (en.Value.GetType() == dt.GetType())
                        {
                            value = new SqlDateTime((DateTime)en.Value).ToSqlString().ToString();
                        }
                        if (en.Value.GetType() == value.GetType() || en.Value.GetType() == dt.GetType()) // if its a string or datatime add quotes
                        {
                            val = " '" + value + "' ";
                        }
                        else
                        {
                            if (value.ToLower().Contains("false")) value = "0";
                            if (value.ToLower().Contains("true")) value = "1";
                            val = " " + value + " ";
                        }


                        sql += "[" + en.Key + "]" + " = " + val + ",";
                    }
                }
            }
            if (sql.Length > 0)
            {
                if (sql[sql.Length - 1] == ',') sql = sql.Substring(0, sql.Length - 1);
            }

            sql += where;

            return nonQuery(sql, database);
        }

        public static int nonQuery(string sql, string database)
        {
            int rows = 0;

            OleDbDataReader reader = null;
            OleDbCommand command = new OleDbCommand(sql);
            int n = 0;
            command.Connection = getConnection(database);
            rows = command.ExecuteNonQuery();
            returnConnection(command.Connection);
            return rows;
        }




        public static void reply(string text)
        {
            //   ISIHttpHandler.isiHttpHandler.replyBox.Text = text;
        }




        public static int insert(string tableName, Dictionary<string, string> d, string database)
        {
            Hashtable h = new Hashtable();
            foreach (KeyValuePair<string, string> kvp in d)
            {
                h.Add(kvp.Key, kvp.Value);
            }
            return insertIntoTable(tableName, h, database);
        }



        public static int insertIntoTable(string tableName, Hashtable hashtable, string database)
        {
            string sql1 = "INSERT INTO " + tableName + " ( ";
            string sql2 = " ) VALUES ( ";
            IDictionaryEnumerator en = hashtable.GetEnumerator();
            bool first = true;
            while (en.MoveNext())
            {

                string value = "";
                Type type = sql1.GetType();
                if (en.Value != null)
                {
                    value = en.Value.ToString();
                    type = en.Value.GetType();
                }

                if (type.Name == "DateTime")
                {
                    value = new SqlDateTime((DateTime)en.Value).ToSqlString().ToString();
                }


                sql1 += "[" + en.Key + "] ,";


                if (type == sql1.GetType() || type.Name == "DateTime") // if its a string add quotes
                {
                    sql2 += " '" + value + "' ,";
                }
                else
                {
                    if (value.ToLower().Contains("false")) value = "0";
                    if (value.ToLower().Contains("true")) value = "1";
                    sql2 += " " + value + " ,";
                }

            }
            if (sql1.Length > 0)
            {
                if (sql1[sql1.Length - 1] == ',') sql1 = sql1.Substring(0, sql1.Length - 1);
                if (sql2[sql2.Length - 1] == ',') sql2 = sql2.Substring(0, sql2.Length - 1);
            }
            string sql = sql1 + sql2 + " )";

            return nonQuery(sql, database);
        }

        public static void logTransactionToDb(string transactionType, string amount, string currency, string kioskID, string insertID, string database)
        {
            Hashtable h = new Hashtable();
            h.Add("TransactionType", transactionType);
            h.Add("Amount", toDollar(amount));
            h.Add("Currency", currency);
            h.Add("Dropped", "0");
            h.Add("KioskID", kioskID);

            string timestamp = getDate();
            h.Add("TimeStampString", timestamp);
            h.Add("InsertID", insertID);
            insertIntoTable("Transactions", h, database);
        }

        public static string toDollar(string cents)
        {
            return (toDecimal(cents) / (decimal)100).ToString("0.00");
        }


        public static decimal toDecimal(string s)
        {
            try
            {
                return System.Convert.ToDecimal(s);


            }
            catch (Exception err)
            {
                return (decimal)0.00;
            }
        }


        public static string getDate()
        {
            DateTime dt = DateTime.Now;
            string ret = dt.Month.ToString() + "/" + dt.Day.ToString() + "/" + dt.Year.ToString() + " " + dt.Hour.ToString() + ":" + dt.Minute.ToString();
            return ret;
        }



    }
}
