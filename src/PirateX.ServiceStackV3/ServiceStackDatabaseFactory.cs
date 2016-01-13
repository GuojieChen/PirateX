using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using PirateX.Core;
using ServiceStack.Common;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using ForeignKeyConstraint = ServiceStack.OrmLite.ForeignKeyConstraint;

namespace PirateX.ServiceStackV3
{
    public class ServiceStackDatabaseFactory : IDatabaseFactory
    {
        private IDbConnectionFactory dbConnectionFactory;
        private readonly bool IsMysql; 

        public ServiceStackDatabaseFactory(string connectionstring)
        {
            IsMysql = IsMySql(connectionstring); 

            if (IsMysql)
                dbConnectionFactory = new OrmLiteConnectionFactory(connectionstring, MySqlDialect.Provider);
            else
                dbConnectionFactory = new OrmLiteConnectionFactory(connectionstring,SqlServerDialect.Provider);

            ConnectionString = connectionstring;
        }

        private static readonly string[] MySqlKeys = new[] {"persist security info", "charset", "allow user variables"};

        private static bool IsMySql(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return false;

            var items = connectionString.Split(new char[] {';'});
            return items.Select(item => item.Split(new char[] {'='})).Any(ss => MySqlKeys.Contains(ss[0].ToLower()));
        }

        public string ConnectionString { get; }

        public bool TableExists<T>()
        {
            using (var db = dbConnectionFactory.OpenDbConnection())
                return db.TableExists(typeof (T).Name);
        }

        public void CreateTableIfNotExists<T>()
        {
            using (var db = dbConnectionFactory.OpenDbConnection())
                db.CreateTableIfNotExists(typeof (T));
        }

        public void DropTable<T>()
        {
            using (var db = dbConnectionFactory.OpenDbConnection())
                db.DropTable(typeof (T));
        }

        public void DropAndCreateTable<T>()
        {
            using (var db = dbConnectionFactory.OpenDbConnection())
                db.DropAndCreateTable(typeof (T));
        }

        public void AlterTable<T>()
        {
            using (var db = dbConnectionFactory.OpenDbConnection())
            {
                if(IsMysql)
                    db.AlterMySqlTable<T>();
                else 
                    db.AlterTableSqlServer<T>();
            }
        }

        public IList<T> Select<T>()
        {
            using (var db = dbConnectionFactory.OpenDbConnection())
                return db.Select<T>();
        }

        public void CreateAndAlterTable(IEnumerable<Type> types)
        {
            using (var db = dbConnectionFactory.OpenDbConnection())
            {
                foreach (var type in types)
                {
                    if (db.TableExists(type.Name))
                    {
                        if (IsMysql)
                            db.AlterMySqlTable(type);
                        else
                            db.AlterTableSqlServer(type);
                    }
                    else
                    {
                        db.CreateTable(false, type);
                    }
                }
            }
        }

    }

