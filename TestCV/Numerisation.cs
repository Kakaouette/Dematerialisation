using System;
using TestCV;
using WIA;

namespace Numerisation_GIST
{
    public abstract class EnvFormatID
    {
        public const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
        public const string wiaFormatGIF = "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}";
        public const string wiaFormatJPEG = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
        public const string wiaFormatPNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}";
        public const string wiaFormatTIFF = "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}";
    }

    public class Numerisation
    {
        // Scanner only device properties (DPS)
        public const int WIA_RESERVED_FOR_NEW_PROPS = 1024;
        public const int WIA_DIP_FIRST = 2;
        public const int WIA_DPA_FIRST = WIA_DIP_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
        public const int WIA_DPC_FIRST = WIA_DPA_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
        public const int WIA_DPS_FIRST = WIA_DPC_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
        public const int WIA_DPS_DOCUMENT_HANDLING_STATUS = WIA_DPS_FIRST + 13;
        public const int WIA_DPS_DOCUMENT_HANDLING_SELECT = WIA_DPS_FIRST + 14;
        public const int FEEDER = 1;
        public const int FLATBED = 2;
        public const int DUPLEX = 4;
        public const int FEED_READY = 1;

        WIA.CommonDialog _dialog = new CommonDialog();
        WIA.Device _scanner;

        public void ADFScan()
        {

            _dialog = new CommonDialog();
            _scanner = _dialog.ShowSelectDevice(WIA.WiaDeviceType.ScannerDeviceType, false, false);


            foreach (Property item in _scanner.Items[1].Properties)
            {
                switch (item.PropertyID)
                {
                    case 6146: //4 is Black-white,gray is 2, color 1
                        SetProperty(item, 2);
                        break;
                    case 6147: //dots per inch/horizontal 
                        SetProperty(item, Program.scanDPI);
                        break;
                    case 6148: //dots per inch/vertical 
                        SetProperty(item, Program.scanDPI);
                        break;
                    case 6149: //x point where to start scan 
                        SetProperty(item, 0);
                        break;
                    case 6150: //y-point where to start scan 
                        SetProperty(item, 0);
                        break;
                        /*
                    case 6151: //horizontal exent 
                        SetProperty(item, (int)(8.5 * 100));
                        break;
                    case 6152: //vertical extent 
                        SetProperty(item, 11 * 100);
                        break;
                        */
                }
            }
            ImageFile image = (ImageFile)_scanner.Items[1].Transfer(EnvFormatID.wiaFormatTIFF);
            image.SaveFile(Program.cheminTemp + "numerisation.tif");
        }

        private void SetProperty(Property property, int value)
        {
            IProperty x = (IProperty)property;
            Object val = value;
            x.set_Value(ref val);
        }


        public void Scan()
        {
            ADFScan();
        }
    }
}
