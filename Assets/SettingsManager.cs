// from https://forum.unity.com/threads/setting-player-window-position.534733/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

#if UNITY_STANDALONE_WIN

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

//using System.Windows.Forms;


#endif

public class SettingsManager : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI text;

	string primaryFilePath;
	Process fExplorerProcess;

	public bool fullScreen = false;
	private static string fullScreenKey = "Full Screen";
	public int minDisplaySize = 384;
	private static int monitorWidth;
	private static int monitorHeight;

	int currX, currY;

#if UNITY_STANDALONE_WIN

	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

	[DllImport("user32.dll", EntryPoint = "FindWindow")]
	public static extern IntPtr FindWindow(string className, string windowName);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

	const UInt32 WM_CLOSE = 0x0010;

	public static IEnumerator SetWindowPosition(int x, int y)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		SetWindowPos(FindWindow(null, Application.productName), 0, x, y, 0, 0, 5);
	}

	public static void OpenFile(string path)
	{
		Process fil = new Process();
		fil.StartInfo.FileName = path;
		fil.EnableRaisingEvents = true;
	}


	public void OpenFileExplorer(string folder)
	{
		fExplorerProcess = new Process();

		fExplorerProcess.StartInfo.FileName = "explorer.exe";
		fExplorerProcess.StartInfo.Arguments = string.Format("\"{0}\"", folder);
		fExplorerProcess.EnableRaisingEvents = true;
		//fExplorerProcess.Exited += new EventHandler(ReOpenExplorer);
		fExplorerProcess.Start();
	}

	private byte[] StringToByteArray(string str)
	{
		System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
		return enc.GetBytes(str);
	}
	public List<string> ListOfTextWindows()
	{
		Process[] processList = Process.GetProcesses().Where<Process>(x=> x.ProcessName.ToLower().Contains("note")).ToArray();
		text.text = processList.Length.ToString();
		foreach (var process in processList)
		{
			text.text += " " + process.ProcessName;
			//text.text += "\n" + process.ProcessName +" " + process.MainWindowTitle;
		}
		return new List<string>();
		//List<string> equipmentNames = process
	}


	public void CreateWeaponFile(CombatAction action, string drawing, string path = "", bool open = false)
	{
		string wepPath = primaryFilePath;
		if (path != "")
			wepPath += path;
		wepPath += "\\"+ action.name + ".weapon";
		if (!File.Exists(wepPath))
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("+-------");
			sb.Append('-', action.name.Length);
			sb.Append("-+---------");
			sb.Append('-', action.damage.ToString().Length);
			sb.Append("-+--------------");
			sb.Append('-', action.energy.ToString().Length);
			sb.Append("-+");
			string topBracket = sb.ToString();

			sb.AppendLine();
			sb.Append("[ Name: ");
			sb.Append(action.name);
			sb.Append(" | Damage: ");
			sb.Append(action.damage);
			sb.Append(" | Energy Cost: ");
			sb.Append(action.energy);
			sb.AppendLine(" |");

			System.Text.StringBuilder bb = new System.Text.StringBuilder("+--------");
			bb.Append('-', action.speed.ToString().Length);
			bb.Append("-+----------");
			bb.Append('-', action.defense.ToString().Length);
			bb.Append("-+-------------");
			bb.Append('-', action.targetting.ToString().Length);
			bb.Append("-+");
			string botbracket = bb.ToString();
			if (botbracket.Length > topBracket.Length)
				sb.AppendLine(botbracket);
			else
				sb.AppendLine(topBracket);

			sb.Append("| Speed: ");
			sb.Append(action.speed);
			sb.Append(" | Defense: ");
			sb.Append(action.defense);
			sb.Append(" | Targetting: ");
			sb.Append(action.targetting.ToString());
			sb.AppendLine(" ]");
			sb.Append(botbracket);


			int drawingLength = drawing.Split('\n')[0].Length;
			int drawingSideDiff = drawingLength - botbracket.Length;
			if (drawingSideDiff > 0)
			{
				sb.Append('-', drawingSideDiff+2);
				sb.AppendLine("+");
				System.Text.StringBuilder dr = new System.Text.StringBuilder("| "+drawing);
				dr.Replace("\n", "| \n| ");
				sb.Append(dr);
				sb.AppendLine("|");
				sb.Append('+');
				sb.Append('-', drawingLength+1);
				sb.Append('+');
			}
			else
			{
				sb.AppendLine();
				System.Text.StringBuilder dr = new System.Text.StringBuilder("| " + drawing);
				dr.Replace("\n", "| \n| ");
				sb.Append(dr);
				sb.AppendLine("|");
				sb.Append('+');
				sb.Append('-', drawingLength + 1);
				sb.Append('+');
			}

			using (FileStream fs = File.Create(wepPath))
			{
				byte[] byteb = StringToByteArray(sb.ToString());
				fs.Write(byteb, 0, byteb.Length);
			}
		}
		if (open)
			OpenFile(wepPath);
	}
