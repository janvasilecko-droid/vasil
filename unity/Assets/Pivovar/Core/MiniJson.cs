// Malý JSON parser bez závislosti na Unity (jádro simulace se kompiluje i testuje mimo Unity).
// Výstup: Dictionary<string, object>, List<object>, double, string, bool, null.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Pivovar.Core
{
    public static class MiniJson
    {
        public static object Parse(string json)
        {
            if (json == null) throw new ArgumentNullException("json");
            var p = new Parser(json);
            p.SkipWs();
            object v = p.ReadValue();
            p.SkipWs();
            if (!p.End) throw new FormatException("JSON: nadbytečný text na pozici " + p.Pos);
            return v;
        }

        sealed class Parser
        {
            readonly string s;
            int i;
            public Parser(string text) { s = text; i = 0; if (s.Length > 0 && s[0] == '﻿') i = 1; }
            public bool End { get { return i >= s.Length; } }
            public int Pos { get { return i; } }

            public void SkipWs()
            {
                while (i < s.Length && (s[i] == ' ' || s[i] == '\t' || s[i] == '\n' || s[i] == '\r')) i++;
            }

            public object ReadValue()
            {
                if (End) throw new FormatException("JSON: neočekávaný konec");
                char c = s[i];
                if (c == '{') return ReadObject();
                if (c == '[') return ReadArray();
                if (c == '"') return ReadString();
                if (c == 't') { Expect("true"); return true; }
                if (c == 'f') { Expect("false"); return false; }
                if (c == 'n') { Expect("null"); return null; }
                return ReadNumber();
            }

            void Expect(string w)
            {
                if (string.CompareOrdinal(s, i, w, 0, w.Length) != 0) throw new FormatException("JSON: očekáváno '" + w + "' na pozici " + i);
                i += w.Length;
            }

            Dictionary<string, object> ReadObject()
            {
                var d = new Dictionary<string, object>();
                i++; SkipWs();
                if (i < s.Length && s[i] == '}') { i++; return d; }
                while (true)
                {
                    SkipWs();
                    if (End || s[i] != '"') throw new FormatException("JSON: klíč objektu na pozici " + i);
                    string k = ReadString();
                    SkipWs();
                    if (End || s[i] != ':') throw new FormatException("JSON: chybí ':' na pozici " + i);
                    i++; SkipWs();
                    d[k] = ReadValue();
                    SkipWs();
                    if (End) throw new FormatException("JSON: neukončený objekt");
                    if (s[i] == ',') { i++; continue; }
                    if (s[i] == '}') { i++; return d; }
                    throw new FormatException("JSON: v objektu na pozici " + i);
                }
            }

            List<object> ReadArray()
            {
                var l = new List<object>();
                i++; SkipWs();
                if (i < s.Length && s[i] == ']') { i++; return l; }
                while (true)
                {
                    SkipWs();
                    l.Add(ReadValue());
                    SkipWs();
                    if (End) throw new FormatException("JSON: neukončené pole");
                    if (s[i] == ',') { i++; continue; }
                    if (s[i] == ']') { i++; return l; }
                    throw new FormatException("JSON: v poli na pozici " + i);
                }
            }

            string ReadString()
            {
                var sb = new StringBuilder();
                i++; // úvodní "
                while (true)
                {
                    if (End) throw new FormatException("JSON: neukončený řetězec");
                    char c = s[i++];
                    if (c == '"') break;
                    if (c != '\\') { sb.Append(c); continue; }
                    if (End) throw new FormatException("JSON: neplatný escape");
                    char e = s[i++];
                    switch (e)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            if (i + 4 > s.Length) throw new FormatException("JSON: neplatné \\u");
                            sb.Append((char)Convert.ToInt32(s.Substring(i, 4), 16));
                            i += 4; break;
                        default: throw new FormatException("JSON: neznámý escape \\" + e);
                    }
                }
                return sb.ToString();
            }

            double ReadNumber()
            {
                int st = i;
                while (i < s.Length)
                {
                    char c = s[i];
                    if ((c >= '0' && c <= '9') || c == '-' || c == '+' || c == '.' || c == 'e' || c == 'E') i++;
                    else break;
                }
                if (st == i) throw new FormatException("JSON: neočekávaný znak '" + s[i] + "' na pozici " + i);
                return double.Parse(s.Substring(st, i - st), NumberStyles.Float, CultureInfo.InvariantCulture);
            }
        }
    }
}
