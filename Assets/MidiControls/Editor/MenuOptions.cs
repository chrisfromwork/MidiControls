// VJUI - Custom UI controls for VJing
// https://github.com/keijiro/VJUI

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System;

namespace Controls.Midi
{
    public static class MenuOptions
    {
        static T LoadResource<T>(string filename) where T : UnityEngine.Object
        {
            var path = System.IO.Path.Combine("Assets/MidiControls/Resources", filename);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        static void PlaceUIElementRoot(GameObject go, MenuCommand menuCommand)
        {
            // Retrieve an internal method "MenuOptions.PlaceUIElementRoot".
            var type = Type.GetType("UnityEditor.UI.MenuOptions,UnityEditor.UI");
            var flags = BindingFlags.NonPublic | BindingFlags.Static;
            var method = type.GetMethod("PlaceUIElementRoot", flags);

            // PlaceUIElementRoot(go, menuCommand)
            method.Invoke(null, new System.Object[]{ go, menuCommand });
        }

        [MenuItem("GameObject/UI/Midi Knob", false, 10)]
        static void AddMidiKnob(MenuCommand menuCommand)
        {
            var go = DefaultControls.CreateMidiKnob(
                LoadResource<Material>("Knob.mat"),
                LoadResource<Sprite>("Knob.png"),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf")
            );
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/Midi Button", false, 11)]
        static void AddMidiButton(MenuCommand menuCommand)
        {
            var go = DefaultControls.CreateMidiButton(
                LoadResource<Sprite>("Button.png"),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf")
            );
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/Midi Toggle", false, 12)]
        static void AddMidiToggle(MenuCommand menuCommand)
        {
            var go = DefaultControls.CreateMidiToggle(
                LoadResource<Sprite>("Toggle.png"),
                LoadResource<Sprite>("Toggle Fill.png"),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf")
            );
            PlaceUIElementRoot(go, menuCommand);
        }
    }
}
