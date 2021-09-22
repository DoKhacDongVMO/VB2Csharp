﻿//----------------------------------------------------------------------------------------
//	Copyright © 2003 - 2021 Tangible Software Solutions, Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	The methods in this class replicate the behavior of IsNumeric and Val.
//----------------------------------------------------------------------------------------
using System;
using System.Globalization;

public static class NumericHelper
{
	public static bool IsNumeric(object expression)
	{
		if (expression == null)
			return false;

		if (expression is string)
		{
			string expressionAsString = (string)expression;

			double testDouble;
			if (double.TryParse(expressionAsString, NumberStyles.Any, CultureInfo.CurrentCulture, out testDouble))
				return true;
			else if (expressionAsString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			{
				long testLong;
				if (long.TryParse(expressionAsString.Substring("0x".Length), NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out testLong))
					return true;
			}
			else if (expressionAsString.StartsWith("&H", StringComparison.OrdinalIgnoreCase))
			{
				long testLong;
				if (long.TryParse(expressionAsString.Substring("&H".Length), NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out testLong))
					return true;
			}
		}
		else
		{
			double testDouble;
			if (double.TryParse(expression.ToString(), out testDouble))
				return true;
		}

		//VB's 'IsNumeric' returns true for any boolean value:
		bool testBool;
		if (bool.TryParse(expression.ToString(), out testBool))
			return true;

		return false;
	}

	public static double Val(string expression)
	{
		if (expression == null)
			return 0;

		if (expression.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
		{
			long testLong;
			if (long.TryParse(expression.Substring("0x".Length), NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out testLong))
				return testLong;
		}
		else if (expression.StartsWith("&H", StringComparison.OrdinalIgnoreCase))
		{
			long testLong;
			if (long.TryParse(expression.Substring("&H".Length), NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out testLong))
				return testLong;
		}

		//try the entire string, then progressively smaller substrings to replicate the behavior of VB's 'Val', which ignores trailing characters after a recognizable value:
		for (int size = expression.Length; size > 0; size--)
		{
			double testDouble;
			if (double.TryParse(expression.Substring(0, size), out testDouble))
				return testDouble;
		}

		//no value is recognized, so return 0:
		return 0;
	}

	public static double Val(object expression)
	{
		if (expression == null)
			return 0;

		double testDouble;
		if (double.TryParse(expression.ToString(), out testDouble))
			return testDouble;

		//VB's 'Val' function returns -1 for 'true':
		bool testBool;
		if (bool.TryParse(expression.ToString(), out testBool))
			return testBool ? -1 : 0;

		//VB's 'Val' function returns the day of the month for dates:
		DateTime testDate;
		if (DateTime.TryParse(expression.ToString(), out testDate))
			return testDate.Day;

		//no value is recognized, so return 0:
		return 0;
	}

	public static int Val(char expression)
	{
		int testInt;
		if (int.TryParse(expression.ToString(), out testInt))
			return testInt;
		else
			return 0;
	}

}