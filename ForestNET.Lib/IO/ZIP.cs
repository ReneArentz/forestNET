using System.IO.Compression;

namespace ForestNET.Lib.IO
{
    /// <summary>
    /// ZIP class with static methods to get size of an archive or check archive if its content is valid.
    /// can zip and unzip archives, only supporting zip format.
    /// </summary>
    public class ZIP
    {

        /* Delegates */

        /// <summary>
        /// delegate definition which can be instanced outside of ZIP class to post progress anywhere of ZIP methods
        /// </summary>
        public delegate void PostProgress(double p_d_progress);

        /* Fields */

        /* Properties */

        /* Methods */

        /// <summary>
        /// get uncompressed size of an zip archive file
        /// </summary>
        /// <param name="p_s_sourceLocation">full path to zip archive file</param>
        /// <returns>byte length of uncompressed archive file content as long value</returns>
        /// <exception cref="ArgumentException">full path does not end with '.zip' or file does not exist</exception>
        /// <exception cref="System.IO.IOException">error reading zip archive content</exception>
        /// <exception cref="InvalidOperationException">could not close zip input stream</exception>
        public static long GetSize(string p_s_sourceLocation)
        {
            long l_return = 0;

            /* check if source location ends with '.zip' */
            if (!p_s_sourceLocation.EndsWith(".zip"))
            {
                throw new ArgumentException("Invalid source location[" + p_s_sourceLocation + "] - must end with '.zip'");
            }

            /* check if source location really exists */
            if (!File.Exists(p_s_sourceLocation))
            {
                throw new ArgumentException("Source location[" + p_s_sourceLocation + "] does not exist");
            }

            /* stream to read out of zip file, create input stream instance */
            using (ZipArchive o_zipInputStream = new(System.IO.File.OpenRead(p_s_sourceLocation), ZipArchiveMode.Read))
            {
                /* iterate all files within zip */
                foreach (ZipArchiveEntry o_zipEntry in o_zipInputStream.Entries)
                {
                    ForestNET.Lib.Global.ILogMass("file length of '" + o_zipEntry.Name + "' = " + ForestNET.Lib.Helper.FormatBytes(o_zipEntry.Length));
                    l_return += o_zipEntry.Length;
                }
            }

            return l_return;
        }

        /// <summary>
        /// check an zip archive file if it's content is valid
        /// </summary>
        /// <param name="p_s_sourceLocation">full path to zip archive file</param>
        /// <returns>bool, true - archive is valid, false - archive is invalid</returns>
        /// <exception cref="ArgumentException">full path does not end with '.zip' or file does not exist</exception>
        /// <exception cref="System.IO.IOException">error reading zip archive content</exception>
        /// <exception cref="InvalidOperationException">could not close zip input stream or zip file object</exception>
        public static bool CheckArchive(string p_s_sourceLocation)
        {
            return ZIP.CheckArchive(p_s_sourceLocation, null);
        }

        /// <summary>
        /// check an zip archive file if it's content is valid
        /// </summary>
        /// <param name="p_s_sourceLocation">full path to zip archive file</param>
        /// <param name="p_del_postProgress">delegate to post progress of check archive method</param>
        /// <returns>bool, true - archive is valid, false - archive is invalid</returns>
        /// <exception cref="ArgumentException">full path does not end with '.zip' or file does not exist</exception>
        /// <exception cref="System.IO.IOException">error reading zip archive content</exception>
        /// <exception cref="InvalidOperationException">could not close zip input stream or zip file object</exception>
        public static bool CheckArchive(string p_s_sourceLocation, ZIP.PostProgress? p_del_postProgress)
        {
            bool b_return = true;

            /* check if source location ends with '.zip' */
            if (!p_s_sourceLocation.EndsWith(".zip"))
            {
                throw new ArgumentException("Source location[" + p_s_sourceLocation + "] must end with '.zip'");
            }

            /* check if source location really exists */
            if (!ForestNET.Lib.IO.File.Exists(p_s_sourceLocation))
            {
                throw new ArgumentException("Source location[" + p_s_sourceLocation + "] does not exist");
            }

            try
            {
                /* stream to read out of zip file */
                using ZipArchive o_zipInputStream = new(System.IO.File.OpenRead(p_s_sourceLocation), ZipArchiveMode.Read);
                /* variable for uncompressed zip archive file size  */
                long l_overallSum = 0;

                /* get size of zip file container content, but only if we need to show progress - otherwise it is wasting resources */
                if (p_del_postProgress != null)
                {
                    l_overallSum = ZIP.GetSize(p_s_sourceLocation);
                }

                /* variable for sum of all read bytes in zip */
                long l_sum = 0;

                /* iterate all files within zip */
                foreach (ZipArchiveEntry o_zipEntry in o_zipInputStream.Entries)
                {
                    /* help variables to read and write stream instances */
                    byte[] a_buffer = new byte[8192];
                    int i_length;

                    ForestNET.Lib.Global.ILogMass("start checking file in archive '" + o_zipEntry.Name + "'");

                    /* read from zip entry stream */
                    using (System.IO.Stream o_stream = o_zipEntry.Open())
                    {
                        while ((i_length = o_stream.Read(a_buffer, 0, a_buffer.Length)) > 0)
                        {
                            if (p_del_postProgress != null)
                            {
                                l_sum += i_length;

                                /* post progress of check archive method */
                                p_del_postProgress.Invoke((double)l_sum / l_overallSum);
                            }
                        }
                    }

                    ForestNET.Lib.Global.ILogMass("finished checking file in archive '" + o_zipEntry.Name + "'");
                }
            }
            catch (Exception)
            {
                /* some failure happened during reading archive, so we set our return value to false */
                b_return = false;
            }
            return b_return;
        }

