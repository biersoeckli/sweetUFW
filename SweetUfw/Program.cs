using System.Net;
using System.Reflection;

namespace SweetUfw
{
    class Program
    {
        static void Main(string[] args)
        {
            var appVersion = Assembly.GetExecutingAssembly().GetName().Version;

            Console.WriteLine("****************");
            Console.WriteLine("*** sweetUFW ***");
            Console.WriteLine($"Version: {appVersion}");
            Console.WriteLine("****************");

            var config = UfwConfig.Get(args?.Length > 0 ? args[0] : null);

            foreach (var configItem in config)
            {
                var port = configItem.Key;
                var hostnames = configItem.Value;
                Console.WriteLine($"** Checking configuration for port {port}");
                var ipAdresses = hostnames.SelectMany(UfwManager.GetIpsForHostname).ToList();
                DeletingObsoleteRules(ipAdresses, port);
                UpdateExistingAndNewRules(ipAdresses, port);
                Console.WriteLine($"** Checking configuration for port {port} done");
            }
            UfwManager.PrintUfwStatus();
        }


        static void DeletingObsoleteRules(List<IPAddress> allowedIpAdresses, int port)
        {
            List<string> obsoleteIpsAdresses = UfwManager.GetAllIpAdressesOfObsoleteRules(allowedIpAdresses.Select(ip => ip.ToString()).ToList(), port);
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
    }
}
