using NHunspell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleCharacterCollect.Services
{
    public interface ISpellService
    {
        bool IsWordCorrect(string word);
    }
    public class SpellService : IDisposable, ISpellService
    {
        Hunspell hunspell;
        public SpellService()
        {
            hunspell = new Hunspell("DLL/en_us.aff", "DLL/en_us.dic");
        }

        public bool IsWordCorrect(string word)
        {
            var correct = hunspell.Spell(word);
            return correct;
        }

        
        public void Dispose()
        {
            hunspell.Dispose();
        }
    }
}
