
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
            string BindIP = "0.0.0.0";
            int Portnumber = 23;
            string Value = null;
            string MOTD = "Welcome to the TCPServer";

            foreach (string arg in args) // Checking all command line arguments
            {

                /* Parse args
                  "--" means start of argument
                  ":" means arument / value separator
                 */

                string option;
                
                if (arg.IndexOf(':') > 1)
                {
                    string[] words = arg.Split(':');
                    option = words[0].ToUpper();
                    Value = words[1].ToUpper();  //Optional   
                }
                else
                {
                    option = arg.ToUpper();
                }

                
                switch (option)
                {
                    case ("--BINDIP"):
                        BindIP = Value;
                        break;

                    case("--PORT"):
                        Portnumber = Convert.ToInt32(Value);
                        break;

                    case("--GUI"):
                        Console.WriteLine("Starting GUI");
                        Form1 myForm = new Form1();
                        myForm.Show();
                        break;

                    case ("--MOTD"):
                        MOTD = Value;
                        break;

                    default:
                        break;
                }

            }

            Console.WriteLine("Starting with options");
            Console.WriteLine("BindIP : {0}", BindIP);
            Console.WriteLine("Port : {0}", Portnumber );
            bool MOTDSent = false;
            IPAddress HOST = IPAddress.Parse(BindIP);
            IPEndPoint serverEP = new IPEndPoint(HOST, Portnumber);
            Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sck.Bind(serverEP);
            sck.Listen(Portnumber); // <-- Denne må vel være feil? 

            try
            {
                Console.WriteLine("Listening for clients...");
                Console.WriteLine("Press Ctrl-X to exit");

                // TODO : Fix this... Console keys not being read before connect.
                Socket msg = sck.Accept();
                Console.WriteLine("First data");
                
                ConsoleKeyInfo cki = new ConsoleKeyInfo(); //Keypress

                // Sending MOTD
                byte[] buffer = Encoding.Default.GetBytes(MOTD);
                msg.Send(buffer, 0, buffer.Length, 0);
                buffer = new byte[255];

                while (true)
                {
                    if (msg.Connected)
                    {
                        if (!MOTDSent)
                        {

                        }
                    }
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
                            Environment.Exit(1);
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
