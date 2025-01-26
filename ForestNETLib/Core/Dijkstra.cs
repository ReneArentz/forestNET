namespace ForestNETLib.Core
{
    /// <summary>
    ///  Class to use dijkstra algorithm on a graph.
    /// </summary>
    public class Dijkstra<T> where T : notnull
    {

        /* Fields */

        private readonly List<Node> a_graph;
        private readonly List<T> a_mapping;
        private System.Collections.Generic.Dictionary<T, int>? a_shortestPathsSums;
        private System.Collections.Generic.Dictionary<T, List<T>>? a_shortestPaths;

        /* Properties */

        public List<Node> Graph
        {
            get { return this.a_graph; }
        }

        public List<T> Mapping
        {
            get { return this.a_mapping; }
        }

        /* Methods */

        /// <summary>
        /// Constructor, initialize graph and shortest paths list
        /// </summary>
        public Dijkstra()
        {
            this.a_graph = [];
            this.a_mapping = [];
            this.a_shortestPathsSums = null;
            this.a_shortestPaths = null;
        }

        /// <summary>
        /// Get weight sum of path to parameter node
        /// </summary>
        /// <param name="p_o_to">destination node</param>
        /// <returns>integer weight sum value</returns>
        public int ShortestPathSumTo(T p_o_to)
        {
            if ((this.a_shortestPathsSums == null) || (!this.a_shortestPathsSums.TryGetValue(p_o_to, out int p_i_value)))
            {
                return int.MaxValue / 2;
            }
            else
            {
                return p_i_value;
            }
        }

        /// <summary>
        /// Get list of nodes which show you the shortest path to destination node
        /// </summary>
        /// <param name="p_o_to">destination node</param>
        /// <returns>generic node list as shortest path to destination node</returns>
        public List<T>? ShortestPathTo(T p_o_to)
        {
            if ((this.a_shortestPaths == null) || (!this.a_shortestPaths.TryGetValue(p_o_to, out List<T>? p_a_value)))
            {
                return null;
            }
            else
            {
                return p_a_value;
            }
        }

        /// <summary>
        /// Add node with a path to another node as destination, give this path a weight
        /// </summary>
        /// <param name="p_o_node">source node</param>
        /// <param name="p_o_to">destination node</param>
        /// <param name="p_i_weight">weight for path from source to destination</param>
        public void Add(T p_o_node, T p_o_to, int p_i_weight)
        {
            /* add node to mapping */
            if (!this.a_mapping.Contains(p_o_node))
            {
                this.a_mapping.Add(p_o_node);
            }

            /* add to node value to mapping */
            if (!this.a_mapping.Contains(p_o_to))
            {
                this.a_mapping.Add(p_o_to);
            }

            /* add new node and use map indexes as values */
            this.a_graph.Add(new Node(this.a_mapping.IndexOf(p_o_node), this.a_mapping.IndexOf(p_o_to), p_i_weight));
            ForestNETLib.Core.Global.ILogConfig("added node with index '" + this.a_mapping.IndexOf(p_o_node) + "', to node '" + this.a_mapping.IndexOf(p_o_to) + "' and weight '" + p_i_weight + "'");
        }

        /// <summary>
        /// Execute dijkstra algorithm on graph to calculate the shortest paths from a start node to all other reachable nodes
        /// </summary>
        /// <param name="p_o_startNode">define a start node to calculate shortest paths to other nodes</param>
        public void ExecuteDijkstra(T p_o_startNode)
        {
            int i_size = this.a_graph.Count;
            int i_amount = this.a_mapping.Count;
            int i_minDistance;
            int[,] a_distances = new int[i_size, i_size];
            int[] a_shortestPaths = new int[i_size];
            int[] a_predecessors = new int[i_size];
            bool[] a_visited = new bool[i_size];

            /* set all distances to 0 */
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < i_size; j++)
                {
                    a_distances[i, j] = 0;
                }
            }

            ForestNETLib.Core.Global.ILogMass("set all distances to 0");

            for (int i = 0; i < i_size; i++)
            {
                /* set shortest paths to max value */
                a_shortestPaths[i] = int.MaxValue / 2;
                /* set all predecessors to -1 */
                a_predecessors[i] = -1;
                /* no node has been visited yet */
                a_visited[i] = false;
                /* get first distances from node information */
                a_distances[this.a_graph[i].NodeValue, this.a_graph[i].To] = this.a_graph[i].Weight;
                ForestNETLib.Core.Global.ILogMass("set node '" + i + "' to standard values: max weights, predecessors -1, visited false and first distances");
            }

            /* set path to own node to 0 */
            if (this.a_mapping.Contains(p_o_startNode))
            {
                a_shortestPaths[this.a_mapping.IndexOf(p_o_startNode)] = 0;
                ForestNETLib.Core.Global.ILogMass("start node '" + p_o_startNode + "' shortest path is '0'");
            }

            /* dijkstra algorithm */
            for (int i = 0; i < i_amount; i++)
            {
                i_minDistance = -1;

                for (int j = 0; j < i_amount; j++)
                {
                    if (!a_visited[j] && ((i_minDistance == -1) || (a_shortestPaths[j] < a_shortestPaths[i_minDistance])))
                    {
                        ForestNETLib.Core.Global.ILogMass("set min distance '" + j + "', because '" + j + "' was not visited and min distance is '-1', or weight sum to '" + j + "(" + a_shortestPaths[j] + ")' is lower than " + ((i_minDistance >= 0) ? a_shortestPaths[i_minDistance].ToString() : "infinity"));
                        i_minDistance = j;
                    }
                }

                a_visited[i_minDistance] = true;
                ForestNETLib.Core.Global.ILogMass(i_minDistance + " was visited just now");

                for (int j = 0; j < i_amount; j++)
                {
                    ForestNETLib.Core.Global.ILogMass("check distance from '" + i + "' to '" + j + "'");
                    if (a_distances[i_minDistance, j] != 0)
                    {
                        if (a_shortestPaths[i_minDistance] + a_distances[i_minDistance, j] < a_shortestPaths[j])
                        {
                            ForestNETLib.Core.Global.ILogMass("distance from '" + i_minDistance + "' to '" + j + "': " + a_distances[i_minDistance, j] + " != 0");
                            ForestNETLib.Core.Global.ILogMass("weight sum(" + a_shortestPaths[i_minDistance] + ") from '" + i_minDistance + "' plus distance from '" + i_minDistance + "' to '" + j + "' [" + a_distances[i_minDistance, j] + "] is lower than weight sum(" + a_shortestPaths[j] + ") from '" + j + "'");
                            ForestNETLib.Core.Global.ILogMass("update weight sum(" + a_shortestPaths[j] + ") from '" + j + "' to (" + (int)(a_shortestPaths[i_minDistance] + a_distances[i_minDistance, j]) + ")");
                            ForestNETLib.Core.Global.ILogMass("update predecessors(" + a_predecessors[j] + ") from '" + j + "' to (" + (int)(i_minDistance + 1) + ")");
                            a_shortestPaths[j] = a_shortestPaths[i_minDistance] + a_distances[i_minDistance, j];
                            a_predecessors[j] = i_minDistance + 1;
                        }
                    }
                }
            }

            this.a_shortestPathsSums = [];
            this.a_shortestPaths = [];

            /* fill shortest path mapping with sum weights with list of shortest paths of dijkstra algorithm */
            for (int i = 0; i < i_amount; i++)
            {
                this.a_shortestPathsSums[this.a_mapping[i]] = a_shortestPaths[i];
                ForestNETLib.Core.Global.ILogFinest("added weight sum of shortest path '" + a_shortestPaths[i] + "' from start node '" + this.a_mapping.IndexOf(p_o_startNode) + " to destination node '" + i + "'");
            }

            /* generate map of shortest paths to each node */
            for (int i = 0; i < i_amount; i++)
            {
                int i_to = i;
                /* start with destination node */
                List<T> a_path =
                [
                    this.a_mapping[i_to]
                ];

                ForestNETLib.Core.Global.ILogFinest("added destination node '" + this.a_mapping[i_to] + "(" + i_to + ")' to shortest path");
                /* go shortest path back until reaching start node */
                while (a_predecessors[i_to] != -1)
                {
                    i_to = a_predecessors[i_to] - 1;
                    a_path.Add(this.a_mapping[i_to]);
                    ForestNETLib.Core.Global.ILogFinest("added predecessors node '" + this.a_mapping[i_to] + "(" + i_to + ")' to shortest path");
                }

                /* reverse shortest path list */
                a_path.Reverse();
                ForestNETLib.Core.Global.ILogMass("reverse shortest path for right order from start node '" + this.a_mapping.IndexOf(p_o_startNode) + "' to destination node '" + i + "'");

                /* add shortest path list to property list with destination node as key */
                this.a_shortestPaths[this.a_mapping[i]] = a_path;
                ForestNETLib.Core.Global.ILogFinest("added shortest path " + a_path + " to class property");
            }
        }

        /* Internal Classes */

        /// <summary>
        /// Internal class to store node information like to-node and weight-value
        /// </summary>
        public class Node
        {

            /* Fields */

            /* Properties */

            public int NodeValue { get; private set; }
            public int To { get; private set; }
            public int Weight { get; private set; }

            /* Methods */

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="p_i_nodeValue">give node a unique integer value</param>
            /// <param name="p_i_to">give node a to node</param>
            /// <param name="p_i_weight">give node a weight</param>
            public Node(int p_i_nodeValue, int p_i_to, int p_i_weight)
            {
                this.NodeValue = p_i_nodeValue;
                this.To = p_i_to;
                this.Weight = p_i_weight;
            }
        }
    }
}