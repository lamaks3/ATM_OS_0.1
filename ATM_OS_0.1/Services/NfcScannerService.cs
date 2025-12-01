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
    private static string сardUid { get; set; } 
    
    public static string GetCardUid()
    {
        return сardUid;
    }
    
    public static void SetCardUid(string num)
    {
        сardUid = num;
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
        listener.Prefixes.Add("http://127.0.0.1:5055/api/nfc/scan/");
        listener.Start();
        Console.WriteLine("[Server status] Server started successfully");

        while (true)
        {
            try
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
                    SetCardUid(uid);
                }

                string responseString = "{\"result\": \"data recieved\"}";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentType = "application/json";
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                context.Response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] request failed: {ex.Message}");
            }

        }
    }
}
