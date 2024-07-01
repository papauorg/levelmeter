using CommandLine;

using Microsoft.Extensions.Configuration;



var result = Parser.Default.ParseArguments<NewCylincricScaleOptions>(args)
    .WithParsedAsync<NewCylincricScaleOptions>(async o =>
    {
        o = o with { GraduationMarkSettings = GetGraduationMarkSettings(o.GraduationMarkSettings, o.ConfigFile, o) };
        var command = new NewCylindricScaleCommand(o);
        await command.InvokeAsync(CancellationToken.None);
    });


static GraduationMarkSettings[] GetGraduationMarkSettings(GraduationMarkSettings[] settings, string configPath, object options)
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Path.Combine(AppContext.BaseDirectory));
    
    if (!string.IsNullOrWhiteSpace(configPath))
        builder.AddJsonFile(configPath, optional: false);

    builder.AddEnvironmentVariables();

    var config = builder.Build();
    config.GetSection("levelmeter").Bind(options); // overwrite defaults

    return settings;
}
