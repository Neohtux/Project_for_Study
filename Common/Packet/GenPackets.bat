start ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PacketDefinitonList.xml

XCOPY /Y GenPacket.cs "../../DummyClient/Packet/GenPacket.cs"
XCOPY /Y GenPacket.cs "../../Server_proj/Server/Packet/GenPacket.cs"