        /// <summary>
        /// zip a source file or directory location to an archive file
        /// </summary>
        /// <param name="p_s_sourceLocation">full path to source file/directory location</param>
        /// <param name="p_s_destinationLocation">full path to destination zip archive file</param>
        /// <exception cref="ArgumentException">full path does not end with '.zip'</exception>
        /// <exception cref="System.IO.IOException">error creating zip archive</exception>
        /// <exception cref="InvalidOperationException">could not close zip input stream or zip file object</exception>
        public static void Zip(string p_s_sourceLocation, string p_s_destinationLocation)
        {
            ZIP.Zip(p_s_sourceLocation, p_s_destinationLocation, CompressionLevel.Optimal, null);
        }

        /// <summary>
        /// zip a source file or directory location to an archive file
        /// </summary>
        /// <param name="p_s_sourceLocation">full path to source file/directory location</param>
        /// <param name="p_s_destinationLocation">full path to destination zip archive file</param>
        /// <param name="p_o_compressionLevel">specify .NET compression level, standard is 'optimal'</param>
        /// <param name="p_del_postProgress">delegate to post progress of zip method</param>
        /// <exception cref="ArgumentException">full path does not end with '.zip'</exception>
        /// <exception cref="System.IO.IOException">error creating zip archive</exception>
        /// <exception cref="InvalidOperationException">could not close zip input stream or zip file object</exception>
        public static void Zip(string p_s_sourceLocation, string p_s_destinationLocation, CompressionLevel p_o_compressionLevel = CompressionLevel.Optimal, ZIP.PostProgress? p_del_postProgress = null)
        {
            /* check if destination location ends with '.zip' */
            if (!p_s_destinationLocation.EndsWith(".zip"))
            {
                throw new ArgumentException("Destination location[" + p_s_destinationLocation + "] must end with '.zip'");
            }

            /* stream to write into zip file */
            using ZipArchive o_zipOutputStream = new(System.IO.File.Create(p_s_destinationLocation), ZipArchiveMode.Update);
            /* variable for sum of all bytes which will be zipped */
            long l_overallSum = 0;

            /* only sum up all bytes if we want progress, otherwise it's wasting resources */
            if (p_del_postProgress != null)
            {
                foreach (ListingElement o_listingElement in ForestNET.Lib.IO.File.ListDirectory(p_s_sourceLocation, true))
                {
                    /* add file size to sum */
                    l_overallSum += o_listingElement.Size;
                }
            }

            /* variable for sum of all read bytes in zip */
            long l_sum = 0;

            /* iterate file or all files of a directory */
            foreach (ListingElement o_listingElement in ForestNET.Lib.IO.File.ListDirectory(p_s_sourceLocation, true))
            {
                /* skip directory listing elements */
                if (o_listingElement.IsDirectory)
                {
                    continue;
                }

                /* skip zip file we want to create, if it is in our source location itself or if full name is just null */
                if ((o_listingElement.FullName == null) || (o_listingElement.FullName.Equals(p_s_destinationLocation)))
                {
                    continue;
                }

                /* create a new zip entry with filename */
                ZipArchiveEntry o_zipEntry = o_zipOutputStream.CreateEntry(o_listingElement.FullName.Substring(p_s_sourceLocation.LastIndexOf(ForestNET.Lib.IO.File.DIR) + 1), p_o_compressionLevel);

                ForestNET.Lib.Global.ILogMass("start zipping file to archive '" + o_zipEntry.Name + "'");

                /* create file stream and input stream instance for zipping a file */
                using (System.IO.FileStream o_fileStream = System.IO.File.OpenRead(o_listingElement.FullName))
                using (System.IO.Stream o_stream = o_zipEntry.Open())
                {

                    /* help variables to read and write stream instances */
                    byte[] a_buffer = new byte[8192];
                    int i_length;

                    /* read from input file stream */
                    while ((i_length = o_fileStream.Read(a_buffer, 0, a_buffer.Length)) > 0)
                    {
                        /* write in output zip stream */
                        o_stream.Write(a_buffer, 0, i_length);

                        if (p_del_postProgress != null)
                        {
                            l_sum += i_length;

                            /* post progress of zip archive method */
                            p_del_postProgress.Invoke((double)l_sum / l_overallSum);
                        }
                    }
                }

                ForestNET.Lib.Global.ILogMass("finished zipping file to archive '" + o_zipEntry.Name + "'");
            }
        }

