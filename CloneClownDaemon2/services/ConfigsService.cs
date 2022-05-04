using CloneClownAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CloneClownDaemon2
{
    public class ConfigsService
    {
        private HttpClient client;
        public ConfigsService()
        {
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri("http://localhost:35125");
        }
        public async Task<List<Configs>> FindAll()
        {
            string result = await this.client.GetStringAsync("/api/configs");

            List<Configs> data = JsonConvert.DeserializeObject<List<Configs>>(result);

            return data;
        }
        public async Task Create(Configs config)
        {
            await this.client.PostAsJsonAsync("/api/users", config);
        }

        public async Task Update(Configs config)
        {
            await this.client.PutAsJsonAsync($"/api/users/{config.id}", config);
        }

    }
    
}
