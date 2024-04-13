using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;

using SweetMeSoft.Base.Attributes;
using SweetMeSoft.Base.GCP;
using SweetMeSoft.Tools;

using System.Linq.Expressions;

namespace SweetMeSoft.GCP;

public class BigQueryRepo
{
    public static string CredentialsFileName;
    public static string CredentialsJson;
    public static string CredentialsToken;
    public static string ProjectId;

    private readonly BigQueryClient client;

    private static BigQueryRepo instance;

    public static BigQueryRepo Instance => instance ??= new BigQueryRepo();

    public BigQueryRepo()
    {
        GoogleCredential gc = null;
        if (string.IsNullOrEmpty(CredentialsFileName))
        {
            gc = GoogleCredential.FromFile(CredentialsFileName);
        }

        if (string.IsNullOrEmpty(CredentialsJson))
        {
            gc = GoogleCredential.FromJson(CredentialsJson);
        }

        if (string.IsNullOrEmpty(CredentialsToken))
        {
            gc = GoogleCredential.FromAccessToken(CredentialsToken);
        }

        client = BigQueryClient.Create(ProjectId, gc);
    }

    public IEnumerable<T> ConvertToObject<T>(List<BigQueryRow> googleList) where T : new()
    {
        var list = new List<T>();
        foreach (var row in googleList)
        {
            try
            {
                var properties = typeof(T).GetProperties();
                var d = new T();
                foreach (var property in properties)
                {
                    if (row.Schema.Fields.Where(model => model.Name == property.Name).Count() > 0)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(d, row[property.Name]?.ToString(), null);
                        }

                        if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                        {
                            property.SetValue(d, Guid.Parse(row[property.Name].ToString()), null);
                        }

                        if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                        {
                            var value = Converters.StringToInt(row[property.Name]?.ToString());
                            property.SetValue(d, value, null);
                        }

                        if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
                        {
                            var value = Converters.StringToDouble(row[property.Name]?.ToString());
                            property.SetValue(d, value, null);
                        }

                        if (property.PropertyType == typeof(float) || property.PropertyType == typeof(float?))
                        {
                            var value = Converters.StringToFloat(row[property.Name]?.ToString());
                            property.SetValue(d, value, null);
                        }

                        if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                        {
                            var value = Converters.StringToDecimal(row[property.Name]?.ToString());
                            property.SetValue(d, value, null);
                        }

                        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                        {
                            var cell = row[property.Name]?.ToString();
                            DateTime? value = cell != null ? DateTime.Parse(cell) : null;
                            property.SetValue(d, value, null);
                        }
                    }
                }

                list.Add(d);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        return list;
    }

    public async Task<IEnumerable<T>> ExecuteQuery<T>(string query) where T : new()
    {
        var results = await client.ExecuteQueryAsync(query, null);
        return ConvertToObject<T>(results.ToList());
    }

    public async Task<IEnumerable<T>> GetAll<T>() where T : new()
    {
        var table = await GetOrCreateTable<T>();
        return Instance.ConvertToObject<T>(await table.ListRowsAsync().ToListAsync());
    }

    public async Task InsertItem<T>(T obj) where T : new()
    {
        await InsertList(new List<T>()
        {
            obj
        });
    }

    public async Task InsertList<T>(IEnumerable<T> list) where T : new()
    {
        var step = 1000;
        var rows = new List<BigQueryInsertRow>();
        var table = await GetOrCreateTable<T>();

        foreach (var document in list)
        {
            rows.Add(ObjectToRow(document));
        }

        var index = 0;
        List<BigQueryInsertRow> toInsert;
        while (index < rows.Count)
        {
            if (rows.Count - index < step)
            {
                toInsert = rows.GetRange(index, rows.Count - index);
            }
            else
            {
                toInsert = rows.GetRange(index, step);
            }

            table.InsertRows(toInsert);
            index += step;
        }
    }

