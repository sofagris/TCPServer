
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string Hostname = "0.0.0.0";
            int Portnumber = 23;
            bool Gui = false;

            foreach (string arg in args) // Checking all command line arguments
            {

                /* Parse args
                 * "--" means start of argument
                 * ":" means arument / value separator

                 */
 
                string option;
                string value; //Optional

                option = "--HOST"; // Just dummy variable to avoid build error
                // value = "0.0.0.0";

                switch (option)
                {
                    case("--HOST"):
                        // Hostname = value;
                        break;

                    case("--PORT"):
                        Portnumber = Convert.ToInt32(value);
                        break;

                    case("--GUI"):
                        Gui = true;
                        break;
                }

            }

            IPAddress HOST = IPAddress.Parse("0.0.0.0");
            IPEndPoint serverEP = new IPEndPoint(HOST, 23);
            Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sck.Bind(serverEP);
            sck.Listen(443); // <-- Denne må vel være feil? 

            try
            {
                Console.WriteLine("Listening for clients...");
                Console.WriteLine("Press Ctrl-X to exit");
                Socket msg = sck.Accept();

                ConsoleKeyInfo cki = new ConsoleKeyInfo(); //Keypress

                // Sending MOTD
                byte[] buffer = Encoding.Default.GetBytes("Welcome to the server of bibabeluba!!");
                msg.Send(buffer, 0, buffer.Length, 0);
                buffer = new byte[255];

                while (true)
                {
                    // Checking buffer for available data
                    if (msg.Available >= 1)
                    {
                        
                        // Reading buffer
                        int rec = msg.Receive(buffer, 0, buffer.Length, 0);
                        byte[] bufferReaction = Encoding.Default.GetBytes(rec.ToString());
                        Console.Write(Encoding.Default.GetString(buffer, 0, rec));
                    }
                    
                    // Checking for Key available
                    if (Console.KeyAvailable)
                    {
                        cki = Console.ReadKey(true); // Reading keypress
                        if (cki.Key == ConsoleKey.X && cki.Modifiers == ConsoleModifiers.Control)
                        {
                            sck.Close();
                            msg.Close();
                        }
                        // Sending data to client
                        byte[] cmdOutput = Encoding.Default.GetBytes(cki.Key.ToString());
                        msg.Send(cmdOutput, 0, cmdOutput.Length, 0);
                        cmdOutput = new byte[255];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
