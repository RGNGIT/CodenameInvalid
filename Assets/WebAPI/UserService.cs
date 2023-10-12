#nullable enable
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class UserService
{
    readonly HttpClient client;

    public UserService(HttpClient client) 
    {
        this.client = client;
    }

    public async Task<ApiResponse<User?>?> Login(string email, string password)
    {
        StringContent content = new StringContent(
            $"{{\"Email\":\"{email}\", \"Password\":\"{password}\"}}", 
            System.Text.Encoding.UTF8, 
            "application/json"
        );

        try 
        {
            HttpResponseMessage response = await client.PostAsync(Config.apiCall + "login", content);

            if (response.IsSuccessStatusCode)
            {
                string stringResponse = await response.Content.ReadAsStringAsync();
                ApiResponse<User?>? parsedResponse = JsonConvert.DeserializeObject<ApiResponse<User?>>(stringResponse);
                return parsedResponse;
            }

            return null;
        } 
        catch
        {
            return null;
        }
    }

    public async Task<ApiResponse<User?>?> Register(string name, string surname, string patronymic, string email, string password, string gender, int age) 
    {
        Dictionary<string, int> genderMap = new()
        {
            { "Мужской", 1 },
            { "Женский", 2 }
        };

        StringContent content = new StringContent(
            $"{{\"Interface_Type\":\"0\", \"Rating\":\"0\", \"Role_Key\":\"1\",\"Name\":\"{name}\", \"Surname\":\"{surname}\", \"Patronymic\":\"{patronymic}\", \"Email\":\"{email}\", \"Login\":\"{email}\", \"Password\":\"{password}\", \"Sex_Key\":\"{genderMap[gender]}\", \"Age\":\"{age}\"}}",
            System.Text.Encoding.UTF8,
            "application/json"
        );

        try 
        {
            HttpResponseMessage response = await client.PostAsync(Config.apiCall + "newPhys", content);

            if (response.IsSuccessStatusCode)
            {
                string stringResponse = await response.Content.ReadAsStringAsync();
                ApiResponse<User?>? parsedResponse = JsonConvert.DeserializeObject<ApiResponse<User?>>(stringResponse);
                return parsedResponse;
            }

            return null;
        }
        catch 
        {
            return null;
        }
    }
}
