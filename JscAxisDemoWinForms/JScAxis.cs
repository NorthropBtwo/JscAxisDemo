using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;
using System.Collections.Concurrent;
using System.Collections;
using System.Windows.Forms;

namespace JscAxisDemoWinForms
{
    public class JScAxis : IDisposable
    {
        public enum State
        {
            POWER_OFF = 0,
            POWER_ON = 1,
            MOVING = 2,
            ERROR = 9,
        }

        TcpClient? client = null;
        Task? receiveTask = null;
        object clientLock = new object();
        State currentState = State.POWER_OFF;

        public event Action<String>? OnReceive;
        public event Action<State>? OnStateChanged;

        public void Connect(IPAddress ip)
        {
            if (client == null)
            {
                client = new();
                client.Connect(ip, 10001);
                receiveTask = Task.Run(() => Receive());

                BlockingCollection<string> queue = new BlockingCollection<string>();
                OnReceive += queue.Add;
                Send("EVT1");
                while (!queue.Take().Contains("EVT1")) ;
                OnReceive -= queue.Add;
            }
        }

        public void PowerContinuous(CancellationToken token = new CancellationToken())
        {
            if (currentState != State.POWER_ON && currentState != State.MOVING)
            {
                BlockingCollection<State> queue = new BlockingCollection<State>();
                OnStateChanged += queue.Add;
                Send("PWC");
                while (queue.Take(token) != State.POWER_ON) ;
                OnStateChanged -= queue.Add;
            }
        }

        public void PowerQuit(CancellationToken token = new CancellationToken())
        {
            if (currentState != State.POWER_OFF)
            {
                BlockingCollection<State> queue = new BlockingCollection<State>();
                OnStateChanged += queue.Add;
                Send("PQ");
                while (queue.Take(token) != State.POWER_OFF) ;
                OnStateChanged -= queue.Add;
            }
        }

        public void StopMoition(CancellationToken token = new CancellationToken())
        {
            if (currentState == State.MOVING)
            {
                BlockingCollection<State> queue = new BlockingCollection<State>();
                OnStateChanged += queue.Add;
                Send("SM");
                while (queue.Take(token) == State.MOVING) ;
                OnStateChanged -= queue.Add;
            }
        }


        public void GoPosition(int position, CancellationToken token = new CancellationToken())
        {
            BlockingCollection<State> queue = new BlockingCollection<State>();
            OnStateChanged += queue.Add;
            Send("G" + position);
            while (queue.Take(token) != State.MOVING); //wait until moving
            while (queue.Take(token) == State.MOVING); //wait until not moving
            OnStateChanged -= queue.Add;
        }

        public async Task GoPositionAsync(int position, CancellationToken token = new CancellationToken())
        {
            BufferBlock<State> queue = new();
            Action<State> action = (s) => queue.Post(s);
            OnStateChanged += action;
            Send("G" + position);
            while (await queue.ReceiveAsync(token) != State.MOVING); //wait until moving
            while (await queue.ReceiveAsync(token) == State.MOVING); //wait until not moving
            OnStateChanged -= action;
        }

        public int TellPosition(CancellationToken token = new CancellationToken())
        {
            BlockingCollection<String> queue = new();
            OnReceive += queue.Add;
            Send("TP");
            string answer;
            do
            {
                answer = queue.Take(token);
            } while (!answer.StartsWith("TP"));
            OnReceive -= queue.Add;
            return int.Parse(answer.Split('\n')[1]);
        }

        public void Send(string message)
        {
            lock (clientLock)
            {
                if (client != null)
                {
                    var stream = client.GetStream();
                    var buffer = Encoding.UTF8.GetBytes(message + Environment.NewLine);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private void Receive()
        {
            if (client != null)
            {
                var stream = client.GetStream();
                byte[] buffer = new byte[4096];
                string strBuffer = "";
                int readLenght;
                do
                {
                    try
                    {
                        readLenght = stream.Read(buffer, 0, buffer.Length); /* throws an error if client was dicconected either from Disconnect method or if connection was lost */
                    } 
                    catch 
                    { 
                        readLenght = 0;
                    }
                    if (readLenght > 0)
                    {
                        strBuffer += Encoding.UTF8.GetString(buffer, 0, readLenght);
                        string[] recievedParts = strBuffer.Split('>');
                        for (int i = 0; i < recievedParts.Length-1; i++)
                        {
                            string received = recievedParts[i].Trim('\n', '\r');
                            OnReceive?.Invoke(received);
                            if (received == "@S0")
                            {
                                currentState = State.POWER_OFF;
                            }
                            else if (received == "@S1")
                            {
                                currentState = State.POWER_ON;
                            }
                            else if (received == "@S2")
                            {
                                currentState = State.MOVING;
                            }
                            else if (received == "@S9")
                            {
                                currentState = State.ERROR;
                            }
                            OnStateChanged?.Invoke(currentState);
                        }
                        strBuffer = recievedParts[recievedParts.Length - 1];
                    }
                } while (readLenght > 0);
            }
        }

        public void Disconnect()
        {
            lock (clientLock)
            {
                if (client != null)
                {
                    client.Dispose();
                    receiveTask?.Wait();
                    receiveTask = null;
                    client = null;
                }
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
