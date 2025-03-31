using System.Text;

namespace ForestNET.Tests.IO
{
    public class FileUnitTest
    {
        [Test]
        public void TestFile()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testFile" + ForestNET.Lib.IO.File.DIR;

                if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                {
                    ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                }

                ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                    Is.True,
                    "directory[" + s_testDirectory + "] does not exist"
                );

                string s_file = s_testDirectory + "file.txt";
                ForestNET.Lib.IO.File o_file = new(s_file, true);
                o_file.AppendLine("First line");
                o_file.AppendLine("Second line");
                o_file.WriteLine("new second line", 2);
                o_file.WriteLine("some line between", 2);
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_file),
                    Is.True,
                    "file[" + s_file + "] does not exist"
                );
                Assert.That(
                    o_file.FileLines, Is.EqualTo(4),
                    "file lines != 4"
                );
                Assert.That(
                    o_file.ReadLine(1), Is.EqualTo("First line"),
                    "line #1 is not 'First line'"
                );
                Assert.That(
                    o_file.ReadLine(2), Is.EqualTo("some line between"),
                    "line #2 is not 'some line between'"
                );
                Assert.That(
                    o_file.ReadLine(3), Is.EqualTo("new second line"),
                    "line #3 is not 'new second line'"
                );
                Assert.That(
                    o_file.ReadLine(4), Is.EqualTo("Second line"),
                    "line #4 is not 'Second line'"
                );

                Assert.That(
                    ForestNET.Lib.IO.File.HasFileExtension(s_file),
                    Is.True,
                    "file[" + s_file + "] has no file extension"
                );

                Assert.That(
                    ForestNET.Lib.IO.File.IsFile(s_file),
                    Is.True,
                    "file[" + s_file + "] is not a file"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.IsFile(s_testDirectory),
                    Is.False,
                    "directory[" + s_testDirectory + "] is a file"
                );

                Assert.That(
                    ForestNET.Lib.IO.File.IsDirectory(s_testDirectory),
                    Is.True,
                    "directory[" + s_testDirectory + "] is not a directory"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.IsDirectory(s_file),
                    Is.False,
                    "file[" + s_file + "] is a directory"
                );

                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_file), Is.EqualTo(64),
                    "file[" + s_file + "] length " + ForestNET.Lib.IO.File.FileLength(s_file) + " != 61"
                );

                o_file.ReplaceLine("First line replace", 1);
                o_file.ReplaceLine("some line between replace", 2);
                Assert.That(
                    o_file.FileLines, Is.EqualTo(4),
                    "file lines != 4"
                );
                Assert.That(
                    o_file.ReadLine(1), Is.EqualTo("First line replace"),
                    "line #1 is not 'First line replace'"
                );
                Assert.That(
                    o_file.ReadLine(2), Is.EqualTo("some line between replace"),
                    "line #2 is not 'some line between replace'"
                );
                Assert.That(
                    o_file.ReadLine(3), Is.EqualTo("new second line"),
                    "line #3 is not 'new second line'"
                );
                Assert.That(
                    o_file.ReadLine(4), Is.EqualTo("Second line"),
                    "line #4 is not 'Second line'"
                );

                o_file.ReplaceContent("Just two lines" + ForestNET.Lib.IO.File.NEWLINE + "And this is the second" + ForestNET.Lib.IO.File.NEWLINE + "Oh!");
                Assert.That(
                    o_file.FileLines, Is.EqualTo(3),
                    "file lines != 3"
                );
                Assert.That(
                    o_file.ReadLine(1), Is.EqualTo("Just two lines"),
                    "line #1 is not 'Just two lines'"
                );
                Assert.That(
                    o_file.ReadLine(2), Is.EqualTo("And this is the second"),
                    "line #2 is not 'And this is the second'"
                );
                Assert.That(
                    o_file.ReadLine(3), Is.EqualTo("Oh!"),
                    "line #3 is not 'Oh!'"
                );

                o_file.DeleteLine(1);
                Assert.That(
                    o_file.FileLines, Is.EqualTo(2),
                    "file lines != 2"
                );
                Assert.That(
                    o_file.ReadLine(1), Is.EqualTo("And this is the second"),
                    "line #1 is not 'And this is the second'"
                );
                Assert.That(
                    o_file.ReadLine(2), Is.EqualTo("Oh!"),
                    "line #2 is not 'Oh!'"
                );

                o_file.TruncateContent();
                Assert.That(
                    o_file.FileLines, Is.EqualTo(0),
                    "file lines != 0"
                );

                o_file.ReplaceContent("First" + ForestNET.Lib.IO.File.NEWLINE + "Second" + ForestNET.Lib.IO.File.NEWLINE + "Third" + ForestNET.Lib.IO.File.NEWLINE + "Fourth" + ForestNET.Lib.IO.File.NEWLINE + "Fifth");
                Assert.That(
                    o_file.FileLines, Is.EqualTo(5),
                    "file lines != 5"
                );
                Assert.That(
                    o_file.ReadLine(1), Is.EqualTo("First"),
                    "line #1 is not 'First'"
                );
                Assert.That(
                    o_file.ReadLine(2), Is.EqualTo("Second"),
                    "line #2 is not 'Second'"
                );
                Assert.That(
                    o_file.ReadLine(3), Is.EqualTo("Third"),
                    "line #3 is not 'Third'"
                );
                Assert.That(
                    o_file.ReadLine(4), Is.EqualTo("Fourth"),
                    "line #4 is not 'Fourth'"
                );
                Assert.That(
                    o_file.ReadLine(5), Is.EqualTo("Fifth"),
                    "line #5 is not 'Fifth'"
                );

                string s_fileNew = s_testDirectory + "fileNew.txt";
                o_file.RenameFile("fileNew.txt");
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_fileNew),
                    Is.True,
                    "file[" + s_fileNew + "] does not exist"
                );

                o_file = new ForestNET.Lib.IO.File(s_fileNew);
                Assert.That(
                    o_file.FileLines, Is.EqualTo(5),
                    "file lines != 5"
                );
                Assert.That(
                    o_file.ReadLine(1), Is.EqualTo("First"),
                    "line #1 is not 'First'"
                );
                Assert.That(
                    o_file.ReadLine(2), Is.EqualTo("Second"),
                    "line #2 is not 'Second'"
                );
                Assert.That(
                    o_file.ReadLine(3), Is.EqualTo("Third"),
                    "line #3 is not 'Third'"
                );
                Assert.That(
                    o_file.ReadLine(4), Is.EqualTo("Fourth"),
                    "line #4 is not 'Fourth'"
                );
                Assert.That(
                    o_file.ReadLine(5), Is.EqualTo("Fifth"),
                    "line #5 is not 'Fifth'"
                );

                Assert.That(
                    o_file.Hash("SHA-512"), Is.EqualTo("85BE5AD52CA9A965C1EAF88FD33E1BEDC6C54E4A020C6EF4C837A2D711282C9CE05B03E6A57238AE44DC2560B177DB68C1179042A0C95D933582DACDBA72EDB1"),
                    "SHA-512 value of file instance is not '85BE5AD52CA9A965C1EAF88FD33E1BEDC6C54E4A020C6EF4C837A2D711282C9CE05B03E6A57238AE44DC2560B177DB68C1179042A0C95D933582DACDBA72EDB1' but '" + o_file.Hash("SHA-512") + "'"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.HashFile(s_fileNew, "SHA-512"), Is.EqualTo("0E953DF5D689063A8D5A8C28A6A64982DDEA748731030E1F1A579FF5800AA0366CC1A2CD5C58D95D20372D60EC05456270ED8C4F1DCB413D9D64D4A166E8FB90"),
                    "SHA-512 value of file is not '0E953DF5D689063A8D5A8C28A6A64982DDEA748731030E1F1A579FF5800AA0366CC1A2CD5C58D95D20372D60EC05456270ED8C4F1DCB413D9D64D4A166E8FB90' but '" + ForestNET.Lib.IO.File.HashFile(s_fileNew, "SHA-512") + "'"
                );

                string s_fileContentFromList = s_testDirectory + "fileContentFromList.txt";
                ForestNET.Lib.IO.File o_fileContentFromList = new(s_fileContentFromList, true);
                List<string> a_lines = [.. new string[] { "one", "two", "three", "four", "five", "six" }];
                o_fileContentFromList.FileContentFromList = a_lines;

                ForestNET.Lib.IO.File o_fileContentFromListCheck = new(s_fileContentFromList);
                Assert.That(
                    o_fileContentFromListCheck.FileLines, Is.EqualTo(6),
                    "file lines != 6"
                );
                Assert.That(
                    o_fileContentFromListCheck.ReadLine(1), Is.EqualTo("one"),
                    "line #1 is not 'one'"
                );
                Assert.That(
                    o_fileContentFromListCheck.ReadLine(2), Is.EqualTo("two"),
                    "line #2 is not 'two'"
                );
                Assert.That(
                    o_fileContentFromListCheck.ReadLine(3), Is.EqualTo("three"),
                    "line #3 is not 'three'"
                );
                Assert.That(
                    o_fileContentFromListCheck.ReadLine(4), Is.EqualTo("four"),
                    "line #4 is not 'four'"
                );
                Assert.That(
                    o_fileContentFromListCheck.ReadLine(5), Is.EqualTo("five"),
                    "line #5 is not 'five'"
                );
                Assert.That(
                    o_fileContentFromListCheck.ReadLine(6), Is.EqualTo("six"),
                    "line #5 is not 'six'"
                );

                ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory + "sub");
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory + "sub"),
                    Is.True,
                    "directory[" + s_testDirectory + "sub" + "] does not exist"
                );

                string s_fileCopy = s_testDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "file.txt";
                ForestNET.Lib.IO.File.CopyFile(s_fileNew, s_fileCopy);
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_fileCopy),
                    Is.True,
                    "file[" + s_fileCopy + "] does not exist"
                );

                ForestNET.Lib.IO.File.MoveFile(s_fileCopy, s_file);
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_file),
                    Is.True,
                    "file[" + s_file + "] does not exist"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_fileCopy),
                    Is.False,
                    "file[" + s_fileCopy + "] does exist"
                );

                ForestNET.Lib.IO.File.DeleteFile(s_file);
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_file),
                    Is.False,
                    "file[" + s_file + "] does exist"
                );

                ForestNET.Lib.IO.File.DeleteFile(s_fileNew);
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_fileNew),
                    Is.False,
                    "file[" + s_fileNew + "] does exist"
                );

                o_file = new ForestNET.Lib.IO.File(s_testDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "file1.txt", true);
                o_file.AppendLine("1");
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_testDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "file1.txt"),
                    Is.True,
                    "file[" + s_testDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "file1.txt" + "] does not exist"
                );

                o_file = new ForestNET.Lib.IO.File(s_testDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "file2.txt", true);
                o_file.AppendLine("2");
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_testDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "file2.txt"),
                    Is.True,
                    "file[" + s_testDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "file2.txt" + "] does not exist"
                );

                o_file = new ForestNET.Lib.IO.File(s_testDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "file3.txt", true);
                o_file.AppendLine("3");
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_testDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "file3.txt"),
                    Is.True,
                    "file[" + s_testDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "file3.txt" + "] does not exist"
                );

                ForestNET.Lib.IO.File.CopyDirectory(s_testDirectory + "sub", s_testDirectory + "copy");
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_testDirectory + "copy" + ForestNET.Lib.IO.File.DIR + "file1.txt"),
                    Is.True,
                    "file[" + s_testDirectory + "copy" + ForestNET.Lib.IO.File.DIR + "file1.txt" + "] does not exist"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_testDirectory + "copy" + ForestNET.Lib.IO.File.DIR + "file2.txt"),
                    Is.True,
                    "file[" + s_testDirectory + "copy" + ForestNET.Lib.IO.File.DIR + "file2.txt" + "] does not exist"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_testDirectory + "copy" + ForestNET.Lib.IO.File.DIR + "file3.txt"),
                    Is.True,
                    "file[" + s_testDirectory + "copy" + ForestNET.Lib.IO.File.DIR + "file3.txt" + "] does not exist"
                );

                ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory + "dest");
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR),
                    Is.True,
                    "directory[" + s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "] does not exist"
                );

                ForestNET.Lib.IO.File.MoveDirectory(s_testDirectory + "copy", s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "copy");
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory + "copy" + ForestNET.Lib.IO.File.DIR),
                    Is.False,
                    "directory[" + s_testDirectory + "copy" + ForestNET.Lib.IO.File.DIR + "] does exist"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "copy" + ForestNET.Lib.IO.File.DIR),
                    Is.True,
                    "directory[" + s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "copy" + ForestNET.Lib.IO.File.DIR + "] does not exist"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "copy" + ForestNET.Lib.IO.File.DIR + "file1.txt"),
                    Is.True,
                    "file[" + s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "copy" + ForestNET.Lib.IO.File.DIR + "file1.txt" + "] does not exist"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "copy" + ForestNET.Lib.IO.File.DIR + "file2.txt"),
                    Is.True,
                    "file[" + s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "copy" + ForestNET.Lib.IO.File.DIR + "file2.txt" + "] does not exist"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "copy" + ForestNET.Lib.IO.File.DIR + "file3.txt"),
                    Is.True,
                    "file[" + s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "copy" + ForestNET.Lib.IO.File.DIR + "file3.txt" + "] does not exist"
                );

                o_file = new ForestNET.Lib.IO.File(s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "fileAlone.txt", true);
                o_file.AppendLine("alone");

                Assert.That(
                    ForestNET.Lib.IO.File.HashDirectory(s_testDirectory + "dest", "SHA-256"), Is.EqualTo("47EB51F861A98F9E15F493E2D5EDF829EF403EDBF6A006C40A8743952FC0CFFE"),
                    "SHA-256 value of directory is not '47EB51F861A98F9E15F493E2D5EDF829EF403EDBF6A006C40A8743952FC0CFFE' but '" + ForestNET.Lib.IO.File.HashDirectory(s_testDirectory + "dest", "SHA-256") + "'"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.HashDirectory(s_testDirectory + "dest", "SHA-256", true), Is.EqualTo("A7A048FE2D6A3C32E3A60C0344E4C5DA6F284A68798BC6AF21C8C865F4D8C54F"),
                    "SHA-256 value of directory and sub-directories is not 'A7A048FE2D6A3C32E3A60C0344E4C5DA6F284A68798BC6AF21C8C865F4D8C54F' but '" + ForestNET.Lib.IO.File.HashDirectory(s_testDirectory + "dest", "SHA-256", true) + "'"
                );

                int i = 0;

                foreach (ForestNET.Lib.IO.ListingElement o_listingElement in ForestNET.Lib.IO.File.ListDirectory(s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "copy"))
                {
                    if (i == 0)
                    {
                        Assert.That(
                            o_listingElement.Name, Is.EqualTo("file1.txt"),
                            "first element in list has not the name 'file1.txt'"
                        );
                    }
                    else if (i == 1)
                    {
                        Assert.That(
                            o_listingElement.Name, Is.EqualTo("file2.txt"),
                            "first element in list has not the name 'file2.txt'"
                        );
                    }
                    else if (i == 3)
                    {
                        Assert.That(
                            o_listingElement.Name, Is.EqualTo("file3.txt"),
                            "first element in list has not the name 'file3.txt'"
                        );
                    }

                    i++;
                }

                ForestNET.Lib.IO.File.RenameDirectory(s_testDirectory + "dest", s_testDirectory + "destRename");
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory + "destRename" + ForestNET.Lib.IO.File.DIR),
                    Is.True,
                    "directory[" + s_testDirectory + "destRename" + ForestNET.Lib.IO.File.DIR + "] does not exist"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR),
                    Is.False,
                    "directory[" + s_testDirectory + "dest" + ForestNET.Lib.IO.File.DIR + "] does exist"
                );

                StringBuilder o_stringBuilder = new();
                o_stringBuilder.Append("one" + ForestNET.Lib.IO.File.NEWLINE + "two" + ForestNET.Lib.IO.File.NEWLINE + "three" + ForestNET.Lib.IO.File.NEWLINE + "four");
                ForestNET.Lib.IO.File.ReplaceFileContent(s_testDirectory + "destRename" + ForestNET.Lib.IO.File.DIR + "fileAlone.txt", o_stringBuilder);
                o_file = new(s_testDirectory + "destRename" + ForestNET.Lib.IO.File.DIR + "fileAlone.txt");
                Assert.That(
                    o_file.FileLines, Is.EqualTo(4),
                    "file lines != 4"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_testDirectory + "destRename" + ForestNET.Lib.IO.File.DIR + "fileAlone.txt"), Is.EqualTo(24),
                    "file length " + ForestNET.Lib.IO.File.FileLength(s_testDirectory + "destRename" + ForestNET.Lib.IO.File.DIR + "fileAlone.txt") + " != 21"
                );

                o_file = new ForestNET.Lib.IO.File(s_testDirectory + "fileContent.txt", true);
                ForestNET.Lib.IO.File.ReplaceFileContent(s_testDirectory + "fileContent.txt", ForestNET.Lib.IO.File.GenerateRandomFileContent_1KB(), Encoding.ASCII);
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_testDirectory + "fileContent.txt"), Is.EqualTo(1024),
                    "file length != 1024"
                );

                ForestNET.Lib.IO.File.ReplaceFileContent(s_testDirectory + "fileContent.txt", ForestNET.Lib.IO.File.GenerateRandomFileContent_1MB(), Encoding.ASCII);
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_testDirectory + "fileContent.txt"), Is.EqualTo(1048576),
                    "file length != 1048576"
                );

                ForestNET.Lib.IO.File.ReplaceFileContent(s_testDirectory + "fileContent.txt", ForestNET.Lib.IO.File.GenerateRandomFileContent_10MB(), Encoding.ASCII);
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_testDirectory + "fileContent.txt"), Is.EqualTo(10485760),
                    "file length != 10485760"
                );

                ForestNET.Lib.IO.File.ReplaceFileContent(s_testDirectory + "fileContent.txt", ForestNET.Lib.IO.File.GenerateRandomFileContent_50MB(), Encoding.ASCII);
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_testDirectory + "fileContent.txt"), Is.EqualTo(52428800),
                    "file length != 52428800"
                );

                long l_length = (long)ForestNET.Lib.Helper.RandomIntegerRange(1048576, 10485760);
                ForestNET.Lib.IO.File.ReplaceFileContent(s_testDirectory + "fileContent.txt", ForestNET.Lib.IO.File.GenerateRandomFileContent(l_length), Encoding.ASCII);
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_testDirectory + "fileContent.txt"), Is.EqualTo(l_length),
                    "file length != " + l_length
                );

                l_length = (long)ForestNET.Lib.Helper.RandomIntegerRange(1048576, 10485760);
                int i_lineLength = ForestNET.Lib.Helper.RandomIntegerRange(128, 512);
                ForestNET.Lib.IO.File.ReplaceFileContent(s_testDirectory + "fileContent.txt", ForestNET.Lib.IO.File.GenerateRandomFileContent(i_lineLength, l_length), Encoding.ASCII);
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_testDirectory + "fileContent.txt"), Is.EqualTo(l_length),
                    "file length != " + l_length
                );

                ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                    Is.False,
                    "directory[" + s_testDirectory + "] does exist"
                );

                if (ForestNET.Lib.IO.File.FolderExists(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testFileFolderStructure" + ForestNET.Lib.IO.File.DIR))
                {
                    ForestNET.Lib.IO.File.DeleteDirectory(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testFileFolderStructure");
                }

                ForestNET.Lib.IO.File.CreateHexFileFolderStructure(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testFileFolderStructure");
                Assert.That(
                    ForestNET.Lib.IO.File.ListDirectory(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testFileFolderStructure"), Has.Count.EqualTo(256),
                    "folder amount in file folder structure != 256"
                );

                ForestNET.Lib.IO.File.DeleteDirectory(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testFileFolderStructure");
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testFileFolderStructure" + ForestNET.Lib.IO.File.DIR),
                    Is.False,
                    "directory[" + s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testFileFolderStructure" + ForestNET.Lib.IO.File.DIR + "] does exist"
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}
