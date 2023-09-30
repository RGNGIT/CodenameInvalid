#nullable enable
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class UserService
{
    readonly HttpClient client;

    public UserService(HttpClient client) 
    {
        this.client = client;
    }

    public async Task<User?> Login(string login, string password)
    {
        HttpResponseMessage response = await client.GetAsync(Config.apiCall + "login");

        if (response.IsSuccessStatusCode) 
        {
            string stringResponse = await response.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<User>(stringResponse);
        }

        return null;
    }
}
