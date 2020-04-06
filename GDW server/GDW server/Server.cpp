#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include <string>
#include <iostream>
#include <vector>
#include <queue>
#include <thread>
#include <fstream>
#include <sstream>

#pragma comment(lib, "Ws2_32.lib")





struct UserData {
	std::string IP = "";
	std::string UserName = "";
	sockaddr_in Address;
	std::string Status = "NOT READY";

};

struct InboundMessagePacket {
	std::string SenderIP = "";
	std::string Message = "";
	sockaddr_in Address;
};

struct OutboundMessagePacket {
	std::string Message = "";
	std::string ToWho = "";
	std::vector<sockaddr_in> AddressesToSendTo;
	std::string FromWho = "";
};

int NumberOfLobbies = 0;
struct Lobby {
	std::vector<UserData> ConnectedUsers;
	std::vector<std::string> ChatLog;
	int LobbyID;
};

struct Score {
	std::string MyName;
	float MyScore = 999999.0f;
};

std::vector<Lobby> ActiveLobbies;
std::vector<UserData> ConnectedUsers;
std::queue<OutboundMessagePacket> MessagesToSend;
std::queue<InboundMessagePacket> MessagesReceived;
std::vector<Score> HighScores;

bool ServerRunning = true;


SOCKET server_socket;
const unsigned int BUF_LEN = 1024;

sockaddr_in fromAddr;
int fromlen = sizeof(fromAddr);
char recv_buf[BUF_LEN];



int GetHighScores() {
	using namespace std;
	ifstream MyFile;
	MyFile.open("Scores.txt");

	int NumberOfPlayers = 0;
	HighScores.clear();


	while (!MyFile.eof()) {
		string Name;
		string UserScore;
		getline(MyFile, Name);
		if (Name != "") {
			NumberOfPlayers++;
			getline(MyFile, UserScore);
			Score TempScore;
			TempScore.MyName = Name;
			TempScore.MyScore = stof(UserScore);
			HighScores.push_back(TempScore);
		}

	}



	MyFile.close();
	return NumberOfPlayers;
}

void StoreScore(std::string TempScore) {

	
	using namespace std;

	string PlayerName = "P" + to_string(GetHighScores());
	ofstream MyFile;
	MyFile.open("Scores.txt", ios::app);
	MyFile << PlayerName + '\n' + TempScore + '\n';
	Score Temp;
	Temp.MyName = PlayerName;
	cout << TempScore << endl;
	Temp.MyScore = stof(TempScore);
	HighScores.push_back(Temp);

	MyFile.close();

}

void SortScores() {
	GetHighScores();

	Score Highest;
	int HighestAt = 0;
	float HighestScore;
	Score SecondHighest;
	int SecondHighestAt = 0;
	float SecondHighestScore;
	Score ThirdHighest;
	int ThirdHighestAt = 0;

	for (int i = 0; i < HighScores.size(); i++) {
		if (HighScores[i].MyScore < Highest.MyScore) {
			Highest = HighScores[i];
			HighestAt = i;
		}
	}
	HighestScore = HighScores[HighestAt].MyScore;
	HighScores[HighestAt].MyScore = 999999.0f;

	for (int i = 0; i < HighScores.size(); i++) {
		if (HighScores[i].MyScore < SecondHighest.MyScore) {
			SecondHighest = HighScores[i];
			SecondHighestAt = i;
		}
	}

	SecondHighestScore = HighScores[SecondHighestAt].MyScore;
	HighScores[SecondHighestAt].MyScore = 999999.0f;
	for (int i = 0; i < HighScores.size(); i++) {
		if (HighScores[i].MyScore < ThirdHighest.MyScore) {
			ThirdHighest = HighScores[i];
			ThirdHighestAt = i;
		}
	}

	if (Highest.MyName == SecondHighest.MyName) {
		SecondHighest.MyName = "N/A";
	}
	if (Highest.MyName == ThirdHighest.MyName) {
		ThirdHighest.MyName = "N/A";
	}
	if (SecondHighest.MyName == ThirdHighest.MyName) {
		ThirdHighest.MyName = "N/A";
	}


	OutboundMessagePacket Temppckt;
	Temppckt.ToWho = "ALL";
	Temppckt.Message = "CS";
	Temppckt.Message += "\n";
	Temppckt.Message += "UPDSCORE";
	Temppckt.Message += "\n";
	Temppckt.Message += "1) " + Highest.MyName + " with: " + std::to_string(HighestScore);
	Temppckt.Message += "\n";
	Temppckt.Message += "2) " + SecondHighest.MyName + " with: " + std::to_string(SecondHighestScore);
	Temppckt.Message += "\n";
	Temppckt.Message += "3) " + ThirdHighest.MyName + " with: " + std::to_string(ThirdHighest.MyScore);

	MessagesToSend.push(Temppckt);
}



