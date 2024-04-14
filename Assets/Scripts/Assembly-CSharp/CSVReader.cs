using System.Collections.Generic;
using UnityEngine;

public class CSVReader
{
	private int _columnCount;

	private List<List<string>> _rows;

	private static char quotes = '"';

	private static char separator = ',';

	private static char newline = '\n';

	private static char carriageReturn = '\r';

	public int ColumnCount => _columnCount;

	public int RowCount => _rows.Count;

	public int GetRowElementCount(int idx)
	{
		return _rows[idx].Count;
	}

	public string GetRowElement(int rowIdx, int colIdx)
	{
		return _rows[rowIdx][colIdx];
	}

	public void LoadFromTextAsset(TextAsset textAsset)
	{
		_columnCount = 0;
		_rows = new List<List<string>>();
		string text = textAsset.text;
		if (text[text.Length - 1] != newline)
		{
			text += newline;
		}
		ParseText(_rows, text);
		foreach (List<string> row in _rows)
		{
			_columnCount = Mathf.Max(_columnCount, row.Count);
		}
	}

	private static void ParseText(List<List<string>> rows, string text)
	{
		int num = 0;
		while (num < text.Length)
		{
			List<string> list = new List<string>();
			num += ParseLine(list, text, num);
			rows.Add(list);
		}
	}

	private static int ParseLine(List<string> row, string text, int pos)
	{
		int num = pos;
		while (pos < text.Length)
		{
			string field = "";
			pos += ParseField(out field, text, pos);
			bool flag = false;
			while (pos < text.Length)
			{
				if (separator == text[pos])
				{
					pos++;
					break;
				}
				if (newline == text[pos] || carriageReturn == text[pos])
				{
					flag = true;
					if (carriageReturn == text[pos] && newline == text[pos + 1])
					{
						pos++;
					}
					pos++;
					break;
				}
			}
			row.Add(field);
			if (flag)
			{
				break;
			}
		}
		return pos - num;
	}

	private static int ParseField(out string field, string text, int pos)
	{
		field = "";
		int num = pos;
		if (quotes == text[pos])
		{
			pos++;
			while (pos < text.Length)
			{
				if (quotes == text[pos])
				{
					if (pos + 1 >= text.Length || quotes != text[pos + 1])
					{
						pos++;
						break;
					}
					field += quotes;
					pos += 2;
				}
				else
				{
					field += text[pos];
					pos++;
				}
			}
		}
		else
		{
			while (pos < text.Length && separator != text[pos] && newline != text[pos] && carriageReturn != text[pos])
			{
				field += text[pos];
				pos++;
			}
		}
		return pos - num;
	}

	public bool Test()
	{
		string text = "id,en,fr\ntest_id,English Test,\"Francais test's\"";
		List<List<string>> list = new List<List<string>>();
		ParseText(list, text);
		DebugUtils.DebugAssert(list[0][0] == "id");
		DebugUtils.DebugAssert(list[0][1] == "en");
		DebugUtils.DebugAssert(list[0][2] == "fr");
		DebugUtils.DebugAssert(list[1][0] == "test_id");
		DebugUtils.DebugAssert(list[1][1] == "English Test");
		DebugUtils.DebugAssert(list[1][2] == "Francais test's");
		return true;
	}
}
