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
    public static string CardUID { get; private set; } = string.Empty;

    public NfcScannerService()
    {
        
    }
    
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
        try
        {
            var process = new Process();
            process.StartInfo.FileName = "adb";
            process.StartInfo.Arguments = "reverse tcp:5055 tcp:5055";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error with adb reverse: " + ex.Message);
        }
    }
    
    public static async Task StartServer()
    {
        RunAdbReverse();
        var listener = new HttpListener();
        listener.Prefixes.Add("http://127.0.0.1:5055/api/identify/");
        listener.Start();
        Console.WriteLine("\n\n\nServer is working");

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
            try
            {
                var json = JsonDocument.Parse(body);
                if (json.RootElement.TryGetProperty("uid", out var uidElement))
                {
                    uid = uidElement.GetString() ?? "";
                    SetCardUID(uid);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка парсинга JSON: " + ex.Message);
            }
            
            if (uid == "0124c499")
            {
                Console.WriteLine("VISA BYN Owner: Andrew");
            }
            else if (uid == "c135e799")
            {
                Console.WriteLine("Belcard BYN Owner: Jack Jones");
            }
            else if (!string.IsNullOrEmpty(uid))
            {
                Console.WriteLine($"Unknown card UID: {uid}");
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