using TagSharpEngine.Interface;

namespace TagSharpEngine.Blocks {
    public class LooseVarGetterBlock : IBaseBlock {
        public IEnumerable<string>? AcceptedNames { get; set; }

        public bool WillAccept(Context ctx) {
            return true;
        }

        public async Task<string?> Process(Context ctx) {
            string? result = !ctx.Response.Variables.TryGetValue(ctx.Verb.Declaration!, out IAdapter? value) ? null : value.GetValue(ctx.Verb);
            return await Task.FromResult(result);
        }
    }
}