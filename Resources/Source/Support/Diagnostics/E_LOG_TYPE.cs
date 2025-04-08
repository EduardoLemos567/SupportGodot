using System;

namespace Support.Diagnostics;

[Flags]
public enum E_LOG_TYPE : byte
{
    NONE = 0b0,
    CONSOLE = 0b1,
    FILE = 0b10,
    NETWORK = 0b100,
    ALL = 0b111
}
