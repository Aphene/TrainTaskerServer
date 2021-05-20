using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AphAsyncHandler
{
    public class Record
    {
        public static string database = "TrainTasker";
        public int ID = 0;
        public string guid = System.Guid.NewGuid().ToString();

        public Record()
        {

        }

        public Record(Hashtable h)
        {
            populate(h);
        }

        public Record(string data)
        {
            parse(data);
        }

        public void populate(Hashtable h)
        {
            foreach (FieldInfo prop in this.GetType().GetFields())
            {
                string name = prop.Name;
                prop.SetValue(this, h[name]);
            }
        }

        public string serialize()
        {

            object ob = this;

            try
            {
                string name;
                Type type = ob.GetType();
              //  string s = type.Name;
                string s = "";
                foreach (FieldInfo prop in ob.GetType().GetFields())
                {
                    // type = Nullable.GetUnderlyingType(prop.FieldType) ?? prop.FieldType;
                    name = prop.Name;
                    object value = prop.GetValue(ob);
                    string valString = "";
                    if (value != null)
                    {
                        valString = value.ToString();
                    }
                    if (s!="") s += ",";
                    s += "\""+name + "\" : \"" + valString+"\"";
                }
                return "{"+s+"}";
            }
            catch (Exception err)
            {
                //  Blog.blog(err.Message + "\r\n" + err.StackTrace);
                return "";
            }
        }


        public object parse(string data)
        {
            try
            {
                data = data.Replace("\"", "");
                data = data.Replace("{", "");
                data = data.Replace("}", "");
                Dictionary<string, string> table = new Dictionary<string, string>();
                string[] pairs = data.Split(new char[] { ',' });
                string objectTypeName = pairs[0];
                Type type = Type.GetType(getNameSpace() + "." + objectTypeName);
                object ob = this;
                for (int i = 0; i < pairs.Length; ++i)
                {
                    string[] pair = pairs[i].Split(new char[] { ':' });
                    if (pair.Length == 2)
                    {
                        string name = pair[0].Trim();
                        string value = pair[1].Trim();
                        table.Add(name, value);
                    }
                }
                return parse(ob, table);
            }
            catch (Exception err)
            {
             
                return null;
            }
        }



        public object parse(object ob, Dictionary<string, string> table)
        {
            try
            {
                string name;
                Type type;
                foreach (FieldInfo prop in ob.GetType().GetFields())
                {

                    type = Nullable.GetUnderlyingType(prop.FieldType) ?? prop.FieldType;
                    name = prop.Name;
                    if (table.ContainsKey(name))
                    {
                        if (type.Name == "Int32") prop.SetValue(ob, parseInt(table[name]));
                        if (type.Name == "String") prop.SetValue(ob, table[name]);
                        if (type.Name == "Bool") prop.SetValue(ob, parseBool(table[name]));
                        if (type.Name == "Boolean") prop.SetValue(ob, parseBool(table[name]));
                        if (type.Name == "Int64") prop.SetValue(ob, parseLong(table[name]));
                        if (type.Name == "Decimal") prop.SetValue(ob, parseDecimal(table[name]));
                        if (type.Name == "DateTime") prop.SetValue(ob, parseDateTime(table[name]));
                    }

                }
                return ob;
            }
            catch (Exception err)
            {
                //   Blog.blog(err.Message + "\r\n" + err.StackTrace);
                return null;
            }
        }

        string getNameSpace()
        {
            Type type = this.GetType();
            string[] fields = type.Name.Split(new char[] { '.' });
            string space = fields[0];
            return space;
        }

        int parseInt(string s)
        {
            try
            {
                return Int32.Parse(s);
            }
            catch
            {
                return 0;
            }
        }

        bool parseBool(string s)
        {
            try
            {
                return bool.Parse(s);
            }
            catch
            {
                return false;
            }
        }

        long parseLong(string s)
        {
            try
            {
                return long.Parse(s);
            }
            catch
            {
                return 0;
            }
        }

        decimal parseDecimal(string s)
        {
            try
            {
                return decimal.Parse(s);
            }
            catch
            {
                return 0;
            }
        }
        DateTime parseDateTime(string s)
        {
            try
            {
                return DateTime.Parse(s);
            }
            catch
            {
                return new DateTime(1899, 1, 1);
            }
        }


        public void insertOb()
        {

            Type type = this.GetType();
            string tableName = type.Name.Replace(getNameSpace() + ".", "");
            Hashtable h = new Hashtable();
            foreach (FieldInfo prop in this.GetType().GetFields())
            {

                string name = prop.Name;
                if (name != "ID")
                {
                    object value = prop.GetValue(this);
                    h.Add(name, value);
                }
            }
            int rows = RTDB.insertIntoTable(tableName, h, database);
            if (rows > 0)
            {
                selectOb();  // populate ID from db.
            }

        }

        public void deleteFromTable()
        {
            Type type = this.GetType();
            string tableName = type.Name.Replace(getNameSpace() + ".", "");
            try
            {
                RTDB.deleteFromTable(tableName, "WHERE ID=" + this.ID.ToString(), database);
            }
            catch (Exception err) { }
        }

        public object select(string sql)
        {
            Type type = this.GetType();


            List<List<object>> table = RTDB.lowSelectObs(sql, database);
            if (table.Count == 0) return null;
            if (table.Count == 1)
            {
                var wtf = true;
                return null;
            }
            List<string> names = new List<string>();
            for (int i = 0; i < table[0].Count; ++i)
            {
                names.Add(table[0][i] as string);
            }

            FieldInfo[] fields = this.GetType().GetFields();

            for (int i = 0; i < fields.Length; ++i)
            //foreach (FieldInfo prop in this.GetType().GetFields())
            {
                FieldInfo prop = fields[i];
                type = Nullable.GetUnderlyingType(prop.FieldType) ?? prop.FieldType;
                string name = prop.Name;
                int index = names.IndexOf(name);
                object value = table[2][index];
                if (type.Name == "String")
                {
                    if (value == null) value = "";
                    if (value.GetType().Name == "DBNull") value = "";
                }
                if (type.Name == "Bool" || type.Name == "Boolean")
                {
                    if (value == null) value = false;
                    if (value.GetType().Name == "DBNull") value = false;
                    if (value.ToString().Contains("1")) value = true;
                    if (value.ToString().Contains("0")) value = false;

                }
                if (type.Name == "DateTime" || type.Name == "DateTime")
                {
                    try
                    {
                        value = parseDateTime(value as string);
                    }
                    catch (Exception err)
                    {
                        value = new DateTime(1899, 1, 1);
                    }

                }
                if (type.Name != value.GetType().Name)
                {
                    var wtf = value.GetType().Name;

                }
                else
                {
                    prop.SetValue(this, value);
                }
            }
            return this;
        }



        public object selectOb()
        {
            Type type = this.GetType();
            string tableName = type.Name.Replace(getNameSpace() + ".", "");
            string sql = "SELECT * FROM " + tableName + " WHERE [guid] = '" + this.guid + "'";
            return select(sql);

        }

        public object selectOb(int id)
        {
            Type type = this.GetType();
            string tableName = type.Name.Replace(getNameSpace() + ".", "");
            string sql = "SELECT * FROM " + tableName + " WHERE [ID] = '" + id.ToString() + "'";
            return select(sql);

        }

        public object selectOb(string where)
        {
            Type type = this.GetType();
            string tableName = type.Name.Replace(getNameSpace() + ".", "");
            where = where.Replace("WHERE", "");
            where = where.Replace("where", "");
            string sql = "SELECT * FROM " + tableName + " WHERE " + where;
            return select(sql);

        }

        public int updateOb()
        {
            Type type = this.GetType();
            string tableName = type.Name.Replace(getNameSpace() + ".", "");
            Hashtable h = new Hashtable();
            foreach (FieldInfo prop in this.GetType().GetFields())
            {

                string name = prop.Name;
                if (name != "ID")
                {
                    object value = prop.GetValue(this);
                    h.Add(name, value);
                }
            }
            int rows = RTDB.updateTable(tableName, h, "WHERE [ID] = " + this.ID.ToString(), database);
            return rows;
        }

        public int updateOb(string sql)
        {
            Type type = this.GetType();
            string tableName = type.Name.Replace(getNameSpace() + ".", "");
            Hashtable h = new Hashtable();
            foreach (FieldInfo prop in this.GetType().GetFields())
            {

                string name = prop.Name;
                if (name != "ID")
                {
                    object value = prop.GetValue(this);
                    h.Add(name, value);
                }
            }
            int rows = RTDB.updateTable(tableName, h, "WHERE [ID] = " + this.ID.ToString(), database);
            return rows;
        }


        public string createTableSQL()
        {
            Type type = this.GetType();
            string tableName = type.Name.Replace(getNameSpace() + ".", "");
            string sql = "";
            sql += "create table " + tableName + "( ";
            sql += "ID INT IDENTITY(1,1) PRIMARY KEY ";

            var fields = this.GetType().GetFields();



            foreach (FieldInfo prop in this.GetType().GetFields())
            {
                type = Nullable.GetUnderlyingType(prop.FieldType) ?? prop.FieldType;
                string name = prop.Name;
                if (name != "ID")
                {
                    if (type.Name == "String")
                    {
                        object value = prop.GetValue(this);
                        int len = value.ToString().Length;
                        if (len == 0) len = 128;
                        sql += "," + name + " VARCHAR(" + len.ToString() + ") ";
                    }
                    if (type.Name == "Int32") sql += "," + name + " INT ";
                    if (type.Name == "Bool") sql += "," + name + " BIT ";
                    if (type.Name == "Boolean") sql += "," + name + " BIT ";
                    if (type.Name == "Int64") sql += "," + name + " BIGINT ";
                    if (type.Name == "Decimal") sql += "," + name + " DECIMAL(10,2) ";
                    if (type.Name == "DateTime") sql += "," + name + " DATETIMEOFFSET ";
                }
            }
            sql += ")";
            return sql;
        }
    }
}
