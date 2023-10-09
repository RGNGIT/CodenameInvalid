#nullable enable
using System.Collections.Generic;
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

    public async Task<User?> Login(string email, string password)
    {
        StringContent content = new StringContent(
            $"{{\"Email\":\"{email}\", \"Password\":\"{password}\"}}", 
            System.Text.Encoding.UTF8, 
            "application/json"
        );

        HttpResponseMessage response = await client.PostAsync(Config.apiCall + "login", content);

        if (response.IsSuccessStatusCode) 
        {
            string stringResponse = await response.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<User>(stringResponse);
        }

        return null;
    }

    public async Task<User?> Register(string name, string surname, string patronymic, string email, string password, string gender, int age) 
    {
        Dictionary<string, int> genderMap = new()
        {
            { "�������", 1 },
            { "�������", 2 }
        };

        StringContent content = new StringContent(
            $"{{\"Name\":\"{name}\", \"Surname\":\"{surname}\", \"Patronymic\":\"{patronymic}\", \"Email\":\"{email}\", \"Password\":\"{password}\", \"Sex_Key\":\"{genderMap[gender]}\", \"Age\":\"{age}\"}}",
            System.Text.Encoding.UTF8,
            "application/json"
        );

        HttpResponseMessage response = await client.PostAsync(Config.apiCall + "newPhys", content);

        if (response.IsSuccessStatusCode) 
        {
            string stringResponse = await response.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<User>(stringResponse);
        }

        return null;
    }
}
