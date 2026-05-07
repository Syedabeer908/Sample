namespace WebApplication1.Infrastructure.Cli
{
    public static class CliRunner
    {
        public static async Task<bool> ExecuteAsync(
            WebApplication app,
            IConfiguration config,
            string[] args)
        {
            if (args.Length == 0)
                return false;

            switch (args[0].ToLower())
            {
                case "bootstrap":
                    await CliCommands.MigrateAsync(app);
                    await CliCommands.SeedAsync(app);
                    await CliCommands.SeedHangfireAsync(config);
                    break;

                case "migrate":
                    await CliCommands.MigrateAsync(app);
                    break;

                case "seed":
                    await CliCommands.SeedAsync(app);
                    break;

                case "hangfire":
                    await CliCommands.SeedHangfireAsync(config);
                    break;
                
            }

            return true;
        }
    }
}