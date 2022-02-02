using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Linq;
using Comun;
namespace Calculator.Cliente
{
    internal class Program
    {
        static void Main(string[] args)
        {
            resultado r = new resultado();
            Console.WriteLine("Console C#\r");
            Console.WriteLine("------------------------\n");

            while (true)
            {
                Operacion op = new Operacion();
                Console.WriteLine("Escribe el primer numero");
                op.operando1 = Convert.ToDouble(Console.ReadLine());
                //Console.WriteLine("Mensaje:");

                Console.WriteLine("Escribe el segundo número");
                op.operando2 = Convert.ToDouble(Console.ReadLine());

                 Console.WriteLine("Escribe una opración");
                string opc = Console.ReadLine();
                switch (opc)
                {
                    case "suma":
                        op.tipoOperaciones = TipoOperacion.suma;
                        break;
                    case "resta":
                        op.tipoOperaciones = TipoOperacion.resta;
                        break;
                    case "multiplicacion":
                        op.tipoOperaciones = TipoOperacion.multiplicacion;
                        break;
                    case "division":
                        op.tipoOperaciones = TipoOperacion.division;
                        break;
                }
                String mensaje = JsonSerializer.Serialize(op);
                var resultado = EnviaMenaje(mensaje);
                

               
                Console.WriteLine(r.result);
            }

        }

        static string EnviaMenaje(string mensaje)
        {
            resultado r = new resultado();
            try
            {
                // Connect to a Remote server
                // Get Host IP Address that is used to establish a connection
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1
                // If a host has multiple addresses, you will get a list of addresses
                
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = host.AddressList[0];

                //IPAddress ipAddress = IPAddress.Parse("ip destino");

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 2800);

                // Create a TCP/IP  socket.
                using Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    // Connect to Remote EndPoint
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    Console.WriteLine("Socket redad for {0}",
                        sender.LocalEndPoint.ToString());

                    var cacheEnvio = Encoding.UTF8.GetBytes(mensaje);

                    // Send the data through the socket.
                    int bytesSend = sender.Send(cacheEnvio);

                    // Receive the response from the remote device.
                    byte[] bufferRec = new byte[1024];
                    int bytesRec1 = sender.Receive(bufferRec);

                    var resultado = Encoding.UTF8.GetString(bufferRec, 0, bytesRec1);
                    r = JsonSerializer.Deserialize<resultado>(resultado);
                    Console.WriteLine("El resultado es " + r.result);

                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                    return resultado;

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;
        }
    }
}
