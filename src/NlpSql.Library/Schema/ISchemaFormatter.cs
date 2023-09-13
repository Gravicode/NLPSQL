

namespace SemanticKernel.Data.NlpSql.Library.Schema;

using System.IO;
using System.Threading.Tasks;

public interface ISchemaFormatter
{
    Task WriteAsync(TextWriter writer, SchemaDefinition schema);
}
