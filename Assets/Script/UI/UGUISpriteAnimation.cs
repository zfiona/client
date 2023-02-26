using UnityEngine;

using System.Collections;

using System.Collections.Generic;

using UnityEngine.UI;

using System;




namespace Assets.Scripts.Module.Tool.iOSTool
{
    [RequireComponent(typeof(Image))]

    public class UGUISpriteAnimation : MonoBehaviour
    {

        private Image ImageSource;

        private int mCurFrame = 0;

        private float mDelta = 0;

        public float FPS = 5;

        public List<Sprite> SpriteFrames;

        public bool IsPlaying = false;

        public bool Foward = true;

        public bool AutoPlay = false;

        public bool Loop = false;

        public bool isNotSetNativeSize = false;

        public Action PlayCompleteAction;

        public int FrameCount
        {

            get
            {

                return SpriteFrames.Count;

            }

        }

        void Awake()
        {
            ImageSource = GetComponent<Image>();
        }

        void Start()
        {

            if (AutoPlay)
            {

                Play();

            }

        }

        private void SetSprite(int idx)
        {

            ImageSource.sprite = SpriteFrames[idx];

            //是否设置为原图
            if (!isNotSetNativeSize)
                ImageSource.SetNativeSize();

        }

        public void Play()
        {

            IsPlaying = true;

            Foward = true;

        }

        public void PlayReverse()
        {

            IsPlaying = true;

            Foward = false;

        }

        void Update()
        {

            if (!IsPlaying || 0 == FrameCount)
            {

                return;

            }

            mDelta += Time.deltaTime;

            if (mDelta > 1 / FPS)
            {

                mDelta = 0;

                if (Foward)
                {

                    mCurFrame++;

                }

                else
                {

                    mCurFrame--;

                }

                if (mCurFrame >= FrameCount)
                {

                    if (Loop)
                    {

                        mCurFrame = 0;

                    }

                    else
                    {

                        IsPlaying = false;
                        if (PlayCompleteAction != null)
                        {
                            PlayCompleteAction.Invoke();
                        }
                        return;

                    }

                }

                else if (mCurFrame < 0)
                {

                    if (Loop)
                    {

                        mCurFrame = FrameCount - 1;

                    }

                    else
                    {

                        IsPlaying = false;

                        return;

                    }

                }

                SetSprite(mCurFrame);

            }

        }

        public void Pause()
        {

            IsPlaying = false;

        }

        public void Resume()
        {

            if (!IsPlaying)
            {

                IsPlaying = true;

            }

        }

        public void Stop()
        {

            mCurFrame = 0;

            SetSprite(mCurFrame);

            IsPlaying = false;

        }

        public void Rewind()
        {
            mCurFrame = 0;

            SetSprite(mCurFrame);

            Play();

        }

    }
}