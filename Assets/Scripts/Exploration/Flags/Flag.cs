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
            GranFinalObtenida,
            PistaHortensia,
            PistaEufrasio,
            PistaJugosol,
            PistaModificacion,
            PistaAdquisicion,
            GinsunnyDelight,
            Whiskolacao,
            PoleoAbsenta,
            Day1,
            EufrasioHablado,
            BajarBoobledona,
            EscorbutoObtenida,
            ExprimirObtenida,
            MaquinaZumosObtenida,
            HipervitaminadoObtenida,
            Car0,
            Car1,
            DennisMencionaExplosion,
            AnaMencionaIngredientes,
            PajitaObtenida,
            GomaObtenida,
            PolvoraObtenida,
            IngredientesObtenidos,
            MontarBombaPedido,
            AlbumFotosVisto,
            PelotaMencionada,
            PelotaObtenida,
            BombaObtenida,
            VictoriaPorDesgasteObtenida,
            DennisMencionaFamilias,
            ParedObtenida,
            LaPalaDeNocobichObtenida,
            GomuGomuNoObtenida,
            PelotaBombaObtenida,
            LlaveMencionada,
            FerranGritaPolvora,
            TarjetaObtenida,
            LlaveObtenida,
            GuanteMencionado,
            GuanteObtenido,
            Car2,
            Car3
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
