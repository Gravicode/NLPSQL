namespace SchemaGenerator
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            //schema 1
            var connectionString = "Server=.\\sqlexpress;Database=AdventureWorksLT2012;User Id=sa;Password=123;TrustServerCertificate=true";
            string SchemaName = "AdventureWorksLT";
            string Desc = "Product, sales, and customer data for the AdentureWorks company.";
            SqlSchemaProviderGenerator generator = new SqlSchemaProviderGenerator(connectionString, SchemaName, Desc);
            await generator.ReverseEngineerSchemaAsync();

            //schema 2
            connectionString = "Server=.\\sqlexpress;Database=DescriptionTest;User Id=sa;Password=123;TrustServerCertificate=true";
            SchemaName = "DescriptionTest";
            Desc = "Associates registered users with interest categories.";
            generator = new SqlSchemaProviderGenerator(connectionString, SchemaName, Desc);
            await generator.ReverseEngineerSchemaAsync();

            Console.WriteLine($"schema config has been generated successfully, you can check: {Repo.RootConfigFolder}");
            Console.ReadLine();
        }
    }
}