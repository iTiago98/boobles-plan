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
        }
    }
}
