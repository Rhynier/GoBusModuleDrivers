using System;
using Microsoft.SPOT;
using NetduinoGo;
using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoGo;

namespace Komodex.NETMF.ModuleTestApp
{
    public static class SevenSegmentDisplayDemo
    {
        private static SevenSegmentDisplay _display;
        private static RgbLed _statusLed;
        private static Potentiometer _potentiometer;

        private static bool _goToNextDemo;

        public static void Run()
        {
            Debug.Print("Starting Seven Segment Display demo...");

            // Initialization
            InterruptPort button = new InterruptPort(Pins.Button, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            button.OnInterrupt += new NativeEventHandler(button_OnInterrupt);

            // RGB LED
            try
            {
                _statusLed = new RgbLed();
                _statusLed.SetColor(0, 0, 25);
                Debug.Print("Found RGB LED");
            }
            catch { }

            // Potentiometer
            try
            {
                _potentiometer = new Potentiometer();
                Debug.Print("Found Potentiometer");
            }
            catch { }

            // Seven Segment Display
            while (_display == null)
            {
                try
                {
                    _display = new SevenSegmentDisplay();
                    Debug.Print("Found Seven Segment Display");
                }
                catch
                {
                    if (_statusLed != null)
                        _statusLed.SetColor(25, 0, 0);
                    Debug.Print("Seven Segment Display not found...");
                    Thread.Sleep(250);
                }
            }

            if (_statusLed != null)
                _statusLed.SetColor(0, 25, 0);
            Debug.Print("Starting demos...");

            // Begin the demos
            while (true)
            {
                // Counting demo
                Debug.Print("Counting 0 to 9999");
                while (!_goToNextDemo)
                {
                    // Count up
                    for (int i = 0; i <= 9999 && !_goToNextDemo; i++)
                        _display.SetValue(i);
                    // Count down
                    for (int i = 9999; i >= 0 && !_goToNextDemo; i--)
                        _display.SetValue(i);
                }
                _goToNextDemo = false;

                // Reverse counting (negative numbers) demo
                Debug.Print("Negative numbers 0 to -999");
                while (!_goToNextDemo)
                {
                    // Count down
                    for (int i = 0; i >= -999 && !_goToNextDemo; i--)
                        _display.SetValue(i);
                    // Count up
                    for (int i = -999; i <= 0 && !_goToNextDemo; i++)
                        _display.SetValue(i);
                }
                _goToNextDemo = false;

                // Floating point demo
                Debug.Print("Floating point numbers with two decimal places from -9.99 to 9.99");
                while (!_goToNextDemo)
                {
                    // Count up
                    for (float i = -9.99f; i <= 9.99f && !_goToNextDemo; i += 0.01f)
                        _display.SetValue(i, 2);
                    // Count down
                    for (float i = 9.99f; i >= -9.99f && !_goToNextDemo; i -= 0.01f)
                        _display.SetValue(i, 2);
                }
                _goToNextDemo = false;

                // Brightness demo
                Debug.Print("Display brightness");
                while (!_goToNextDemo)
                {
                    float brightness = 1;

                    if (_potentiometer != null)
                    {
                        Debug.Print("(Use potentiometer to set brightness)");
                        while (!_goToNextDemo)
                        {
                            brightness = _potentiometer.GetValue();
                            _display.SetBrightness(brightness);
                            _display.SetValue(brightness, 3);
                        }
                    }
                    else
                    {
                        while (brightness > 0 && !_goToNextDemo)
                        {
                            _display.SetBrightness(brightness);
                            _display.SetValue(brightness, 3);
                            brightness -= 0.005f;
                        }
                        brightness = 0;
                        while (brightness < 1 && !_goToNextDemo)
                        {
                            _display.SetBrightness(brightness);
                            _display.SetValue(brightness, 3);
                            brightness += 0.005f;
                        }
                    }
                }
                _display.SetBrightness(1);
                _goToNextDemo = false;

                // Custom character demo
                Debug.Print("Custom characters");
                int delayTime = 50;
                Digit xx = (Digit)0x00; // Nothing
                Digit tt = (Digit)0x01; // Top
                Digit tr = (Digit)0x03; // Top right
                Digit rr = (Digit)0x06; // Right
                Digit br = (Digit)0x0c; // Bottom right
                Digit bb = (Digit)0x08; // Bottom
                Digit bl = (Digit)0x18; // Bottom left
                Digit ll = (Digit)0x30; // Left
                Digit tl = (Digit)0x21; // Top left
                while (!_goToNextDemo)
                {
                    _display.SetValue(tl, xx, xx, xx);
                    Thread.Sleep(delayTime);
                    if (_goToNextDemo) break;
                    _display.SetValue(tt, tt, xx, xx);
                    Thread.Sleep(delayTime);
                    if (_goToNextDemo) break;
                    _display.SetValue(xx, tt, tt, xx);
                    Thread.Sleep(delayTime);
                    if (_goToNextDemo) break;
                    _display.SetValue(xx, xx, tt, tt);
                    Thread.Sleep(delayTime);
                    if (_goToNextDemo) break;
                    _display.SetValue(xx, xx, xx, tr);
                    Thread.Sleep(delayTime);
                    if (_goToNextDemo) break;
                    _display.SetValue(xx, xx, xx, rr);
                    Thread.Sleep(delayTime);
                    if (_goToNextDemo) break;
                    _display.SetValue(xx, xx, xx, br);
                    Thread.Sleep(delayTime);
                    if (_goToNextDemo) break;
                    _display.SetValue(xx, xx, bb, bb);
                    Thread.Sleep(delayTime);
                    if (_goToNextDemo) break;
                    _display.SetValue(xx, bb, bb, xx);
                    Thread.Sleep(delayTime);
                    if (_goToNextDemo) break;
                    _display.SetValue(bb, bb, xx, xx);
                    Thread.Sleep(delayTime);
                    if (_goToNextDemo) break;
                    _display.SetValue(bl, xx, xx, xx);
                    Thread.Sleep(delayTime);
                    if (_goToNextDemo) break;
                    _display.SetValue(ll, xx, xx, xx);
                    Thread.Sleep(delayTime);
                }
                _goToNextDemo = false;

                // Clock demo
                Debug.Print("Clock display");
                while (!_goToNextDemo)
                {
                    _display.SetTimeDisplay(DateTime.Now.TimeOfDay, TimeDisplayMode.Clock12hAMApostrophe);
                    Thread.Sleep(100);
                }
                _display.SetColon(false);
                _display.SetApostrophe(false);
                _goToNextDemo = false;

                // Stopwatch demo
                Debug.Print("Stopwatch display");
                TimeSpan timeSpan = new TimeSpan();
                while (!_goToNextDemo)
                {
                    _display.SetTimeDisplay(timeSpan, TimeDisplayMode.MinuteSecond);
                    timeSpan = new TimeSpan(0, timeSpan.Minutes, timeSpan.Seconds+1);
                    Thread.Sleep(10);
                }
                _display.SetColon(false);
                _display.SetApostrophe(false);
                _goToNextDemo = false;
            }
        }

        static void button_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            _goToNextDemo = true;
        }
    }
}
