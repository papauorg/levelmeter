using CommandLine;

using Microsoft.Extensions.Configuration;


var result = await Parser.Default.ParseArguments<NewCylincricScaleOptions>(args)
.WithParsedAsync<NewCylincricScaleOptions>(async o =>
{
    o = ApplyAdditionalConfig<NewCylincricScaleOptions>(o.ConfigFile, o);
    o.Validate();
    var command = new NewCylindricScaleCommand(o);
    await command.InvokeAsync(CancellationToken.None);
});


static T ApplyAdditionalConfig<T>(string configPath, object options)
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Path.Combine(AppContext.BaseDirectory));
    
    if (!string.IsNullOrWhiteSpace(configPath))
        builder.AddJsonFile(configPath, optional: false);

    builder.AddEnvironmentVariables();

    var config = builder.Build();
    config.GetSection("scale-config").Bind(options); // overwrite defaults

    return (T)options;
}
