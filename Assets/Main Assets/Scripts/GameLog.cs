using UnityEngine;
using System.Collections;

public static class GameLog {

    static private bool logToEditor = true;

    public static void InitGameLog()
    {
        string logPath = @"D:\Unitycraft3\Logs";
        System.IO.Directory.CreateDirectory(logPath);
        if (System.IO.File.Exists(logPath + "/Game.log"))
        {
            System.DateTime time = System.DateTime.UtcNow;
            string backupFileName = "Game(" + time.Hour + "-" + time.Minute + "-" + time.Second + ").log";
            System.IO.Directory.CreateDirectory(logPath + "/backups");
            System.IO.File.Move(logPath + "/Game.log", logPath + "/backups/" + backupFileName);
        }
    }

    public static void Log(string service, string component, string logLine)
    {
        string debugString = ("[" + service + "][" + component + "] " + logLine);
        if (logToEditor) { Debug.Log(debugString); }
        WriteToFile("<" + System.DateTime.UtcNow.ToString() + ">"+debugString);
    }

    public static void LogIfTrue(string service, string component, string logLine, bool flag)
    {
        if (flag)
        {
            string debugString = ("[" + service + "][" + component + "] " + logLine);
            if (logToEditor) { Debug.Log(debugString); }
            WriteToFile("<" + System.DateTime.UtcNow.ToString() + ">" + debugString);
        }
    }

    private static void WriteToFile(string logLine)
    {
        using (System.IO.StreamWriter writer = System.IO.File.AppendText("D:/UnityCraft3/Logs/Game.log"))
        {
            writer.WriteLine(logLine);
            writer.Close();
        }
    }
}
