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
    public class UsersService
    {
        private HttpClient client;
        public UsersService()
        {
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri("http://localhost:35125");
        }
        public async Task<List<User>> FindAll()
        {
            string result = await this.client.GetStringAsync("/api/users");

            List<User> data = JsonConvert.DeserializeObject<List<User>>(result);

            return data;
        }
        public async Task Create(User user)
        {
            await this.client.PostAsJsonAsync("/api/users", user);
        }

        public async Task Update(User user)
        {
            await this.client.PutAsJsonAsync($"/api/users/{user.id}", user);
        }

        public async Task CreateThisUser()
        {
            string hostName = Dns.GetHostName();
            HttpClient httpClient = new HttpClient();
            string ip = await httpClient.GetStringAsync("https://api.ipify.org");
            User thisUser = new User(hostName, ip, true, new DateTime(), null, null);
            await Create(thisUser);
        }
        public async Task<User> FindThisUser()
        {
            string hostName = Dns.GetHostName();
            HttpClient httpClient = new HttpClient();
            string ip = await httpClient.GetStringAsync("https://api.ipify.org");
            List<User> users = await FindAll();
            foreach (User user in users)
            {
                if (user.username == hostName || user.IP == ip)
                    return user;
            }
            return null;
        }
    }
    
}
