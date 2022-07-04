
using System;

public struct FInt32
{
    // New determininistic point from https://stackoverflow.com/questions/605124/fixed-point-math-in-c
    // Coudn't get my deterministic point figured out because of sin, cos and other things =(

    ///<summary>
    ///The lowest negative value possible for this datatype.
    ///</summary>
    public static readonly FInt32 MinValue;
    ///<summary>
    ///The highest positive value possible for this datatype.
    ///</summary>
    public static readonly FInt32 MaxValue;

    static FInt32()
    {
        MinValue = FInt32.Create(int.MinValue, false);
        MaxValue = FInt32.Create(int.MaxValue, false);
    }

    public int RawValue;
    public const int SHIFT_AMOUNT = 12; //12 is 4096

    public const int One = 1 << SHIFT_AMOUNT;
    public const int OneI = 1 << SHIFT_AMOUNT;
    public static FInt32 OneF = FInt32.Create( 1, true );

    #region Constructors
    public static FInt32 Create( int StartingRawValue, bool UseMultiple )
    {
        FInt32 fInt;
        fInt.RawValue = StartingRawValue;
        if ( UseMultiple )
            fInt.RawValue = fInt.RawValue << SHIFT_AMOUNT;
        return fInt;
    }

    public static FInt32 Create( int Value )
    {
        FInt32 fInt;
        fInt.RawValue = Value << SHIFT_AMOUNT;
        return fInt;
    }

    public static FInt32 Create( double DoubleValue )
    {
        FInt32 fInt;
        fInt.RawValue = (Int32)Math.Round( DoubleValue * One );
        return fInt;
    }
    #endregion

    public int IntValue
    {
        get { return (int)( this.RawValue >> SHIFT_AMOUNT ); }
    }

    public int ToInt()
    {
        return (int)( this.RawValue >> SHIFT_AMOUNT );
    }

    public double ToDouble()
    {
        return (double)this.RawValue / (double)One;
    }

    public float ToFloat ()
    {
        return (float) ((double)this.RawValue / One);
    }

    public FInt32 Inverse
    {
        get { return FInt32.Create( -this.RawValue, false ); }
    }

    #region FromParts
    /// <summary>
    /// Create a fixed-int number from parts.  For example, to create 1.5 pass in 1 and 500.
    /// </summary>
    /// <param name="PreDecimal">The number above the decimal.  For 1.5, this would be 1.</param>
    /// <param name="PostDecimal">The number below the decimal, to three digits.  
    /// For 1.5, this would be 500. For 1.005, this would be 5.</param>
    /// <returns>A fixed-int representation of the number parts</returns>
    public static FInt32 FromParts( int PreDecimal, int PostDecimal )
    {
        FInt32 f = FInt32.Create( PreDecimal, true );
        if ( PostDecimal != 0 )
            f.RawValue += ( FInt32.Create( PostDecimal ) / 1000 ).RawValue;

        return f;
    }
    #endregion

    #region *
    public static FInt32 operator *( FInt32 one, FInt32 other )
    {
        FInt32 fInt;
        fInt.RawValue = ( one.RawValue * other.RawValue ) >> SHIFT_AMOUNT;
        return fInt;
    }

    public static FInt32 operator *( FInt32 one, int multi )
    {
        return one * (FInt32)multi;
    }

    public static FInt32 operator *( int multi, FInt32 one )
    {
        return one * (FInt32)multi;
    }
    #endregion

    #region /
    public static FInt32 operator /( FInt32 one, FInt32 other )
    {
        FInt32 fInt;
        fInt.RawValue = ( one.RawValue << SHIFT_AMOUNT ) / ( other.RawValue );
        return fInt;
    }

    public static FInt32 operator /( FInt32 one, int divisor )
    {
        return one / (FInt32)divisor;
    }

    public static FInt32 operator /( int divisor, FInt32 one )
    {
        return (FInt32)divisor / one;
    }
    #endregion

    #region %
    public static FInt32 operator %( FInt32 one, FInt32 other )
    {
        FInt32 fInt;
        fInt.RawValue = ( one.RawValue ) % ( other.RawValue );
        return fInt;
    }

    public static FInt32 operator %( FInt32 one, int divisor )
    {
        return one % (FInt32)divisor;
    }

    public static FInt32 operator %( int divisor, FInt32 one )
    {
        return (FInt32)divisor % one;
    }
    #endregion

    #region +
    public static FInt32 operator +( FInt32 one, FInt32 other )
    {
        FInt32 fInt;
        fInt.RawValue = one.RawValue + other.RawValue;
        return fInt;
    }

    public static FInt32 operator +( FInt32 one, int other )
    {
        return one + (FInt32)other;
    }

    public static FInt32 operator +( int other, FInt32 one )
    {
        return one + (FInt32)other;
    }
    #endregion

