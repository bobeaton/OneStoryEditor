using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections;
using System.Diagnostics;

namespace NetLoc
{
	class ILInstruction
	{
		public readonly OpCode opCode;
		public readonly object operand;

		public ILInstruction(OpCode opCode, object operand)
		{
			this.opCode = opCode;
			this.operand = operand;
		}
	}

	class ILReader : IEnumerable<ILInstruction>
	{
		Byte[] m_byteArray;
		Int32 m_position;
		MethodBase m_enclosingMethod;

		static OpCode[] s_OneByteOpCodes = new OpCode[0x100];
		static OpCode[] s_TwoByteOpCodes = new OpCode[0x100];

		static ILReader()
		{
			foreach (FieldInfo fi in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				OpCode opCode = (OpCode)fi.GetValue(null);
				UInt16 value = (UInt16)opCode.Value;
				if (value < 0x100)
					s_OneByteOpCodes[value] = opCode;
				else if ((value & 0xff00) == 0xfe00)
					s_TwoByteOpCodes[value & 0xff] = opCode;
			}
		}

		public static List<string> FindStringCalls(MethodBase caller, MethodInfo callee)
		{
			List<string> strs = new List<string>();

			Module module = caller.Module;

			ILReader reader = new ILReader(caller);
			List<ILInstruction> instrs = new List<ILInstruction>(reader);
			for (int i = 1; i < instrs.Count; i++)
			{
				if (instrs[i].opCode == OpCodes.Call)
				{
					Type[] genericTypeArguments = null;
					Type[] genericMethodArguments = null;
					genericTypeArguments = caller.DeclaringType.GetGenericArguments();
					if ((!caller.IsConstructor) && (!caller.Name.Equals(".cctor")))
						genericMethodArguments = caller.GetGenericArguments();

					if (callee.Equals(module.ResolveMethod((int)instrs[i].operand,
						genericTypeArguments, genericMethodArguments)))
					{
						// Get string
                        if (instrs[i - 1].opCode == OpCodes.Ldstr)
                        {
                            string str = module.ResolveString((int)instrs[i - 1].operand);
                            strs.Add(str);
                        }
                        else
                            Debug.Print("Call to {0} in {1} ({2}) was not a string call", callee, caller.Name, caller.DeclaringType.Name);
					}
				}
			}
			return strs;
		}

		public ILReader(MethodBase enclosingMethod)
		{
			this.m_enclosingMethod = enclosingMethod;
			MethodBody methodBody = m_enclosingMethod.GetMethodBody();
			this.m_byteArray = (methodBody == null) ? new Byte[0] : methodBody.GetILAsByteArray();
			this.m_position = 0;
		}

		public IEnumerator<ILInstruction> GetEnumerator()
		{
			while (m_position < m_byteArray.Length)
				yield return Next();

			m_position = 0;
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

		ILInstruction Next()
		{
			Int32 offset = m_position;
			OpCode opCode = OpCodes.Nop;

			// read first 1 or 2 bytes as opCode
			Byte code = ReadByte();
			if (code != 0xFE)
				opCode = s_OneByteOpCodes[code];
			else
			{
				code = ReadByte();
				opCode = s_TwoByteOpCodes[code];
			}

			object operand = null;

			switch (opCode.OperandType)
			{
				case OperandType.InlineNone: operand = null; break;
				case OperandType.ShortInlineBrTarget: operand = ReadSByte(); break;
				case OperandType.InlineBrTarget: operand = ReadInt32(); break;
				case OperandType.ShortInlineI: operand = ReadByte(); break;
				case OperandType.InlineI: operand = ReadInt32(); break;
				case OperandType.InlineI8: operand = ReadInt64(); break;
				case OperandType.ShortInlineR: operand = ReadSingle(); break;
				case OperandType.InlineR: operand = ReadDouble(); break;
				case OperandType.ShortInlineVar: operand = ReadByte(); break;
				case OperandType.InlineVar: operand = ReadUInt16(); break;
				case OperandType.InlineString: operand = ReadInt32(); break;
				case OperandType.InlineSig: operand = ReadInt32(); break;
				case OperandType.InlineField: operand = ReadInt32(); break;
				case OperandType.InlineType: operand = ReadInt32(); break;
				case OperandType.InlineTok: operand = ReadInt32(); break;
				case OperandType.InlineMethod: operand = ReadInt32(); break;

				case OperandType.InlineSwitch:
					Int32 cases = ReadInt32();
					Int32[] deltas = new Int32[cases];
					for (Int32 i = 0; i < cases; i++)
						deltas[i] = ReadInt32();
					operand = deltas;
					break;
				default:
					throw new BadImageFormatException("unexpected OperandType " + opCode.OperandType);
			}
			return new ILInstruction(opCode, operand);
		}

		Byte ReadByte() { return (Byte)m_byteArray[m_position++]; }
		SByte ReadSByte() { return (SByte)ReadByte(); }

		UInt16 ReadUInt16() { m_position += 2; return BitConverter.ToUInt16(m_byteArray, m_position - 2); }
		UInt32 ReadUInt32() { m_position += 4; return BitConverter.ToUInt32(m_byteArray, m_position - 4); }
		UInt64 ReadUInt64() { m_position += 8; return BitConverter.ToUInt64(m_byteArray, m_position - 8); }

		Int32 ReadInt32() { m_position += 4; return BitConverter.ToInt32(m_byteArray, m_position - 4); }
		Int64 ReadInt64() { m_position += 8; return BitConverter.ToInt64(m_byteArray, m_position - 8); }

		Single ReadSingle() { m_position += 4; return BitConverter.ToSingle(m_byteArray, m_position - 4); }
		Double ReadDouble() { m_position += 8; return BitConverter.ToDouble(m_byteArray, m_position - 8); }
	}
}