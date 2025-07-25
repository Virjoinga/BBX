using System;
using System.Collections.Generic;
using System.Linq;

public static class CommandLineArgsManager
{
	private static List<string> _args;

	public static bool HasArg(string name)
	{
		ParseCommandLineArgs();
		return _args.Contains(name);
	}

	public static string GetArg(string name, string defaultValue)
	{
		ParseCommandLineArgs();
		int num = _args.IndexOf(name);
		if (num < 0 || num > _args.Count)
		{
			return defaultValue;
		}
		return _args[num + 1];
	}

	public static T GetArg<T>(string name, T defaultValue) where T : struct, IConvertible
	{
		if (!Enum<T>.TryParse(GetArg(name, defaultValue.ToString()), out var value))
		{
			return defaultValue;
		}
		return value;
	}

	private static void ParseCommandLineArgs()
	{
		if (_args == null)
		{
			_args = Environment.GetCommandLineArgs().ToList();
		}
	}

	public static bool IsHeadlessMode()
	{
		if (Environment.CommandLine.Contains("-batchmode"))
		{
			return Environment.CommandLine.Contains("-nographics");
		}
		return false;
	}
}
