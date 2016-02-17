using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using WIA;

namespace Numerisation_GIST
{
    public class Numerisation
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
        public Numerisation()
        {
            WIA.ICommonDialog dialog = new WIA.CommonDialog();
            WIA.Device device = null;
            try {
                device = dialog.ShowSelectDevice
                    (WIA.WiaDeviceType.ScannerDeviceType, true, false);
            }catch(Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Problème lors de la communication avec le scanner : vérifiez les branchements, que le scanner est allumé et réessayez");
            }

            if (device != null)
            {
                this.deviceID = device.DeviceID;
            }
        }
        /// <summary>
        /// Use scanner to scan an image (scanner is selected by its unique id).
        /// </summary>
        /// <param name="scannerName"></param>
        /// <returns>Scanned images.</returns>
        public List<Image> Scan()
        {
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
                                SetProperty(prop, Program.scanDPI);
                                break;
                            case 6148: //ppp vertical 
                                SetProperty(prop, Program.scanDPI);
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
}
