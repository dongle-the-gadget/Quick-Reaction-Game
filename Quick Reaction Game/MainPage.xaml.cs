using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Quick_Reaction_Game
{
    public enum Player
    {
        Player1,
        Player2
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Player whichPlayer;
        private DispatcherTimer timer;
        private GpioPin player1Button;
        private GpioPin player2Button;
        private GpioPinValue player1LEDValue;
        private GpioPinValue player2LEDValue;
        private GpioPin player1LED;
        private GpioPin player2LED;
        private bool player1Pressed;
        private bool player2Pressed;

        public MainPage()
        {
            this.InitializeComponent();
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50),
                
            };
            timer.Tick += Timer_Tick;
            if(player1Button != null & player2Button != null & player1LED != null & player2LED != null)
            {
                
            }
        }

        private void InitGpio()
        {
            var gpio = GpioController.GetDefault();
            if(gpio == null)
            {
                return;
            }
            player1Button = gpio.OpenPin(5);
            player2Button = gpio.OpenPin(17);
            player1LED = gpio.OpenPin(4);
            player2LED = gpio.OpenPin(6);
            if (player1Button.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
            {
                // If player 1 button supported InputPullUp, set it to InputPullUp
                player1Button.SetDriveMode(GpioPinDriveMode.InputPullUp);
            }
            else
            {
                // If not, set it to Input
                player1Button.SetDriveMode(GpioPinDriveMode.Input);
            }
            if (player2Button.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
            {
                // If player 2 button supported InputPullUp, set it to InputPullUp
                player2Button.SetDriveMode(GpioPinDriveMode.InputPullUp);
            }
            else
            {
                // If not, set it to Input
                player2Button.SetDriveMode(GpioPinDriveMode.Input);
            }
            // Set the LEDs to Output
            player1LED.SetDriveMode(GpioPinDriveMode.Output);
            player2LED.SetDriveMode(GpioPinDriveMode.Output);
            // Set debounce timeout for the buttons to fillter any noise from the buttons
            player1Button.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            player2Button.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            // Add ValueChanged event when the buttons changed value
            player1Button.ValueChanged += Player1Button_ValueChanged;
            player2Button.ValueChanged += Player2Button_ValueChanged;
        }

        private void Player2Button_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            // Button pressed
            if(args.Edge == GpioPinEdge.FallingEdge)
            {
                // Check if player 1 pressed first, if yes, player 1 win
                if(player1Pressed)
                {
                    whichPlayer = Player.Player1;
                    timer.Start();
                }
                // If player 1 pressed first, player 2 win
                else
                {
                    whichPlayer = Player.Player2;
                    timer.Start();
                }
            }
            else
            {
                // Check if player 1 pressed first, if yes, player 1 win
                if (player1Pressed)
                {
                    whichPlayer = Player.Player1;
                    timer.Start();
                }
            }
        }

        private void Player1Button_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if(args.Edge == GpioPinEdge.FallingEdge)
            {
                // Check if player 2 pressed first, if yes, player 2 win
                if (player2Pressed)
                {
                    whichPlayer = Player.Player2;
                    timer.Start();
                }
                // If player 1 pressed first, player 1 win
                else
                {
                    whichPlayer = Player.Player1;
                    timer.Start();
                }
            }
            else
            {
                // Check if player 2 pressed first, if yes, player 2 win
                if (player1Pressed)
                {
                    whichPlayer = Player.Player2;
                    timer.Start();
                }
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            Player whichPlayer = this.whichPlayer;
            if(whichPlayer == Player.Player1)
            {
                if(player1LEDValue == GpioPinValue.High)
                {
                    player1LEDValue = GpioPinValue.Low;
                    player1LED.Write(player1LEDValue);
                }
                else
                {
                    player1LEDValue = GpioPinValue.High;
                    player1LED.Write(player1LEDValue);
                }
            }
            else
            {
                if (player2LEDValue == GpioPinValue.High)
                {
                    player2LEDValue = GpioPinValue.Low;
                    player2LED.Write(player1LEDValue);
                }
                else
                {
                    player2LEDValue = GpioPinValue.High;
                    player2LED.Write(player1LEDValue);
                }
            }
        }
    }
}
