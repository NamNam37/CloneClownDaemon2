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
    public class LogsService
    {
        private HttpClient client;
        public LogsService()
        {
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri("http://localhost:35125");
        }
        public async Task<List<Logs>> FindAll()
        {
            string result = await this.client.GetStringAsync("/api/logs");

            List<Logs> data = JsonConvert.DeserializeObject<List<Logs>>(result);

            return data;
        }
        public async Task Create(Logs log)
        {
            await this.client.PostAsJsonAsync("/api/logs", log);
        }

        public async Task Update(Logs log)
        {
            await this.client.PutAsJsonAsync($"/api/logs/{log.id}", log);
        }

    }
    
}
