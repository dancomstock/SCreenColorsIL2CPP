using System.Collections;
using MelonLoader;
using SCreenSignCommandsIL2CPP;
using UnityEngine;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(SCreenColorsIL2CPP.Core), "SCreenColorsIL2CPP", "1.0.0", "animandan", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace SCreenColorsIL2CPP
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            Melon<SCreenSignCommandsIL2CPP.Core>.Instance.register_command("/color", "Choose Sign Colors", signcolor);
        }

        public IEnumerator signcolor(Il2CppScheduleOne.EntityFramework.LabelledSurfaceItem instance)
        {
            yield return null;
            string result = "";
            bool signRgbFlag = false;
            bool textrRgbFlag = false;
            var material = instance.gameObject.GetComponentInChildren<MeshRenderer>().material;
            try
            {
                string message = instance.Message;
                var args = SignCommand.getArgs(instance);
                if (args.ContainsKey("signcolor"))
                {
                    if (args["signcolor"] == "rgb")
                    {
                        signRgbFlag = true;
                    }
                    else 
                    {
                        material.color = StringToColor(args["signcolor"]);
                    }
                }

                if (args.ContainsKey("textcolor"))
                {
                    if (args["textcolor"] == "rgb")
                    {
                        textrRgbFlag = true;
                    }
                    else
                    {
                        instance.Label.color = StringToColor(args["textcolor"]);
                    }
                }
                result = args.GetValueOrDefault("remaining_text", "");
            }
            catch (System.Exception ex)
            {
                Melon<Core>.Logger.Msg($"error: {ex}");
                result = ex.ToString();
            }
            instance.Label.text = result;
            if (signRgbFlag || textrRgbFlag)
            {
                while (instance.Message.Contains("signcolor=rgb") || instance.Message.Contains("textcolor=rgb"))
                {
                    yield return null;
                    var args = SignCommand.getArgs(instance);
                    //yield return new WaitForSeconds(args.GetValueOrDefault(waittime));
                    instance.Label.text = args.GetValueOrDefault("remaining_text", "");

                    if (instance.Message.Contains("signcolor=rgb"))
                    {
                        material.color = rgb_cycle(material.color);
                    }
                    if (instance.Message.Contains("textcolor=rgb"))
                    {
                        instance.Label.color = rgb_cycle(instance.Label.color);
                    }
                }
            }
        }

        public Color rgb_cycle(Color currentColor)
        {
            //adapted from Madgvox's post here: https://discussions.unity.com/t/color-cycle-script-need-help/832829/4
            float H, S, V;
            Color.RGBToHSV(currentColor, out H, out S, out V);
            S = 0.5f;
            V = 0.5f;
            H += 1f + Time.deltaTime;
            if (H > 1f)
            {
                H -= 1f;
            }
            return Color.HSVToRGB(H, S, V);
        }

        public Color StringToColor(string colorString)
        {
            if (string.IsNullOrWhiteSpace(colorString))
                throw new System.ArgumentException("Color string cannot be null or empty.");

            // Check for named colors
            if (ColorUtility.TryParseHtmlString(colorString, out Color namedColor))
            {
                return namedColor;
            }

            // Check for hexadecimal color codes
            if (colorString.StartsWith("#") || colorString.Length == 6 || colorString.Length == 8)
            {
                if (ColorUtility.TryParseHtmlString(colorString.StartsWith("#") ? colorString : $"#{colorString}", out Color hexColor))
                {
                    return hexColor;
                }
            }

            // Check for RGBA values
            string[] rgba = colorString.Split(',');
            if (rgba.Length == 4)
            {
                Melon<Core>.Logger.Msg($"RGBA: {rgba[0]}, {rgba[1]}, {rgba[2]}, {rgba[3]}");
                float r = float.Parse(rgba[0]) / 255f;
                float g = float.Parse(rgba[1]) / 255f;
                float b = float.Parse(rgba[2]) / 255f;
                float a = float.Parse(rgba[3]);
                return new Color(r, g, b, a);
            }

            throw new System.FormatException($"Invalid color string format: {colorString}");
        }
    }
}