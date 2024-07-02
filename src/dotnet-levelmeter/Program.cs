using CommandLine;

using Microsoft.Extensions.Configuration;

using Papau.Levelmeter.Commands;


var result = await Parser.Default.ParseArguments<NewCylincricScaleOptions>(args)
.WithParsedAsync<NewCylincricScaleOptions>(async o =>
{
    o = ApplyAdditionalConfig<NewCylincricScaleOptions>(o.ConfigFile, o);
    
    if (!string.IsNullOrWhiteSpace(o.Output) && !string.IsNullOrWhiteSpace(o.ConfigFile))
        o = o with { Output = string.Format(o.Output, Path.GetFileNameWithoutExtension(o.ConfigFile)) };

    o.Validate();
    var command = new NewCylindricScaleCommand(o);
    await command.InvokeAsync(CancellationToken.None);
});


static T ApplyAdditionalConfig<T>(string configPath, T options)
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Path.Combine(AppContext.BaseDirectory));
    
    if (!string.IsNullOrWhiteSpace(configPath))
    {
        builder.AddJsonFile(configPath, optional: false);
    }

    builder.AddEnvironmentVariables();

    var config = builder.Build();
    config.GetSection("scale-config").Bind(options); // overwrite defaults

    return options;
}
