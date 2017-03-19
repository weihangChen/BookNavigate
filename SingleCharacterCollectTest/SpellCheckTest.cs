using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHunspell;
using System.Collections.Generic;

namespace SingleCharacterCollectTest
{
    [TestClass]
    public class SpellCheckTest
    {
        [TestMethod]
        public void SpellTest()
        {
            using (Hunspell hunspell = new Hunspell("DLL/en_us.aff", "DLL/en_us.dic"))
            {
                Assert.IsTrue(hunspell.Spell("Recommendation"));
                Assert.IsFalse(hunspell.Spell("Recommendaton"));
                
            }
        }
    }
}
