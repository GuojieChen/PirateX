using System;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;

namespace PirateX.ServiceStackV3
{
    public class PirateXMySqlDialectProvider : MySqlDialectProvider
    {
        public static PirateXMySqlDialectProvider Instance = new PirateXMySqlDialectProvider();

        protected bool _ensureUtc;
        public void EnsureUtc(bool shouldEnsureUtc)
        {
            StringConverter.UseUnicode = true;
            _ensureUtc = shouldEnsureUtc;
            //RegisterConverter<string>(new GameStringConvert());
        }

        //public override string GetColumnDefinition(string fieldName, Type fieldType, bool isPrimaryKey, bool autoIncrement, bool isNullable,
        //    bool isRowVersion, int? fieldLength, int? scale, string defaultValue, string customFieldDefinition)
        //{
        //     var definition = base.GetColumnDefinition(fieldName, fieldType, isPrimaryKey, autoIncrement, isNullable, isRowVersion, fieldLength, scale, defaultValue, customFieldDefinition);

        //    if (fieldLength >= StringConverter.StringLength)//超过 8000的都设置成 VARCHAR(MAX)
        //    {
        //        var orig = StringConverter.ColumnDefinition;

        //        definition = definition.Replace($"{StringConverter.StringLength}", "20000");
        //    }

        //    return definition;
        //}

        //public override string ToCreateTableStatement(Type tableType)
        //{
        //    StringBuilder stringBuilder1 = new StringBuilder();
        //    StringBuilder stringBuilder2 = new StringBuilder();
        //    ModelDefinition model1 = OrmLiteDialectProviderBase<MySqlDialectProvider>.GetModel(tableType);
        //    foreach (FieldDefinition fieldDef in model1.FieldDefinitions)
        //    {
        //        if (stringBuilder1.Length != 0)
        //            stringBuilder1.Append(", \n  ");
        //        stringBuilder1.Append(this.GetColumnDefinition2(fieldDef));
        //        if (fieldDef.ForeignKey != null)
        //        {
        //            ModelDefinition model2 = OrmLiteDialectProviderBase<MySqlDialectProvider>.GetModel(fieldDef.ForeignKey.ReferenceType);
        //            stringBuilder2.AppendFormat(", \n\n  CONSTRAINT {0} FOREIGN KEY ({1}) REFERENCES {2} ({3})", (object)this.GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(model1, model2, this.NamingStrategy, fieldDef)), (object)this.GetQuotedColumnName(fieldDef.FieldName), (object)this.GetQuotedTableName(model2), (object)this.GetQuotedColumnName(model2.PrimaryKey.FieldName));
        //            if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnDelete))
        //                stringBuilder2.AppendFormat(" ON DELETE {0}", (object)fieldDef.ForeignKey.OnDelete);
        //            if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnUpdate))
        //                stringBuilder2.AppendFormat(" ON UPDATE {0}", (object)fieldDef.ForeignKey.OnUpdate);
        //        }
        //    }
        //    return new StringBuilder(string.Format("CREATE TABLE {0} \n(\n  {1}{2} \n); \n", (object)this.GetQuotedTableName(model1), (object)stringBuilder1, (object)stringBuilder2)).ToString();
        //}


        //public string GetColumnDefinition2(FieldDefinition fieldDef)
        //{
        //    if (PlatformExtensions.FirstAttribute<TextAttribute>(fieldDef.PropertyInfo) != null)
        //    {
        //        StringBuilder stringBuilder = new StringBuilder();
        //        stringBuilder.AppendFormat("{0} {1}", (object)this.GetQuotedColumnName(fieldDef.FieldName), (object)"TEXT");
        //        stringBuilder.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
        //        return stringBuilder.ToString();
        //    }
        //    string columnDefinition = this.GetColumnDefinition(fieldDef.FieldName, fieldDef.ColumnType, fieldDef.IsPrimaryKey, fieldDef.AutoIncrement, fieldDef.IsNullable, fieldDef.IsRowVersion, fieldDef.FieldLength, fieldDef.Scale, this.GetDefaultValue(fieldDef), fieldDef.CustomFieldDefinition);
        //    if (fieldDef.IsRowVersion)
        //        return columnDefinition + " DEFAULT 1";
        //    return columnDefinition;
        //}

        public override string GetDefaultValue(FieldDefinition fieldDef)
        {
            var defaultValue = base.GetDefaultValue(fieldDef);
            if (fieldDef.IsPrimaryKey)
                return defaultValue;

            var fieldType = fieldDef.FieldType;

            if (fieldType == typeof(DateTime))
            {
                defaultValue = defaultValue.IsNullOrEmpty() ? $"'{DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}'" : $"'{defaultValue}'";
            }
            else if (fieldType == typeof(DateTimeOffset))
            {
                defaultValue = defaultValue.IsNullOrEmpty() ? "'00:00:00'" : $"'{defaultValue}'";
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

            return defaultValue;
        }

    }
}
