using TagSharpEngine.Interface;

namespace TagSharpEngine.Blocks {
    public class SleepBlock : IBaseBlock {
        public IEnumerable<string>? AcceptedNames
        {
            get { return AcceptedNames; }
            set
            {
                string[] AcceptName = { "sleep", "wait", "await", "delay", "suspend" };
                AcceptedNames = AcceptName.AsEnumerable();
            }
        }        

        public async Task<string?> Process(Context ctx) {
            var time = ctx.Verb.Parameter!;
            var display_msg = ctx.Verb.Payload;
            var abs_time = int.Parse(time);

            await Task.Delay(abs_time * 1000);
            return display_msg;
        }
    }
}
