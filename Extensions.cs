using SDL2;
using TestGame.Colors;
using Version = SDL2.Version;

namespace TestGame;

public static class Extensions {
    public static Color ToColor(this KnownColor color) {
        Color c = new();
        byte a = (byte)( (int)color >> 24 & 0xFF );
        byte r = (byte)( (int)color >> 16 & 0xFF );
        byte g = (byte)( (int)color >> 8 & 0xFF );
        byte b = (byte)( (int)color & 0xFF );
        c.A = a;
        c.R = r;
        c.G = g;
        c.B = b;

        return c;
    }

    public static FRect ToFRect(this Rect rect) =>
        new() {
            X = rect.X,
            Y = rect.Y,
            W = rect.W,
            H = rect.H
        };

    public static Rect ToRect(this FRect rect) =>
        new() {
            X = (int)Math.Floor(rect.X),
            Y = (int)Math.Floor(rect.Y),
            W = (int)Math.Floor(rect.W),
            H = (int)Math.Floor(rect.H)
        };

    public static bool Intersects(this Rect rect, int x, int y) {
        return x >= rect.X || x <= rect.X + rect.W && y >= rect.Y || y <= rect.Y + rect.H;
    }

    public static bool Intersects(this FRect rect, float x, float y) {
        return x >= rect.X || x <= rect.X + rect.W && y >= rect.Y || y <= rect.Y + rect.H;
    }

    /// <summary>
    /// Grows the list by multiplying a <see cref="multiple"/> and the length of <see cref="source"/>
    /// </summary>
    /// <typeparam name="T">Any object</typeparam>
    /// <param name="source">The source array</param>
    /// <param name="multiple">The multiple to expand the array by</param>
    /// <returns>An expanded array of duplicated <typeparamref name="T"/></returns>
    public static T[] Expand< T >(this T[] source, int multiple) {
        T[] clone = new T[source.Length];

        Array.Copy(source, clone, source.Length);
        Array.Resize(ref source, multiple * source.Length);

        for (int y = 0; y < clone.Length; y++) {
            for (int x = 0; x < multiple; x++) {
                source[ x + ( y * multiple ) ] = clone[ y ];
            }
        }

        return source;
    }

    /// <summary>
    /// Duplicate an Array of <typeparamref name="T"/> <see cref="multiple"/> times
    /// </summary>
    /// <typeparam name="T">Any object</typeparam>
    /// <param name="source">The source array</param>
    /// <param name="multiple">The multiple to extend the array by</param>
    /// <returns>An extended array of duplicated <typeparamref name="T"/></returns>
    public static T[] Multiply< T >(this T[] source, int multiple) {
        T[] clone = new T[source.Length];

        Array.Copy(source, clone, source.Length);
        Array.Resize(ref source, multiple * source.Length);
        for (int i = 0; i < source.Length; i += clone.Length) {
            Array.Copy(clone, 0, source, i, clone.Length);
        }

        return source;
    }

    public static Color SetInverseBasedOn(this Color color, Color other) {
        double lum = other.CalculateLuminance();

        return lum > 0.5 ? KnownColor.Black.ToColor() : KnownColor.White.ToColor();
    }

    public static double CalculateLuminance(this Color color) {
        return ( 0.299 * color.R + 0.587 * color.G + 0.114 * color.B ) / 255.0;
    }

    public static string LongestString(this List< MenuItem > list) {
        int index = 0;
        if (list.Count == 0) return "";
        for (int i = 1; i < list.Count; i++) {
            if (list[ index ].Text.Length < list[ i ].Text.Length) {
                index = i;
            }
        }

        return list[ index ].Text;
    }

    public static bool IsEmpty(this string s) => string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);

    public static string String(this Version v) => $"{v.Major}.{v.Minor}.{v.Patch}";
}