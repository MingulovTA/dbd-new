using System;
using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample 
{
	[RoomType("UnityMushrooms")]
	public class GameCode : Game<Player>
	{
		public override void GameStarted() 
		{
			Console.WriteLine("GameStarted: " + RoomId);
		}
		
		public override void GameClosed() 
		{
			Console.WriteLine("RoomId: " + RoomId);
		}

		public override void UserJoined(Player player) 
		{
			Console.WriteLine("UserJoined: " + player.Name);
			Broadcast("UserJoined", player.ConnectUserId);
		}

		public override void UserLeft(Player player) 
		{
			Console.WriteLine("UserLeft: " + player.Name);
			Broadcast("UserLeft", player.ConnectUserId);
		}

		public override void GotMessage(Player player, Message message) 
		{
			Console.WriteLine($"Cmd of {player.Name}: {message.Type}");
			Broadcast(message);
		}
	}
}