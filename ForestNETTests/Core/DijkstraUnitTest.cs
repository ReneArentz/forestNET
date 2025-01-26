namespace ForestNETTests.Core
{
    public class DijkstraUnitTest
    {
        [Test]
        public void TestDijkstra()
        {
            /* Test graph */

            ForestNETLib.Core.Dijkstra<int> o_graphInteger = new();
            o_graphInteger.Add(1, 6, 3);
            o_graphInteger.Add(2, 1, 1);
            o_graphInteger.Add(2, 3, 2);
            o_graphInteger.Add(2, 6, 4);
            o_graphInteger.Add(3, 4, 6);
            o_graphInteger.Add(3, 5, 3);
            o_graphInteger.Add(5, 3, 2);
            o_graphInteger.Add(5, 4, 2);
            o_graphInteger.Add(6, 2, 1);
            o_graphInteger.Add(6, 5, 7);
            o_graphInteger.Add(6, 7, 8);
            o_graphInteger.Add(7, 2, 1);
            o_graphInteger.Add(7, 3, 9);
            o_graphInteger.Add(7, 5, 2);
            o_graphInteger.Add(7, 8, 1);
            o_graphInteger.Add(8, 6, 10);
            o_graphInteger.Add(8, 1, 1);
            o_graphInteger.Add(4, 7, 11);

            List<int> a_shortestPathsIntegerResult = [1, 0, 2, 7, 5, 4, 12, 13];

            List<List<int>> a_shortestPathsToIntegerResult = [
                [2, 1],
                [2],
                [2, 3],
                [2, 3, 5, 4],
                [2, 3, 5],
                [2, 6],
                [2, 6, 7],
                [2, 6, 7, 8]
            ];

            int i_startNode = 2;
            o_graphInteger.ExecuteDijkstra(i_startNode);

            for (int i = 0; i < o_graphInteger.Mapping.Count; i++)
            {
                Assert.That(
                    o_graphInteger.ShortestPathSumTo(i + 1), Is.EqualTo(a_shortestPathsIntegerResult[i]),
                    "shortest path sum[" + o_graphInteger.ShortestPathSumTo((i + 1)) + "] from '" + i_startNode + "' to '" + (i + 1) + "' is not '" + a_shortestPathsIntegerResult[i] + "'"
                );
            }

            for (int i = 0; i < o_graphInteger.Mapping.Count; i++)
            {
                Assert.That(
                    o_graphInteger.ShortestPathTo(i + 1)?.SequenceEqual(a_shortestPathsToIntegerResult[i]),
                    Is.True,
                    "shortest path " + ForestNETLib.Core.Helper.PrintArrayList<int>(o_graphInteger.ShortestPathTo(i + 1) ?? []) + " from '" + i_startNode + "' to '" + (i + 1) + "' is not '" + ForestNETLib.Core.Helper.PrintArrayList<int>(a_shortestPathsToIntegerResult[i]) + "'"
                );
            }

            /* Test graph with string values */

            List<string> a_locations =
            [
                "London",
                "Birmingham",
                "Oxford",
                "Cambridge",
                "Southampton",
                "Bristol",
                "Liverpool",
                "Manchester"
            ];

            ForestNETLib.Core.Dijkstra<string> o_graph = new();
            o_graph.Add(a_locations[0], a_locations[5], 3);
            o_graph.Add(a_locations[1], a_locations[0], 1);
            o_graph.Add(a_locations[1], a_locations[2], 2);
            o_graph.Add(a_locations[1], a_locations[5], 4);
            o_graph.Add(a_locations[2], a_locations[3], 6);
            o_graph.Add(a_locations[2], a_locations[4], 3);
            o_graph.Add(a_locations[4], a_locations[2], 2);
            o_graph.Add(a_locations[4], a_locations[3], 2);
            o_graph.Add(a_locations[5], a_locations[1], 1);
            o_graph.Add(a_locations[5], a_locations[4], 7);
            o_graph.Add(a_locations[5], a_locations[6], 8);
            o_graph.Add(a_locations[6], a_locations[1], 1);
            o_graph.Add(a_locations[6], a_locations[2], 9);
            o_graph.Add(a_locations[6], a_locations[4], 2);
            o_graph.Add(a_locations[6], a_locations[7], 1);
            o_graph.Add(a_locations[7], a_locations[5], 10);
            o_graph.Add(a_locations[7], a_locations[0], 1);
            o_graph.Add(a_locations[3], a_locations[6], 11);

            List<int> a_shortestPathsResult = [13, 12, 14, 0, 13, 16, 11, 12];

            List<List<string>> a_shortestPathsToResult = [
                ["Cambridge", "Liverpool", "Birmingham", "London"],
                ["Cambridge", "Liverpool", "Birmingham"],
                ["Cambridge", "Liverpool", "Birmingham", "Oxford"],
                ["Cambridge"],
                ["Cambridge", "Liverpool", "Southampton"],
                ["Cambridge", "Liverpool", "Birmingham", "Bristol"],
                ["Cambridge", "Liverpool"],
                ["Cambridge", "Liverpool", "Manchester"]
            ];

            string s_startNode = a_locations[3]; /* Cambridge */
            o_graph.ExecuteDijkstra(s_startNode);

            for (int i = 0; i < a_locations.Count; i++)
            {
                Assert.That(
                    o_graph.ShortestPathSumTo(a_locations[i]), Is.EqualTo(a_shortestPathsResult[i]),
                    "shortest path sum[" + o_graph.ShortestPathSumTo(a_locations[i]) + "] from '" + s_startNode + "' to '" + a_locations[i] + "' is not '" + a_shortestPathsResult[i] + "'"
                );
            }

            for (int i = 0; i < a_locations.Count; i++)
            {
                Assert.That(
                    o_graph.ShortestPathTo(a_locations[i])?.SequenceEqual(a_shortestPathsToResult[i]),
                    Is.True,
                    "shortest path " + ForestNETLib.Core.Helper.PrintArrayList<string>(o_graph.ShortestPathTo(a_locations[i]) ?? []) + " from '" + s_startNode + "' to '" + a_locations[i] + "' is not '" + ForestNETLib.Core.Helper.PrintArrayList<string>(a_shortestPathsToResult[i]) + "'"
                );
            }
        }
    }
}
