// UnityBasedTimer.cs
// 创建者： 张毅文
// 创建时间：2022/05/30
// 概要：

using UnityEngine;

namespace GameUtils
{
    /// <summary>
    /// 基于UnityEngine.Time的计时器
    /// </summary>
    public class UnityBasedTimer
    {
        public float m_fStartTime;
        public float m_fEndTime;

        private bool m_bIsEnd;

        public float CurrentPassedTime
        {
            get
            {
                if (m_bIsEnd)
                {
                    return m_fEndTime - m_fStartTime;
                }
                
                return Time.realtimeSinceStartup - m_fStartTime;
            }
        }

        public UnityBasedTimer()
        {
            m_fStartTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 结束计时
        /// </summary>
        /// <returns>从Timer创建到结束计时的时间</returns>
        public float StopTimer()
        {            
            m_bIsEnd = true;
            
            m_fEndTime = Time.realtimeSinceStartup;
            float formStartToEnd = m_fEndTime - m_fStartTime;

            return formStartToEnd;
        }
    }
}