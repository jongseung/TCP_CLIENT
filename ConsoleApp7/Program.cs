using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
/*
* TCP통신 기반의 1Room 멀티채팅 서버 프로그램
* 기능)
* 메인스레드
* 서버 설정, 클라이언트 접속대기, 접속한 클라이언트를 매개변수로 서브스레드 생성
* 서브스레드
* 클라이언트가 송신하는 채팅내용을 다른 클라이언트에게 송신
* 서브스레드2...n
* 클라이언트가 송신 데이터를 수신 및 채팅내용을 저장변수에 전달
*/

namespace TCP_CLIENT
{
    //접속한 클라이언트를 관리하는 클래스
    class User
    {
        //접속한 클라이언트의 소켓, 스트림
        public TcpClient socket;        //나중에는 접근지정자 변경해주는게 좋다. 
        public NetworkStream stream;
        //BinaryFormatter 객체
        public BinaryFormatter formatter;
        //ID값 저장변수
        public string ID;

        public User(TcpClient client)
        {
            socket = client;
            stream = socket.GetStream();
            formatter = new BinaryFormatter();
        }
        //서브스레드 - 서버가 송신한 데이터를 수신받아 메세지 화면에 출력
        public void RecvData()
        {
            for(; ; )
            {
                try
                {
                    //chat 변수와 ID값을 합침
                    string chat = (string)formatter.Deserialize(stream);

                    Console.WriteLine(chat);
                }
                catch //데이터 수신 대기중 서버가 연결을 끊었을때의 예외처리
                {
                    Console.WriteLine("수신대기중 연결된 서버가 연결을 끊음");
                    break;
                }
            }
            Close();
        }
        // 해당 서버에게 데이터를 송신하는 메소드
        public bool SendData(string chat)
        {
            bool isConnected = false; // 안되는경우로 하는게 보편적이다.
            //조건문 - 서버와 다른스레드에서 이미 연결을 끊었는지 확인
            if (socket.Connected)
            {

                try
                {
                    //연결된 스트림으로 문자열을 전송
                    formatter.Serialize(stream, chat);
                    //Serialize에서 예외가 발생하지 않으면 연결이 유지되었음을 확인함
                    isConnected = true;
                }
                catch //서버와 연결을 끊은 경우의 예외처리
                {
                    Console.WriteLine("서버와 연결을 끊음");
                    Close();
                }
            }
            return isConnected;
        }
        //클라이언트 연결종료 메소드
        public void Close()
        {
            socket.Close();
            stream.Close();
        }

    }
    class program
    {

        static void Main(string[] args)
        {
            User user;
            while (true)
            {

                try
                {
                    TcpClient client = new TcpClient("127.0.0.1", 15000);
                    user = new User(client);
                    break;
                }
                catch //서버가 닫혀있는 경우의 예외처리
                {
                    Console.WriteLine("서버가 응답하지 않음...");
                    Thread.Sleep(5000);
                }
            }
            //ID 입력 및 송신
            Console.Write("ID 입력 : ");
            user.ID = Console.ReadLine();
            user.SendData(user.ID);

            Task recv_task = new Task(new Action(user.RecvData));
            recv_task.Start();
            for(; ; )
            {
                string chat = Console.ReadLine();
                bool isConnected = user.SendData(chat);
                if (!isConnected)
                {
                    Console.WriteLine("서버 연결 끊김");
                    break;
                }
            }
            user.Close();
        }

    }
}