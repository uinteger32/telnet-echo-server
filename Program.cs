using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
class Program
{
    static void Main(string[] args)
    {
        // Simple Telnet server   
        TelnetEchoServer server = new TelnetEchoServer();
        server.BeginAcceptClients();
    }
}

class TelnetEchoServer
{
    TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 23);

    public TelnetEchoServer()
    {
        listener.Start();
    }

    public void BeginAcceptClients()
    {
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread t = new Thread(new ParameterizedThreadStart(Receive));
            t.Start((object)client);
        }
    }

    void Receive(object obj)
    {
        TcpClient client = (TcpClient)obj;

        NetworkStream ns = client.GetStream();
        while (client.Connected)
        {
            byte[] buffer = new byte[4096];
            int length = ns.Read(buffer);
            if (length <= 0)
            {
                break;
            }
            byte[] data = new byte[length];
            Array.Copy(buffer, data, length);

            string dataString = $"ECHO: {Encoding.ASCII.GetString(data)}\r\n";

            ns.Write(Encoding.ASCII.GetBytes(dataString));
        }
        ns.Close();
        client.Close();
    }
}