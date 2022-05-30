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
        }
    }
}
