namespace TestGame.Colors {
    public static class ColorConverter {
        /// <summary>
        /// Converts an HSV value to an RGB value
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="l">Luminescence</param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        public static void Hsl2Rgb(
            double h,
            double s,
            double l,
            out byte r,
            out byte g,
            out byte b) {
            if (s == 0) {
                // Achromatic (gray)
                r = g = b = (byte)( l * 255.0 );
            }
            else {
                double q = l < 0.5 ? l * ( 1 + s ) : l + s - l * s;
                double p = 2 * l - q;
                double hk = h / 360.0;
                double[] t = [0, 0, 0];
                t[ 0 ] = hk + 1.0 / 3.0; // Tr
                t[ 1 ] = hk; // Tg
                t[ 2 ] = hk - 1.0 / 3.0; // Tb

                for (int i = 0; i < 3; i++) {
                    if (t[ i ] < 0) t[ i ] += 1.0;
                    if (t[ i ] > 1) t[ i ] -= 1.0;

                    t[ i ] = t[ i ] switch {
                        < 1.0 / 6.0 => p + ( ( q - p ) * 6.0 * t[ i ] ),
                        < 1.0 / 2.0 => q,
                        < 2.0 / 3.0 => p + ( ( q - p ) * ( 2.0 / 3.0 - t[ i ] ) * 6.0 ),
                        _ => p
                    };
                }

                r = (byte)( t[ 0 ] * 255.0 );
                g = (byte)( t[ 1 ] * 255.0 );
                b = (byte)( t[ 2 ] * 255.0 );
            }
        }
    }
}