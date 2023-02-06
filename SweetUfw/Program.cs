// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Reflection;
using SweetUfw;
using UpdateUFWFirewall;

var appVersion = Assembly.GetExecutingAssembly().GetName().Version;

Console.WriteLine("****************");
Console.WriteLine("*** sweetUFW ***");
Console.WriteLine($"Version: {appVersion}");
Console.WriteLine("****************");

var config = UfwConfig.Get();

foreach (var configItem in config)
{
    Console.WriteLine($"Checking configuration for hostname {configItem.Key}");
    var ipAdresses = UfwManager.GetIpsForHostname(configItem.Key);
    
    foreach (var port in configItem.Value)
    {
        Console.WriteLine($"Checking configuration for {configItem.Key}:{port}");
        DeletingObsoleteRules(ipAdresses, port);
        UpdateExistingAndNewRules(ipAdresses, port);
    }
    Console.WriteLine($"Checking configuration for hostname {configItem.Key} done");
}

static void DeletingObsoleteRules(List<IPAddress> ipAdresses, int port)
{
    List<string> obsoleteIpsAdresses = UfwManager.GetAllIpAdressesOfObsoleteRules(ipAdresses.Select(ip => ip.ToString()).ToList(), port);
    foreach (string ipAddress in obsoleteIpsAdresses)
    {
        Console.WriteLine($"Deleting UFW rule for IP address {ipAddress} and port {port}.");
        UfwManager.DeleteAllowTcpFromIp(ipAddress, port);
    }
}

static void UpdateExistingAndNewRules(List<IPAddress> ipAdresses, int port)
{
    foreach (IPAddress ipAddress in ipAdresses)
    {
        if (UfwManager.CheckIfUfwRuleExists(ipAddress.ToString(), port))
        {
            Console.WriteLine($"UFW rule for IP address {ipAddress} and port {port} already exists.");
        }
        else
        {
            Console.WriteLine($"Creating UFW rule for IP address {ipAddress} and port {port}.");
            UfwManager.AllowTcpFromIp(ipAddress.ToString(), port);
        }
    }
}