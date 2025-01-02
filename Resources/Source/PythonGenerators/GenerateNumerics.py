# OLD and not used anymore since GodotSupport now can support .net 8.0 with INumber<N>.

from pathlib import Path

USING = (
    "using Godot;",
    "using System;",
    "using System.Diagnostics;",
    "using System.Runtime.CompilerServices;",
)
IDENTATION = "    "
NAMESPACE = "namespace Support.Numerics;"
NEWLINE = "\n"
TYPES = ("int", "float", "double", "bool")
CAP_TYPES = tuple(t.capitalize() for t in TYPES)
VECTOR_LETTERS = ("x", "y", "z", "w")
FIRST_VECTOR_ARG = "v1"
SECOND_VECTOR_ARG = "v2"
SINGLE_VECTOR_ARG = "v"
NUMERIC_ARG = "n"
COMA = ", "
FOLDER = Path(__file__).parent


class BaseGenerator:
    def __init__(self, typeId: int, vectorSize: int):
        self.typeId = typeId
        self.vectorSize = vectorSize
        self.result = ""
        self.identation = 0
        self.lastRegion = ""

    def write(self, line: str, newlines: int = 1):
        self.result += f"{IDENTATION * self.identation}{line}{NEWLINE * newlines}"

    def gen_format(self, piece: str, i: int = 0, **additional):
        return piece.format(
            NL=NEWLINE,
            L=VECTOR_LETTERS[i],
            T=self.gen_vector_type(),
            BT=self.gen_vector_type(0, 3),
            NT=self.gen_numeric_type(),
            VA1=FIRST_VECTOR_ARG,
            VA2=SECOND_VECTOR_ARG,
            VA=SINGLE_VECTOR_ARG,
            NA=NUMERIC_ARG,
            **additional,
        )

    def write_ident(self):
        self.write("{")
        self.ident(1)

    def write_unident(self, extra: str = None):
        self.ident(-1)
        self.write("}" if extra == None else "}" + extra)

    def write_region_start(self, name: str):
        self.write(f"#region {name}")
        self.lastRegion = name

    def write_region_end(self):
        self.write(f"#endregion {self.lastRegion}")

    def write_inline_tip(self):
        self.write("[MethodImpl(MethodImplOptions.AggressiveInlining)]")

    def write_hide_tip(self):
        self.write("[DebuggerBrowsable(DebuggerBrowsableState.Never)]")

    def ident(self, level: int):
        self.identation += level

    def gen_sequence(
        self, piece: str = "{L}", separator: str = COMA, size: int = 0, **additional
    ) -> str:
        if size == 0:
            size = self.vectorSize
        return separator.join(
            self.gen_format(piece, i, **additional) for i in range(size)
        )

    def gen_vector_type(self, size: int = 0, typeId: int = None):
        if size == 0:
            size = self.vectorSize
        if typeId is None:
            typeId = self.typeId
        return CAP_TYPES[typeId] + str(size)

    def gen_numeric_type(self):
        return TYPES[self.typeId]

    def write_func_header(
        self,
        name: str,
        body: str,
        args: str = "",
        returnType: str = None,
        static: bool = False,
        override: bool = False,
        readonly: bool = True,
    ):
        if returnType is None:
            returnType = self.gen_numeric_type()
        self.write(
            self.gen_format(
                "public {ST}{OV}{RO}{RT} {NAME}({ARGS}) => {BODY}",
                ST="static " if static else "",
                OV="override " if override else "",
                RO="readonly " if readonly else "",
                RT=returnType,
                NAME=name,
                ARGS=args,
                BODY=body,
            )
        )

    def write_array_getter_header(
        self, returnType: str, args: str = "", body: str = None, readonly: bool = False
    ):
        self.write(
            self.gen_format(
                "public {RO}{RT} this[{ARGS}]{BODY}",
                RO="readonly " if readonly else "",
                RT=returnType,
                ARGS=args,
                BODY=f" => {body}" if body is not None else "",
            )
        )

    def write_object_operators(self):
        self.write_region_start("OBJECT_OPERATORS")
        self.write_inline_tip()
        self.write_func_header(
            "Equals",
            self.gen_format("obj is {T} {VA} && Equals({VA});"),
            "object obj",
            "bool",
            False,
            True,
            True,
        )
        self.write_inline_tip()
        self.write_func_header(
            "Equals",
            self.gen_sequence("{L} == {VA}.{L}", " && ") + ";",
            self.gen_format("{T} {VA}"),
            "bool",
            False,
            False,
            True,
        )
        self.write_inline_tip()
        self.write_func_header(
            "GetHashCode",
            f"HashCode.Combine({self.gen_sequence()});",
            "",
            "int",
            False,
            True,
            True,
        )
        self.write_func_header(
            "ToString",
            f'$"{self.gen_vector_type()}({self.gen_sequence("{{{L}}}")})";',
            "",
            "string",
            False,
            True,
            True,
        )
        self.write_region_end()

    def write_operator_function1(self, signal: str):
        self.write_inline_tip()
        body_sequence = self.gen_sequence("{VA}.{L} {S} {NA}", S=signal)
        self.write_func_header(
            f"operator {signal}",
            f"new({body_sequence});",
            self.gen_format("in {T} {VA}, {NT} {NA}"),
            self.gen_vector_type(),
            True,
            False,
            False,
        )

    def write_operator_function2(self, signal: str, returnType: str):
        self.write_inline_tip()
        body_sequence = self.gen_sequence("{VA1}.{L} {S} {VA2}.{L}", S=signal)
        self.write_func_header(
            f"operator {signal}",
            f"new({body_sequence});",
            self.gen_format("in {T} {VA1}, in {T} {VA2}"),
            returnType,
            True,
            False,
            False,
        )


