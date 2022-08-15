using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]

public struct FInt
{
    // New determininistic point from https://stackoverflow.com/questions/605124/fixed-point-math-in-c
    // Coudn't get my deterministic point figured out because of sin, cos and other things =(

    ///<summary>
    ///The lowest negative value possible for this datatype.
    ///</summary>
    public static readonly FInt MinValue;
    ///<summary>
    ///The highest positive value possible for this datatype.
    ///</summary>
    public static readonly FInt MaxValue;

    public static readonly FInt Half;

    static FInt()
    {
        MinValue = FInt.Create(long.MinValue, false);
        MaxValue = FInt.Create(long.MaxValue, false);

        Half = new FInt(0, 5);
    }

    public long RawValue;
    public const int SHIFT_AMOUNT = 12; //12 is 4096

    public const long One = 1 << SHIFT_AMOUNT;
    public const int OneI = 1 << SHIFT_AMOUNT;
    public static FInt OneF = FInt.Create( 1, true );

    #region Constructors

    public FInt( long Value )
    {
        RawValue = Value << SHIFT_AMOUNT;
    }

    public FInt(long integer, long decimals)
    {
        long decEnsurer = 1;

        while (decEnsurer <= decimals) decEnsurer*=10;

        RawValue = (integer << SHIFT_AMOUNT) + (decimals << SHIFT_AMOUNT) / decEnsurer;
    }

    public static FInt Create( long StartingRawValue, bool UseMultiple )
    {
        FInt fInt;
        fInt.RawValue = StartingRawValue;
        if ( UseMultiple )
            fInt.RawValue = fInt.RawValue << SHIFT_AMOUNT;
        return fInt;
    }

    public static FInt Create( long Value )
    {
        FInt fInt;
        fInt.RawValue = Value << SHIFT_AMOUNT;
        return fInt;
    }

