using TagSharpEngine.Interface;

namespace TagSharpEngine.Blocks {
    public class StrictVarGetterBlock: IBaseBlock {
        public IEnumerable<string>? AcceptedNames { get; set; }

        public bool WillAccept(Context ctx) {
            return ctx.Response.Variables.ContainsKey(ctx.Verb.Declaration!);
        }

        public async Task<string?> Process(Context ctx) {
            return await Task.FromResult(ctx.Response.Variables[ctx.Verb.Declaration!].GetValue(ctx.Verb));
        }
    }
}