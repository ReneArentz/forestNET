namespace ForestNETTests.Core
{
    public class SortsUnitTest
    {
        [Test]
        public void TestSorts()
        {
            ExecuteSortsList(true, 5000, true, true, true, false, false, false);
            ExecuteSortsList(false, 5000, true, true, true, false, false, false);
            ExecuteSortsList(true, 5000, false, false, false, true, true, true);
            ExecuteSortsList(false, 5000, false, false, false, true, true, true);

            ExecuteSortsMap(true, 5000, true, true, true, false, false, false, true);
            ExecuteSortsMap(true, 5000, true, true, true, false, false, false, false);
            ExecuteSortsMap(false, 5000, true, true, true, false, false, false, true);
            ExecuteSortsMap(false, 5000, true, true, true, false, false, false, false);

            ExecuteSortsMap(true, 5000, false, false, false, true, true, true, true);
            ExecuteSortsMap(true, 5000, false, false, false, true, true, true, false);
            ExecuteSortsMap(false, 5000, false, false, false, true, true, true, true);
            ExecuteSortsMap(false, 5000, false, false, false, true, true, true, false);
        }

        private static void ExecuteSortsList(bool p_b_unique, int p_i_amount, bool p_b_selection, bool p_b_insertion, bool p_b_bubble, bool p_b_heap, bool p_b_merge, bool p_b_quick)
        {
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
                    a_list.Add(ForestNETLib.Core.Helper.RandomIntegerRange(1, p_i_amount));
                }
            }

            /* ###################################################################################################### */

            if (p_b_selection)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);
                ForestNETLib.Core.Sorts.SelectionSort(a_list);
                ExecuteSortsListValidate(p_b_unique, a_list);
            }

            /* ###################################################################################################### */

            if (p_b_insertion)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);
                ForestNETLib.Core.Sorts.InsertionSort(a_list);
                ExecuteSortsListValidate(p_b_unique, a_list);
            }

            /* ###################################################################################################### */

            if (p_b_bubble)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);
                ForestNETLib.Core.Sorts.BubbleSort(a_list);
                ExecuteSortsListValidate(p_b_unique, a_list);
            }

            /* ###################################################################################################### */

            if (p_b_heap)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);
                ForestNETLib.Core.Sorts.HeapSort(a_list);
                ExecuteSortsListValidate(p_b_unique, a_list);
            }

            /* ###################################################################################################### */

            if (p_b_merge)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);
                ForestNETLib.Core.Sorts.MergeSort(a_list);
                ExecuteSortsListValidate(p_b_unique, a_list);
            }

            /* ###################################################################################################### */

            if (p_b_quick)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);
                ForestNETLib.Core.Sorts.QuickSort(a_list);
                ExecuteSortsListValidate(p_b_unique, a_list);
            }
        }

        private static void ExecuteSortsListValidate(bool p_b_unique, List<int> p_a_list)
        {
            if (p_b_unique)
            {
                int i = 1;

                foreach (int i_foo in p_a_list)
                {
                    Assert.That(i_foo, Is.EqualTo(i++), "ExecuteSortsListValidate error: " + i_foo + " != " + (i + 1));
                }
            }
            else
            {
                int i = -1;

                foreach (int i_foo in p_a_list)
                {
                    if (i > 0)
                    {
                        Assert.That(i, Is.LessThanOrEqualTo(i_foo), "ExecuteSortsListValidate error: " + i + " > " + i_foo);
                    }

                    i = i_foo;
                }
            }
        }

        private static void ExecuteSortsMap(bool p_b_unique, int p_i_amount, bool p_b_selection, bool p_b_insertion, bool p_b_bubble, bool p_b_heap, bool p_b_merge, bool p_b_quick, bool p_b_sortByValue)
        {
            List<int> a_list = [];
            Dictionary<string, int> a_map = [];
            List<KeyValuePair<string, int>> a_return;

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
                    a_list.Add(ForestNETLib.Core.Helper.RandomIntegerRange(1, p_i_amount));
                }
            }

            /* ###################################################################################################### */

            if (p_b_selection)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);

                for (int i = 1; i <= p_i_amount; i++)
                {
                    a_map[i.ToString("X8")] = a_list[i - 1];
                }

                a_return = ForestNETLib.Core.Sorts.SelectionSort(a_map, p_b_sortByValue);
                ExecuteSortsMapValidate(p_b_unique, p_b_sortByValue, a_return);
            }

            /* ###################################################################################################### */

            if (p_b_insertion)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);

                for (int i = 1; i <= p_i_amount; i++)
                {
                    a_map[i.ToString("X8")] = a_list[i - 1];
                }

                a_return = ForestNETLib.Core.Sorts.InsertionSort(a_map, p_b_sortByValue);
                ExecuteSortsMapValidate(p_b_unique, p_b_sortByValue, a_return);
            }

            /* ###################################################################################################### */

            if (p_b_bubble)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);

                for (int i = 1; i <= p_i_amount; i++)
                {
                    a_map[i.ToString("X8")] = a_list[i - 1];
                }

                a_return = ForestNETLib.Core.Sorts.BubbleSort(a_map, p_b_sortByValue);
                ExecuteSortsMapValidate(p_b_unique, p_b_sortByValue, a_return);
            }

            /* ###################################################################################################### */

            if (p_b_heap)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);

                for (int i = 1; i <= p_i_amount; i++)
                {
                    a_map[i.ToString("X8")] = a_list[i - 1];
                }

                a_return = ForestNETLib.Core.Sorts.HeapSort(a_map, p_b_sortByValue);
                ExecuteSortsMapValidate(p_b_unique, p_b_sortByValue, a_return);
            }

            /* ###################################################################################################### */

            if (p_b_merge)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);

                for (int i = 1; i <= p_i_amount; i++)
                {
                    a_map[i.ToString("X8")] = a_list[i - 1];
                }

                a_return = ForestNETLib.Core.Sorts.MergeSort(a_map, p_b_sortByValue);
                ExecuteSortsMapValidate(p_b_unique, p_b_sortByValue, a_return);
            }

            /* ###################################################################################################### */

            if (p_b_quick)
            {
                ForestNETLib.Core.Helper.ShuffleList(a_list);

                for (int i = 1; i <= p_i_amount; i++)
                {
                    a_map[i.ToString("X8")] = a_list[i - 1];
                }

                a_return = ForestNETLib.Core.Sorts.QuickSort(a_map, p_b_sortByValue);
                ExecuteSortsMapValidate(p_b_unique, p_b_sortByValue, a_return);
            }
        }

        private static void ExecuteSortsMapValidate(bool p_b_unique, bool p_b_sortByValue, List<KeyValuePair<string, int>> p_a_map)
        {
            if (p_b_unique)
            {
                int i = 1;
                string? s_foo = null;

                foreach (KeyValuePair<string, int> o_entry in p_a_map)
                {
                    if (p_b_sortByValue)
                    {
                        Assert.That(o_entry.Value, Is.EqualTo(i++), "ExecuteSortsMapValidate error: " + o_entry.Value + " != " + (i + 1));
                    }
                    else
                    {
                        if (s_foo != null)
                        {
                            Assert.That(s_foo.CompareTo(o_entry.Key), Is.LessThanOrEqualTo(0), "ExecuteSortsMapValidate error: " + s_foo + " > " + o_entry.Key);
                        }

                        s_foo = o_entry.Key;
                    }
                }
            }
            else
            {
                int i = -1;
                string? s_foo = null;

                foreach (KeyValuePair<string, int> o_entry in p_a_map)
                {
                    if (!p_b_sortByValue)
                    {
                        if (p_b_sortByValue)
                        {
                            Assert.That(o_entry.Value, Is.EqualTo(i++), "ExecuteSortsMapValidate error: " + o_entry.Value + " != " + (i + 1));
                        }
                        else
                        {
                            if (s_foo != null)
                            {
                                Assert.That(s_foo.CompareTo(o_entry.Key), Is.LessThanOrEqualTo(0), "ExecuteSortsMapValidate error: " + s_foo + " > " + o_entry.Key);
                            }

                            s_foo = o_entry.Key;
                        }
                    }
                    else
                    {
                        if (i > 0)
                        {
                            Assert.That(i, Is.LessThanOrEqualTo(o_entry.Value), "ExecuteSortsMapValidate error: " + i + " > " + o_entry.Value);
                        }

                        i = o_entry.Value;
                    }
                }
            }
        }
    }
}