#endif

	void Start()
	{
		primaryFilePath = string.Format("{0}", Application.dataPath).Replace("/", "\\") + "\\Play Area\\";
		monitorWidth = Screen.resolutions[Screen.resolutions.Length - 1].width;
		monitorHeight = Screen.resolutions[Screen.resolutions.Length - 1].height;
		CheckAndSet();

#if UNITY_STANDALONE_WIN

		Directory.CreateDirectory(primaryFilePath);
		Directory.CreateDirectory(primaryFilePath + "\\Character");
		Directory.CreateDirectory(primaryFilePath + "\\Character\\Equipped");
		Directory.CreateDirectory(primaryFilePath + "\\Readied Actions");
		OpenFileExplorer(primaryFilePath);


#endif

	}
	/*
	private void ReOpenExplorer(object sender, System.EventArgs e)
	{
	}
	*/

	public string[] GetReadiedFiles()
	{
		string filePath = primaryFilePath +"\\Readied Actions";

		return Directory.GetFiles(filePath).Where(x => x.Contains(".weapon")).Select(x => x.Remove(x.Length-7)).ToArray();
	}
	public void CleanReadiedActions()
	{
		foreach (string file in Directory.GetFiles(primaryFilePath + "\\Readied Actions"))
		{
			string destination = primaryFilePath + file.Substring(file.LastIndexOf('\\') + 1);
			if (File.Exists(destination))
				File.Delete(file);
			else
				File.Move(file, destination);
		}
	}

	#region Window Setup
	private void CheckAndSet()
	{
		if (PlayerPrefs.GetInt(fullScreenKey, 0) >= 1)
		{
			SetFullScreen();
		}
		else
		{
			SetWindowed();
		}
	}

	public void SetFullScreen()
	{
		Screen.SetResolution(monitorWidth, monitorHeight, true);
		fullScreen = true;
		PlayerPrefs.SetInt(fullScreenKey, 1);
		PlayerPrefs.Save();
	}

	public void SetWindowed()
	{
		SetWindowResolution();
		fullScreen = false;
		PlayerPrefs.SetInt(fullScreenKey, 0);
		PlayerPrefs.Save();
	}

	private void SetWindowResolution()
	{
		int multiplier = 1;
		if (monitorWidth >= monitorHeight)
		{
			multiplier = monitorHeight / minDisplaySize;
			if ((monitorHeight % minDisplaySize) == 0)
			{
				multiplier--;
			}
		}
		else
		{
			multiplier = monitorWidth / minDisplaySize;
			if ((monitorWidth % minDisplaySize) == 0)
			{
				multiplier--;
			}
		}
		int size = minDisplaySize * multiplier;
		if (size < minDisplaySize)
		{
			size = minDisplaySize;
		}
		Screen.SetResolution(size, size, false);

#if UNITY_STANDALONE_WIN
		int x = monitorWidth / 2;
		x -= size  * 1 / 5;
		int y = monitorHeight / 2;
		y -= size / 2;
		currX = x;
		currY = y;
		StartCoroutine(SetWindowPosition(x, y));

#endif
	}
	#endregion
}