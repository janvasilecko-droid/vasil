// Běhová knihovna překladače GDScript -> C#: dynamická hodnota Gv (obdoba Variantu z Godotu).
// Čistý C# bez Unity (testuje se v CI přes dotnet), kompatibilní s C# 9 (Unity 6).
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Pivovar.Gd
{
    public enum K : byte { Null = 0, Bool, Int, Float, Str, Vec2, Vec2i, Rect2, Color, Arr, Dict, Obj, Fn, Script, Any }

    public sealed class GdFn
    {
        public Func<Gv[], Gv> f;
        public GdFn(Func<Gv[], Gv> f) { this.f = f; }
    }

    public sealed class GdColor
    {
        public readonly double r, g, b, a;
        public GdColor(double r, double g, double b, double a) { this.r = r; this.g = g; this.b = b; this.a = a; }
    }

    public sealed class GdRect
    {
        public readonly double x, y, w, h;
        public GdRect(double x, double y, double w, double h) { this.x = x; this.y = y; this.w = w; this.h = h; }
    }

    public sealed class GdArray
    {
        public List<Gv> l;
        public string typed;   // "PackedVector2Array" apod. (jen informativně)
        public GdArray() { l = new List<Gv>(); }
        public GdArray(List<Gv> list) { l = list; }
        public int Count { get { return l.Count; } }
    }

    public sealed class GdDict
    {
        public readonly List<Gv> keys = new List<Gv>();
        public readonly List<Gv> vals = new List<Gv>();
        readonly Dictionary<Gv, int> idx = new Dictionary<Gv, int>(GvComparer.I);
        public int Count { get { return keys.Count; } }
        public bool TryGet(Gv k, out Gv v)
        {
            int i;
            if (idx.TryGetValue(k, out i)) { v = vals[i]; return true; }
            v = default(Gv); return false;
        }
        public Gv Get(Gv k) { int i; return idx.TryGetValue(k, out i) ? vals[i] : default(Gv); }
        public bool Has(Gv k) { return idx.ContainsKey(k); }
        public void Set(Gv k, Gv v)
        {
            int i;
            if (idx.TryGetValue(k, out i)) { vals[i] = v; return; }
            idx[k] = keys.Count; keys.Add(k); vals.Add(v);
        }
        public bool Remove(Gv k)
        {
            int i;
            if (!idx.TryGetValue(k, out i)) return false;
            keys.RemoveAt(i); vals.RemoveAt(i); idx.Remove(k);
            for (int j = i; j < keys.Count; j++) idx[keys[j]] = j;
            return true;
        }
        public void Clear() { keys.Clear(); vals.Clear(); idx.Clear(); }
        public GdDict Dup(bool deep)
        {
            var d = new GdDict();
            for (int i = 0; i < keys.Count; i++) d.Set(keys[i], deep ? vals[i].Duplicate(true) : vals[i]);
            return d;
        }
    }

    public sealed class GvComparer : IEqualityComparer<Gv>
    {
        public static readonly GvComparer I = new GvComparer();
        public bool Equals(Gv a, Gv b) { return Gv.KeyEq(a, b); }
        public int GetHashCode(Gv v) { return v.KeyHash(); }
    }

    public struct Gv
    {
        public K k;
        public double d, e;
        public object o;

        public static readonly Gv Null = default(Gv);

        // ---------- konstruktory ----------
        public static Gv B(bool b) { Gv v; v.k = K.Bool; v.d = b ? 1 : 0; v.e = 0; v.o = null; return v; }
        public static Gv I(long i) { Gv v; v.k = K.Int; v.d = i; v.e = 0; v.o = null; return v; }
        public static Gv F(double f) { Gv v; v.k = K.Float; v.d = f; v.e = 0; v.o = null; return v; }
        public static Gv S(string s) { Gv v; v.k = s == null ? K.Null : K.Str; v.d = 0; v.e = 0; v.o = s; return v; }
        public static Gv Vec(double x, double y) { Gv v; v.k = K.Vec2; v.d = x; v.e = y; v.o = null; return v; }
        public static Gv Veci(double x, double y) { Gv v; v.k = K.Vec2i; v.d = Math.Truncate(x); v.e = Math.Truncate(y); v.o = null; return v; }
        public static Gv Col(double r, double g, double b, double a) { Gv v; v.k = K.Color; v.d = 0; v.e = 0; v.o = new GdColor(r, g, b, a); return v; }
        public static Gv Rect(double x, double y, double w, double h) { Gv v; v.k = K.Rect2; v.d = 0; v.e = 0; v.o = new GdRect(x, y, w, h); return v; }
        public static Gv A(GdArray a) { Gv v; v.k = K.Arr; v.d = 0; v.e = 0; v.o = a; return v; }
        public static Gv D(GdDict a) { Gv v; v.k = K.Dict; v.d = 0; v.e = 0; v.o = a; return v; }
        public static Gv O(object obj) { Gv v; v.k = obj == null ? K.Null : K.Obj; v.d = 0; v.e = 0; v.o = obj; return v; }
        public static Gv Fn(Func<Gv[], Gv> f) { Gv v; v.k = K.Fn; v.d = 0; v.e = 0; v.o = new GdFn(f); return v; }
        public static Gv Scr(object script) { Gv v; v.k = K.Script; v.d = 0; v.e = 0; v.o = script; return v; }

        public static implicit operator Gv(int i) { return I(i); }
        public static implicit operator Gv(long i) { return I(i); }
        public static implicit operator Gv(double f) { return F(f); }
        public static implicit operator Gv(float f) { return F(f); }
        public static implicit operator Gv(bool b) { return B(b); }
        public static implicit operator Gv(string s) { return S(s); }

        // ---------- typy ----------
        public bool IsNull { get { return k == K.Null; } }
        public bool IsNum { get { return k == K.Int || k == K.Float; } }
        public long L { get { return (long)d; } }
        public int N { get { return (int)d; } }
        public float Fl { get { return (float)d; } }
        public string Str { get { return o as string; } }
        public GdArray Arr { get { return o as GdArray; } }
        public GdDict Dict { get { return o as GdDict; } }
        public GdObject Obj { get { return o as GdObject; } }
        public GdColor Cl { get { return o as GdColor; } }
        public GdRect Rc { get { return o as GdRect; } }

        public bool T()
        {
            switch (k)
            {
                case K.Null: return false;
                case K.Bool: case K.Int: case K.Float: return d != 0;
                case K.Str: return ((string)o).Length > 0;
                case K.Vec2: case K.Vec2i: return d != 0 || e != 0;
                case K.Arr: return ((GdArray)o).l.Count > 0;
                case K.Dict: return ((GdDict)o).Count > 0;
                default: return o != null;
            }
        }

        public Gv AsF() { if (k == K.Float) return this; if (k == K.Int || k == K.Bool) return F(d); if (k == K.Str) return F(Gd.ParseDouble((string)o)); return this; }
        public Gv AsI() { if (k == K.Int) return this; if (k == K.Float) return I((long)Math.Truncate(d)); if (k == K.Bool) return I((long)d); if (k == K.Str) return I((long)Gd.ParseDouble((string)o)); return this; }
        public double Dbl() { return d; }

        // ---------- aritmetika ----------
        public static Gv operator +(Gv a, Gv b)
        {
            if (a.k == K.Int && b.k == K.Int) return I((long)a.d + (long)b.d);
            if (a.IsNum && b.IsNum) return F(a.d + b.d);
            if (a.k == K.Str && b.k == K.Str) return S((string)a.o + (string)b.o);
            if (a.k == K.Str) return S((string)a.o + b.ToStr());
            if (a.k == K.Vec2 && b.k == K.Vec2) return Vec(a.d + b.d, a.e + b.e);
            if (a.k == K.Vec2i && b.k == K.Vec2i) return Veci(a.d + b.d, a.e + b.e);
            if (a.k == K.Arr && b.k == K.Arr) { var l = new List<Gv>(a.Arr.l); l.AddRange(b.Arr.l); return A(new GdArray(l)); }
            if (a.k == K.Color && b.k == K.Color) { var x = a.Cl; var y = b.Cl; return Col(x.r + y.r, x.g + y.g, x.b + y.b, x.a + y.a); }
            if (a.k == K.Rect2 && b.k == K.Vec2) { var r = a.Rc; return Rect(r.x + b.d, r.y + b.e, r.w, r.h); }
            if (a.IsNull && b.IsNull) return Null;
            throw new GdError("Neplatné operandy pro +: " + a.k + ", " + b.k);
        }
        public static Gv operator -(Gv a, Gv b)
        {
            if (a.k == K.Int && b.k == K.Int) return I((long)a.d - (long)b.d);
            if (a.IsNum && b.IsNum) return F(a.d - b.d);
            if (a.k == K.Vec2 && b.k == K.Vec2) return Vec(a.d - b.d, a.e - b.e);
            if (a.k == K.Vec2i && b.k == K.Vec2i) return Veci(a.d - b.d, a.e - b.e);
            if (a.k == K.Color && b.k == K.Color) { var x = a.Cl; var y = b.Cl; return Col(x.r - y.r, x.g - y.g, x.b - y.b, x.a - y.a); }
            throw new GdError("Neplatné operandy pro -: " + a.k + ", " + b.k);
        }
        public static Gv operator -(Gv a)
        {
            if (a.k == K.Int) return I(-(long)a.d);
            if (a.k == K.Float) return F(-a.d);
            if (a.k == K.Vec2) return Vec(-a.d, -a.e);
            if (a.k == K.Vec2i) return Veci(-a.d, -a.e);
            throw new GdError("Neplatný operand pro unární -: " + a.k);
        }
        public static Gv operator *(Gv a, Gv b)
        {
            if (a.k == K.Int && b.k == K.Int) return I((long)a.d * (long)b.d);
            if (a.IsNum && b.IsNum) return F(a.d * b.d);
            if (a.k == K.Vec2 && b.IsNum) return Vec(a.d * b.d, a.e * b.d);
            if (a.IsNum && b.k == K.Vec2) return Vec(a.d * b.d, a.d * b.e);
            if (a.k == K.Vec2 && b.k == K.Vec2) return Vec(a.d * b.d, a.e * b.e);
            if (a.k == K.Vec2i && b.IsNum) return Veci(a.d * b.d, a.e * b.d);
            if (a.k == K.Color && b.IsNum) { var c = a.Cl; return Col(c.r * b.d, c.g * b.d, c.b * b.d, c.a * b.d); }
            if (a.k == K.Color && b.k == K.Color) { var c = a.Cl; var f = b.Cl; return Col(c.r * f.r, c.g * f.g, c.b * f.b, c.a * f.a); }
            if (a.k == K.Str && b.k == K.Int) { var sb = new StringBuilder(); for (long i = 0; i < (long)b.d; i++) sb.Append((string)a.o); return S(sb.ToString()); }
            throw new GdError("Neplatné operandy pro *: " + a.k + ", " + b.k);
        }
        public static Gv operator /(Gv a, Gv b)
        {
            if (a.k == K.Int && b.k == K.Int)
            {
                long bb = (long)b.d;
                if (bb == 0) throw new GdError("Dělení nulou");
                return I((long)a.d / bb);
            }
            if (a.IsNum && b.IsNum) return F(a.d / b.d);
            if (a.k == K.Vec2 && b.IsNum) return Vec(a.d / b.d, a.e / b.d);
            if (a.k == K.Vec2 && b.k == K.Vec2) return Vec(a.d / b.d, a.e / b.e);
            if (a.k == K.Vec2i && b.IsNum) return Veci(a.d / b.d, a.e / b.d);
            if (a.k == K.Color && b.IsNum) { var c = a.Cl; return Col(c.r / b.d, c.g / b.d, c.b / b.d, c.a / b.d); }
            throw new GdError("Neplatné operandy pro /: " + a.k + ", " + b.k);
        }
        public static Gv operator %(Gv a, Gv b)
        {
            if (a.k == K.Str) return S(Gd.Format((string)a.o, b));
            if (a.k == K.Int && b.k == K.Int)
            {
                long bb = (long)b.d;
                if (bb == 0) throw new GdError("Modulo nulou");
                return I((long)a.d % bb);
            }
            if (a.IsNum && b.IsNum) return F(a.d % b.d);
            throw new GdError("Neplatné operandy pro %: " + a.k + ", " + b.k);
        }
        public static Gv Pow(Gv a, Gv b)
        {
            if (a.k == K.Int && b.k == K.Int && b.d >= 0) { long r = 1; for (long i = 0; i < (long)b.d; i++) r *= (long)a.d; return I(r); }
            return F(Math.Pow(a.d, b.d));
        }
        public static Gv BitOr(Gv a, Gv b) { return I((long)a.d | (long)b.d); }
        public static Gv BitAnd(Gv a, Gv b) { return I((long)a.d & (long)b.d); }
        public static Gv BitXor(Gv a, Gv b) { return I((long)a.d ^ (long)b.d); }
        public static Gv BitNot(Gv a) { return I(~(long)a.d); }
        public static Gv Shl(Gv a, Gv b) { return I((long)a.d << (int)b.d); }
        public static Gv Shr(Gv a, Gv b) { return I((long)a.d >> (int)b.d); }

        // ---------- porovnání ----------
        public static bool Eq(Gv a, Gv b)
        {
            if (a.IsNum && b.IsNum) return a.d == b.d;
            if (a.k == K.Bool && b.k == K.Bool) return a.d == b.d;
            if (a.k == K.Bool && b.IsNum) return a.d == b.d;
            if (a.IsNum && b.k == K.Bool) return a.d == b.d;
            if (a.k != b.k)
            {
                if ((a.k == K.Vec2 && b.k == K.Vec2i) || (a.k == K.Vec2i && b.k == K.Vec2)) return a.d == b.d && a.e == b.e;
                return false;
            }
            switch (a.k)
            {
                case K.Null: return true;
                case K.Str: return string.Equals((string)a.o, (string)b.o, StringComparison.Ordinal);
                case K.Vec2: case K.Vec2i: return a.d == b.d && a.e == b.e;
                case K.Color: { var x = a.Cl; var y = b.Cl; return x.r == y.r && x.g == y.g && x.b == y.b && x.a == y.a; }
                case K.Rect2: { var x = a.Rc; var y = b.Rc; return x.x == y.x && x.y == y.y && x.w == y.w && x.h == y.h; }
                case K.Arr:
                    {
                        var x = a.Arr.l; var y = b.Arr.l;
                        if (ReferenceEquals(x, y)) return true;
                        if (x.Count != y.Count) return false;
                        for (int i = 0; i < x.Count; i++) if (!Eq(x[i], y[i])) return false;
                        return true;
                    }
                case K.Dict:
                    {
                        var x = a.Dict; var y = b.Dict;
                        if (ReferenceEquals(x, y)) return true;
                        if (x.Count != y.Count) return false;
                        for (int i = 0; i < x.keys.Count; i++)
                        {
                            Gv v; if (!y.TryGet(x.keys[i], out v) || !Eq(x.vals[i], v)) return false;
                        }
                        return true;
                    }
                default: return ReferenceEquals(a.o, b.o);
            }
        }
        public static bool Ne(Gv a, Gv b) { return !Eq(a, b); }
        public static bool Lt(Gv a, Gv b)
        {
            if (a.IsNum && b.IsNum) return a.d < b.d;
            if (a.k == K.Str && b.k == K.Str) return string.CompareOrdinal((string)a.o, (string)b.o) < 0;
            if (a.k == K.Vec2 && b.k == K.Vec2) return a.d < b.d || (a.d == b.d && a.e < b.e);
            if (a.k == K.Arr && b.k == K.Arr) return CmpArr(a.Arr, b.Arr) < 0;
            if (a.IsNull || b.IsNull) return false;
            if (a.k == K.Bool || b.k == K.Bool) return a.d < b.d;
            throw new GdError("Neplatné porovnání <: " + a.k + ", " + b.k);
        }
        public static bool Gt(Gv a, Gv b) { return Lt(b, a); }
        public static bool Le(Gv a, Gv b)
        {
            if (a.IsNum && b.IsNum) return a.d <= b.d;
            return Lt(a, b) || Eq(a, b);
        }
        public static bool Ge(Gv a, Gv b) { return Le(b, a); }
        static int CmpArr(GdArray a, GdArray b)
        {
            int n = Math.Min(a.Count, b.Count);
            for (int i = 0; i < n; i++)
            {
                if (Lt(a.l[i], b.l[i])) return -1;
                if (Lt(b.l[i], a.l[i])) return 1;
            }
            return a.Count.CompareTo(b.Count);
        }
        public static int Cmp(Gv a, Gv b) { return Lt(a, b) ? -1 : (Lt(b, a) ? 1 : 0); }

        public static bool In(Gv x, Gv c)
        {
            switch (c.k)
            {
                case K.Arr: foreach (var v in c.Arr.l) if (Eq(v, x)) return true; return false;
                case K.Dict: return c.Dict.Has(x);
                case K.Str: return x.k == K.Str && ((string)c.o).Contains((string)x.o);
                case K.Rect2: return false;
                default: return false;
            }
        }
        public static bool Is(Gv v, string t)
        {
            switch (t)
            {
                case "int": return v.k == K.Int;
                case "float": return v.k == K.Float;
                case "bool": return v.k == K.Bool;
                case "String": case "StringName": return v.k == K.Str;
                case "Dictionary": return v.k == K.Dict;
                case "Array": return v.k == K.Arr;
                case "Vector2": return v.k == K.Vec2;
                case "Vector2i": return v.k == K.Vec2i;
                case "Color": return v.k == K.Color;
                case "Rect2": return v.k == K.Rect2;
                case "Callable": return v.k == K.Fn;
                case "Object": case "RefCounted": return v.k == K.Obj;
                case "PackedVector2Array": return v.k == K.Arr;
                default:
                    if (v.k == K.Obj) return ((GdObject)v.o).IsA(t);
                    return false;
            }
        }

        // ---------- klíče slovníku ----------
        public static bool KeyEq(Gv a, Gv b)
        {
            if (a.k != b.k)
            {
                if (a.IsNum && b.IsNum) return a.d == b.d;   // 1 == 1.0 jako klíč (shovívavě)
                return false;
            }
            switch (a.k)
            {
                case K.Null: return true;
                case K.Bool: case K.Int: case K.Float: return a.d == b.d;
                case K.Str: return string.Equals((string)a.o, (string)b.o, StringComparison.Ordinal);
                case K.Vec2: case K.Vec2i: return a.d == b.d && a.e == b.e;
                case K.Arr: case K.Dict: case K.Color: case K.Rect2: return Eq(a, b);
                default: return ReferenceEquals(a.o, b.o);
            }
        }
        public int KeyHash()
        {
            switch (k)
            {
                case K.Null: return 0;
                case K.Bool: case K.Int: case K.Float: return d.GetHashCode();
                case K.Str: return ((string)o).GetHashCode();
                case K.Vec2: case K.Vec2i: return d.GetHashCode() * 31 + e.GetHashCode();
                case K.Arr: return ((GdArray)o).Count;
                case K.Dict: return ((GdDict)o).Count;
                default: return o == null ? 0 : System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(o);
            }
        }

        // ---------- přístup ----------
        public Gv G(string name)
        {
            switch (k)
            {
                case K.Dict:
                    {
                        Gv v; if (((GdDict)o).TryGet(S(name), out v)) return v;
                        return default(Gv);
                    }
                case K.Obj: return ((GdObject)o).Get(name);
                case K.Vec2: case K.Vec2i:
                    if (name == "x") return k == K.Vec2 ? F(d) : I((long)d);
                    if (name == "y") return k == K.Vec2 ? F(e) : I((long)e);
                    break;
                case K.Color:
                    {
                        var c = (GdColor)o;
                        switch (name) { case "r": return F(c.r); case "g": return F(c.g); case "b": return F(c.b); case "a": return F(c.a);
                            case "r8": return I((long)Math.Round(c.r * 255)); case "g8": return I((long)Math.Round(c.g * 255)); case "b8": return I((long)Math.Round(c.b * 255)); case "a8": return I((long)Math.Round(c.a * 255));
                            case "h": case "s": case "v":
                                {
                                    double mx = Math.Max(c.r, Math.Max(c.g, c.b)), mn = Math.Min(c.r, Math.Min(c.g, c.b)), dl = mx - mn, h = 0;
                                    if (dl != 0) { if (mx == c.r) h = ((c.g - c.b) / dl) % 6; else if (mx == c.g) h = (c.b - c.r) / dl + 2; else h = (c.r - c.g) / dl + 4; h /= 6; if (h < 0) h += 1; }
                                    return F(name == "h" ? h : (name == "s" ? (mx == 0 ? 0 : dl / mx) : mx));
                                } }
                        break;
                    }
                case K.Rect2:
                    {
                        var r = (GdRect)o;
                        switch (name)
                        {
                            case "position": return Vec(r.x, r.y);
                            case "size": return Vec(r.w, r.h);
                            case "end": return Vec(r.x + r.w, r.y + r.h);
                        }
                        break;
                    }
                case K.Script: return Gd.ScriptGet(this, name);
                case K.Null: throw new GdError("Čtení vlastnosti '" + name + "' z null");
            }
            throw new GdError("Neznámá vlastnost '" + name + "' typu " + k);
        }

        public Gv S(string name, Gv val)
        {
            switch (k)
            {
                case K.Dict: ((GdDict)o).Set(S(name), val); return this;
                case K.Obj: ((GdObject)o).Set(name, val); return this;
                case K.Vec2: if (name == "x") return Vec(val.d, e); if (name == "y") return Vec(d, val.d); break;
                case K.Vec2i: if (name == "x") return Veci(val.d, e); if (name == "y") return Veci(d, val.d); break;
                case K.Color:
                    {
                        var c = (GdColor)o;
                        switch (name)
                        {
                            case "r": return Col(val.d, c.g, c.b, c.a); case "g": return Col(c.r, val.d, c.b, c.a);
                            case "b": return Col(c.r, c.g, val.d, c.a); case "a": return Col(c.r, c.g, c.b, val.d);
                        }
                        break;
                    }
                case K.Rect2:
                    {
                        var r = (GdRect)o;
                        if (name == "position") return Rect(val.d, val.e, r.w, r.h);
                        if (name == "size") return Rect(r.x, r.y, val.d, val.e);
                        break;
                    }
                case K.Null: throw new GdError("Zápis vlastnosti '" + name + "' do null");
            }
            throw new GdError("Nelze nastavit vlastnost '" + name + "' typu " + k);
        }

        public Gv I(Gv idx)
        {
            switch (k)
            {
                case K.Arr:
                    {
                        var l = ((GdArray)o).l; int i = (int)idx.d; if (i < 0) i += l.Count;
                        if (i < 0 || i >= l.Count) throw new GdError("Index mimo pole: " + i + " / " + l.Count);
                        return l[i];
                    }
                case K.Dict:
                    {
                        Gv v; if (((GdDict)o).TryGet(idx, out v)) return v;
                        return default(Gv);
                    }
                case K.Str: { var s = (string)o; int i = (int)idx.d; if (i < 0) i += s.Length; return S(s.Substring(i, 1)); }
                case K.Vec2: case K.Vec2i: return (int)idx.d == 0 ? (k == K.Vec2 ? F(d) : I((long)d)) : (k == K.Vec2 ? F(e) : I((long)e));
                case K.Color: { var c = (GdColor)o; switch ((int)idx.d) { case 0: return F(c.r); case 1: return F(c.g); case 2: return F(c.b); default: return F(c.a); } }
                case K.Obj: return ((GdObject)o).Invoke("_get_index", new Gv[] { idx });
                case K.Null: throw new GdError("Indexování null");
            }
            throw new GdError("Nelze indexovat typ " + k);
        }

        public Gv SI(Gv idx, Gv val)
        {
            switch (k)
            {
                case K.Arr:
                    {
                        var l = ((GdArray)o).l; int i = (int)idx.d; if (i < 0) i += l.Count;
                        if (i < 0 || i >= l.Count) throw new GdError("Zápis mimo pole: " + i + " / " + l.Count);
                        l[i] = val; return this;
                    }
                case K.Dict: ((GdDict)o).Set(idx, val); return this;
                case K.Vec2: return (int)idx.d == 0 ? Vec(val.d, e) : Vec(d, val.d);
                case K.Vec2i: return (int)idx.d == 0 ? Veci(val.d, e) : Veci(d, val.d);
                case K.Null: throw new GdError("Zápis do null indexu");
            }
            throw new GdError("Nelze zapisovat do indexu typu " + k);
        }

        public Gv C(string name, params Gv[] a) { return GvMethods.Call(this, name, a); }
        public Gv Call(params Gv[] a)
        {
            if (k == K.Fn) return ((GdFn)o).f(a);
            if (k == K.Obj) return ((GdObject)o).Invoke("call", a);
            if (k == K.Null) return Null;
            throw new GdError("Volání nevolatelné hodnoty typu " + k);
        }

        public IEnumerable<Gv> Iter()
        {
            switch (k)
            {
                case K.Int: { long n = (long)d; for (long i = 0; i < n; i++) yield return I(i); yield break; }
                case K.Float: { long n = (long)d; for (long i = 0; i < n; i++) yield return I(i); yield break; }
                case K.Arr: { var snap = ((GdArray)o).l.ToArray(); foreach (var v in snap) yield return v; yield break; }
                case K.Dict: { var snap = ((GdDict)o).keys.ToArray(); foreach (var v in snap) yield return v; yield break; }
                case K.Str: { foreach (char c in (string)o) yield return S(c.ToString()); yield break; }
                case K.Vec2: case K.Vec2i: { yield return F(d); yield return F(e); yield break; }
                case K.Null: yield break;
                default: throw new GdError("Nelze iterovat typ " + k);
            }
        }

        public Gv Duplicate(bool deep)
        {
            if (k == K.Arr)
            {
                var src = ((GdArray)o).l; var l = new List<Gv>(src.Count);
                foreach (var v in src) l.Add(deep ? v.Duplicate(true) : v);
                var a = new GdArray(l); a.typed = ((GdArray)o).typed; return A(a);
            }
            if (k == K.Dict) return D(((GdDict)o).Dup(deep));
            return this;
        }

        // ---------- text ----------
        public string ToStr()
        {
            switch (k)
            {
                case K.Null: return "<null>";
                case K.Bool: return d != 0 ? "true" : "false";
                case K.Int: return ((long)d).ToString(CultureInfo.InvariantCulture);
                case K.Float: return Gd.FloatStr(d);
                case K.Str: return (string)o;
                case K.Vec2: return "(" + Gd.FloatStr(d) + ", " + Gd.FloatStr(e) + ")";
                case K.Vec2i: return "(" + ((long)d).ToString(CultureInfo.InvariantCulture) + ", " + ((long)e).ToString(CultureInfo.InvariantCulture) + ")";
                case K.Color: { var c = (GdColor)o; return "(" + Gd.FloatStr(c.r) + ", " + Gd.FloatStr(c.g) + ", " + Gd.FloatStr(c.b) + ", " + Gd.FloatStr(c.a) + ")"; }
                case K.Rect2: { var r = (GdRect)o; return "[P: (" + Gd.FloatStr(r.x) + ", " + Gd.FloatStr(r.y) + "), S: (" + Gd.FloatStr(r.w) + ", " + Gd.FloatStr(r.h) + ")]"; }
                case K.Arr: { var sb = new StringBuilder("["); var l = ((GdArray)o).l; for (int i = 0; i < l.Count; i++) { if (i > 0) sb.Append(", "); sb.Append(l[i].ToRepr()); } return sb.Append("]").ToString(); }
                case K.Dict:
                    {
                        var dd = (GdDict)o; var sb = new StringBuilder("{");
                        for (int i = 0; i < dd.Count; i++) { if (i > 0) sb.Append(", "); sb.Append(dd.keys[i].ToRepr()).Append(": ").Append(dd.vals[i].ToRepr()); }
                        return sb.Append("}").ToString();
                    }
                case K.Obj: return ((GdObject)o).ToString();
                default: return "<" + k + ">";
            }
        }
        public string ToRepr() { return k == K.Str ? "\"" + (string)o + "\"" : ToStr(); }
        public override string ToString() { return ToStr(); }
    }

    public class GdError : Exception { public GdError(string m) : base(m) { } }
}
