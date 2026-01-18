using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

public class RemoveStringPlaceholdersSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null) return;

        foreach (var prop in schema.Properties)
        {
            // Remove default "string" example value
            if (prop.Value.Type == "string" && prop.Value.Example?.ToString() == "string")
            {
                prop.Value.Example = null;
            }

            // Optional: set to null explicitly or a custom example
            // prop.Value.Example = new OpenApiString("example@example.com");
        }
    }
}
