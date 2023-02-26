// TimeRaceGuideAnimHelper.cs
// 创建者： 张毅文
// 创建时间：2022/06/15
// 概要：

using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameUtils
{
    public class TimeRaceGuideAnimHelper : MonoBehaviour
    {
        public Text m_txtTime;

        private UnityBasedTimer m_timer;

        private int m_counter;
        
        public void StartTimer()
        {
            m_timer = new UnityBasedTimer();
        }

        public void StopTimer()
        {
            m_timer.StopTimer();
        }

        private void Update()
        {
            if (m_timer == null)
            {
                return;
            }
            UpdateTimeDisplay(m_timer.CurrentPassedTime);
            
        }

        public void UpdateTimeDisplay(float timeInSeconds)
        {
            m_txtTime.text = timeInSeconds.ToString("00.00");
        }
    }
}