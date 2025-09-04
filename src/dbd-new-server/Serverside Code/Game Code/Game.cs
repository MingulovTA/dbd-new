using System;
using System.Collections.Generic;
using System.Linq;
using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample 
{


	[RoomType("UnityMushrooms")]
	public class GameCode : Game<Player> 
	{
		public override void GameStarted() 
		{
			Console.WriteLine("Game is started: " + RoomId);
			RestartGame();
		}

		private void RestartGame() 
		{
			foreach(Player pl in Players) 
			{
				pl.Score = 0;
				pl.TeamId = 0;
			}
			Broadcast("SvRestartGame");
			AddTimer(KillFirst, 5000);
		}

		private void KillFirst()
		{
			Random random = new Random();
			int randomIndex = random.Next(0, PlayerCount-1);
			var firstKiller = Players.ToList()[randomIndex];
			firstKiller.TeamId = 1;
			Broadcast("SvKill", "Server", firstKiller.ConnectUserId);
		}


		public override void GameClosed() 
		{
			Console.WriteLine("RoomId: " + RoomId);
		}

		public override void UserJoined(Player player) 
		{
			Broadcast("SvUserJoined",player.ConnectUserId);
		}

		public override void UserLeft(Player player) 
		{
			Broadcast("SvUserLeft", player.ConnectUserId);
		}

		public override void GotMessage(Player player, Message message) 
		{
			switch(message.Type) 
			{
				case "ClMove":
					player.PosX = message.GetFloat(0);
					player.PosY = message.GetFloat(1);
					player.PosZ = message.GetFloat(2);
					Broadcast("SvMove", player.ConnectUserId, player.PosX, player.PosY, player.PosZ);
					break;
				case "ClKill":
					var targetId = message.GetString(0);
					var targetPlayer = Players.FirstOrDefault(p => p.ConnectUserId == targetId);

					if (targetPlayer != null && player.TeamId == 1 && targetPlayer.TeamId == 0)
					{
						targetPlayer.TeamId = 1;
						Broadcast("SvKill", player.ConnectUserId, targetId);
						player.Score++;
						Broadcast("SvScoreChanged", player.ConnectUserId, player.Score);
						CheckForEndOfRound();
					}
					break;
				case "ClKilledBy":
					var killerID = message.GetString(0);
					var killerPlayer = Players.FirstOrDefault(p => p.ConnectUserId == killerID);

					if (killerPlayer != null && player.TeamId == 0 && killerPlayer.TeamId == 1)
					{
						player.TeamId = 1;
						Broadcast("SvKill", killerID, player.ConnectUserId);
						killerPlayer.Score++;
						Broadcast("SvScoreChanged", killerID, killerPlayer.Score);
						CheckForEndOfRound();
					}
					break;

				case "Chat":
					Console.WriteLine("player.ConnectUserId: {0}", message.GetString(0));
					foreach(Player pl in Players) {
						if(pl.ConnectUserId != player.ConnectUserId) {
							pl.Send("Chat", player.ConnectUserId, message.GetString(0));
						}
					}
					break;
			}
		}

		private void CheckForEndOfRound()
		{
			if (Players.All(p=>p.TeamId==1))
				AddTimer(RestartGame, 5000);
		}
	}
}