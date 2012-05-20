using NUnit.Framework;
using Reducely;

namespace reducely.tests
{
    [TestFixture]
    public class CanReduceByComposition
    {
        [Test]
        public void RemoveSwearWordsByComposition()
        {
            const string sentence = "this library is a fucking bloody pile of shit smelly code!";
            string censored;
            using(var builder = new ReducelyBuilder())
            using(var testBuilder = builder.For(new [] { GetType().Assembly} /* a list of assemblies that contain the reduce classes */))
            {
                var censor = testBuilder.Build<string, SwearWords>();
                censored = censor.For(sentence).By(new SwearWords());
            }

            Assert.That(censored.Replace(" ", ""), Is.EqualTo("thislibraryisapileofsmellycode!"));            
        }

        public class SwearWords
        {
            public string Fuck { get { return "fucking"; } }
            public string Suck { get { return "suck"; } }
            public string Shit { get { return "shit"; } }
            public string Bloody { get { return "bloody"; } }
        }

        public class CantSayFuck : ReduceFor<string, SwearWords>
        {
            public override string By(SwearWords criteria)
            {
                if (Source.Contains(criteria.Fuck))
                    Source = Source.Replace(criteria.Fuck, "");

                return Source;
            }
        }

        public class CantSaySuck : ReduceFor<string, SwearWords>
        {
            public override string By(SwearWords criteria)
            {
                if (Source.Contains(criteria.Suck))
                    Source = Source.Replace(criteria.Suck, "");

                return Source;
            }
        }

        public class CantSayShit : ReduceFor<string, SwearWords>
        {
            public override string By(SwearWords criteria)
            {
                if (Source.Contains(criteria.Shit))
                    Source = Source.Replace(criteria.Shit, "");

                return Source;
            }
        }

        public class CantSayBloody : ReduceFor<string, SwearWords>
        {
            public override string By(SwearWords criteria)
            {
                if (Source.Contains(criteria.Bloody))
                    Source = Source.Replace(criteria.Bloody, "");

                return Source;
            }
        }
    }
}