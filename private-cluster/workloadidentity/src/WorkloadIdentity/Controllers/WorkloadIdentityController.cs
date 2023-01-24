using Microsoft.AspNetCore.Mvc;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WorkloadIdentity.Models;
using WorkloadIdentity.Data;

namespace WorkloadIdentity.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WorkloadIdentityController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly ILogger<WorkloadIdentityController> _logger;
        private readonly CustomerContext _context;

        public WorkloadIdentityController(ILogger<WorkloadIdentityController> logger, IConfiguration configuration, CustomerContext context)
        {
            _logger = logger;
            _config = configuration;
            _context = context; 
        }


        [HttpGet]
        public string GetSecret()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var keyVaultUrl = $"https://{_config["keyvaultname"]}.vault.azure.net/";
            var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = _config["AZURE_CLIENT_ID"] }));
            var secret = secretClient.GetSecret(_config["secretname"]);
            Console.WriteLine($"The secret is '{secret.Value.Value}'.");
            stopwatch.Stop();
            Console.WriteLine($"Method completed in { stopwatch.ElapsedMilliseconds} ms.");
            return "Secret is: " + secret.Value.Value;

        }

        [HttpGet]
        public async Task<List<Customer>> GetData()
        {
            var customers = await _context.Customers.ToListAsync();
            return customers;
        }
    }
}