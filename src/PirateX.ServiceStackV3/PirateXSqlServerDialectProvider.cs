using System;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace PirateX.ServiceStackV3
{
    public class PirateXSqlServerDialectProvider : SqlServerOrmLiteDialectProvider
    {
        public static PirateXSqlServerDialectProvider Instance = new PirateXSqlServerDialectProvider();

        public PirateXSqlServerDialectProvider()
        {
            StringConverter.UseUnicode = true;
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
                    if (fieldType == typeof (bool))
                    {
                        defaultValue = "0";
                    }
                    else if (fieldType == typeof (string))
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
                else
                {
                    customFieldDefinition = StringConverter.GetColumnDefinition(fieldLength);
                }
            }

            var fieldDefinition = base.GetColumnDefinition(fieldName, fieldType, isPrimaryKey, autoIncrement, isNullable, isRowVersion, fieldLength, scale, defaultValue, customFieldDefinition);

            if (fieldLength > StringConverter.StringLength)//超过 8000的都设置成 VARCHAR(MAX)
            {
                var orig = StringConverter.ColumnDefinition;

                fieldDefinition = fieldDefinition.Replace(orig,StringConverter.MaxColumnDefinition);
            }
            return fieldDefinition; 
        }

        public override string GetQuotedValue(object value, Type fieldType)
        {
            if (value == null) return "NULL";

            IOrmLiteConverter converter = null;
            try
            {
                var isEnum = fieldType.IsEnum || value.GetType().IsEnum;
                if (isEnum)
                    return EnumConverter.ToQuotedString(fieldType, value);

                if (Converters.TryGetValue(fieldType, out converter))
                    return converter.ToQuotedString(fieldType, value);

                if (fieldType.IsRefType())
                    return ReferenceTypeConverter.ToQuotedString(fieldType, value);

                if (fieldType.IsValueType())
                    return ValueTypeConverter.ToQuotedString(fieldType, value);
            }
            catch (Exception ex)
            {
                Log.Error("Error in {0}.ToQuotedString() value '{0}' and Type '{1}'"
                    .Fmt(converter.GetType().Name, value != null ? value.GetType().Name : "undefined", fieldType.Name), ex);
                throw;
            }

            return ShouldQuoteValue(fieldType)
                    ? GetQuotedValue(value.ToString())
                    : value.ToString();
        }
    }
}
