using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumbersRenderer : MonoBehaviour
{
    [Range(0, 999)]
    private int _value = 0;

    [SerializeField]
    private NumberRenderer[] _numberRenderes = new NumberRenderer[3];

    public int Value
    {
        get { return _value; }

        set
        {
            _value = value;
            Render();
        }
    }

    private void Render()
    {
        _numberRenderes[0].Value = (int)Mathf.Floor(_value / 100.0f)%10;
        _numberRenderes[1].Value = (int)Mathf.Floor(_value / 10.0f)%10;
        _numberRenderes[2].Value = (int)Mathf.Floor(_value / 1.0f)%10;
    }
}
