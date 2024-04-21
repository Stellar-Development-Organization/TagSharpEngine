using TagSharpEngine.Interface;

namespace TagSharpEngine.Blocks {
    public class RandomBlock : IBaseBlock {
        public IEnumerable<string>? AcceptedNames { get; set; }

        public RandomBlock() {
            AcceptedNames = new string[] { "random", "#", "rand" };
        }

        public bool WillAccept(Context ctx) {
            return AcceptedNames!.Contains(ctx.Verb.Declaration!);
        }

        public async Task<string?> Process(Context ctx) {
            List<string> spl;
            if (ctx.Verb.Payload!.Contains("~")) {
                spl = ctx.Verb.Payload.Split('~').ToList();
            } else {
                spl = ctx.Verb.Payload.Split(",").ToList();
            }

            Random rand = new(spl.GetHashCode());
            int idx = rand.Next(spl.Count);

            return await Task.FromResult(spl[idx]) ?? null;
        }
    }
}