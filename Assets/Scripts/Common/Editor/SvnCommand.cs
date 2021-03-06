using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Collections.Generic;

public class SvnCommand
{
    private static string GetSelectedObjectPaths()
    {
        List<string> selectedObjectPaths = new List<string>(Selection.objects.Length);

        string projectPath = Application.dataPath.Replace("Assets", string.Empty);

        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            selectedObjectPaths.Add(path);
        }
        string pathsString = string.Empty;

        for( int i = 0 ; i < selectedObjectPaths.Count ; i++)
        {
            string item = selectedObjectPaths[i];
            if (i == 0)
            {
                pathsString = string.Format("{1}{0}", item, projectPath);
            }
            else 
            {
                pathsString = string.Format("{0} {2}{1}", pathsString, item, projectPath);
            }
        }
        return pathsString;
    }
    
    [MenuItem("Assets/Svn/SVN Log(Selected)", false, 9)]
    public static void SvnLogSelected()
    {
        string pathsString = GetSelectedObjectPaths();

        BeginSvnCommand("log", pathsString);
    }

    [MenuItem("Assets/Svn/SVN Update(Selected)", false, 10)]
    public static void SvnUpdateSelected()
    {
        string pathsString = GetSelectedObjectPaths();

        BeginSvnCommand("update", pathsString);
    }

    [MenuItem("Assets/Svn/SVN Commit(Selected)", false, 10)]
    public static void SvnCommitSelectedFromPopUpMenu()
    {
        string pathsString = GetSelectedObjectPaths();

        BeginSvnCommand("commit", pathsString);
    }

    [MenuItem("Assets/Svn/SVN Update (ALL)", false, 11)]
    public static void SvnUpdateFromPopUpMenu()
    {
        BeginSvnCommand("update", string.Empty);
    }


    [MenuItem("Assets/Svn/SVN Commit (ALL)", false, 11)]
    public static void SvnCommit_()
    {
        BeginSvnCommand("commit", string.Empty);
    }
    
    [MenuItem("Svn/1 Svn Update %u", false, 10)]
    static void SvnUpdateProject()
    {
        BeginSvnCommand("update", string.Empty);
    }


    [MenuItem("Svn/2 Svn Commit %m", false, 20)]
    static void SvnCommitProject()
    {
        BeginSvnCommand("commit", string.Empty);
    }
    
    [MenuItem("Svn/Svn Log", false, 20)]
    static void SvnLogProject()
    {
        BeginSvnCommand("log", string.Empty);
    }

    static public System.Diagnostics.Process BeginSvnCommand(string command, string parameter, bool _wait = false, bool useGUI = true, bool showGuide = true, bool createNoWindow = false)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 1f; 
        AssetDatabase.SaveAssets();
        Time.timeScale = originalTimeScale;

        string path = Application.dataPath;
        if (string.IsNullOrEmpty(parameter) == false)
        {
            path = parameter;
        }

        if (showGuide)
        {
            if (EditorUtility.DisplayDialog("svn command", string.Format( "Do you want to run {0}?\n{1}", command, path),  "OK", "Cancel"))
            {
                return DoSvnCommand(command, parameter, _wait, useGUI, createNoWindow);
            }
        }else
        {
            return DoSvnCommand(command, parameter, _wait, useGUI, createNoWindow);
        }
		return null;
    }

    public static System.Diagnostics.Process DoSvnCommand(string svnCommand, string toPath, bool _isWait = false, bool useGUI = true, bool createNoWindow = false, string Arguments = null)
    {
        if (string.IsNullOrEmpty(toPath))
        {
            toPath = Application.dataPath.Replace("Assets", string.Empty);
        }

        System.Diagnostics.Process p = new System.Diagnostics.Process();
        if (Application.platform == RuntimePlatform.OSXEditor || useGUI == false)
        {
            p.StartInfo.FileName = "svn";
            //Debug.Log(p.StartInfo.Arguments);
            string commitMessage = string.Empty;


            p.StartInfo.Arguments = string.Format("{1} \"{0}\" {2}", toPath, svnCommand, Arguments);
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = createNoWindow;
            p.StartInfo.WorkingDirectory = Application.dataPath + "/..";
            p.StartInfo.UseShellExecute = false;
            p.Start();
        }
        else
        {
            string pathStr = string.Empty;

            string[] paths = toPath.Split(' ');
            for(int i = 0 ; i < paths.Length ; i++)
            {
                string item = paths[i];
                if (i == 0)
                {
                    pathStr = string.Format("\"{0}\"", item);
                }
                else
                {
                    pathStr = string.Format("{0} \"{1}\"", pathStr, item);
                }
            }

            p.StartInfo.FileName = "TortoiseProc.exe";
			p.StartInfo.Arguments = string.Format("/command:{1} /path:{0} /closeonend:0", pathStr, svnCommand);
            p.Start();

            //p의 일이 다 끝날때 까지 기다린다.
            if (_isWait)
            {
                Debug.Log("WaitForExit 기능때문에 정지하는지 확인하기 위해서 WaitForExit 기능 사용안함");
                //p.WaitForExit();
            }
            Debug.Log(p.StartInfo.Arguments);
        }

        if( svnCommand == "update")
            AssetDatabase.Refresh();

        return p;
    }
}
