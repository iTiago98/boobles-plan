using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.Flags
{
    [System.Serializable]
    public class Flag
    {
        public enum State { Undefined = -1, True = 1, False = 0 }
        
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
            ArcadioWaiting,
            AllCoins,
            RecreativaIntro,
            QuecaIntro,
            CafeIntro,
            SofaIntro,
            MachineFixed,
            ZumoIntro,
            GranFinalObtenida
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
