using System;
using System.Threading.Tasks;

namespace ATM_OS.Business.Interfaces.Repositories;

public interface INfcScannerService
{
    string GetCardUid();
    void ClearCardUid();
    Task StartScanner();
    void StopScanner();
}