    public static FInt Create( double DoubleValue )
    {
        FInt fInt;
        fInt.RawValue = (Int64)Math.Round( DoubleValue * One );
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

    public FInt Inverse
    {
        get { return FInt.Create( -this.RawValue, false ); }
    }

    #region FromParts
    /// <summary>
    /// Create a fixed-int number from parts.  For example, to create 1.5 pass in 1 and 500.
    /// </summary>
    /// <param name="PreDecimal">The number above the decimal.  For 1.5, this would be 1.</param>
    /// <param name="PostDecimal">The number below the decimal, to three digits.  
    /// For 1.5, this would be 500. For 1.005, this would be 5.</param>
    /// <returns>A fixed-int representation of the number parts</returns>
    public static FInt FromParts( int PreDecimal, int PostDecimal )
    {
        FInt f = FInt.Create( PreDecimal, true );
        if ( PostDecimal != 0 )
            f.RawValue += ( FInt.Create( PostDecimal ) / 1000 ).RawValue;

        return f;
    }
    #endregion

    #region *
    public static FInt operator *( FInt one, FInt other )
    {
        FInt fInt;
        fInt.RawValue = ( one.RawValue * other.RawValue ) >> SHIFT_AMOUNT;
        return fInt;
    }

    public static FInt operator *( FInt one, int multi )
    {
        return one * (FInt)multi;
    }

    public static FInt operator *( FInt one, long multi )
    {
        return one * (FInt)multi;
    }

    public static FInt operator *( int multi, FInt one )
    {
        return one * (FInt)multi;
    }

    public static FInt operator *( long multi, FInt one )
    {
        return one * (FInt)multi;
    }
    #endregion

    #region /
    public static FInt operator /( FInt one, FInt other )
    {
        FInt fInt;
        fInt.RawValue = ( one.RawValue << SHIFT_AMOUNT ) / ( other.RawValue );
        return fInt;
    }

    public static FInt operator /( FInt one, int divisor )
    {
        return one / (FInt)divisor;
    }

    public static FInt operator /( int divisor, FInt one )
    {
        return (FInt)divisor / one;
    }
    #endregion

    #region %
    public static FInt operator %( FInt one, FInt other )
    {
        FInt fInt;
        fInt.RawValue = ( one.RawValue ) % ( other.RawValue );
        return fInt;
    }

    public static FInt operator %( FInt one, int divisor )
    {
        return one % (FInt)divisor;
    }

    public static FInt operator %( int divisor, FInt one )
    {
        return (FInt)divisor % one;
    }
    #endregion

    #region +
    public static FInt operator +( FInt one, FInt other )
    {
        FInt fInt;
        fInt.RawValue = one.RawValue + other.RawValue;
        return fInt;
    }

    public static FInt operator +( FInt one, int other )
    {
        return one + (FInt)other;
    }

    public static FInt operator +( int other, FInt one )
    {
        return one + (FInt)other;
    }
    #endregion

    #region -
    public static FInt operator -( FInt one, FInt other )
    {
        FInt fInt;
        fInt.RawValue = one.RawValue - other.RawValue;
        return fInt;
    }

    public static FInt operator -( FInt one, int other )
    {
        return one - (FInt)other;
    }

    public static FInt operator -( int other, FInt one )
    {
        return (FInt)other - one;
    }

    public static FInt operator - (FInt one)
    {
        FInt fInt;
        fInt.RawValue = -one.RawValue;
        return fInt;
    }
    #endregion

    #region ==
    public static bool operator ==( FInt one, FInt other )
    {
        return one.RawValue == other.RawValue;
    }

    public static bool operator ==( FInt one, int other )
    {
        return one == (FInt)other;
    }

    public static bool operator ==( FInt one, long other )
    {
        return one == (FInt)other;
    }

    public static bool operator ==( int other, FInt one )
    {
        return (FInt)other == one;
    }

    public static bool operator ==( long other, FInt one )
    {
        return (FInt)other == one;
    }
    #endregion

    #region !=
    public static bool operator !=( FInt one, FInt other )
    {
        return one.RawValue != other.RawValue;
    }

    public static bool operator !=( FInt one, int other )
    {
        return one != (FInt)other;
    }

    public static bool operator !=( FInt one, long other )
    {
        return one != (FInt)other;
    }

    public static bool operator !=( int other, FInt one )
    {
        return (FInt)other != one;
    }

    public static bool operator !=( long other, FInt one )
    {
        return (FInt)other != one;
    }
    #endregion

    #region >=
    public static bool operator >=( FInt one, FInt other )
    {
        return one.RawValue >= other.RawValue;
    }

    public static bool operator >=( FInt one, int other )
    {
        return one >= (FInt)other;
    }

    public static bool operator >=( int other, FInt one )
    {
        return (FInt)other >= one;
    }

    public static bool operator >=( FInt one, long other )
    {
        return one >= (FInt)other;
    }

    public static bool operator >=( long other, FInt one )
    {
        return (FInt)other >= one;
    }
    #endregion

    #region <=
    public static bool operator <=( FInt one, FInt other )
    {
        return one.RawValue <= other.RawValue;
    }

    public static bool operator <=( FInt one, int other )
    {
        return one <= (FInt)other;
    }

    public static bool operator <=( int other, FInt one )
    {
        return (FInt)other <= one;
    }

    public static bool operator <=( FInt one, long other )
    {
        return one <= (FInt)other;
    }

    public static bool operator <=( long other, FInt one )
    {
        return (FInt)other <= one;
    }
    #endregion

    #region >
    public static bool operator >( FInt one, FInt other )
    {
        return one.RawValue > other.RawValue;
    }

    public static bool operator >( FInt one, int other )
    {
        return one > (FInt)other;
    }

    public static bool operator >( int other, FInt one )
    {
        return (FInt)other > one;
    }

    public static bool operator >( FInt one, long other )
    {
        return one > (FInt)other;
    }

    public static bool operator >( long other, FInt one )
    {
        return (FInt)other > one;
    }
    #endregion

    #region <
    public static bool operator <( FInt one, FInt other )
    {
        return one.RawValue < other.RawValue;
    }

    public static bool operator <( FInt one, int other )
    {
        return one < (FInt)other;
    }

    public static bool operator <( int other, FInt one )
    {
        return (FInt)other < one;
    }

    public static bool operator <( FInt one, long other )
    {
        return one < (FInt)other;
    }

    public static bool operator <( long other, FInt one )
    {
        return (FInt)other < one;
    }
    #endregion

    public static explicit operator int( FInt src )
    {
        return (int)( src.RawValue >> SHIFT_AMOUNT );
    }

    public static explicit operator FInt( int src )
    {
        return FInt.Create( src ) ;
    }

    public static explicit operator uint( FInt src )
    {
        return (uint)( src.RawValue >> SHIFT_AMOUNT );
    }

    public static explicit operator long( FInt src )
    {
        return src.RawValue >> SHIFT_AMOUNT;
    }

    public static explicit operator ulong( FInt src )
    {
        return (ulong)( src.RawValue >> SHIFT_AMOUNT );
    }

    public static explicit operator float( FInt src )
    {
        return (float)((double) src.RawValue / One); 
    }

    public static explicit operator FInt( uint src )
    {
        return FInt.Create( (int)src ) ;
    }

    public static explicit operator FInt( long src )
    {
        return FInt.Create( src, true );
    }

    public static explicit operator FInt( ulong src )
    {
        return FInt.Create( (long)src, true );
    }

    public static explicit operator FInt( bool src )
    {
        return src? (FInt) 1: (FInt) 0;
    }

    public static explicit operator FInt(float src)
    {
        return FInt.Create((double)src);
    }

    public static explicit operator FInt(double src)
    {
        return FInt.Create(src);
    }

    public static explicit operator FInt(FInt32 src)
    {
        FInt fInt;
        fInt.RawValue = src.RawValue;
        return fInt;
    }

    public static explicit operator bool( FInt src )
    {
        return src.RawValue > 0;
    }

    public static FInt Parse( string src )
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

        return (FInt) Int64.Parse(src);
    }

    public static FInt operator <<( FInt one, int Amount )
    {
        return FInt.Create( one.RawValue << Amount, false );
    }

    public static FInt operator >>( FInt one, int Amount )
    {
        return FInt.Create( one.RawValue >> Amount, false );
    }

    public override bool Equals( object obj )
    {
        if ( obj is FInt )
            return ( (FInt)obj ).RawValue == this.RawValue;
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