﻿using System;
using Spruce.Attribs;
using Spruce.Tokens;
using XSharp.Tokens;
using XSharp.x86;
using XSharp.x86.Params;
using Reg = XSharp.Tokens.Reg;
using Reg08 = XSharp.Tokens.Reg08;
using Reg16 = XSharp.Tokens.Reg16;
using Reg32 = XSharp.Tokens.Reg32;
using Size = XSharp.x86.Params.Size;

namespace XSharp.Emitters
{
    /// <summary>
    /// The class that provides assignments for different types.
    /// </summary>
    /// <seealso cref="XSharp.Emitters.Emitters" />
    public class Assignments : Emitters
    {
        public Assignments(Compiler aCompiler, x86.Assemblers.Assembler aAsm) : base(aCompiler, aAsm)
        {
        }

        // EAX = [EBX]
        [Emitter(typeof(Reg), typeof(OpEquals), typeof(OpOpenBracket), typeof(Reg), typeof(OpCloseBracket))]
        protected void MemoryAssignToReg(Register aRegister, string aOpEquals, string aOpOpenBracket, Register aSourceRegister, string aOpCloseBracket)
        {
            Asm.Emit(OpCode.Mov, aRegister, aRegister.RegSize, new Address(aSourceRegister));
        }

        [Emitter(typeof(Reg08), typeof(OpEquals), typeof(Reg08))] // AH = BH
        [Emitter(typeof(Reg16), typeof(OpEquals), typeof(Reg16))] // AX = BX
        [Emitter(typeof(Reg32), typeof(OpEquals), typeof(Reg32))] // EAX = EBX
        protected void RegAssigReg(Register aDestReg, string aEquals, Register aSrcReg)
        {
            Asm.Emit(OpCode.Mov, aDestReg, aSrcReg);
        }

        // [EAX] = EAX
        [Emitter(typeof(OpOpenBracket), typeof(Reg), typeof(OpCloseBracket), typeof(OpEquals), typeof(Reg))]
        protected void RegAssignToMemory(string aOpOpenBracket, Register aTargetRegisterRoot, string aOpCloseBracket, string aOpEquals, Register source)
        {
            Asm.Emit(OpCode.Mov, source.RegSize, new Address(aTargetRegisterRoot), source);
        }

        // [EAX] = .varname
        [Emitter(typeof(OpOpenBracket), typeof(Reg), typeof(OpCloseBracket), typeof(OpEquals), typeof(Variable))]
        protected void VariableAssignToMemory(string aOpOpenBracket, Register aTargetRegisterRoot, string aOpCloseBracket, string aOpEquals, Address source)
        {
            Asm.Emit(OpCode.Mov, "dword", new Address(aTargetRegisterRoot), source.AddPrefix(Compiler.GetPrefixForVar));
        }

        // [EAX] = 0x10
        [Emitter(typeof(OpOpenBracket), typeof(Reg), typeof(OpCloseBracket), typeof(OpEquals), typeof(Int32u))]
        protected void IntegerAssignToMemory(string aOpOpenBracket, Register aTargetRegisterRoot, string aOpCloseBracket, string aOpEquals, object source)
        {
            Asm.Emit(OpCode.Mov, "dword", new Address(aTargetRegisterRoot), source);
        }

        // Don't use Reg. This method ensures proper data sizes.
        // This could be combined with RegAssignReg by using object type for last arg, but this is a bit cleaner to separate.
        [Emitter(typeof(Reg08), typeof(OpEquals), typeof(Int08u))] // AH = 0
        [Emitter(typeof(Reg16), typeof(OpEquals), typeof(Int16u))] // AX = 0
        [Emitter(typeof(Reg32), typeof(OpEquals), typeof(Int32u))] // EAX = 0
        protected void RegAssigNum(Register aDestReg, string aEquals, object aVal)
        {
            Asm.Emit(OpCode.Mov, aDestReg, aVal);
        }

        // AX = #Test
        [Emitter(typeof(Reg), typeof(OpEquals), typeof(Const))]
        protected void RegAssigConst(Register aReg, string aEquals, string aVal)
        {
            Asm.Emit(OpCode.Mov, aReg, $"{Compiler.GetPrefixForConst}{aVal}");
        }

        // EAX = .Varname
        [Emitter(typeof(Reg), typeof(OpEquals), typeof(Variable))]
        protected void RegAssigVar(Register aReg, string aEquals, Address aVal)
        {
            Asm.Emit(OpCode.Mov, aReg, aReg.RegSize, aVal.AddPrefix(Compiler.GetPrefixForVar));
        }

        // AL = @.varname
        [Emitter(typeof(Reg), typeof(OpEquals), typeof(VariableAddress))]
        protected void RegAssignVarAddr(Register aReg, string aEquals, string aVal)
        {
            Asm.Emit(OpCode.Mov, aReg, $"{Compiler.GetPrefixForConst}{aVal}");
        }
    }
}