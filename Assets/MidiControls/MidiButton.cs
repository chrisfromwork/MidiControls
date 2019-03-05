using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MidiJack;
using System.Collections.Generic;

namespace Controls.Midi
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class MidiButton : Selectable, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] public MidiChannel _midiChannel = MidiChannel.All;
        [Range(0,127)]
        [SerializeField] public int _noteNumber = 0;
        [System.Serializable] public class ButtonEvent : UnityEvent { }
        [SerializeField] ButtonEvent _onButtonDown = new ButtonEvent();
        [SerializeField] ButtonEvent _onButtonUp = new ButtonEvent();
        [SerializeField] public bool _useControl = false;
        [SerializeField] public int _controlNumber = 0;
        [SerializeField] float _controlThreshold = 0.5f;

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

        [System.Serializable]
        public class StabEvent : UnityEvent<float> { }
        [SerializeField] StabEvent _stabEvent = new StabEvent();
        [SerializeField] float _stabLength = 0;
        [SerializeField] float _stabStartingValue = 1.0f;
        [SerializeField] float _stabEndingValue = 0.0f;
        float _stabStartTime = 0;
        [SerializeField] bool _midiSyncStab = false;
        [SerializeField] float _numBeatsStab = 1;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable()) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            base.OnPointerDown(eventData);
            HandleButtonDown();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable()) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            base.OnPointerUp(eventData);
            HandleButtonUp();
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
            if (!_useControl)
            {
                if ((_midiChannel == MidiJack.MidiChannel.All || _midiChannel == channel) &&
                    (_noteNumber == note))
                {
                    DoStateTransition(SelectionState.Pressed, true);
                    HandleButtonDown();
                }
            }
        }

        private void onNoteOff(MidiChannel channel, int note)
        {
            if (!_useControl)
            {
                if ((_midiChannel == MidiChannel.All || _midiChannel == channel) &&
                    (_noteNumber == note))
                {
                    DoStateTransition(SelectionState.Normal, true);
                    HandleButtonUp();
                }
            }
        }

        private void UpdateLabel()
        {
            var label = gameObject.transform.Find("Label");
            if (label)
            {
                var text = label.GetComponent<Text>();
                if (!_useControl)
                {
                    text.text = _midiChannel.ToString().ToUpper() + " N" + _noteNumber;
                }
                else
                {
                    text.text = _midiChannel.ToString().ToUpper() + " C" + _controlNumber;
                }
            }
        }

        private void HandleButtonDown()
        {
            _onButtonDown.Invoke();
            _stabStartTime = Time.time;

            if (_midiSyncStab)
            {
                _stabLength = MidiDriver.Instance.lastbeatLength * _numBeatsStab;
            }
        }

        private void HandleButtonUp()
        {
            _onButtonUp.Invoke();
        }

        private void Start()
        {
            UpdateLabel();
        }

        private float _prevVal = 0;
        private void Update()
        {
            if (_useControl)
            {
                var tempVal = MidiMaster.GetKnob(_midiChannel, _controlNumber, _prevVal);
                if (tempVal != _prevVal)
                {
                    if (tempVal > _controlThreshold)
                    {
                        DoStateTransition(SelectionState.Pressed, true);
                        HandleButtonDown();
                    }
                    else
                    {
                        DoStateTransition(SelectionState.Normal, true);
                        HandleButtonUp();
                    }

                    _prevVal = tempVal;
                }
            }

            float dt = Time.time - _stabStartTime;
            if (dt < _stabLength)
            {
                var value = Mathf.Lerp(_stabStartingValue, _stabEndingValue, dt / _stabLength);
                _stabEvent.Invoke(value);
            }
        }
    }
}
