namespace Sandbox.Tests.AI
{
    public class MNISTHandling
    {

        /* Fields */

        private Stream? o_inputStreamFromFileImage = null;
        private Stream? o_inputStreamFromFileLabel = null;

        private int i_index = -1;

        /* Properties */

        public int Amount { get; private set; }
        public bool HasNext
        {
            get
            {
                return this.i_index + 1 < this.Amount;
            }
        }
        public int ImageWidth { get; private set; }
        public int ImageHeight { get; private set; }
        public byte[] CurrentImageData { get; private set; }
        public byte CurrentLabelValue { get; private set; }
        private int AmountImage { get; set; }
        private int AmountLabel { get; set; }

        /* Methods */

        public MNISTHandling(string p_o_fileImage, string p_o_fileLabel)
        {
            this.CurrentImageData = [];

            this.OpenFileImage(p_o_fileImage, 2051);

            /* read image height + width of all records */
            this.ImageHeight = Read_u_int32(this.o_inputStreamFromFileImage ?? throw new NullReferenceException("file image stream is null"));
            this.ImageWidth = Read_u_int32(this.o_inputStreamFromFileImage ?? throw new NullReferenceException("file image stream is null"));

            //Console.WriteLine(p_o_fileImage.Substring(p_o_fileImage.LastIndexOf(ForestNET.Lib.IO.File.DIR)) + " contains " + this.AmountImage + " images with " + this.ImageWidth + "x" + this.ImageHeight + " pixels");

            this.OpenFileLabel(p_o_fileLabel, 2049);

            //Console.WriteLine(p_o_fileImage.Substring(p_o_fileImage.LastIndexOf(ForestNET.Lib.IO.File.DIR)) + " contains " + this.AmountLabel + " labels" );

            if (this.AmountImage != this.AmountLabel)
            {
                throw new ApplicationException("Amount of images[" + this.AmountImage + "] differs from amount of labels[" + this.AmountLabel + "]");
            }
            else
            {
                this.Amount = this.AmountImage;
            }
        }

        private void OpenFileImage(string p_o_file, int p_i_magic)
        {
            /* open file input streams */
            if (p_o_file.EndsWith(".gz"))
            {
                this.o_inputStreamFromFileImage = new System.IO.Compression.GZipStream(File.OpenRead(p_o_file), System.IO.Compression.CompressionMode.Decompress);
            }
            else
            {
                this.o_inputStreamFromFileImage = File.OpenRead(p_o_file);
            }

            /* check header(magic number) */
            if (Read_u_int32(this.o_inputStreamFromFileImage) != p_i_magic)
            {
                throw new System.IO.IOException("Header(magic number) is invalid");
            }

            /* read amount of records */
            this.AmountImage = Read_u_int32(this.o_inputStreamFromFileImage);
        }

        private void OpenFileLabel(string p_o_file, int p_i_magic)
        {
            /* open file input streams */
            if (p_o_file.EndsWith(".gz"))
            {
                this.o_inputStreamFromFileLabel = new System.IO.Compression.GZipStream(File.OpenRead(p_o_file), System.IO.Compression.CompressionMode.Decompress);
            }
            else
            {
                this.o_inputStreamFromFileLabel = File.OpenRead(p_o_file);
            }

            /* check header(magic number) */
            if (Read_u_int32(this.o_inputStreamFromFileLabel) != p_i_magic)
            {
                throw new System.IO.IOException("Header(magic number) is invalid");
            }

            /* read amount of records */
            this.AmountLabel = Read_u_int32(this.o_inputStreamFromFileLabel);
        }

        public void Close()
        {
            this.o_inputStreamFromFileLabel?.Close();
            this.o_inputStreamFromFileImage?.Close();
        }

        private void IncrementtIndex()
        {
            if (!this.HasNext)
            {
                throw new System.IO.IOException("cannot increment index - eof");
            }

            this.i_index++;
        }

        private static int Read_u_int32(Stream? p_o_inputStream)
        {
            /* read u_int32 with 4 bytes */
            byte[] by_temp = Read(p_o_inputStream, new byte[4]);

            /* we must reverse byte order to read u_int32 to int value */
            return
                (((int)by_temp[0]) & 0xFF) << 24 |
                (((int)by_temp[1]) & 0xFF) << 16 |
                (((int)by_temp[2]) & 0xFF) << 8 |
                (((int)by_temp[3]) & 0xFF);
        }

        private static byte[] Read(Stream? p_o_inputStream, byte[] data)
        {
            int i = 0;

            if (p_o_inputStream == null)
            {
                throw new System.IO.IOException("input stream is null");
            }

            /* read data from file stream, until all bytes have been set */
            do
            {
                int i_length = p_o_inputStream.Read(data, i, data.Length - i);

                /* check return value */
                if (i_length < 1)
                {
                    throw new System.IO.IOException("cannot read data - eof");
                }

                i += i_length;
            }
            while (i < data.Length);

            return data;
        }

        public byte[] GetDataAsBinaryArray(byte[] p_a_values)
        {
            byte[] a_return = new byte[p_a_values.Length];
            int i_index = 0;

            for (int y = 0; y < this.ImageHeight; y++)
            {
                for (int x = 0; x < this.ImageWidth; x++, i_index++)
                {
                    if ((255 - (((int)p_a_values[i_index]) & 0xFF)) >= 255)
                    {
                        a_return[i_index] = 0;
                    }
                    else
                    {
                        a_return[i_index] = 1;
                    }
                }
            }

            return a_return;
        }


        public void SelectNext()
        {
            this.IncrementtIndex();

            /* read next image data record */
            this.CurrentImageData = Read(this.o_inputStreamFromFileImage, new byte[this.ImageWidth * this.ImageHeight]);

            /* read label with one byte */
            this.CurrentLabelValue = Read(this.o_inputStreamFromFileLabel, new byte[1])[0];
        }
    }
}