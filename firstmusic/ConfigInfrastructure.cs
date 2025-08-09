using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace firstmusic
{
    public class ConfigInfrastructure
    {
        public IConfiguration Configuration { get; set; }

        public ConfigInfrastructure(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string BO_FILE_ROOT()
        {
            return Configuration.GetValue<string>("FileRoot");
        }

        public static string LOG_STANDARD = @"\Logs\Standard";
    }

}
