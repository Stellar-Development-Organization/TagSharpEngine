using TagSharpEngine.Interface;

namespace TagSharpEngine.Blocks {
    public class UpperBlock : IBaseBlock {
        public IEnumerable<string>? AcceptedNames { get; set; }

        public UpperBlock() {
            string[] names = { "upper", "uppercase" };
            AcceptedNames = names.AsEnumerable();
        }

        public async Task<string?> Process(Context ctx) {
            string text = ctx.Verb.Parameter!.ToUpper();
            return await Task.FromResult(text);
        }
    }

    public class LowerBlock : IBaseBlock {
        public IEnumerable<string>? AcceptedNames
        {
            get { 
                string[] names = { "lower", "lowercase" };
                return names;
            }
            set
            {
                string[] names = { "lower", "lowercase" };
                AcceptedNames = names.AsEnumerable();
            }
        }

        public async Task<string?> Process(Context ctx) {
            string? text = ctx.Verb?.Parameter?.ToString().ToLower();
            return await Task.FromResult(text);
        }
    }
}
