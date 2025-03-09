namespace ForestNETLib.Core
{
    /// <summary>
    /// Collection of static methods to sort dynamic lists and dynamic key-value maps.
    /// Available sort algorithms:
    ///  - selection
    ///  - insertion
    ///  - bubble
    ///  - heap
    ///  - merge
    ///  - quick
    /// Also possibility to get sort progress with delegate implementation.
    /// </summary>
    public class Sorts
    {

        /* Delegates */

        /// <summary>
        /// delegate which can be implemented as a parameter with the sort methods to post sort progress
        /// </summary>
        public delegate void PostProgress(double p_d_progress);

        /* Fields */

        /* Properties */

        /* Methods */

        /// <summary>
        /// Sort a dynamic list with selection sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        public static void SelectionSort<T>(List<T?> p_ol_list) where T : IComparable
        {
            Sorts.SelectionSort(p_ol_list, null);
        }

        /// <summary>
        /// Sort a dynamic list with selection sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        public static void SelectionSort<T>(List<T?> p_ol_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable
        {
            long l_overallCount = 0;
            int i_min, i_left = 0;

            do
            {
                i_min = i_left;

                /* find minimum */
                for (int i = i_left + 1; i < p_ol_list.Count; i++)
                {
                    /* compare */
                    if (p_ol_list[i]?.CompareTo(p_ol_list[i_min]) < 0)
                    {
                        i_min = i;
                    }
                }

                /* swap */
                T? o_temp = p_ol_list[i_min];
                p_ol_list[i_min] = p_ol_list[i_left];
                p_ol_list[i_left] = o_temp;

                i_left++;
                l_overallCount++;

                p_del_postProgress?.Invoke(l_overallCount / (double)p_ol_list.Count);
            } while (i_left < p_ol_list.Count);

            p_del_postProgress?.Invoke(l_overallCount / (double)p_ol_list.Count);
        }


        /// <summary>
        /// Sort a dynamic list with insertion sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        public static void InsertionSort<T>(List<T?> p_ol_list) where T : IComparable
        {
            Sorts.InsertionSort(p_ol_list, null);
        }

        /// <summary>
        /// Sort a dynamic list with insertion sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        public static void InsertionSort<T>(List<T?> p_ol_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable
        {
            long l_overallCount = 0;

            for (int i = 1; i < p_ol_list.Count; i++)
            {
                T? o_temp = p_ol_list[i];
                int j = i;

                /* compare */
                while ((j > 0) && (p_ol_list[j - 1]?.CompareTo(o_temp) > 0))
                {
                    /* swap */
                    p_ol_list[j] = p_ol_list[j - 1];
                    j--;
                }

                p_ol_list[j] = o_temp;

                l_overallCount++;

                p_del_postProgress?.Invoke(l_overallCount / (double)p_ol_list.Count);
            }

            p_del_postProgress?.Invoke(l_overallCount / (double)p_ol_list.Count);
        }


        /// <summary>
        /// Sort a dynamic list with bubble sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        public static void BubbleSort<T>(List<T?> p_ol_list) where T : IComparable
        {
            Sorts.BubbleSort(p_ol_list, null);
        }

        /// <summary>
        /// Sort a dynamic list with bubble sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        public static void BubbleSort<T>(List<T?> p_ol_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable
        {
            long l_overallCount = 0;

            for (int j = p_ol_list.Count - 1; j >= 0; j--)
            {
                for (int i = 0; i <= j - 1; i++)
                {
                    /* compare */
                    if (p_ol_list[i]?.CompareTo(p_ol_list[i + 1]) > 0)
                    {
                        /* swap */
                        T? o_temp = p_ol_list[i];
                        p_ol_list[i] = p_ol_list[i + 1];
                        p_ol_list[i + 1] = o_temp;
                    }
                }

                l_overallCount++;

                p_del_postProgress?.Invoke(l_overallCount / (double)p_ol_list.Count);
            }

            p_del_postProgress?.Invoke(l_overallCount / (double)p_ol_list.Count);
        }


        /// <summary>
        /// Sort a dynamic list with heap sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        public static void HeapSort<T>(List<T?> p_ol_list) where T : IComparable
        {
            Sorts.HeapSort(p_ol_list, null);
        }

        /// <summary>
        /// Sort a dynamic list with heap sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        public static void HeapSort<T>(List<T?> p_ol_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable
        {
            long l_overallCount = 0;

            /* create help list */
            List<T?> a_heap = [];

            /* initialize help list with null */
            for (int i = 0; i < p_ol_list.Count + 1; i++)
            {
                a_heap.Add(default);
            }

            for (int i = 0; i < p_ol_list.Count; i++)
            {
                /* fill help list */
                a_heap[i + 1] = p_ol_list[i];
                int j = i + 1;

                while (j > 1)
                {
                    /* compare */
                    if (a_heap[j / 2]?.CompareTo(a_heap[j]) < 0)
                    {
                        /* swap */
                        T? o_temp = a_heap[j];
                        a_heap[j] = a_heap[j / 2];
                        a_heap[j / 2] = o_temp;
                        j /= 2;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < p_ol_list.Count; i++)
            {
                /* set max element from help list in list */
                p_ol_list[p_ol_list.Count - 1 - i] = a_heap[1];

                /* delete max element in help list */
                a_heap[1] = a_heap[p_ol_list.Count - i];
                a_heap[p_ol_list.Count - i] = default;
                int j = 1;

                while ((2 * j) <= (p_ol_list.Count - i))
                {
                    int i_maxChild;

                    /* compare */
                    if ((2 * j + 1) <= (p_ol_list.Count - i))
                    {
                        /* compare */
                        if ((a_heap[2 * j + 1] == null) || (a_heap[2 * j]?.CompareTo(a_heap[2 * j + 1]) > 0))
                        {
                            i_maxChild = 2 * j;
                        }
                        else
                        {
                            i_maxChild = 2 * j + 1;
                        }
                    }
                    else
                    {
                        i_maxChild = 2 * j;
                    }

                    /* compare */
                    if ((a_heap[i_maxChild] != null) && (a_heap[j]?.CompareTo(a_heap[i_maxChild]) < 0))
                    {
                        /* swap */
                        T? o_temp = a_heap[j];
                        a_heap[j] = a_heap[i_maxChild];
                        a_heap[i_maxChild] = o_temp;

                        j = i_maxChild;
                    }
                    else
                    {
                        break;
                    }
                }

                l_overallCount++;

                p_del_postProgress?.Invoke(l_overallCount / (double)p_ol_list.Count);
            }

            p_del_postProgress?.Invoke(l_overallCount / (double)p_ol_list.Count);
        }


        /// <summary>
        /// Sort a dynamic list with merge sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        public static void MergeSort<T>(List<T?> p_ol_list) where T : IComparable
        {
            Sorts.MergeSort(p_ol_list, null);
        }

        /// <summary>
        /// Sort a dynamic list with merge sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        public static void MergeSort<T>(List<T?> p_ol_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable
        {
            long l_overallCount = 0;

            MergeSortRecursive(0, p_ol_list.Count - 1, p_ol_list, ref l_overallCount, p_del_postProgress);

            p_del_postProgress?.Invoke(l_overallCount / (double)p_ol_list.Count);
        }

        /// <summary>
        /// Recursive call of the merge sort, only private access within sorts collection
        /// </summary>
        /// <param name="p_i_left">left index(start) of merge part within recursion</param>
        /// <param name="p_i_right">right index(end) of merge part within recursion</param>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        /// <param name="p_l_overallCount">overall count for sort algorithm for progress</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>

        private static void MergeSortRecursive<T>(int p_i_left, int p_i_right, List<T?> p_ol_list, ref long p_l_overallCount, Sorts.PostProgress? p_del_postProgress) where T : IComparable
        {
            /* cancel condition */
            if (p_i_left < p_i_right)
            {
                int i_center = (p_i_left + p_i_right) / 2;
                MergeSortRecursive(p_i_left, i_center, p_ol_list, ref p_l_overallCount, p_del_postProgress);
                MergeSortRecursive(i_center + 1, p_i_right, p_ol_list, ref p_l_overallCount, p_del_postProgress);
                Merge(p_i_left, i_center, p_i_right, p_ol_list, ref p_l_overallCount, p_del_postProgress);
            }
        }

        /// <summary>
        /// Implementation of merge sort which can be used recursively, only private access within sorts collection
        /// </summary>
        /// <param name="p_i_left">left index(start) of merge part within recursion</param>
        /// <param name="p_i_center">center index(middle) of merge part within recursion</param>
        /// <param name="p_i_right">right index(end) of merge part within recursion</param>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        /// <param name="p_l_overallCount">overall count for sort algorithm for progress</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        private static void Merge<T>(int p_i_left, int p_i_center, int p_i_right, List<T?> p_ol_list, ref long p_l_overallCount, Sorts.PostProgress? p_del_postProgress) where T : IComparable
        {
            /* create help list */
            List<T?> a_help = [];

            /* initialize help list with null */
            for (int j = 0; j < (p_i_right - p_i_left + 1); j++)
            {
                a_help.Add(default);
            }

            int i = 0;
            int l = p_i_left;
            int r = p_i_center + 1;

            while ((l <= p_i_center) && (r <= p_i_right))
            {
                /* compare */
                if (p_ol_list[l]?.CompareTo(p_ol_list[r]) <= 0)
                {
                    /* swap */
                    a_help[i] = p_ol_list[l];
                    l++;
                }
                else
                {
                    /* swap */
                    a_help[i] = p_ol_list[r];
                    r++;
                }

                i++;
            }

            if (l > p_i_center)
            {
                for (int j = r; j <= p_i_right; j++)
                {
                    /* swap */
                    a_help[i] = p_ol_list[j];
                    i++;
                }
            }
            else
            {
                for (int j = l; j <= p_i_center; j++)
                {
                    /* swap */
                    a_help[i] = p_ol_list[j];
                    i++;
                }
            }

            for (i = 0; i <= (p_i_right - p_i_left); i++)
            {
                p_ol_list[i + p_i_left] = a_help[i];
            }

            p_l_overallCount++;

            p_del_postProgress?.Invoke(p_l_overallCount / (double)p_ol_list.Count);
        }


        /// <summary>
        /// Sort a dynamic list with quick sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        public static void QuickSort<T>(List<T?> p_ol_list) where T : IComparable
        {
            Sorts.QuickSort(p_ol_list, null);
        }

        /// <summary>
        /// Sort a dynamic list with quick sort
        /// </summary>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        public static void QuickSort<T>(List<T?> p_ol_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable
        {
            long l_overallCount = 0;

            bool b_sortNeeded = false;

            /* check if we have a list which is already sorted, otherwise the recursion will kill our stack */
            for (int i = 0; i < p_ol_list.Count - 1; i++)
            {
                if (p_ol_list[i]?.CompareTo(p_ol_list[i + 1]) > 0)
                {
                    b_sortNeeded = true;
                    break;
                }
            }

            if (b_sortNeeded)
            {
                QuickSortRecursive(0, p_ol_list.Count - 1, p_ol_list, ref l_overallCount, p_del_postProgress);

                p_del_postProgress?.Invoke((l_overallCount / 2) / (double)p_ol_list.Count);
            }
        }

        /// <summary>
        /// Implementation of quick sort which can be used recursively, only private access within sorts collection
        /// </summary>
        /// <param name="p_i_left">left index(start) of quicksort part within recursion</param>
        /// <param name="p_i_right">right index(end) of quicksort part within recursion</param>
        /// <typeparam name="T">type of elements in list</typeparam>
        /// <param name="p_ol_list">dynamic list which will be sorted</param>
        /// <param name="p_l_overallCount">overall count for sort algorithm for progress</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        private static void QuickSortRecursive<T>(int p_i_left, int p_i_right, List<T?> p_ol_list, ref long p_l_overallCount, Sorts.PostProgress? p_del_postProgress) where T : IComparable
        {
            if (p_i_left < p_i_right)
            {
                int i = p_i_left;
                int j = p_i_right - 1;
                T? o_pivot = p_ol_list[p_i_right];

                do
                {
                    /* compare */
                    while ((p_ol_list[i]?.CompareTo(o_pivot) <= 0) && (i < p_i_right))
                    {
                        i++;
                    }

                    /* compare */
                    while ((p_ol_list[j]?.CompareTo(o_pivot) >= 0) && (j > p_i_left))
                    {
                        j--;
                    }

                    if (i < j)
                    {
                        /* swap */
                        T? o_temp = p_ol_list[i];
                        p_ol_list[i] = p_ol_list[j];
                        p_ol_list[j] = o_temp;
                    }
                }
                while (i < j);

                if (p_ol_list[i]?.CompareTo(o_pivot) > 0)
                {
                    /* swap */
                    T? o_temp = p_ol_list[i];
                    p_ol_list[i] = p_ol_list[p_i_right];
                    p_ol_list[p_i_right] = o_temp;
                }

                QuickSortRecursive(p_i_left, i - 1, p_ol_list, ref p_l_overallCount, p_del_postProgress);
                QuickSortRecursive(i + 1, p_i_right, p_ol_list, ref p_l_overallCount, p_del_postProgress);

                p_l_overallCount++;

                p_del_postProgress?.Invoke((p_l_overallCount / 2) / (double)p_ol_list.Count);
            }

            p_l_overallCount++;

            p_del_postProgress?.Invoke((p_l_overallCount / 2) / (double)p_ol_list.Count);
        }

        /* ############################################################################################################ */

        /// <summary>
        /// Sort a dynamic key-value list with selection sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> SelectionSort<T, U>(Dictionary<T, U> p_m_list) where T : IComparable where U : IComparable
        {
            return Sorts.SelectionSort(p_m_list, true);
        }

        /// <summary>
        /// Sort a dynamic key-value list with selection sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>

        public static List<KeyValuePair<T, U>> SelectionSort<T, U>(Dictionary<T, U> p_m_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.SelectionSort(p_m_list, true, p_del_postProgress);
        }

        /// <summary>
        /// Sort a dynamic key-value list with selection sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>

        public static List<KeyValuePair<T, U>> SelectionSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue) where T : IComparable where U : IComparable
        {
            return Sorts.SelectionSort(p_m_list, p_b_sortByValue, null);
        }

        /// <summary>
        /// Sort a dynamic key-value list with selection sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>

        public static List<KeyValuePair<T, U>> SelectionSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.GenericMapSort(p_m_list, p_b_sortByValue, p_del_postProgress, 0);
        }


        /// <summary>
        /// Sort a dynamic key-value list with insertion sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> InsertionSort<T, U>(Dictionary<T, U> p_m_list) where T : IComparable where U : IComparable
        {
            return Sorts.InsertionSort(p_m_list, true);
        }

        /// <summary>
        /// Sort a dynamic key-value list with insertion sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> InsertionSort<T, U>(Dictionary<T, U> p_m_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.InsertionSort(p_m_list, true, p_del_postProgress);
        }

        /// <summary>
        /// Sort a dynamic key-value list with insertion sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> InsertionSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue) where T : IComparable where U : IComparable
        {
            return Sorts.InsertionSort(p_m_list, p_b_sortByValue, null);
        }

        /// <summary>
        /// Sort a dynamic key-value list with insertion sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> InsertionSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.GenericMapSort(p_m_list, p_b_sortByValue, p_del_postProgress, 1);
        }


        /// <summary>
        /// Sort a dynamic key-value list with bubble sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> BubbleSort<T, U>(Dictionary<T, U> p_m_list) where T : IComparable where U : IComparable
        {
            return Sorts.BubbleSort(p_m_list, true);
        }

        /// <summary>
        /// Sort a dynamic key-value list with bubble sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> BubbleSort<T, U>(Dictionary<T, U> p_m_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.BubbleSort(p_m_list, true, p_del_postProgress);
        }

        /// <summary>
        /// Sort a dynamic key-value list with bubble sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> BubbleSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue) where T : IComparable where U : IComparable
        {
            return Sorts.BubbleSort(p_m_list, p_b_sortByValue, null);
        }

        /// <summary>
        /// Sort a dynamic key-value list with bubble sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> BubbleSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.GenericMapSort(p_m_list, p_b_sortByValue, p_del_postProgress, 2);
        }


        /// <summary>
        /// Sort a dynamic key-value list with heap sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> HeapSort<T, U>(Dictionary<T, U> p_m_list) where T : IComparable where U : IComparable
        {
            return Sorts.HeapSort(p_m_list, true);
        }

        /// <summary>
        /// Sort a dynamic key-value list with heap sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> HeapSort<T, U>(Dictionary<T, U> p_m_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.HeapSort(p_m_list, true, p_del_postProgress);
        }

        /// <summary>
        /// Sort a dynamic key-value list with heap sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> HeapSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue) where T : IComparable where U : IComparable
        {
            return Sorts.HeapSort(p_m_list, p_b_sortByValue, null);
        }

        /// <summary>
        /// Sort a dynamic key-value list with heap sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> HeapSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.GenericMapSort(p_m_list, p_b_sortByValue, p_del_postProgress, 3);
        }


        /// <summary>
        /// Sort a dynamic key-value list with merge sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> MergeSort<T, U>(Dictionary<T, U> p_m_list) where T : IComparable where U : IComparable
        {
            return Sorts.MergeSort(p_m_list, true);
        }

        /// <summary>
        /// Sort a dynamic key-value list with merge sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> MergeSort<T, U>(Dictionary<T, U> p_m_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.MergeSort(p_m_list, true, p_del_postProgress);
        }

        /// <summary>
        /// Sort a dynamic key-value list with merge sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> MergeSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue) where T : IComparable where U : IComparable
        {
            return Sorts.MergeSort(p_m_list, p_b_sortByValue, null);
        }

        /// <summary>
        /// Sort a dynamic key-value list with merge sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> MergeSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.GenericMapSort(p_m_list, p_b_sortByValue, p_del_postProgress, 4);
        }


        /// <summary>
        /// Sort a dynamic key-value list with quick sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> QuickSort<T, U>(Dictionary<T, U> p_m_list) where T : IComparable where U : IComparable
        {
            return Sorts.QuickSort(p_m_list, true);
        }

        /// <summary>
        /// Sort a dynamic key-value list with quick sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> QuickSort<T, U>(Dictionary<T, U> p_m_list, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.QuickSort(p_m_list, true, p_del_postProgress);
        }

        /// <summary>
        /// Sort a dynamic key-value list with quick sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> QuickSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue) where T : IComparable where U : IComparable
        {
            return Sorts.QuickSort(p_m_list, p_b_sortByValue, null);
        }

        /// <summary>
        /// Sort a dynamic key-value list with quick sort, sort by value
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        public static List<KeyValuePair<T, U>> QuickSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue, Sorts.PostProgress? p_del_postProgress) where T : IComparable where U : IComparable
        {
            return Sorts.GenericMapSort(p_m_list, p_b_sortByValue, p_del_postProgress, 5);
        }


        /// <summary>
        /// Implementation of all sort methods for dynamic key-value lists, only private access within sorts collection
        /// </summary>
        /// <typeparam name="T">type of key elements in key-value list</typeparam>
        /// <typeparam name="U">type of value elements in key-value list</typeparam>
        /// <param name="p_m_list">dynamic key-value list which will be sorted</param>
        /// <param name="p_b_sortByValue">true - sort key-value list by value, false - sort key-value list by key</param>
        /// <param name="p_del_postProgress">delegate object with implemented method to post sort progress</param>
        /// <param name="p_i_sortAlgorithm">0 - selection, 1 - insertion, 2 - bubble, 3 - heap, 4 - merge, 5 - quick</param>
        /// <returns>list of map entries - List&lt; KeyValuePair&lt;T, U&gt; &gt;</returns>
        /// <exception cref="InvalidOperationException">thrown if a temporary help list has not the same amount of elements, a key-value entry could not be found or the sorted result has not the same amount of elements as the parameter key-value list</exception>
        private static List<KeyValuePair<T, U>> GenericMapSort<T, U>(Dictionary<T, U> p_m_list, bool p_b_sortByValue, Sorts.PostProgress? p_del_postProgress, int p_i_sortAlgorithm) where T : IComparable where U : IComparable
        {
            /* check for valid sort algorithm parameter */
            if ((p_i_sortAlgorithm < 0) || (p_i_sortAlgorithm > 5))
            {
                throw new InvalidOperationException("Parameter for sort algorithm[" + p_i_sortAlgorithm + "] is not between[0..5]");
            }

            /* return value */
            List<KeyValuePair<T, U>> m_return = [];

            if (p_b_sortByValue)
            { /* sort by value */
                /* put values is hash set to determine duplicates */
                HashSet<U> a_valuesSet = [.. p_m_list.Values];
                bool b_duplicates = false;

                /* check if dynamics key-value list as parameter contains duplicates */
                if (a_valuesSet.Count != p_m_list.Values.Count)
                {
                    b_duplicates = true;
                }

                if (b_duplicates)
                { /* handle duplicates */
                    List<U?> a_tempList = [];
                    Dictionary<U, int> m_duplicateMap = [];
                    Dictionary<string, T> m_tempMap = [];

                    foreach (KeyValuePair<T, U> o_entry in p_m_list)
                    {
                        /* add value to temporary list which will be sorted later */
                        a_tempList.Add(o_entry.Value);

                        if (m_duplicateMap.TryGetValue(o_entry.Value, out int i_duplicateMapValue))
                        {
                            /* duplicate found, increment amount value in duplicate hash map */
                            m_duplicateMap[o_entry.Value] = (i_duplicateMapValue + 1);
                        }
                        else
                        {
                            /* new value, put value into duplicate hash map with amount 1 */
                            m_duplicateMap.Add(o_entry.Value, 1);
                        }

                        /* put value with amount and key in temporary map - is used for reconstruction of sorted values and return key-value list */
                        m_tempMap.Add(o_entry.Value.ToString() + "__sep__" + m_duplicateMap[o_entry.Value], o_entry.Key);
                    }

                    /* check if temporary map has same amount of elements as the given unsorted key-value list */
                    if (m_tempMap.Count != p_m_list.Count)
                    {
                        throw new InvalidOperationException("Temp map size[" + m_tempMap.Count + "] is not equal parameter map size[" + p_m_list.Count + "]");
                    }

                    /* execution sort algorithm with temporary list */
                    if (p_i_sortAlgorithm == 0)
                    {
                        Sorts.SelectionSort(a_tempList, p_del_postProgress);
                    }
                    else if (p_i_sortAlgorithm == 1)
                    {
                        Sorts.InsertionSort(a_tempList, p_del_postProgress);
                    }
                    else if (p_i_sortAlgorithm == 2)
                    {
                        Sorts.BubbleSort(a_tempList, p_del_postProgress);
                    }
                    else if (p_i_sortAlgorithm == 3)
                    {
                        Sorts.HeapSort(a_tempList, p_del_postProgress);
                    }
                    else if (p_i_sortAlgorithm == 4)
                    {
                        Sorts.MergeSort(a_tempList, p_del_postProgress);
                    }
                    else if (p_i_sortAlgorithm == 5)
                    {
                        Sorts.QuickSort(a_tempList, p_del_postProgress);
                    }

                    long l_overallCount = 0;

                    /* reconstruction of sorted values and return key-value list */
                    for (int i = 0; i < a_tempList.Count; i++)
                    {
                        for (int j = 1; j <= p_m_list.Count; j++)
                        {
                            /* look for key in temporary map with sorted temporary list values */
                            if (m_tempMap.ContainsKey(a_tempList[i] + "__sep__" + j))
                            {
                                /* get key */
                                T foo = m_tempMap[a_tempList[i] + "__sep__" + j];
                                /* get value */
                                U bar = p_m_list[foo];

                                /* add key value entry to return list */
                                m_return.Add(new KeyValuePair<T, U>(foo, bar));

                                /* remove found key, so an exception will occur if it will be searched again */
                                m_tempMap.Remove(a_tempList[i] + "__sep__" + j);
                                break;
                            }

                            /* could not find key in temporary map */
                            if (j == p_m_list.Count)
                            {
                                throw new InvalidOperationException("Could not find temp map value[" + a_tempList[i] + "__sep__integer" + j + "]");
                            }
                        }

                        l_overallCount++;

                        /* post reconstruction progress */
                        p_del_postProgress?.Invoke(l_overallCount / (double)p_m_list.Count);
                    }
                }
                else
                { /* no duplicates */
                    List<U?> a_tempList = [];
                    Dictionary<U, T?> m_tempMap = [];

                    /* fill temporary list with values and temporary map with values and keys */
                    foreach (KeyValuePair<T, U> o_entry in p_m_list)
                    {
                        a_tempList.Add(o_entry.Value);
                        m_tempMap.Add(o_entry.Value, o_entry.Key);
                    }

                    /* check if temporary map has same amount of elements as the given unsorted key-value list */
                    if (m_tempMap.Count != p_m_list.Count)
                    {
                        throw new InvalidOperationException("Temp map size[" + m_tempMap.Count + "] is not equal parameter map size[" + p_m_list.Count + "]");
                    }

                    /* execution sort algorithm with temporary list */
                    if (p_i_sortAlgorithm == 0)
                    {
                        Sorts.SelectionSort(a_tempList, p_del_postProgress);
                    }
                    else if (p_i_sortAlgorithm == 1)
                    {
                        Sorts.InsertionSort(a_tempList, p_del_postProgress);
                    }
                    else if (p_i_sortAlgorithm == 2)
                    {
                        Sorts.BubbleSort(a_tempList, p_del_postProgress);
                    }
                    else if (p_i_sortAlgorithm == 3)
                    {
                        Sorts.HeapSort(a_tempList, p_del_postProgress);
                    }
                    else if (p_i_sortAlgorithm == 4)
                    {
                        Sorts.MergeSort(a_tempList, p_del_postProgress);
                    }
                    else if (p_i_sortAlgorithm == 5)
                    {
                        Sorts.QuickSort(a_tempList, p_del_postProgress);
                    }

                    long l_overallCount = 0;

                    for (int i = 0; i < a_tempList.Count; i++)
                    {
                        /* get key value */
                        U? o_key = a_tempList[i] ?? throw new InvalidOperationException("Could not use key with value[null] for temporary map");

                        /* check if key exists in temporary map */
                        if (!m_tempMap.TryGetValue(o_key, out T? value))
                        {
                            throw new InvalidOperationException("Could not find key value[" + a_tempList[i] + "] in temporary map");
                        }

                        /* get key from temporary map by value of sorted temporary list */
                        T foo = value ?? throw new InvalidOperationException("Could not use key with value[null] from temporary map");

                        /* add key value entry to return list */
                        m_return.Add(new KeyValuePair<T, U>(foo, o_key));

                        /* remove key from temporary map */
                        m_tempMap.Remove(o_key);

                        l_overallCount++;

                        /* post reconstruction progress */
                        p_del_postProgress?.Invoke(l_overallCount / (double)p_m_list.Count);
                    }
                }
            }
            else
            {  /* sort by key */
                List<T?> a_tempList = [];
                Dictionary<T, U> m_tempMap = [];

                /* fill temporary list with values and temporary map with values and keys */
                foreach (KeyValuePair<T, U> o_entry in p_m_list)
                {
                    a_tempList.Add(o_entry.Key);
                    m_tempMap.Add(o_entry.Key, o_entry.Value);
                }

                /* check if temporary map has same amount of elements as the given unsorted key-value list */
                if (m_tempMap.Count != p_m_list.Count)
                {
                    throw new InvalidOperationException("Temp map size[" + m_tempMap.Count + "] is not equal parameter map size[" + p_m_list.Count + "]");
                }

                /* execution sort algorithm with temporary list */
                if (p_i_sortAlgorithm == 0)
                {
                    Sorts.SelectionSort(a_tempList, p_del_postProgress);
                }
                else if (p_i_sortAlgorithm == 1)
                {
                    Sorts.InsertionSort(a_tempList, p_del_postProgress);
                }
                else if (p_i_sortAlgorithm == 2)
                {
                    Sorts.BubbleSort(a_tempList, p_del_postProgress);
                }
                else if (p_i_sortAlgorithm == 3)
                {
                    Sorts.HeapSort(a_tempList, p_del_postProgress);
                }
                else if (p_i_sortAlgorithm == 4)
                {
                    Sorts.MergeSort(a_tempList, p_del_postProgress);
                }
                else if (p_i_sortAlgorithm == 5)
                {
                    Sorts.QuickSort(a_tempList, p_del_postProgress);
                }

                long l_overallCount = 0;

                for (int i = 0; i < a_tempList.Count; i++)
                {
                    /* get key value */
                    T? o_key = a_tempList[i] ?? throw new InvalidOperationException("Could not use key with value[null] for temporary map");

                    /* check if key exists in temporary map */
                    if (!m_tempMap.TryGetValue(o_key, out U? foo))
                    {
                        throw new InvalidOperationException("Could not find key value[" + o_key + "] in temporary map");
                    }

                    /* add key value entry to return list */
                    m_return.Add(new KeyValuePair<T, U>(o_key, foo ?? throw new InvalidOperationException("Could not use key with value[null] from temporary map")));

                    /* remove key from temporary map */
                    m_tempMap.Remove(o_key);

                    l_overallCount++;

                    /* post reconstruction progress */
                    p_del_postProgress?.Invoke(l_overallCount / (double)p_m_list.Count);
                }
            }

            /* last check if sorted result list has not the same amount of elements as the given unsorted key-value list */
            if (m_return.Count != p_m_list.Count)
            {
                throw new InvalidOperationException("Result list size[" + m_return.Count + "] is not equal parameter map size[" + p_m_list.Count + "]");
            }

            return m_return;
        }
    }
}
