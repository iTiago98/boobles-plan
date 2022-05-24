using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.Flags
{
    [System.Serializable]
    public class Flag
    {
        public enum State { Undefined, True, False }
        public enum Reference
        {
            None,
            AnaIntro,
            AnaPareados
        }

        [SerializeField] private string _flagName;
        [SerializeField] private Reference _flagReference;
        [SerializeField] private State _flagState;
        
        public string FlagName
        {
            get { return _flagName; }
            set { _flagName = value; }
        }

        public State FlagState
        {
            get { return _flagState; }
            set { _flagState = value; }
        }

        public Reference FlagReference
        {
            get { return _flagReference; }
            set { _flagReference = value; }
        }
    }
}
