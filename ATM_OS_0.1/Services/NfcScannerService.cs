using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.Json;

namespace ATM_OS;

public class NfcScannerService
{
    public static string CardUID { get; private set; } 
    
    public static string GetCardUID()
    {
        return CardUID;
    }
    
    public static void SetCardUID(string num)
    {
        CardUID = num;
    }
    
    static void RunAdbReverse()
    {
        var process = new Process();
        process.StartInfo.FileName = "adb";
        process.StartInfo.Arguments = "reverse tcp:5055 tcp:5055";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();
        process.WaitForExit();
    }
    
    public static async Task StartServer()
    {
        RunAdbReverse();
        var listener = new HttpListener();
        listener.Prefixes.Add("http://127.0.0.1:5055/api/identify/");
        listener.Start();
        Console.WriteLine("[Server status] Server started successfully");

        while (true)
        {
            var context = await listener.GetContextAsync();
            var request = context.Request;

            string body;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                body = await reader.ReadToEndAsync();
            }

            Console.WriteLine($"Request received: {body}");
            string uid = "";
            
            var json = JsonDocument.Parse(body);
            if (json.RootElement.TryGetProperty("uid", out var uidElement))
            {
                uid = uidElement.GetString() ?? "";
                SetCardUID(uid);
            }
            
            string responseString = "{\"result\": \"ok\"}";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.Close();
        }
    }
}