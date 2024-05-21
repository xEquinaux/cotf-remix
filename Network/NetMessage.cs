using Microsoft.Win32.SafeHandles;
using SharpDX.MediaFoundation.DirectX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cotf.Network
{
    public class NetMessage
    {
        internal static NetworkStream Local;
        internal static NetPlayer[] stream = new NetPlayer[256];
        private static BinaryWriter writer;
        private static BinaryReader reader;
        protected static BinaryReader Reader(int whoAmI) => new BinaryReader(stream[whoAmI].Stream);
        protected static BinaryWriter Writer(int whoAmI) => new BinaryWriter(stream[whoAmI].Stream);
        internal Player instance => Main.myPlayer;

        internal static async void Initialize(IPAddress ip, int port = 8000, byte whoAmI = 0)
        {
            if (Main.netMode == NetModeID.Singleplayer)
                return;
            if (Main.netMode == NetModeID.MultiplayerClient)
            {
                TcpClient tcp = new TcpClient();
                UdpClient udp = new UdpClient();
                //  Primary network handling
                int tries = 0, max = 15;
                do
                {
                    try
                    {
                        tcp.Connect(ip, port);
                        udp.Client = tcp.Client;
                    }
                    catch 
                    {
                        Task.WaitAny(new[] { Task.Delay(1000) });
                        continue; 
                    }
                    Task.WaitAny(new[] { Task.Delay(1000) });
                } while (!udp.Client.Connected && tries++ < max);
                Local = new NetworkStream(udp.Client);
                writer = new BinaryWriter(Local);
                reader = new BinaryReader(Local);
                HandlePackets();
            }
            if (Main.netMode == NetModeID.Server)
            {
                //  Begin listen
                UdpListener listen = new UdpListener(IPAddress.Any, port);
                listen.Start(255);
                UdpClient client = listen.AcceptClient();
                NetworkStream net = new NetworkStream(client.Client);
                //  Temporary NetPlayer object
                NetPlayer player = new NetPlayer()
                {
                    Stream = net,
                    Udp = client
                };
                //  Init stream
                var first = stream.FirstOrDefault(t => t == null);
                if (first == default)
                {
                    //  Either run who-is-connected check and remove disconnected entries
                    //  or deny connection, server being full
                    return;
                }
                whoAmI = (byte)stream.ToList().IndexOf(first);
                //  Get player data
                stream[whoAmI] = await RecieveData(player, whoAmI);
            }
        }
        private static Task<NetPlayer> RecieveData(NetPlayer player, int whoAmI)
        {
            return new Task<NetPlayer>(() => 
            {
                Wait(player);
                WaitHandle.WaitAny(new[] { player.wait });
                if (!player.hasData) return null;
                //  Give index
                player.whoAmI = whoAmI;
                //  Process data
                SendData(PacketID.PlayerData, whoAmI, -1, new NetInfo(new Packet(PacketID.PlayerData, whoAmI, -1)));
                //  Load and handle Player objects
                SendData(PacketID.SyncPlayerData, whoAmI, -1, new NetInfo(new Packet(PacketID.SyncPlayerData, whoAmI, -1)));
                return player;
            });
        }
        private static void Wait(NetPlayer player)
        {
            Task.Factory.StartNew(() => 
            {
                int tries = 0, max = 30;
                while (player.Connected && tries++ < max)
                {
                    Task.WaitAll(new[] {Task.Delay(1000)});
                    if (player.Stream.DataAvailable)
                    {
                        player.hasData = true;
                        player.wait.Set();
                    }
                }
                player.hasData = false;
                player.wait.Set();
            });
        }
        private static void HandlePackets()
        {
            Task.Factory.StartNew(() => 
            { 
                if (Main.netMode == NetModeID.Singleplayer)
                    return;
                while (Local.Socket.Connected)
                {
                    if (!Local.DataAvailable)
                    {
                        Task.WaitAll(new[] { Task.Delay(5) });
                        continue;
                    }
                    Packet get = GetPacket();
                    RecieveData(get.packet, get.to, get.from, null, get.num, get.num2, get.num3);
                }
            });
        }
        private static Packet GetPacket()
        {
            Packet packet = new Packet();
            long start = 0;
            int tries = 0, max = 10;
            while (tries++ < max)
            {
                try
                {
                    start = reader.ReadInt64();
                    break;
                }
                catch 
                {
                    continue;
                }
            }
            if (start == -100L)
            { 
                packet.from = reader.ReadByte();
                packet.to = reader.ReadByte();
                packet.packet = reader.ReadByte();
                packet.num  = reader.ReadInt32();
                packet.num2 = reader.ReadInt32();
                packet.num3 = reader.ReadInt32();
                return packet;
            }
            return null;
        }
        private static void RecieveData(byte packet, int toWhom = -1, int fromWhom = -1, NetInfo info = null, int num = 0, int num2 = 0, int num3 = 0)
        {
            if (Main.netMode == NetModeID.Singleplayer)
                return;
            if (Main.netMode == NetModeID.MultiplayerClient)
            {
                switch (packet)
                {
                    case PacketID.JoinWorld:
                        break;
                    case PacketID.PlayerData:
                        //  Getting whoAmI from toWhom specifically recieved by player who recently connected
                        Main.myPlayer.whoAmI = toWhom;
                        Main.player[toWhom] = Main.myPlayer;
                        break;
                }
            }
            if (Main.netMode == NetModeID.Server)
            {

            }
        }
        private static void SendData(NetPlayer player, byte packet, int toWhom = -1, int fromWhom = -1, NetInfo info = null, int num = 0, int num2 = 0, int num3 = 0)
        {
            if (Main.netMode == NetModeID.Singleplayer)
                return;
            if (Main.netMode == NetModeID.MultiplayerClient)
            {
                switch (packet)
                {

                }
            }
            if (Main.netMode == NetModeID.Server)
            {

            }
        }
        public static void SendData(byte packet, int toWhom = -1, int fromWhom = -1, NetInfo info = null, int num = 0, int num2 = 0, int num3 = 0)
        {
            if (Main.netMode == NetModeID.Singleplayer)
                return;
            if (Main.netMode == NetModeID.MultiplayerClient)
            {
                switch (packet)
                {

                }
            }
            if (Main.netMode == NetModeID.Server)
            {
                Packet send = info?.packet;
                switch (packet)
                {
                    case PacketID.PlayerData:
                        //  Generic send of whoAmI data
                        send.Send(packet, toWhom, info);
                        break;
                }
            }
        }
    }
    public class Packet
    {
        public Packet()
        {
        }
        public Packet(byte id, int to, int from)
        {
            this.packet = id;
            this.to = to;
            this.from = from;
        }
        public const long 
            START = -100;
        public byte packet;
        public int to, from;
        public int num, num2, num3;
        private BinaryWriter write;
        public void Send(byte packet, int toWhom = 255, NetInfo info = null)
        {
            if (NetMessage.stream[toWhom] == null)
                return;
            write = new BinaryWriter(NetMessage.stream[toWhom].Stream);
            write.Write(-100L);
            write.Write(from);
            write.Write(to);
            write.Write(packet); 
            write.Write(num);
            write.Write(num2);
            write.Write(num3);
        }
    }
    public class PacketID
    {
        public const byte
            JoinWorld = 0,
            PlayerData = 1,
            SyncPlayerData = 2;
    }
    public class NetPlayer
    {
        internal bool hasData;
        internal bool Available => Stream.DataAvailable;
        public bool Connected => Udp.Client.Connected;
        public int whoAmI
        {
            get { return instance.whoAmI; }
            set { instance.whoAmI = value; }
        }
        public UdpClient Udp { get; internal set; }
        public NetworkStream Stream { get; internal set; }
        public AutoResetEvent wait = new AutoResetEvent(true);
        public Player instance;
    }
    public class NetInfo
    {
        public NetInfo(Packet packet)
        {
            this.packet = packet;
        }
        public string text;
        public Packet packet;
    }
    public sealed class NetMessageID
    {
        public const byte
            SyncPlayer = 0;
    }
    public sealed class NetModeID
    {
        public const int
            Singleplayer = 0,
            MultiplayerClient = 1,
            Server = 2;
    }
    public sealed class UdpListener
    {
        TcpListener listen;
        ~UdpListener() { }
        public UdpListener(IPAddress ip, int port)
        {
            listen = new TcpListener(ip, port);
        }
        public void Start(int backlog)
        {
            listen.Start(backlog);
        }
        public UdpClient AcceptClient()
        {
            UdpClient client = new UdpClient();
            client.Client = listen.AcceptSocket();
            return client;
        }
    }
}
