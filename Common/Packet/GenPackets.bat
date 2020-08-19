start ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PacketDefinitonList.xml

XCOPY /Y GenPackets.cs "../../DummyClient/Packet""
XCOPY /Y GenPackets.cs "../../Server_proj/Server/Packet""
