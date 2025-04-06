namespace Sandbox.Tests
{
    public class SortsTest
    {
        public static void TestSorts()
        {
            TestSortsList(true, 50_000, true, true, true, false, false, false);
            TestSortsList(true, 5_000_000, false, false, false, true, true, true);
            TestSortsList(false, 50_000, true, true, true, false, false, false);
            TestSortsList(false, 5_000_000, false, false, false, true, true, true);

            TestSortsMap(true, 50_000, true, true, true, false, false, false, true);
            TestSortsMap(true, 5_000_000, false, false, false, true, true, true, true);
            TestSortsMap(true, 50_000, true, true, true, false, false, false, false);
            TestSortsMap(true, 5_000_000, false, false, false, true, true, true, false);
            TestSortsMap(false, 50_000, true, true, true, false, false, false, true);
            TestSortsMap(false, 5_000_000, false, false, false, true, true, true, true);
            TestSortsMap(false, 50_000, true, true, true, false, false, false, false);
            TestSortsMap(false, 5_000_000, false, false, false, true, true, true, false);
        }

        private static void TestSortsList(bool p_b_unique, int p_i_amount, bool p_b_selection, bool p_b_insertion, bool p_b_bubble, bool p_b_heap, bool p_b_merge, bool p_b_quick)
        {
            int i_showValuesAmount = 10;

            ForestNET.Lib.Global.Log("Generating '" + $"{p_i_amount:N0}" + "' " + ((p_b_unique) ? "unique" : "random") + " values . . . ");

            List<int> a_list = [];

            if (p_b_unique)
            {
                for (int i = 1; i <= p_i_amount; i++)
                {
                    a_list.Add(i);
                }
            }
            else
            {
                for (int i = 1; i <= p_i_amount; i++)
                {
                    a_list.Add(ForestNET.Lib.Helper.RandomIntegerRange(1, p_i_amount));
                }
            }

            ForestNET.Lib.ConsoleProgressBar o_progressBar = new(4, 4);

            ForestNET.Lib.Sorts.PostProgress del_postProgress =
                p_d_progress =>
                {
                    o_progressBar.Report = p_d_progress;
                };

            /* ###################################################################################################### */

            if (p_b_selection)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);
                o_progressBar.Init("SelectionSort . . .", "Done.");
                ForestNET.Lib.Sorts.SelectionSort(a_list, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (int i_foo in a_list)
                {
                    System.Console.Write(i_foo + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }

            /* ###################################################################################################### */

            if (p_b_insertion)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);
                o_progressBar.Init("InsertionSort . . .", "Done.");
                ForestNET.Lib.Sorts.InsertionSort(a_list, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (int i_foo in a_list)
                {
                    System.Console.Write(i_foo + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }

            /* ###################################################################################################### */

            if (p_b_bubble)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);
                o_progressBar.Init("BubbleSort . . .", "Done.");
                ForestNET.Lib.Sorts.BubbleSort(a_list, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (int i_foo in a_list)
                {
                    System.Console.Write(i_foo + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }

            /* ###################################################################################################### */

            if (p_b_heap)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);
                o_progressBar.Init("HeapSort . . .", "Done.");
                ForestNET.Lib.Sorts.HeapSort(a_list, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (int i_foo in a_list)
                {
                    System.Console.Write(i_foo + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }

            /* ###################################################################################################### */

            if (p_b_merge)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);
                o_progressBar.Init("MergeSort . . .", "Done.");
                ForestNET.Lib.Sorts.MergeSort(a_list, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (int i_foo in a_list)
                {
                    System.Console.Write(i_foo + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }

            /* ###################################################################################################### */

            if (p_b_quick)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);
                o_progressBar.Init("QuickSort . . .", "Done.");
                ForestNET.Lib.Sorts.QuickSort(a_list, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (int i_foo in a_list)
                {
                    System.Console.Write(i_foo + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }
        }

        private static void TestSortsMap(bool p_b_unique, int p_i_amount, bool p_b_selection, bool p_b_insertion, bool p_b_bubble, bool p_b_heap, bool p_b_merge, bool p_b_quick, bool p_b_sortByValue)
        {
            int i_showValuesAmount = 10;

            ForestNET.Lib.Global.Log("Generating '" + $"{p_i_amount:N0}" + "' " + ((p_b_unique) ? "unique" : "random") + " values - sort by value: " + p_b_sortByValue + " . . . ");

            List<int> a_list = [];
            List<KeyValuePair<string, int>> a_return = [];
            Dictionary<string, int> m_map = [];

            if (p_b_unique)
            {
                for (int i = 1; i <= p_i_amount; i++)
                {
                    a_list.Add(i);
                }
            }
            else
            {
                for (int i = 1; i <= p_i_amount; i++)
                {
                    a_list.Add(ForestNET.Lib.Helper.RandomIntegerRange(1, p_i_amount));
                }
            }

            ForestNET.Lib.ConsoleProgressBar o_progressBar = new(4, 4);

            ForestNET.Lib.Sorts.PostProgress del_postProgress =
                p_d_progress =>
                {
                    o_progressBar.Report = p_d_progress;
                };

            /* ###################################################################################################### */

            if (p_b_selection)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);

                for (int j = 1; j <= p_i_amount; j++)
                {
                    m_map[j.ToString("X8")] = a_list[j - 1];
                }

                o_progressBar.Init("SelectionSort . . .", "Done.");
                a_return = ForestNET.Lib.Sorts.SelectionSort(m_map, p_b_sortByValue, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (KeyValuePair<string, int> o_foo in a_return)
                {
                    System.Console.Write(o_foo.Key + ":" + o_foo.Value + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }

            /* ###################################################################################################### */

            if (p_b_insertion)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);

                for (int j = 1; j <= p_i_amount; j++)
                {
                    m_map[j.ToString("X8")] = a_list[j - 1];
                }

                o_progressBar.Init("InsertionSort . . .", "Done.");
                a_return = ForestNET.Lib.Sorts.InsertionSort(m_map, p_b_sortByValue, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (KeyValuePair<string, int> o_foo in a_return)
                {
                    System.Console.Write(o_foo.Key + ":" + o_foo.Value + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }

            /* ###################################################################################################### */

            if (p_b_bubble)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);

                for (int j = 1; j <= p_i_amount; j++)
                {
                    m_map[j.ToString("X8")] = a_list[j - 1];
                }

                o_progressBar.Init("BubbleSort . . .", "Done.");
                a_return = ForestNET.Lib.Sorts.BubbleSort(m_map, p_b_sortByValue, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (KeyValuePair<string, int> o_foo in a_return)
                {
                    System.Console.Write(o_foo.Key + ":" + o_foo.Value + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }

            /* ###################################################################################################### */

            if (p_b_heap)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);

                for (int j = 1; j <= p_i_amount; j++)
                {
                    m_map[j.ToString("X8")] = a_list[j - 1];
                }

                o_progressBar.Init("HeapSort . . .", "Done.");
                a_return = ForestNET.Lib.Sorts.HeapSort(m_map, p_b_sortByValue, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (KeyValuePair<string, int> o_foo in a_return)
                {
                    System.Console.Write(o_foo.Key + ":" + o_foo.Value + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }

            /* ###################################################################################################### */

            if (p_b_merge)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);

                for (int j = 1; j <= p_i_amount; j++)
                {
                    m_map[j.ToString("X8")] = a_list[j - 1];
                }

                o_progressBar.Init("MergeSort . . .", "Done.");
                a_return = ForestNET.Lib.Sorts.MergeSort(m_map, p_b_sortByValue, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (KeyValuePair<string, int> o_foo in a_return)
                {
                    System.Console.Write(o_foo.Key + ":" + o_foo.Value + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }

            /* ###################################################################################################### */

            if (p_b_quick)
            {
                ForestNET.Lib.Helper.ShuffleList(a_list);

                for (int j = 1; j <= p_i_amount; j++)
                {
                    m_map[j.ToString("X8")] = a_list[j - 1];
                }

                o_progressBar.Init("QuickSort . . .", "Done.");
                a_return = ForestNET.Lib.Sorts.QuickSort(m_map, p_b_sortByValue, del_postProgress);
                o_progressBar.Close();

                int i = 1;

                foreach (KeyValuePair<string, int> o_foo in a_return)
                {
                    System.Console.Write(o_foo.Key + ":" + o_foo.Value + " ");

                    if (i++ == i_showValuesAmount)
                    {
                        System.Console.WriteLine("");
                        break;
                    }
                }
            }
        }
    }
}