    public async Task<IEnumerable<T>> GetByField<T>(Expression<Func<T, bool>> where) where T : new()
    {
        var attr = GetTableAttr<T>();
        var translator = new QueryTranslator();
        string whereExp = translator.Translate(where);
        return await ExecuteQuery<T>("SELECT * FROM `intl-rosa-sandbox." + attr.Dataset + "." + attr.Name + "` WHERE " + whereExp + ";");
    }

    public async Task<T> Single<T>(Expression<Func<T, bool>> predicate) where T : new()
    {
        var results = (await GetByField(predicate)).ToList();
        return results.Count switch
        {
            0 => default,
            1 => results[0],
            _ => throw new Exception("Multiple elements found."),
        };
    }

    public async Task<long> Update<T>(T obj, Expression<Func<T, bool>> where) where T : new()
    {
        var attr = GetTableAttr<T>();
        var translator = new QueryTranslator();
        string whereExp = translator.Translate(where);
        string setExt = GetSetExpression(obj);
        var results = await client.ExecuteQueryAsync("UPDATE `intl-rosa-sandbox." + attr.Dataset + "." + attr.Name + "` SET " + setExt + " WHERE " + whereExp + ";", null);
        return results.NumDmlAffectedRows ?? 0;
    }

    public async Task<long> Update<T, TKey>(T original, T edited, Expression<Func<T, TKey>> key) where T : new()
    {
        var attr = GetTableAttr<T>();
        var translator = new QueryTranslator();
        string whereExp = translator.Translate(key);
        var propInfo = edited.GetType().GetProperty(whereExp);
        var itemValue = propInfo.GetValue(edited, null);
        whereExp += " = '" + itemValue + "'";
        string setExt = GetSetExpression(edited, original);
        var results = await client.ExecuteQueryAsync("UPDATE `intl-rosa-sandbox." + attr.Dataset + "." + attr.Name + "` SET " + setExt + " WHERE " + whereExp + ";", null);
        return results.NumDmlAffectedRows ?? 0;
    }

    public async Task<long> Delete<T>(Expression<Func<T, bool>> where) where T : new()
    {
        var attr = GetTableAttr<T>();
        var translator = new QueryTranslator();
        string whereExp = translator.Translate(where);
        var results = await client.ExecuteQueryAsync("DELETE `intl-rosa-sandbox." + attr.Dataset + "." + attr.Name + "` WHERE " + whereExp + ";", null);
        return results.NumDmlAffectedRows ?? 0;
    }

    private async Task<BigQueryTable> GetOrCreateTable<T>()
    {
        try
        {
            var attr = GetTableAttr<T>();
            var dataset = await client.GetOrCreateDatasetAsync(attr.Dataset);
            return await dataset.GetOrCreateTableAsync(attr.Name ?? typeof(T).Name, CreateSchema<T>().Build());
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    private BigQueryInsertRow ObjectToRow<T>(T obj) where T : new()
    {
        var properties = typeof(T).GetProperties();
        var row = new BigQueryInsertRow();
        foreach (var prop in properties)
        {
            var propType = prop.PropertyType;
            if (propType == typeof(string) || propType == typeof(Guid) || propType == typeof(Guid?))
            {
                row.Add(prop.Name, prop.GetValue(obj)?.ToString());
            }

            if (propType == typeof(int) || propType == typeof(int?)
                || propType == typeof(bool) || propType == typeof(bool?)
                || propType == typeof(double) || propType == typeof(double?)
                || propType == typeof(float) || propType == typeof(float?))
            {
                row.Add(prop.Name, prop.GetValue(obj));
            }

            if (propType == typeof(decimal) || propType == typeof(decimal?))
            {
                row.Add(prop.Name, Converters.DecimalToString((decimal)prop.GetValue(obj)));
            }

            if (propType == typeof(DateTime) || propType == typeof(DateTime?))
            {
                row.Add(prop.Name, ((DateTime)prop.GetValue(obj)).ToUniversalTime());
            }
        }

        return row;
    }

    private BigQueryTableAttribute GetTableAttr<T>()
    {
        if (typeof(T).GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "BigQueryTableAttribute") is not BigQueryTableAttribute attr)
        {
            throw new Exception("Table " + typeof(T).FullName + " has not BigQueryTableAttribute. Please add it.");
        }

        if (string.IsNullOrEmpty(attr.Dataset))
        {
            throw new Exception("Table " + typeof(T).FullName + " has not Dataset defined in BigQueryTableAttribute. Please add it.");
        }

        if (string.IsNullOrEmpty(attr.Name))
        {
            attr.Name = typeof(T).Name;
        }

        return attr;
    }

    private TableSchemaBuilder CreateSchema<T>()
    {
        var schema = new TableSchemaBuilder();
        var properties = typeof(T).GetProperties().Where(x => x.GetAccessors()[0].IsFinal || !x.GetAccessors()[0].IsVirtual);
        foreach (var prop in properties)
        {
            if (prop.GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "IgnoreColumnAttribute") is not IgnoreColumnAttribute attr)
            {
                schema.Add(new TableFieldSchema { Name = prop.Name, Type = MapTypeToBigQuery(prop.PropertyType) });
            }
        }

        return schema;
    }

