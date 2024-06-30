using CommandLine;

var result = Parser.Default.ParseArguments<NewCylincricScaleOptions>(args)
    .WithParsedAsync<NewCylincricScaleOptions>(async o => await new NewCylindricScaleCommand(o).InvokeAsync(CancellationToken.None));
