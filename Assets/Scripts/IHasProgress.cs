using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProgress
{
    public event EventHandler<OnProgreessChangedEventArgs> OnProgressChanged;
    public class OnProgreessChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }
}
