using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPG.Network
{
    public class NetworkCore
    {
        public NetType NetType { get; private set; } = NetType.Alone;

        public Socket Server { get; private set; }

        public Socket Client { get; private set; }
        /// <summary>
        /// 用于存放 <see cref="NetType.NetHost"/> 模式下向该端连接的套接字.
        /// </summary>
        public Dictionary<string, Socket> Clients { get; private set; }

        /// <summary>
        /// 用于从远程端读取数据.
        /// </summary>
        public BinaryReader BinaryReader { get; private set; }

        /// <summary>
        /// 用于向远程端发送数据.
        /// </summary>
        public BinaryWriter BinaryWriter { get; private set; }

        /// <summary>
        /// 用于双端通讯的 <see cref="System.Net.Sockets.NetworkStream"/>.
        /// </summary>
        public NetworkStream NetworkStream { get; private set; }

        /// <summary>
        /// 用于存放向该端连接的套接字用于网络流通讯的 <see cref="System.Net.Sockets.NetworkStream"/>.
        /// </summary>
        public Dictionary<Socket, NetworkStream> ClientStreams { get; private set; }

        public DataPackageTransfer DataPackageTransfer { get; private set; } = new DataPackageTransfer();

        /// <summary>
        /// 参与工作的网络模块.
        /// </summary>
        public List<INetworkMode> NetworkModes { get; private set; } = new List<INetworkMode>();

        /// <summary>
        /// 启动服务器, 并将 <see cref="NetType"/> 变更为 <see cref="NetType.NetHost"/>.
        /// </summary>
        /// <param name="port">端口.</param>
        /// <param name="maxConnect">最大连接数.</param>
        public void StartServer(int port, int maxConnect)
        {
            NetType = NetType.NetHost;
            NetworkModes.Add(DataPackageTransfer);
            Clients = new Dictionary<string, Socket>();
            ClientStreams = new Dictionary<Socket, NetworkStream>();
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            Server.Bind(endPoint);
            Server.Listen(maxConnect);
            Thread listenClient = new Thread(() =>
            {
                while (true)
                {
                    Socket _connect = Server.Accept();
                    string _connectIP = _connect.RemoteEndPoint.ToString();
                    //LogCore.AddLog(string.Concat(_connectIP, " is connected."));
                    Clients.Add(_connectIP, _connect);
                    NetworkStream _networkStream = new NetworkStream(_connect, true);
                    ClientStreams.Add(_connect, _networkStream);
                }
            });
            listenClient.IsBackground = true;
            listenClient.Start();
        }

        /// <summary>
        /// 使用指定端口连接指定 IP 上的服务器.
        /// </summary>
        /// <param name="ip">IP.</param>
        /// <param name="port">端口.</param>
        public void Connect(string ip, int port)
        {
            NetType = NetType.NetClient;
            NetworkModes.Add(DataPackageTransfer);
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Client.Connect(endPoint);
        }

        /// <summary>
        /// 逻辑刷新.
        /// </summary>
        public void DoUpdate()
        {
            if (NetType == NetType.NetClient)
            {
                INetworkMode _networkMode;
                NetworkStream = new NetworkStream(Client, true);
                if (NetworkStream.DataAvailable)
                {
                    BinaryReader = new BinaryReader(NetworkStream, Encoding.UTF8);
                    for (int count = 0; count < NetworkModes.Count; count++)
                    {
                        _networkMode = NetworkModes[count];
                        _networkMode.ReceiveDatas(BinaryReader, NetModeState.Over);
                    }
                }
                BinaryWriter = new BinaryWriter(NetworkStream);
                for (int count = 0; count < NetworkModes.Count; count++)
                {
                    _networkMode = NetworkModes[count];
                    _networkMode.SendDatas(BinaryWriter, NetModeState.Over);
                }
                BinaryWriter.Flush();
            }
            else if (NetType == NetType.NetHost)
            {
                INetworkMode _networkMode;
                NetworkStream networkStream;
                for (int count = 0; count < ClientStreams.Count; count++)
                {
                    NetModeState _netModeState = NetModeState.Conduct;
                    networkStream = ClientStreams.Values.ElementAt(count);
                    BinaryReader = new BinaryReader(networkStream, Encoding.UTF8);
                    if (count == ClientStreams.Count - 1)
                        _netModeState = NetModeState.Over;
                    if (networkStream.DataAvailable)
                    {
                        for (int count2 = 0; count2 < NetworkModes.Count; count2++)
                        {
                            _networkMode = NetworkModes[count2];
                            _networkMode.ReceiveDatas(BinaryReader, _netModeState);
                        }
                    }
                    BinaryWriter = new BinaryWriter(networkStream);
                    for (int count3 = 0; count3 < NetworkModes.Count; count3++)
                    {
                        _networkMode = NetworkModes[count3];
                        _networkMode.SendDatas(BinaryWriter, _netModeState);
                    }
                    BinaryWriter.Flush();
                }
            }
        }
    }
}
