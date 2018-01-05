using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using PirateX.Core;
using PirateX.Core.Container;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ForeignKeyConstraint = ServiceStack.OrmLite.ForeignKeyConstraint;

namespace PirateX.ServiceStackV3
{
    public static class ServerExtention
    {
        private static List<string> GetSqlServerColumnNames(IDbConnection db, string tableName)
        {
            var columns = new List<string>();
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = $"SELECT name FROM SysColumns WHERE id=Object_Id('{tableName}')";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var ordinal = reader.GetOrdinal("name");
                    columns.Add(reader.GetString(ordinal).ToLower());
                }
                reader.Close();
            }
            return columns;
        }

        private static List<string> GetMySqlColumnNames(IDbConnection db, string tableName)
        {
            var columns = new List<string>();
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "SHOW COLUMNS FROM  " + tableName;
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var ordinal = reader.GetOrdinal("FIELD");
                    columns.Add(reader.GetString(ordinal).ToLower());
                }
                reader.Close();
            }
            return columns;
        }

        private static List<string> GetColumnNames(IDbConnection db, string tableName)
        {
            var columns = new List<string>();
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = $"SELECT name FROM SysColumns WHERE id=Object_Id('{tableName}')";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var ordinal = reader.GetOrdinal("name");
                    columns.Add(reader.GetString(ordinal).ToLower());
                }
                reader.Close();
            }
            return columns;
        }

        public static void AlterTableSqlServer<T>(this IDbConnection db)
        {
            var type = typeof(T);
            AlterTableSqlServer(db, type, type.GetModelMetadata());
        }

        public static void AlterTableSqlServer(this IDbConnection db, Type type)
        {
            var t = typeof(ServerExtention);

            t.GetMethod("AlterTableSqlServer", BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(type)
                .Invoke(t, new[] { db });
        }

        public static void AlterTableSqlServer(this IDbConnection db, Type type, ModelDefinition model)
        {
            if (db.TableExists(model.ModelName) == false)
            {
                db.CreateTable(false, type);

                if (type.GetCustomAttributes(typeof(DefaultEventAttribute), false).Any())
                {
                    var attr = (DefaultEventAttribute) type.GetCustomAttributes(typeof(DefaultEventAttribute),
                        false)[0];
                    if (!string.IsNullOrEmpty(attr.Name))
                    {
                        db.ExecuteSql(attr.Name);
                    }
                }

                return;
            }

            // find each of the missing fields
            var columns = GetColumnNames(db, model.ModelName);
            for (int i = 0; i < columns.Count(); i++)
                columns[i] = columns[i].Trim(' ').Trim('"').ToLower();
            var missing = model.FieldDefinitions
                .Where(field => columns.Contains(field.FieldName.ToLower()) == false)
                .ToList();

            // add a new column for each missing field
            foreach (var field in missing)
            {
                string alterSql = null;
                //if (string.IsNullOrEmpty(field.DefaultValue))
                //{
                //    var fieldType = field.FieldType;
                //    if (fieldType == typeof(DateTime))
                //    {
                //        field.DefaultValue = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                //    }
                //    else if (fieldType == typeof(DateTimeOffset))
                //    {
                //        field.DefaultValue = "00:00:00";
                //    }
                //    else if (fieldType.IsPrimitive)
                //    {
                //        if (fieldType == typeof(bool))
                //        {
                //            field.DefaultValue = "0";
                //        }
                //        else if (fieldType == typeof(string))
                //        {
                //            field.DefaultValue = "";
                //        }
                //        else
                //        {
                //            var typeCode = fieldType.GetTypeCode();
                //            switch (typeCode)
                //            {
                //                case TypeCode.Double:
                //                case TypeCode.Decimal:
                //                case TypeCode.Byte:
                //                case TypeCode.Int16:
                //                case TypeCode.Int32:
                //                case TypeCode.Int64:
                //                case TypeCode.SByte:
                //                case TypeCode.UInt16:
                //                case TypeCode.UInt32:
                //                case TypeCode.UInt64:
                //                    field.DefaultValue = "0";
                //                    break;
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    alterSql = string.Format("ALTER TABLE {0} ADD [{1}] {2} {3}",
                //    model.ModelName,
                //    field.FieldName,
                //    db.GetDialectProvider().GetColumnTypeDefinition(field.FieldType),
                //    string.Format("DEFAULT '{0}'", field.DefaultValue)
                //    );
                //}

                alterSql = string.Format("ALTER TABLE [{0}] ADD {1}",
                    model.ModelName,
                    //                "",
                    PirateXSqlServerDialectProvider.Instance.GetColumnDefinition(field.FieldName, field.FieldType,
                        field.IsPrimaryKey, field.AutoIncrement, field.IsNullable, field.IsRowVersion,
                        field.FieldLength, field.Scale, field.DefaultValue, field.CustomFieldDefinition)
                    //field.DefaultValue == null?"":string.Format("DEFAULT '{0}'", field.DefaultValue)
                );


                db.ExecuteSql(alterSql);
                if (field.DefaultValue != null)
                    db.ExecuteSql(string.Format("UPDATE {0} SET {1} = {2}", model.ModelName,
                        SqlServerOrmLiteDialectProvider.Instance.GetQuotedColumnName(field.FieldName),
                        SqlServerOrmLiteDialectProvider.Instance.GetQuotedValue(field.DefaultValue)));
            }
        }
        public static void AlterTableMysql(this IDbConnection db, Type type)
        {
            var t = typeof(ServerExtention);

            t.GetMethod("AlterMySqlTable2", BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(type)
                .Invoke(t, new[] { db });
        }
        public static void AlterMySqlTable2<T>(this IDbConnection db)
        {
            var type = typeof(T);
            AlterMySqlTable(db, type, type.GetModelMetadata());
        }

        public static void AlterMySqlTable(this IDbConnection db, Type type, ModelDefinition model)
        {
            // just create the table if it doesn't already exist
            if (db.TableExists(model.ModelName) == false)
            {
                db.CreateTable(false, type);

                if (type.GetCustomAttributes(typeof(DefaultEventAttribute), false).Any())
                {
                    var attr = (DefaultEventAttribute) type.GetCustomAttributes(typeof(DefaultEventAttribute),
                        false)[0];
                    if (!string.IsNullOrEmpty(attr.Name))
                    {
                        db.ExecuteSql(attr.Name);
                    }
                }

                //db.DataImport<T>(type,model);
                return;
            }

            // find each of the missing fields
            var columns = GetMySqlColumnNames(db, model.ModelName);
            var missing = model.FieldDefinitions
                .Where(field => columns.Contains(field.FieldName.ToLower()) == false)
                .ToList();

            // add a new column for each missing field
            foreach (var field in missing)
            {
                string alterSql = null;
                if (string.IsNullOrEmpty(field.DefaultValue))
                {
                    alterSql = string.Format("ALTER TABLE {0} ADD {1} {2}",
                        model.ModelName,
                        "", //field.FieldName,
                        db.GetDialectProvider().GetColumnDefinition(field.FieldName, field.FieldType,
                            field.IsPrimaryKey, field.AutoIncrement, field.IsNullable, field.IsRowVersion,
                            field.FieldLength, field.Scale, field.DefaultValue,
                            field.CustomFieldDefinition) // .GetColumnTypeDefinition(field.FieldType)
                    );
                }
                else
                {
                    alterSql = string.Format("ALTER TABLE {0} ADD {1} {2} {3}",
                        model.ModelName,
                        "", //field.FieldName,
                        db.GetDialectProvider().GetColumnDefinition(field.FieldName, field.FieldType,
                            field.IsPrimaryKey, field.AutoIncrement, field.IsNullable, field.IsRowVersion,
                            field.FieldLength, field.Scale, field.DefaultValue, field.CustomFieldDefinition),
                        string.Format("DEFAULT '{0}'", field.DefaultValue)
                    );
                }

                db.ExecuteSql(alterSql);

                //db.ExecuteSql(string.Format("UPDATE {0} SET {1} = {2}", model.ModelName, PokemonXMySqlDialectProvider.Instance.GetQuotedColumnName(field.FieldName),
                //    PokemonXMySqlDialectProvider.Instance.GetQuotedValue(field.DefaultValue)));
            }
        }

        public static bool IsValueType(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        internal static bool CheckForIdField(IEnumerable<PropertyInfo> objProperties)
        {
            // Not using Linq.Where() and manually iterating through objProperties just to avoid dependencies on System.Xml??
            foreach (var objProperty in objProperties)
            {
                if (objProperty.Name != OrmLiteConfig.IdField) continue;
                return true;
            }
            return false;
        }
    }
    
}
