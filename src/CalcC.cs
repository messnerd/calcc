using System;
using System.Linq;
using static CalcC.TokenType;
using System.Text.RegularExpressions;

namespace CalcC
{
    public partial class CalcC
    {
        public string Cil { get; set; }

        public void CompileToCil(string src)
        {
            // Emit the preamble
            var cil = Preamble();

            // Tokenize the input string (in this case,
            // just split on spaces).
            var tokens = src.Split(' ').Select(t => t.Trim());

            //var stack = new Stack<int>();
            //var dictionary = new Dictionary<char,int>(); //not used?

            foreach (var token in tokens)
            {
                var tokenType = GetTokenType(token);

                if (tokenType == TokenType.Number)
                {
                    cil += @"
    ldloc.0
    ldc.i4.s " + token + @"
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)";
                }
                else if (tokenType == TokenType.BinaryOperator)
                {
                    if (token == "+")
                    {
                        cil += @"
    ldloc.0
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    add
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)";
                    }
                    if (token == "-")
                    {
                        cil += @"
    ldloc.0
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    sub
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)";
                    }
                    if (token == "*")
                    {
                        cil += @"
    ldloc.0
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    mul
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)";
                    }
                    if (token == "/")
                    {
                        cil += @"
    ldloc.0
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    div
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)";
                    }
                    if (token == "%")
                    {
                        cil += @"
    ldloc.0
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    rem
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)";
                    }
                }
                else
                {
                    throw new Exception();
                }
                // TODO:
                // Finish the code in this loop to emit
                // the correct CIL instructions based on the
                // tokenType and the token's value.
                //
                // Hint: this will likely be a big
                // switch-case statement or a bunch of
                // if-else-if statements.
                //
                // To emit the instructions, all you 
                // have to do is say
                //   `cil += "...";
                // to append the instructions to the output.
                //
                // If you get stuck, think about what the
                // code would look like in C# and use
                // sharplab.io to see what the CIL would be.
            }

            // Emit the postamble.
            cil += Postamble();

            Cil = cil;
        }

        // Returns the type of token represented by the string
        private static TokenType GetTokenType(string token)
        {
            switch (token)
            {
                case var x when new Regex(@"[0-9]").IsMatch(token):
                    return TokenType.Number;

                // case var x when new Regex(@"[]").IsMatch(token):
                //     return TokenType.UnaryOperator;

                case var x when new Regex(@"[+\-*\/%]").IsMatch(token):
                    return TokenType.BinaryOperator;

                // case var x when new Regex(@"[]").IsMatch(token):
                //     return TokenType.StoreInstruction;

                // case var x when new Regex(@"[]").IsMatch(token):
                //     return TokenType.RetrieveInstruction;

                // case var x when new Regex(@"[]").IsMatch(token):
                //     return TokenType.Blank;

                default:
                    return TokenType.Unknown;
            }
        }

        // Preamble:
        // * Initialize the assembly
        // * Declare `static void main()` function
        // * Declare two local variables: the Stack and the registers Dictionary<>
        // * Call the constructors on the Stack<> and the registers Dictionary<>
        //
        // Note the @"..." string construct; this is for multiline strings.
        private static string Preamble()
        {
            return @"
// Preamble
.assembly _ { }
.assembly extern System.Collections {}
.assembly extern System.Console {}
.assembly extern System.Private.CoreLib {}

.method public hidebysig static void main() cil managed
{
    .entrypoint
    .maxstack 3

    // Declare two local vars: a Stack<int> and a Dictionary<char, int>
    // Why do we need these? All test cases are int32, why cant we just load values directly?
    .locals init (
        [0] class [System.Collections]System.Collections.Generic.Stack`1<int32> stack,
        [1] class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32> registers
    )

    // Initialize the Stack<>
    newobj instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::.ctor()
    stloc.0
    // Initialize the Dictionary<>
    newobj instance void class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32>::.ctor()
    stloc.1
";
        }

        // Postamble.  Pop the top of the stack and print whatever is there.
        private static string Postamble()
        {
            return @"
    // Pop the top of the stack and print it
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    call void [System.Console]System.Console::WriteLine(int32)

    ret
}";
        }
    }
}
