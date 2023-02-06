# sweetUFW

A tool to sync (dynamic) IPs by their hostnames with ufw firewall ports.

## how it works
1. Download the artifact from the <a href="https://github.com/biersoeckli/sweetUFW/actions">github actions page</a> or checkout the code and run locally.

2. Create a config file with the following content (/my/path/to/config/sweetUFW.conf.json):
```json
{
    "22": ["biersoeckli.ch"]
}
```
  - The property name of the object is the port where incoming TCP connections are allowed (here 22).
  - Value ["biersoeckli.ch"] => allowed hostnames for accessing the defined port (property name).

3. Start the tool with the filepath of the config (step 2) as argument

```bash
./SweetUfw /my/path/to/config/sweetUFW.conf.json
```
    
4. Create a crontab job running the command above. The tool syncs the IP adresses behind the hostnames an their allowed ports with the UFW firewall.