void SendToAll() {

	if (MessagesToSend.size() > 0) {
		OutboundMessagePacket MessagePacket = MessagesToSend.front();
		std::string Message = MessagePacket.Message;

		char* message = (char*)Message.c_str();

		for (int i = 0; i < ConnectedUsers.size(); i++) {
			//sockaddr_in tempaddr = ConnectedUsers[i].Address;

			if (sendto(server_socket, message, BUF_LEN, 0,
				(struct sockaddr*) & ConnectedUsers[i].Address, BUF_LEN) == SOCKET_ERROR) {
				printf("sendto() failed %d\n", WSAGetLastError());
			}
			std::cout << "Sent data to: " << ConnectedUsers[i].UserName << std::endl;
		}

		MessagesToSend.pop();
	}
}

void SendToSpecific() {

	if (MessagesToSend.size() > 0) {
		OutboundMessagePacket MessagePacket = MessagesToSend.front();
		std::string Message = MessagePacket.Message;

		char* message = (char*)Message.c_str();

		for (int i = 0; i < MessagePacket.AddressesToSendTo.size(); i++) {
			//sockaddr_in tempaddr = MessagePacket.AddressesToSendTo[i];

			if (sendto(server_socket, message, BUF_LEN, 0,
				(struct sockaddr*) & MessagePacket.AddressesToSendTo[i], BUF_LEN) == SOCKET_ERROR) {
				printf("sendto() failed %d\n", WSAGetLastError());
			}

			std::cout << "Sent data to client: " << MessagePacket.FromWho << std::endl;
		}

		MessagesToSend.pop();
	}
}

void SendProcess() {
	while (ServerRunning == true) {
		if (MessagesToSend.size() > 0) {
			OutboundMessagePacket CurrentPacket = MessagesToSend.front();
			if (CurrentPacket.ToWho == "ALL") {
				SendToAll();
			}
			else if (CurrentPacket.ToWho == "SPECIFIC") {
				SendToSpecific();
			}
		}
	}
}
void RecProcess() {
	while (ServerRunning == true) {
		memset(recv_buf, 0, BUF_LEN);
		if (recvfrom(server_socket, recv_buf, sizeof(recv_buf), 0, (struct sockaddr*) & fromAddr, &fromlen) == SOCKET_ERROR) {
			printf("recvfrom() failed...%d\n", WSAGetLastError());
		}

		char ipbuf[INET_ADDRSTRLEN];
		inet_ntop(AF_INET, &fromAddr, ipbuf, sizeof(ipbuf));

		std::string SendersIP = ipbuf;

		InboundMessagePacket TempPacket;
		TempPacket.SenderIP = SendersIP;
		TempPacket.Message = recv_buf;
		TempPacket.Address = fromAddr;
		MessagesReceived.push(TempPacket);
	}
}

