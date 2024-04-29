using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

var baseSchemaFilePath = "../../third-party/cti-stix2-json-schemas/schemas/sdos/vulnerability.json";

var schema = JsonSchema.FromFileAsync(baseSchemaFilePath).Result;

var generatorSettings = new CSharpGeneratorSettings
{
    ClassStyle = CSharpClassStyle.Poco,
    Namespace = "Vulnerabilities.Schema",
    JsonLibrary = CSharpJsonLibrary.SystemTextJson
};

var generator = new CSharpGenerator(schema, generatorSettings);

var code = generator.GenerateFile();

File.WriteAllText("./Pocos.cs", code);