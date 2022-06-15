using System.Collections.Generic;

namespace Booble.Flags
{
    [System.Serializable]
    public class FlagList
    {
        private Flag _anaIntro;
        private Flag _anaPareados;
        private Flag _arcadioIntro;
        private Flag _citrianoIntro;
        private Flag _citrianoEsperando;
        private Flag _dennisIntro;
        private Flag _dennisEsperando;
        private Flag _pinPonBrosMoneda;
        private Flag _pinPonBrosIntro;
        private Flag _monedaPinPonBrosObtenida;
        private Flag _anaZumos;
        private Flag _monedaSofasObtenida;
        private Flag _monedaMaquinaCafesObtenida;
        private Flag _monedaMaquinaZumosObtenida;
        private Flag _monedaPinPonBrosEntregada;
        private Flag _monedaSofasEntregada;
        private Flag _monedaMaquinaCafesEntregada;
        private Flag _monedaMaquinaZumoGastada;
        private Flag _arcadioPartida;

        public List<Flag> Flags
        {
            get
            {
                List<Flag> list = new List<Flag>();

                list.Add(_anaIntro);
                list.Add(_anaPareados);
                list.Add(_arcadioIntro);
                list.Add(_citrianoIntro);
                list.Add(_citrianoEsperando);
                list.Add(_dennisIntro);
                list.Add(_dennisEsperando);
                list.Add(_pinPonBrosMoneda);
                list.Add(_pinPonBrosIntro);
                list.Add(_monedaPinPonBrosObtenida);
                list.Add(_anaZumos);
                list.Add(_monedaSofasObtenida);
                list.Add(_monedaMaquinaCafesObtenida);
                list.Add(_monedaMaquinaZumosObtenida);
                list.Add(_monedaPinPonBrosEntregada);
                list.Add(_monedaSofasEntregada);
                list.Add(_monedaMaquinaCafesEntregada);
                list.Add(_monedaMaquinaZumoGastada);
                list.Add(_arcadioPartida);

                return list;
            }
        }
        
        public FlagList()
        {
            _anaIntro = new Flag(Flag.Reference.AnaIntro);
            _anaPareados = new Flag(Flag.Reference.AnaPareados);
            _arcadioIntro = new Flag(Flag.Reference.ArcadioIntro);
            _citrianoIntro = new Flag(Flag.Reference.CitrianoIntro);
            _citrianoEsperando = new Flag(Flag.Reference.CitrianoEsperando);
            _dennisIntro = new Flag(Flag.Reference.DennisIntro);
            _dennisEsperando = new Flag(Flag.Reference.DennisEsperando);
            _pinPonBrosMoneda = new Flag(Flag.Reference.PinPonBrosMoneda);
            _pinPonBrosIntro = new Flag(Flag.Reference.PinPonBrosIntro);
            _monedaPinPonBrosObtenida = new Flag(Flag.Reference.MonedaPinPonBrosObtenida);
            _anaZumos = new Flag(Flag.Reference.AnaZumos);
            _monedaSofasObtenida = new Flag(Flag.Reference.MonedaSofasObtenida);
            _monedaMaquinaCafesObtenida = new Flag(Flag.Reference.MonedaMaquinaCafesObtenida);
            _monedaMaquinaZumosObtenida = new Flag(Flag.Reference.MonedaMaquinaZumosObtenida);
            _monedaPinPonBrosEntregada = new Flag(Flag.Reference.MonedaPinPonBrosEntregada);
            _monedaSofasEntregada = new Flag(Flag.Reference.MonedaSofasEntregada);
            _monedaMaquinaCafesEntregada = new Flag(Flag.Reference.MonedaMaquinaCafesEntregada);
            _monedaMaquinaZumoGastada = new Flag(Flag.Reference.MonedaMaquinaZumoGastada);
            _arcadioPartida = new Flag(Flag.Reference.ArcadioPartida);
        }
    }
}
