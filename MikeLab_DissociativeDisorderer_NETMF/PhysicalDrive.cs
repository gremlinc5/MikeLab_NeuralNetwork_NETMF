using System;
using System.IO;
using System.Text;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using Microsoft.SPOT.Hardware;

using GHI.Utilities;
using GHI.IO.Storage;
using GHI.Pins;

namespace MikeLab.PhysicalMassMemory
{
    public static class PhysicalDrive
    {
        //Make sure to set the pin to your sd card detect pin.
        private static InputPort sdCardDetect;
        private static InputPort sdCardWriteProtect;

        public static VolumeInfo volume_info { get; private set; }
        public static bool ReadyRead { get; set; }
        public static bool ReadyWrite { get; set; }

        public static void Init()
        {
            sdCardDetect = new InputPort(GHI.Pins.G400D.Gpio.PD4, false, Port.ResistorMode.Disabled);
            sdCardWriteProtect = new InputPort(GHI.Pins.G400D.Gpio.PD5, false, Port.ResistorMode.Disabled);

            ReadyRead = false;
            ReadyWrite = false;
            volume_info = null;

            RemovableMedia.Insert += new InsertEventHandler(RemovableMedia_Insert);
            RemovableMedia.Eject += new EjectEventHandler(RemovableMedia_Eject);

            // Start auto mounting thread
            new Thread(SDMountThread).Start();
        }

        // This event is fired by unmount; not neccesarily by physical ejection of media
        static void RemovableMedia_Eject(object sender, MediaEventArgs e)
        {
            Debug.Print("SD card unmounted, eject event fired");
            ReadyRead = false;
            ReadyWrite = false;
            volume_info = null;
        }

        static void RemovableMedia_Insert(object sender, MediaEventArgs e)
        {
            Debug.Print("Insert event fired; SD card mount is finished.");

            if (e.Volume.IsFormatted)
            {
                Debug.Print("SDCard Mounted!");
                volume_info = e.Volume;
                ReadyRead = true;
            }
            else
            {
                Debug.Print("SDCard is not formatted.");
                ReadyRead = false;
            }
        }

        static void SDMountThread()
        {
            SDCard SD = null;
            const int POLL_TIME = 500; // check every 500 millisecond

            bool sdExists;
            while (true)
            {
                try // If SD card was removed while mounting, it may throw exceptions
                {
                    sdExists = !sdCardDetect.Read();

                    // make sure it is fully inserted and stable
                    if (sdExists)
                    {
                        Thread.Sleep(50);
                        sdExists = !sdCardDetect.Read();
                    }

                    if (sdExists && SD == null)
                    {
                        SD = new SDCard();
                        SD.Mount();
                    }
                    else if (!sdExists && SD != null)
                    {
                        SD.Unmount();
                        SD.Dispose();
                        SD = null;
                    }
                    else if (sdExists && SD != null)
                    {
                        if (sdCardWriteProtect.Read() == true)
                        {
                            ReadyWrite = false;
                            //Debug.Print("ReadyWrite: false");
                        }
                        else
                        {
                            ReadyWrite = true;
                            //Debug.Print("ReadyWrite: true. Flushing!");
                            volume_info.FlushAll();
                        }
                    }
                }
                catch
                {
                    if (SD != null)
                    {
                        SD.Dispose();
                        SD = null;
                    }
                }

                Thread.Sleep(POLL_TIME);
            }
        }

        public static int ReadFileOnSDCardAsString(string path, out string file)
        {
            int i = 0;
            while (ReadyRead == false)
            {
                Thread.Sleep(10);
                i += 10;
                if (i >= 5000)
                {
                    file = "";
                    return 0;
                }
            }
            FileStream FileHandle = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[33333];
            int read_count = FileHandle.Read(data, 0, data.Length);
            FileHandle.Close();
            Debug.Print("The size of data read is: " +
                        read_count.ToString());
            Debug.Print("Data from file:");
            Debug.Print(new string(Encoding.UTF8.GetChars(data), 0, read_count));
            file = new string(Encoding.UTF8.GetChars(data), 0, read_count);
            return read_count;
        }
    }
}
