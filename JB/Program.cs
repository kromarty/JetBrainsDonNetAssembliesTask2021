using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace JB
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("enter your Build path");
            string fileName = Console.ReadLine();
            Console.WriteLine("Enter output path");
            string outputPath = Console.ReadLine();
            Directory.CreateDirectory(outputPath);
            ModuleDefinition module = ModuleDefinition.ReadModule(fileName);
            MethodReference decimalSub = module.ImportReference(typeof(decimal).GetMethod("op_Subtraction"));
            var subInstruction = Instruction.Create(OpCodes.Call, decimalSub).Operand;
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody)
                        continue;
                    var ilProcessor = method.Body.GetILProcessor();
                    foreach(var instruction in ilProcessor.Body.Instructions)
                    {
                        if (instruction.OpCode == OpCodes.Add)
                        {
                            instruction.OpCode = OpCodes.Sub;
                        }
                        else if (instruction.Operand != null)
                        {
                            if (instruction.Operand.ToString().Contains("op_Addition"))
                                instruction.Operand = subInstruction;
                        }
                    }
                }
            }
            module.Write(outputPath + "\\" + Path.GetFileNameWithoutExtension(module.Name) + "_modified" + Path.GetExtension(module.Name));
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
