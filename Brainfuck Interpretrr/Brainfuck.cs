using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;

namespace Brainfuck_Interpreter
{
    public class Brainfuck
    {
        /// <summary>
        /// Translates the brainfuck code into its valid C# counterpart
        /// </summary>
        /// <param name="brainfuck">The code to translate</param>
        /// <returns>The C# program to run</returns>
        private static string Translate(string brainfuck)
        {
            Dictionary<char, string> dictionary = new Dictionary<char, string>();
            dictionary.Add('>', "p++;");                                // Move the pointer to the right
            dictionary.Add('<', "p--;");                                // Move the pointer to the left
            dictionary.Add('+', "m[p]++;");                             // Increment the memory cell under the pointer
            dictionary.Add('-', "m[p]++;");                             // Decrement the memory cell under the pointer
            dictionary.Add('.', "Console.Write(m[p]);");                // Output the character signified by the cell at the pointer
            dictionary.Add(',', "m[p] = (char)Console.Read();");        // Input a character and store it in the cell at the pointer
            dictionary.Add('[', "while(m[p]!=0){");                     // Jump past the matching ']' if the cell under the pointer is 0
            dictionary.Add(']', "}");                                   // Jump back to the matching '[' if the cell under the pointer is nonzero

            return "using System;class Program{public static void Main(){var p = 0; var m = new char[50000];" +
                string.Join(string.Empty, brainfuck.Where(c => dictionary.ContainsKey(c)).Select(c => dictionary[c])) + "}}";
        }

        /// <summary>
        /// Compiles and Assembles brainfuck 
        /// </summary>
        /// <param name="brainfuck">The brainfuck source code to compile and assemble</param>
        /// <returns></returns>
        private static Assembly CompileAndAssemble(string brainfuck)
        {
            string csharp = Translate(brainfuck);                                                                   // Translates brainfuck into csharp code
            CSharpCodeProvider compiler = new CSharpCodeProvider();                                                 // Initializes CSharpCodeProvider which will be used as a compiler
            CompilerParameters compilerParameters = new CompilerParameters();                                       // Initializes CompilerParameters which will be used to specify compile time parameters (duh)
            compilerParameters.ReferencedAssemblies.Add("System.dll");                                              // Adds System.dll as a referenced assembly file
            CompilerResults compilerResults = compiler.CompileAssemblyFromSource(compilerParameters, csharp);       // Returns compiler results (e.g. errors(if any), the assembly, etc)
            Assembly assembly = compilerResults.CompiledAssembly;                                                   // Gets the compiled assembly from compilerResults

            return assembly;                                                                                        // Returns the assembly
        }

        /// <summary>
        /// Interprets and runs the brainfuck code
        /// </summary>
        /// <param name="brainfuck">The brainfuck code to run</param>
        public static void Run(string brainfuck)
        {

            Assembly assembly = CompileAndAssemble(brainfuck);                                              // Compiles and assembles the brainfuck code
            object instance = assembly.CreateInstance("Program");                                           // Creates new instance of the resulting program
            instance.GetType().InvokeMember("Main", BindingFlags.InvokeMethod, null, instance, null);       // Runs the instance provided by "instance"
        }

        /// <summary>
        /// Interprets and runs the brainfuck code
        /// </summary>
        /// <param name="brainfuck">The brainfuck code to run</param>
        public static void Run(FileInfo brainfuck)
        {

            StreamReader fileReader = new StreamReader(brainfuck.FullName);                                 // Create a StreamReader based on the file info from the parameters
            string brainfuckStream = fileReader.ReadToEnd();                                                // put the entire file into string "brainfuckStream" so it can be translated to C#
            fileReader.Dispose();                                                                           // Releases resources used by the StreamReader
            Assembly assembly = CompileAndAssemble(brainfuckStream);                                        // Compiles and assembles the brainfuck code
            object instance = assembly.CreateInstance("Program");                                           // Creates new instance of the resulting program
            instance.GetType().InvokeMember("Main", BindingFlags.InvokeMethod, null, instance, null);       // Runs the instance provided by "instance"
        }
    }
}