class GenerateNumeric(BaseGenerator):
    def generate(self):
        self.write(NEWLINE.join(USING), 2)
        self.write(NAMESPACE, 2)
        self.write(self.gen_format("public struct {T} : IEquatable<{T}>"))
        self.write_ident()
        self.write_properties()
        self.write_swizzles()
        self.write_constructors()
        self.write_math_operators()
        self.write_logical_operators()
        self.write_object_operators()
        self.write_functions()
        self.write_converters()
        self.write_unident()

    def write_properties(self):
        self.write_region_start("PROPERTIES")
        for name, vx, vy in (
            ("Zero", 0, 0),
            ("One", 1, 1),
            ("Up", 0, 1),
            ("Down", 0, -1),
            ("Left", -1, 0),
            ("Right", 1, 0),
        ):
            self.write(
                self.gen_format(
                    "public static readonly {T} {NAME} = new({VX}, {VY});",
                    NAME=name,
                    VX=vx,
                    VY=vy,
                )
            )
        self.write(self.gen_format("public {NT} {SEQ};", SEQ=self.gen_sequence()))
        self.write_region_end()

    def write_constructors(self):
        self.write_region_start("CONSTRUCTORS")
        self.write(
            self.gen_format(
                "public {T}({NT} n) {{ {SEQ}; }}",
                SEQ=self.gen_sequence("{L} = n", "; "),
            )
        )
        self.write(
            self.gen_format(
                "public {T}({SEQ1}) {{ {SEQ2} }}",
                SEQ1=self.gen_sequence("{NT} {L} = 0"),
                SEQ2=self.gen_sequence("this.{L} = {L};", " "),
            )
        )
        self.write_region_end()

    def write_swizzles(self):
        def write_function(size: int):
            has_setter = size <= self.vectorSize
            getter_body = self.gen_sequence("this[i{L}]", size=size)
            setter_body = self.gen_sequence("this[i{L}] = value.{L};", " ", size)
            self.write_hide_tip()
            if has_setter:
                self.write_array_getter_header(
                    self.gen_vector_type(size),
                    self.gen_sequence("int i{L}", size=size),
                )
                self.write_ident()
                self.write(f"readonly get => new({getter_body});")
                self.write(f"set {{ {setter_body} }}")
                self.write_unident()
            else:
                # write a readonly one line getter
                self.write_array_getter_header(
                    self.gen_vector_type(size),
                    self.gen_sequence("int i{L}", size=size),
                    f"new({getter_body});",
                )

        self.write_region_start("SWIZZLES")
        self.write_hide_tip()
        self.write(f"public {self.gen_numeric_type()} this[int ix]")
        self.write_ident()
        self.write("readonly get")
        self.write_ident()
        self.write("return ix switch")
        self.write_ident()
        for i in range(self.vectorSize):
            self.write(f"{i} => {VECTOR_LETTERS[i]},")
        self.write('_ => throw new ArgumentException("Index out of valid range")')
        self.write_unident(";")
        self.write_unident()
        self.write("set")
        self.write_ident()
        self.write("switch (ix)")
        self.write_ident()
        for i in range(self.vectorSize):
            self.write(f"case {i}: {VECTOR_LETTERS[i]} = value; break;")
        self.write('default: throw new ArgumentException("Index out of valid range");')
        self.write_unident()
        self.write_unident()
        self.write_unident()
        write_function(2)
        write_function(3)
        write_function(4)
        self.write_region_end()

    def write_math_operators(self):
        self.write_region_start("MATH_OPERATORS")
        signals = ("+", "-", "*", "/", "%")
        for signal in signals:
            self.write_operator_function2(signal, self.gen_vector_type())
        for signal in signals:
            self.write_operator_function1(signal)
        self.write_inline_tip()
        self.write(
            self.gen_format(
                "public static {T} operator -(in {T} v) => {BODY};",
                BODY=self.gen_format("new({SEQ})", SEQ=self.gen_sequence("-v.{L}")),
            )
        )
        self.write_region_end()

    def write_logical_operators(self):
        self.write_region_start("LOGICAL_OPERATORS")
        for signal in (">", ">=", "<", "<=", "==", "!="):
            self.write_operator_function2(signal, self.gen_vector_type(0, 3))
        self.write_region_end()

    def write_functions(self):
        def write_inline(name: str, body: str, args: str = "", returnType: str = None):
            if returnType is None:
                returnType = self.gen_numeric_type()
            self.write_inline_tip()
            self.write_func_header(
                name,
                body,
                args,
                returnType,
            )

        def gen_chained_func(func: str):
            i = self.vectorSize - 1
            body = VECTOR_LETTERS[i]
            i -= 1
            while i >= 0:
                body = f"{func}({VECTOR_LETTERS[i]}, {body})"
                i -= 1
            return body + ";"

        def write_inline_multiple_line(name: str, func_piece: str, args: str = ""):
            write_inline(
                name,
                "new(",
                args,
                self.gen_vector_type(),
            )
            self.ident(1)
            for i in range(self.vectorSize):
                f = self.gen_format(func_piece)
                self.write(
                    self.gen_format(
                        "{F}{TERM}",
                        F=f,
                        TERM="," if i < self.vectorSize - 1 else ");",
                    )
                )
            self.ident(-1)

        sqrt_function = "Toolbox.ISqrt" if self.typeId == 0 else "Mathf.Sqrt"
        self.write_region_start("FUNCTIONS")
        write_inline("SqrMagnitude", self.gen_sequence("{L} * {L}", " + ") + ";")
        write_inline("Magnitude", f"{sqrt_function}(SqrMagnitude());")
        write_inline(
            "Normalized", "this / Magnitude();", returnType=self.gen_vector_type()
        )
        write_inline(
            "Abs",
            self.gen_format("new({SEQ})", SEQ=self.gen_sequence("Mathf.Abs({L})"))
            + ";",
            returnType=self.gen_vector_type(),
        )
        write_inline("MinValue", gen_chained_func("Mathf.Min"))
        write_inline("MaxValue", gen_chained_func("Mathf.Max"))
        write_inline(
            "SqrDistance",
            "(other - this).SqrMagnitude();",
            f"in {self.gen_vector_type()} other",
        )
        write_inline(
            "Distance",
            self.gen_format("{SQRT}(SqrDistance({VA}));", SQRT=sqrt_function),
            self.gen_format("in {SEQ} {VA}", SEQ=self.gen_vector_type()),
        )
        write_inline_multiple_line(
            "Min",
            "Mathf.Min({L}, {VA}.{L})",
            self.gen_format("in {T} {VA}"),
        )
        write_inline_multiple_line(
            "Max",
            "Mathf.Max({L}, {VA}.{L})",
            self.gen_format("in {T} {VA}"),
        )
        write_inline_multiple_line(
            "Clamp",
            "Mathf.Clamp({L}, min.{L}, max.{L})",
            self.gen_format("in {T} min, in {T} max"),
        )
        self.write_region_end()

    def write_converters(self):
        def write_converter(
            fromType: str,
            toType: str,
            implicit: bool = False,
            upperLetter: bool = False,
            castTo: str = None,
        ):
            seq = COMA.join(
                self.gen_format(
                    "{CAST}{VA}.{LETTER}",
                    CAST="" if castTo is None else f"({castTo})",
                    LETTER=VECTOR_LETTERS[i].upper()
                    if upperLetter
                    else VECTOR_LETTERS[i],
                )
                for i in range(self.vectorSize)
            )
            self.write_func_header(
                f"operator {toType}",
                f"new({seq});",
                self.gen_format(
                    "in {FROM_TYPE} {VA}",
                    FROM_TYPE=fromType,
                ),
                "implicit" if implicit else "explicit",
                True,
                False,
                False,
            )

        def write_godot_converters(godotType):
            write_converter(self.gen_vector_type(), godotType, True, False)
            write_converter(godotType, self.gen_vector_type(), False, True)

        def write_rounders(func):
            piece = "Mathf.{FUNC}ToInt({L})"
            self.write_func_header(
                f"{func}ToInt",
                f"new({self.gen_sequence(piece, FUNC=func)});",
                returnType=self.gen_vector_type(0, 0),
            )

        self.write_region_start("CONVERTERS")
        match (self.typeId):
            case 1 | 2:
                # float->int, double->int
                for f in ("Round", "Ceil", "Floor"):
                    write_rounders(f)
        match (self.typeId):
            case 0:
                write_godot_converters(f"Vector{self.vectorSize}I")
                for i in range(1, 3):
                    write_converter(
                        self.gen_vector_type(0, i),
                        self.gen_vector_type(),
                        castTo=TYPES[0],
                    )
            case 1:
                write_godot_converters(f"Vector{self.vectorSize}")
                # double->float
                write_converter(
                    self.gen_vector_type(0, 2), self.gen_vector_type(), castTo=TYPES[1]
                )
                # int->float
                write_converter(
                    self.gen_vector_type(0, 0), self.gen_vector_type(), True
                )
            case 2:
                # float->double
                write_converter(
                    self.gen_vector_type(0, 1), self.gen_vector_type(), True
                )
                # int->double
                write_converter(
                    self.gen_vector_type(0, 0), self.gen_vector_type(), True
                )
        self.write_region_end()


