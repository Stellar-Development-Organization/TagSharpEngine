using TagSharpEngine.Error;
using TagSharpEngine.Interface;

namespace TagSharpEngine {
    public class Node {
        public string Output = string.Empty.ToString();
        public Verb? Verb;
        public Tuple<int, int> Coordinates;

        public Node(Tuple<int, int> coordinates, Verb? verb = null) {
            Verb = verb;
            Coordinates = coordinates;
        }

        public override string ToString() {
            return Verb + " at " + Coordinates;
        }

        public static List<Node> BuildNodeTree(string message) {
            ///
            List<Node> nodes = new();
            string previous = @"";

            List<int> starts = new();

            for (int i = 0; i < message.Length; i++) {
                char ch = message[i];

                switch (ch) {
                    case '{' when previous != @"\\":
                        starts.Add(i);
                        break;

                    case '}' when previous != @"\\":
                        if (starts.Count == 0) {
                            continue;
                        }

                        int val = starts[^1];
                        starts.RemoveAt(starts.Count - 1);

                        Tuple<int, int> coordinates = new(val, i);
                        Node node = new(coordinates);
                        nodes.Add(node);
                        break;
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

        public Response(Dictionary<string, IAdapter> ?variables = null) { 
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

    public class Interpreter {
        public List<IBlock> Blocks;

        public Interpreter(List<IBlock> blocks) {
            Blocks = blocks;
        }

        private Context GetContext(
            Node node, 
            string final, 
            Response response, 
            string originalMsg, 
            int verbLimit, 
            bool dotParam
        ) {

            var (start, ends) = node.Coordinates;
            node.Verb = new Verb(final.Substring(start, ends + 1), verbLimit, dotParam);
            return new Context(node.Verb, response, this, originalMsg);
        }

        private List<IBlock> GetAcceptors(Context ctx) {
            var acceptors = new List<IBlock>();

            foreach (IBlock block in Blocks) {
                if (block.WillAccept(ctx)) acceptors.Append(block);
            }

            return acceptors;
        }

        public string? ProcessBlocks(Context ctx, Node node) {
            var acceptors = GetAcceptors(ctx);

            foreach (IBlock? b in acceptors) {
                string? value = b.Process(ctx);

                if (value is not null) {
                    node.Output = value;
                    return value;
                }
            }

            return null;
        }

        private static int CheckWorkLoad(int charLimit, int totalWork, string output) {
            totalWork += output.Length;
            if (totalWork > charLimit) throw new CharLimitExceeded("The workload characters exceeded the limit."); 

            return totalWork;
        }

        private static Tuple<string, int> TextDeform(int start, int end, string final, string output) {
            var msgSlice = end + 1 - start;
            int replacement = output.Length;

            var differ = replacement - msgSlice;
            final = final.Substring(0, start) + output + final.Substring(end, 1);

            return (final, differ).ToTuple();
        }

        private static void TranslateNodes(List<Node> nodeList, int index, int start, int differ) {
            for (int i = index + 1; i < nodeList.Count; i++) {
                Node futureNode = nodeList[i];
                int newStart, newEnd;

                newStart = futureNode.Coordinates.Item1 > start ? 
                futureNode.Coordinates.Item1 + differ : futureNode.Coordinates.Item1;

                newEnd = futureNode.Coordinates.Item2 > start ?
                futureNode.Coordinates.Item2 + differ : futureNode.Coordinates.Item2;

                futureNode.Coordinates = (newStart, newEnd).ToTuple();
            }
        }

        private string Solve(
            string msg, 
            List<Node> nodeList, 
            Response response, 
            int charLimit, 
            bool dotParam,
            int verbLimit = 2000
        ) {
            int totalWork = 0;
            string final = msg;

            for (int i = 0; i < nodeList.Count; i++) {
                Node node = nodeList[i];
                var (start, ends) = node.Coordinates;
                var ctx = GetContext(node, final, response, msg, verbLimit, dotParam);
                string? output;

                try {
                    output = ProcessBlocks(ctx, node);
                } catch (StopError exc) {
                    return final.Substring(0, start) + exc.Message;
                }

                if (output is null) {
                    continue;
                }

                totalWork = CheckWorkLoad(charLimit, totalWork, output);
                var (finalize, differ) = TextDeform(start, ends, final, output);
                final = finalize;
                TranslateNodes(nodeList, i, start, differ);
            }

            return final;
        }

        public Response Process(
            string message, 
            int char_limit,
            Dictionary<string, IAdapter>? seed_var = null, 
            bool dot_param = false
        ) {
            var response = new Response(seed_var);
            List<Node> node_build_list = Node.BuildNodeTree(message);

            string output;
            
            try {
                output = Solve(message, node_build_list, response, char_limit, dot_param);
            } catch (TagSharpError exc) {
                throw exc;
            } catch (Exception exc) {
                throw new ProcessError(exc, response, this);
            }
            
            return ReturnResponse(response, output);
        }

        private static Response ReturnResponse(Response response, string output) {
            response.Body = response.Body is not null ? response.Body.Trim() : output.Trim();
            return response;
        }
    }
}
