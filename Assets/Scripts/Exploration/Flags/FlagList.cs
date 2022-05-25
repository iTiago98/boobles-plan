using System.Collections.Generic;

namespace Booble.Flags
{
    [System.Serializable]
    public class FlagList
    {
        private Flag _anaIntro;
        private Flag _anaPareados;
        private Flag _arcadioIntro;

        public List<Flag> List
        {
            get
            { 
                List<Flag> list = new List<Flag>();

                list.Add(_anaIntro);
                list.Add(_anaPareados);
                list.Add(_arcadioIntro);

                return list;
            }
        }
        
        public FlagList()
        {
            _anaIntro = new Flag(Flag.Reference.AnaIntro);
            _anaPareados = new Flag(Flag.Reference.AnaPareados);
            _arcadioIntro = new Flag(Flag.Reference.ArcadioIntro);
        }
    }
}
