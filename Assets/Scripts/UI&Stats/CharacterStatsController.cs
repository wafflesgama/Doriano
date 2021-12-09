using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StatChanged(float previousValue, float newValue);
public class Stat
{
    public event StatChanged OnStatChanged;
    public event StatChanged OnMaxStatChanged;

    public float MinStatValue { get; set; }

    public float MaxStatValue
    {
        get { return MaxStatValue; }
        set { OnMaxStatChanged.Invoke(MaxStatValue, value); MaxStatValue = value; }
    }

    public float StatValue
    {
        get { return StatValue; }
        set { OnStatChanged.Invoke(StatValue, value); StatValue = value; }
    }

    public void AddToValue(float valueToAdd)
    {
        var newValue = StatValue + valueToAdd;
        if (newValue < MinStatValue)
            newValue = MinStatValue;
        else if (newValue > MaxStatValue)
            newValue = MaxStatValue;

        StatValue = newValue;
    }

}
public class CharacterStatsController : MonoBehaviour
{
    public Stat health;
    public Stat stamina;

    private
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
