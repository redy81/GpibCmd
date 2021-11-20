# GpibCmd
Control GPIB instruments from the command line

GPIBcmd is a command line instrument control in C#.

![License](https://img.shields.io/badge/license-MIT-red.svg)

## Introduction

GPIBcmd is a simple command line application to send commands to instruments through any VISA interface. 

I have created it to quickly configure instruments for differents setup, so I could set all the options by just running a simple batch.


## Requirements

To use GPIBcmd you should have the GPIB drivers installed and a reachable _Visa32.dll_ library. Also, .NET Core 3.1 needs to be in the system.
GPIBcmd was tested with the Keysight library, however it should work indipendently of the supplier.

## Usage

GPIBcmd works in two modes:

1. Command line mode
2. Input file mode

The command line syntaxes for the two modes are the following:

1. `GPIBcmd \[-h\] \[-q\] \[-n\] \[-t Timeout\] -a "GPIB Address" "VISA String 1" \[-d Delay\] ...`
2. `GPIBcmd \[-i\] "Input file"`
 
The available flags are:
* `-h`: shows the help text with a basic description of the usage.
* `-q`: enables the quiet mode. In quiet mode no extra text is output to the console, only the result of the read operations are displayed.
* `-n`: disables the automatic adding of a linefeed (\n) after every Visa command string. You can use this flag if your system is sending the EOI signal at the end of the write, and the LF is not required.
* `-t Timeout`: change the default timeout of the VISA commands to the specified value in ms. The default timeout value is 5000 ms.
* `-a Address`: specify the GPIB address to use for the following commands. The address can be a GPIB address in the format *GPIB\:\:0\:\:INSTR* or an alias. This flag can be used multiple times, to specify a new address to control multiple instruments. The address must be specified before sending a command.
* `-d Delay`: add a delay (at the position where this flag is added) of the specified time in ms.
* `-i FilePath`: specifies to use the *Input File Mode* and to read the data from the file *FilePath*. when using this mode, *-i* should be the only flag specified.
* `VISA String x`: It one VISA command to send to the instrument. If the command ends with `?` then a read command is automatically send and the answer is output to the console. As many VISA strings as required can be added, until the limit of the command line length. Quotes are needed if the command contains spaces.

### Command Line Mode

When operating in this mode, before any Command can be send, an address must be specified; this means that `-a Address` must be specified before of the first Command (flags `-q`, `-n`, `-t Timeout` are configuration flags and can be specified anywhere on the CMD).

Example:
* `GPIBcmd -a GPIB0::5::INSTR "*IDN?" "VOLTAGE:OUTPUT 300"` :heavy_check_mark: Valid cmd operation with a query command (*IDN?)
* `GPIBcmd -a GPIB0::5::INSTR "VOLT:OUT 300" -a GPIB0::2::INSTR "INPUT:THD ON"` :heavy_check_mark: Valid cmd operation, sending commands to different instruments
* `GPIBcmd "VOLT:OUT 300" -a GPIB0::2::INSTR "INPUT:THD ON"` :x: Invalid: a command is specified before an address


### Input File Mode

In Input File Mode, the configuration tokens are read from a specified file. This allows to overcome the limit of the command line length.

In the file, each token **must** be on a different line. Quotes should also be omitted, as they are not required.

The same rules that applies for the command line on the order of the tokes, applies also to the order of the tokens in the file. The file is read top to bottom.

File extension is not relevant, any is accepted.

to run in this mode, the `-i` flag should be used. This should the the only flag on the command line, anything else is ignored. All other flags should be set in the file.

Command line example:

`GPIBcmd -i instrumentConfig.txt`

File example:

```
-a
GPIB0::5::INSTR
VOLT:OUT 300
-d
500
-a
GPIB0::2::INSTR 
*IDN?
INPUT:THD ON
VOLTAGE?
```

## Notes

The GPIB address can be any address supported by the installed VISA drivers. Aliases are also supported.

The best way to find the correct address is to open the installed GUI (Keysignt IO Library, NI Max, etc...) and see which address is listed there for the instrument. Any instrument that appears in the GUI connection list, it's supported by GPIBcmd.

This means also USB, COM, etc connections are supported. Aliases are supported too.


### Donation

If you find this software useful, or it helped you to save time, you can offer me a coffee. :smile:

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate/?hosted_button_id=VNMV7XY9J5HBG)

**Bitcoin**: bc1q4379vq6jfg8swgqajyaetm02fyk2mwj0mwy8wj

**Bitcoin**: 1DbK9AoXxMRYYENwsoDEzqyxppo1cMUQUN

**Ethereum**: 0x381C29dE5781EEa0182568146a1B2c32205DF85B

**Doge**: DGbL57EA844QzgqRc1Z9kXLJKj261J138i