void ProcessMsg() {
	if (MessagesReceived.size() > 0) {
		InboundMessagePacket Current = MessagesReceived.front();
		std::string RecMsg = Current.Message;
		std::string SendersIP = Current.SenderIP;

		if (RecMsg.substr(0, 4) == "JOIN") {

			OutboundMessagePacket JoinMsg;
			JoinMsg.ToWho = "SPECIFIC";
			UserData Player;
			Player.IP = SendersIP;
			Player.Address = fromAddr;
			if (ConnectedUsers.size() <= 0) {
				Player.UserName = "PLAYER";
				ConnectedUsers.push_back(Player);
				JoinMsg.AddressesToSendTo.push_back(Player.Address);
			}
			else if (ConnectedUsers.size() > 0) {
				int Similarities = 0;
				for (int i = 0; i < ConnectedUsers.size(); i++) {
					if (SendersIP == ConnectedUsers[i].IP) {
						Similarities++;
					}
				}

				if (Similarities == 0) {
					Player.UserName = "GHOST";
					ConnectedUsers.push_back(Player);
					JoinMsg.AddressesToSendTo.push_back(Player.Address);
				}
			}

			if (JoinMsg.AddressesToSendTo.size() > 0) {
				JoinMsg.Message = "CPP";
				JoinMsg.Message += "\n";
				JoinMsg.Message += "ACCEPT";
				JoinMsg.Message += "\n";
				JoinMsg.Message += Player.UserName;

				MessagesToSend.push(JoinMsg);
				//std::cout << "a player joined the game" << std::endl;
			}

		}
		else if (RecMsg.substr(0, 5) == "READY") {


			std::istringstream iss(RecMsg);
			std::string Line;
			std::getline(iss, Line);
			std::getline(iss, Line);
			for (int i = 0; i < ConnectedUsers.size(); i++) {
				if (ConnectedUsers[i].UserName == Line) {
					ConnectedUsers[i].Status = "READY";
				}
			}

		}
		else if (RecMsg.substr(0, 7) == "GETSTAT") {
			std::cout << "Getting stats" << std::endl;
			OutboundMessagePacket Temppckt;
			Temppckt.ToWho = "SPECIFIC";
			Temppckt.Message = "CS";
			Temppckt.Message += "\n";
			Temppckt.Message += "SETSTAT";
			Temppckt.Message += "\n";


			std::string GhostStatus = "Not Joined";
			std::string PlayerStatus = "Not joined";
			for (int i = 0; i < ConnectedUsers.size(); i++) {

				if (ConnectedUsers[i].UserName == "GHOST") {
					GhostStatus = ConnectedUsers[i].Status;
				}
				if (ConnectedUsers[i].UserName == "PLAYER") {
					PlayerStatus = ConnectedUsers[i].Status;
				}


				if (ConnectedUsers[i].IP == SendersIP) {
					Temppckt.AddressesToSendTo.push_back(ConnectedUsers[i].Address);
				}
			}




			Temppckt.Message += PlayerStatus;
			Temppckt.Message += "\n";
			Temppckt.Message += GhostStatus;

			MessagesToSend.push(Temppckt);

		}
		else if (RecMsg.substr(0, 8) == "ADDSCORE") {
			std::istringstream iss(RecMsg);
			std::string Line;
			std::getline(iss, Line);
			std::getline(iss, Line);
			StoreScore(Line);
			SortScores();

		}
		else if (RecMsg.substr(0, 7) == "UPDDOOR" || RecMsg.substr(0, 8) == "UPDRAWER" || RecMsg.substr(0, 4) == "UPDP" || RecMsg.substr(0, 4) == "UPDG") {
			OutboundMessagePacket Temppckt;
			Temppckt.ToWho = "SPECIFIC";
			Temppckt.Message = "CS";
			Temppckt.Message += "\n";
			Temppckt.Message += RecMsg;



			for (int i = 0; i < ConnectedUsers.size(); i++) {
				if (ConnectedUsers[i].IP != SendersIP) {
					Temppckt.AddressesToSendTo.push_back(ConnectedUsers[i].Address);
				}

				if (ConnectedUsers[i].IP == SendersIP) {
					Temppckt.FromWho = ConnectedUsers[i].UserName;
				}
			}
			MessagesToSend.push(Temppckt);
		}
		else if (RecMsg.substr(0, 4) == "UPDT") {
			OutboundMessagePacket Temppckt;
			Temppckt.ToWho = "ALL";
			Temppckt.Message = "CS";
			Temppckt.Message += "\n";
			Temppckt.Message += RecMsg;

			MessagesToSend.push(Temppckt);
		}
		else if (RecMsg.substr(0, 5) == "ACTT1") {
			OutboundMessagePacket Temppckt;
			Temppckt.ToWho = "ALL";
			Temppckt.Message = "CS";
			Temppckt.Message += "\n";
			Temppckt.Message += RecMsg;

			MessagesToSend.push(Temppckt);
		}

		MessagesReceived.pop();

	}
}

void ServerProc() {
	while (ServerRunning == true) {
		ProcessMsg();

	}

}




int main() {


	//Initialize winsock
	WSADATA wsa;

	int error;
	error = WSAStartup(MAKEWORD(2, 2), &wsa);

	if (error != 0) {
		printf("Failed to initialize %d\n", error);
		return 1;
	}

	//Create a Server socket

	struct addrinfo* ptr = NULL, hints;

	memset(&hints, 0, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_DGRAM;
	hints.ai_protocol = IPPROTO_UDP;
	hints.ai_flags = AI_PASSIVE;

	if (getaddrinfo("localhost"/*NULL*/, "8888", &hints, &ptr) != 0) {
		printf("Getaddrinfo failed!! %d\n", WSAGetLastError());
		WSACleanup();
		return 1;
	}



	server_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

	if (server_socket == INVALID_SOCKET) {
		printf("Failed creating a socket %d\n", WSAGetLastError());
		WSACleanup();
		return 1;
	}

	// Bind socket

	if (bind(server_socket, ptr->ai_addr, (int)ptr->ai_addrlen) == SOCKET_ERROR) {
		printf("Bind failed: %d\n", WSAGetLastError());
		closesocket(server_socket);
		freeaddrinfo(ptr);
		WSACleanup();
		return 1;
	}



	// Receive msg from client

	std::thread ServerProcess{ ServerProc };
	std::thread SendProc{ SendProcess };
	std::thread RecProc{ RecProcess };
	


	ServerProcess.join();
	SendProc.join();
	RecProc.join();

	// Struct that will hold the IP address of the client that sent the message (we don't have accept() anymore to learn the address)




	closesocket(server_socket);
	freeaddrinfo(ptr);
	WSACleanup();

	return 0;
}