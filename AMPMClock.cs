using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalClock
{
    internal class AMPMClock
    {
        public static bool isActive = false;
        // Making a dictionary to store a segment of the display, where the x, y position is the key
        static Dictionary<(int, int), Segment> currentSegment = [];

        public static void Run()
        {
            currentSegment = [];
            Console.CursorVisible = false;
            Thread thread = new(multiThread);
            thread.Start();
            while (isActive)
            {
                RenderBox(0, 0);
                RenderTime(3, 2);
                Thread.Sleep(1000);
            }
        }

        static void multiThread()
        {
            while (isActive)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Tab:
                        MilitaryClock.isActive = true;
                        AMPMClock.isActive = false;
                        Console.Clear();
                        MilitaryClock.Run();
                        break;

                }
            }
        }

        // Render the time to the console at the given x, y position
        static void RenderTime(int x, int y)
        {
            // Get the current time in the format HH:mm:ss tt (12-hour format with AM/PM)
            string time = DateTime.Now.ToString("hh:mm:ss tt", new CultureInfo("en-US"));
            // Set the offset to the x position
            int offset = x;

            // Loop through each character in the time string
            for (int i = 0; i < time.Length; i++)
            {
                // If the character is a colon, then render the colon separator
                if (time[i] == ':')
                {
                    RenderColon(offset, y + 4);
                    offset += 2;
                }
                // If the character is a space, skip rendering
                else if (time[i] == ' ')
                {
                    offset += 2;
                }
                // If the character is anything other than a colon or space, then render the segment of the display
                else
                {
                    Segment segment = GetSegment(time[i]);
                    RenderSegment(segment, offset, y);
                    offset += 8;
                }
            }
        }

        // I use a struct to make rendering each segment possible with a boolean value
        public struct Segment
        {
            public bool A;
            public bool B;
            public bool C;
            public bool D;
            public bool E;
            public bool F;
            public bool G;
            public bool H;
        }

        // Get the boolean values for each segment of the display
        public static Segment GetSegment(char c)
        {
            switch (c)
            {
                case '0':
                    return new Segment { A = true, B = true, C = true, D = true, E = true, F = true, G = false, H = false };
                case '1':
                    return new Segment { A = false, B = true, C = true, D = false, E = false, F = false, G = false, H = false };
                case '2':
                    return new Segment { A = true, B = true, C = false, D = true, E = true, F = false, G = true, H = false };
                case '3':
                    return new Segment { A = true, B = true, C = true, D = true, E = false, F = false, G = true, H = false };
                case '4':
                    return new Segment { A = false, B = true, C = true, D = false, E = false, F = true, G = true, H = false };
                case '5':
                    return new Segment { A = true, B = false, C = true, D = true, E = false, F = true, G = true, H = false };
                case '6':
                    return new Segment { A = true, B = false, C = true, D = true, E = true, F = true, G = true, H = false };
                case '7':
                    return new Segment { A = true, B = true, C = true, D = false, E = false, F = false, G = false, H = false };
                case '8':
                    return new Segment { A = true, B = true, C = true, D = true, E = true, F = true, G = true, H = false };
                case '9':
                    return new Segment { A = true, B = true, C = true, D = true, E = false, F = true, G = true, H = false };
                case 'A':
                    return new Segment { A = true, B = true, C = true, D = false, E = true, F = true, G = true, H = false };
                case 'P':
                    return new Segment { A = true, B = true, C = false, D = false, E = true, F = true, G = true, H = false };
                case 'M':
                    return new Segment { A = true, B = true, C = true, D = false, E = true, F = true, G = false, H = true };
                default:
                    return new Segment { A = false, B = false, C = false, D = false, E = false, F = false, G = false, H = false };
            }
        }

        // Render a segment of the display at the given x, y position
        // A segment is made up of a string of 0's and spaces
        // Using the Render method to render each segment of the display
        static void RenderSegment(Segment segment, int x, int y)
        {
            // Check the dictionary to see if the segment value is already true for the given x, y position
            var key = (x, y);
            // If the segment value is already true, then we skip the rendering of the segment
            if (currentSegment.ContainsKey(key) && currentSegment[key].Equals(segment))
            {
                return;
            }
            // If the segment value is opposite, then we render the segment
            currentSegment[key] = segment;

            // Render the A segment
            Render(x + 1, y, segment.A ? "00000" : "     ");

            // Render the B and C segments, using a for loop to render the 5 rows on the same x position
            for (int i = 0; i < 5; i++)
            {
                Render(x + 6, y + 1 + i, segment.B ? "0" : " ");
                Render(x + 6, y + 7 + i, segment.C ? "0" : " ");
            }

            // Render the D segment
            Render(x + 1, y + 12, segment.D ? "00000" : "     ");

            // Render the E and F segments, using a for loop to render the 5 rows on the same x position
            for (int i = 0; i < 5; i++)
            {
                Render(x, y + 7 + i, segment.E ? "0" : " ");
                Render(x, y + 1 + i, segment.F ? "0" : " ");
            }

            // Render the G segment
            Render(x + 1, y + 6, segment.G ? "00000" : "     ");

            // Render the H segment
            if (segment.H) Render(x + 3, y, " ");
            for (int i = 0; i < 3; i++)
            {
                Render(x + 3, y + 1 + i, segment.H ? "0" : " ");
            }
        }

        // Render a string to the console at the given x, y position
        // Render each character in the string as a color block
        static void Render(int x, int y, string str)
        {
            Console.SetCursorPosition(x, y);
            // Loop through each character in the string and set the background and foreground color
            foreach (char ch in str)
            {
                // If the character is '0', then set the background and foreground color to white
                if (ch == '0')
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                // If the character is anything other than a 0, then set the background and foreground color to black
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                // Write the character to the console (color block)
                Console.Write(ch);
            }
            // Reset the background and foreground color to black
            Console.ResetColor();
        }

        // Render the colon separator
        static void RenderColon(int x, int y)
        {
            // Render the top and bottom dots of the colon
            Render(x, y, "0");
            Render(x, y + 3, "0");
        }

        // Render a box around the clock
        static void RenderBox(int x, int y)
        {
            int height = 13 + 4;
            int width = (6 * 8) + (2 * 8) + (2 * 5) + 2;

            Render(x, y, new string('0', width));
            for (int i = 0; i < height; i++)
            {
                Render(x, y + i, "0");
                Render(x + width, y + i, "0");
            }
            Render(x, y + 16, new string('0', width));
        }
    }
}

