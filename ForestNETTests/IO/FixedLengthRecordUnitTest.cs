using ForestNETLib.IO;

namespace ForestNETTests.IO
{
    public class FixedLengthRecordUnitTest
    {
        [Test]
        public void TestFixedLengthRecord()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_resourcesFLRDirectory = Environment.CurrentDirectory + ForestNETLib.IO.File.DIR + "Resources" + ForestNETLib.IO.File.DIR + "flr" + ForestNETLib.IO.File.DIR;
                string s_testDirectory = s_currentDirectory + ForestNETLib.IO.File.DIR + "testFLR" + ForestNETLib.IO.File.DIR;

                if (ForestNETLib.IO.File.FolderExists(s_testDirectory))
                {
                    ForestNETLib.IO.File.DeleteDirectory(s_testDirectory);
                }

                ForestNETLib.IO.File.CreateDirectory(s_testDirectory);
                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_testDirectory),
                    Is.True,
                    "directory[" + s_testDirectory + "] does not exist"
                );

                FLRReadWithoutGroups(s_resourcesFLRDirectory + "TestFLRWithoutGroups.txt");
                FLRReadWithGroupHeader(s_resourcesFLRDirectory + "TestFLRWithGroupHeader.txt");
                FLRReadWithGroupFooter(s_resourcesFLRDirectory + "TestFLRWithGroupFooter.txt");
                FLRReadWithGroupHeaderAndFooter(s_resourcesFLRDirectory + "TestFLRWithGroupHeaderAndGroupFooter.txt");
                FLRReadEverything(s_resourcesFLRDirectory + "TestFLREverything.txt");
                FLRReadEverythingWithSubtypes(s_resourcesFLRDirectory + "TestFLREverythingWithSubtypes.txt");

                FLRWriteTests(s_testDirectory + "TestWriteFLRWithoutGroups.txt", s_resourcesFLRDirectory + "TestFLRWithoutGroups.txt", 0);
                FLRWriteTests(s_testDirectory + "TestWriteFLRWithGroupHeader.txt", s_resourcesFLRDirectory + "TestFLRWithGroupHeader.txt", 1);
                FLRWriteTests(s_testDirectory + "TestWriteFLRWithGroupFooter.txt", s_resourcesFLRDirectory + "TestFLRWithGroupFooter.txt", 2);
                FLRWriteTests(s_testDirectory + "TestWriteFLRWithGroupHeaderAndGroupFooter.txt", s_resourcesFLRDirectory + "TestFLRWithGroupHeaderAndGroupFooter.txt", 3);
                FLRWriteTests(s_testDirectory + "TestWriteFLREverything.txt", s_resourcesFLRDirectory + "TestFLREverything.txt", 4);
                FLRWriteTests(s_testDirectory + "TestFLREverythingWithSubtypes.txt", s_resourcesFLRDirectory + "TestFLREverythingWithSubtypes.txt", 5);

                ForestNETLib.IO.File.DeleteDirectory(s_testDirectory);
                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_testDirectory),
                    Is.False,
                    "directory[" + s_testDirectory + "] does exist"
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static void FLRReadWithoutGroups(string p_s_flrFileName)
        {
            /* test with flr regex */
            FixedLengthRecordData o_flrData = new();
            FixedLengthRecordFile o_flrFile = new(o_flrData, "^000.*$");
            o_flrFile.ReadFile(p_s_flrFileName);

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                int i = 0;

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    FixedLengthRecordData o_record = (FixedLengthRecordData)o_foo.Value;
                    CompareRecords(i++, o_record);
                }
            }

            /* test with flr known length */
            o_flrFile = new(o_flrData, null, 320);
            o_flrFile.ReadFile(p_s_flrFileName);

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                int i = 0;

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    FixedLengthRecordData o_record = (FixedLengthRecordData)o_foo.Value;
                    CompareRecords(i++, o_record);
                }
            }
        }

        private static void FLRReadWithGroupHeader(string p_s_flrFileName)
        {
            /* test with flr regex */
            FixedLengthRecordData o_flrData = new();
            FixedLengthRecordGroupHeaderData o_groupHeaderData = new();
            FixedLengthRecordFile o_flrFile = new(o_flrData, "^000.*$")
            {
                GroupHeader = new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_groupHeaderData, "^\\+H\\+.*$")
            };
            o_flrFile.ReadFile(p_s_flrFileName);

            int i = 0;
            int j = 0;

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                FixedLengthRecordGroupHeaderData o_groupHeader = (o_stack.Value.GroupHeader as FixedLengthRecordGroupHeaderData) ?? throw new Exception("Group header in stack #" + (i + 1) + " is null");
                CompareGroupHeaders(i++, o_groupHeader);

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    FixedLengthRecordData o_record = (FixedLengthRecordData)o_foo.Value;
                    CompareRecords(j++, o_record);
                }
            }

            /* test with flr known length */
            o_flrFile = new(o_flrData, null, 320)
            {
                GroupHeader = new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_groupHeaderData, null, 106)
            };
            o_flrFile.ReadFile(p_s_flrFileName);

            i = 0;
            j = 0;

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                FixedLengthRecordGroupHeaderData o_groupHeader = (o_stack.Value.GroupHeader as FixedLengthRecordGroupHeaderData) ?? throw new Exception("Group header in stack #" + (i + 1) + " is null");
                CompareGroupHeaders(i++, o_groupHeader);

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    FixedLengthRecordData o_record = (FixedLengthRecordData)o_foo.Value;
                    CompareRecords(j++, o_record);
                }
            }
        }

        private static void FLRReadWithGroupFooter(string p_s_flrFileName)
        {
            /* test with flr regex */
            FixedLengthRecordData o_flrData = new();
            FixedLengthRecordGroupFooterData o_groupFooterData = new();
            FixedLengthRecordFile o_flrFile = new(o_flrData, "^000.*$")
            {
                GroupFooter = new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_groupFooterData, "^\\+F\\+.*$")
            };
            o_flrFile.ReadFile(p_s_flrFileName);

            int j = 0;
            int k = 0;

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                int i_sumInt = 0;

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    FixedLengthRecordData o_record = (FixedLengthRecordData)o_foo.Value;
                    CompareRecords(j++, o_record);

                    i_sumInt += o_record.FieldInt / 2;
                }

                FixedLengthRecordGroupFooterData o_groupFooter = o_stack.Value.GroupFooter as FixedLengthRecordGroupFooterData ?? throw new Exception("Group footer in stack #" + (k + 1) + " is null");
                CompareGroupFooters(k++, o_groupFooter);

                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(i_sumInt), "sum over the field Int has unexpected value for stack #" + (k + 1) + ": '" + i_sumInt + "' != '" + o_groupFooter.FieldSumInt + "'");
            }

            /* test with flr known length */
            o_flrFile = new(o_flrData, null, 320)
            {
                GroupFooter = new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_groupFooterData, null, 81)
            };
            o_flrFile.ReadFile(p_s_flrFileName);

            j = 0;
            k = 0;

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                int i_sumInt = 0;

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    FixedLengthRecordData o_record = (FixedLengthRecordData)o_foo.Value;
                    CompareRecords(j++, o_record);

                    i_sumInt += o_record.FieldInt / 2;
                }

                FixedLengthRecordGroupFooterData o_groupFooter = o_stack.Value.GroupFooter as FixedLengthRecordGroupFooterData ?? throw new Exception("Group footer in stack #" + (j + 1) + " is null");
                CompareGroupFooters(k++, o_groupFooter);

                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(i_sumInt), "sum over the field Int has unexpected value for stack #" + (k + 1) + ": '" + i_sumInt + "' != '" + o_groupFooter.FieldSumInt + "'");
            }
        }

        private static void FLRReadWithGroupHeaderAndFooter(string p_s_flrFileName)
        {
            /* test with flr regex */
            FixedLengthRecordData o_flrData = new();
            FixedLengthRecordGroupHeaderData o_groupHeaderData = new();
            FixedLengthRecordGroupFooterData o_groupFooterData = new();
            FixedLengthRecordFile o_flrFile = new(o_flrData, "^000.*$")
            {
                GroupHeader = new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_groupHeaderData, "^\\+H\\+.*$"),
                GroupFooter = new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_groupFooterData, "^\\+F\\+.*$")
            };
            o_flrFile.ReadFile(p_s_flrFileName);

            int i = 0;
            int j = 0;
            int k = 0;

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                int i_sumInt = 0;

                FixedLengthRecordGroupHeaderData o_groupHeader = o_stack.Value.GroupHeader as FixedLengthRecordGroupHeaderData ?? throw new Exception("Group header in stack #" + (i + 1) + " is null");
                CompareGroupHeaders(i++, o_groupHeader);

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    FixedLengthRecordData o_record = (FixedLengthRecordData)o_foo.Value;
                    CompareRecords(j++, o_record);

                    i_sumInt += o_record.FieldInt / 2;
                }

                FixedLengthRecordGroupFooterData o_groupFooter = o_stack.Value.GroupFooter as FixedLengthRecordGroupFooterData ?? throw new Exception("Group footer in stack #" + (k + 1) + " is null");
                CompareGroupFooters(k++, o_groupFooter);

                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(i_sumInt), "sum over the field Int has unexpected value for stack #" + (i + 1) + ": '" + i_sumInt + "' != '" + o_groupFooter.FieldSumInt + "'");
            }

            /* test with flr known length */
            o_flrFile = new(o_flrData, null, 320)
            {
                GroupHeader = new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_groupHeaderData, null, 106),
                GroupFooter = new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_groupFooterData, null, 81)
            };
            o_flrFile.ReadFile(p_s_flrFileName);

            i = 0;
            j = 0;
            k = 0;

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                int i_sumInt = 0;

                FixedLengthRecordGroupHeaderData o_groupHeader = o_stack.Value.GroupHeader as FixedLengthRecordGroupHeaderData ?? throw new Exception("Group header in stack #" + (i + 1) + " is null");
                CompareGroupHeaders(i++, o_groupHeader);

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    FixedLengthRecordData o_record = (FixedLengthRecordData)o_foo.Value;
                    CompareRecords(j++, o_record);

                    i_sumInt += o_record.FieldInt / 2;
                }

                FixedLengthRecordGroupFooterData o_groupFooter = o_stack.Value.GroupFooter as FixedLengthRecordGroupFooterData ?? throw new Exception("Group footer in stack #" + (k + 1) + " is null");
                CompareGroupFooters(k++, o_groupFooter);

                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(i_sumInt), "sum over the field Int has unexpected value for stack #" + (i + 1) + ": '" + i_sumInt + "' != '" + o_groupFooter.FieldSumInt + "'");
            }
        }

        private static void FLRReadEverything(string p_s_flrFileName)
        {
            /* test with flr regex */
            FixedLengthRecordData o_flrData = new();
            FixedLengthRecordOtherData o_flrOtherData = new();
            FixedLengthRecordAnotherData o_flrAnotherData = new();
            FixedLengthRecordGroupHeaderData o_groupHeaderData = new();
            FixedLengthRecordGroupFooterData o_groupFooterData = new();
            FixedLengthRecordFile o_flrFile = new(o_flrData, "^000.*$", o_groupHeaderData, "^\\+H\\+.*$", o_groupFooterData, "^\\+F\\+.*$");
            o_flrFile.AddFLRType(new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_flrOtherData, "^100.*$"));
            o_flrFile.AddFLRType(new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_flrAnotherData, "^200.*$"));
            o_flrFile.ReadFile(p_s_flrFileName);

            int i = 0;
            int j = 0;
            int k = 0;

            int l = 0;
            int m = 0;

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                int i_sumInt = 0;

                FixedLengthRecordGroupHeaderData o_groupHeader = o_stack.Value.GroupHeader as FixedLengthRecordGroupHeaderData ?? throw new Exception("Group header in stack #" + (i + 1) + " is null");
                CompareGroupHeaders(i++, o_groupHeader);

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    if (o_foo.Value is FixedLengthRecordData o_record)
                    {
                        CompareRecords(j++, o_record);
                        i_sumInt += o_record.FieldInt / 2;
                    }
                    else if (o_foo.Value is FixedLengthRecordOtherData o_recordOther)
                    {
                        CompareOtherRecords(l++, o_recordOther);
                        i_sumInt += o_recordOther.FieldInt / 2;
                    }
                    else if (o_foo.Value is FixedLengthRecordAnotherData o_recordAnother)
                    {
                        CompareAnotherRecords(m++, o_recordAnother);
                        i_sumInt += o_recordAnother.FieldInt / 2;
                    }
                }

                FixedLengthRecordGroupFooterData o_groupFooter = o_stack.Value.GroupFooter as FixedLengthRecordGroupFooterData ?? throw new Exception("Group footer in stack #" + (k + 1) + " is null");
                CompareGroupFootersEverything(k++, o_groupFooter);

                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(i_sumInt), "sum over the field Int has unexpected value for stack #" + (i + 1) + ": '" + i_sumInt + "' != '" + o_groupFooter.FieldSumInt + "'");
            }

            /* test with flr known length */
            o_flrFile = new(o_flrData, null, 320, o_groupHeaderData, null, 106, o_groupFooterData, null, 81);
            o_flrFile.AddFLRType(new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_flrOtherData, null, 58));
            o_flrFile.AddFLRType(new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_flrAnotherData, null, 72));
            o_flrFile.ReadFile(p_s_flrFileName);

            i = 0;
            j = 0;
            k = 0;

            l = 0;
            m = 0;

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                int i_sumInt = 0;

                FixedLengthRecordGroupHeaderData o_groupHeader = o_stack.Value.GroupHeader as FixedLengthRecordGroupHeaderData ?? throw new Exception("Group header in stack #" + (i + 1) + " is null");
                CompareGroupHeaders(i++, o_groupHeader);

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    if (o_foo.Value is FixedLengthRecordData o_record)
                    {
                        CompareRecords(j++, o_record);
                        i_sumInt += o_record.FieldInt / 2;
                    }
                    else if (o_foo.Value is FixedLengthRecordOtherData o_recordOther)
                    {
                        CompareOtherRecords(l++, o_recordOther);
                        i_sumInt += o_recordOther.FieldInt / 2;
                    }
                    else if (o_foo.Value is FixedLengthRecordAnotherData o_recordAnother)
                    {
                        CompareAnotherRecords(m++, o_recordAnother);
                        i_sumInt += o_recordAnother.FieldInt / 2;
                    }
                }

                FixedLengthRecordGroupFooterData o_groupFooter = o_stack.Value.GroupFooter as FixedLengthRecordGroupFooterData ?? throw new Exception("Group footer in stack #" + (k + 1) + " is null");
                CompareGroupFootersEverything(k++, o_groupFooter);

                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(i_sumInt), "sum over the field Int has unexpected value for stack #" + (i + 1) + ": '" + i_sumInt + "' != '" + o_groupFooter.FieldSumInt + "'");
            }
        }

        private static void FLRReadEverythingWithSubtypes(string p_s_flrFileName)
        {
            /* test with flr regex */
            FixedLengthRecordData o_flrData = new();
            FixedLengthRecordOtherData o_flrOtherData = new();
            FixedLengthRecordAnotherData o_flrAnotherData = new();
            FixedLengthRecordDataWithSubtypes o_flrDataWithSubtypes = new();
            FixedLengthRecordGroupHeaderData o_groupHeaderData = new();
            FixedLengthRecordGroupFooterData o_groupFooterData = new();
            FixedLengthRecordFile o_flrFile = new(o_flrData, "^000.*$", o_groupHeaderData, "^\\+H\\+.*$", o_groupFooterData, "^\\+F\\+.*$");
            o_flrFile.AddFLRType(new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_flrOtherData, "^100.*$"));
            o_flrFile.AddFLRType(new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_flrAnotherData, "^200.*$"));
            o_flrFile.AddFLRType(new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_flrDataWithSubtypes, "^300.*$"));
            o_flrFile.ReadFile(p_s_flrFileName);

            int i = 0;
            int j = 0;
            int k = 0;

            int l = 0;
            int m = 0;
            int n = 0;

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                int i_sumInt = 0;

                FixedLengthRecordGroupHeaderData o_groupHeader = o_stack.Value.GroupHeader as FixedLengthRecordGroupHeaderData ?? throw new Exception("Group header in stack #" + (i + 1) + " is null");
                CompareGroupHeaders(i++, o_groupHeader);

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    if (o_foo.Value is FixedLengthRecordData o_record)
                    {
                        CompareRecords(j++, o_record);
                        i_sumInt += o_record.FieldInt / 2;
                    }
                    else if (o_foo.Value is FixedLengthRecordOtherData o_recordOther)
                    {
                        CompareOtherRecords(l++, o_recordOther);
                        i_sumInt += o_recordOther.FieldInt / 2;
                    }
                    else if (o_foo.Value is FixedLengthRecordAnotherData o_recordAnother)
                    {
                        CompareAnotherRecords(m++, o_recordAnother);
                        i_sumInt += o_recordAnother.FieldInt / 2;
                    }
                    else if (o_foo.Value is FixedLengthRecordDataWithSubtypes o_recordWithSubtypes)
                    {
                        CompareRecordsWithSubtypes(n++, o_recordWithSubtypes);
                    }
                }

                FixedLengthRecordGroupFooterData o_groupFooter = o_stack.Value.GroupFooter as FixedLengthRecordGroupFooterData ?? throw new Exception("Group footer in stack #" + (k + 1) + " is null");
                CompareGroupFootersEverythingWithSubtypes(k++, o_groupFooter);

                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(i_sumInt), "sum over the field Int has unexpected value for stack #" + (i + 1) + ": '" + i_sumInt + "' != '" + o_groupFooter.FieldSumInt + "'");
            }

            /* test with flr known length */
            o_flrFile = new(o_flrData, null, 320, o_groupHeaderData, null, 106, o_groupFooterData, null, 81);
            o_flrFile.AddFLRType(new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_flrOtherData, null, 58));
            o_flrFile.AddFLRType(new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_flrAnotherData, null, 72));
            o_flrFile.AddFLRType(new ForestNETLib.IO.FixedLengthRecordFile.FLRType(o_flrDataWithSubtypes, null, 217));
            o_flrFile.ReadFile(p_s_flrFileName);

            i = 0;
            j = 0;
            k = 0;

            l = 0;
            m = 0;
            n = 0;

            foreach (KeyValuePair<int, FixedLengthRecordFile.FixedLengthRecordStack> o_stack in o_flrFile.Stacks)
            {
                int i_sumInt = 0;

                FixedLengthRecordGroupHeaderData o_groupHeader = o_stack.Value.GroupHeader as FixedLengthRecordGroupHeaderData ?? throw new Exception("Group header in stack #" + (i + 1) + " is null");
                CompareGroupHeaders(i++, o_groupHeader);

                foreach (KeyValuePair<int, IFixedLengthRecord> o_foo in o_stack.Value.FLRs)
                {
                    if (o_foo.Value is FixedLengthRecordData o_record)
                    {
                        CompareRecords(j++, o_record);
                        i_sumInt += o_record.FieldInt / 2;
                    }
                    else if (o_foo.Value is FixedLengthRecordOtherData o_recordOther)
                    {
                        CompareOtherRecords(l++, o_recordOther);
                        i_sumInt += o_recordOther.FieldInt / 2;
                    }
                    else if (o_foo.Value is FixedLengthRecordAnotherData o_recordAnother)
                    {
                        CompareAnotherRecords(m++, o_recordAnother);
                        i_sumInt += o_recordAnother.FieldInt / 2;
                    }
                    else if (o_foo.Value is FixedLengthRecordDataWithSubtypes o_recordWithSubtypes)
                    {
                        CompareRecordsWithSubtypes(n++, o_recordWithSubtypes);
                    }
                }

                FixedLengthRecordGroupFooterData o_groupFooter = o_stack.Value.GroupFooter as FixedLengthRecordGroupFooterData ?? throw new Exception("Group footer in stack #" + (k + 1) + " is null");
                CompareGroupFootersEverythingWithSubtypes(k++, o_groupFooter);

                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(i_sumInt), "sum over the field Int has unexpected value for stack #" + (i + 1) + ": '" + i_sumInt + "' != '" + o_groupFooter.FieldSumInt + "'");
            }
        }

        private static void CompareGroupHeaders(int p_i_groupHeader, FixedLengthRecordGroupHeaderData o_groupHeader)
        {
            if (p_i_groupHeader == 0)
            {
                Assert.That(o_groupHeader.FieldCustomerNumber, Is.EqualTo(123), "values for field 'CustomerNumber' are not equal");
                Assert.That(o_groupHeader.FieldDate, Is.EqualTo(new System.DateTime(2011, 1, 1)), "values for field 'Date' are not equal");
                Assert.That(o_groupHeader.FieldDoubleWithSeparator, Is.EqualTo(314598.443589d), "values for field 'DoubleWithSeparator' are not equal");
            }
            else if (p_i_groupHeader == 1)
            {
                Assert.That(o_groupHeader.FieldCustomerNumber, Is.EqualTo(321), "values for field 'CustomerNumber' are not equal");
                Assert.That(o_groupHeader.FieldDate, Is.EqualTo(new System.DateTime(2022, 4, 2)), "values for field 'Date' are not equal");
                Assert.That(o_groupHeader.FieldDoubleWithSeparator, Is.EqualTo(157783.965224d), "values for field 'DoubleWithSeparator' are not equal");
            }
            else if (p_i_groupHeader == 2)
            {
                Assert.That(o_groupHeader.FieldCustomerNumber, Is.EqualTo(132), "values for field 'CustomerNumber' are not equal");
                Assert.That(o_groupHeader.FieldDate, Is.EqualTo(new System.DateTime(2033, 6, 3)), "values for field 'Date' are not equal");
                Assert.That(o_groupHeader.FieldDoubleWithSeparator, Is.EqualTo(453665.357896d), "values for field 'DoubleWithSeparator' are not equal");
            }
        }

        private static void CompareGroupFooters(int p_i_groupFooter, FixedLengthRecordGroupFooterData o_groupFooter)
        {
            if (p_i_groupFooter == 0)
            {
                Assert.That(o_groupFooter.FieldAmountRecords, Is.EqualTo(3), "values for field 'AmountRecords' are not equal");
                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(1200061721), "values for field 'SumInt' are not equal");
            }
            else if (p_i_groupFooter == 1)
            {
                Assert.That(o_groupFooter.FieldAmountRecords, Is.EqualTo(3), "values for field 'AmountRecords' are not equal");
                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(766660500), "values for field 'SumInt' are not equal");
            }
            else if (p_i_groupFooter == 2)
            {
                Assert.That(o_groupFooter.FieldAmountRecords, Is.EqualTo(2), "values for field 'AmountRecords' are not equal");
                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(260606060), "values for field 'SumInt' are not equal");
            }
        }

        private static void CompareGroupFootersEverything(int p_i_groupFooter, FixedLengthRecordGroupFooterData o_groupFooter)
        {
            if (p_i_groupFooter == 0)
            {
                Assert.That(o_groupFooter.FieldAmountRecords, Is.EqualTo(6), "values for field 'AmountRecords' are not equal");
                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(1200132092), "values for field 'SumInt' are not equal");
            }
            else if (p_i_groupFooter == 1)
            {
                Assert.That(o_groupFooter.FieldAmountRecords, Is.EqualTo(6), "values for field 'AmountRecords' are not equal");
                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(766737533), "values for field 'SumInt' are not equal");
            }
            else if (p_i_groupFooter == 2)
            {
                Assert.That(o_groupFooter.FieldAmountRecords, Is.EqualTo(8), "values for field 'AmountRecords' are not equal");
                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(260642429), "values for field 'SumInt' are not equal");
            }
        }

        private static void CompareGroupFootersEverythingWithSubtypes(int p_i_groupFooter, FixedLengthRecordGroupFooterData o_groupFooter)
        {
            if (p_i_groupFooter == 0)
            {
                Assert.That(o_groupFooter.FieldAmountRecords, Is.EqualTo(7), "values for field 'AmountRecords' are not equal");
                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(1200132092), "values for field 'SumInt' are not equal");
            }
            else if (p_i_groupFooter == 1)
            {
                Assert.That(o_groupFooter.FieldAmountRecords, Is.EqualTo(7), "values for field 'AmountRecords' are not equal");
                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(766737533), "values for field 'SumInt' are not equal");
            }
            else if (p_i_groupFooter == 2)
            {
                Assert.That(o_groupFooter.FieldAmountRecords, Is.EqualTo(10), "values for field 'AmountRecords' are not equal");
                Assert.That(o_groupFooter.FieldSumInt, Is.EqualTo(260642429), "values for field 'SumInt' are not equal");
            }
        }

        private static void CompareRecords(int p_i_record, FixedLengthRecordData o_record)
        {
            if (p_i_record == 0)
            {
                Assert.That(o_record.FieldId, Is.EqualTo(1), "values for field 'Id' are not equal");
                Assert.That(o_record.FieldUUID, Is.EqualTo("9d08862f-a9d0-4970-bba2-eb95dc9245f8"), "values for field 'UUID' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo("Das ist einfach "), "values for field 'ShortText' are not equal");
                Assert.That(o_record.FieldText, Is.EqualTo("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed et."), "values for field 'Text' are not equal");
                Assert.That(o_record.FieldSmallInt, Is.EqualTo((short)1001), "values for field 'SmallInt' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(900000123), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldBigInt, Is.EqualTo(653398433456789458L), "values for field 'BigInt' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2011, 1, 1, 2, 4, 8)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2011, 1, 1)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(2, 4, 8)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldLocalTime, Is.EqualTo(new System.TimeSpan(1, 4, 8)), "values for field 'LocalTime' are not equal");
                Assert.That(o_record.FieldLocalDate, Is.EqualTo(new System.DateTime(2011, 1, 1)), "values for field 'LocalDate' are not equal");
                Assert.That(o_record.FieldLocalDateTime, Is.EqualTo(ForestNETLib.Core.Helper.FromISO8601UTC("2011-01-01T01:04:08Z")), "values for field 'LocalDateTime' are not equal");
                Assert.That(o_record.FieldByteCol, Is.EqualTo((byte)127), "values for field 'ByteCol' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(1448.83f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(1511.171755d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(208.22104724543748m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldBool, Is.EqualTo(false), "values for field 'Bool' are not equal");
                Assert.That(o_record.FieldText2, Is.EqualTo("Living valley had silent eat mer"), "values for field 'Text2' are not equal");
                Assert.That(o_record.FieldShortText2, Is.EqualTo("One Two "), "values for field 'ShortText2' are not equal");
            }
            else if (p_i_record == 1)
            {
                Assert.That(o_record.FieldId, Is.EqualTo(2), "values for field 'Id' are not equal");
                Assert.That(o_record.FieldUUID, Is.EqualTo("7ac8ac59-055c-46e2-894b-2ae02f7ed26e"), "values for field 'UUID' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo("Test Test Test T"), "values for field 'ShortText' are not equal");
                Assert.That(o_record.FieldText, Is.EqualTo("Procuring education on consulted assurance in do. Is sympathize."), "values for field 'Text' are not equal");
                Assert.That(o_record.FieldSmallInt, Is.EqualTo((short)1002), "values for field 'SmallInt' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(800000321), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldBigInt, Is.EqualTo(726273033145988523L), "values for field 'BigInt' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2022, 2, 2, 4, 8, 16)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2022, 2, 2)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(4, 8, 16)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldLocalTime, Is.EqualTo(new System.TimeSpan(3, 8, 16)), "values for field 'LocalTime' are not equal");
                Assert.That(o_record.FieldLocalDate, Is.EqualTo(new System.DateTime(2022, 2, 2)), "values for field 'LocalDate' are not equal");
                Assert.That(o_record.FieldLocalDateTime, Is.EqualTo(ForestNETLib.Core.Helper.FromISO8601UTC("2022-02-02T03:08:16Z")), "values for field 'LocalDateTime' are not equal");
                Assert.That(o_record.FieldByteCol, Is.EqualTo((byte)64), "values for field 'ByteCol' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(2195.12f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(-755.585877d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(47.03507874239581m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldBool, Is.EqualTo(true), "values for field 'Bool' are not equal");
                Assert.That(o_record.FieldText2, Is.EqualTo("its esteem bed. In last an or we"), "values for field 'Text2' are not equal");
                Assert.That(o_record.FieldShortText2, Is.EqualTo("Three Fo"), "values for field 'ShortText2' are not equal");
            }
            else if (p_i_record == 2)
            {
                Assert.That(o_record.FieldId, Is.EqualTo(3), "values for field 'Id' are not equal");
                Assert.That(o_record.FieldUUID, Is.EqualTo("15b19fdc-2d37-4481-8e88-bc022a4ed715"), "values for field 'UUID' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo("A B C D E F G H "), "values for field 'ShortText' are not equal");
                Assert.That(o_record.FieldText, Is.EqualTo("he expression mr no travelling. Preference he he at travelling. "), "values for field 'Text' are not equal");
                Assert.That(o_record.FieldSmallInt, Is.EqualTo((short)1003), "values for field 'SmallInt' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(700123000), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldBigInt, Is.EqualTo(973697365456213587L), "values for field 'BigInt' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2033, 3, 3, 6, 16, 32)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2033, 3, 3)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(6, 16, 32)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldLocalTime, Is.EqualTo(new System.TimeSpan(5, 16, 32)), "values for field 'LocalTime' are not equal");
                Assert.That(o_record.FieldLocalDate, Is.EqualTo(new System.DateTime(2033, 3, 3)), "values for field 'LocalDate' are not equal");
                Assert.That(o_record.FieldLocalDateTime, Is.EqualTo(ForestNETLib.Core.Helper.FromISO8601UTC("2033-03-03T05:16:32Z")), "values for field 'LocalDateTime' are not equal");
                Assert.That(o_record.FieldByteCol, Is.EqualTo((byte)32), "values for field 'ByteCol' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(4390.24f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(3585.195292d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(67.58769685598953m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldBool, Is.EqualTo(true), "values for field 'Bool' are not equal");
                Assert.That(o_record.FieldText2, Is.EqualTo("nt wise as left. Visited civilly"), "values for field 'Text2' are not equal");
                Assert.That(o_record.FieldShortText2, Is.EqualTo("ur Five "), "values for field 'ShortText2' are not equal");
            }
            else if (p_i_record == 3)
            {
                Assert.That(o_record.FieldId, Is.EqualTo(4), "values for field 'Id' are not equal");
                Assert.That(o_record.FieldUUID, Is.EqualTo("e1ac53e1-72e9-41d2-8278-17c1c8be79f2"), "values for field 'UUID' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo(" a b c d e f g h"), "values for field 'ShortText' are not equal");
                Assert.That(o_record.FieldText, Is.EqualTo("resolution. So striking at of to welcomed resolved. Northward by"), "values for field 'Text' are not equal");
                Assert.That(o_record.FieldSmallInt, Is.EqualTo((short)1004), "values for field 'SmallInt' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(600321000), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldBigInt, Is.EqualTo(555672589158833618L), "values for field 'BigInt' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2044, 4, 4, 8, 32, 4)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2044, 4, 4)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(8, 32, 4)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldLocalTime, Is.EqualTo(new System.TimeSpan(7, 32, 4)), "values for field 'LocalTime' are not equal");
                Assert.That(o_record.FieldLocalDate, Is.EqualTo(new System.DateTime(2044, 4, 4)), "values for field 'LocalDate' are not equal");
                Assert.That(o_record.FieldLocalDateTime, Is.EqualTo(ForestNETLib.Core.Helper.FromISO8601UTC("2044-04-04T07:32:04Z")), "values for field 'LocalDateTime' are not equal");
                Assert.That(o_record.FieldByteCol, Is.EqualTo((byte)16), "values for field 'ByteCol' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(7317.07f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(681.56234d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(481.99316491999418m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldBool, Is.EqualTo(true), "values for field 'Bool' are not equal");
                Assert.That(o_record.FieldText2, Is.EqualTo("am demesne so colonel he calling"), "values for field 'Text2' are not equal");
                Assert.That(o_record.FieldShortText2, Is.EqualTo("Six Seve"), "values for field 'ShortText2' are not equal");
            }
            else if (p_i_record == 4)
            {
                Assert.That(o_record.FieldId, Is.EqualTo(5), "values for field 'Id' are not equal");
                Assert.That(o_record.FieldUUID, Is.EqualTo("f33dc48a-8656-4cd1-970f-c3c14571038a"), "values for field 'UUID' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo("1 2 3 4 5 6 7 8 "), "values for field 'ShortText' are not equal");
                Assert.That(o_record.FieldText, Is.EqualTo("described up household therefore attention. Excellence          "), "values for field 'Text' are not equal");
                Assert.That(o_record.FieldSmallInt, Is.EqualTo((short)1005), "values for field 'SmallInt' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(512000000), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldBigInt, Is.EqualTo(536914555663547894L), "values for field 'BigInt' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2055, 5, 5, 10, 4, 8)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2055, 5, 5)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(10, 4, 8)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldLocalTime, Is.EqualTo(new System.TimeSpan(9, 4, 8)), "values for field 'LocalTime' are not equal");
                Assert.That(o_record.FieldLocalDate, Is.EqualTo(new System.DateTime(2055, 5, 5)), "values for field 'LocalDate' are not equal");
                Assert.That(o_record.FieldLocalDateTime, Is.EqualTo(ForestNETLib.Core.Helper.FromISO8601UTC("2055-05-05T09:04:08Z")), "values for field 'LocalDateTime' are not equal");
                Assert.That(o_record.FieldByteCol, Is.EqualTo((byte)8), "values for field 'ByteCol' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(4390.24f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(4726.24936d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(19.38838036399012m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldBool, Is.EqualTo(false), "values for field 'Bool' are not equal");
                Assert.That(o_record.FieldText2, Is.EqualTo("So unreserved do interested incr"), "values for field 'Text2' are not equal");
                Assert.That(o_record.FieldShortText2, Is.EqualTo("n Eight "), "values for field 'ShortText2' are not equal");
            }
            else if (p_i_record == 5)
            {
                Assert.That(o_record.FieldId, Is.EqualTo(6), "values for field 'Id' are not equal");
                Assert.That(o_record.FieldUUID, Is.EqualTo("9fbac7dd-af73-4052-bf93-10762d7966bd"), "values for field 'UUID' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo(" 9 8 7 6 5 4 3 2"), "values for field 'ShortText' are not equal");
                Assert.That(o_record.FieldText, Is.EqualTo("decisively nay man yet impression for contrasted remarkably. The"), "values for field 'Text' are not equal");
                Assert.That(o_record.FieldSmallInt, Is.EqualTo((short)1006), "values for field 'SmallInt' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(421000000), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldBigInt, Is.EqualTo(785633065136485210L), "values for field 'BigInt' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2066, 6, 6, 12, 8, 16)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2066, 6, 6)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(12, 8, 16)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldLocalTime, Is.EqualTo(new System.TimeSpan(11, 8, 16)), "values for field 'LocalTime' are not equal");
                Assert.That(o_record.FieldLocalDate, Is.EqualTo(new System.DateTime(2066, 6, 6)), "values for field 'LocalDate' are not equal");
                Assert.That(o_record.FieldLocalDateTime, Is.EqualTo(ForestNETLib.Core.Helper.FromISO8601UTC("2066-06-06T11:08:16Z")), "values for field 'LocalDateTime' are not equal");
                Assert.That(o_record.FieldByteCol, Is.EqualTo((byte)4), "values for field 'ByteCol' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(8799.24f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(-8922.053556d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(73.12946012133004m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldBool, Is.EqualTo(false), "values for field 'Bool' are not equal");
                Assert.That(o_record.FieldText2, Is.EqualTo("easing sentiments. Vanity day gi"), "values for field 'Text2' are not equal");
                Assert.That(o_record.FieldShortText2, Is.EqualTo("Nine Ten"), "values for field 'ShortText2' are not equal");
            }
            else if (p_i_record == 6)
            {
                Assert.That(o_record.FieldId, Is.EqualTo(7), "values for field 'Id' are not equal");
                Assert.That(o_record.FieldUUID, Is.EqualTo(null), "values for field 'UUID' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                Assert.That(o_record.FieldText, Is.EqualTo(null), "values for field 'Text' are not equal");
                Assert.That(o_record.FieldSmallInt, Is.EqualTo((short)0), "values for field 'SmallInt' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(0), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldBigInt, Is.EqualTo(0L), "values for field 'BigInt' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(null), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(null), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(null), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldLocalTime, Is.EqualTo(null), "values for field 'LocalTime' are not equal");
                Assert.That(o_record.FieldLocalDate, Is.EqualTo(null), "values for field 'LocalDate' are not equal");
                Assert.That(o_record.FieldLocalDateTime, Is.EqualTo(null), "values for field 'LocalDateTime' are not equal");
                Assert.That(o_record.FieldByteCol, Is.EqualTo((byte)0), "values for field 'ByteCol' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(0.0f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(0.0d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(0.0m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldBool, Is.EqualTo(false), "values for field 'Bool' are not equal");
                Assert.That(o_record.FieldText2, Is.EqualTo(null), "values for field 'Text2' are not equal");
                Assert.That(o_record.FieldShortText2, Is.EqualTo(null), "values for field 'ShortText2' are not equal");
            }
            else if (p_i_record == 7)
            {
                Assert.That(o_record.FieldId, Is.EqualTo(8), "values for field 'Id' are not equal");
                Assert.That(o_record.FieldUUID, Is.EqualTo("1babc8e1-e8c7-4ad4-bf4d-61bde1838c6b"), "values for field 'UUID' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo("Java Development"), "values for field 'ShortText' are not equal");
                Assert.That(o_record.FieldText, Is.EqualTo("spoke happy for you are out. Fertile how old address did showing"), "values for field 'Text' are not equal");
                Assert.That(o_record.FieldSmallInt, Is.EqualTo((short)1007), "values for field 'SmallInt' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(301010101), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldBigInt, Is.EqualTo(779589670663214588L), "values for field 'BigInt' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2077, 7, 7, 14, 16, 32)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2077, 7, 7)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(14, 16, 32)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldLocalTime, Is.EqualTo(new System.TimeSpan(13, 16, 32)), "values for field 'LocalTime' are not equal");
                Assert.That(o_record.FieldLocalDate, Is.EqualTo(new System.DateTime(2077, 7, 7)), "values for field 'LocalDate' are not equal");
                Assert.That(o_record.FieldLocalDateTime, Is.EqualTo(ForestNETLib.Core.Helper.FromISO8601UTC("2077-07-07T13:16:32Z")), "values for field 'LocalDateTime' are not equal");
                Assert.That(o_record.FieldByteCol, Is.EqualTo((byte)2), "values for field 'ByteCol' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(3996.24f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(-6766.160668d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(240.35392490677043m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldBool, Is.EqualTo(false), "values for field 'Bool' are not equal");
                Assert.That(o_record.FieldText2, Is.EqualTo("ving points within six not law. "), "values for field 'Text2' are not equal");
                Assert.That(o_record.FieldShortText2, Is.EqualTo("Eleven T"), "values for field 'ShortText2' are not equal");
            }
            else if (p_i_record == 8)
            {
                Assert.That(o_record.FieldId, Is.EqualTo(9), "values for field 'Id' are not equal");
                Assert.That(o_record.FieldUUID, Is.EqualTo("e979c923-654a-44b7-a70a-099942c9a834"), "values for field 'UUID' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo(".NET Development"), "values for field 'ShortText' are not equal");
                Assert.That(o_record.FieldText, Is.EqualTo("because sitting replied six. Had arose guest visit going off.   "), "values for field 'Text' are not equal");
                Assert.That(o_record.FieldSmallInt, Is.EqualTo((short)1008), "values for field 'SmallInt' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(220202020), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldBigInt, Is.EqualTo(846022040001245036L), "values for field 'BigInt' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2088, 8, 8, 16, 32, 4)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2088, 8, 8)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(16, 32, 4)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldLocalTime, Is.EqualTo(new System.TimeSpan(15, 32, 04)), "values for field 'LocalTime' are not equal");
                Assert.That(o_record.FieldLocalDate, Is.EqualTo(new System.DateTime(2088, 8, 8)), "values for field 'LocalDate' are not equal");
                Assert.That(o_record.FieldLocalDateTime, Is.EqualTo(ForestNETLib.Core.Helper.FromISO8601UTC("2088-08-08T15:32:04Z")), "values for field 'LocalDateTime' are not equal");
                Assert.That(o_record.FieldByteCol, Is.EqualTo((byte)1), "values for field 'ByteCol' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(3090.56f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(9461.026778d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(80.70784981354087m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldBool, Is.EqualTo(true), "values for field 'Bool' are not equal");
                Assert.That(o_record.FieldText2, Is.EqualTo("Few impression difficulty his us"), "values for field 'Text2' are not equal");
                Assert.That(o_record.FieldShortText2, Is.EqualTo("welve Th"), "values for field 'ShortText2' are not equal");
            }
        }

        private static void CompareOtherRecords(int p_i_record, FixedLengthRecordOtherData o_record)
        {
            if (p_i_record == 0)
            {
                Assert.That(o_record.FieldStringId, Is.EqualTo("00000A"), "values for field 'StringId' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(45872), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2011, 1, 1, 2, 4, 8)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2011, 1, 1)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(2, 4, 8)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo("A B C D E F G"), "values for field 'ShortText' are not equal");
            }
            else if (p_i_record == 1)
            {
                Assert.That(o_record.FieldStringId, Is.EqualTo("00000B"), "values for field 'StringId' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(62348), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2022, 2, 2, 4, 8, 16)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2022, 2, 2)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(4, 8, 16)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo(" H I J K L M "), "values for field 'ShortText' are not equal");
            }
            else if (p_i_record == 2)
            {
                Assert.That(o_record.FieldStringId, Is.EqualTo("00000A"), "values for field 'Id' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(45486), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2033, 3, 3, 6, 16, 32)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2033, 3, 3)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(6, 16, 32)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo("N O P Q R S T"), "values for field 'ShortText' are not equal");
            }
            else if (p_i_record == 3)
            {
                Assert.That(o_record.FieldStringId, Is.EqualTo("00000B"), "values for field 'StringId' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(97322), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2044, 4, 4, 8, 32, 4)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2044, 4, 4)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(8, 32, 4)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo(" U V W X Y Z "), "values for field 'ShortText' are not equal");
            }
            else if (p_i_record == 4)
            {
                Assert.That(o_record.FieldStringId, Is.EqualTo("00000A"), "values for field 'StringId' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(36582), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2055, 5, 5, 10, 4, 8)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2055, 5, 5)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(10, 4, 8)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo("a b c d e f g"), "values for field 'ShortText' are not equal");
            }
            else if (p_i_record == 5)
            {
                Assert.That(o_record.FieldStringId, Is.EqualTo("00000B"), "values for field 'StringId' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(0), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(null), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(null), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(null), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
            }
            else if (p_i_record == 6)
            {
                Assert.That(o_record.FieldStringId, Is.EqualTo("00000C"), "values for field 'StringId' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(22558), "values for field 'Int' are not equal");
                Assert.That(o_record.FieldTimestamp, Is.EqualTo(new System.DateTime(2066, 6, 6, 12, 8, 16)), "values for field 'Timestamp' are not equal");
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2066, 6, 6)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldTime, Is.EqualTo(new System.TimeSpan(12, 8, 16)), "values for field 'Time' are not equal");
                Assert.That(o_record.FieldShortText, Is.EqualTo(" h i j k l m "), "values for field 'ShortText' are not equal");
            }
        }

        private static void CompareAnotherRecords(int p_i_record, FixedLengthRecordAnotherData o_record)
        {
            if (p_i_record == 0)
            {
                Assert.That(o_record.FieldStringId, Is.EqualTo("00000A"), "values for field 'StringId' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(1448.83f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(1511.171755d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(208.22104724543748m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(32522), "values for field 'Int' are not equal");
            }
            else if (p_i_record == 1)
            {
                Assert.That(o_record.FieldStringId, Is.EqualTo("00000A"), "values for field 'StringId' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(2195.12f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(-755.585877d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(47.03507874239581m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(11258), "values for field 'Int' are not equal");
            }
            else if (p_i_record == 2)
            {
                Assert.That(o_record.FieldStringId, Is.EqualTo("00000A"), "values for field 'StringId' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(4390.24f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(3585.195292d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(67.58769685598953m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(13598), "values for field 'Int' are not equal");
            }
            else if (p_i_record == 3)
            {
                Assert.That(o_record.FieldStringId, Is.EqualTo("00000B"), "values for field 'StringId' are not equal");
                Assert.That(o_record.FieldFloatCol, Is.EqualTo(0.0f), "values for field 'FloatCol' are not equal");
                Assert.That(o_record.FieldDoubleCol, Is.EqualTo(0.0d), "values for field 'DoubleCol' are not equal");
                Assert.That(o_record.FieldDecimal, Is.EqualTo(0.0m), "values for field 'Decimal' are not equal");
                Assert.That(o_record.FieldInt, Is.EqualTo(0), "values for field 'Int' are not equal");
            }
        }

        private static void CompareRecordsWithSubtypes(int p_i_record, FixedLengthRecordDataWithSubtypes o_record)
        {
            if (p_i_record == 0)
            {
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2010, 10, 10, 0, 0, 0)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldLastNotice, Is.EqualTo("Last notice   1"), "values for field 'LastNotice' are not equal");

                int i = 1;

                foreach (FixedLengthRecordSubtypeOne o_one in o_record.FieldListOnes ?? [])
                {
                    if (i == 1)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(111), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test1     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 2)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(222), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test2     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 3)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(333), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test3     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 4)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(444), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test4     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 5)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(555), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test5     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 6)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(666), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test6     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 7)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 8)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 9)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 10)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 11)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }

                    i++;
                }

                i = 1;

                foreach (FixedLengthRecordSubtypeTwo o_two in o_record.FieldListTwos ?? [])
                {
                    if (i == 1)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(13579.246d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 2)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(-951.753d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 3)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(0.0d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 4)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(0.0d), "values for field 'DoubleValue' are not equal");
                    }

                    i++;
                }
            }
            else if (p_i_record == 1)
            {
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2020, 2, 2, 0, 0, 0)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldLastNotice, Is.EqualTo("Last notice   2"), "values for field 'LastNotice' are not equal");

                int i = 1;

                foreach (FixedLengthRecordSubtypeOne o_one in o_record.FieldListOnes ?? [])
                {
                    if (i == 1)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 2)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 3)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 4)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 5)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 6)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 7)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(7), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("     Test7"), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 8)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(8), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("     Test8"), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 9)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(9), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("     Test9"), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 10)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(10), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("    Test10"), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 11)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(11), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("    Test11"), "values for field 'ShortText' are not equal");
                    }

                    i++;
                }

                i = 1;

                foreach (FixedLengthRecordSubtypeTwo o_two in o_record.FieldListTwos ?? [])
                {
                    if (i == 1)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(0.0d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 2)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(0.0d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 3)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(-4623527.958d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 4)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(-66538.974d), "values for field 'DoubleValue' are not equal");
                    }

                    i++;
                }
            }
            else if (p_i_record == 2)
            {
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2030, 3, 3, 0, 0, 0)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldLastNotice, Is.EqualTo("Last notice   3"), "values for field 'LastNotice' are not equal");

                int i = 1;

                foreach (FixedLengthRecordSubtypeOne o_one in o_record.FieldListOnes ?? [])
                {
                    if (i == 1)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(111), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test1     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 2)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(222), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test2     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 3)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(333), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test3     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 4)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(444), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test4     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 5)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(555), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test5     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 6)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(666), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test6     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 7)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(7), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("     Test7"), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 8)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(8), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("     Test8"), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 9)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(9), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("     Test9"), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 10)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(10), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("    Test10"), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 11)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(11), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("    Test11"), "values for field 'ShortText' are not equal");
                    }

                    i++;
                }

                i = 1;

                foreach (FixedLengthRecordSubtypeTwo o_two in o_record.FieldListTwos ?? [])
                {
                    if (i == 1)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(13579.246d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 2)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(-951.753d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 3)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(-4623527.958d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 4)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(-66538.974d), "values for field 'DoubleValue' are not equal");
                    }

                    i++;
                }
            }
            else if (p_i_record == 3)
            {
                Assert.That(o_record.FieldDate, Is.EqualTo(new System.DateTime(2040, 4, 4, 0, 0, 0)), "values for field 'Date' are not equal");
                Assert.That(o_record.FieldLastNotice, Is.EqualTo("Last notice   4"), "values for field 'LastNotice' are not equal");

                int i = 1;

                foreach (FixedLengthRecordSubtypeOne o_one in o_record.FieldListOnes ?? [])
                {
                    if (i == 1)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(111), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test1     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 2)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 3)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(333), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test3     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 4)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 5)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(555), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("Test5     "), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 6)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 7)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(7), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("     Test7"), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 8)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 9)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(9), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("     Test9"), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 10)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(0), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo(null), "values for field 'ShortText' are not equal");
                    }
                    else if (i == 11)
                    {
                        Assert.That(o_one.FieldThreeDigitId, Is.EqualTo(11), "values for field 'ThreeDigitId' are not equal");
                        Assert.That(o_one.FieldShortText, Is.EqualTo("    Test11"), "values for field 'ShortText' are not equal");
                    }

                    i++;
                }

                i = 1;

                foreach (FixedLengthRecordSubtypeTwo o_two in o_record.FieldListTwos ?? [])
                {
                    if (i == 1)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(13579.246d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 2)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(0.0d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 3)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(-4623527.958d), "values for field 'DoubleValue' are not equal");
                    }
                    else if (i == 4)
                    {
                        Assert.That(o_two.FieldDoubleValue, Is.EqualTo(0.0d), "values for field 'DoubleValue' are not equal");
                    }

                    i++;
                }
            }
        }


        private static void FLRWriteTests(string p_s_flrFileName, string p_s_contentCompareFileName, int p_i_mode)
        {
            FixedLengthRecordData o_flrData = new();
            FixedLengthRecordFile o_flrFile = new(o_flrData, "^000.*$");
            CreateStackData(p_i_mode, o_flrFile);
            o_flrFile.WriteFile(p_s_flrFileName, new System.Text.UTF8Encoding(false)); // use UTF8 but without BOM

            /* compare file hashes */
            Assert.That(
                ForestNETLib.IO.File.HashFile(p_s_flrFileName, "SHA-512"),
                Is.EqualTo(ForestNETLib.IO.File.HashFile(p_s_contentCompareFileName, "SHA-512")),
                "flr file[" + p_s_flrFileName + "] has not the expected content"
            );
        }

        private static void CreateStackData(int p_i_mode, ForestNETLib.IO.FixedLengthRecordFile p_o_flrFile)
        {
            /*
		     * mode = 0 -> only one stack with 9 records
		     * mode = 1 -> 3 stacks with group headers and records
		     * mode = 2 -> 3 stacks with group footers and records
		     * mode = 3 -> 3 stacks with group headers, footers and record
		     * mode = 4 -> 3 stacks with group headers, footers and 3 different types of records
		     * mode = 5 -> 3 stacks with group headers, footers and 4 different types of records - one type has two subtypes
		     */

            int i_stackNumber = 0;
            int i_recordNumber = 0;

            ForestNETLib.IO.FixedLengthRecordFile.FixedLengthRecordStack o_stack = ForestNETLib.IO.FixedLengthRecordFile.CreateNewStack();

            if ((p_i_mode == 1) || (p_i_mode == 3) || (p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordGroupHeaderData o_groupHeader = new()
                {
                    FieldCustomerNumber = 123,
                    FieldDate = new System.DateTime(2011, 1, 1),
                    FieldDoubleWithSeparator = 314598.443589d
                };

                o_stack.GroupHeader = o_groupHeader;
            }

            FixedLengthRecordData o_flr = new()
            {
                FieldId = 1,
                FieldUUID = "9d08862f-a9d0-4970-bba2-eb95dc9245f8",
                FieldShortText = "Das ist einfach ",
                FieldText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed et.",
                FieldSmallInt = (short)1001,
                FieldInt = 900000123,
                FieldBigInt = 653398433456789458L,
                FieldTimestamp = new System.DateTime(2011, 1, 1, 2, 4, 8),
                FieldDate = new System.DateTime(2011, 1, 1),
                FieldTime = new System.TimeSpan(2, 4, 8),
                FieldLocalTime = new System.TimeSpan(1, 4, 8),
                FieldLocalDate = new System.DateTime(2011, 1, 1),
                FieldLocalDateTime = ForestNETLib.Core.Helper.FromISO8601UTC("2011-01-01T01:04:08Z"),
                FieldByteCol = (byte)127,
                FieldFloatCol = 1448.83f,
                FieldDoubleCol = 1511.171755d,
                FieldDecimal = 208.22104724543748m,
                FieldBool = false,
                FieldText2 = "Living valley had silent eat mer",
                FieldShortText2 = "One Two "
            };

            o_stack.AddFixedLengthRecord(i_recordNumber++, o_flr);

            if ((p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordOtherData o_flrOther = new()
                {
                    FieldStringId = "00000A",
                    FieldInt = 45872,
                    FieldTimestamp = new System.DateTime(2011, 1, 1, 2, 4, 8),
                    FieldDate = new System.DateTime(2011, 1, 1),
                    FieldTime = new System.TimeSpan(2, 4, 8),
                    FieldShortText = "A B C D E F G"
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrOther);
            }

            o_flr = new()
            {
                FieldId = 2,
                FieldUUID = "7ac8ac59-055c-46e2-894b-2ae02f7ed26e",
                FieldShortText = "Test Test Test T",
                FieldText = "Procuring education on consulted assurance in do. Is sympathize.",
                FieldSmallInt = (short)1002,
                FieldInt = 800000321,
                FieldBigInt = 726273033145988523L,
                FieldTimestamp = new System.DateTime(2022, 2, 2, 4, 8, 16),
                FieldDate = new System.DateTime(2022, 2, 2),
                FieldTime = new System.TimeSpan(4, 8, 16),
                FieldLocalTime = new System.TimeSpan(3, 8, 16),
                FieldLocalDate = new System.DateTime(2022, 2, 2),
                FieldLocalDateTime = ForestNETLib.Core.Helper.FromISO8601UTC("2022-02-02T03:08:16Z"),
                FieldByteCol = (byte)64,
                FieldFloatCol = 2195.12f,
                FieldDoubleCol = -755.585877d,
                FieldDecimal = 47.03507874239581m,
                FieldBool = true,
                FieldText2 = "its esteem bed. In last an or we",
                FieldShortText2 = "Three Fo"
            };

            o_stack.AddFixedLengthRecord(i_recordNumber++, o_flr);

            if ((p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordOtherData o_flrOther = new()
                {
                    FieldStringId = "00000B",
                    FieldInt = 62348,
                    FieldTimestamp = new System.DateTime(2022, 2, 2, 4, 8, 16),
                    FieldDate = new System.DateTime(2022, 2, 2),
                    FieldTime = new System.TimeSpan(4, 8, 16),
                    FieldShortText = " H I J K L M "
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrOther);
            }

            o_flr = new()
            {
                FieldId = 3,
                FieldUUID = "15b19fdc-2d37-4481-8e88-bc022a4ed715",
                FieldShortText = "A B C D E F G H ",
                FieldText = "he expression mr no travelling. Preference he he at travelling. ",
                FieldSmallInt = (short)1003,
                FieldInt = 700123000,
                FieldBigInt = 973697365456213587L,
                FieldTimestamp = new System.DateTime(2033, 3, 3, 6, 16, 32),
                FieldDate = new System.DateTime(2033, 3, 3),
                FieldTime = new System.TimeSpan(6, 16, 32),
                FieldLocalTime = new System.TimeSpan(5, 16, 32),
                FieldLocalDate = new System.DateTime(2033, 3, 3),
                FieldLocalDateTime = ForestNETLib.Core.Helper.FromISO8601UTC("2033-03-03T05:16:32Z"),
                FieldByteCol = (byte)32,
                FieldFloatCol = 4390.24f,
                FieldDoubleCol = 3585.195292d,
                FieldDecimal = 67.58769685598953m,
                FieldBool = true,
                FieldText2 = "nt wise as left. Visited civilly",
                FieldShortText2 = "ur Five "
            };

            o_stack.AddFixedLengthRecord(i_recordNumber++, o_flr);

            if (p_i_mode == 5)
            {
                FixedLengthRecordDataWithSubtypes o_flrWithSubtypes = new()
                {
                    FieldDate = new System.DateTime(2010, 10, 10),
                    FieldListOnes =
                    [
                        new() { FieldThreeDigitId = 111, FieldShortText = "Test1     " },
                        new() { FieldThreeDigitId = 222, FieldShortText = "Test2     " },
                        new() { FieldThreeDigitId = 333, FieldShortText = "Test3     " },
                        new() { FieldThreeDigitId = 444, FieldShortText = "Test4     " },
                        new() { FieldThreeDigitId = 555, FieldShortText = "Test5     " },
                        new() { FieldThreeDigitId = 666, FieldShortText = "Test6     " }
                    ],
                    FieldListTwos =
                    [
                        new() { FieldDoubleValue = 13579.246d },
                        new() { FieldDoubleValue = -951.753d }
                    ],
                    FieldLastNotice = "Last notice   1"
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrWithSubtypes);
            }

            if ((p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordAnotherData o_flrAnother = new()
                {
                    FieldStringId = "00000A",
                    FieldFloatCol = 1448.83f,
                    FieldDoubleCol = 1511.171755d,
                    FieldDecimal = 208.22104724543748m,
                    FieldInt = 32522
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrAnother);
            }

            if ((p_i_mode == 2) || (p_i_mode == 3) || (p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordGroupFooterData o_groupFooter = new()
                {
                    FieldAmountRecords = (p_i_mode >= 4) ? ((p_i_mode == 4) ? 6 : 7) : 3,
                    FieldSumInt = ((p_i_mode == 4) || (p_i_mode == 5)) ? 1200132092 : 1200061721
                };

                o_stack.GroupFooter = o_groupFooter;

                p_o_flrFile.AddStack(i_stackNumber++, o_stack);
                o_stack = ForestNETLib.IO.FixedLengthRecordFile.CreateNewStack();
                i_recordNumber = 0;
            }

            if ((p_i_mode == 1) || (p_i_mode == 3) || (p_i_mode == 4) || (p_i_mode == 5))
            {
                if (p_i_mode == 1)
                {
                    p_o_flrFile.AddStack(i_stackNumber++, o_stack);
                    o_stack = ForestNETLib.IO.FixedLengthRecordFile.CreateNewStack();
                    i_recordNumber = 0;
                }

                FixedLengthRecordGroupHeaderData o_groupHeader = new()
                {
                    FieldCustomerNumber = 321,
                    FieldDate = new System.DateTime(2022, 4, 2),
                    FieldDoubleWithSeparator = 157783.965224d
                };

                o_stack.GroupHeader = o_groupHeader;
            }

            o_flr = new()
            {
                FieldId = 4,
                FieldUUID = "e1ac53e1-72e9-41d2-8278-17c1c8be79f2",
                FieldShortText = " a b c d e f g h",
                FieldText = "resolution. So striking at of to welcomed resolved. Northward by",
                FieldSmallInt = (short)1004,
                FieldInt = 600321000,
                FieldBigInt = 555672589158833618L,
                FieldTimestamp = new System.DateTime(2044, 4, 4, 8, 32, 4),
                FieldDate = new System.DateTime(2044, 4, 4),
                FieldTime = new System.TimeSpan(8, 32, 4),
                FieldLocalTime = new System.TimeSpan(7, 32, 4),
                FieldLocalDate = new System.DateTime(2044, 4, 4),
                FieldLocalDateTime = ForestNETLib.Core.Helper.FromISO8601UTC("2044-04-04T07:32:04Z"),
                FieldByteCol = (byte)16,
                FieldFloatCol = 7317.07f,
                FieldDoubleCol = 681.56234d,
                FieldDecimal = 481.99316491999418m,
                FieldBool = true,
                FieldText2 = "am demesne so colonel he calling",
                FieldShortText2 = "Six Seve"
            };

            o_stack.AddFixedLengthRecord(i_recordNumber++, o_flr);

            if ((p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordOtherData o_flrOther = new()
                {
                    FieldStringId = "00000A",
                    FieldInt = 45486,
                    FieldTimestamp = new System.DateTime(2033, 3, 3, 6, 16, 32),
                    FieldDate = new System.DateTime(2033, 3, 3),
                    FieldTime = new System.TimeSpan(6, 16, 32),
                    FieldShortText = "N O P Q R S T"
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrOther);
            }

            if (p_i_mode == 5)
            {
                FixedLengthRecordDataWithSubtypes o_flrWithSubtypes = new()
                {
                    FieldDate = new System.DateTime(2020, 2, 2),
                    FieldListOnes =
                    [
                        new() { FieldThreeDigitId = 0, FieldShortText = null },
                        new() { FieldThreeDigitId = 0, FieldShortText = null },
                        new() { FieldThreeDigitId = 0, FieldShortText = null },
                        new() { FieldThreeDigitId = 0, FieldShortText = null },
                        new() { FieldThreeDigitId = 0, FieldShortText = null },
                        new() { FieldThreeDigitId = 0, FieldShortText = null },
                        new() { FieldThreeDigitId = 7, FieldShortText = "     Test7" },
                        new() { FieldThreeDigitId = 8, FieldShortText = "     Test8" },
                        new() { FieldThreeDigitId = 9, FieldShortText = "     Test9" },
                        new() { FieldThreeDigitId = 10, FieldShortText = "    Test10" },
                        new() { FieldThreeDigitId = 11, FieldShortText = "    Test11" }
                    ],
                    FieldListTwos =
                    [
                        new() { FieldDoubleValue = 0.0d },
                        new() { FieldDoubleValue = 0.0d },
                        new() { FieldDoubleValue = -4623527.958d },
                        new() { FieldDoubleValue = -66538.974d }
                    ],
                    FieldLastNotice = "Last notice   2"
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrWithSubtypes);
            }

            if ((p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordAnotherData o_flrAnother = new()
                {
                    FieldStringId = "00000A",
                    FieldFloatCol = 2195.12f,
                    FieldDoubleCol = -755.585877d,
                    FieldDecimal = 47.03507874239581m,
                    FieldInt = 11258
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrAnother);
            }

            o_flr = new()
            {
                FieldId = 5,
                FieldUUID = "f33dc48a-8656-4cd1-970f-c3c14571038a",
                FieldShortText = "1 2 3 4 5 6 7 8 ",
                FieldText = "described up household therefore attention. Excellence          ",
                FieldSmallInt = (short)1005,
                FieldInt = 512000000,
                FieldBigInt = 536914555663547894L,
                FieldTimestamp = new System.DateTime(2055, 5, 5, 10, 4, 8),
                FieldDate = new System.DateTime(2055, 5, 5),
                FieldTime = new System.TimeSpan(10, 4, 8),
                FieldLocalTime = new System.TimeSpan(9, 4, 8),
                FieldLocalDate = new System.DateTime(2055, 5, 5),
                FieldLocalDateTime = ForestNETLib.Core.Helper.FromISO8601UTC("2055-05-05T09:04:08Z"),
                FieldByteCol = (byte)8,
                FieldFloatCol = 4390.24f,
                FieldDoubleCol = 4726.24936d,
                FieldDecimal = 19.38838036399012m,
                FieldBool = false,
                FieldText2 = "So unreserved do interested incr",
                FieldShortText2 = "n Eight "
            };

            o_stack.AddFixedLengthRecord(i_recordNumber++, o_flr);

            o_flr = new()
            {
                FieldId = 6,
                FieldUUID = "9fbac7dd-af73-4052-bf93-10762d7966bd",
                FieldShortText = " 9 8 7 6 5 4 3 2",
                FieldText = "decisively nay man yet impression for contrasted remarkably. The",
                FieldSmallInt = (short)1006,
                FieldInt = 421000000,
                FieldBigInt = 785633065136485210L,
                FieldTimestamp = new System.DateTime(2066, 6, 6, 12, 8, 16),
                FieldDate = new System.DateTime(2066, 6, 6),
                FieldTime = new System.TimeSpan(12, 8, 16),
                FieldLocalTime = new System.TimeSpan(11, 8, 16),
                FieldLocalDate = new System.DateTime(2066, 6, 6),
                FieldLocalDateTime = ForestNETLib.Core.Helper.FromISO8601UTC("2066-06-06T11:08:16Z"),
                FieldByteCol = (byte)4,
                FieldFloatCol = 8799.24f,
                FieldDoubleCol = -8922.053556d,
                FieldDecimal = 73.12946012133004m,
                FieldBool = false,
                FieldText2 = "easing sentiments. Vanity day gi",
                FieldShortText2 = "Nine Ten"
            };

            o_stack.AddFixedLengthRecord(i_recordNumber++, o_flr);

            if ((p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordOtherData o_flrOther = new()
                {
                    FieldStringId = "00000B",
                    FieldInt = 97322,
                    FieldTimestamp = new System.DateTime(2044, 4, 4, 8, 32, 4),
                    FieldDate = new System.DateTime(2044, 4, 4),
                    FieldTime = new System.TimeSpan(8, 32, 4),
                    FieldShortText = " U V W X Y Z "
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrOther);
            }

            if ((p_i_mode == 2) || (p_i_mode == 3) || (p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordGroupFooterData o_groupFooter = new()
                {
                    FieldAmountRecords = (p_i_mode >= 4) ? ((p_i_mode == 4) ? 6 : 7) : 3,
                    FieldSumInt = ((p_i_mode == 4) || (p_i_mode == 5)) ? 766737533 : 766660500
                };

                o_stack.GroupFooter = o_groupFooter;

                p_o_flrFile.AddStack(i_stackNumber++, o_stack);
                o_stack = ForestNETLib.IO.FixedLengthRecordFile.CreateNewStack();
                i_recordNumber = 0;
            }

            if ((p_i_mode == 1) || (p_i_mode == 3) || (p_i_mode == 4) || (p_i_mode == 5))
            {
                if (p_i_mode == 1)
                {
                    p_o_flrFile.AddStack(i_stackNumber++, o_stack);
                    o_stack = ForestNETLib.IO.FixedLengthRecordFile.CreateNewStack();
                    i_recordNumber = 0;
                }

                FixedLengthRecordGroupHeaderData o_groupHeader = new()
                {
                    FieldCustomerNumber = 132,
                    FieldDate = new System.DateTime(2033, 6, 3),
                    FieldDoubleWithSeparator = 453665.357896d
                };

                o_stack.GroupHeader = o_groupHeader;
            }

            o_flr = new()
            {
                FieldId = 7,
                FieldUUID = null,
                FieldShortText = null,
                FieldText = null,
                FieldSmallInt = (short)0,
                FieldInt = 0,
                FieldBigInt = 0L,
                FieldTimestamp = null,
                FieldDate = null,
                FieldTime = null,
                FieldLocalTime = null,
                FieldLocalDate = null,
                FieldLocalDateTime = null,
                FieldByteCol = (byte)0,
                FieldFloatCol = 0.0f,
                FieldDoubleCol = 0.0d,
                FieldDecimal = 0.0m,
                FieldBool = false,
                FieldText2 = null,
                FieldShortText2 = null
            };

            o_stack.AddFixedLengthRecord(i_recordNumber++, o_flr);

            if ((p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordAnotherData o_flrAnother = new()
                {
                    FieldStringId = "00000A",
                    FieldFloatCol = 4390.24f,
                    FieldDoubleCol = 3585.195292d,
                    FieldDecimal = 67.58769685598953m,
                    FieldInt = 13598
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrAnother);
            }

            if ((p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordAnotherData o_flrAnother = new()
                {
                    FieldStringId = "00000B",
                    FieldFloatCol = 0.0f,
                    FieldDoubleCol = 0.0d,
                    FieldDecimal = 0.0m,
                    FieldInt = 0
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrAnother);
            }

            o_flr = new()
            {
                FieldId = 8,
                FieldUUID = "1babc8e1-e8c7-4ad4-bf4d-61bde1838c6b",
                FieldShortText = "Java Development",
                FieldText = "spoke happy for you are out. Fertile how old address did showing",
                FieldSmallInt = (short)1007,
                FieldInt = 301010101,
                FieldBigInt = 779589670663214588L,
                FieldTimestamp = new System.DateTime(2077, 7, 7, 14, 16, 32),
                FieldDate = new System.DateTime(2077, 7, 7),
                FieldTime = new System.TimeSpan(14, 16, 32),
                FieldLocalTime = new System.TimeSpan(13, 16, 32),
                FieldLocalDate = new System.DateTime(2077, 7, 7),
                FieldLocalDateTime = ForestNETLib.Core.Helper.FromISO8601UTC("2077-07-07T13:16:32Z"),
                FieldByteCol = (byte)2,
                FieldFloatCol = 3996.24f,
                FieldDoubleCol = -6766.160668d,
                FieldDecimal = 240.35392490677043m,
                FieldBool = false,
                FieldText2 = "ving points within six not law. ",
                FieldShortText2 = "Eleven T"
            };

            o_stack.AddFixedLengthRecord(i_recordNumber++, o_flr);

            if ((p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordOtherData o_flrOther = new()
                {
                    FieldStringId = "00000A",
                    FieldInt = 36582,
                    FieldTimestamp = new System.DateTime(2055, 5, 5, 10, 4, 8),
                    FieldDate = new System.DateTime(2055, 5, 5),
                    FieldTime = new System.TimeSpan(10, 4, 8),
                    FieldShortText = "a b c d e f g"
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrOther);
            }

            if ((p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordOtherData o_flrOther = new()
                {
                    FieldStringId = "00000B",
                    FieldInt = 0,
                    FieldTimestamp = null,
                    FieldDate = null,
                    FieldTime = null,
                    FieldShortText = null
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrOther);
            }

            o_flr = new()
            {
                FieldId = 9,
                FieldUUID = "e979c923-654a-44b7-a70a-099942c9a834",
                FieldShortText = ".NET Development",
                FieldText = "because sitting replied six. Had arose guest visit going off.   ",
                FieldSmallInt = (short)1008,
                FieldInt = 220202020,
                FieldBigInt = 846022040001245036L,
                FieldTimestamp = new System.DateTime(2088, 8, 8, 16, 32, 4),
                FieldDate = new System.DateTime(2088, 8, 8),
                FieldTime = new System.TimeSpan(16, 32, 4),
                FieldLocalTime = new System.TimeSpan(15, 32, 4),
                FieldLocalDate = new System.DateTime(2088, 8, 8),
                FieldLocalDateTime = ForestNETLib.Core.Helper.FromISO8601UTC("2088-08-08T15:32:04Z"),
                FieldByteCol = (byte)1,
                FieldFloatCol = 3090.56f,
                FieldDoubleCol = 9461.026778d,
                FieldDecimal = 80.70784981354087m,
                FieldBool = true,
                FieldText2 = "Few impression difficulty his us",
                FieldShortText2 = "welve Th"
            };

            o_stack.AddFixedLengthRecord(i_recordNumber++, o_flr);

            if ((p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordOtherData o_flrOther = new()
                {
                    FieldStringId = "00000C",
                    FieldInt = 22558,
                    FieldTimestamp = new System.DateTime(2066, 6, 6, 12, 8, 16),
                    FieldDate = new System.DateTime(2066, 6, 6),
                    FieldTime = new System.TimeSpan(12, 8, 16),
                    FieldShortText = " h i j k l m "
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrOther);
            }

            if (p_i_mode == 5)
            {
                FixedLengthRecordDataWithSubtypes o_flrWithSubtypes = new()
                {
                    FieldDate = new System.DateTime(2030, 3, 3),
                    FieldListOnes =
                    [
                        new() { FieldThreeDigitId = 111, FieldShortText = "Test1     " },
                        new() { FieldThreeDigitId = 222, FieldShortText = "Test2     " },
                        new() { FieldThreeDigitId = 333, FieldShortText = "Test3     " },
                        new() { FieldThreeDigitId = 444, FieldShortText = "Test4     " },
                        new() { FieldThreeDigitId = 555, FieldShortText = "Test5     " },
                        new() { FieldThreeDigitId = 666, FieldShortText = "Test6     " },
                        new() { FieldThreeDigitId = 7, FieldShortText = "     Test7" },
                        new() { FieldThreeDigitId = 8, FieldShortText = "     Test8" },
                        new() { FieldThreeDigitId = 9, FieldShortText = "     Test9" },
                        new() { FieldThreeDigitId = 10, FieldShortText = "    Test10" },
                        new() { FieldThreeDigitId = 11, FieldShortText = "    Test11" }
                    ],
                    FieldListTwos =
                    [
                        new() { FieldDoubleValue = 13579.246d },
                        new() { FieldDoubleValue = -951.753d },
                        new() { FieldDoubleValue = -4623527.958d },
                        new() { FieldDoubleValue = -66538.974d }
                    ],
                    FieldLastNotice = "Last notice   3"
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrWithSubtypes);

                o_flrWithSubtypes = new()
                {
                    FieldDate = new System.DateTime(2040, 4, 4),
                    FieldListOnes =
                    [
                        new() { FieldThreeDigitId = 111, FieldShortText = "Test1     " },
                        new() { FieldThreeDigitId = 0, FieldShortText = null },
                        new() { FieldThreeDigitId = 333, FieldShortText = "Test3     " },
                        new() { FieldThreeDigitId = 0, FieldShortText = null },
                        new() { FieldThreeDigitId = 555, FieldShortText = "Test5     " },
                        new() { FieldThreeDigitId = 0, FieldShortText = null },
                        new() { FieldThreeDigitId = 7, FieldShortText = "     Test7" },
                        new() { FieldThreeDigitId = 0, FieldShortText = null },
                        new() { FieldThreeDigitId = 9, FieldShortText = "     Test9" },
                        new() { FieldThreeDigitId = 0, FieldShortText = null },
                        new() { FieldThreeDigitId = 11, FieldShortText = "    Test11" }
                    ],
                    FieldListTwos =
                    [
                        new() { FieldDoubleValue = 13579.246d },
                        new() { FieldDoubleValue = 0.0d },
                        new() { FieldDoubleValue = -4623527.958d },
                        new() { FieldDoubleValue = 0.0d }
                    ],
                    FieldLastNotice = "Last notice   4"
                };

                o_stack.AddFixedLengthRecord(i_recordNumber++, o_flrWithSubtypes);
            }

            if ((p_i_mode == 2) || (p_i_mode == 3) || (p_i_mode == 4) || (p_i_mode == 5))
            {
                FixedLengthRecordGroupFooterData o_groupFooter = new()
                {
                    FieldAmountRecords = (p_i_mode >= 4) ? ((p_i_mode == 4) ? 8 : 10) : 2,
                    FieldSumInt = ((p_i_mode == 4) || (p_i_mode == 5)) ? 260642429 : 260606060
                };

                o_stack.GroupFooter = o_groupFooter;
            }

            p_o_flrFile.AddStack(i_stackNumber++, o_stack);
        }


        [Test]
        public void TestFixedLengthRecordStandardTransposeMethods()
        {
            try
            {
                /* String */

                Assert.That(
                    (string)(ForestNETLib.IO.StandardTransposeMethods.TransposeString("This is just a test This is just a test This is just a test This is just a test") ?? ""),
                    Is.EqualTo("This is just a test This is just a test This is just a test This is just a test"),
                    "FixedLengthRecord StandardTransposeMethods TransposeString(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.TransposeString("This is just a test This is just a test This is just a test This is just a test", 35),
                    Is.EqualTo("This is just a test This is just a "),
                    "FixedLengthRecord StandardTransposeMethods TransposeString(Object, Integer) method does not return expected value"
                );

                /* Boolean */

                Assert.That(
                    (bool)(ForestNETLib.IO.StandardTransposeMethods.TransposeBoolean("true") ?? ""),
                    Is.EqualTo(true),
                    "FixedLengthRecord StandardTransposeMethods TransposeBoolean(String) method does not return expected value"
                );

                Assert.That(
                    (bool)(ForestNETLib.IO.StandardTransposeMethods.TransposeBoolean("1") ?? ""),
                    Is.EqualTo(true),
                    "FixedLengthRecord StandardTransposeMethods TransposeBoolean(String) method does not return expected value"
                );

                Assert.That(
                    (bool)(ForestNETLib.IO.StandardTransposeMethods.TransposeBoolean("y") ?? ""),
                    Is.EqualTo(true),
                    "FixedLengthRecord StandardTransposeMethods TransposeBoolean(String) method does not return expected value"
                );

                Assert.That(
                    (bool)(ForestNETLib.IO.StandardTransposeMethods.TransposeBoolean("j") ?? ""),
                    Is.EqualTo(true),
                    "FixedLengthRecord StandardTransposeMethods TransposeBoolean(String) method does not return expected value"
                );

                Assert.That(
                    (bool)(ForestNETLib.IO.StandardTransposeMethods.TransposeBoolean("0") ?? ""),
                    Is.EqualTo(false),
                    "FixedLengthRecord StandardTransposeMethods TransposeBoolean(String) method does not return expected value"
                );

                Assert.That(
                    (bool)(ForestNETLib.IO.StandardTransposeMethods.TransposeBoolean("test") ?? ""),
                    Is.EqualTo(false),
                    "FixedLengthRecord StandardTransposeMethods TransposeBoolean(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.TransposeBoolean(true, 1),
                    Is.EqualTo("1"),
                    "FixedLengthRecord StandardTransposeMethods TransposeBoolean(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.TransposeBoolean(false, 1),
                    Is.EqualTo("0"),
                    "FixedLengthRecord StandardTransposeMethods TransposeBoolean(Object, Integer) method does not return expected value"
                );

                /* Byte */

                Assert.That(
                    (byte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeByte("000001") ?? 0),
                    Is.EqualTo((byte)1),
                    "FixedLengthRecord StandardTransposeMethods TransposeByte(String) method does not return expected value"
                );

                Assert.That(
                    (byte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeByte("0127") ?? 0),
                    Is.EqualTo((byte)127),
                    "FixedLengthRecord StandardTransposeMethods TransposeByte(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeByte((byte)1, 6),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeByte(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeByte((byte)127, 7),
                    Is.EqualTo("0000127"),
                    "FixedLengthRecord StandardTransposeMethods TransposeByte(Object, Integer) method does not return expected value"
                );

                /* SByte */

                Assert.That(
                    (sbyte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByte("000001") ?? 0),
                    Is.EqualTo((sbyte)1),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByte(String) method does not return expected value"
                );

                Assert.That(
                    (sbyte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByte("0127") ?? 0),
                    Is.EqualTo((sbyte)127),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByte(String) method does not return expected value"
                );

                Assert.That(
                    (sbyte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByte("-00001") ?? 0),
                    Is.EqualTo((sbyte)-1),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByte(String) method does not return expected value"
                );

                Assert.That(
                    (sbyte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByte("+000127") ?? 0),
                    Is.EqualTo((sbyte)127),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByte(String) method does not return expected value"
                );

                Assert.That(
                    (sbyte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByte("-000112") ?? 0),
                    Is.EqualTo((sbyte)-112),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByte(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByte((sbyte)1, 6),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByte(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByte((sbyte)127, 4),
                    Is.EqualTo("0127"),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByte(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (sbyte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByteWithSign("+000001") ?? 0),
                    Is.EqualTo((sbyte)1),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByteWithSign(String) method does not return expected value"
                );

                Assert.That(
                    (sbyte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByteWithSign("+0127") ?? 0),
                    Is.EqualTo((sbyte)127),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByteWithSign(String) method does not return expected value"
                );

                Assert.That(
                    (sbyte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByteWithSign("-00001") ?? 0),
                    Is.EqualTo((sbyte)-1),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByteWithSign(String) method does not return expected value"
                );

                Assert.That(
                    (sbyte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByteWithSign("+000127") ?? 0),
                    Is.EqualTo((sbyte)127),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByteWithSign(String) method does not return expected value"
                );

                Assert.That(
                    (sbyte)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByteWithSign("-000112") ?? 0),
                    Is.EqualTo((sbyte)-112),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByteWithSign(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByteWithSign((sbyte)1, 6),
                    Is.EqualTo("+00001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByteWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByteWithSign((sbyte)127, 4),
                    Is.EqualTo("+127"),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByteWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByteWithSign((sbyte)-1, 5),
                    Is.EqualTo("-0001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByteWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByteWithSign((sbyte)127, 6),
                    Is.EqualTo("+00127"),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByteWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeSignedByteWithSign((sbyte)-112, 7),
                    Is.EqualTo("-000112"),
                    "FixedLengthRecord StandardTransposeMethods TransposeSignedByteWithSign(Object, Integer) method does not return expected value"
                );

                /* Short */

                Assert.That(
                    (short)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeShort("000001") ?? 0),
                    Is.EqualTo((short)1),
                    "FixedLengthRecord StandardTransposeMethods TransposeShort(String) method does not return expected value"
                );

                Assert.That(
                    (short)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeShort("032767") ?? 0),
                    Is.EqualTo((short)32767),
                    "FixedLengthRecord StandardTransposeMethods TransposeShort(String) method does not return expected value"
                );

                Assert.That(
                    (short)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeShortWithSign("-00001") ?? 0),
                    Is.EqualTo((short)-1),
                    "FixedLengthRecord StandardTransposeMethods TransposeShortWithSign(String) method does not return expected value"
                );

                Assert.That(
                    (short)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeShortWithSign("+032767") ?? 0),
                    Is.EqualTo((short)32767),
                    "FixedLengthRecord StandardTransposeMethods TransposeShortWithSign(String) method does not return expected value"
                );

                Assert.That(
                    (short)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeShortWithSign("-00016348") ?? 0),
                    Is.EqualTo((short)-16348),
                    "FixedLengthRecord StandardTransposeMethods TransposeShortWithSign(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeShort((short)1, 6),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeShort(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeShort((short)32767, 7),
                    Is.EqualTo("0032767"),
                    "FixedLengthRecord StandardTransposeMethods TransposeShort(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeShortWithSign((short)-1, 5),
                    Is.EqualTo("-0001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeShortWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeShortWithSign((short)32767, 7),
                    Is.EqualTo("+032767"),
                    "FixedLengthRecord StandardTransposeMethods TransposeShortWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeShortWithSign((short)-16348, 9),
                    Is.EqualTo("-00016348"),
                    "FixedLengthRecord StandardTransposeMethods TransposeShortWithSign(Object, Integer) method does not return expected value"
                );

                /* Integer */

                Assert.That(
                    (int)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeInteger("000001") ?? 0),
                    Is.EqualTo(1),
                    "FixedLengthRecord StandardTransposeMethods TransposeInteger(String) method does not return expected value"
                );

                Assert.That(
                    (int)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeInteger("002147483647") ?? 0),
                    Is.EqualTo(2147483647),
                    "FixedLengthRecord StandardTransposeMethods TransposeInteger(String) method does not return expected value"
                );

                Assert.That(
                    (int)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeIntegerWithSign("-00001") ?? 0),
                    Is.EqualTo(-1),
                    "FixedLengthRecord StandardTransposeMethods TransposeIntegerWithSign(String) method does not return expected value"
                );

                Assert.That(
                    (int)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeIntegerWithSign("+02147483647") ?? 0),
                    Is.EqualTo(2147483647),
                    "FixedLengthRecord StandardTransposeMethods TransposeIntegerWithSign(String) method does not return expected value"
                );

                Assert.That(
                    (int)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeIntegerWithSign("-0001073741823") ?? 0),
                    Is.EqualTo(-1073741823),
                    "FixedLengthRecord StandardTransposeMethods TransposeIntegerWithSign(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeInteger(1, 6),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeInteger(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeInteger(2147483647, 12),
                    Is.EqualTo("002147483647"),
                    "FixedLengthRecord StandardTransposeMethods TransposeInteger(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeIntegerWithSign(-1, 5),
                    Is.EqualTo("-0001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeIntegerWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeIntegerWithSign(2147483647, 12),
                    Is.EqualTo("+02147483647"),
                    "FixedLengthRecord StandardTransposeMethods TransposeIntegerWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeIntegerWithSign(-1073741823, 14),
                    Is.EqualTo("-0001073741823"),
                    "FixedLengthRecord StandardTransposeMethods TransposeIntegerWithSign(Object, Integer) method does not return expected value"
                );

                /* Long */

                Assert.That(
                    (long)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeLong("000001") ?? 0),
                    Is.EqualTo(1L),
                    "FixedLengthRecord StandardTransposeMethods TransposeLong(String) method does not return expected value"
                );

                Assert.That(
                    (long)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeLong("09223372036854775807") ?? 0),
                    Is.EqualTo(9223372036854775807L),
                    "FixedLengthRecord StandardTransposeMethods TransposeLong(String) method does not return expected value"
                );

                Assert.That(
                    (long)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeLongWithSign("-00001") ?? 0),
                    Is.EqualTo(-1L),
                    "FixedLengthRecord StandardTransposeMethods TransposeLongWithSign(String) method does not return expected value"
                );

                Assert.That(
                    (long)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeLongWithSign("+009223372036854775807") ?? 0),
                    Is.EqualTo(9223372036854775807L),
                    "FixedLengthRecord StandardTransposeMethods TransposeLongWithSign(String) method does not return expected value"
                );

                Assert.That(
                    (long)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeLongWithSign("-0004611686018427387903") ?? 0),
                    Is.EqualTo(-4611686018427387903L),
                    "FixedLengthRecord StandardTransposeMethods TransposeLongWithSign(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeLong(1L, 6),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeLong(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeLong(9223372036854775807L, 22),
                    Is.EqualTo("0009223372036854775807"),
                    "FixedLengthRecord StandardTransposeMethods TransposeLong(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeLongWithSign(-1L, 5),
                    Is.EqualTo("-0001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeLongWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeLongWithSign(9223372036854775807L, 21),
                    Is.EqualTo("+09223372036854775807"),
                    "FixedLengthRecord StandardTransposeMethods TransposeLongWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeLongWithSign(-4611686018427387903L, 23),
                    Is.EqualTo("-0004611686018427387903"),
                    "FixedLengthRecord StandardTransposeMethods TransposeLongWithSign(Object, Integer) method does not return expected value"
                );

                /* UShort */

                Assert.That(
                    (ushort)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedShort("000001") ?? 0),
                    Is.EqualTo((ushort)1),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedShort(String) method does not return expected value"
                );

                Assert.That(
                    (ushort)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedShort("065535") ?? 0),
                    Is.EqualTo((ushort)65535),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedShort(String) method does not return expected value"
                );

                Assert.Throws<InvalidCastException>(() => ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedShortWithSign("-00001"));

                Assert.That(
                    (ushort)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedShortWithSign("+065535") ?? 0),
                    Is.EqualTo((ushort)65535),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedShortWithSign(String) method does not return expected value"
                );

                Assert.Throws<InvalidCastException>(() => ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedShortWithSign("-00016348"));

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedShort((ushort)1, 6),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedShort(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedShort((ushort)32767, 7),
                    Is.EqualTo("0032767"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedShort(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedShortWithSign((ushort)1, 5),
                    Is.EqualTo("+0001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedShortWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedShortWithSign((ushort)32767, 7),
                    Is.EqualTo("+032767"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedShortWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedShortWithSign((ushort)65535, 9),
                    Is.EqualTo("+00065535"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedShortWithSign(Object, Integer) method does not return expected value"
                );

                /* UInteger */

                Assert.That(
                    (uint)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedInteger("000001") ?? 0),
                    Is.EqualTo((uint)1),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedInteger(String) method does not return expected value"
                );

                Assert.That(
                    (uint)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedInteger("004294967295") ?? 0),
                    Is.EqualTo((uint)4294967295),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedInteger(String) method does not return expected value"
                );

                Assert.Throws<InvalidCastException>(() => ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedIntegerWithSign("-00001"));

                Assert.That(
                    (uint)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedIntegerWithSign("+04294967295") ?? 0),
                    Is.EqualTo((uint)4294967295),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedIntegerWithSign(String) method does not return expected value"
                );

                Assert.Throws<InvalidCastException>(() => ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedIntegerWithSign("-0001073741823"));

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedInteger((uint)1, 6),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedInteger(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedInteger((uint)4294967295, 12),
                    Is.EqualTo("004294967295"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedInteger(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedIntegerWithSign((uint)1, 5),
                    Is.EqualTo("+0001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedIntegerWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedIntegerWithSign((uint)4294967295, 12),
                    Is.EqualTo("+04294967295"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedIntegerWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedIntegerWithSign((uint)1073741823, 14),
                    Is.EqualTo("+0001073741823"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedIntegerWithSign(Object, Integer) method does not return expected value"
                );

                /* ULong */

                Assert.That(
                    (ulong)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedLong("000001") ?? 0),
                    Is.EqualTo((ulong)1L),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedLong(String) method does not return expected value"
                );

                Assert.That(
                    (ulong)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedLong("018446744073709551615") ?? 0),
                    Is.EqualTo((ulong)18446744073709551615L),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedLong(String) method does not return expected value"
                );

                Assert.Throws<InvalidCastException>(() => ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedLongWithSign("-00001"));

                Assert.That(
                    (ulong)(ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedLongWithSign("+0018446744073709551615") ?? 0),
                    Is.EqualTo((ulong)18446744073709551615L),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedLongWithSign(String) method does not return expected value"
                );

                Assert.Throws<InvalidCastException>(() => ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedLongWithSign("-0004611686018427387903"));

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedLong((ulong)1L, 6),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedLong(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedLong((ulong)18446744073709551615L, 24),
                    Is.EqualTo("000018446744073709551615"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedLong(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedLongWithSign((ulong)1L, 5),
                    Is.EqualTo("+0001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedLongWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedLongWithSign((ulong)18446744073709551615L, 22),
                    Is.EqualTo("+018446744073709551615"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedLongWithSign(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.Numbers.TransposeUnsignedLongWithSign((ulong)4611686018427387903L, 23),
                    Is.EqualTo("+0004611686018427387903"),
                    "FixedLengthRecord StandardTransposeMethods TransposeUnsignedLongWithSign(Object, Integer) method does not return expected value"
                );

                /* DateTime */

                DateTime o_dateTime = new(2020, 03, 14, 06, 02, 03);

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ISO8601("2020-03-14T05:02:03Z") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ISO8601(String) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ISO8601("2020-03-14 05:02:03Z") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ISO8601(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ISO8601_NoSeparator(o_dateTime, 20),
                    Is.EqualTo("2020-03-14 05:02:03Z"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ISO8601(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ISO8601(o_dateTime, 20),
                    Is.EqualTo("2020-03-14T05:02:03Z"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ISO8601(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_RFC1123("Sat, 14 Mar 2020 05:02:03 GMT") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_RFC1123(String) method does not return expected value"
                );

                o_dateTime = new DateTime(2020, 03, 14, 16, 02, 03);

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_RFC1123(o_dateTime, 29),
                    Is.EqualTo("Sat, 14 Mar 2020 15:02:03 GMT"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_RFC1123(Object, Integer) method does not return expected value"
                );

                o_dateTime = new DateTime(2011, 01, 01, 02, 04, 08);

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmddhhiiss("20110101020408") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmddhhiiss(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmddhhiiss(o_dateTime, 14),
                    Is.EqualTo("20110101020408"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmddhhiiss(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmddhhiiss_ISO("2011-01-01 02:04:08") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmddhhiiss_ISO(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmddhhiiss_ISO(o_dateTime, 19),
                    Is.EqualTo("2011-01-01 02:04:08"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmddhhiiss_ISO(Object, Integer) method does not return expected value"
                );

                o_dateTime = new DateTime(2011, 10, 01, 02, 04, 08);

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ddmmyyyyhhiiss_Dot("01.10.2011 02:04:08") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ddmmyyyyhhiiss_Dot(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ddmmyyyyhhiiss_Dot(o_dateTime, 19),
                    Is.EqualTo("01.10.2011 02:04:08"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ddmmyyyyhhiiss_Dot(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ddmmyyyyhhiiss_Slash("01/10/2011 02:04:08") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ddmmyyyyhhiiss_Slash(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ddmmyyyyhhiiss_Slash(o_dateTime, 19),
                    Is.EqualTo("01/10/2011 02:04:08"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ddmmyyyyhhiiss_Slash(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmddhhiiss_Dot("2011.10.01 02:04:08") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmddhhiiss_Dot(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmddhhiiss_Dot(o_dateTime, 19),
                    Is.EqualTo("2011.10.01 02:04:08"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmddhhiiss_Dot(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmddhhiiss_Slash("2011/10/01 02:04:08") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmddhhiiss_Slash(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmddhhiiss_Slash(o_dateTime, 19),
                    Is.EqualTo("2011/10/01 02:04:08"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmddhhiiss_Slash(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_mmddyyyyhhiiss_Dot("10.01.2011 02:04:08") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_mmddyyyyhhiiss_Dot(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_mmddyyyyhhiiss_Dot(o_dateTime, 19),
                    Is.EqualTo("10.01.2011 02:04:08"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_mmddyyyyhhiiss_Dot(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_mmddyyyyhhiiss_Slash("10/01/2011 02:04:08") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_mmddyyyyhhiiss_Slash(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_mmddyyyyhhiiss_Slash(o_dateTime, 19),
                    Is.EqualTo("10/01/2011 02:04:08"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_mmddyyyyhhiiss_Slash(Object, Integer) method does not return expected value"
                );

                o_dateTime = new DateTime(2011, 01, 01);

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmdd("20110101") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmdd(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmdd(o_dateTime, 8),
                    Is.EqualTo("20110101"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmdd(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmdd_ISO("2011-01-01") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmdd(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmdd_ISO(o_dateTime, 10),
                    Is.EqualTo("2011-01-01"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmdd(Object, Integer) method does not return expected value"
                );

                o_dateTime = new DateTime(2011, 10, 01);

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ddmmyyyy_Dot("01.10.2011") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ddmmyyyy_Dot(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ddmmyyyy_Dot(o_dateTime, 10),
                    Is.EqualTo("01.10.2011"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ddmmyyyy_Dot(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ddmmyyyy_Slash("01/10/2011") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ddmmyyyy_Slash(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_ddmmyyyy_Slash(o_dateTime, 10),
                    Is.EqualTo("01/10/2011"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_ddmmyyyy_Slash(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmdd_Dot("2011.10.01") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmdd_Dot(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmdd_Dot(o_dateTime, 10),
                    Is.EqualTo("2011.10.01"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmdd_Dot(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmdd_Slash("2011/10/01") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmdd_Slash(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_yyyymmdd_Slash(o_dateTime, 10),
                    Is.EqualTo("2011/10/01"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_yyyymmdd_Slash(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_mmddyyyy_Dot("10.01.2011") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_mmddyyyy_Dot(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_mmddyyyy_Dot(o_dateTime, 10),
                    Is.EqualTo("10.01.2011"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_mmddyyyy_Dot(Object, Integer) method does not return expected value"
                );

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_mmddyyyy_Slash("10/01/2011") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_mmddyyyy_Slash(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_mmddyyyy_Slash(o_dateTime, 10),
                    Is.EqualTo("10/01/2011"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_mmddyyyy_Slash(Object, Integer) method does not return expected value"
                );

                o_dateTime = new DateTime(1, 1, 1, 02, 03, 04);

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_hhiiss("020304") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_hhiiss(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_hhiiss(o_dateTime, 6),
                    Is.EqualTo("020304"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_hhiiss(Object, Integer) method does not return expected value"
                );

                o_dateTime = new DateTime(1, 1, 1, 02, 03, 0);

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_hhii("0203") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_hhii(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_hhii(o_dateTime, 4),
                    Is.EqualTo("0203"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_hhii(Object, Integer) method does not return expected value"
                );

                o_dateTime = new DateTime(1, 1, 1, 02, 03, 04);

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_hhiiss_Colon("02:03:04") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_hhiiss_Colon(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_hhiiss_Colon(o_dateTime, 8),
                    Is.EqualTo("02:03:04"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_hhiiss_Colon(Object, Integer) method does not return expected value"
                );

                o_dateTime = new DateTime(1, 1, 1, 02, 03, 0);

                Assert.That(
                    (DateTime)(ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_hhii_Colon("02:03") ?? DateTime.MinValue),
                    Is.EqualTo(o_dateTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_hhii_Colon(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.DateTime.TransposeDateTime_hhii_Colon(o_dateTime, 5),
                    Is.EqualTo("02:03"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDateTime_hhii_Colon(Object, Integer) method does not return expected value"
                );

                /* TimeSpan */

                System.TimeSpan o_localTime = new(02, 03, 04);

                Assert.That(
                    (TimeSpan)(ForestNETLib.IO.StandardTransposeMethods.TimeSpan.TransposeTimeSpan_hhiiss("020304") ?? TimeSpan.MinValue),
                    Is.EqualTo(o_localTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeTimeSpan_hhiiss(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.TimeSpan.TransposeTimeSpan_hhiiss(o_localTime, 6),
                    Is.EqualTo("020304"),
                    "FixedLengthRecord StandardTransposeMethods TransposeTimeSpan_hhiiss(Object, Integer) method does not return expected value"
                );

                o_localTime = new System.TimeSpan(2, 3, 0);

                Assert.That(
                    (TimeSpan)(ForestNETLib.IO.StandardTransposeMethods.TimeSpan.TransposeTimeSpan_hhii("0203") ?? TimeSpan.MinValue),
                    Is.EqualTo(o_localTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeTimeSpan_hhii(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.TimeSpan.TransposeTimeSpan_hhii(o_localTime, 4),
                    Is.EqualTo("0203"),
                    "FixedLengthRecord StandardTransposeMethods TransposeTimeSpan_hhii(Object, Integer) method does not return expected value"
                );

                o_localTime = new System.TimeSpan(2, 3, 4);

                Assert.That(
                    (TimeSpan)(ForestNETLib.IO.StandardTransposeMethods.TimeSpan.TransposeTimeSpan_hhiiss_Colon("02:03:04") ?? TimeSpan.MinValue),
                    Is.EqualTo(o_localTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeTimeSpan_hhiiss_Colon(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.TimeSpan.TransposeTimeSpan_hhiiss_Colon(o_localTime, 8),
                    Is.EqualTo("02:03:04"),
                    "FixedLengthRecord StandardTransposeMethods TransposeTimeSpan_hhiiss_Colon(Object, Integer) method does not return expected value"
                );

                o_localTime = new System.TimeSpan(2, 3, 0);

                Assert.That(
                    (TimeSpan)(ForestNETLib.IO.StandardTransposeMethods.TimeSpan.TransposeTimeSpan_hhii_Colon("02:03") ?? TimeSpan.MinValue),
                    Is.EqualTo(o_localTime),
                    "FixedLengthRecord StandardTransposeMethods TransposeTimeSpan_hhii_Colon(String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.TimeSpan.TransposeTimeSpan_hhii_Colon(o_localTime, 5),
                    Is.EqualTo("02:03"),
                    "FixedLengthRecord StandardTransposeMethods TransposeTimeSpan_hhii_Colon(Object, Integer) method does not return expected value"
                );

                /* Float */

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("000001", 0) ?? 0f),
                    Is.EqualTo(1f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String) method does not return expected value"
                );

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("-00001", 0) ?? 0f),
                    Is.EqualTo(-1f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String) method does not return expected value"
                );

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("00000000000000000000000", 13) ?? 0f),
                    Is.EqualTo(0.0f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String) method does not return expected value"
                );

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("+00000000000000000000000", 13) ?? 0f),
                    Is.EqualTo(0.0f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String) method does not return expected value"
                );

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("-00000000000000000000000", 13) ?? 0f),
                    Is.EqualTo(0.0f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String) method does not return expected value"
                );

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("002147483647", 0) ?? 0f),
                    Is.EqualTo(2147483647f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String) method does not return expected value"
                );

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("00214748.3647", 0) ?? 0f),
                    Is.EqualTo(214748.3647f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String) method does not return expected value"
                );

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("+00214748.3647", 0) ?? 0f),
                    Is.EqualTo(214748.3647f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String) method does not return expected value"
                );

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("-0002147,483647", 0) ?? 0f),
                    Is.EqualTo(-2147.483647f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String) method does not return expected value"
                );

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("002147483647", 8) ?? 0f),
                    Is.EqualTo(214748.3647f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String, Integer) method does not return expected value"
                );

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("+002147483647", 9) ?? 0f),
                    Is.EqualTo(214748.3647f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String, Integer) method does not return expected value"
                );

                Assert.That(
                    (float)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat("-002147483647", 8) ?? 0f),
                    Is.EqualTo(-21474.83647f),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(String, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat(1f, 6, 0, null, null),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat(-1f, 6, 0, null, null),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat(214748f, 8, 0, null, null),
                    Is.EqualTo("00214748"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer, String, String) method does not return expected value"
                );

                /* use your system group and decimal separator here, for GER it is '.' and ',' */
                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat(214748.1230101f, 8, 2),
                    Is.EqualTo("00.214.748,12"), // Is.EqualTo("00,214,748.12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat(214748.1230101f, 8, 2, ",", "."),
                    Is.EqualTo("00.214.748,12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat(-214748.1230101f, 8, 2, ",", null),
                    Is.EqualTo("00214748,12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat(-214748.1230101f, 8, 2, null, null),
                    Is.EqualTo("0021474812"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat(214748.1230101f, 8, 4, ",", null),
                    Is.EqualTo("00214748,1250"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat(-214748.1230101f, 8, 4, null, null),
                    Is.EqualTo("002147481250"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign(1f, 6, 0, null, null),
                    Is.EqualTo("+000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign(-1f, 6, 0, null, null),
                    Is.EqualTo("-000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign(214748f, 8, 0, null, null),
                    Is.EqualTo("+00214748"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                /* use your system group and decimal separator here, for GER it is '.' and ',' */
                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign(214748.1230101f, 8, 2),
                    Is.EqualTo("+00.214.748,12"), // Is.EqualTo("+00,214,748.12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign(214748.1230101f, 8, 2, ",", "."),
                    Is.EqualTo("+00.214.748,12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign(-214748.1230101f, 8, 2, ",", null),
                    Is.EqualTo("-00214748,12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign(214748.1230101f, 8, 2, null, null),
                    Is.EqualTo("+0021474812"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign(-214748.1230101f, 8, 4, ",", null),
                    Is.EqualTo("-00214748,1250"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign(-214748.1230101f, 8, 4, null, null),
                    Is.EqualTo("-002147481250"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat((0f), 5, 8, null, null),
                    Is.EqualTo("0000000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign((0f), 5, 8, null, null),
                    Is.EqualTo("+0000000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat((0f), 5, 8, ",", null),
                    Is.EqualTo("00000,00000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign((0f), 5, 8, ",", null),
                    Is.EqualTo("+00000,00000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloat((0f), 5, 8, ",", "."),
                    Is.EqualTo("00.000,00000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloat(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign((0f), 5, 8, ",", "."),
                    Is.EqualTo("+00.000,00000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeFloatWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                /* Double */

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("000001", 0) ?? 0d),
                    Is.EqualTo(1d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String) method does not return expected value"
                );

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("-00001", 0) ?? 0d),
                    Is.EqualTo(-1d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String) method does not return expected value"
                );

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("00000000000000000000000", 13) ?? 0d),
                    Is.EqualTo(0.0d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String) method does not return expected value"
                );

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("+00000000000000000000000", 13) ?? 0d),
                    Is.EqualTo(0.0d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String) method does not return expected value"
                );

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("-00000000000000000000000", 13) ?? 0d),
                    Is.EqualTo(0.0d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String) method does not return expected value"
                );

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("002147483647", 0) ?? 0d),
                    Is.EqualTo(2147483647d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String) method does not return expected value"
                );

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("00214748.3647", 0) ?? 0d),
                    Is.EqualTo(214748.3647d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String) method does not return expected value"
                );

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("+00214748.3647", 0) ?? 0d),
                    Is.EqualTo(214748.3647d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String) method does not return expected value"
                );

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("-0002147,483647", 0) ?? 0d),
                    Is.EqualTo(-2147.483647d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String) method does not return expected value"
                );

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("002147483647", 8) ?? 0d),
                    Is.EqualTo(214748.3647d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String, Integer) method does not return expected value"
                );

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("+002147483647", 9) ?? 0d),
                    Is.EqualTo(214748.3647d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String, Integer) method does not return expected value"
                );

                Assert.That(
                    (double)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble("-002147483647", 8) ?? 0d),
                    Is.EqualTo(-21474.83647d),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(String, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble(1d, 6, 0, null, null),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble(-1d, 6, 0, null, null),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble(214748d, 8, 0, null, null),
                    Is.EqualTo("00214748"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer, String, String) method does not return expected value"
                );

                /* use your system group and decimal separator here, for GER it is '.' and ',' */
                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble(214748.1230101d, 8, 2),
                    Is.EqualTo("00.214.748,12"), // Is.EqualTo("00,214,748.12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble(214748.1230101d, 8, 2, ",", "."),
                    Is.EqualTo("00.214.748,12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble(-214748.12301019812d, 8, 4, ",", null),
                    Is.EqualTo("00214748,1230"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble(214748.12301019812d, 8, 4, null, null),
                    Is.EqualTo("002147481230"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble(-214748.12301019812d, 8, 10, ",", null),
                    Is.EqualTo("00214748,1230101980"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble(-214748.12301019812d, 8, 10, null, null),
                    Is.EqualTo("002147481230101980"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign(1d, 6, 0, null, null),
                    Is.EqualTo("+000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign(-1d, 6, 0, null, null),
                    Is.EqualTo("-000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign(214748d, 8, 0, null, null),
                    Is.EqualTo("+00214748"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                /* use your system group and decimal separator here, for GER it is '.' and ',' */
                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign(214748.1230101d, 8, 2),
                    Is.EqualTo("+00.214.748,12"), // Is.EqualTo("+00,214,748.12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign(214748.1230101d, 8, 2, ",", "."),
                    Is.EqualTo("+00.214.748,12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign(-214748.12301019812d, 8, 4, ",", null),
                    Is.EqualTo("-00214748,1230"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign(214748.12301019812d, 8, 4, null, null),
                    Is.EqualTo("+002147481230"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign(-214748.12301019812d, 8, 10, ",", null),
                    Is.EqualTo("-00214748,1230101980"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign(-214748.12301019812d, 8, 10, null, null),
                    Is.EqualTo("-002147481230101980"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble((0d), 6, 9, null, null),
                    Is.EqualTo("000000000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign((0d), 6, 9, null, null),
                    Is.EqualTo("+000000000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble((0d), 6, 9, ",", null),
                    Is.EqualTo("000000,000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign((0d), 6, 9, ",", null),
                    Is.EqualTo("+000000,000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDouble((0d), 6, 9, ",", "."),
                    Is.EqualTo("000.000,000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDouble(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign((0d), 6, 9, ",", "."),
                    Is.EqualTo("+000.000,000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDoubleWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                /* Decimal */

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("000001", 0) ?? 0m),
                    Is.EqualTo(1m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String) method does not return expected value"
                );

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("-00001", 0) ?? 0m),
                    Is.EqualTo(-1m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String) method does not return expected value"
                );

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("00000000000000000000000", 13) ?? 0m),
                    Is.EqualTo(0.0m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String) method does not return expected value"
                );

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("+00000000000000000000000", 13) ?? 0m),
                    Is.EqualTo(0.0m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String) method does not return expected value"
                );

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("-00000000000000000000000", 13) ?? 0m),
                    Is.EqualTo(0.0m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String) method does not return expected value"
                );

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("002147483647", 0) ?? 0m),
                    Is.EqualTo(2147483647m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String) method does not return expected value"
                );

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("00214748.3647", 0) ?? 0m),
                    Is.EqualTo(214748.3647m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String) method does not return expected value"
                );

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("+00214748.3647", 0) ?? 0m),
                    Is.EqualTo(214748.3647m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String) method does not return expected value"
                );

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("-0002147,483647", 0) ?? 0m),
                    Is.EqualTo(-2147.483647m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String) method does not return expected value"
                );

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("002147483647", 8) ?? 0m),
                    Is.EqualTo(214748.3647m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String, Integer) method does not return expected value"
                );

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("+002147483647", 9) ?? 0m),
                    Is.EqualTo(214748.3647m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String, Integer) method does not return expected value"
                );

                Assert.That(
                    (decimal)(ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal("-002147483647", 8) ?? 0m),
                    Is.EqualTo(-21474.83647m),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(String, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((1m), 6, 0, null, null),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((-1m), 6, 0, null, null),
                    Is.EqualTo("000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((214748m), 8, 0, null, null),
                    Is.EqualTo("00214748"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer, String, String) method does not return expected value"
                );

                /* use your system group and decimal separator here, for GER it is '.' and ',' */
                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((214748.1230101m), 8, 2),
                    Is.EqualTo("00.214.748,12"), // Is.EqualTo("00,214,748.12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((214748.1230101m), 8, 2, ",", "."),
                    Is.EqualTo("00.214.748,12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((-214748.12301019812m), 8, 4, ",", null),
                    Is.EqualTo("00214748,1230"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((214748.12301019812m), 8, 4, null, null),
                    Is.EqualTo("002147481230"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((-214748.12301019812001m), 8, 11, ",", null),
                    Is.EqualTo("00214748,12301019812"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((-214748.12301019812001m), 8, 11, null, null),
                    Is.EqualTo("0021474812301019812"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((1m), 6, 0, null, null),
                    Is.EqualTo("+000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((-1m), 6, 0, null, null),
                    Is.EqualTo("-000001"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((214748m), 8, 0, null, null),
                    Is.EqualTo("+00214748"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                /* use your system group and decimal separator here, for GER it is '.' and ',' */
                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((214748.1230101m), 8, 2),
                    Is.EqualTo("+00.214.748,12"), // Is.EqualTo("+00,214,748.12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((214748.1230101m), 8, 2, ",", "."),
                    Is.EqualTo("+00.214.748,12"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((-214748.12301019812m), 8, 4, ",", null),
                    Is.EqualTo("-00214748,1230"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((214748.12301019812m), 8, 4, null, null),
                    Is.EqualTo("+002147481230"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((-214748.12301019812009m), 8, 11, ",", null),
                    Is.EqualTo("-00214748,12301019812"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((-214748.12301019812009m), 8, 11, null, null),
                    Is.EqualTo("-0021474812301019812"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((0m), 8, 11, null, null),
                    Is.EqualTo("0000000000000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((0m), 8, 11, null, null),
                    Is.EqualTo("+0000000000000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((0m), 8, 11, ",", null),
                    Is.EqualTo("00000000,00000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((0m), 8, 11, ",", null),
                    Is.EqualTo("+00000000,00000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal((0m), 8, 11, ",", "."),
                    Is.EqualTo("00.000.000,00000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimal(Object, Integer, Integer, String, String) method does not return expected value"
                );

                Assert.That(
                    ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign((0m), 8, 11, ",", "."),
                    Is.EqualTo("+00.000.000,00000000000"),
                    "FixedLengthRecord StandardTransposeMethods TransposeDecimalWithSign(Object, Integer, Integer, String, String) method does not return expected value"
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}