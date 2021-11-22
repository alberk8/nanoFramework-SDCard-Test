using nanoFramework.Hardware.Esp32;
using nanoFramework.System.IO.FileSystem;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using GC = nanoFramework.Runtime.Native.GC;

namespace nanoFrameworkSDCard
{
    public class Program
    {
        private static SDCard mycard;
        public static void Main()
        {

           
            PrintNativeMemory("1. ");

            // the SDCard can only be initialized once even after a disposed.  Recreating a new instance will always fail.
            mycard = new SDCard(new SDCard.SDCardMmcParameters { dataWidth = SDCard.SDDataWidth._4_bit });

           
           MountSDCard();

            //// Debug.WriteLine("Hello from nanoFramework!");
            int count = 1;
            for (int i = 1; i < 100; i++)
            {
                PrintNativeMemory("2. ");
                ReadAndWriteSD(count);
                PrintNativeMemory("3. ");
                count++;
                Thread.Sleep(1_000);
            }

            UnMountSDCard();

           
            Thread.Sleep(Timeout.Infinite);

           
        }

        static void ReadAndWriteSD(int count)
        {
            int mod = count % 10;
            string sFP = "D:\\sampleFile";
            sFP += mod.ToString() + ".txt";

            string sampleText = "Lorem Ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. ";


            //mycard = new SDCard(new SDCard.SDCardMmcParameters { dataWidth = SDCard.SDDataWidth._4_bit });

            //var mountcard = MountSDCard();

            if (mycard.IsMounted)
            {
                Debug.WriteLine($"Create File in SD Card {sFP}");
                byte[] sampleBuffer = Encoding.UTF8.GetBytes(sampleText);

                // This also leak memory
                //using FileStream file = File.Create(sFP);
                //file.Flush();
                //file.Close();

                //Leaks memory
                using FileStream file1 = new FileStream(sFP, FileMode.Create, FileAccess.ReadWrite);
                file1.Write(sampleBuffer, 0, sampleBuffer.Length);
                file1.Flush();
                file1.Close();

                //using FileStream fs = new FileStream(sFP, FileMode.Open, FileAccess.ReadWrite);
                // fs.Write(sampleBuffer, 0, sampleBuffer.Length);
                //fs.Flush();
                //fs.Close();

            }

            // UnMountSDCard();
        }

        static void PrintNativeMemory(string prependstr)
        {
            uint totalMem, totalFree, freeBlock;
            NativeMemory.GetMemoryInfo(NativeMemory.MemoryType.Internal, out totalMem, out totalFree, out freeBlock);
            Debug.WriteLine($"nF Memory {GC.Run(false)}");
            Debug.WriteLine($"{prependstr} Total Mem {totalMem} Total Free {totalFree} Free Block {freeBlock}\n");
        }

        static void UnMountSDCard()
        {
            if (mycard.IsMounted)
            {
                mycard.Unmount();
                mycard.Dispose();
                Debug.WriteLine("Card UnMounted");
                PrintNativeMemory("6. ");

            }
        }

        static bool MountSDCard()
        {
            try
            {
                mycard.Mount();
                Debug.WriteLine("Card Mounted");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Card failed to mount : {ex.Message}");
                Debug.WriteLine($"IsMounted {mycard.IsMounted}");
            }

            return false;
        }
    }
}
