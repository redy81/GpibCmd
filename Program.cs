using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace GpibCmd
{
    internal enum OperationType
    {
        Delay,
        OpenInstrument,
        Command
    }

    internal class Operation
    {
        public OperationType Type;
        public string Data;

        public Operation( OperationType type, string data )
        {
            Type = type;
            Data = data;
        }

        public int DataInt
        {
            get
            {
                if (int.TryParse( Data, out int val ))
                {
                    return val;
                }
                else
                {
                    return -1;
                }
            }
        }
    }

    internal static class Utils
    {
        public const int ExitCodeNotEnoughParameters = 1;
        public const int ExitCodeInputFileError = 2;
        public const int ExitCodeInstrumentError = 3;
        public const int ExitCodeParameterError = 4;
        public const int ExitCodeProcessingError = 5;
        public const int ExitCodeMissingDll = 6;

        public static string GetStringParameter( List<string> argsBuffer, ref int idx )
        {
            idx++;

            if (idx >= argsBuffer.Count)
            {
                Console.WriteLine( "Not enough parameters specified. Error at '" + argsBuffer[idx - 1] + "'" );
                Environment.Exit( ExitCodeParameterError );
            }
            else
            {
                return argsBuffer[idx];
            }

            return "";  // To make the compiler happy
        }

        public static int GetIntParameter( List<string> argsBuffer, ref int idx )
        {
            string param = GetStringParameter( argsBuffer, ref idx );

            if (int.TryParse( param, out int value ))
            {
                return value;
            }
            else
            {
                Console.WriteLine( "Invalid integer specified. Error at '" + argsBuffer[idx - 1] + "'" );
                Environment.Exit( ExitCodeParameterError );
            }

            return -1;  // To make the compiler happy
        }
    }

    class Program
    {
        private static bool isVerbose = true;

        private static void Print_Help()
        {
            Console.WriteLine( "Usage:" );
            Console.WriteLine( "GPIBcmd [-h] [-q] [-n] [-t Timeout] -a \"GPIB Address\" \"Visa String 1\" ..." );
            Console.WriteLine( "GPIBcmd -i \"Input file path\"" );
            Console.WriteLine( "" );
            Console.WriteLine( "-h             Shows this help." );
            Console.WriteLine( "" );
            Console.WriteLine( "-q             Quiet mode. It will only print the answers to the console." );
            Console.WriteLine( "" );
            Console.WriteLine( "-n             Do not add LF at the end of Visa command strings." );
            Console.WriteLine( "" );
            Console.WriteLine( "-t Timeout     Changes the default GPIB timeout. In ms. Default is 5000 ms." );
            Console.WriteLine( "" );
            Console.WriteLine( "-i \"File path\" Specify an input file. It contains command line options, one" );
            Console.WriteLine( "               token per line. Use this to bypass the limit of command line" );
            Console.WriteLine( "               length." );
            Console.WriteLine( "               When an input file is specified, all other command line options" );
            Console.WriteLine( "               are ignored." );
            Console.WriteLine( "" );
            Console.WriteLine( "-a Address     Changes the GPIB address to [address]. It must be the first" );
            Console.WriteLine( "               parameter set, before the VISA commands. It can be used " );
            Console.WriteLine( "               multiple times." );
            Console.WriteLine( "" );
            Console.WriteLine( "-d Delay_ms    Waits for the specified number of ms" );
            Console.WriteLine( "" );
            Console.WriteLine( "GPIB Address   Can be either an alias, or a normal GPIB address in the form" );
            Console.WriteLine( "               GPIBx::y::INSTR." );
            Console.WriteLine( "" );
            Console.WriteLine( "Visa String n   is the command to be sent to the instrument. If the string" );
            Console.WriteLine( "                contains a '?', a readback will be automatically performed and" );
            Console.WriteLine( "                output to the console." );
            Console.WriteLine( "" );
            Console.WriteLine( "Example:" );
            Console.WriteLine( "GPIBcmd -a \"GPIB0::5::INSTR\" \"RECALL 1\" -d 500 \"OUTPUT ON\"" );
            Console.WriteLine( "GPIBcmd -i \"C:\\Gpib\\CmdList.txt\"" );
        }

        private static void Writeln( string text )
        {
            if (isVerbose)
            {
                Console.WriteLine( text );
            }
        }

        static void Main( string[] args )
        {
            int visaTimeout = 5000;
            string CommandTerminator = "\n";

            Writeln( "GPIBcmd v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " - F.Branchetti" );
            Writeln( "" );

            List<string> InputArgs = null;

            if ((args.Length <= 2) || args.Any( x => string.Compare( x, "-h", true ) == 0 ))
            {
                Print_Help();
                Environment.Exit( Utils.ExitCodeNotEnoughParameters );
            }

            if (args.Any( x => string.Compare( x, "-i", true ) == 0 ))
            {
                int idx = args.ToList().FindIndex( x => string.Compare( x, "-i", true ) == 0 );

                string fileName = Utils.GetStringParameter( args.ToList(), ref idx );

                try
                {
                    InputArgs = File.ReadAllLines( fileName ).ToList();
                }
                catch (Exception ex)
                {
                    Writeln( "Parameter error. -i specified, but file could not be read!" );
                    Writeln( "Error: " + ex.Message );
                    Environment.Exit( Utils.ExitCodeParameterError );
                }
            }
            else
            {
                InputArgs = args.ToList();
            }

            List<Operation> OperationsList = new List<Operation>();

            for (int i = 0; i < InputArgs.Count; i++)
            {
                switch (InputArgs[i].Trim().ToLower())
                {
                    case "-a":
                    {
                        string par = Utils.GetStringParameter( InputArgs, ref i );

                        OperationsList.Add( new Operation( OperationType.OpenInstrument, par ) );
                    }
                    break;

                    case "-t":
                    {
                        int val = Utils.GetIntParameter( InputArgs, ref i );

                        if (val < 0)
                        {
                            Writeln( "Parameter error. -t found, timeout is invalid." );
                            Environment.Exit( Utils.ExitCodeParameterError );
                        }

                        visaTimeout = val;
                    }
                    break;

                    case "-d":
                    {
                        int val = Utils.GetIntParameter( InputArgs, ref i );

                        if (val < 0)
                        {
                            Writeln( "Parameter error. -d found, delay is invalid." );
                            Environment.Exit( Utils.ExitCodeParameterError );
                        }

                        OperationsList.Add( new Operation( OperationType.Delay, val.ToString() ) );
                    }
                    break;

                    case "-q":
                        isVerbose = false;
                        break;

                    case "-n":
                        CommandTerminator = "";
                        break;

                    case "-h":
                        // Already handled before, just skip it
                        break;

                    case "-i":
                        Writeln( "Parameter error. -i cannot be used in the input file." );
                        Environment.Exit( Utils.ExitCodeParameterError );
                        break;

                    default:
                        OperationsList.Add( new Operation( OperationType.Command, InputArgs[i] ) );
                        break;
                }
            }

            if (OperationsList.First().Type != OperationType.OpenInstrument)
            {
                Writeln( "Parameter error. -a should be specified first to select the address." );
                Environment.Exit( Utils.ExitCodeParameterError );
            }

            int visaSession = 0;
            int visaInstrument = 0;

            try
            {
                if (MinimalVisa32.viOpenDefaultRM( out visaSession ) != MinimalVisa32.VI_SUCCESS)
                {
                    Writeln( "VISA Error: cannot open a VISA session." );
                    Environment.Exit( Utils.ExitCodeInstrumentError );
                }
            }
            catch (DllNotFoundException ex)
            {
                Writeln( "VISA Error, cannot find one of the VISA DLLs:" );
                Writeln( ex.Message );
                Environment.Exit( Utils.ExitCodeMissingDll );
            }

            string gpibAddress = string.Empty;
            string command;
            bool isInstrumentOpen = false;
            int returnCode = 0;

            foreach (var op in OperationsList)
            {
                switch (op.Type)
                {
                    case OperationType.Delay:
                    {
                        Writeln( "- Delay: " + op.Data + " ms" );

                        Thread.Sleep( op.DataInt );
                    }
                    break;

                    case OperationType.OpenInstrument:
                    {
                        if (isInstrumentOpen)
                        {
                            if (MinimalVisa32.viClose( visaInstrument ) != MinimalVisa32.VI_SUCCESS)
                            {
                                Writeln( "VISA Error: cannot close the instrument." );
                                returnCode = Utils.ExitCodeInstrumentError;
                                break;
                            }

                            isInstrumentOpen = false;
                        }

                        gpibAddress = op.Data;

                        if (MinimalVisa32.viOpen( visaSession, gpibAddress, MinimalVisa32.VI_NULL, visaTimeout, out visaInstrument ) != MinimalVisa32.VI_SUCCESS)
                        {
                            Writeln( "VISA Error: cannot open the instrument. (Address: " + gpibAddress + " )" );
                            returnCode = Utils.ExitCodeInstrumentError;
                            break;
                        }

                        Writeln( "* " + gpibAddress + " Open!" );

                        isInstrumentOpen = true;
                    }
                    break;

                    case OperationType.Command:
                    {
                        if (!isInstrumentOpen)
                        {
                            Writeln( "VISA Error: no instrument is open." );
                            returnCode = Utils.ExitCodeInstrumentError;
                            break;
                        }

                        command = op.Data;

                        Writeln( "> " + command );

                        if (MinimalVisa32.viPrintf( visaInstrument, command + CommandTerminator ) != MinimalVisa32.VI_SUCCESS)
                        {
                            Writeln( "VISA Error: cannot open the instrument." );
                            returnCode = Utils.ExitCodeInstrumentError;
                            break;
                        }

                        if (command.TrimEnd().Last() == '?')
                        {
                            StringBuilder vAns = new StringBuilder();

                            if (MinimalVisa32.viScanf( visaInstrument, "%T", vAns ) != MinimalVisa32.VI_SUCCESS)
                            {
                                Writeln( "VISA Error: cannot read the instrument answer. (Query: " + command + " )" );
                                returnCode = Utils.ExitCodeInstrumentError;
                                break;
                            }

                            Console.WriteLine( "< " + vAns.ToString().TrimEnd() );
                        }

                    }
                    break;

                    default:
                        break;
                }

                if (returnCode > 0)
                {
                    break;
                }
            }

            if (isInstrumentOpen)
            {
                if (MinimalVisa32.viClose( visaInstrument ) != MinimalVisa32.VI_SUCCESS)
                {
                    Writeln( "VISA Error: cannot close the instrument." );
                    returnCode = Utils.ExitCodeInstrumentError;
                }
            }

            if (MinimalVisa32.viClose( visaSession ) != MinimalVisa32.VI_SUCCESS)
            {
                Writeln( "VISA Error: cannot close the VISA session." );
                returnCode = Utils.ExitCodeInstrumentError;
            }

            Environment.Exit( returnCode );
        }
    }
}
