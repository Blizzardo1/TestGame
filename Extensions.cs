using SharpSDL3.Enums;
using SharpSDL3.Structs;
using TestGame.Colors;
using TestGame.GameObjects.Characters;

namespace TestGame;

public static class Extensions {

    private static readonly Log? _log = Log.GetCurrentClassLogger(LogCategory.Custom, "Root Extensions");
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

    public static bool Intersects(this Rect rect, Rect other) {
        return rect.X <= other.X + other.W
            && rect.X + rect.W >= other.X
            && rect.Y <= other.Y + other.H
            && rect.Y + rect.H >= other.Y;
    }

    public static bool Intersects(this FRect rect, FRect other) {
        return rect.X <= other.X + other.W
            && rect.X + rect.W >= other.X
            && rect.Y <= other.Y + other.H
            && rect.Y + other.H >= other.Y;
    }

    public static Intersection GetIntersect(this Entity a, Entity b) {
        Intersection intersection = new() {
            Type = Intersection.IntersectionTypes.None,
            Direction = Intersection.Directionals.None
        };
        if (!a.HitBox.Intersects(b.HitBox)) {
            return intersection;
        }

        intersection.Type = Intersection.IntersectionTypes.Rectangle;

        float distX = (b.X - a.X);
        float distY = (b.Y - a.Y);
        float timeX = Math.Abs(distX) / a.VelocityX;
        float timeY = Math.Abs(distY) / a.VelocityY;

        _log?.Debug($"distX: {distX} distY: {distY}; timeX: {timeX} timeY: {timeY}");

        if (distX < 0 && a.Direction.HasFlag(Direction.Left) && timeX > timeY) {
            intersection.Direction |= Intersection.Directionals.Left;
            return intersection;
        } else if (distX > 0 && a.Direction.HasFlag(Direction.Right) && timeX > timeY) {
            intersection.Direction |= Intersection.Directionals.Right;
            return intersection;
        }

        if (distY < 0 && a.Direction.HasFlag(Direction.Up) && timeY > timeX) {
            intersection.Direction |= Intersection.Directionals.Up;
        } else if (distY > 0 && a.Direction.HasFlag(Direction.Down) && timeY > timeX) {
            intersection.Direction |= Intersection.Directionals.Down;
        }

        return intersection;
    }

    public static Intersection GetIntersect(this FRect rect, FRect other) {
        Intersection intersection = new() {
            Type = Intersection.IntersectionTypes.None,
            Direction = Intersection.Directionals.None
        };
        if (!rect.Intersects(other)) {
            return intersection;
        }

        intersection.Type = Intersection.IntersectionTypes.Rectangle;

        if (rect.X < other.X) {
            intersection.Direction |= Intersection.Directionals.Left;
        } else if (rect.X > other.X) {
            intersection.Direction |= Intersection.Directionals.Right;
        }

        if (rect.Y < other.Y) {
            intersection.Direction |= Intersection.Directionals.Up;
        } else if (rect.Y > other.Y) {
            intersection.Direction |= Intersection.Directionals.Down;
        }

        return intersection;
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


    /// <summary>
    /// Floating point Equality function to see if the two 64-bit numbers are almost equal to each other
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <param name="precision">How precice the equality to be calculating against</param>
    /// <returns>Truw if the two numbers are similar by precision</returns>
    public static bool AlmostEquals(this double a, double b, double precision = 0.000000001d) {
        return Math.Abs(a - b) <= precision;
    }

    public static bool IsEmpty(this string s) => string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
}