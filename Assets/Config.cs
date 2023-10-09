public static class Config
{
    public static string proto { get; set; } = "http://";
    public static string address { get; set; } = proto + "localhost:8081/";
    public static string apiCall { get; set; } = address + "api/";
}
