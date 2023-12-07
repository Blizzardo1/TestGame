using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace TestGame;

public struct Size {
    #region Equality members

    // ReSharper disable once MemberCanBePrivate.Global
    public bool Equals(Size other) {
        return Width == other.Width && Height == other.Height;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) {
        return obj is Size other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode() {
        return HashCode.Combine(Width, Height);
    }

    #endregion

    public int Width { get; set; }
    public int Height { get; set; }

    public Size(int width, int height) {
        Width = width;
        Height = height;
    }

    public readonly void Deconstruct(out int width, out int height) {
        width = Width;
        height = Height;
    }

    public static bool operator ==(Size left, Size right) => left.Equals(right);
    public static bool operator !=(Size left, Size right) => !( left == right );
}