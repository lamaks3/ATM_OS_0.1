using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.Json;
using ATM_OS_Configuration;
using ATM_OS;
using ATM_OS.Business.Interfaces.Repositories;

namespace ATM_OS_services;

public class NfcScannerService : INfcScannerService
{
    private static string сardUid;
    private HttpListener _listener;
    
    public string GetCardUid()
    {
        return сardUid;
    }
    
    private void SetCardUid(string num)
    {
        сardUid = num;
    }

    public void ClearCardUid()
    {
        сardUid = string.Empty;
    }
    
    static void RunAdbReverse()
    {
        var process = new Process();
        process.StartInfo.FileName = "adb";
        process.StartInfo.Arguments = "reverse tcp:5055 tcp:5055"; //forward
        process.Start();
        process.WaitForExit();
    }

    public async Task StartScanner()
    {

        RunAdbReverse();
        _listener = new HttpListener();
        _listener.Prefixes.Add(AtmConfiguration.NfcServerUrl);
        _listener.Start();
        Console.WriteLine("[Server status] Server started");

        while (true)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                var request = context.Request;

                string body;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    body = reader.ReadToEnd();
                }

                Console.WriteLine($"[Server] Request received: {body}");

                var json = JsonDocument.Parse(body);
                if (json.RootElement.TryGetProperty("uid", out var uidElement))
                {
                    SetCardUid((uidElement.GetString() ?? "").ToString());
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
    
    public void StopScanner()
    {
        _listener?.Stop();
        _listener?.Close();
        Console.WriteLine("[NFC Scanner] Stopped");
    }
}
