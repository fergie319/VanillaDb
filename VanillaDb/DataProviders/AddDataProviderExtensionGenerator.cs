using System;
using System.Collections.Generic;
using System.Linq;
using VanillaDb.Models;

namespace VanillaDb.DataProviders
{
    internal class AddDataProviderExtensionGenerator : ICodeTemplate
    {
        private IEnumerable<TableModel> Tables { get; set; }

        private TableModel Table { get; set; }

        public AddDataProviderExtensionGenerator(IEnumerable<TableModel> tables)
        {
            Tables = tables;
            Table = tables.FirstOrDefault();
        }

        public string FileExtension => "cs";

        public string GenerateName()
        {
            return "DataProvidersExtensions";
        }

        public string TransformText()
        {
            var result =
$@"{GetTableUsings()}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {Table.Namespace}.DataProviders
{{
    /// <summary>Extension method for registering data providers for dependency injection.</summary>
    public static class DataProvidersExtensions
    {{
        /// <summary>Adds the data providers.</summary>
        /// <param name=""services"">The services.</param>
        /// <param name=""connectionStringName"">Name of the connection string.</param>
        /// <returns></returns>
        public static IServiceCollection AddDataProviders(this IServiceCollection services, string connectionStringName)
        {{
{GetTablesTypeRegistration()}
            return services;
        }}
    }}
}}
";

            return result;
        }

        private string GetTableUsings()
        {
            var tableUsings = new List<string>();
            foreach (var table in Tables)
            {
                tableUsings.Add($"using {table.Namespace}.DataProviders.{table.TableAlias};");
            }

            return string.Join(Environment.NewLine, tableUsings);
        }

        private string GetTablesTypeRegistration()
        {
            var typeRegistrations = new List<string>();
            foreach (var table in Tables)
            {
                typeRegistrations.Add(
$@"            services.AddScoped<I{table.TableAlias}DataProvider, {table.TableAlias}SqlDataProvider>(s =>
            {{
                var configuration = s.GetService<IConfiguration>();
                var connectionString = configuration.GetConnectionString(connectionStringName);
                var dataProvider = ActivatorUtilities.CreateInstance<{table.TableAlias}SqlDataProvider>(s, connectionString);
                return dataProvider;
            }});
");
            }

            return string.Join(Environment.NewLine, typeRegistrations);
        }
    }
}
