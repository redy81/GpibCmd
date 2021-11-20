
using System;
using System.Runtime.InteropServices;
using System.Text;

internal sealed class MinimalVisa32
{
    public const int VI_SPEC_VERSION = 0x00500100;

#if SIMULATION
    public static int viOpenDefaultRM( out int sesn )
    {
        sesn = 1;

        Console.WriteLine( ">>> viOpenDefaultRM" );
        Console.WriteLine( "    - sesn (out) = " + sesn.ToString() );

        return VI_SUCCESS;
    }

    public static int viClose( int vi )
    {
        Console.WriteLine( ">>> viClose" );
        Console.WriteLine( "    - vi = " + vi.ToString() );

        return VI_SUCCESS;
    }

    private static Random rnd = new Random();

    public static int viOpen( int sesn, string viDesc, int mode, int timeout, out int vi )
    {
        vi = rnd.Next( 9998 ) + 2;

        Console.WriteLine( ">>> viOpen" );
        Console.WriteLine( "    - sesn = " + sesn.ToString() );
        Console.WriteLine( "    - viDesc = " + viDesc.ToString() );
        Console.WriteLine( "    - mode = " + mode.ToString() );
        Console.WriteLine( "    - timeout = " + timeout.ToString() );
        Console.WriteLine( "    - vi (out) = " + vi.ToString() );

        return VI_SUCCESS;
    }

    public static int viPrintf( int vi, string writeFmt )
    {
        Console.WriteLine( ">>> viPrintf" );
        Console.WriteLine( "    - vi = " + vi.ToString() );
        Console.WriteLine( "    - writeFmt = " + writeFmt );

        return VI_SUCCESS;
    }

    public static int viScanf( int vi, string readFmt, StringBuilder arg )
    {
        Console.WriteLine( ">>> viScanf" );
        Console.WriteLine( "    - vi = " + vi.ToString() );
        Console.WriteLine( "    - readFmt = " + readFmt );

        arg.Append( "VISA Return string test_!_0123456789" );

        return VI_SUCCESS;
    }
#else
    // Functions that I use ---------------------------------------------------
    [DllImport( "VISA32.DLL", EntryPoint = "viOpenDefaultRM", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true )]
    public static extern int viOpenDefaultRM(out int sesn);

    [DllImport( "VISA32.DLL", EntryPoint = "viClose", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true )]
    public static extern int viClose( int vi );

    [DllImport( "VISA32.DLL", EntryPoint = "viOpen", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true )]
    public static extern int viOpen( int sesn, string viDesc, int mode, int timeout, out int vi );

    [DllImport( "VISA32.DLL", EntryPoint = "viPrintf", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
    public static extern int viPrintf( int vi, string writeFmt );

    [DllImport( "VISA32.DLL", EntryPoint = "viScanf", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
    public static extern int viScanf( int vi, string readFmt, StringBuilder arg );
#endif

    // - Success Codes (don't care about errors) ------------------------------
    public const int VI_SUCCESS = 0;
    public const int VI_SUCCESS_EVENT_EN = 0x3FFF0002;
    public const int VI_SUCCESS_EVENT_DIS = 0x3FFF0003;
    public const int VI_SUCCESS_QUEUE_EMPTY = 0x3FFF0004;
    public const int VI_SUCCESS_TERM_CHAR = 0x3FFF0005;
    public const int VI_SUCCESS_MAX_CNT = 0x3FFF0006;
    public const int VI_SUCCESS_DEV_NPRESENT = 0x3FFF007D;
    public const int VI_SUCCESS_TRIG_MAPPED = 0x3FFF007E;
    public const int VI_SUCCESS_QUEUE_NEMPTY = 0x3FFF0080;
    public const int VI_SUCCESS_NCHAIN = 0x3FFF0098;
    public const int VI_SUCCESS_NESTED_SHARED = 0x3FFF0099;
    public const int VI_SUCCESS_NESTED_EXCLUSIVE = 0x3FFF009A;
    public const int VI_SUCCESS_SYNC = 0x3FFF009B;

    //// - Other VISA Definitions ---------------------------------------------
    public const short VI_NULL = 0;
    public const short VI_TRUE = 1;
    public const short VI_FALSE = 0;
}

