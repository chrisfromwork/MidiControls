using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum WaveType
{
    Sine,
    Triangle,
    Saw,
    InverseSaw,
    Square
}


public class MidiWave : MonoBehaviour, ICanvasElement {

    #region Editable properties
    [SerializeField] WaveType _waveType;
    public WaveType waveType
    {
        get { return _waveType; }
        set { _waveType = value; }
    }

    [SerializeField] float _value;
    public float value
    {
        get { return _value; }
    }

    [SerializeField] float _position;
    public float position
    {
        get { return _position; }
    }

    [SerializeField]
    float _period = 2000;
    public float period
    {
        get { return _period; }
        set { _period = value; }
    }

    [SerializeField]
    float _offset = 0;
    public float offset
    {
        get { return _offset; }
        set { _offset = value; }
    }

    [SerializeField] Graphic _graphic;
    public Graphic graphic
    {
        get { return _graphic; }
        set { _graphic = value; }
    }

    [System.Serializable] public class WaveEvent : UnityEvent<float> { }

    [SerializeField] WaveEvent _onValueChanged = new WaveEvent();

    public WaveEvent onValueChanged
    {
        get { return _onValueChanged; }
        set { _onValueChanged = value; }
    }
    #endregion

    #region ICanvasElement implementation

    public void Rebuild(CanvasUpdate executing)
    {
#if UNITY_EDITOR
        if (executing == CanvasUpdate.Prelayout)
            onValueChanged.Invoke(value);
#endif
    }

    public bool IsDestroyed()
    {
        return false;
    }

    public void LayoutComplete() { }
    public void GraphicUpdateComplete() { }

    #endregion

    private float periodsInGraphic = 2.0f;

    void UpdatePosition()
    {
        float time = (Time.time * 1000) + _offset;
        _position = (time % (periodsInGraphic * _period)) / (periodsInGraphic * _period);
    }

    void UpdateVisual()
    {
        if (!_graphic)
            return;

        _graphic.material.SetFloat("_Position", _position);
    }

    void UpdateValue()
    {
        float position = 0;
        switch(waveType)
        {
            case WaveType.Sine:
                _value = Mathf.Sin(_position * 4 * Mathf.PI);
                break;
            case WaveType.Saw:
                position = periodsInGraphic * (_position % (1.0f / periodsInGraphic));
                _value = 1.0f - (2.0f * position);
                break;
            case WaveType.InverseSaw:
                position = periodsInGraphic * (_position % (1.0f / periodsInGraphic));
                _value = (2.0f * position) - 1.0f;
                break;
            case WaveType.Square:
                position = periodsInGraphic * (_position % (1.0f / periodsInGraphic));
                _value = (position < 0.5f) ? 1.0f : -1.0f;
                break;
            case WaveType.Triangle:
                position = periodsInGraphic * (_position % (1.0f / periodsInGraphic));
                if (position < 0.25f)
                {
                    // 0 to 1
                    _value = 4 * position;
                }
                else if (position < 0.5f)
                {
                    // 1 to 0
                    _value = 1 - (4 * (position - 0.25f));
                }
                else if (position < 0.75f)
                {
                    // 0 to -1
                    _value = -(4 * (position - 0.5f));
                }
                else
                {
                    // -1 to 0
                    _value = -1 + (4 * (position - 0.75f));
                }
                break;
        }

        _onValueChanged.Invoke(_value);
    }

    // Use this for initialization
    void Start () {
        UpdateVisual();

        _position = 0;
    }

    // Update is called once per frame
    void Update ()
    {
        UpdatePosition();
        UpdateVisual();
        UpdateValue();
    }
}
