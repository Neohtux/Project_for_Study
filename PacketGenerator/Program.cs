using System;
using System.IO;
using System.Text;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static string genPacket;
        static void Main(string[] args)
        {
            string pdlPath = "../PacketDefinitonList.xml";

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true, //주석 무시
                IgnoreWhitespace = true //스페이스바 무시
            
            };
            if (args.Length >= 1)
                pdlPath = args[0];

            //using 블록 내부에서 자동으로 r.Dispose() 만들어줌.
            using (XmlReader r = XmlReader.Create(pdlPath, settings))
            {
                r.MoveToContent(); //헤더를 건너뛰고 컨텐츠로 이동
                string fileText = PacketFormat.fileFomrat;
                fileText += "\n";
                genPacket += fileText;
                while (r.Read())
                {
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element) // <PDL> 내부 깊이
                    {
                        ParsePacket(r);
                        Console.WriteLine(r.Name + " " + r["name"]);
                    }              
                    
                }
                
                File.WriteAllText("GenPacket.cs", genPacket);
            }
            
        }

        public static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement) return;

            if (r.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid packet name");
                return;
            }

            string packetName = r["name"];

            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without name");
                return;
            }
          
            Tuple<string, string, string> t = ParseMember(r);
            genPacket += string.Format(PacketFormat.packetFormat,
               packetName, t.Item1, t.Item2, t.Item3);


        }
        //{1} 멤버 변수
        //{2} 멤버 변수 Des
        //{3} 멤버 변수 Ser
        public static Tuple<string,string,string> ParseMember(XmlReader r)
        {
            string packetName = r["name"];

            string memberCode = "";
            string desCode = "";
            string serCode = "";

            int depth = r.Depth + 1; //파싱하려고 하는 위치
            while(r.Read()) //XML의 내용부분을 한줄씩 읽음
            {
                if (r.Depth != depth) break;


                string memberName = r["name"];
                if(string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return null;
                }

                if (string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine; //엔터키.한줄 한줄 거쳐서 표현
                if (string.IsNullOrEmpty(desCode) == false)
                    desCode += Environment.NewLine;
                if (string.IsNullOrEmpty(serCode) == false)
                    serCode += Environment.NewLine;

                string memberType = r.Name.ToLower();
                switch(memberType)
                {
                    case "ushort":
                        serCode += string.Format(PacketFormat.serFormat,memberName,memberType);
                        desCode += string.Format(PacketFormat.desFormat, memberName,ToMemberType(memberType), memberType);
               
                        break;
                    case "ulong":
                        serCode += string.Format(PacketFormat.serFormat, memberName, memberType);
                        desCode += string.Format(PacketFormat.desFormat, memberName, ToMemberType(memberType), memberType);
                        //    serCode = string.Format(PacketFormat.serFormat, memberName, memberType);
                        break;
                    case "string":
                        break;
                    case "int":
                        break;

                }
            }
            memberCode = memberCode.Replace("\n", "\n\t");
            desCode = desCode.Replace("\n", "\n\t\t");
            serCode = serCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, serCode, desCode);
        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "ushort":
                    return "ToUInt16";
                    break;
                case "ulong":
                    return "ToUInt64";
                    break;
                case "int":
                    return "ToInt32";
                    break;
                    
            }
            return "";
        }

    }
}
