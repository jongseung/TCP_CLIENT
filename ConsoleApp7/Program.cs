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
    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
