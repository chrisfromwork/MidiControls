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

        [MenuItem("GameObject/UI/Sine Wave", false, 13)]
        static void AddSineWave(MenuCommand menuCommand)
        {
            var waveType = WaveType.Sine;
            var go = DefaultControls.CreateWave(
                waveType,
                GetSprite(waveType),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf")
            );
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/Saw Wave", false, 14)]
        static void AddSawWave(MenuCommand menuCommand)
        {
            var waveType = WaveType.Saw;
            var go = DefaultControls.CreateWave(
                waveType,
                GetSprite(waveType),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf")
            );
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/Inverse Saw Wave", false, 15)]
        static void AddInverseSawWave(MenuCommand menuCommand)
        {
            var waveType = WaveType.InverseSaw;
            var go = DefaultControls.CreateWave(
                waveType,
                GetSprite(waveType),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf")
            );
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/Square Wave", false, 16)]
        static void AddSquareWave(MenuCommand menuCommand)
        {
            var waveType = WaveType.Square;
            var go = DefaultControls.CreateWave(
                waveType,
                GetSprite(waveType),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf")
            );
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/Triangle Wave", false, 17)]
        static void AddTriangleWave(MenuCommand menuCommand)
        {
            var waveType = WaveType.Triangle;
            var go = DefaultControls.CreateWave(
                waveType,
                GetSprite(waveType),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf")
            );
            PlaceUIElementRoot(go, menuCommand);
        }

        static Sprite GetSprite(WaveType wave)
        {
            switch (wave)
            {
                case WaveType.InverseSaw:
                    return LoadResource<Sprite>("inversesaw.png");
                case WaveType.Saw:
                    return LoadResource<Sprite>("saw.png");
                case WaveType.Sine:
                    return LoadResource<Sprite>("sine.png");
                case WaveType.Square:
                    return LoadResource<Sprite>("square.png");
                case WaveType.Triangle:
                    return LoadResource<Sprite>("triangle.png");
                default:
                    throw new Exception("Sprite doesn't exist for wave type.");
            }
        }

        [MenuItem("GameObject/UI/Akai APC40", false, 18)]
        static void AddAPC40(MenuCommand menuCommand)
        {
            var gameObject = new GameObject();
            gameObject.name = "Akai APC40";
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasGameObject = new GameObject();
            canvasGameObject.name = "Akai APC40 Canvas";
            canvasGameObject.transform.parent = gameObject.transform;
            canvas = canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            RectTransform rect;
            MidiButton button;
            GameObject go;

            // top buttons
            // channels 0 to 7
            // notes 53 to 57
            float xSize = 80;
            float ySize = -92;
            for (int m = 0; m < 5; m++)
            {
                for (int n = 0; n < 8; n++)
                {
                    go = DefaultControls.CreateMidiButton(
                        LoadResource<Sprite>("Button.png"),
                        LoadResource<Font>("DejaVuSans-ExtraLight.ttf"));
                    go.name = "MIDI Button " + n + "x" + m;
                    button = go.GetComponent<MidiButton>();
                    button._midiChannel = (MidiJack.MidiChannel)n;
                    button._noteNumber = 53 + m;
                    button.transform.parent = canvas.transform;
                    rect = button.gameObject.GetComponent<RectTransform>();
                    rect.localPosition = new Vector3(n * xSize, m * ySize, 0);
                }
            }

            // clip stop
            // channel 0
            // note 52
            float xOffset = 0;
            float yOffset = (5 * ySize) - 35;
            for (int n = 0; n < 8; n++)
            {
                go = DefaultControls.CreateMidiButton(
                    LoadResource<Sprite>("Button.png"),
                    LoadResource<Font>("DejaVuSans-ExtraLight.ttf"));
                go.name = "MIDI Clip Stop " + n;
                button = go.GetComponent<MidiButton>();
                button._midiChannel = (MidiJack.MidiChannel)n;
                button._noteNumber = 52;
                button.transform.parent = canvas.transform;
                rect = button.gameObject.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(xOffset + n * xSize, yOffset, 0);
            }

            // channels 0
            // note 82 to 86
            xOffset = (8 * xSize) + 35;
            yOffset = 0;
            for (int m = 0; m < 5; m++)
            {
                go = DefaultControls.CreateMidiButton(
                    LoadResource<Sprite>("Button.png"),
                    LoadResource<Font>("DejaVuSans-ExtraLight.ttf"));
                go.name = "MIDI Scene Launch " + m;
                button = go.GetComponent<MidiButton>();
                button._midiChannel = MidiJack.MidiChannel.Ch1;
                button._noteNumber = 82 + m;
                button.transform.parent = canvas.transform;
                rect = button.gameObject.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(xOffset, yOffset + ySize * m, 0);
            }

            xOffset = (8 * xSize) + 35;
            yOffset = (5 * ySize) - 35;
            go = DefaultControls.CreateMidiButton(
                LoadResource<Sprite>("Button.png"),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf"));
            go.name = "MIDI Stop All Clips";
            button = go.GetComponent<MidiButton>();
            button._midiChannel = MidiJack.MidiChannel.Ch1;
            button._noteNumber = 81;
            button.transform.parent = canvas.transform;
            rect = button.gameObject.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(xOffset, yOffset, 0);

            MidiKnob knob;

            // top knobs
            // channel 0
            // control 48 to 55
            xOffset = (9 * xSize) + 35 + 35;
            yOffset = 0;
            for (int m = 0; m < 2; m++)
            {
                for (int n = 0; n < 4; n++)
                {
                    go = DefaultControls.CreateMidiKnob(
                        LoadResource<Material>("Knob.mat"),
                        LoadResource<Sprite>("Knob.png"),
                        LoadResource<Font>("DejaVuSans-ExtraLight.ttf"));
                    go.name = "MIDI Knob Group 1 " + n + "x" + m;
                    knob = go.GetComponent<MidiKnob>();
                    knob.transform.parent = canvas.transform;
                    knob._midiChannel = MidiJack.MidiChannel.Ch1;
                    knob._controlNumber = m * 4 + n + 48;
                    rect = knob.gameObject.GetComponent<RectTransform>();
                    rect.localPosition = new Vector3(xOffset + n * xSize, yOffset + m * ySize, 0);
                }
            }

            // bottom knobs
            // channel 0
            // control 16 to 23
            xOffset = (9 * xSize) + 35 + 35;
            yOffset = (3 * ySize);
            for (int m = 0; m < 2; m++)
            {
                for (int n = 0; n < 4; n++)
                {
                    go = DefaultControls.CreateMidiKnob(
                        LoadResource<Material>("Knob.mat"),
                        LoadResource<Sprite>("Knob.png"),
                        LoadResource<Font>("DejaVuSans-ExtraLight.ttf"));
                    go.name = "MIDI Knob Group 2 " + n + "x" + m;
                    knob = go.GetComponent<MidiKnob>();
                    knob.transform.parent = canvas.transform;
                    knob._midiChannel = MidiJack.MidiChannel.All;
                    knob._controlNumber = m * 4 + n + 16;
                    rect = knob.gameObject.GetComponent<RectTransform>();
                    rect.localPosition = new Vector3(xOffset + n * xSize, yOffset + m * ySize, 0);
                }
            }

            // bottom slider
            // channel 0
            // control 15
            xOffset = (9 * xSize) + 35 + 35 + (1.5f * xSize);
            yOffset = (5 * ySize) - 35;
            go = DefaultControls.CreateMidiKnob(
                LoadResource<Material>("Knob.mat"),
                LoadResource<Sprite>("Knob.png"),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf"));
            go.name = "MIDI Main Slider";
            knob = go.GetComponent<MidiKnob>();
            knob.transform.parent = canvas.transform;
            knob._midiChannel = MidiJack.MidiChannel.Ch1;
            knob._controlNumber = 15;
            rect = knob.gameObject.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(xOffset, yOffset, 0);

            MidiToggle toggle;

            // channels 0 to 7
            // notes 50 to 48
            xOffset = 0;
            yOffset = (6 * ySize) - 35 - 35;
            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 8; n++)
                {
                    go = DefaultControls.CreateMidiToggle(
                        LoadResource<Sprite>("Toggle.png"),
                        LoadResource<Sprite>("Toggle Fill.png"),
                        LoadResource<Font>("DejaVuSans-ExtraLight.ttf") );
                    go.name = "MIDI Toggle " + n + "x" + m;
                    toggle = go.GetComponent<MidiToggle>();
                    toggle.transform.parent = canvas.transform;
                    toggle._midiChannel = (MidiJack.MidiChannel)n;
                    toggle._noteNumber = 50 - m;
                    toggle._trueOnOff = true;
                    rect = toggle.gameObject.GetComponent<RectTransform>();
                    rect.localPosition = new Vector3(xOffset + n * xSize, yOffset + m * ySize, 0);
                }
            }

            // bottom sliders
            // channels 0 to 7
            // control 7
            xOffset = 0;
            yOffset = (9 * ySize) - 35 - 35 - 35;
            for (int n = 0; n < 8; n++)
            {
                go = DefaultControls.CreateMidiKnob(
                    LoadResource<Material>("Knob.mat"),
                    LoadResource<Sprite>("Knob.png"),
                    LoadResource<Font>("DejaVuSans-ExtraLight.ttf"));
                go.name = "MIDI Bottom Slider " + n;
                knob = go.GetComponent<MidiKnob>();
                knob.transform.parent = canvas.transform;
                knob._midiChannel = (MidiJack.MidiChannel)n;
                knob._controlNumber = 7;
                rect = knob.gameObject.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(xOffset + n * xSize, yOffset, 0);
            }

            // final slider
            // channel 0
            // control 14
            xOffset = 8 * xSize + 35;
            yOffset = (9 * ySize) - 35 - 35 - 35;
            go = DefaultControls.CreateMidiKnob(
                LoadResource<Material>("Knob.mat"),
                LoadResource<Sprite>("Knob.png"),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf"));
            go.name = "MIDI Master Slider";
            knob = go.GetComponent<MidiKnob>();
            knob.transform.parent = canvas.transform;
            knob._midiChannel = MidiJack.MidiChannel.Ch1;
            knob._controlNumber = 14;
            rect = knob.gameObject.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(xOffset, yOffset, 0);

            // cue level
            // channel 0
            // control 47
            xOffset = 8 * xSize + 35;
            yOffset = (7 * ySize) - 35 - 35;
            go = DefaultControls.CreateMidiKnob(
                LoadResource<Material>("Knob.mat"),
                LoadResource<Sprite>("Knob.png"),
                LoadResource<Font>("DejaVuSans-ExtraLight.ttf"));
            go.name = "MIDI Knob Cue Level";
            knob = go.GetComponent<MidiKnob>();
            knob.transform.parent = canvas.transform;
            knob._midiChannel = MidiJack.MidiChannel.Ch1;
            knob._controlNumber = 47;
            rect = knob.gameObject.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(xOffset, yOffset, 0);

            rect = canvas.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(40, -40, 0);
            rect.localScale = Vector3.one * 0.4f;
        }
    }
}
