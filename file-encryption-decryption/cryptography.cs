using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace FileEncryptDecrypt
{
    public partial class Form1 : Form
    {
        // Initialize components ///////////////////////////////////////////////////////////////////
        public Form1()
        {
            InitializeComponent();
        }

        // Browse button click /////////////////////////////////////////////////////////////////////
        private void BrowseButtonClick(object sender, EventArgs e)
        {
            if (BrowseFileDialog.ShowDialog(this) == DialogResult.OK)   // Open File Dialog >> Click OK
                FileNameTextBox.Text = BrowseFileDialog.FileName;       // Copy filepath to FileNameTextBox
        }

        // Encrypt button click ////////////////////////////////////////////////////////////////////
        private void EncryptButtonClick(object sender, EventArgs e)
        {
            CryptoHandler("encrypt");
        }

        // Decrypt button click ////////////////////////////////////////////////////////////////////
        private void DecryptButtonClick(object sender, EventArgs e)
        {
            CryptoHandler("decrypt");
        }

        // Cryptographic Handler ///////////////////////////////////////////////////////////////////
        private void CryptoHandler(string mode)
        {
            try
            {
                // Declare variables ===============================================================
                string FileName = FileNameTextBox.Text;                 // File Name and Path
                bool FileExists = false;                                // Does it exist?
                var Overwrite = DialogResult.No;                        // If so, overwrite?

                // Error handling ==================================================================
                if (KeyTextBox.Text == "")                              // Tests for missing key
                    throw new ArgumentException("Please enter a key.");
                if (mode == "decrypt")                                  // Tests if file being decrypted ends in .des
                    if (!FileName.EndsWith(".des"))
                        throw new ArgumentException("Not a .des file.");
                if (FileName == "")                                     // Tests for empty file path
                    throw new FileNotFoundException();
                if (mode == "encrypt")                                  // Test if destination file exists
                    if (File.Exists(FileName + ".des"))
                    {
                        FileExists = true;
                        Overwrite = MessageBox.Show("Output file exists. Overwrite?", "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    }
                if (mode == "decrypt")                                  // Tests if destination file exists
                    if (File.Exists(FileName.Remove(FileName.Length - 4)))
                    {
                        FileExists = true;
                        Overwrite = MessageBox.Show("Output file exists. Overwrite?", "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    }
                if (!FileExists | (FileExists & (Overwrite == DialogResult.Yes)))   // If file doesn't exist or it does exist but overwrite authorized
                {
                    if (mode == "encrypt")                              // Execute encryption if mode is selected
                        EncryptFile(FileName, FileName + ".des", KeyTextBox.Text);
                    if (mode == "decrypt")                              // Execute decryption if mode is selected
                        DecryptFile(FileName, FileName.Remove(FileName.Length - 4), KeyTextBox.Text);
                }
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Could not open source or destination file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Bad key or file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Encrypt file ////////////////////////////////////////////////////////////////////////////
        static void EncryptFile(string InputFilename, string OutputFilename, string Key)
        {
            FileStream fsOriginal = new FileStream(InputFilename, FileMode.Open, FileAccess.Read);      // Open file
            FileStream fsEncrypted = new FileStream(OutputFilename, FileMode.Create, FileAccess.Write); // Create file
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();      // Instantiate the DESCryptoServiceProvider class
            byte[] RawKey = UnicodeEncoding.Unicode.GetBytes(Key);              // Convert the Key into raw bytes
            byte[] EightByteKey = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };        // Initialize a key which will be copied into DES.Key and DES.IV
            for (int i = 0; i < RawKey.Length; i += 2)                          // Iterate through the RawKey
                EightByteKey[(i / 2) % 8] += RawKey[i];                         // Copy byte values to the 8 byte key array
            DES.Key = EightByteKey;                                             // Copy 8 byte key into DES.Key
            DES.IV = EightByteKey;                                              // Copy 8 byte key into DES.IV
            ICryptoTransform Encryptor = DES.CreateEncryptor();                 // Instantiate Encryptor
            CryptoStream cStream = new CryptoStream(fsEncrypted, Encryptor, CryptoStreamMode.Write);    // Encrypted Stream
            byte[] InputArray = new byte[fsOriginal.Length];                    // Create an array to hold the input in bytes
            fsOriginal.Read(InputArray, 0, InputArray.Length);                  // Read the input in bytes
            cStream.Write(InputArray, 0, InputArray.Length);                    // Write to the output file
            cStream.Close();                                                    // Closes the CryptoStream
            fsOriginal.Close();                                                 // Closes the original file stream
            fsEncrypted.Close();                                                // Closes the encrypted file stream
        }

        // Decrypt file ////////////////////////////////////////////////////////////////////////////
        static void DecryptFile(string InputFilename, string OutputFilename, string Key)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();      // Instantiate the DESCryptoServiceProvider class
            byte[] RawKey = UnicodeEncoding.Unicode.GetBytes(Key);              // Convert the Key into raw bytes
            byte[] EightByteKey = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };        // Initialize a key which will be copied into DES.Key and DES.IV
            for (int i = 0; i < RawKey.Length; i += 2)                          // Iterate through the RawKey
                EightByteKey[(i / 2) % 8] += RawKey[i];                         // Copy byte values to the 8 byte key array
            DES.Key = EightByteKey;                                             // Copy 8 byte key into DES.Key
            DES.IV = EightByteKey;                                              // Copy 8 byte key into DES.IV
            FileStream fsEncrypted = new FileStream(InputFilename, FileMode.Open, FileAccess.Read);     // Open file
            ICryptoTransform Decryptor = DES.CreateDecryptor();                 // Instantiate Decryptor
            CryptoStream cStream = new CryptoStream(fsEncrypted, Decryptor, CryptoStreamMode.Read);     // Decrypt Stream
            string ToWrite = new StreamReader(cStream).ReadToEnd();             // Check for a BadDataException
            StreamWriter fsDecrypted = new StreamWriter(OutputFilename);        // Implements a TextWriter and creates a file for writing
            fsDecrypted.Write(ToWrite);                                         // Write the decrypted stream
            fsDecrypted.Flush();                                                // Clears buffer for current writer
            fsDecrypted.Close();                                                // Closes the StreamWriter
        }
    }
}