    private static string MapTypeToBigQuery(Type propertyType)
    {
        if (propertyType == typeof(string) || propertyType == typeof(Guid) || propertyType == typeof(Guid?))
        {
            return "STRING";
        }

        if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
        {
            return "TIMESTAMP";
        }

        if (propertyType == typeof(float) || propertyType == typeof(float?))
        {
            return "FLOAT";
        }

        if (propertyType == typeof(int) || propertyType == typeof(int?))
        {
            return "INTEGER";
        }

        if (propertyType == typeof(decimal) || propertyType == typeof(decimal?) || propertyType == typeof(double) || propertyType == typeof(double?))
        {
            return "NUMERIC";
        }

        if (propertyType == typeof(bool) || propertyType == typeof(bool?))
        {
            return "BOOL";
        }

        return "";
    }

    private string GetSetExpression<T>(T edited, T original = default)
    {
        var setExp = "";
        var properties = typeof(T).GetProperties();
        foreach (var prop in properties)
        {
            if (!string.IsNullOrEmpty(setExp) && !setExp.EndsWith(", "))
            {
                setExp += ", ";
            }

            var propType = prop.PropertyType;
            if (propType == typeof(string) || propType == typeof(Guid) || propType == typeof(Guid?))
            {
                var editedValue = prop.GetValue(edited)?.ToString().Replace("\\", "\\\\").Replace("'", "\\'");
                if (original != null)
                {
                    var originalValue = prop.GetValue(original)?.ToString().Replace("\\", "\\\\").Replace("'", "\\'");
                    if (editedValue != originalValue)
                    {
                        setExp += prop.Name + " = '" + editedValue + "'";
                    }
                }
                else
                {
                    setExp += prop.Name + " = '" + editedValue + "'";
                }
            }

            if (propType == typeof(int) || propType == typeof(int?)
                || propType == typeof(bool) || propType == typeof(bool?)
                || propType == typeof(double) || propType == typeof(double?)
                || propType == typeof(float) || propType == typeof(float?)
                || propType == typeof(decimal) || propType == typeof(decimal?))
            {
                var editedValue = prop.GetValue(edited).ToString();
                if (original != null)
                {
                    var originalValue = prop.GetValue(original).ToString();
                    if (editedValue != originalValue)
                    {
                        setExp += prop.Name + " = " + prop.GetValue(edited);
                    }
                }
                else
                {
                    setExp += prop.Name + " = " + prop.GetValue(edited);
                }
            }

            if (propType == typeof(DateTime) || propType == typeof(DateTime?))
            {
                var editedValue = ((DateTime)prop.GetValue(edited)).ToString("yyyy-MM-ddTHH:mm:ss.fff");
                if (original != null)
                {
                    var originalValue = ((DateTime)prop.GetValue(original)).ToString("yyyy-MM-ddTHH:mm:ss.fff");
                    if (editedValue != originalValue)
                    {
                        setExp += prop.Name + " = '" + editedValue + "'";
                    }
                }
                else
                {
                    setExp += prop.Name + " = '" + editedValue + "'";
                }
            }
        }

        return setExp.Trim().Trim(',');
    }
}