    public static class SqlserverExtention
    {
        private static List<string> GetSqlServerColumnNames(IDbConnection db, string tableName)
        {
            var columns = new List<string>();
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "SELECT name FROM SysColumns WHERE id=Object_Id('{0}')".Fmt(tableName);
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


        public static void AlterTableSqlServer<T>(this IDbConnection db)
        {
            AlterTableSqlServer(db,typeof(T));
        }

        public static void AlterTableSqlServer(this IDbConnection db, Type type)
        {
            var model = GetModelDefinition(type);

            // just create the table if it doesn't already exist
            if (db.TableExists(model.ModelName) == false)
            {
                db.CreateTable(false, type);

                if (type.GetCustomAttributes(typeof(DefaultEventAttribute), false).Any())
                {
                    var attr = (DefaultEventAttribute)type.GetCustomAttributes(typeof(DefaultEventAttribute), false)[0];
                    if (!string.IsNullOrEmpty(attr.Name))
                    {
                        db.ExecuteSql(attr.Name);
                    }
                }
                return;
            }

            // find each of the missing fields
            var columns = GetSqlServerColumnNames(db, model.ModelName);
            for (int i = 0; i < columns.Count(); i++)
                columns[i] = columns[i].Trim(' ').Trim('"').ToLower();
            var missing = model.FieldDefinitions
                .Where(field => columns.Contains(field.FieldName.ToLower()) == false)
                .ToList();

            // add a new column for each missing field
            foreach (var field in missing)
            {
                string alterSql = null;

                if (string.IsNullOrEmpty(field.DefaultValue))
                {
                    alterSql = string.Format("ALTER TABLE [{0}] ADD {1} {2}",
                    model.ModelName,
                    "",//field.FieldName,
                    db.GetDialectProvider().GetColumnDefinition(field.FieldName, field.FieldType, field.IsPrimaryKey, field.AutoIncrement, field.IsNullable, field.FieldLength, field.Scale, field.DefaultValue)
                    );
                }
                else
                {
                    alterSql = string.Format("ALTER TABLE [{0}] ADD {1} {2} {3}",
                    model.ModelName,
                    "",//field.FieldName,
                    db.GetDialectProvider().GetColumnDefinition(field.FieldName, field.FieldType, field.IsPrimaryKey, field.AutoIncrement, field.IsNullable, field.FieldLength, field.Scale, field.DefaultValue),
                    string.Format("DEFAULT '{0}'", field.DefaultValue)
                    );
                }

                db.ExecuteSql(alterSql);
            }

        }

        public static void AlterMySqlTable<T>(this IDbConnection db)
        {
            AlterMySqlTable(db,typeof(T));
        }

        public static void AlterMySqlTable(this IDbConnection db,Type type)
        {
            var model = GetModelDefinition(type);
            // just create the table if it doesn't already exist
            if (db.TableExists(model.ModelName) == false)
            {
                db.CreateTable(false, type);

                if (type.GetCustomAttributes(typeof(DefaultEventAttribute), false).Any())
                {
                    var attr = (DefaultEventAttribute)type.GetCustomAttributes(typeof(DefaultEventAttribute), false)[0];
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
                    "",//field.FieldName,
                    db.GetDialectProvider().GetColumnDefinition(field.FieldName, field.FieldType, field.IsPrimaryKey, field.AutoIncrement, field.IsNullable,  field.FieldLength, field.Scale, field.DefaultValue)
                    );
                }
                else
                {
                    alterSql = string.Format("ALTER TABLE {0} ADD {1} {2} {3}",
                    model.ModelName,
                    "",//field.FieldName,
                    db.GetDialectProvider().GetColumnDefinition(field.FieldName, field.FieldType, field.IsPrimaryKey, field.AutoIncrement, field.IsNullable, field.FieldLength, field.Scale, field.DefaultValue),
                    string.Format("DEFAULT '{0}'", field.DefaultValue)
                    );
                }

                db.ExecuteSql(alterSql);
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


        private static bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType
                && theType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }


        private static Dictionary<Type, ModelDefinition> typeModelDefinitionMap = new Dictionary<Type, ModelDefinition>();
        public static ModelDefinition GetModelDefinition(Type modelType)
        {
            ModelDefinition modelDef = null;
            if (typeModelDefinitionMap.TryGetValue(modelType, out modelDef))
                return modelDef;

            if (modelType.IsValueType() || modelType == typeof(string))
                return null;

            var modelAliasAttr = modelType .FirstAttribute<AliasAttribute>();
            var schemaAttr = modelType.FirstAttribute<SchemaAttribute>();
            modelDef = new ModelDefinition
            {
                ModelType = modelType,
                Name = modelType.Name,
                Alias = modelAliasAttr != null ? modelAliasAttr.Name : null,
                Schema = schemaAttr != null ? schemaAttr.Name : null
            };

            modelDef.CompositeIndexes.AddRange(
                modelType.GetCustomAttributes(typeof(CompositeIndexAttribute), true).ToList()
                .ConvertAll(x => (CompositeIndexAttribute)x));

            var objProperties = modelType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance).ToList();

            var hasIdField = CheckForIdField(objProperties);

            var i = 0;
            foreach (var propertyInfo in objProperties)
            {
                var sequenceAttr = propertyInfo.FirstAttribute<SequenceAttribute>();
                var computeAttr = propertyInfo.FirstAttribute<ComputeAttribute>();
                var pkAttribute = propertyInfo.FirstAttribute<PrimaryKeyAttribute>();
                var decimalAttribute = propertyInfo.FirstAttribute<DecimalLengthAttribute>();
                var belongToAttribute = propertyInfo.FirstAttribute<BelongToAttribute>();
                var isFirst = i++ == 0;

                var isPrimaryKey = propertyInfo.Name == OrmLiteConfig.IdField || (!hasIdField && isFirst)
                    || pkAttribute != null;

                var isNullableType = IsNullableType(propertyInfo.PropertyType);

                var isNullable = (!propertyInfo.PropertyType.IsValueType
                                   && propertyInfo.FirstAttribute<RequiredAttribute>() == null)
                                 || isNullableType;

                var propertyType = isNullableType
                    ? Nullable.GetUnderlyingType(propertyInfo.PropertyType)
                    : propertyInfo.PropertyType;

                var aliasAttr = propertyInfo.FirstAttribute<AliasAttribute>();

                var indexAttr = propertyInfo.FirstAttribute<IndexAttribute>();
                var isIndex = indexAttr != null;
                var isUnique = isIndex && indexAttr.Unique;

                var stringLengthAttr = propertyInfo.FirstAttribute<StringLengthAttribute>();

                var defaultValueAttr = propertyInfo.FirstAttribute<DefaultAttribute>();

                var referencesAttr = propertyInfo.FirstAttribute<ReferencesAttribute>();
                var foreignKeyAttr = propertyInfo.FirstAttribute<ForeignKeyAttribute>();

                if (decimalAttribute != null && stringLengthAttr == null)
                    stringLengthAttr = new StringLengthAttribute(decimalAttribute.Precision);

                var fieldDefinition = new FieldDefinition
                {
                    Name = propertyInfo.Name,
                    Alias = aliasAttr != null ? aliasAttr.Name : null,
                    FieldType = propertyType,
                    PropertyInfo = propertyInfo,
                    IsNullable = isNullable,
                    IsPrimaryKey = isPrimaryKey,
                    AutoIncrement =
                        isPrimaryKey &&
                        propertyInfo.FirstAttribute<AutoIncrementAttribute>() != null,
                    IsIndexed = isIndex,
                    IsUnique = isUnique,
                    FieldLength =
                        stringLengthAttr != null
                            ? stringLengthAttr.MaximumLength
                            : (int?)null,
                    DefaultValue =
                        defaultValueAttr != null ? defaultValueAttr.DefaultValue : null,
                    ForeignKey =
                        foreignKeyAttr == null
                            ? referencesAttr == null
                                  ? null
                                  : new ForeignKeyConstraint(referencesAttr.Type)
                            : new ForeignKeyConstraint(foreignKeyAttr.Type,
                                                       foreignKeyAttr.OnDelete,
                                                       foreignKeyAttr.OnUpdate,
                                                       foreignKeyAttr.ForeignKeyName),
                    GetValueFn = propertyInfo.GetPropertyGetterFn(),
                    SetValueFn = propertyInfo.GetPropertySetterFn(),
                    Sequence = sequenceAttr != null ? sequenceAttr.Name : string.Empty,
                    IsComputed = computeAttr != null,
                    ComputeExpression =
                        computeAttr != null ? computeAttr.Expression : string.Empty,
                    Scale = decimalAttribute != null ? decimalAttribute.Scale : (int?)null,
                    BelongToModelName = belongToAttribute != null ? GetModelDefinition(belongToAttribute.BelongToTableType).ModelName : null,
                };

                if (propertyInfo.FirstAttribute<IgnoreAttribute>() != null)
                    modelDef.IgnoredFieldDefinitions.Add(fieldDefinition);
                else
                    modelDef.FieldDefinitions.Add(fieldDefinition);
            }

            modelDef.SqlSelectAllFromTable = "SELECT {0} FROM {1} ".Fmt(OrmLiteConfig.DialectProvider.GetColumnNames(modelDef),
                                                                        OrmLiteConfig.DialectProvider.GetQuotedTableName(
                                                                            modelDef));
            Dictionary<Type, ModelDefinition> snapshot, newCache;
            do
            {
                snapshot = typeModelDefinitionMap;
                newCache = new Dictionary<Type, ModelDefinition>(typeModelDefinitionMap);
                newCache[modelType] = modelDef;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref typeModelDefinitionMap, newCache, snapshot), snapshot));

            return modelDef;
        }

    }
    
}
