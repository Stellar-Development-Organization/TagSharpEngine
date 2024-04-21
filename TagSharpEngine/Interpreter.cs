using TagSharpEngine.Error;
using TagSharpEngine.Interface;

namespace TagSharpEngine {
    public class Node {
        public string? Output { get; set; }
        public Verb? Verb { get; set; }
        public Tuple<int, int> Coordinates { set; get; }

        public Node(Tuple<int, int> coordinates, Verb? verb = null) {
            Verb = verb;
            Coordinates = coordinates;
        }

        public override string ToString() {
            return Verb + " at " + Coordinates;
        }

        public static List<Node> BuildNodeTree(string message) {
            List<Node> nodes = new();
            string previous = @"";

            List<int> starts = new();
            for (int i = 0; i < message.Length; i++) {
                char ch = message[i];

                if (ch == '{' && previous != "\\") {
                    starts.Add(i);
                }

                if (ch == '}' && previous != "\\") {
                    if (starts.Count == 0) {
                        continue;
                    }

                    int start = starts[^1];
                    starts.RemoveAt(starts.Count - 1);

                    nodes.Add(new Node((start, i).ToTuple(), null));
                }

                previous = ch.ToString();
            }

            return nodes;
        }
    }

    public class Response {
        public string? Body;
        public Dictionary<string, object> Actions;
        public Dictionary<string, IAdapter> Variables;

        public Response(Dictionary<string, IAdapter>? variables = null) { 
            Body = null;
            Variables = variables ?? new();
            Actions = new();
        }
    }

    public class Context {
        public string OrignialMessage;
        public Interpreter Interpreter;
        public Verb Verb;
        public Response Response;

        public Context(Verb verb, Response resp, Interpreter interpreter, string og) {
            OrignialMessage = og;
            Verb = verb;
            Interpreter = interpreter;
            Response = resp;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Interpreter {
        public List<IBaseBlock> Blocks { get; set; }

        public Interpreter(List<IBaseBlock> blocks) {
            Blocks = blocks;
        }

        private Context GetContext(
            Node node, 
            string final,
            Response response,
            string originalMessage,
            int verbLimit,
            bool dotParam
        ) {
            var (start, end) = node.Coordinates;
            Console.WriteLine((start, end));
            node.Verb = new Verb(final.Substring(start, end + 1), limit: verbLimit, dotParam: dotParam);
            
            return new Context(node.Verb, response, this, originalMessage);
        }

        private List<IBaseBlock> GetAcceptors(Context ctx) {
            List<IBaseBlock> acceptors = new();
            foreach (var block in Blocks) {
                var result = block.WillAccept(ctx);
                Console.WriteLine(result);

                if (result) {
                    acceptors.Add(block);
                }
            }

            return acceptors;
        }

        private async Task<string?> ProcessBlocks(Context ctx, Node node) {
            var acceptors = GetAcceptors(ctx);
            foreach (var block in acceptors) {
                string? value = await block.Process(ctx);

                if (value is not null) {
                    node.Output = value;
                    return value;
                }
            }

            return null;
        }

        private static int CheckWorkLoad(int limit, int total, string output) {
            total += output.Length;
            if (total > limit) {
                throw new CharLimitExceeded("The TSE interpreter exceeded the workload.");
            }

            return total;
        }

        private static Tuple<string, int> TextDeform(int start, int end, string finalString, string output) {
            var msgLen = end + 1 - start;
            var replaceLen = output.Length;

            int diff = replaceLen - msgLen;
            finalString = finalString[..start] + output + finalString[(end + 1)..];

            return (finalString, diff).ToTuple();
        }

        private static void TranslateNodes(List<Node> nodes, int index, int start, int diff) {
            for (int i = index + 1; i < nodes.Count; i++) {
                Node future = nodes[i];
                int newStart, newEnd;

                newStart = future.Coordinates.Item1 > start ? future.Coordinates.Item1 + diff : future.Coordinates.Item1;
                newEnd = future.Coordinates.Item2 > start ? future.Coordinates.Item2 + diff : future.Coordinates.Item2;

                future.Coordinates = (newStart, newEnd).ToTuple();
            }
        }

        private async Task<string> Solve(
            string message, 
            List<Node> nodes, 
            Response resp, 
            int charLimit, 
            bool dotParam,
            int verbLimit = 2000
        ) {
            int totalWork = 0;
            string finalMsg = message;

            for (int i = 0; i < nodes.Count; i++) {
                Node node = nodes[i];
                var (start, end) = node.Coordinates;

                Context ctx = GetContext(node, finalMsg, resp, message, verbLimit, dotParam);
                string? output;

                try {
                    output = await ProcessBlocks(ctx, node);
                } catch (StopError exc) {
                    return finalMsg[..start] + exc.Message;
                }

                if (output is null) {
                    continue;
                }

                totalWork = CheckWorkLoad(charLimit, totalWork, output);
                var (final, diff) = TextDeform(start, end, finalMsg, output);

                finalMsg = final;
                TranslateNodes(nodes, i, start, diff);
            }

            return finalMsg;
        }

        /// <summary>
        /// Process the TagScript message given by user.
        /// </summary>
        /// <returns></returns>
        public async Task<Response> Process(
            string message,
            int charLimit,
            Dictionary<string, IAdapter>? seedVar = null,
            bool dotParam = false
        ) {
            Response response = new(seedVar);
            List<Node> nodes = Node.BuildNodeTree(message);

            string output;

            try {
                output = await Solve(message, nodes, response, charLimit, dotParam);
            } catch (TagSharpError) {
                throw;
            } catch (Exception exc) {
                throw new ProcessError(exc, response, this);
            }

            return ReturnResponse(response, output);
        }

        private static Response ReturnResponse(Response response, string output) {
            response.Body = response.Body is null ? output.Trim() : response.Body.Trim();
            return response;
        }
    }
}
