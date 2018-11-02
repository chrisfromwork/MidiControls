using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MidiJack;

namespace Controls.Midi
{
    public sealed class MidiToggle : Selectable, IPointerClickHandler, ICanvasElement
    {
        #region Editable properties
        [SerializeField] MidiChannel _midiChannel = MidiChannel.All;
        [Range(0, 127)]
        [SerializeField] int _noteNumber = 0;
        [SerializeField] bool _isOn;

        [SerializeField] bool _useControl = false;
        [SerializeField] int _controlNumber = 0;
        [SerializeField] float _controlThreshold = 0.5f;

        public bool isOn
        {
            get { return _isOn; }
            set { Set(value); }
        }

        [SerializeField] Graphic _graphic;

        public Graphic graphic
        {
            get { return _graphic; }
            set { _graphic = value; UpdateVisuals(); }
        }

        [System.Serializable] public class ToggleEvent : UnityEvent<bool> { }

        [SerializeField] ToggleEvent _onValueChanged = new ToggleEvent();

        public ToggleEvent onValueChanged
        {
            get { return _onValueChanged; }
            set { _onValueChanged = value; }
        }

        #endregion

        #region Private methods

        void Set(bool value, bool sendCallback = true)
        {
            if (_isOn == value) return;

            _isOn = value;
            UpdateVisuals();

            if (sendCallback) _onValueChanged.Invoke(_isOn);
        }

        void InternalToggle()
        {
            if (!IsActive() || !IsInteractable()) return;
            isOn = !_isOn;
        }

        void UpdateVisuals()
        {
            if (_graphic == null) return;

            _graphic.canvasRenderer.SetAlpha(_isOn ? 1 : 0);
        }

        #endregion

        #region Selectable functions

        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateVisuals();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (IsActive())
            {
                UpdateVisuals();
                UpdateLabel();
            }

            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
            if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }
#endif

        #endregion

        #region IPointerClickHandler implementation

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            InternalToggle();
        }

        #endregion

        #region ICanvasElement implementation

        public void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(_isOn);
#endif
        }

        public void LayoutComplete() { }
        public void GraphicUpdateComplete() { }

        #endregion

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
                if ((_midiChannel == MidiChannel.All || _midiChannel == channel) &&
                    (_noteNumber == note))
                {
                    isOn = !isOn;
                    DoStateTransition(SelectionState.Pressed, true);
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

        private void Start()
        {
            UpdateLabel();
        }

        float _prevVal = 0;
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
                        isOn = true;
                    }
                    else
                    {
                        DoStateTransition(SelectionState.Normal, true);
                        isOn = false;
                    }

                    _prevVal = tempVal;
                }
            }
        }
    }
}
