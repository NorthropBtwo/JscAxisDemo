﻿using System;
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
using System.Net.Http.Headers;

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
            REFERENCED = 10,
        }

        TcpClient? client = null;
        Task? receiveTask = null;
        object clientLock = new object();
        State currentState = State.POWER_OFF;
        bool hasError = false;
        bool isPowered = false;

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

        public void Reference(CancellationToken token = new CancellationToken())
        {
            State? newState = null;
            if (currentState != State.MOVING)
            {
                BlockingCollection<State> queue = new BlockingCollection<State>();
                State oldState = currentState;
                OnStateChanged += queue.Add;
                Send("REF");
                try
                {
                    CancellationToken currentToken = token;
                    bool axisWasMovingOnce = false;
                    while ((newState = queue.Take(currentToken)) != State.REFERENCED)
                    {
                        if ((newState == State.POWER_OFF && oldState != State.ERROR) || newState == State.ERROR) //State.POWER_OFF is only ok if we recover from an Error
                        {
                            throw new Exception("error during reference");
                        }
                        else if (newState == State.POWER_ON && axisWasMovingOnce) //the axis moved and stopped now
                        {
                            currentToken = new CancellationTokenSource(500).Token; //after axis stopped, Reference event should follow soon. Otherwise, it was no succesful
                        }
                        else if (newState == State.MOVING)
                        {
                            axisWasMovingOnce = true;
                            currentToken = token;
                        }
                        else
                        {
                            currentToken = token;
                        }

                        oldState = (State)newState;
                    }
                }
                catch (OperationCanceledException) { throw; }
                finally
                {
                    OnStateChanged -= queue.Add;
                }
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
            if (!hasError || isPowered) /* do not execute if error is active except softlimit error is active */
            {
                BlockingCollection<State> queue = new BlockingCollection<State>();
                State newState;
                OnStateChanged += queue.Add;
                Send("G" + position);
                while ((newState = queue.Take(token)) != State.MOVING && newState != State.ERROR) ; //wait until moving or error
                while (newState != State.ERROR && (newState = queue.Take(token)) == State.MOVING) ;  //wait until not moving or error
                OnStateChanged -= queue.Add;
            }
        }

        public async Task GoPositionAsync(int position, CancellationToken token = new CancellationToken())
        {
            if (!hasError || isPowered) /* do not execute if error is active except softlimit error is active */
            {
                BufferBlock<State> queue = new();
                State newState;
                Action<State> action = (s) => queue.Post(s);
                OnStateChanged += action;
                Send("G" + position);
                while ((newState = await queue.ReceiveAsync(token)) != State.MOVING && newState != State.ERROR) ; //wait until moving or error
                while (newState != State.ERROR && (newState = await queue.ReceiveAsync(token)) == State.MOVING) ;  //wait until not moving or error
                OnStateChanged -= action;
            }
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

        public string TellErrorString(CancellationToken token = new CancellationToken())
        {
            BlockingCollection<String> queue = new();
            OnReceive += queue.Add;
            Send("TES");
            string answer;
            do
            {
                answer = queue.Take(token);
            } while (!answer.StartsWith("TES"));
            OnReceive -= queue.Add;
            return answer.Split('\n')[1].Trim('\r');
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
                        for (int i = 0; i < recievedParts.Length - 1; i++)
                        {
                            string received = recievedParts[i].Trim('\n', '\r');
                            OnReceive?.Invoke(received);
                            if (received == "@S0")
                            {
                                currentState = State.POWER_OFF;
                                hasError = false;
                                isPowered = false;
                                OnStateChanged?.Invoke(currentState);
                            }
                            else if (received == "@S1")
                            {
                                currentState = State.POWER_ON;
                                hasError = false;
                                isPowered = true;
                                OnStateChanged?.Invoke(currentState);
                            }
                            else if (received == "@S2")
                            {
                                currentState = State.MOVING;
                                hasError = false;
                                OnStateChanged?.Invoke(currentState);
                            }
                            else if (received == "@S9")
                            {
                                currentState = State.ERROR;
                                hasError = true;
                                OnStateChanged?.Invoke(currentState);
                            }
                            else if (received == "@H")
                            {
                                OnStateChanged?.Invoke(State.REFERENCED);
                            }
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
