using HarmonyLib;
using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnboundLib;
using UnityEngine;

namespace RWF.Patches
{
    [HarmonyPatch(typeof(CardBarHandler), "Update")]
    class CardBarHandler_Patch_Update
    {
        static bool AtInstruction(List<CodeInstruction> instructions, int index, CodeInstruction[] compareInstructions, int compareAt = 0)
        {
            for (int i = 0; i < compareInstructions.Length; i++)
            {
                if (index + (i - compareAt) >= instructions.Count || !CodeInstructionEquals(instructions[index + (i - compareAt)], compareInstructions[i]))
                {
                    return false;
                }
            }
            return true;
        }
        static bool CodeInstructionEquals(CodeInstruction instruction, CodeInstruction other)
        {
            if (instruction.operand is Label)
            {
                return instruction.opcode == other.opcode;
            }
            return instruction.opcode == other.opcode && instruction.operand == other.operand;
        }
        static void Prefix(CardBarHandler __instance, CardBar[] ___cardBars, ref float ___m_lastHoverTime, ref bool ___m_dpadHover)
        {
            foreach (InputDevice inputDevice in InputManager.ActiveDevices)
            {
                if (inputDevice.DPadUp.WasPressed)
                {
                    object[] tryGetHoveredBarArgs = new object[] { null };
                    if ((bool)__instance.GetPropertyValue("NoneHovers"))
                    {
                        ___cardBars[0].OnHover(0);
                        ___m_lastHoverTime = Time.unscaledTime;
                        ___m_dpadHover = true;
                    }
                    else if ((bool)__instance.InvokeMethod("TryGetHoveredBar", tryGetHoveredBarArgs) && (CardBar)tryGetHoveredBarArgs[0] != ___cardBars[0])
                    {
                        CardBar hoveredBar = (CardBar)tryGetHoveredBarArgs[0];
                        if (___cardBars[Array.IndexOf(___cardBars, hoveredBar)-1].OnHover(hoveredBar.CurrentHoverIndex))
                        {
                            hoveredBar.StopHover();
                            ___m_lastHoverTime = Time.unscaledTime;
                            ___m_dpadHover = true;
                        }
                    }
                    else
                    {
                        ___m_lastHoverTime = -100f;
                    }
                }
                else if (inputDevice.DPadDown.WasPressed)
                {
                    object[] tryGetHoveredBarArgs = new object[] { null };
                    if ((bool) __instance.InvokeMethod("TryGetHoveredBar", tryGetHoveredBarArgs) && (CardBar) tryGetHoveredBarArgs[0] != ___cardBars.Last())
                    {
                        CardBar hoveredBar = (CardBar) tryGetHoveredBarArgs[0];
                        if (___cardBars[Array.IndexOf(___cardBars, hoveredBar) + 1].OnHover(hoveredBar.CurrentHoverIndex))
                        {
                            hoveredBar.StopHover();
                            ___m_lastHoverTime = Time.unscaledTime;
                            ___m_dpadHover = true;
                        }
                    }
                    else
                    {
                        ___m_lastHoverTime = -100f;
                    }
                }
            }
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();
            var DPadUPPressed = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Callvirt, UnboundLib.ExtensionMethods.GetMethodInfo(typeof(InputDevice), "get_DPadUp")),
                new CodeInstruction(OpCodes.Callvirt, UnboundLib.ExtensionMethods.GetMethodInfo(typeof(InputControl), "get_WasPressed")),
                new CodeInstruction(OpCodes.Brfalse)
            };
            var DPadLeftPressed = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Callvirt, UnboundLib.ExtensionMethods.GetMethodInfo(typeof(InputDevice), "get_DPadLeft")),
                new CodeInstruction(OpCodes.Callvirt, UnboundLib.ExtensionMethods.GetMethodInfo(typeof(InputControl), "get_WasPressed")),
                new CodeInstruction(OpCodes.Brfalse)
            };
            bool removing = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (AtInstruction(list, i, DPadUPPressed))
                {
                    removing = true;
                } else if (AtInstruction(list, i, DPadLeftPressed))
                {
                    removing = false;
                }
                if (!removing)
                {
                    yield return list[i];
                }
            }
        }
    }
}
