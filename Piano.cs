using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using Sanford.Multimedia.Midi;
using System.Threading;
using System.Windows.Forms;

namespace PianoKeyboard
{
    class Piano
    {


        public void Send(ChannelMessage message)
        {
            
            int LowNoteID = 0;
            int HighNoteID = 87;


            Byte b ;
            if (message.Command == ChannelCommand.NoteOn &&
                message.Data1 >= LowNoteID && message.Data1 <= HighNoteID)
            {
                b = (byte) (message.Data1 -21) ;

                    KeyDown(b);

               

            }
            else if (message.Command == ChannelCommand.NoteOff &&
                message.Data1 >= LowNoteID && message.Data1 <= HighNoteID)
            {
                b = (byte) (message.Data1 - 21);
                KeyUp(b);
            }
        }


        private List<PianoKey> _PianoKeys = new List<PianoKey>();

        public List<PianoKey> PianoKeys
        {
            get { return _PianoKeys; }
            set { _PianoKeys = value; }
        }

        public Piano(Viewport3D mainViewport, float whiteKeyWidth,float keyGap,float blackKeyWidth,float keyLength,float keyHeight,float blackKeyLengthRatio)
        {

            float runningTotalOrigin = 0;
            
            for (byte i = 0; i < 88; i++)
            {   

                PianoKey pk = new PianoKey(mainViewport, i, runningTotalOrigin, keyLength, keyHeight, whiteKeyWidth);
                _PianoKeys.Add(pk);

                if ( !pk.IsNextKeyBlack  )
                {
                    runningTotalOrigin = runningTotalOrigin + whiteKeyWidth + keyGap;
                }

            }
        }


        public void KeyDown(byte i)
        {
            _PianoKeys[i].KeyDown = true;
            _PianoKeys[i].Draw(null);
        }

        public void KeyUp(byte i)
        {
            _PianoKeys[i].KeyDown = false;
            _PianoKeys[i].Draw(null);
        }

        public void Draw(Viewport3D mainViewport)
        {
            foreach (PianoKey pk in _PianoKeys)
            {
                pk.Draw(mainViewport);
            }
        }

    }
}
