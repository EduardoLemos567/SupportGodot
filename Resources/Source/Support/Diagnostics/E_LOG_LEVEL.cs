using System;

namespace Support.Diagnostics;

[Flags]
public enum E_LOG_LEVEL : byte
{
    NONE = 0b0,
    ERROR = 0b1,
    WARNING = 0b10,
    INFO = 0b1000,
    DEBUG = 0b10000,
    LAP = 0b100000, // Used to print timings
    ALL = 0b111111
}
