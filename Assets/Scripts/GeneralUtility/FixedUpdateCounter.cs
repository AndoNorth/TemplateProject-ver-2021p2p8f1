using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedUpdateCounter
{
    int _count = 0;
    int _fixedUpdatesPerSec = 0;
    float _elapsedTime = 0f;
    public int GetCount()
    {
        return _fixedUpdatesPerSec;
    }
    public void OnFixedUpdate()
    {
        _count++;
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= 1.0f)
        {
            _fixedUpdatesPerSec = _count;
            _count = 0;
            _elapsedTime = 0f;
        }
    }
}
