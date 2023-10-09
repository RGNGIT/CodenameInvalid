using System.Net.Http;

public class API
{
    static HttpClient client = new HttpClient();

    public UserService UserServiceInstanse() => new UserService(client);
}