class GenerateBoolean(BaseGenerator):
    def __init__(self, vectorSize: int):
        super().__init__(3, vectorSize)

    def generate(self):
        self.write(NEWLINE.join(USING), 2)
        self.write(NAMESPACE, 2)
        self.write(self.gen_format("public struct {T} : IEquatable<{T}>"))
        self.write_ident()
        self.write_properties()
        self.write_constructors()
        self.write_logical_operators()
        self.write_object_operators()
        self.write_unident()

    def write_properties(self):
        def write_function(name, body):
            self.write(
                self.gen_format(
                    "public readonly {NT} {NAME} => {BODY}", NAME=name, BODY=body
                )
            )

        self.write_region_start("PROPERTIES")
        self.write(self.gen_format("public {NT} {SEQ};", SEQ=self.gen_sequence()))
        write_function("AllTrue", f"{self.gen_sequence(separator=' && ')};")
        write_function("AllEqual", f"{self.gen_sequence(separator=' == ')};")
        write_function("AnyTrue", f"{self.gen_sequence(separator=' || ')};")
        self.write(
            self.gen_format(
                "public readonly int {NAME} => {BODY};",
                NAME="TrueCount",
                BODY=self.gen_sequence("({L}? 1 : 0)", " + "),
            )
        )
        self.write_region_end()

    def write_constructors(self):
        self.write_region_start("CONSTRUCTORS")
        self.write(
            self.gen_format(
                "public {T}({NT} n) {{ {SEQ}; }}",
                SEQ=self.gen_sequence("{L} = n", "; "),
            )
        )
        self.write(
            self.gen_format(
                "public {T}({SEQ1}) {{ {SEQ2} }}",
                SEQ1=self.gen_sequence("{NT} {L} = false"),
                SEQ2=self.gen_sequence("this.{L} = {L};", " "),
            )
        )
        self.write_region_end()

    def write_logical_operators(self):
        self.write_region_start("LOGICAL_OPERATORS")
        for signal in ("==", "!="):
            self.write_inline_tip()
            self.write_func_header(
                f"operator {signal}",
                self.gen_sequence("{VA1}.{L} {S} {VA2}.{L}", " && ", S=signal) + ";",
                self.gen_format("in {T} {VA1}, in {T} {VA2}"),
                self.gen_numeric_type(),
                True,
                False,
                False,
            )
        self.write_region_end()


def write_file(gen: BaseGenerator):
    file_name = FOLDER / f"{gen.gen_vector_type()}.cs"
    with open(file_name, "w") as file:
        file.write(gen.result)
    print(f"{file_name} writen")


def build():
    for i in range(2, 3 + 2):
        for typeId in range(3):
            gn = GenerateNumeric(typeId, i)
            gn.generate()
            write_file(gn)
        gb = GenerateBoolean(i)
        gb.generate()
        write_file(gb)


if __name__ == "__main__":
    build()