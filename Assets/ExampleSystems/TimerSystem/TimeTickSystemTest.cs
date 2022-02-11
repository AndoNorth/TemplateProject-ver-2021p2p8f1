using System;
using UnityEngine;

namespace TemplateProject {
    public class TimeTickSystemTest : MonoBehaviour
    {
        [SerializeField]
        int ticks = 50;
        [SerializeField]
        Vector2 barSize = new Vector2(10f, 1f);
        void Start()
        {
            TimeTickSystem.OnTick += delegate (object sender, TimeTickSystem.OnTickEventArgs e)
            {
                // Debug.Log("Tick");
            };
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 position = GeneralUtility.GetMouseWorldPosition();
                new TimerBar(position, ticks, "Test", new Vector3(barSize.x, barSize.y));
            }
        }

        private class TimerBar
        {
            private int _currentTick;
            private int _maxTick;
            private bool _isTicking;
            GameObject _gameObject;
            private GeneralUtility.World_Bar _timerBar;

            public TimerBar(Vector3 position, int ticksToFinish, string timerBarName, Vector3 barSize)
            {
                _currentTick = 0;
                _maxTick = ticksToFinish;
                _isTicking = true;

                _gameObject = new GameObject("Timer Bar " + timerBarName);
                _gameObject.transform.position = position;

                _timerBar = new GeneralUtility.World_Bar(_gameObject.transform, new Vector3(0, 0), barSize, Color.white, Color.red, 1f, -10, new GeneralUtility.World_Bar.Outline());
                

                TimeTickSystem.OnTick += TimeTicketSystem_OnTick;
            }

            private void TimeTicketSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
            {
                if (_isTicking)
                {
                    _currentTick += 1;
                    Debug.Log("Tick");
                    if (_currentTick >= _maxTick)
                    {
                        _isTicking = false;
                        UnityEngine.Object.Destroy(_gameObject);
                    }
                    else
                    {
                        _timerBar.SetSize(_currentTick * 1f / _maxTick);
                    }
                }
            }
        }
    }
}
