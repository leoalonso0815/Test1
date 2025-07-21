using System;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// 带有文件打印的日志辅助器。
/// </summary>
public class FileLogger
{
    private string CurrentLogPath = "";

    private string CurrentLog = "";
    //private string PreviousLogPath = "";

    public void Init()
    {
        var nowTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"); //{nowTime}
        CurrentLog = $"{Application.persistentDataPath}/log";
        CurrentLogPath = GetRegularPath(Path.Combine(CurrentLog, $"{nowTime}.log"));
        PlayerPrefs.SetString("lastLog", CurrentLogPath);
        //PreviousLogPath = GetRegularPath(Path.Combine(Application.persistentDataPath, "before.log"));
        Debug.Log($"文件日志路径：{CurrentLogPath}");
        
        Application.logMessageReceived += OnLogMessageReceived;

        try
        {
            // if (File.Exists(PreviousLogPath))
            // {
            //     File.Delete(PreviousLogPath);
            // }
            if (!Directory.Exists(CurrentLog))
            {
                Directory.CreateDirectory(CurrentLog);
            }
            // if (File.Exists(CurrentLogPath))
            // {
            //    File.Move(CurrentLogPath, CurrentLogPath);
            // }
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void OnLogMessageReceived(string logMessage, string stackTrace, LogType logType)
    {
        var time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";
        var log =
            $"[{logType}] {logMessage ?? "<Empty Message>"}{Environment.NewLine} {stackTrace ?? "<Empty StackTrace>"}{Environment.NewLine}";
        // log = $"{logMessage}{Environment.NewLine}";
        try
        {
            File.AppendAllText(CurrentLogPath, log, Encoding.UTF8);
        }
        catch(Exception e)
        {
            Debug.LogError($"log文件生成失败:{CurrentLogPath}  {e}");
            throw;
        }
    }

    private static string GetRegularPath(string path)
    {
        if (path == null)
        {
            return null;
        }

        return path.Replace('\\', '/');
    }
}