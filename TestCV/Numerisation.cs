using NTwain;
using NTwain.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Media.Imaging;
using WIA;

namespace Numerisation_GIST
{
    public abstract class Numerisation
    {
        public Boolean ready;

        public Numerisation()
        {
            ready = false;
        }

        public abstract List<Image> Scan();
    }

    public class NumerisationWIA : Numerisation
    {
        public String deviceID { get; private set; }

        private static void SetProperty(Property property, int value)
        {
            IProperty x = (IProperty)property;
            Object val = value;
            x.set_Value(ref val);
        }

        /// <summary>
        /// Use scanner to scan an image (with user selecting the scanner from a dialog).
        /// </summary>
        /// <returns>Scanned images.</returns>
        public NumerisationWIA()
        {
            WIA.ICommonDialog dialog = new WIA.CommonDialog();
            WIA.Device device = null;
            try {
                device = dialog.ShowSelectDevice
                    (WIA.WiaDeviceType.UnspecifiedDeviceType, true, false);
            }catch(Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Problème lors de la communication avec le scanner : vérifiez les branchements, que le scanner est allumé et réessayez");
                throw new Exception();
            }

            if (device != null)
            {
                this.deviceID = device.DeviceID;
                this.ready = true;
            }
        }
        /// <summary>
        /// Use scanner to scan an image (scanner is selected by its unique id).
        /// </summary>
        /// <param name="scannerName"></param>
        /// <returns>Scanned images.</returns>
        public override List<Image> Scan()
        {
            Console.WriteLine("Scan WIA");
            List<Image> images = new List<Image>();
            bool hasMorePages = true;
            while (hasMorePages)
            {
                // select the correct scanner using the provided scannerId parameter
                WIA.DeviceManager manager = new WIA.DeviceManager();
                WIA.Device device = null;
                foreach (WIA.DeviceInfo info in manager.DeviceInfos)
                {
                    if (info.DeviceID == deviceID)
                    {
                        // connect to scanner
                        device = info.Connect();
                        break;
                    }
                }
                // device was not found
                if (device == null)
                {
                    // enumerate available devices
                    string availableDevices = "";
                    foreach (WIA.DeviceInfo info in manager.DeviceInfos)
                    {
                        availableDevices += info.DeviceID + "\n";
                    }
                    // show error with available devices
                    throw new Exception("The device with provided ID could not be found. Available Devices:\n" + availableDevices);
                }
                WIA.Item item = device.Items[1] as WIA.Item;
                try
                {
                    foreach (Property prop in item.Properties)
                    {
                        switch (prop.PropertyID)
                        {
                            case 6146: //1 : couleur, 2 : gris, 4 : binaire
                                SetProperty(prop, 2);
                                break;
                            case 6147: //ppp horizontal 
                                SetProperty(prop, Program.numerisationDPI);
                                break;
                            case 6148: //ppp vertical 
                                SetProperty(prop, Program.numerisationDPI);
                                break;
                            case 6149: //x point where to start scan 
                                SetProperty(prop, 0);
                                break;
                            case 6150: //y-point where to start scan 
                                SetProperty(prop, 0);
                                break;
                        }
                    }

                    // scan image
                    WIA.ICommonDialog wiaCommonDialog = new WIA.CommonDialog();
                    WIA.ImageFile image = (WIA.ImageFile)wiaCommonDialog.ShowTransfer(item, EnvFormatID.wiaFormatTIFF, false);

                    if(image == null)
                    {
                        Console.WriteLine("Numérisation annulé");
                        return null;
                    }

                    // save to temp file
                    string fileName = Path.GetTempFileName();
                    File.Delete(fileName);
                    image.SaveFile(fileName);
                    image = null;
                    // add file to output list
                    images.Add(Image.FromFile(fileName));
                }
                catch (System.ArgumentException e)
                {
                    Console.WriteLine("Le PPP spécifié n'est pas supporté par le scanner");
                    throw e;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                finally
                {
                    item = null;
                    //determine if there are any more pages waiting
                    WIA.Property documentHandlingSelect = null;
                    WIA.Property documentHandlingStatus = null;
                    foreach (WIA.Property prop in device.Properties)
                    {
                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_SELECT)
                            documentHandlingSelect = prop;
                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_STATUS)
                            documentHandlingStatus = prop;
                    }
                    // assume there are no more pages
                    hasMorePages = false;
                    // may not exist on flatbed scanner but required for feeder
                    if (documentHandlingSelect != null)
                    {
                        // check for document feeder
                        if ((Convert.ToUInt32(documentHandlingSelect.get_Value()) &
                        WIA_DPS_DOCUMENT_HANDLING_SELECT.FEEDER) != 0)
                        {
                            hasMorePages = ((Convert.ToUInt32(documentHandlingStatus.get_Value()) &
                            WIA_DPS_DOCUMENT_HANDLING_STATUS.FEED_READY) != 0);
                        }
                    }
                }
            }
            return images;
        }

