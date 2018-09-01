using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System;

namespace Controls.Midi
{
    public static class DefaultControls
    {
        const float kWidth = 80;
        private static Vector3 labelPosition = new Vector3(0.0f, 5.0f, 0.0f);

        // Retrieve and invoke a private method "DefaultControls.CreateUIElementRoot".
        static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            var type = Type.GetType("UnityEngine.UI.DefaultControls,UnityEngine.UI");
            var flags = BindingFlags.NonPublic | BindingFlags.Static;
            var method = type.GetMethod("CreateUIElementRoot", flags);
            return (GameObject)method.Invoke(null, new System.Object[]{ name, size });
        }

        // Retrieve and invoke a private method "DefaultControls.CreateUIObject".
        static GameObject CreateUIObject(string name, GameObject parent)
        {
            var type = Type.GetType("UnityEngine.UI.DefaultControls,UnityEngine.UI");
            var flags = BindingFlags.NonPublic | BindingFlags.Static;
            var method = type.GetMethod("CreateUIObject", flags);
            return (GameObject)method.Invoke(null, new System.Object[]{ name, parent });
        }

        static void SetDefaultColorTransitionValues(Selectable selectable, bool whiteOnPress)
        {
            var colors = selectable.colors;
            colors.normalColor = new Color32(72, 72, 72, 255);
            colors.highlightedColor = new Color32(72, 72, 72, 255);
            if (whiteOnPress)
                colors.pressedColor = Color.white;
            else
                colors.pressedColor = new Color32(72, 72, 72, 255);
            colors.disabledColor = new Color32(20, 20, 20, 128);
            colors.fadeDuration = 0.03f;
            selectable.colors = colors;
        }

        static void FitToParent(GameObject go, Vector2 offset)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.offsetMin = new Vector2(offset.x, 0);
            rt.offsetMax = new Vector2(0, offset.y);
        }

        // Actual controls

        // Knob
        public static GameObject CreateMidiKnob(Material material, Sprite sprite, Font font)
        {
            // UI hierarchy
            var root = CreateUIElementRoot("Midi Knob", Vector2.one * kWidth);
            var graphic = CreateUIObject("Graphic", root);
            var label = CreateUIObject("Label", root);

            // Stretch settings
            FitToParent(graphic, Vector2.zero);

            // Graphic
            var image = graphic.AddComponent<Image>();
            image.material = material;
            image.sprite = sprite;
            image.color = Color.white;

            // Label
            var text = label.AddComponent<Text>();
            text.text = "ALL 0";
            text.alignment = TextAnchor.UpperCenter;
            text.font = font;
            label.transform.position = labelPosition;

            // Knob
            var knob = root.AddComponent<MidiKnob>();
            SetDefaultColorTransitionValues(knob, false);
            knob.targetGraphic = image;
            knob.graphic = image;

            return root;
        }

        // Button
        public static GameObject CreateMidiButton(Sprite sprite, Font font)
        {
            // UI hierarchy
            var root = CreateUIElementRoot("Midi Button", Vector2.one * kWidth);
            var label = CreateUIObject("Label", root);

            // Graphic
            var image = root.AddComponent<Image>();
            image.sprite = sprite;
            image.color = Color.white;

            // Label
            var text = label.AddComponent<Text>();
            text.text = "ALL 0";
            text.alignment = TextAnchor.UpperCenter;
            text.font = font;
            label.transform.position = labelPosition;

            // Button
            var button = root.AddComponent<MidiButton>();
            SetDefaultColorTransitionValues(button, true);

            return root;
        }

        // Toggle
        public static GameObject CreateMidiToggle(Sprite bgSprite, Sprite fillSprite, Font font)
        {
            // UI hierarchy
            var root = CreateUIElementRoot("Midi Toggle", Vector2.one * kWidth);
            var background = CreateUIObject("Background", root);
            var checkmark = CreateUIObject("Checkmark", background);
            var label = CreateUIObject("Label", root);

            // Stretch settings
            FitToParent(background, Vector2.zero);
            FitToParent(checkmark, Vector2.zero);

            // Background image
            var bgImage = background.AddComponent<Image>();
            bgImage.sprite = bgSprite;
            bgImage.color = Color.white;

            // Checkmark image
            var ckImage = checkmark.AddComponent<Image>();
            ckImage.sprite = fillSprite;
            ckImage.color = new Color32(240, 240, 240, 255);

            // Label
            var text = label.AddComponent<Text>();
            text.text = "ALL 0";
            text.alignment = TextAnchor.UpperCenter;
            text.font = font;
            label.transform.position = labelPosition;

            // Toggle
            var toggle = root.AddComponent<MidiToggle>();
            SetDefaultColorTransitionValues(toggle, true);
            toggle.targetGraphic = bgImage;
            toggle.graphic = ckImage;

            return root;
        }
    }
}
