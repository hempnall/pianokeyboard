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
using Sanford.Multimedia.Midi.UI;
using System.Windows.Forms;
using System.Threading;

namespace PianoKeyboard
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {




        private System.Windows.Forms.OpenFileDialog openMidiFileDialog = new OpenFileDialog();
        //private Sanford.Multimedia.Midi.UI.PianoControl pianoControl1;
        //private System.Windows.Forms.StatusStrip statusStrip1;
        //private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        //private System.Windows.Forms.ToolStripMenuItem mIDIToolStripMenuItem;
        //private System.Windows.Forms.ToolStripMenuItem outputDeviceToolStripMenuItem;
        private Sequence sequence1 = new Sequence();
        private Sequencer sequencer1 = new Sequencer();
        private System.Windows.Forms.Timer timer1;


        private bool scrolling = false;

        private bool playing = false;

        private bool closing = false;

        private OutputDevice outDevice;

        private int outDeviceID = 0;

        private OutputDeviceDialog outDialog = new OutputDeviceDialog();

        private void updateCamera_Click(object sender, RoutedEventArgs e)
        {
            SetCamera();
        }

        private bool isDown = false;
        private void ToggleKey_Click(object sender, RoutedEventArgs e)
        {
            if (openMidiFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;

                try
                {
                    sequencer1.Stop();
                    playing = false;
                    sequence1.Load(fileName);

                    try
                    {
                        playing = true;
                        sequencer1.Start();
                        //timer1.Start();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }

                }
                catch(Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }                
            
            }
        }


        private void SetCamera()
        {
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
            Point3D position = new Point3D(
                Convert.ToDouble(cameraPositionXTextBox.Text),
                Convert.ToDouble(cameraPositionYTextBox.Text),
                Convert.ToDouble(cameraPositionZTextBox.Text)
            );
            Vector3D lookDirection = new Vector3D(
                Convert.ToDouble(lookAtXTextBox.Text),
                Convert.ToDouble(lookAtYTextBox.Text),
                Convert.ToDouble(lookAtZTextBox.Text)
            );
            camera.Position = position;
            camera.LookDirection = lookDirection;
        }

        Piano piano;

        public Window1()
        {
            InitializeComponent();


            // 
            // openMidiFileDialog
            // 
            this.openMidiFileDialog.DefaultExt = "mid";
            this.openMidiFileDialog.Filter = "MIDI files|*.mid|All files|*.*";
            this.openMidiFileDialog.Title = "Open MIDI file";



            // 
            // sequence1
            // 
            this.sequence1.Format = 1;
            // 
            // sequencer1
            // 
            this.sequencer1.Position = 0;
            this.sequencer1.Sequence = this.sequence1;
            this.sequencer1.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
            this.sequencer1.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);
            this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);
            //this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
            this.sequencer1.Chased += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);

            SetCamera();
            piano = new Piano(mainViewport, 0.3F, 0.02F, 0.3F, 1.8F,0.3F, 66.0F);
            piano.Draw(mainViewport);
        }




        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            String keys = "AWSDRFTGYHJIKOL";

            String key = e.Key.ToString();
            int i = keys.IndexOf(key);

            if (i != -1)
                piano.KeyDown((byte) i);
        }

        private void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            String keys = "AWSDRFTGYHJIKOL";

            String key = e.Key.ToString();
            int i = keys.IndexOf(key);

            if (i != -1)
                piano.KeyUp((byte) i);
        }


        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            if (closing)
            {
                return;
            }

            



            if (Dispatcher.Thread == Thread.CurrentThread)
            {
                outDevice.Send(e.Message);
                piano.Send(e.Message);
            }
            else
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new EventHandler<ChannelMessageEventArgs>(HandleChannelMessagePlayed), sender, new object[] { e });
            }


            
        }

        private void HandleChased(object sender, ChasedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }


        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
                piano.Send(message);
            }
        }


        private void HandlePlayingCompleted(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            if (OutputDevice.DeviceCount == 0)
            {
                System.Windows.MessageBox.Show("No MIDI output devices available.");

                Close();
            }
            else
            {
                try
                {
                    outDevice = new OutputDevice(outDeviceID);

                    //sequence1.LoadProgressChanged += HandleLoadProgressChanged;
                    //sequence1.LoadCompleted += HandleLoadCompleted;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);

                    Close();
                }
            }



        }
    }
}