        /// <summary>
        /// unzip a source zip file archive to a destination location, does not create destination path or delete source zip archive file
        /// </summary>
        /// <param name="p_s_sourceLocation">full path to source zip archive file</param>
        /// <param name="p_s_destinationLocation">full path to destination file/directory location</param>
        /// <exception cref="ArgumentException">source location does not end with '.zip' or does not exist, destination location does not exist or is not a directory</exception>
        /// <exception cref="System.IO.IOException">error uncompressing content of source zip archive</exception>
        /// <exception cref="InvalidOperationException">could not close zip input stream or zip file object</exception>
        public static void Unzip(string p_s_sourceLocation, string p_s_destinationLocation)
        {
            ZIP.Unzip(p_s_sourceLocation, p_s_destinationLocation, false, false, null);
        }

        /// <summary>
        /// unzip a source zip file archive to a destination location, does not create destination path or delete source zip archive file
        /// </summary>
        /// <param name="p_s_sourceLocation">full path to source zip archive file</param>
        /// <param name="p_s_destinationLocation">full path to destination file/directory location</param>
        /// <param name="p_del_postProgress">delegate to post progress of zip method</param>
        /// <exception cref="ArgumentException">source location does not end with '.zip' or does not exist, destination location does not exist or is not a directory</exception>
        /// <exception cref="System.IO.IOException">error uncompressing content of source zip archive</exception>
        /// <exception cref="InvalidOperationException">could not close zip input stream or zip file object</exception>
        public static void Unzip(string p_s_sourceLocation, string p_s_destinationLocation, ZIP.PostProgress? p_del_postProgress)
        {
            ZIP.Unzip(p_s_sourceLocation, p_s_destinationLocation, false, false, p_del_postProgress);
        }

        /// <summary>
        /// unzip a source zip file archive to a destination location
        /// </summary>
        /// <param name="p_s_sourceLocation">full path to source zip archive file</param>
        /// <param name="p_s_destinationLocation">full path to destination file/directory location</param>
        /// <param name="p_b_createDestinationPath">flag to create destination path automatically</param>
        /// <param name="p_b_deleteSourceLocation">flag to delete zip file after content has been uncompressed</param>
        /// <exception cref="ArgumentException">source location does not end with '.zip' or does not exist, destination location does not exist or is not a directory</exception>
        /// <exception cref="System.IO.IOException">error uncompressing content of source zip archive</exception>
        /// <exception cref="InvalidOperationException">could not close zip input stream or zip file object</exception>
        public static void Unzip(string p_s_sourceLocation, string p_s_destinationLocation, bool p_b_createDestinationPath, bool p_b_deleteSourceLocation)
        {
            ZIP.Unzip(p_s_sourceLocation, p_s_destinationLocation, p_b_createDestinationPath, p_b_deleteSourceLocation, null);
        }

