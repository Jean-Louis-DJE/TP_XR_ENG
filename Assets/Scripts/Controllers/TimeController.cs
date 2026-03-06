using UnityEngine;
using System;

public class TimeController : MonoBehaviour
{
    public float secondsPerDay = 100f;

    TimeModel model;
    DateTime current;

    public void Init(TimeModel m)
    {
        model = m;
        current = DateTime.Now;
        model.SetTime(current);
    }

    void Update()
    {
        if (model == null) return;
        if (!model.IsPlaying) return;

        current = current.AddDays(Time.deltaTime * secondsPerDay);
        model.SetTime(current);
    }
}
