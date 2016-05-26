using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace MyMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpGet("/badge")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Badge()
        {
            var format = System.IO.File.ReadAllText("wwwroot/asset/badge.svg");
            var version = GetCliVersion();
            var length = version.Length;
            var svgLength = 7 * length + 140;
            var content = string.Format(format, version, svgLength, svgLength - 136);
            this.Response.Headers[HeaderNames.LastModified] = DateTime.UtcNow.ToString("R");
            return new ContentResult { Content = content, ContentType = "image/svg+xml; charset=utf-8; api-version=2.2" };
        }

        private string GetCliVersion()
        {
            var stdout = string.Empty;
            RunProcessAndWaitForExit("dotnet", "--version", TimeSpan.FromSeconds(30), out stdout);
            return stdout.Trim();
        }

        // source from dotnet/cli. https://github.com/dotnet/cli/blob/rel/1.0.0/test/Microsoft.DotNet.Tools.Tests.Utilities/ProcessExtensions.cs#L89-112
        private static int RunProcessAndWaitForExit(string fileName, string arguments, TimeSpan timeout, out string stdout)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(startInfo);

            stdout = null;
            if (process.WaitForExit((int)timeout.TotalMilliseconds))
            {
                stdout = process.StandardOutput.ReadToEnd();
            }
            else
            {
                process.Kill();
            }

            return process.ExitCode;
        }
    }
}
