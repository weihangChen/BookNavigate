using NHunspell;
using System;

namespace Infrastructure.Services
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
            hunspell = new Hunspell("en_us.aff", "en_us.dic");
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
