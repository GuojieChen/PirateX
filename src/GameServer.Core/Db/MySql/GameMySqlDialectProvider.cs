using System;
using System.Text;
using ServiceStack;
using ServiceStack.OrmLite.MySql;

namespace GameServer.Core.Db.MySql
{
    public class GameMySqlDialectProvider : MySqlDialectProvider
    {
        protected bool _ensureUtc;
        public void EnsureUtc(bool shouldEnsureUtc)
        {
            base.UseUnicode = true;
            _ensureUtc = shouldEnsureUtc;
        }

        public override object ConvertDbValue(object value, Type type)
        {
            if (value == null || value is DBNull) return null;

            if (type == typeof(bool))
            {
                return
                    value is bool
                        ? value
                        : (int.Parse(value.ToString()) != 0); //backward compatibility (prev version mapped bool as bit(1))
            }

            if (_ensureUtc && type == typeof(DateTime))
            {
                var result = base.ConvertDbValue(value, type);
                if (result is DateTime)
                    return DateTime.SpecifyKind((DateTime)result, DateTimeKind.Utc);
                return result;
            }

            if (type == typeof(byte[]))
                return value;

            return base.ConvertDbValue(value, type);
        }

        public override string ToCreateTableStatement(Type tableType)
        {
            var sbColumns = new StringBuilder();
            var sbConstraints = new StringBuilder();

            var modelDef = GetModel(tableType);
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (sbColumns.Length != 0) sbColumns.Append(", \n  ");

                sbColumns.Append(GetColumnDefinition(fieldDef));

                if (fieldDef.ForeignKey == null) continue;

                var refModelDef = GetModel(fieldDef.ForeignKey.ReferenceType);
                sbConstraints.AppendFormat(
                    ", \n\n  CONSTRAINT {0} FOREIGN KEY ({1}) REFERENCES {2} ({3})",
                    GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, NamingStrategy, fieldDef)),
                    GetQuotedColumnName(fieldDef.FieldName),
                    GetQuotedTableName(refModelDef),
                    GetQuotedColumnName(refModelDef.PrimaryKey.FieldName));

                if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnDelete))
                    sbConstraints.AppendFormat(" ON DELETE {0}", fieldDef.ForeignKey.OnDelete);

                if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnUpdate))
                    sbConstraints.AppendFormat(" ON UPDATE {0}", fieldDef.ForeignKey.OnUpdate);
            }
            var sql = new StringBuilder($"CREATE TABLE {GetQuotedTableName(modelDef)} \n(\n  {sbColumns}{sbConstraints} \n); \n");

            return sql.ToString();
        }
        
        public override string GetColumnDefinition(string fieldName, Type fieldType, bool isPrimaryKey, bool autoIncrement, bool isNullable,
            bool isRowVersion, int? fieldLength, int? scale, string defaultValue, string customFieldDefinition)
        {
            if (!isPrimaryKey && defaultValue.IsNullOrEmpty())
            {
                if (fieldType == typeof(DateTime))
                {
                    defaultValue = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                }
                else if (fieldType == typeof(DateTimeOffset))
                {
                    defaultValue = "00:00:00";
                }
                else if (fieldType.IsPrimitive)
                {
                    if (fieldType == typeof(bool))
                    {
                        defaultValue = "0";
                    }
                    else if (fieldType == typeof(string))
                    {
                        defaultValue = "";
                    }
                    else
                    {
                        var typeCode = fieldType.GetTypeCode();
                        switch (typeCode)
                        {
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                            case TypeCode.Byte:
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.SByte:
                            case TypeCode.UInt16:
                            case TypeCode.UInt32:
                            case TypeCode.UInt64:
                                defaultValue = "0";
                                break;
                        }
                    }
                }
            }

            var fieldDefinition = base.GetColumnDefinition(fieldName, fieldType, isPrimaryKey, autoIncrement, isNullable, isRowVersion, fieldLength, scale, defaultValue, customFieldDefinition);
            if (fieldLength > base.DefaultStringLength)
            {
                var orig = string.Format(StringLengthUnicodeColumnDefinitionFormat, DefaultStringLength);

                fieldDefinition = fieldDefinition.Replace(orig, MaxStringColumnDefinition);
            }

            return fieldDefinition;
        }
    }
}
