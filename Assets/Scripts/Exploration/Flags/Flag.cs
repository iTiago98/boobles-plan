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
            AnaPareados,
            ArcadioIntro,
            CitrianoIntro,
            CitrianoEsperando,
            DennisIntro,
            DennisEsperando,
            PinPonBrosMoneda,
            PinPonBrosIntro,
            MonedaPinPonBrosObtenida,
            AnaZumos,
            MonedaSofasObtenida,
            MonedaMaquinaCafesObtenida,
            MonedaMaquinaZumosObtenida,
            MonedaPinPonBrosEntregada,
            MonedaSofasEntregada,
            MonedaMaquinaCafesEntregada,
            MonedaMaquinaZumoGastada,
            ArcadioPartida,
            ArcadioWaiting
        }

        [SerializeField] private Reference _flagReference;
        [SerializeField] private State _flagState;
        
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

        public Flag(Reference reference)
        {
            _flagReference = reference;
            _flagState = State.False;
        }
    }
}
