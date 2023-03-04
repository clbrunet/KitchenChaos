using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProgression
{
    public event EventHandler<OnProgressionEventArgs> OnProgression;
    public class OnProgressionEventArgs : EventArgs
    {
        public float progression;

        public OnProgressionEventArgs(float progress)
        {
            this.progression = progress;
        }
    }
}
