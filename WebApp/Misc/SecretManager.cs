using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;

namespace WebApp.Misc
{
    public class SecretManager
    {
        private readonly string _secret_file;

        public SecretManager(string serverPath)
        {
            _secret_file = Path.Combine(serverPath, ConfigurationManager.AppSettings["SecretFile"]);
        }

        public string GetSecret(string key)
        {
            string json = File.ReadAllText(_secret_file);
            var jobect = JsonConvert.DeserializeObject<JObject>(json);
            return jobect.Value<string>(key);
        }
    }
}