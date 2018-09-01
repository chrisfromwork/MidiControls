using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MidiJack;

namespace Controls.Midi
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class MidiButton : Selectable, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] MidiChannel _midiChannel = MidiChannel.All;
        [Range(0,127)]
        [SerializeField] int _noteNumber = 0;
        [System.Serializable] public class ButtonEvent : UnityEvent { }
        [SerializeField] ButtonEvent _onButtonDown = new ButtonEvent();
        [SerializeField] ButtonEvent _onButtonUp = new ButtonEvent();

        public ButtonEvent onButtonDown
        {
            get { return _onButtonDown; }
            set { _onButtonDown = value; }
        }

        public ButtonEvent onButtonUp
        {
            get { return _onButtonUp; }
            set { _onButtonUp = value; }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable()) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            base.OnPointerDown(eventData);
            _onButtonDown.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable()) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            base.OnPointerUp(eventData);
            _onButtonUp.Invoke();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (IsActive())
            {
                UpdateLabel();
            }
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            MidiMaster.noteOnEvent += onNoteOn;
            MidiMaster.noteOffEvent += onNoteOff;
        }

        private void onNoteOn(MidiChannel channel, int note, float velocity)
        {
            if ((_midiChannel == MidiJack.MidiChannel.All || _midiChannel == channel) &&
                (_noteNumber == note))
            {
                DoStateTransition(SelectionState.Pressed, true);
                _onButtonDown.Invoke();
            }
        }

        private void onNoteOff(MidiChannel channel, int note)
        {
            if ((_midiChannel == MidiChannel.All || _midiChannel == channel) &&
                (_noteNumber == note))
            {
                DoStateTransition(SelectionState.Normal, true);
                _onButtonUp.Invoke();
            }
        }

        private void UpdateLabel()
        {
            var label = gameObject.transform.Find("Label");
            if (label)
            {
                var text = label.GetComponent<Text>();
                text.text = _midiChannel.ToString().ToUpper() + " N" + _noteNumber;
            }
        }
    }
}
