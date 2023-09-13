namespace SchemaGenerator;

using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SemanticKernel.Data.NlpSql.Library.Schema;

/// <summary>
/// Generator for utilizing <see cref="SqlSchemaProvider"/> to capture live database schema
/// definitions: <see cref="SchemaDefinition"/>.
/// </summary>
public class SqlSchemaProviderGenerator
{
    public string SchemaName { get; set; }
    public string Description { get; set; }
    public string[] TableNames { get; set; }
    public string ConfigPath { get; set; } = string.Empty;
    public string ConnectionString { get; set; }
    public SqlSchemaProviderGenerator(string ConnectionString, string SchemaName,string Desc, string ConfigPath = "", params string[] TableNames)
    {
        this.ConnectionString = ConnectionString;
        this.SchemaName = SchemaName;
        this.Description = Desc;
        this.TableNames = TableNames;
        this.ConfigPath = ConfigPath;
    }

    /// <summary>
    /// Reverse engineer live database (design-time task).
    /// </summary>
    /// <remarks>
    /// After testing with the sample data-sources, try one of your own!
    /// </remarks>
    //[Fact(Skip = SkipReason)]
    public async Task ReverseEngineerSchemaAsync()
    {
        if (!string.IsNullOrEmpty(ConfigPath))
        {
            Repo.RootConfigFolder = ConfigPath;
        }
        Repo.CheckPath();
        await this.CaptureSchemaAsync(SchemaName, Description, TableNames).ConfigureAwait(false);

    }

    private async Task CaptureSchemaAsync(string databaseKey, string? description, params string[] tableNames)
    {
        using var connection = new SqlConnection(this.ConnectionString);
        await connection.OpenAsync().ConfigureAwait(false);

        var provider = new SqlSchemaProvider(connection);

        var schema = await provider.GetSchemaAsync(description, tableNames).ConfigureAwait(false);

        await connection.CloseAsync().ConfigureAwait(false);

        // Capture YAML for inspection
        var yamlText = await schema.FormatAsync(YamlSchemaFormatter.Instance).ConfigureAwait(false);
        await this.SaveSchemaAsync("yaml", databaseKey, yamlText).ConfigureAwait(false);

        // Capture json for reserialization
        await this.SaveSchemaAsync("json", databaseKey, schema.ToJson()).ConfigureAwait(false);
    }

    private async Task SaveSchemaAsync(string extension, string databaseKey, string schemaText)
    {
        var fileName = Path.Combine(Repo.RootConfigFolder, "schema", $"{databaseKey}.{extension}");

        using var streamCompact =
            new StreamWriter(
                fileName,
                new FileStreamOptions
                {
                    Access = FileAccess.Write,
                    Mode = FileMode.Create,
                });

        await streamCompact.WriteAsync(schemaText).ConfigureAwait(false);
    }
}
