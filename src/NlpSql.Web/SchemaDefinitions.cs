

namespace SemanticKernel.Data.NlpSql;

using System.Collections.Generic;

/// <summary>
/// Defines the schemas initialized by the console.
/// </summary>
public class SchemaDefinitions
{
    public List<string> SchemaNames { get; set; } = new();
    /// <summary>
    /// Enumerates the names of the schemas to be registered with the console.
    /// </summary>
    /// <remarks>
    /// After testing with the sample data-sources, try one of your own!
    /// </remarks>
    public IEnumerable<string> GetNames()
    {
        foreach(var item in SchemaNames)
        {
            yield return item;
        }
    }
}