    #region -
    public static FInt32 operator -( FInt32 one, FInt32 other )
    {
        FInt32 fInt;
        fInt.RawValue = one.RawValue - other.RawValue;
        return fInt;
    }

    public static FInt32 operator -( FInt32 one, int other )
    {
        return one - (FInt32)other;
    }

    public static FInt32 operator -( int other, FInt32 one )
    {
        return (FInt32)other - one;
    }

    public static FInt32 operator - (FInt32 one)
    {
        FInt32 fInt;
        fInt.RawValue = -one.RawValue;
        return fInt;
    }
    #endregion

    #region ==
    public static bool operator ==( FInt32 one, FInt32 other )
    {
        return one.RawValue == other.RawValue;
    }

    public static bool operator ==( FInt32 one, int other )
    {
        return one == (FInt32)other;
    }

    public static bool operator ==( int other, FInt32 one )
    {
        return (FInt32)other == one;
    }
    #endregion

    #region !=
    public static bool operator !=( FInt32 one, FInt32 other )
    {
        return one.RawValue != other.RawValue;
    }

    public static bool operator !=( FInt32 one, int other )
    {
        return one != (FInt32)other;
    }

    public static bool operator !=( int other, FInt32 one )
    {
        return (FInt32)other != one;
    }
    #endregion

    #region >=
    public static bool operator >=( FInt32 one, FInt32 other )
    {
        return one.RawValue >= other.RawValue;
    }

    public static bool operator >=( FInt32 one, int other )
    {
        return one >= (FInt32)other;
    }

    public static bool operator >=( int other, FInt32 one )
    {
        return (FInt32)other >= one;
    }
    #endregion

    #region <=
    public static bool operator <=( FInt32 one, FInt32 other )
    {
        return one.RawValue <= other.RawValue;
    }

    public static bool operator <=( FInt32 one, int other )
    {
        return one <= (FInt32)other;
    }

    public static bool operator <=( int other, FInt32 one )
    {
        return (FInt32)other <= one;
    }
    #endregion

    #region >
    public static bool operator >( FInt32 one, FInt32 other )
    {
        return one.RawValue > other.RawValue;
    }

    public static bool operator >( FInt32 one, int other )
    {
        return one > (FInt32)other;
    }

    public static bool operator >( int other, FInt32 one )
    {
        return (FInt32)other > one;
    }
    #endregion

    #region <
    public static bool operator <( FInt32 one, FInt32 other )
    {
        return one.RawValue < other.RawValue;
    }

    public static bool operator <( FInt32 one, int other )
    {
        return one < (FInt32)other;
    }

    public static bool operator <( int other, FInt32 one )
    {
        return (FInt32)other < one;
    }
    #endregion

    public static explicit operator int( FInt32 src )
    {
        return (int)( src.RawValue >> SHIFT_AMOUNT );
    }

    public static explicit operator FInt32( int src )
    {
        return FInt32.Create( src ) ;
    }

    public static explicit operator FInt32( long src )
    {
        return FInt32.Create( (int)src, true );
    }

    public static explicit operator FInt32( ulong src )
    {
        return FInt32.Create( (int)(long)src, true );
    }

    public static explicit operator FInt32( bool src )
    {
        return src? (FInt32) 1: (FInt32) 0;
    }

    public static explicit operator FInt32(float src)
    {
        return FInt32.Create((double)src);
    }

    public static explicit operator FInt32(double src)
    {
        return FInt32.Create(src);
    }

    public static explicit operator bool( FInt32 src )
    {
        return src.RawValue > 0;
    }

    public static FInt32 Parse( string src )
    {
        if(src.Contains("."))
        {
            var arr = src.Split('.');

            string decimals;

            if(arr[1].Length == 3) decimals = arr[1];
            else if (arr[1].Length > 3) decimals = arr[1].Substring(0, 3);
            else
            {
                decimals = arr[1];
                int compensation = 3 - decimals.Length;

                for (int i = 0; i< compensation; ++i)
                {
                    decimals += '0';
                }
            }

            return FromParts(Int32.Parse(arr[0]), Int32.Parse(decimals));
        }

        return (FInt32) Int32.Parse(src);
    }

    public static FInt32 Parse (FInt src, out bool overflow)
    {
        overflow = src.RawValue > int.MaxValue || src.RawValue < int.MinValue;

        return FInt32.Create((int)src.RawValue, false);
    }

    public static FInt32 operator <<( FInt32 one, int Amount )
    {
        return FInt32.Create( one.RawValue << Amount, false );
    }

    public static FInt32 operator >>( FInt32 one, int Amount )
    {
        return FInt32.Create( one.RawValue >> Amount, false );
    }

    public override bool Equals( object obj )
    {
        if ( obj is FInt32 )
            return ( (FInt32)obj ).RawValue == this.RawValue;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return RawValue.GetHashCode();
    }

    public override string ToString()
    {
        return ((double) this.RawValue / One).ToString();
    }
}