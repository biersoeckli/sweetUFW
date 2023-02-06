using System.Net;
using System.Text.RegularExpressions;

namespace SweetUfw
{
    class UfwManager
    {
        public static List<IPAddress> GetIpsForHostname(string hostname)
        {
            IPHostEntry host = Dns.GetHostEntry(hostname);
            return host.AddressList.ToList();
        }

        public static bool CheckIfUfwRuleExists(string ipAddress, int port)
        {
            string firewallCommand = $"ufw status verbose | grep ALLOW | grep {ipAddress} | grep {port}";
            var output = RunCommand(firewallCommand);
            return output.Length != 0;
        }

        public static void DeleteAllowTcpFromIp(string ipAddress, int port)
        {
            string firewallCommand = "sudo ufw delete allow from " + ipAddress + " proto tcp to any port " + port;
            RunCommand(firewallCommand);
        }

        public static void AllowTcpFromIp(string ipAddress, int port)
        {
            string firewallCommand = "sudo ufw allow from " + ipAddress + " proto tcp to any port " + port;
            RunCommand(firewallCommand);
        }

        public static List<string> GetAllIpAdressesOfObsoleteRules(List<string> allowedIpAdresses, int port)
        {
            string firewallCommand = $"ufw status | grep ALLOW | grep {port} | grep -E -v '{string.Join("|", allowedIpAdresses)}'";
            var output = RunCommand(firewallCommand);

            // Regular expression pattern for IPv4 addresses
            string ipv4Pattern = @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";

            // Regular expression pattern for IPv6 addresses
            string ipv6Pattern = @"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:))";

            // Find all IPv4 addresses
            MatchCollection ipv4Matches = Regex.Matches(output, ipv4Pattern);
            // Find all IPv6 addresses
            MatchCollection ipv6Matches = Regex.Matches(output, ipv6Pattern);

            var ipList = ipv4Matches.Select(ipMatch => ipMatch.Value).ToList();
            ipList.AddRange(ipv6Matches.Select(ipv6Match => ipv6Match.Value));
            return ipList;
        }

        public static void PrintUfwStatus()
        {
            RunCommand("sudo ufw status");
        }

        private static string RunCommand(string firewallCommand)
        {
            Console.WriteLine($"Running Firewall Command: {firewallCommand}");
            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo.FileName = "/bin/bash";
                process.StartInfo.Arguments = "-c \"" + firewallCommand + "\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                Console.WriteLine("Output: " + output);
                return output;
            }
        }
    }
}