        /// <summary>
        /// unzip a source zip file archive to a destination location
        /// </summary>
        /// <param name="p_s_sourceLocation">full path to source zip archive file</param>
        /// <param name="p_s_destinationLocation">full path to destination file/directory location</param>
        /// <param name="p_b_createDestinationPath">flag to create destination path automatically</param>
        /// <param name="p_b_deleteSourceLocation">flag to delete zip file after content has been uncompressed</param>
        /// <param name="p_del_postProgress">delegate to post progress of zip method</param>
        /// <exception cref="ArgumentException">source location does not end with '.zip' or does not exist, destination location does not exist or is not a directory</exception>
        /// <exception cref="System.IO.IOException">error uncompressing content of source zip archive</exception>
        /// <exception cref="InvalidOperationException">could not close zip input stream or zip file object</exception>
        public static void Unzip(string p_s_sourceLocation, string p_s_destinationLocation, bool p_b_createDestinationPath, bool p_b_deleteSourceLocation, ZIP.PostProgress? p_del_postProgress)
        {
            /* check if source location ends with '.zip' */
            if (!p_s_sourceLocation.EndsWith(".zip"))
            {
                throw new ArgumentException("Source location[" + p_s_sourceLocation + "] must end with '.zip'");
            }

            /* check if source location really exists */
            if (!ForestNET.Lib.IO.File.Exists(p_s_sourceLocation))
            {
                throw new ArgumentException("Source location[" + p_s_sourceLocation + "] does not exist");
            }

            /* check if destination location really exists */
            if (!ForestNET.Lib.IO.File.FolderExists(p_s_destinationLocation))
            {
                /* if create flag has not been set, throw exception */
                if (!p_b_createDestinationPath)
                {
                    throw new ArgumentException("Destination location[" + p_s_destinationLocation + "] does not exist");
                }
                else
                {
                    /* otherwise create destination location */
                    ForestNET.Lib.IO.File.CreateDirectory(p_s_destinationLocation);
                }
            }

            /* check if destination location is a directory */
            if (!ForestNET.Lib.IO.File.IsDirectory(p_s_destinationLocation))
            {
                throw new ArgumentException("Destination location[" + p_s_destinationLocation + "] must be a 'directory'");
            }

            /* zip archive instance to read out of zip file */
            using (ZipArchive o_zipInputStream = new(System.IO.File.OpenRead(p_s_sourceLocation), ZipArchiveMode.Read))
            {
                /* variable for uncompressed zip archive file size  */
                long l_overallSum = 0;

                /* get size of zip file container content, but only if we need to show progress - otherwise it is wasting resources */
                if (p_del_postProgress != null)
                {
                    l_overallSum = ZIP.GetSize(p_s_sourceLocation);
                }

                /* variable for sum of all read bytes in zip */
                long l_sum = 0;

                /* iterate all files within zip */
                foreach (ZipArchiveEntry o_zipEntry in o_zipInputStream.Entries)
                {
                    /* get zip entry path */
                    string? s_pathParentFolder = System.IO.Path.GetDirectoryName(System.IO.Path.Combine(p_s_destinationLocation, o_zipEntry.FullName));

                    /* check if path exists */
                    if ((s_pathParentFolder != null) && (!ForestNET.Lib.IO.File.FolderExists(s_pathParentFolder)))
                    {
                        /* create path */
                        ForestNET.Lib.IO.File.CreateDirectory(s_pathParentFolder);
                    }

                    /* our file stream instance where we unzip file data */
                    System.IO.FileStream o_fileStream;

                    try
                    {
                        /* check if destination file can be created */
                        using (o_fileStream = System.IO.File.OpenWrite(System.IO.Path.Combine(p_s_destinationLocation, o_zipEntry.FullName)))
                        {
                            /* help variables to read and write stream instances */
                            byte[] a_buffer = new byte[8192];
                            int i_length;

                            ForestNET.Lib.Global.ILogMass("start unzipping file from archive '" + o_zipEntry.Name + "'");

                            /* read from zip entry stream */
                            using (System.IO.Stream o_stream = o_zipEntry.Open())
                            {
                                while ((i_length = o_stream.Read(a_buffer, 0, a_buffer.Length)) > 0)
                                {
                                    /* write in output file stream */
                                    o_fileStream.Write(a_buffer, 0, i_length);

                                    if (p_del_postProgress != null)
                                    {
                                        l_sum += i_length;

                                        /* post progress of unzip archive method */
                                        p_del_postProgress.Invoke((double)l_sum / l_overallSum);
                                    }
                                }
                            }

                            ForestNET.Lib.Global.ILogMass("finished unzipping file from archive '" + o_zipEntry.Name + "'");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        throw new System.IO.IOException("Could not create file[" + p_s_destinationLocation + ForestNET.Lib.IO.File.DIR + o_zipEntry.Name + "]; " + o_exc.Message);
                    }
                }
            }

            /* check if we should delete source location */
            if (p_b_deleteSourceLocation)
            {
                /* delete source location */
                ForestNET.Lib.IO.File.DeleteFile(p_s_sourceLocation);
            }
        }
    }
}