        public abstract class EnvFormatID
        {
            public const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
            public const string wiaFormatGIF = "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}";
            public const string wiaFormatJPEG = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
            public const string wiaFormatPNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}";
            public const string wiaFormatTIFF = "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}";
        }

        class WIA_DPS_DOCUMENT_HANDLING_SELECT
        {
            public const uint FEEDER = 0x00000001;
            public const uint FLATBED = 0x00000002;
        }
        class WIA_DPS_DOCUMENT_HANDLING_STATUS
        {
            public const uint FEED_READY = 0x00000001;
        }
        class WIA_PROPERTIES
        {
            public const uint WIA_RESERVED_FOR_NEW_PROPS = 1024;
            public const uint WIA_DIP_FIRST = 2;
            public const uint WIA_DPA_FIRST = WIA_DIP_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPC_FIRST = WIA_DPA_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            //
            // Scanner only device properties (DPS)
            //
            public const uint WIA_DPS_FIRST = WIA_DPC_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPS_DOCUMENT_HANDLING_STATUS = WIA_DPS_FIRST + 13;
            public const uint WIA_DPS_DOCUMENT_HANDLING_SELECT = WIA_DPS_FIRST + 14;
        }
    }

    public class NumerisationTwain : Numerisation
    {
        List<Image> lesImagesNum;

        TwainSession session;
        DataSource myDS;

        public NumerisationTwain(){
            lesImagesNum = new List<Image>();
            ready = true;
        }

        //Peut permettre d'annuler un scan, appel lors du début du transfert
        void session_SourceDisable(object sender, EventArgs e)
        {
            myDS.Close();
            session.Close();
        }

        //Peut permettre d'annuler un scan, appel lors du début du transfert
        void session_TransferReady(object sender, NTwain.TransferReadyEventArgs e)
        {
            
        }


        //Appel lorsque des données sont transférées
        void session_DataTransferred(object sender, NTwain.DataTransferredEventArgs e)
        {
            if (e.NativeData != IntPtr.Zero)
            {
                Bitmap img = null;
                //Need to save out the data.
                Stream s = e.GetNativeImageStream();
                BitmapSource bitmapsource = s.ConvertToWpfBitmap();

                using (MemoryStream outStream = new MemoryStream())
                {
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                    enc.Save(outStream);
                    img = new Bitmap(outStream);
                }

                if (img != null)
                {
                    lesImagesNum.Add(img);
                }
            }
        }

        public override List<Image> Scan()
        {
            Console.WriteLine("Scan Twain");
            lesImagesNum.Clear();
            // can use the utility method to create appId or make one yourself
            var appId = TWIdentity.CreateFromAssembly(DataGroups.Image, Assembly.GetExecutingAssembly());

            // new it up and handle events
            session = new TwainSession(appId);

            session.TransferReady += session_TransferReady;
            session.DataTransferred += session_DataTransferred;
            session.SourceDisabled += session_SourceDisable;
            
            // finally open it
            session.Open();
            var t = session.GetEnumerator();

            Console.WriteLine(t.ToString());
            while (t.MoveNext())
            {
                Console.WriteLine(t.ConvertToString());
            }



            // choose and open the first source found
            // note that TwainSession implements IEnumerable<DataSource> so we can use this extension method.
            IEnumerable<DataSource> lesSources = session.GetSources();
            Console.WriteLine("Nb Source : " + session.Count());
            myDS = session.FirstOrDefault();
            myDS.Open();

            // All low-level triplet operations are defined through these properties.
            // If the operation you want is not available, that most likely means 
            // it's not for consumer use or it's been abstracted away with an equivalent API in this lib.
            //myDS.DGControl;
            //myDS.DGImage.;

            // The wrapper has many methods that corresponds to the TWAIN capability triplet msgs like
            // GetValues(), GetCurrent(), GetDefault(), SetValue(), etc.
            // (see TWAIN spec for reference)


            // This example sets pixel type of scanned image to BW and
            // IPixelType is the wrapper property on the data source.
            // The name of the wrapper property is the same as the CapabilityId enum.
            PixelType typeCouleur = PixelType.Gray;

            if (myDS.Capabilities.ICapPixelType.CanSet &&
                myDS.Capabilities.ICapPixelType.GetValues().Contains(typeCouleur))
            {
                myDS.Capabilities.ICapPixelType.SetValue(typeCouleur);
            }

            //Même chose avec le DPI
            TWFix32 DPI = Program.numerisationDPI;

            if(myDS.Capabilities.ICapXResolution.CanSet &&
                myDS.Capabilities.ICapXResolution.GetValues().Contains(DPI))
            {
                myDS.Capabilities.ICapXResolution.SetValue(DPI);
            }

            if (myDS.Capabilities.ICapYResolution.CanSet &&
                myDS.Capabilities.ICapYResolution.GetValues().Contains(DPI))
            {
                myDS.Capabilities.ICapYResolution.SetValue(DPI);
            }
            
            myDS.Enable(SourceEnableMode.ShowUI, false, System.IntPtr.Zero);

            EventWaitHandle session_SourceDisable_Wait = new EventWaitHandle(false, EventResetMode.AutoReset);
            session_SourceDisable_Wait.WaitOne();
            return lesImagesNum;
        }
    }
}
