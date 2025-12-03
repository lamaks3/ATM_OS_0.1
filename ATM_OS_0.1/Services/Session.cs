namespace ATM_OS;

public class Session
{
    public string CardUID { get; private set; }

    public Session(string cardUid)
    {
        CardUID = cardUid;
    }
}