// Vestavěné metody typů Godotu (Array, Dictionary, String, Vector2, Color, Rect2, Callable) volané dynamicky z přeloženého kódu.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Pivovar.Gd
{
    public static class GvMethods
    {
        static Gv A(Gv[] a, int i) { return i < a.Length ? a[i] : default(Gv); }
        static GdError NoMethod(Gv self, string n) { return new GdError("Metoda '" + n + "' není pro typ " + self.k + " implementována"); }

        public static Gv Call(Gv self, string n, Gv[] a)
        {
            switch (self.k)
            {
                case K.Obj: return ((GdObject)self.o).Invoke(n, a);
                case K.Arr: return ArrCall(self, n, a);
                case K.Dict: return DictCall(self, n, a);
                case K.Str: return StrCall(self, n, a);
                case K.Vec2: return VecCall(self, n, a);
                case K.Vec2i: return VeciCall(self, n, a);
                case K.Color: return ColCall(self, n, a);
                case K.Rect2: return RectCall(self, n, a);
                case K.Fn: return FnCall(self, n, a);
                case K.Script: return Gd.ScriptCall(self, n, a);
                case K.Int: case K.Float: return NumCall(self, n, a);
                case K.Null: throw new GdError("Volání metody '" + n + "' na null");
                case K.Bool: if (n == "to_string") return Gv.S(self.ToStr()); break;
            }
            throw NoMethod(self, n);
        }

        // ---------- čísla ----------
        static Gv NumCall(Gv s, string n, Gv[] a)
        {
            switch (n)
            {
                case "is_nan": return Gv.B(double.IsNaN(s.d));
                case "is_inf": return Gv.B(double.IsInfinity(s.d));
                case "to_string": return Gv.S(s.ToStr());
                case "clamp": return Gd.clamp(s, A(a, 0), A(a, 1));
            }
            throw NoMethod(s, n);
        }

        // ---------- Callable ----------
        static Gv FnCall(Gv s, string n, Gv[] a)
        {
            var f = (GdFn)s.o;
            switch (n)
            {
                case "call": return f.f(a);
                case "callv": { var l = A(a, 0).Arr; return f.f(l != null ? l.l.ToArray() : new Gv[0]); }
                case "bind": { var bound = a; return Gv.Fn(x => { var all = new Gv[x.Length + bound.Length]; Array.Copy(x, all, x.Length); Array.Copy(bound, 0, all, x.Length, bound.Length); return f.f(all); }); }
                case "is_valid": return Gv.B(true);
                case "is_null": return Gv.B(false);
            }
            throw NoMethod(s, n);
        }

        // ---------- pole ----------
        static int Norm(int i, int count) { return i < 0 ? i + count : i; }

        static void StableSort(List<Gv> l, Comparison<Gv> cmp)
        {
            var arr = l.ToArray();
            var tmp = new Gv[arr.Length];
            MergeSort(arr, tmp, 0, arr.Length, cmp);
            l.Clear(); l.AddRange(arr);
        }
        static void MergeSort(Gv[] a, Gv[] t, int lo, int hi, Comparison<Gv> cmp)
        {
            if (hi - lo < 2) return;
            int mid = (lo + hi) / 2;
            MergeSort(a, t, lo, mid, cmp); MergeSort(a, t, mid, hi, cmp);
            int i = lo, j = mid, k = lo;
            while (i < mid && j < hi) t[k++] = cmp(a[j], a[i]) < 0 ? a[j++] : a[i++];
            while (i < mid) t[k++] = a[i++];
            while (j < hi) t[k++] = a[j++];
            Array.Copy(t, lo, a, lo, hi - lo);
        }

        static Gv ArrCall(Gv s, string n, Gv[] a)
        {
            var arr = (GdArray)s.o; var l = arr.l;
            switch (n)
            {
                case "append": case "push_back": l.Add(A(a, 0)); return Gv.Null;
                case "append_array": { var o = A(a, 0).Arr; if (o != null) l.AddRange(new List<Gv>(o.l)); return Gv.Null; }
                case "push_front": l.Insert(0, A(a, 0)); return Gv.Null;
                case "pop_back": { if (l.Count == 0) return Gv.Null; var v = l[l.Count - 1]; l.RemoveAt(l.Count - 1); return v; }
                case "pop_front": { if (l.Count == 0) return Gv.Null; var v = l[0]; l.RemoveAt(0); return v; }
                case "pop_at": { int i = Norm((int)A(a, 0).d, l.Count); if (i < 0 || i >= l.Count) return Gv.Null; var v = l[i]; l.RemoveAt(i); return v; }
                case "insert": { int i = Norm((int)A(a, 0).d, l.Count); l.Insert(Math.Max(0, Math.Min(i, l.Count)), A(a, 1)); return Gv.Null; }
                case "remove_at": { int i = Norm((int)A(a, 0).d, l.Count); if (i >= 0 && i < l.Count) l.RemoveAt(i); return Gv.Null; }
                case "erase": { for (int i = 0; i < l.Count; i++) if (Gv.Eq(l[i], A(a, 0))) { l.RemoveAt(i); break; } return Gv.Null; }
                case "clear": l.Clear(); return Gv.Null;
                case "size": return Gv.I(l.Count);
                case "is_empty": return Gv.B(l.Count == 0);
                case "has": return Gv.B(Gv.In(A(a, 0), s));
                case "find": { int from = a.Length > 1 ? (int)a[1].d : 0; for (int i = from; i < l.Count; i++) if (Gv.Eq(l[i], A(a, 0))) return Gv.I(i); return Gv.I(-1); }
                case "rfind": { for (int i = l.Count - 1; i >= 0; i--) if (Gv.Eq(l[i], A(a, 0))) return Gv.I(i); return Gv.I(-1); }
                case "count": { int c = 0; foreach (var v in l) if (Gv.Eq(v, A(a, 0))) c++; return Gv.I(c); }
                case "front": return l.Count > 0 ? l[0] : Gv.Null;
                case "back": return l.Count > 0 ? l[l.Count - 1] : Gv.Null;
                case "slice":
                    {
                        int b = Norm((int)A(a, 0).d, l.Count); int e = a.Length > 1 ? Norm((int)a[1].d, l.Count) : l.Count;
                        b = Math.Max(0, Math.Min(b, l.Count)); e = Math.Max(b, Math.Min(e, l.Count));
                        var r = new List<Gv>(); for (int i = b; i < e; i++) r.Add(l[i]); return Gv.A(new GdArray(r));
                    }
                case "sort": StableSort(l, Gv.Cmp); return Gv.Null;
                case "sort_custom":
                    {
                        var f = A(a, 0);
                        StableSort(l, (x, y) => f.Call(x, y).T() ? -1 : (f.Call(y, x).T() ? 1 : 0)); return Gv.Null;
                    }
                case "reverse": l.Reverse(); return Gv.Null;
                case "duplicate": return s.Duplicate(a.Length > 0 && A(a, 0).T());
                case "map": { var f = A(a, 0); var r = new List<Gv>(l.Count); foreach (var v in l.ToArray()) r.Add(f.Call(v)); return Gv.A(new GdArray(r)); }
                case "filter": { var f = A(a, 0); var r = new List<Gv>(); foreach (var v in l.ToArray()) if (f.Call(v).T()) r.Add(v); return Gv.A(new GdArray(r)); }
                case "any": { var f = A(a, 0); foreach (var v in l.ToArray()) if (f.Call(v).T()) return Gv.B(true); return Gv.B(false); }
                case "all": { var f = A(a, 0); foreach (var v in l.ToArray()) if (!f.Call(v).T()) return Gv.B(false); return Gv.B(true); }
                case "reduce": { var f = A(a, 0); Gv acc = A(a, 1); int st = 0; if (a.Length < 2) { if (l.Count == 0) return Gv.Null; acc = l[0]; st = 1; } for (int i = st; i < l.Count; i++) acc = f.Call(acc, l[i]); return acc; }
                case "min": { if (l.Count == 0) return Gv.Null; var m = l[0]; foreach (var v in l) if (Gv.Lt(v, m)) m = v; return m; }
                case "max": { if (l.Count == 0) return Gv.Null; var m = l[0]; foreach (var v in l) if (Gv.Lt(m, v)) m = v; return m; }
                case "resize": { int c = (int)A(a, 0).d; while (l.Count > c) l.RemoveAt(l.Count - 1); while (l.Count < c) l.Add(Gv.Null); return Gv.I(0); }
                case "fill": { for (int i = 0; i < l.Count; i++) l[i] = A(a, 0); return Gv.Null; }
                case "shuffle": { for (int i = l.Count - 1; i > 0; i--) { int j = Gd.Rng.Next(i + 1); var t = l[i]; l[i] = l[j]; l[j] = t; } return Gv.Null; }
                case "pick_random": return l.Count > 0 ? l[Gd.Rng.Next(l.Count)] : Gv.Null;
                case "hash": return Gv.I(l.Count);
                case "get": { int i = Norm((int)A(a, 0).d, l.Count); return i >= 0 && i < l.Count ? l[i] : Gv.Null; }
                case "set": { int i = Norm((int)A(a, 0).d, l.Count); if (i >= 0 && i < l.Count) l[i] = A(a, 1); return Gv.Null; }
                case "to_string": return Gv.S(s.ToStr());
                case "join": { var sb = new StringBuilder(); var sep = A(a, 0).Str ?? ""; for (int i = 0; i < l.Count; i++) { if (i > 0) sb.Append(sep); sb.Append(l[i].ToStr()); } return Gv.S(sb.ToString()); }
                case "bsearch": { for (int i = 0; i < l.Count; i++) if (!Gv.Lt(l[i], A(a, 0))) return Gv.I(i); return Gv.I(l.Count); }
                case "sum": { Gv acc = Gv.I(0); foreach (var v in l) acc = acc + v; return acc; }
                case "to_byte_array": return s;
                case "encode_s16": { int off = (int)A(a, 0).d; long v = (long)A(a, 1).d; if (off >= 0 && off + 1 < l.Count) { l[off] = Gv.I(v & 0xff); l[off + 1] = Gv.I((v >> 8) & 0xff); } return Gv.Null; }
                case "encode_u8": { int off = (int)A(a, 0).d; if (off >= 0 && off < l.Count) l[off] = Gv.I((long)A(a, 1).d & 0xff); return Gv.Null; }
            }
            throw NoMethod(s, n);
        }

        // ---------- slovník ----------
        static Gv DictCall(Gv s, string n, Gv[] a)
        {
            var d = (GdDict)s.o;
            switch (n)
            {
                case "get": { Gv v; if (d.TryGet(A(a, 0), out v)) return v; return A(a, 1); }
                case "get_or_add": { Gv v; if (d.TryGet(A(a, 0), out v)) return v; d.Set(A(a, 0), A(a, 1)); return A(a, 1); }
                case "has": return Gv.B(d.Has(A(a, 0)));
                case "has_all": { foreach (var k in A(a, 0).Arr.l) if (!d.Has(k)) return Gv.B(false); return Gv.B(true); }
                case "keys": return Gv.A(new GdArray(new List<Gv>(d.keys)));
                case "values": return Gv.A(new GdArray(new List<Gv>(d.vals)));
                case "erase": return Gv.B(d.Remove(A(a, 0)));
                case "size": return Gv.I(d.Count);
                case "is_empty": return Gv.B(d.Count == 0);
                case "clear": d.Clear(); return Gv.Null;
                case "duplicate": return s.Duplicate(a.Length > 0 && A(a, 0).T());
                case "merge":
                    {
                        var o = A(a, 0).Dict; bool over = a.Length > 1 && a[1].T();
                        if (o != null) for (int i = 0; i < o.keys.Count; i++) if (over || !d.Has(o.keys[i])) d.Set(o.keys[i], o.vals[i]);
                        return Gv.Null;
                    }
                case "find_key": { for (int i = 0; i < d.keys.Count; i++) if (Gv.Eq(d.vals[i], A(a, 0))) return d.keys[i]; return Gv.Null; }
                case "to_string": return Gv.S(s.ToStr());
                case "hash": return Gv.I(d.Count);
            }
            throw NoMethod(s, n);
        }

        // ---------- řetězec ----------
        static Gv StrCall(Gv s, string n, Gv[] a)
        {
            string t = (string)s.o;
            switch (n)
            {
                case "length": return Gv.I(t.Length);
                case "is_empty": return Gv.B(t.Length == 0);
                case "begins_with": return Gv.B(t.StartsWith(A(a, 0).Str, StringComparison.Ordinal));
                case "ends_with": return Gv.B(t.EndsWith(A(a, 0).Str, StringComparison.Ordinal));
                case "contains": return Gv.B(t.Contains(A(a, 0).Str));
                case "containsn": return Gv.B(t.ToLowerInvariant().Contains(A(a, 0).Str.ToLowerInvariant()));
                case "to_upper": return Gv.S(t.ToUpperInvariant());
                case "to_lower": return Gv.S(t.ToLowerInvariant());
                case "capitalize": return Gv.S(t.Length > 0 ? char.ToUpperInvariant(t[0]) + t.Substring(1) : t);
                case "replace": return Gv.S(t.Replace(A(a, 0).Str, A(a, 1).Str));
                case "replacen": return Gv.S(Replacen(t, A(a, 0).Str, A(a, 1).Str));
                case "strip_edges": return Gv.S(t.Trim());
                case "lstrip": return Gv.S(t.TrimStart(A(a, 0).Str.ToCharArray()));
                case "rstrip": return Gv.S(t.TrimEnd(A(a, 0).Str.ToCharArray()));
                case "trim_prefix": return Gv.S(t.StartsWith(A(a, 0).Str, StringComparison.Ordinal) ? t.Substring(A(a, 0).Str.Length) : t);
                case "trim_suffix": return Gv.S(t.EndsWith(A(a, 0).Str, StringComparison.Ordinal) ? t.Substring(0, t.Length - A(a, 0).Str.Length) : t);
                case "substr": { int b = (int)A(a, 0).d; int len = a.Length > 1 ? (int)a[1].d : -1; if (b > t.Length) b = t.Length; if (len < 0 || b + len > t.Length) len = t.Length - b; return Gv.S(t.Substring(b, len)); }
                case "left": { int c = Math.Min(t.Length, (int)A(a, 0).d); return Gv.S(t.Substring(0, c)); }
                case "right": { int c = Math.Min(t.Length, (int)A(a, 0).d); return Gv.S(t.Substring(t.Length - c)); }
                case "find": { int from = a.Length > 1 ? (int)a[1].d : 0; return Gv.I(t.IndexOf(A(a, 0).Str, from, StringComparison.Ordinal)); }
                case "rfind": return Gv.I(t.LastIndexOf(A(a, 0).Str, StringComparison.Ordinal));
                case "count": { var sub = A(a, 0).Str; if (sub.Length == 0) return Gv.I(0); int c = 0, i = 0; while ((i = t.IndexOf(sub, i, StringComparison.Ordinal)) >= 0) { c++; i += sub.Length; } return Gv.I(c); }
                case "split":
                    {
                        var sep = A(a, 0).Str; bool allowEmpty = a.Length < 2 || a[1].T();
                        var r = new List<Gv>();
                        if (sep.Length == 0) { foreach (char c in t) r.Add(Gv.S(c.ToString())); }
                        else foreach (var part in t.Split(new string[] { sep }, StringSplitOptions.None)) if (allowEmpty || part.Length > 0) r.Add(Gv.S(part));
                        return Gv.A(new GdArray(r));
                    }
                case "split_floats":
                    {
                        var r = new List<Gv>(); foreach (var part in t.Split(new string[] { A(a, 0).Str }, StringSplitOptions.RemoveEmptyEntries)) r.Add(Gv.F(Gd.ParseDouble(part)));
                        return Gv.A(new GdArray(r));
                    }
                case "join": { var sb = new StringBuilder(); bool first = true; foreach (var v in A(a, 0).Iter()) { if (!first) sb.Append(t); sb.Append(v.ToStr()); first = false; } return Gv.S(sb.ToString()); }
                case "to_int": { return Gv.I((long)Gd.ParseLeadingInt(t)); }
                case "to_float": return Gv.F(Gd.ParseDouble(t));
                case "is_valid_int": { long x; return Gv.B(long.TryParse(t, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out x)); }
                case "is_valid_float": { double x; return Gv.B(double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out x)); }
                case "pad_zeros": return Gv.S(t.PadLeft((int)A(a, 0).d, '0'));
                case "pad_decimals": return Gv.S(t);
                case "repeat": { var sb = new StringBuilder(); for (int i = 0; i < (int)A(a, 0).d; i++) sb.Append(t); return Gv.S(sb.ToString()); }
                case "format": return Gv.S(Gd.FormatNamed(t, A(a, 0)));
                case "to_string": return s;
                case "get_slice": { var parts = t.Split(new string[] { A(a, 0).Str }, StringSplitOptions.None); int i = (int)A(a, 1).d; return Gv.S(i >= 0 && i < parts.Length ? parts[i] : ""); }
                case "unicode_at": return Gv.I((int)t[(int)A(a, 0).d]);
                case "casecmp_to": return Gv.I(string.CompareOrdinal(t, A(a, 0).Str));
                case "naturalnocasecmp_to": return Gv.I(string.Compare(t, A(a, 0).Str, StringComparison.OrdinalIgnoreCase));
                case "hash": return Gv.I(t.GetHashCode());
                case "c_escape": return s;
                case "to_utf8_buffer": return Gv.A(new GdArray());
                case "is_absolute_path": return Gv.B(t.StartsWith("/") || t.Contains(":"));
                case "get_file": { int i = Math.Max(t.LastIndexOf('/'), t.LastIndexOf('\\')); return Gv.S(i >= 0 ? t.Substring(i + 1) : t); }
                case "get_extension": { int i = t.LastIndexOf('.'); return Gv.S(i >= 0 ? t.Substring(i + 1) : ""); }
                case "begins_with_n": return Gv.B(false);
                case "sha256_text": return Gv.S(t);
                case "md5_text": return Gv.S(t);
                case "uri_encode": return Gv.S(Uri.EscapeDataString(t));
            }
            throw NoMethod(s, n);
        }

        static string Replacen(string t, string from, string to)
        {
            if (from.Length == 0) return t;
            var sb = new StringBuilder(); int i = 0;
            while (true)
            {
                int j = t.IndexOf(from, i, StringComparison.OrdinalIgnoreCase);
                if (j < 0) { sb.Append(t.Substring(i)); break; }
                sb.Append(t, i, j - i).Append(to); i = j + from.Length;
            }
            return sb.ToString();
        }

        // ---------- Vector2 ----------
        static Gv VecCall(Gv s, string n, Gv[] a)
        {
            double x = s.d, y = s.e;
            switch (n)
            {
                case "length": return Gv.F(Math.Sqrt(x * x + y * y));
                case "length_squared": return Gv.F(x * x + y * y);
                case "normalized": { double l = Math.Sqrt(x * x + y * y); return l > 0 ? Gv.Vec(x / l, y / l) : Gv.Vec(0, 0); }
                case "distance_to": { var o = A(a, 0); return Gv.F(Math.Sqrt((x - o.d) * (x - o.d) + (y - o.e) * (y - o.e))); }
                case "distance_squared_to": { var o = A(a, 0); return Gv.F((x - o.d) * (x - o.d) + (y - o.e) * (y - o.e)); }
                case "direction_to": { var o = A(a, 0); double dx = o.d - x, dy = o.e - y; double l = Math.Sqrt(dx * dx + dy * dy); return l > 0 ? Gv.Vec(dx / l, dy / l) : Gv.Vec(0, 0); }
                case "dot": { var o = A(a, 0); return Gv.F(x * o.d + y * o.e); }
                case "cross": { var o = A(a, 0); return Gv.F(x * o.e - y * o.d); }
                case "lerp": { var o = A(a, 0); double t = A(a, 1).d; return Gv.Vec(x + (o.d - x) * t, y + (o.e - y) * t); }
                case "rotated": { double c = Math.Cos(A(a, 0).d), sn = Math.Sin(A(a, 0).d); return Gv.Vec(x * c - y * sn, x * sn + y * c); }
                case "angle": return Gv.F(Math.Atan2(y, x));
                case "angle_to": { var o = A(a, 0); return Gv.F(Math.Atan2(x * o.e - y * o.d, x * o.d + y * o.e)); }
                case "angle_to_point": { var o = A(a, 0); return Gv.F(Math.Atan2(o.e - y, o.d - x)); }
                case "abs": return Gv.Vec(Math.Abs(x), Math.Abs(y));
                case "round": return Gv.Vec(Math.Round(x, MidpointRounding.AwayFromZero), Math.Round(y, MidpointRounding.AwayFromZero));
                case "floor": return Gv.Vec(Math.Floor(x), Math.Floor(y));
                case "ceil": return Gv.Vec(Math.Ceiling(x), Math.Ceiling(y));
                case "min": { var o = A(a, 0); return Gv.Vec(Math.Min(x, o.d), Math.Min(y, o.e)); }
                case "max": { var o = A(a, 0); return Gv.Vec(Math.Max(x, o.d), Math.Max(y, o.e)); }
                case "clamp": { var lo = A(a, 0); var hi = A(a, 1); return Gv.Vec(Math.Max(lo.d, Math.Min(hi.d, x)), Math.Max(lo.e, Math.Min(hi.e, y))); }
                case "snapped": { var st = A(a, 0); return Gv.Vec(st.d == 0 ? x : Math.Floor(x / st.d + 0.5) * st.d, st.e == 0 ? y : Math.Floor(y / st.e + 0.5) * st.e); }
                case "orthogonal": return Gv.Vec(y, -x);
                case "move_toward": { var o = A(a, 0); double dl = A(a, 1).d; double dx = o.d - x, dy = o.e - y; double l = Math.Sqrt(dx * dx + dy * dy); if (l <= dl || l < 1e-9) return o; return Gv.Vec(x + dx / l * dl, y + dy / l * dl); }
                case "limit_length": { double mx = a.Length > 0 ? A(a, 0).d : 1.0; double l = Math.Sqrt(x * x + y * y); return l > mx && l > 0 ? Gv.Vec(x / l * mx, y / l * mx) : s; }
                case "is_equal_approx": { var o = A(a, 0); return Gv.B(Math.Abs(x - o.d) < 1e-5 && Math.Abs(y - o.e) < 1e-5); }
                case "is_zero_approx": return Gv.B(Math.Abs(x) < 1e-5 && Math.Abs(y) < 1e-5);
                case "reflect": { var nn = A(a, 0); double dt = x * nn.d + y * nn.e; return Gv.Vec(2 * nn.d * dt - x, 2 * nn.e * dt - y); }
                case "slide": { var nn = A(a, 0); double dt = x * nn.d + y * nn.e; return Gv.Vec(x - nn.d * dt, y - nn.e * dt); }
                case "project": { var o = A(a, 0); double l2 = o.d * o.d + o.e * o.e; double k = l2 > 0 ? (x * o.d + y * o.e) / l2 : 0; return Gv.Vec(o.d * k, o.e * k); }
                case "sign": return Gv.Vec(Math.Sign(x), Math.Sign(y));
                case "to_string": return Gv.S(s.ToStr());
                case "aspect": return Gv.F(y != 0 ? x / y : 0);
            }
            throw NoMethod(s, n);
        }

        static Gv VeciCall(Gv s, string n, Gv[] a)
        {
            switch (n)
            {
                case "length": return Gv.F(Math.Sqrt(s.d * s.d + s.e * s.e));
                case "to_string": return Gv.S(s.ToStr());
            }
            return VecCall(Gv.Vec(s.d, s.e), n, a);
        }

        // ---------- Color ----------
        static double Cl01(double v) { return v < 0 ? 0 : (v > 1 ? 1 : v); }
        static Gv ColCall(Gv s, string n, Gv[] a)
        {
            var c = (GdColor)s.o;
            switch (n)
            {
                case "lerp": { var o = A(a, 0).Cl; double t = A(a, 1).d; return Gv.Col(c.r + (o.r - c.r) * t, c.g + (o.g - c.g) * t, c.b + (o.b - c.b) * t, c.a + (o.a - c.a) * t); }
                case "darkened": { double k = 1 - A(a, 0).d; return Gv.Col(c.r * k, c.g * k, c.b * k, c.a); }
                case "lightened": { double k = A(a, 0).d; return Gv.Col(c.r + (1 - c.r) * k, c.g + (1 - c.g) * k, c.b + (1 - c.b) * k, c.a); }
                case "inverted": return Gv.Col(1 - c.r, 1 - c.g, 1 - c.b, c.a);
                case "blend": { var o = A(a, 0).Cl; double sa = o.a; double ra = c.a + sa * (1 - c.a); if (ra <= 0) return Gv.Col(0, 0, 0, 0); return Gv.Col((c.r * c.a * (1 - sa) + o.r * sa) / ra, (c.g * c.a * (1 - sa) + o.g * sa) / ra, (c.b * c.a * (1 - sa) + o.b * sa) / ra, ra); }
                case "to_html": { bool wa = a.Length == 0 || a[0].T(); string h = ((int)Math.Round(Cl01(c.r) * 255)).ToString("x2") + ((int)Math.Round(Cl01(c.g) * 255)).ToString("x2") + ((int)Math.Round(Cl01(c.b) * 255)).ToString("x2"); if (wa) h += ((int)Math.Round(Cl01(c.a) * 255)).ToString("x2"); return Gv.S(h); }
                case "get_luminance": return Gv.F(0.2126 * c.r + 0.7152 * c.g + 0.0722 * c.b);
                case "is_equal_approx": { var o = A(a, 0).Cl; return Gv.B(Math.Abs(c.r - o.r) < 1e-5 && Math.Abs(c.g - o.g) < 1e-5 && Math.Abs(c.b - o.b) < 1e-5 && Math.Abs(c.a - o.a) < 1e-5); }
                case "to_string": return Gv.S(s.ToStr());
                case "to_rgba32": return Gv.I(((long)Math.Round(Cl01(c.r) * 255) << 24) | ((long)Math.Round(Cl01(c.g) * 255) << 16) | ((long)Math.Round(Cl01(c.b) * 255) << 8) | (long)Math.Round(Cl01(c.a) * 255));
            }
            throw NoMethod(s, n);
        }

        // ---------- Rect2 ----------
        static Gv RectCall(Gv s, string n, Gv[] a)
        {
            var r = (GdRect)s.o;
            switch (n)
            {
                case "has_point": { var p = A(a, 0); return Gv.B(p.d >= r.x && p.e >= r.y && p.d < r.x + r.w && p.e < r.y + r.h); }
                case "grow": { double g = A(a, 0).d; return Gv.Rect(r.x - g, r.y - g, r.w + 2 * g, r.h + 2 * g); }
                case "grow_individual": { double l = A(a, 0).d, t = A(a, 1).d, rr = A(a, 2).d, b = A(a, 3).d; return Gv.Rect(r.x - l, r.y - t, r.w + l + rr, r.h + t + b); }
                case "get_center": return Gv.Vec(r.x + r.w * 0.5, r.y + r.h * 0.5);
                case "intersects": { var o = A(a, 0).Rc; return Gv.B(r.x < o.x + o.w && r.x + r.w > o.x && r.y < o.y + o.h && r.y + r.h > o.y); }
                case "intersection": { var o = A(a, 0).Rc; double x0 = Math.Max(r.x, o.x), y0 = Math.Max(r.y, o.y), x1 = Math.Min(r.x + r.w, o.x + o.w), y1 = Math.Min(r.y + r.h, o.y + o.h); return x1 > x0 && y1 > y0 ? Gv.Rect(x0, y0, x1 - x0, y1 - y0) : Gv.Rect(0, 0, 0, 0); }
                case "merge": { var o = A(a, 0).Rc; double x0 = Math.Min(r.x, o.x), y0 = Math.Min(r.y, o.y), x1 = Math.Max(r.x + r.w, o.x + o.w), y1 = Math.Max(r.y + r.h, o.y + o.h); return Gv.Rect(x0, y0, x1 - x0, y1 - y0); }
                case "expand": { var p = A(a, 0); double x0 = Math.Min(r.x, p.d), y0 = Math.Min(r.y, p.e), x1 = Math.Max(r.x + r.w, p.d), y1 = Math.Max(r.y + r.h, p.e); return Gv.Rect(x0, y0, x1 - x0, y1 - y0); }
                case "encloses": { var o = A(a, 0).Rc; return Gv.B(o.x >= r.x && o.y >= r.y && o.x + o.w <= r.x + r.w && o.y + o.h <= r.y + r.h); }
                case "abs": return Gv.Rect(r.w < 0 ? r.x + r.w : r.x, r.h < 0 ? r.y + r.h : r.y, Math.Abs(r.w), Math.Abs(r.h));
                case "get_area": return Gv.F(r.w * r.h);
                case "get_end": return Gv.Vec(r.x + r.w, r.y + r.h);
                case "to_string": return Gv.S(s.ToStr());
            }
            throw NoMethod(s, n);
        }
    }
}
