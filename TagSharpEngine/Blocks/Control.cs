using TagSharpEngine.Interface;

namespace TagSharpEngine.Blocks {
    public class IfControl : IBaseBlock {
        public IEnumerable<string>? AcceptedNames { get; set; }

        public IfControl() {
            AcceptedNames = new string[] { "if" };
        }  

        public async Task<string?> Process(Context ctx) {
            var parse = Utils.HelperParseIf(ctx.Verb.Parameter!);
            return await Task.FromResult(Utils.ParseToOutput(ctx.Verb.Payload!, parse));
        }
    }

    public class AllControl : IBaseBlock {
        public IEnumerable<string>? AcceptedNames { get; set; }

        public AllControl() {
            AcceptedNames = new string[] { "all", "and" };
        }

        public async Task<string?> Process(Context ctx) {
            var parse = Utils.HelperParseListIf(ctx.Verb.Parameter!).All(item => item is not null && item!.Value);
            return await Task.FromResult(Utils.ParseToOutput(ctx.Verb.Payload!, parse));
        }
    }

    public class AnyControl: IBaseBlock {
        public IEnumerable<string>? AcceptedNames { get; set; }

        public AnyControl() {
            AcceptedNames = new string[] { "any", "or" };
        }

        public async Task<string?> Process(Context ctx) {
            var parse = Utils.HelperParseListIf(ctx.Verb.Parameter!).Any();
            return await Task.FromResult(Utils.ParseToOutput(ctx.Verb.Payload!, parse));
        }
    }
}