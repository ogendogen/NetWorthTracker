using System.ComponentModel;
using System.Diagnostics;

namespace NetWorthTracker.Api.Configuration;

internal static class SopsConfiguration
{
    public static async Task AddForCurrentEnvironmentAsync(
        ConfigurationManager configuration,
        IHostEnvironment environment,
        string[] args)
    {
        var encryptedFilePath = Path.Combine(
            environment.ContentRootPath,
            $"appsettings.secrets.{environment.EnvironmentName}.enc.json");

        if (!File.Exists(encryptedFilePath))
        {
            throw new FileNotFoundException(
                $"The encrypted SOPS configuration file was not found: '{encryptedFilePath}'.",
                encryptedFilePath);
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "sops",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            },
        };

        process.StartInfo.ArgumentList.Add("decrypt");
        process.StartInfo.ArgumentList.Add("--output-type");
        process.StartInfo.ArgumentList.Add("json");
        process.StartInfo.ArgumentList.Add(encryptedFilePath);

        try
        {
            process.Start();
        }
        catch (Win32Exception exception)
        {
            throw new InvalidOperationException(
                "SOPS could not be started. Install it and ensure the 'sops' executable is on PATH.",
                exception);
        }

        using var decryptedConfiguration = new MemoryStream();
        var standardOutputTask = process.StandardOutput.BaseStream.CopyToAsync(decryptedConfiguration);
        var standardErrorTask = process.StandardError.ReadToEndAsync();

        await Task.WhenAll(
            standardOutputTask,
            standardErrorTask,
            process.WaitForExitAsync());

        if (process.ExitCode != 0)
        {
            var standardError = await standardErrorTask;
            throw new InvalidOperationException(
                $"SOPS failed to decrypt '{encryptedFilePath}' (exit code {process.ExitCode}): {standardError}");
        }

        decryptedConfiguration.Position = 0;
        configuration.AddJsonStream(decryptedConfiguration);
        configuration.AddEnvironmentVariables();
        configuration.AddCommandLine(args);
    }
}