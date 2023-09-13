

using System.Data;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SemanticKernel.Data.NlpSql.Library;
using SemanticKernel.Data.NlpSql.Library.Schema;

namespace SemanticKernel.Data.NlpSql;

public class NlpSqlWeb
{
    private const ConsoleColor FocusColor = ConsoleColor.Yellow;
    private const ConsoleColor QueryColor = ConsoleColor.Green;
    private const ConsoleColor SystemColor = ConsoleColor.Cyan;

    private readonly IKernel _kernel;
    private readonly SqlConnectionProvider _sqlProvider;
    private readonly SqlQueryGenerator _queryGenerator;
    //private readonly ILogger<NlpSqlWeb> _logger;
    public SchemaDefinitions SchemaNames { get; set; }
    public NlpSqlWeb(
        IKernel kernel,
        SqlConnectionProvider sqlProvider,
        string[] SchemaNames,
        string ConfigPath="",
        double minRelevance = 0.6
        )
    {
        this.SchemaNames = new();
        this.SchemaNames.SchemaNames.AddRange(SchemaNames);
        if (SchemaNames == null || SchemaNames.Length <= 0)
        {
            throw new Exception("please provide schema names.");
        }
        this._kernel = kernel;
        this._sqlProvider = sqlProvider;
        //this._logger = logger;
        if (!string.IsNullOrEmpty(ConfigPath))
        {
            if (!Directory.Exists(ConfigPath))
            {
                throw new DirectoryNotFoundException("please specify correct config path.");
            }
            Repo.RootConfigFolder = ConfigPath;
        }
        this._queryGenerator = new SqlQueryGenerator(this._kernel, Repo.RootConfigFolder, minRelevance);
        Setup();
    }

    public async void Setup()
    {
        var schemaNames = this.SchemaNames.GetNames();
        await SchemaProvider.InitializeAsync(
            this._kernel,
            schemaNames.Select(s => Path.Combine(Repo.RootConfigFolder, "schema", $"{s}.json"))).ConfigureAwait(false);
    }

    public async Task<DataTable> GenerateQuery(string objective)
    {

        if (string.IsNullOrWhiteSpace(objective))
        {
            return default;
        }

        var context = this._kernel.CreateNewContext();
        var query = await this._queryGenerator.SolveObjectiveAsync(
                objective,
                context);

        context.Variables.TryGetValue(SqlQueryGenerator.ContextParamSchemaId, out var schemaId);

        var reader = await ProcessQueryAsync(schemaId, query);
        return reader;
        // Display query result and (optionally) execute.
    }

    public async Task<string> GetTable(DataTable table)
    {
        var tableHtml = table != null ? GetTableHtml(table) : "<p>Unable to translate request into a query.</p>";
        return tableHtml;
    }

    void WriteLine(ConsoleColor col, string Message)
    {
        Console.ForegroundColor = col;
        Console.WriteLine(Message);
    }
    async Task<DataTable> ProcessQueryAsync(string? schema, string? query)
    {
        if (string.IsNullOrWhiteSpace(schema) || string.IsNullOrWhiteSpace(query))
        {
            this.WriteLine(SystemColor, $"Unable to translate request into a query.{Environment.NewLine}");
            return default;
        }

        this.WriteLine(SystemColor, $"{Environment.NewLine}SCHEMA:");
        this.WriteLine(QueryColor, schema);
        this.WriteLine(SystemColor, $"{Environment.NewLine}QUERY:");
        this.WriteLine(QueryColor, query);


        Console.WriteLine("Executing...");

        var reader = await ProcessDataAsync(
            schema,
            query
            );
       
        return reader;
       
    }

    // Execute query and display the resulting data-set.
    async Task<DataTable> ProcessDataAsync(string schema, string query)
    {
        try
        {
            using var connection = await this._sqlProvider.ConnectAsync(schema);
            using var command = connection.CreateCommand();

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            command.CommandText = query;
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

            using var reader = await command.ExecuteReaderAsync();
            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
        {
           
            this.WriteLine(FocusColor, exception.Message);
        }
        return default;
    }

   
    public string GetTableHtml(DataTable dt)
    {
        var sb = new StringBuilder();
        
        sb.Append("<table class=\"table table-bordered table-hovered\">");
        
        sb.Append("<thead><tr>");
        foreach(DataColumn col in dt.Columns)
        {
            sb.Append($"<th scope=\"col\">{col.ColumnName}</th>");
        }
        sb.Append("</tr></thead>");

        sb.Append("<tbody>");
        int count = 0;
       foreach(DataRow dr in dt.Rows)
        {
            sb.Append("<tr>");
          
            foreach (DataColumn col in dt.Columns)
            {
                sb.Append($"<td>{dr[col]}</td>");
            }
            sb.Append("</tr>");
            count++;
        }
        sb.Append("</tbody>");
        sb.Append("</table>");
        return sb.ToString();
    }


}
