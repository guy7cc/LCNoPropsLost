using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

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

            int i = 0;
            for (; i < code.Count - 2; i++)
            {
                LCNoPropsLostPlugin.Logger.LogInfo(i + code[i].opcode.ToString());
                if (code[i].opcode == OpCodes.Call
                    && code[i].Calls(getter)
                    && code[i + 1].opcode == OpCodes.Ldfld
                    && code[i + 1].LoadsField(field)
                    && code[i + 2].opcode == OpCodes.Brfalse)
                {
                    insertionIndex = i;
                    label = (Label)code[i + 2].operand;
                    break;
                }
            }

            if (insertionIndex != -1)
            {
                var instructionsToInsert = new List<CodeInstruction>();
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Br, label));
                code.InsertRange(insertionIndex, instructionsToInsert);
            }

            return code;
        }
    }
}
