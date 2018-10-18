using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MotencDemo
{
    public partial class MainWindow : Window
    {
        private MotencAPI motencAPI;

        private Semaphore sem = new Semaphore(1, 1);
        private Thread readThread;
        private ManualResetEvent cancelReadThread = new ManualResetEvent(false);

        private SolidColorBrush defaultLEDColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4F4F5"));

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            createRegCboBoxes();
            motencAPI = new MotencAPI();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUI();
            readThread = new Thread(new ThreadStart(ReadThread));
            readThread.IsBackground = true;
            readThread.Start();
        }

        private void wndwMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
            motencAPI.Dispose();
        }

        private delegate void UpdateUIDelegate();
        private void UpdateUI()
        {
            if(!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new UpdateUIDelegate(UpdateUI));
                return;
            }

            lock(sem)
            {
                txtEnc1.Text = Convert.ToString(motencAPI.ReadEncoder(0));
                txtEnc2.Text = Convert.ToString(motencAPI.ReadEncoder(1));
                txtEnc3.Text = Convert.ToString(motencAPI.ReadEncoder(2));
                txtEnc4.Text = Convert.ToString(motencAPI.ReadEncoder(3));

                intializeLEDs(grdInputs, true);
                intializeLEDs(grdOutputs, false);

                txtReg0.Text = String.Format("{0:X8}", motencAPI.ReadRegister(cboRegs1.SelectedIndex));
                txtReg1.Text = String.Format("{0:X8}", motencAPI.ReadRegister(cboRegs2.SelectedIndex));
                txtReg2.Text = String.Format("{0:X8}", motencAPI.ReadRegister(cboRegs3.SelectedIndex));
                txtReg3.Text = String.Format("{0:X8}", motencAPI.ReadRegister(cboRegs4.SelectedIndex));
                txtReg4.Text = String.Format("{0:X8}", motencAPI.ReadRegister(cboRegs5.SelectedIndex));
                txtReg5.Text = String.Format("{0:X8}", motencAPI.ReadRegister(cboRegs6.SelectedIndex));

                txtReadADC0.Text = String.Format("{0}", motencAPI.ReadAnalogIN(cboADCBank.SelectedIndex, 0));
                txtReadADC1.Text = String.Format("{0}", motencAPI.ReadAnalogIN(cboADCBank.SelectedIndex, 1));
                txtReadADC2.Text = String.Format("{0}", motencAPI.ReadAnalogIN(cboADCBank.SelectedIndex, 2));
                txtReadADC3.Text = String.Format("{0}", motencAPI.ReadAnalogIN(cboADCBank.SelectedIndex, 3));
            }
        }

        private void createRegCboBoxes()
        {
            List<int> regRange = new List<int>();
            for (int i = 0; i < 41; i++)
                regRange.Add(i);
            cboRegs1.ItemsSource = regRange;
            cboRegs2.ItemsSource = regRange;
            cboRegs3.ItemsSource = regRange;
            cboRegs4.ItemsSource = regRange;
            cboRegs5.ItemsSource = regRange;
            cboRegs6.ItemsSource = regRange;
        }

        private void ReadThread()
        {
            while (true)
            {
                if (cancelReadThread.WaitOne(100, false))
                    return;

                UpdateUI();
            }
        }

        private void txtEnc1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            MessageBoxResult rs = MessageBox.Show("Reset Encoder 0?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            lock(sem)
                motencAPI.ResetEncoder(0);
        }

        private void txtEnc2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            MessageBoxResult rs = MessageBox.Show("Reset Encoder 1?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            lock (sem)
                motencAPI.ResetEncoder(1);
        }

        private void txtEnc3_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            MessageBoxResult rs = MessageBox.Show("Reset Encoder 2?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            lock (sem)
                motencAPI.ResetEncoder(2);
        }

        private void txtEnc4_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            MessageBoxResult rs = MessageBox.Show("Reset Encoder 3?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            lock (sem)
                motencAPI.ResetEncoder(3);
        }

        private void btnRstAllEncs_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            MessageBoxResult rs = MessageBox.Show("Reset All Encoder Values?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            lock (sem)
                for (int enc = 0; enc < 4; enc++)
                    motencAPI.ResetEncoder(enc);
        }

        private void intializeLEDs(Grid grid, bool input)
        {
            int index = 0;
            foreach (StackPanel sp in grid.Children)
                foreach(var el in sp.Children)
                    if(el is Ellipse)
                        changeLEDColor((Ellipse)el, index++, input);
        }

        private void changeLEDColor(Ellipse led, int index, bool input)
        {
            long ioData = input ? motencAPI.DigitalInputs : motencAPI.DigitalOutputs;
            if (input)
                led.Fill = (ioData & (1 << index)) == 0 ? led.Fill = new SolidColorBrush(Colors.GreenYellow) : defaultLEDColor;
            else
                led.Fill = (ioData & (1 << index)) == 0 ? led.Fill = defaultLEDColor : new SolidColorBrush(Colors.GreenYellow);
        }

        private void setOutput(object sender)
        {
            Ellipse outputLED = (Ellipse)sender;
            int index = int.Parse(new string(outputLED.Name.Where(char.IsDigit).ToArray()));

            motencAPI.DigitalOutputs ^= (ushort)(1 << index);
            changeLEDColor(outputLED, index, false);
        }

        private void ledOut0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut5_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut7_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut8_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut9_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut10_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut11_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut12_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut13_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut14_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void ledOut15_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setOutput(sender);
        }

        private void btnDACPlus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int channel = cboDACChannels.SelectedIndex;
            motencAPI.SetDAC(channel, 10);
        }

        private void btnDACPlus_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            int channel = cboDACChannels.SelectedIndex;
            motencAPI.SetDAC(channel, 0);
        }

        private void btnDACNeg_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int channel = cboDACChannels.SelectedIndex;
            motencAPI.SetDAC(channel, -10);
        }

        private void btnDACNeg_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            int channel = cboDACChannels.SelectedIndex;
            motencAPI.SetDAC(channel, 0);
        }
    }
}
