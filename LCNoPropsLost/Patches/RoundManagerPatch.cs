using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace LCNoPropsLost.Patches
{
	[HarmonyPatch(typeof(RoundManager))]
	class RoundManagerPatch
    {
		[HarmonyPatch("DespawnPropsAtEndOfRound")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> keepProps(IEnumerable<CodeInstruction> instructions, ILGenerator il)
		{
			var code = new List<CodeInstruction>(instructions);
			int insertionIndex = -1;
			Label label = il.DefineLabel();

			var type = typeof(StartOfRound);
			var property = type.GetProperty("Instance");
			var getter = property.GetMethod;
			var field = type.GetField("allPlayersDead");

			object target = null;

			for (int i = 0; i < code.Count - 2; i++)
			{
				if (code[i].opcode == OpCodes.Call 
					&& code[i].Calls(getter) 
					&& code[i + 1].opcode == OpCodes.Ldfld 
					&& code[i + 1].LoadsField(field)
					&& code[i + 2].opcode == OpCodes.Brfalse)
                {
					insertionIndex = i;
					code[i].labels.Add(label);
					target = code[i + 2].operand;
					break;
				}
			}
			var instructionsToInsert = new List<CodeInstruction>();
			instructionsToInsert.Add(new CodeInstruction(OpCodes.Br, target));

			if (insertionIndex != -1)
			{
				code.InsertRange(insertionIndex, instructionsToInsert);
			}
			return code;
		}
	}
}
